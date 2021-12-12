using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Test.TestObjects;
using Xunit;

namespace TrappyKeepy.Test.End2End
{
    public class SessionsEnd2EndTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private SpawnyDb _db; 
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private DtoTestObjects _dto;
        private JsonSerializerOptions _jsonOpts;

        public SessionsEnd2EndTests(WebApplicationFactory<Program> webApplicationFactory)
        {
            _db = new SpawnyDb();
            _webApplicationFactory = webApplicationFactory;
            _dto = new DtoTestObjects();
            _jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostSessionsWithValidCredentialsShouldCreateNewToken()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var user = _dto.TestUserSessionDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                response = await client.PostAsync("/v1/sessions", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response);
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("success", controllerResponse.Status);
            Assert.NotNull(controllerResponse.Data);
            var token = controllerResponse.Data.ToString();
            Assert.NotNull(token);
        }
    }
}
