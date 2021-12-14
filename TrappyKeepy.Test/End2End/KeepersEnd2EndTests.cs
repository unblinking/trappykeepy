using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Test.TestObjects;
using Xunit;

namespace TrappyKeepy.Test.End2End
{
    [Collection("Sequential")]
    public class KeepersEnd2EndTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private SpawnyDb _db;
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private DtoTestObjects _dto;
        private JsonSerializerOptions _jsonOpts;


        public KeepersEnd2EndTests(WebApplicationFactory<Program> webApplicationFactory)
        {
            _db = new SpawnyDb();
            _webApplicationFactory = webApplicationFactory;
            _dto = new DtoTestObjects();
            _jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region CREATE KEEPER

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostKeepersAsAdminRoleWithNewKeeperShouldCreateNewKeeperReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();

            var filename = "TrappyKeepy.Test.pdb";
            FileStream fs = File.OpenRead(filename);

            HttpContent fileStreamContent = new StreamContent(fs);
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            HttpContent filenameContent = new StringContent(filename);

            MultipartFormDataContent formdata = new MultipartFormDataContent();
            formdata.Add(fileStreamContent, "file", filename);
            formdata.Add(filenameContent, "filename");

            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                var token = await _db.AuthenticateAdmin(client);
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/keepers", formdata);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            //Assert.Equal("OK", response.ReasonPhrase.ToString());
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("success", controllerResponse.Status);
            Assert.NotNull(controllerResponse.Data);
            var dataString = controllerResponse.Data.ToString();
            Assert.NotNull(dataString);
            var newKeeper = JsonSerializer.Deserialize<KeeperDto>(dataString, _jsonOpts);
            Assert.NotNull(newKeeper);
            Assert.NotNull(newKeeper.Id);
        }

        #endregion CREATE KEEPER
    }
}
