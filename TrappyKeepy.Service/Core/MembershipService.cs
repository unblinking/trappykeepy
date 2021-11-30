using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    /// <summary>
    /// The membership service.
    /// A user may have one or more group memberships.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        /// <summary>
        /// Group database operations into a single transaction (unit of work).
        /// </summary>
        private readonly IUnitOfWork uow;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MembershipService(IUnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        /// <summary>
        /// Create a new membership in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> Create(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify requried parameters.
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

            try
            {
                // Begin this transaction.
                uow.Begin();

                if (request.Item is null) // Had to put this extra if() here to eliminate a "deferrence of possibly null reference" warning.
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Group id and user id are required to create a membership.";
                    return response;
                }
                // Verify the requested membership is not already created.
                var existingMembershipCount = await uow.memberships.CountByGroupAndUser(
                    request.Item.GroupId,
                    request.Item.UserId
                );
                if (existingMembershipCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership already exists.";
                    return response;
                }

                // Create the new membership record now.
                var id = await uow.memberships.Create(request.Item);

                // Commit changes in this transaction.
                uow.Commit();

                // Pass a MembershipDto back to the controller.
                response.Item = new MembershipDto() { GroupId = id };

                // Success if we made it this far.
                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Read all memberships from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> ReadAll(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            try
            {
                // Read the membership records now.
                var memberships = await uow.memberships.ReadAll();

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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Read all memberships from the database for a specified group.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> ReadByGroupId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to find a membership by group id.";
                return response;
            }

            try
            {
                // Read the membership records now.
                var memberships = await uow.memberships.ReadByGroupId((Guid)request.Id);

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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Read all memberships from the database for a specified user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> ReadByUserId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to find a membership by user by id.";
                return response;
            }

            try
            {
                // Read the membership record now.
                var memberships = await uow.memberships.ReadByUserId((Guid)request.Id);

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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Delete all memberships from the database for a specified group.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> DeleteByGroupId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to delete membership(s) by group id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that at least one membership exists for the group.
                var existingCount = await uow.memberships.CountByColumnValue("group_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership(s) for delete not found.";
                    return response;
                }

                // Delete the membership record(s) now.
                var successful = await uow.memberships.DeleteByGroupId((Guid)request.Id);

                // If the membership record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Membership(s) not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                uow.Commit();

                // TODO: Return the number of memberships deleted?

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Delete all memberships in the database for a specified user id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> DeleteByUserId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to delete membership(s) by user id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that at least one membership exists for the user.
                var existingCount = await uow.memberships.CountByColumnValue("user_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership(s) for delete not found.";
                    return response;
                }

                // Delete the membership record(s) now.
                var successful = await uow.memberships.DeleteByUserId((Guid)request.Id);

                // If the membership record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Membership(s) not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                uow.Commit();

                // TODO: Return the number of memberships deleted?

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Delete a membership from the database for a specific group/user id combo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MembershipServiceResponse> DeleteByGroupIdAndUserId(MembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (
                request.GroupId is null || request.GroupId == Guid.Empty ||
                request.UserId is null || request.UserId == Guid.Empty
            )
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id and user id are required to delete a specific membership.";
                return response;
            }

            try
            {
                // Verify that the membership exists.
                var existingCount = await uow.memberships.CountByGroupAndUser(
                    (Guid)request.GroupId,
                    (Guid)request.UserId
                );
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership for delete does not exist.";
                    return response;
                }

                // Delete the membership record now.
                var successful = await uow.memberships.DeleteByGroupIdAndUserId(
                    (Guid)request.GroupId,
                    (Guid)request.UserId
                );

                // If the membership record couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Membership was not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                uow.Commit();

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }
    }
}
