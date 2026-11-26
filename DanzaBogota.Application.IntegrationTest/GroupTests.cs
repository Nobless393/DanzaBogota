using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DanzaBogota.Application.IntegrationTest
{
    [TestClass]
    public class GroupTests
    {
        private static TestWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;
        private static string _adminToken = null!;
        private static string _grupoToken = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _factory = new TestWebApplicationFactory();
            await _factory.InitializeDatabaseAsync();
            _client = _factory.CreateClient();

            _adminToken = await LoginAsync("Alexc312@hotmail.com", "Admin!123456");
            _grupoToken = await LoginAsync("grupo@danzabogota.com", "Grupo!123456");
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

        // ── LISTAR GRUPOS ──────────────────────────────────────────────────

        [TestMethod]
        [Description("Listar todos los grupos requiere rol Admin.")]
        public async Task GetAll_SinToken_Retorna401()
        {
            var response = await _client.GetAsync("/api/Group");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        [Description("Listar todos los grupos con rol Admin retorna lista.")]
        public async Task GetAll_ConRolAdmin_Retorna200()
        {
            var req = AuthRequest(HttpMethod.Get, "/api/Group", _adminToken);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
        }

        [TestMethod]
        [Description("Listar mis grupos con rol Grupo retorna 200.")]
        public async Task GetMyGroups_ConRolGrupo_Retorna200()
        {
            var req = AuthRequest(HttpMethod.Get, "/api/Group/my-groups", _grupoToken);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
        }

        // ── CREAR GRUPO ────────────────────────────────────────────────────

        [TestMethod]
        [Description("Crear grupo con rol Grupo retorna el ID del nuevo grupo.")]
        public async Task Create_ConRolGrupo_RetornaId()
        {
            var payload = new
            {
                name = "Grupo de Prueba Test",
                description = "Descripción de prueba",
                directorName = "Director Test",
                phone = "3001234567",
                email = "grupo.test@ejemplo.com",
                socialMediaUrl = (string?)null,
                localityId = 1,
                categoryId = 1,
                useDirectorRut = false
            };

            var req = AuthRequest(HttpMethod.Post, "/api/Group", _grupoToken);
            req.Content = JsonContent.Create(payload);
            var response = await _client.SendAsync(req);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());
            Assert.IsTrue(body.GetProperty("data").GetInt32() > 0);
        }

        [TestMethod]
        [Description("Crear grupo sin autenticación debe retornar 401.")]
        public async Task Create_SinToken_Retorna401()
        {
            var payload = new
            {
                name = "Grupo sin auth",
                directorName = "Director",
                localityId = 1,
                categoryId = 1,
                useDirectorRut = false
            };

            var response = await _client.PostAsJsonAsync("/api/Group", payload);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── VER GRUPO POR ID ───────────────────────────────────────────────

        [TestMethod]
        [Description("Ver un grupo por ID es público — retorna 200 si existe.")]
        public async Task GetById_GrupoExistente_Retorna200()
        {
            // Primero crear uno para tener un ID válido
            var payload = new
            {
                name = "Grupo Para Consultar",
                description = "Test",
                directorName = "Director Consulta",
                phone = (string?)null,
                email = (string?)null,
                socialMediaUrl = (string?)null,
                localityId = 1,
                categoryId = 1,
                useDirectorRut = true
            };

            var createReq = AuthRequest(HttpMethod.Post, "/api/Group", _grupoToken);
            createReq.Content = JsonContent.Create(payload);
            var createResp = await _client.SendAsync(createReq);
            var createBody = await createResp.Content.ReadFromJsonAsync<JsonElement>();
            var groupId = createBody.GetProperty("data").GetInt32();

            // Consultar sin token (público)
            var response = await _client.GetAsync($"/api/Group/{groupId}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        [Description("Ver grupo con ID que no existe retorna 404.")]
        public async Task GetById_GrupoInexistente_Retorna404()
        {
            var response = await _client.GetAsync("/api/Group/999999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
