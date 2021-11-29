using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    public class MembershipService : IMembershipService
    {
        // TODO: Figure out some way to inject UnitOfWork here for testing.
        // TODO: This connection string env var is ugly too.

        private string connectionString;

        public MembershipService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public MembershipService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<MembershipServiceResponse> Create(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            if (
                request.Item is null ||
                request.Item?.GroupId is null || request.Item?.GroupId == Guid.Empty ||
                request.Item?.UserId is null || request.Item?.UserId == Guid.Empty
            )
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id and user id are required to create a membership.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    if (request.Item is null) // Had to put this extra if() here to eliminate a "deferrence of possibly null reference" warning.
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Group id and user id are required to create a membership.";
                        return response;
                    }
                    // Verify the requested membership is not already created.
                    var existingMembershipCount = await unitOfWork.MembershipRepository
                        .CountByGroupAndUser(request.Item.GroupId, request.Item.UserId);
                    if (existingMembershipCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested membership already exists.";
                        return response;
                    }

                    // Create the new membership record now.
                    var id = await unitOfWork.MembershipRepository.Create(request.Item);

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    // Pass a MembershipDto back to the controller.
                    response.Item = new MembershipDto() { GroupId = id };

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<MembershipServiceResponse> ReadAll(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the membership records now.
                    var memberships = await unitOfWork.MembershipRepository.ReadAll();

                    // Pass a list of membershipDtos back to the controller.
                    var membershipDtos = new List<MembershipDto>();
                    foreach (var membership in memberships)
                    {
                        var membershipDto = new MembershipDto()
                        {
                            GroupId = membership.GroupId,
                            UserId = membership.UserId,
                        };
                        membershipDtos.Add(membershipDto);
                    }
                    response.List = membershipDtos;

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<MembershipServiceResponse> ReadByGroupId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to find a membership by group.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the membership records now.
                    var memberships = await unitOfWork.MembershipRepository.ReadByGroupId((Guid)request.Id);

                    // Pass a list of membershipDtos back to the controller.
                    var membershipDtos = new List<MembershipDto>();
                    foreach (var membership in memberships)
                    {
                        var membershipDto = new MembershipDto()
                        {
                            GroupId = membership.GroupId,
                            UserId = membership.UserId,
                        };
                        membershipDtos.Add(membershipDto);
                    }
                    response.List = membershipDtos;

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<MembershipServiceResponse> ReadByUserId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to find a membership by user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the membership record now.
                    var memberships = await unitOfWork.MembershipRepository.ReadByUserId((Guid)request.Id);

                    // Pass a list of membershipDtos back to the controller.
                    var membershipDtos = new List<MembershipDto>();
                    foreach (var membership in memberships)
                    {
                        var membershipDto = new MembershipDto()
                        {
                            GroupId = membership.GroupId,
                            UserId = membership.UserId,
                        };
                        membershipDtos.Add(membershipDto);
                    }
                    response.List = membershipDtos;

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<MembershipServiceResponse> DeleteByGroupId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to delete membership(s) by group.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that at least one membership exists for the group.
                    var existingCount = await unitOfWork.MembershipRepository.CountByColumnValue("group_id", (Guid)request.Id);
                    if (existingCount < 1)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested membership(s) for delete not found.";
                        return response;
                    }

                    // Delete the membership record(s) now.
                    var successful = await unitOfWork.MembershipRepository.DeleteByGroupId((Guid)request.Id);

                    // If the membership record(s) couldn't be deleted, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Membership(s) not deleted.";
                        return response;
                    }

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    // TODO: Return the number of memberships deleted?
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<MembershipServiceResponse> DeleteByUserId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to delete membership(s) by user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that at least one membership exists for the user.
                    var existingCount = await unitOfWork.MembershipRepository.CountByColumnValue("user_id", (Guid)request.Id);
                    if (existingCount < 1)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested membership(s) for delete not found.";
                        return response;
                    }

                    // Delete the membership record(s) now.
                    var successful = await unitOfWork.MembershipRepository.DeleteByUserId((Guid)request.Id);

                    // If the membership record(s) couldn't be deleted, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Membership(s) not deleted.";
                        return response;
                    }

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    // TODO: Return the number of memberships deleted?
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<MembershipServiceResponse> DeleteByGroupIdAndUserId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();
            if (
                request.GroupId is null || request.GroupId == Guid.Empty ||
                request.UserId is null || request.UserId == Guid.Empty
            )
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id and user id are required to delete a membership.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the membership exists.
                    var existingCount = await unitOfWork.MembershipRepository.CountByGroupAndUser((Guid)request.GroupId, (Guid)request.UserId);
                    if (existingCount < 1)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested membership for delete does not exist.";
                        return response;
                    }

                    // Delete the membership record now.
                    var successful = await unitOfWork.MembershipRepository.DeleteByGroupIdAndUserId((Guid)request.GroupId, (Guid)request.UserId);

                    // If the membership record couldn't be deleted, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Membership was not deleted.";
                        return response;
                    }

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }
    }
}
