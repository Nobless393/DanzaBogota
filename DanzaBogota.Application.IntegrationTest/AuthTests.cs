using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DanzaBogota.Application.IntegrationTest
{
    [TestClass]
    public class AuthTests
    {
        private static TestWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _factory = new TestWebApplicationFactory();
            await _factory.InitializeDatabaseAsync();
            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        // ── LOGIN ──────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Login con credenciales correctas del admin debe retornar token.")]
        public async Task Login_ConCredencialesValidas_RetornaToken()
        {
            var payload = new { email = "Alexc312@hotmail.com", password = "Admin!123456" };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", payload);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());

            var data = body.GetProperty("data");
            Assert.IsFalse(string.IsNullOrWhiteSpace(data.GetProperty("token").GetString()));
            Assert.IsTrue(data.GetProperty("roles").EnumerateArray()
                .Any(r => r.GetString() == "Admin"));
        }

        [TestMethod]
        [Description("Login con contraseña incorrecta debe retornar 401.")]
        public async Task Login_ConPasswordIncorrecta_Retorna401()
        {
            var payload = new { email = "Alexc312@hotmail.com", password = "MalPassword" };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", payload);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsFalse(body.GetProperty("success").GetBoolean());
        }

        [TestMethod]
        [Description("Login con correo que no existe debe retornar 401.")]
        public async Task Login_ConCorreoInexistente_Retorna401()
        {
            var payload = new { email = "noexiste@test.com", password = "Admin!123456" };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", payload);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── REGISTER ───────────────────────────────────────────────────────

        [TestMethod]
        [Description("Registro de nuevo usuario con datos válidos debe retornar token.")]
        public async Task Register_ConDatosValidos_RetornaToken()
        {
            var payload = new
            {
                firstName = "Laura",
                lastName = "Pérez",
                email = "laura.perez.test@ejemplo.com",
                password = "Test!12345",
                role = "Grupo"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/register", payload);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsTrue(body.GetProperty("success").GetBoolean());

            var data = body.GetProperty("data");
            Assert.IsFalse(string.IsNullOrWhiteSpace(data.GetProperty("token").GetString()));
        }

        [TestMethod]
        [Description("Registro con correo ya existente debe retornar 400.")]
        public async Task Register_ConCorreoDuplicado_Retorna400()
        {
            var payload = new
            {
                firstName = "Otro",
                lastName = "Usuario",
                email = "Alexc312@hotmail.com", // ya existe
                password = "Test!12345",
                role = "Grupo"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/register", payload);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.IsFalse(body.GetProperty("success").GetBoolean());
        }

        [TestMethod]
        [Description("Registro con contraseña débil debe retornar 400.")]
        public async Task Register_ConPasswordDebil_Retorna400()
        {
            var payload = new
            {
                firstName = "Ana",
                lastName = "García",
                email = "ana.test.weak@ejemplo.com",
                password = "123",  // demasiado corta
                role = "Grupo"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/register", payload);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
