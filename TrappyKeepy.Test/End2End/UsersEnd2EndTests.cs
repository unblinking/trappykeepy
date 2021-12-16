using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
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

        #region CREATE USER PERMIT

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsAdminRoleWithNewPermitDtoShouldCreateNewPermitReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var keeper = _db.GetKeeperApiDll();
            var user = _db.GetUserBasic();
            var permit = new PermitDto()
            {
                KeeperId = keeper.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(permit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/permits", content);
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
            var newPermit = JsonSerializer.Deserialize<PermitDto>(dataString, _jsonOpts);
            Assert.NotNull(newPermit);
            Assert.NotNull(newPermit.Id);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsManagerRoleWithNewPermitDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            var keeper = _db.GetKeeperApiDll();
            var user = _db.GetUserBasic();
            var permit = new PermitDto()
            {
                KeeperId = keeper.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(permit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/permits", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsBasicRoleWithNewPermitDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            var keeper = _db.GetKeeperApiDll();
            var user = _db.GetUserBasic();
            var permit = new PermitDto()
            {
                KeeperId = keeper.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(permit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/permits", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsAdminRoleWithIncompletePermitDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var keeper = _db.GetKeeperApiDll();
            var user = _db.GetUserBasic();
            var permit = new PermitDto()
            {
                KeeperId = keeper.Id
            };
            var json = JsonSerializer.Serialize(permit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/permits", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsAdminRoleWithExistingPermitDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var keeper = _db.GetKeeperApiDll();
            var user = _db.GetUserAdmin();
            var permit = new PermitDto()
            {
                KeeperId = keeper.Id,
                UserId = user.Id
            };
            var json = JsonSerializer.Serialize(permit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{user.Id}/permits", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsAdminRoleWithEmptyGuidPermitDtoShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var permit = new PermitDto()
            {
                KeeperId = Guid.Empty,
                UserId = Guid.Empty
            };
            var json = JsonSerializer.Serialize(permit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync($"/v1/users/{Guid.Empty}/permits", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PostUsersIdPermitsAsAdminRoleWithoutPermitDtoShouldReturn400BadRequest()
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
                response = await client.PostAsync($"/v1/users/{Guid.Empty}/permits", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        #endregion CREATE USER PERMIT

        #region READ ALL USERS

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUsersAsAdminRoleShouldReturnUsersList200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync("/v1/users");
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
            var users = JsonSerializer.Deserialize<List<UserDto>>(dataString, _jsonOpts);
            Assert.NotNull(users);
            Assert.True(users.Count > 0);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUsersAsManagerRoleShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync("/v1/users");
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUsersAsBasicRoleShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync("/v1/users");
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        #endregion READ ALL USERS

        #region READ USER BY ID

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUserByIdAsAdminRoleWithValidIdShouldReturnUser200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _db.GetUserBasic();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync($"/v1/users/{user.Id}");
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
            var userReturned = JsonSerializer.Deserialize<UserComplexDto>(dataString, _jsonOpts);
            Assert.NotNull(userReturned);
            Assert.NotNull(userReturned.Memberships);
            Assert.NotNull(userReturned.Permits);
            Assert.True(userReturned.Memberships.Count > 0);
            Assert.True(userReturned.Permits.Count > 0);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUserByIdAsManagerRoleWithValidIdShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            var user = _db.GetUserBasic();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync($"/v1/users/{user.Id}");
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUserByIdAsBasicRoleWithValidIdShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            var user = _db.GetUserBasic();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync($"/v1/users/{user.Id}");
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUserByIdAsAdminRoleWithEmptyIdShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync($"/v1/users/{Guid.Empty}");
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task GetUserByIdAsAdminRoleWithNonexistingIdShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.GetAsync($"/v1/users/{Guid.NewGuid()}");
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        #endregion READ USER BY ID

        #region UPDATE USER

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserAsAdminRoleWithUpdatedUserDtoShouldUpdateExistingUserReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _db.GetUserBasic();
            user.Name = "Inigo Montoya";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}", content);
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
            Assert.Equal("User updated.", dataString);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserAsManagerRoleWithUpdatedUserDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            var user = _db.GetUserBasic();
            user.Name = "Inigo Montoya";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserAsBasicRoleWithUpdatedUserDtoShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            var user = _db.GetUserBasic();
            user.Name = "Inigo Montoya";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserAsAdminRoleWithUpdatedUserDtoWithUrlIdMistatchShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _db.GetUserBasic();
            user.Name = "Inigo Montoya";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{Guid.NewGuid()}", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserAsAdminRoleWithNewUserDtoShouldReturn400BadRequest()
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
                response = await client.PutAsync($"/v1/users/{user.Id}", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        #endregion UPDATE USER

        #region UPDATE USER PASSWORD

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserPasswordAsAdminRoleWithUpdatedUserPasswordShouldUpdateExistingUserPasswordReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _db.GetUserBasic();
            user.Password = "1.2.3.4.";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}/password", content);
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
            Assert.Equal("User password updated.", dataString);
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserPasswordAsManagerRoleWithUpdatedUserPasswordShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateManager();
            var user = _db.GetUserBasic();
            user.Password = "1.2.3.4.";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}/password", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserPasswordAsBasicRoleWithUpdatedUserPasswordShouldReturn403Forbidden()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateBasic();
            var user = _db.GetUserBasic();
            user.Password = "1.2.3.4.";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}/password", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Forbidden", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserPasswordAsAdminRoleWithUrlIdMismatchShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _db.GetUserBasic();
            user.Password = "1.2.3.4.";
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{Guid.NewGuid()}/password", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task PutUserPasswordAsAdminRoleWithoutNewPasswordShouldReturn400BadRequest()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var adminUser = _db.GetUserAdmin();
            var user = new UserDto()
            {
                Id = adminUser.Id
            };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.PutAsync($"/v1/users/{user.Id}/password", content);
            }

            // ---------- ASSERT ----------
            Assert.NotNull(response.ReasonPhrase);
            Assert.Equal("Bad Request", response.ReasonPhrase.ToString());
        }

        #endregion UPDATE USER PASSWORD

        #region DELETE USER

        [Fact]
        [Trait("TestType", "End2End")]
        public async Task DeleteUserAsAdminRoleWithExistingUserShouldDeleteUserReturn200Ok()
        {
            // ---------- ARRANGE ----------
            await _db.RecycleDb();
            var token = _db.AuthenticateAdmin();
            var user = _db.GetUserBasic();
            HttpResponseMessage? response;

            // ---------- ACT ----------
            using (var client = _webApplicationFactory.CreateDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await client.DeleteAsync($"/v1/users/{user.Id}");
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
            Assert.Equal("User deleted.", dataString);
        }

        #endregion DELETE USER

    }
}
