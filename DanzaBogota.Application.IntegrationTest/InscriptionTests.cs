using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DanzaBogota.Application.IntegrationTest
{
    [TestClass]
    public class InscriptionTests
    {
        private static TestWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;
        private static string _adminToken = null!;
        private static string _gestorToken = null!;
        private static string _grupoToken = null!;
        private static int _eventId;
        private static int _groupId;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _factory = new TestWebApplicationFactory();
            await _factory.InitializeDatabaseAsync();
            _client = _factory.CreateClient();

            _adminToken  = await LoginAsync("Alexc312@hotmail.com", "Admin!123456");
            _gestorToken = await LoginAsync("gestor@danzabogota.com", "Gestor!123456");
            _grupoToken  = await LoginAsync("grupo@danzabogota.com", "Grupo!123456");

            // Crear un evento y un grupo que se usarán en los tests
            _eventId = await CreateTestEventAsync();
            _groupId = await CreateTestGroupAsync();

            // Publicar el evento para que sea inscribible
            await PublishEventAsync(_eventId);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        // ── HELPERS ────────────────────────────────────────────────────────

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

        private static async Task<int> CreateTestEventAsync()
        {
            var payload = new
            {
                title = "Evento para Inscripción",
                description = "Evento de prueba para tests de inscripción.",
                imageUrl = (string?)null,
                startDate = DateTimeOffset.UtcNow.AddDays(20).ToString("o"),
                endDate = DateTimeOffset.UtcNow.AddDays(21).ToString("o"),
                locationName = "Auditorio Central",
                address = "Carrera 5 # 10-20",
                localityId = 1,
                maxParticipants = 200,
                maxParticipantsPerGroup = 20,
                eventTypeIds = new[] { 1 },
                categoryIds = new[] { 1 }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "/api/Event");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _gestorToken);
            req.Content = JsonContent.Create(payload);
            var resp = await _client.SendAsync(req);
            var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("data").GetInt32();
        }

        private static async Task<int> CreateTestGroupAsync()
        {
            var payload = new
            {
                name = "Grupo Inscripción Test",
                description = "Grupo para pruebas de inscripción",
                directorName = "Director Inscripción",
                phone = "3109876543",
                email = "inscripcion@test.com",
                socialMediaUrl = (string?)null,
                localityId = 1,
                categoryId = 1,
                useDirectorRut = true
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "/api/Group");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _grupoToken);
            req.Content = JsonContent.Create(payload);
            var resp = await _client.SendAsync(req);
            var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("data").GetInt32();
        }

        private static async Task PublishEventAsync(int eventId)
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, $"/api/Event/{eventId}/publish");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
            await _client.SendAsync(req);
        }

        // ── INSCRIBIRSE ────────────────────────────────────────────────────

        [TestMethod]
        [Description("Crear inscripción con rol Grupo retorna el ID de la inscripción.")]
        public async Task Create_ConRolGrupo_RetornaId()
        {
            var payload = new
            {
                eventId = _eventId,
                groupId = _groupId,
                notes = "Notas de prueba para la inscripción"
            };

            var req = AuthRequest(HttpMethod.Post, "/api/Inscription", _grupoToken);
            req.Content = JsonContent.Create(payload);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
            Assert.IsTrue(body.GetProperty("data").GetInt32() > 0);
        }

        [TestMethod]
        [Description("Crear inscripción sin autenticación debe retornar 401.")]
        public async Task Create_SinToken_Retorna401()
        {
            var payload = new
            {
                eventId = _eventId,
                groupId = _groupId,
                notes = (string?)null
            };

            var response = await _client.PostAsJsonAsync("/api/Inscription", payload);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── MIS INSCRIPCIONES ──────────────────────────────────────────────

        [TestMethod]
        [Description("Listar mis inscripciones con rol Grupo retorna lista.")]
        public async Task GetMyInscriptions_ConRolGrupo_Retorna200()
        {
            var req = AuthRequest(HttpMethod.Get, "/api/Inscription/my-inscriptions", _grupoToken);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
        }

        [TestMethod]
        [Description("Listar mis inscripciones sin token retorna 401.")]
        public async Task GetMyInscriptions_SinToken_Retorna401()
        {
            var response = await _client.GetAsync("/api/Inscription/my-inscriptions");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── INSCRIPCIONES DE UN EVENTO ─────────────────────────────────────

        [TestMethod]
        [Description("Admin puede ver inscripciones de un evento.")]
        public async Task GetByEvent_ConRolAdmin_Retorna200()
        {
            var req = AuthRequest(HttpMethod.Get, $"/api/Inscription/event/{_eventId}", _adminToken);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
        }

        [TestMethod]
        [Description("Rol Grupo no puede ver inscripciones de un evento.")]
        public async Task GetByEvent_ConRolGrupo_Retorna403()
        {
            var req = AuthRequest(HttpMethod.Get, $"/api/Inscription/event/{_eventId}", _grupoToken);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ── ESTADÍSTICAS ───────────────────────────────────────────────────

        [TestMethod]
        [Description("Admin puede ver estadísticas de inscripciones.")]
        public async Task GetStats_ConRolAdmin_Retorna200()
        {
            var req = AuthRequest(HttpMethod.Get, "/api/Inscription/stats", _adminToken);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
        }

        [TestMethod]
        [Description("Estadísticas sin token retorna 401.")]
        public async Task GetStats_SinToken_Retorna401()
        {
            var response = await _client.GetAsync("/api/Inscription/stats");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
