using System;
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
    public class UsersEnd2EndTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private SpawnyDb _db;
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private DtoTestObjects _dto;
        private JsonSerializerOptions _jsonOpts;


        public UsersEnd2EndTests(WebApplicationFactory<Program> webApplicationFactory)
        {
            _db = new SpawnyDb();
            _webApplicationFactory = webApplicationFactory;
            _dto = new DtoTestObjects();
            _jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region CREATE USER

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsAdminRoleWithNewBasicUserDtoShouldCreateNewUserReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _dto.TestUserNewBasicDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("OK", response.ReasonPhrase.ToString());
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("success", controllerResponse.Status);
            Assert.NotNull(controllerResponse.Data);
            var dataString = controllerResponse.Data.ToString();
            Assert.NotNull(dataString);
            var newUser = JsonSerializer.Deserialize<UserDto>(dataString, _jsonOpts);
            Assert.NotNull(newUser);
            Assert.NotNull(newUser.Id);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsManagerRoleWithNewBasicUserDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            var user = _dto.TestUserNewBasicDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsBasicRoleWithNewBasicUserDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            var user = _dto.TestUserNewBasicDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsAdminRoleWithIncompleteUserDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _dto.TestUserIncompleteDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("fail", controllerResponse.Status);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsAdminRoleWithExistingBasicUserDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _dto.TestUserBasicDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("fail", controllerResponse.Status);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsAdminRoleWithEmptyStringsUserDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _dto.TestUserEmptyStringsDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("fail", controllerResponse.Status);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersAsAdminRoleWithoutUserDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var json = JsonSerializer.Serialize("");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync("/v1/users", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        #endregion CREATE USER

        #region CREATE USER MEMBERSHIP

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsAdminRoleWithNewMembershipDtoShouldCreateNewMembershipReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var group = _db.GetGroupLaughingstocks();
            var user = _db.GetUserBasic();
            var membership = new MembershipDto()
            {
                GroupId = group.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(membership);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("OK", response.ReasonPhrase.ToString());
            var responseJson = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseJson);
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, _jsonOpts);
            Assert.NotNull(controllerResponse);
            Assert.NotNull(controllerResponse.Status);
            Assert.Equal("success", controllerResponse.Status);
            Assert.NotNull(controllerResponse.Data);
            var dataString = controllerResponse.Data.ToString();
            Assert.NotNull(dataString);
            var newMembership = JsonSerializer.Deserialize<MembershipDto>(dataString, _jsonOpts);
            Assert.NotNull(newMembership);
            Assert.NotNull(newMembership.Id);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsManagerRoleWithNewMembershipDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            var group = _db.GetGroupLaughingstocks();
            var user = _db.GetUserBasic();
            var membership = new MembershipDto()
            {
                GroupId = group.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(membership);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsBasicRoleWithNewMembershipDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            var group = _db.GetGroupLaughingstocks();
            var user = _db.GetUserBasic();
            var membership = new MembershipDto()
            {
                GroupId = group.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(membership);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsAdminRoleWithIncompleteNewMembershipDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var group = _db.GetGroupLaughingstocks();
            var user = _db.GetUserBasic();
            var membership = new MembershipDto()
            {
                GroupId = group.Id
            };
            var json = JsonSerializer.Serialize(membership);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsAdminRoleWithExistingMembershipDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var group = _db.GetGroupAdmins();
            var user = _db.GetUserAdmin();
            var membership = new MembershipDto()
            {
                GroupId = group.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(membership);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsAdminRoleWithEmptyGuidMembershipDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var membership = new MembershipDto()
            {
                GroupId = Guid.Empty,
                UserId = Guid.Empty
            };
            var json = JsonSerializer.Serialize(membership);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{Guid.Empty}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdMembershipsAsAdminRoleWithoutMembershipDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var json = JsonSerializer.Serialize("");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{Guid.Empty}/memberships", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        #endregion CREATE USER MEMBERSHIP



    }
}
