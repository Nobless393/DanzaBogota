using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DanzaBogota.Application.IntegrationTest
{
    [TestClass]
    public class EventTests
    {
        private static TestWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;
        private static string _adminToken = null!;
        private static string _gestorToken = null!;
        private static string _grupoToken = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _factory = new TestWebApplicationFactory();
            await _factory.InitializeDatabaseAsync();
            _client = _factory.CreateClient();

            _adminToken  = await LoginAsync("Alexc312@hotmail.com", "Admin!123456");
            _gestorToken = await LoginAsync("gestor@danzabogota.com", "Gestor!123456");
            _grupoToken  = await LoginAsync("grupo@danzabogota.com", "Grupo!123456");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private static async Task<string> LoginAsync(string email, string password)
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/login",
                new { email, password });
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("data").GetProperty("token").GetString()!;
        }

        private HttpRequestMessage AuthRequest(HttpMethod method, string url, string token)
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return req;
        }

        // ── EVENTOS PÚBLICOS ───────────────────────────────────────────────

        [TestMethod]
        [Description("Listar eventos publicados no requiere autenticación.")]
        public async Task GetPublished_SinToken_Retorna200()
        {
            var response = await _client.GetAsync("/api/Event/published?page=1&pageSize=10");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());

            var data = body.GetProperty("data");
            Assert.IsTrue(data.TryGetProperty("items", out _));
        }

        [TestMethod]
        [Description("Contador de eventos finalizados es público.")]
        public async Task GetFinishedCount_SinToken_Retorna200()
        {
            var response = await _client.GetAsync("/api/Event/finished-count");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
        }

        // ── ACCESO CON ROL ─────────────────────────────────────────────────

        [TestMethod]
        [Description("Listar todos los eventos requiere rol Admin o Gestor.")]
        public async Task GetAll_SinToken_Retorna401()
        {
            var response = await _client.GetAsync("/api/Event?page=1&pageSize=10");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        [Description("Listar todos los eventos con rol Grupo debe retornar 403.")]
        public async Task GetAll_ConRolGrupo_Retorna403()
        {
            var req = AuthRequest(HttpMethod.Get, "/api/Event?page=1&pageSize=10", _grupoToken);
            var response = await _client.SendAsync(req);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        [Description("Listar todos los eventos con rol Admin debe retornar 200.")]
        public async Task GetAll_ConRolAdmin_Retorna200()
        {
            var req = AuthRequest(HttpMethod.Get, "/api/Event?page=1&pageSize=10", _adminToken);
            var response = await _client.SendAsync(req);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // ── CREAR EVENTO ───────────────────────────────────────────────────

        [TestMethod]
        [Description("Crear evento con rol Gestor debe retornar el ID del nuevo evento.")]
        public async Task Create_ConRolGestor_RetornaId()
        {
            var payload = new
            {
                title = "Festival de Danza Test",
                description = "Descripción del festival de prueba.",
                imageUrl = (string?)null,
                startDate = DateTimeOffset.UtcNow.AddDays(10).ToString("o"),
                endDate = DateTimeOffset.UtcNow.AddDays(11).ToString("o"),
                locationName = "Teatro Municipal",
                address = "Calle 10 # 5-20, Bogotá",
                localityId = 1,
                maxParticipants = 100,
                maxParticipantsPerGroup = 10,
                eventTypeIds = new[] { 1 },
                categoryIds = new[] { 1 }
            };

            var req = AuthRequest(HttpMethod.Post, "/api/Event", _gestorToken);
            req.Content = JsonContent.Create(payload);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
            Assert.IsTrue(body.GetProperty("data").GetInt32() > 0);
        }

        [TestMethod]
        [Description("Crear evento sin autenticación debe retornar 401.")]
        public async Task Create_SinToken_Retorna401()
        {
            var payload = new
            {
                title = "Evento sin auth",
                description = "Test",
                startDate = DateTimeOffset.UtcNow.AddDays(5).ToString("o"),
                endDate = DateTimeOffset.UtcNow.AddDays(6).ToString("o"),
                locationName = "Lugar",
                address = "Dirección",
                localityId = 1,
                maxParticipants = 50,
                maxParticipantsPerGroup = 5,
                eventTypeIds = new[] { 1 },
                categoryIds = new[] { 1 }
            };

            var response = await _client.PostAsJsonAsync("/api/Event", payload);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── PUBLICAR / CANCELAR ────────────────────────────────────────────

        [TestMethod]
        [Description("Publicar un evento existente con rol Admin debe retornar 200.")]
        public async Task PublishAndCancel_ConRolAdmin_Retorna200()
        {
            // Crear primero
            var payload = new
            {
                title = "Evento para publicar",
                description = "Test publicar.",
                imageUrl = (string?)null,
                startDate = DateTimeOffset.UtcNow.AddDays(15).ToString("o"),
                endDate = DateTimeOffset.UtcNow.AddDays(16).ToString("o"),
                locationName = "Auditorio",
                address = "Cra 7 # 32-16",
                localityId = 1,
                maxParticipants = 80,
                maxParticipantsPerGroup = 8,
                eventTypeIds = new[] { 1 },
                categoryIds = new[] { 1 }
            };

            var createReq = AuthRequest(HttpMethod.Post, "/api/Event", _adminToken);
            createReq.Content = JsonContent.Create(payload);
            var createResp = await _client.SendAsync(createReq);
            var createBody = await createResp.Content.ReadFromJsonAsync<JsonElement>();
            var eventId = createBody.GetProperty("data").GetInt32();

            // Publicar
            var publishReq = AuthRequest(HttpMethod.Patch, $"/api/Event/{eventId}/publish", _adminToken);
            var publishResp = await _client.SendAsync(publishReq);
            Assert.AreEqual(HttpStatusCode.OK, publishResp.StatusCode);

            // Cancelar
            var cancelReq = AuthRequest(HttpMethod.Patch, $"/api/Event/{eventId}/cancel", _adminToken);
            var cancelResp = await _client.SendAsync(cancelReq);
            Assert.AreEqual(HttpStatusCode.OK, cancelResp.StatusCode);
        }
    }
}
