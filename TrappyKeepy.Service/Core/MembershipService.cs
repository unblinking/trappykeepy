using AutoMapper;
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
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Automapper http://automapper.org/
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new membership in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IMembershipServiceResponse> Create(IMembershipServiceRequest request)
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
                response.ErrorMessage = "GroupId (UUID) and UserId (UUID) are required to create a membership.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var membership = _mapper.Map<Membership>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify the requested membership is not already created.
                var existingMembershipCount = await _uow.memberships.CountByGroupAndUser(membership.GroupId, membership.UserId);
                if (existingMembershipCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership already exists.";
                    return response;
                }

                // Create the new membership record now.
                var newMembership = await _uow.memberships.Create(membership);

                // Commit changes in this transaction.
                _uow.Commit();

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<IMembershipDto>(newMembership);

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
        public async Task<IMembershipServiceResponse> ReadAll(IMembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            try
            {
                // Read the membership records now.
                var memberships = await _uow.memberships.ReadAll();

                // Map the repository's domain objects to DTOs for the response to the controller.
                var membershipDtos = new List<IMembershipDto>();
                foreach (var membership in memberships) membershipDtos.Add(_mapper.Map<IMembershipDto>(membership));
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
        public async Task<IMembershipServiceResponse> ReadByGroupId(IMembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to find a membership by group id.";
                return response;
            }

            try
            {
                // Read the membership records now.
                var memberships = await _uow.memberships.ReadByGroupId((Guid)request.Id);

                // Map the repository's domain objects to DTOs for the response to the controller.
                var membershipDtos = new List<IMembershipDto>();
                foreach (var membership in memberships) membershipDtos.Add(_mapper.Map<IMembershipDto>(membership));
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
        public async Task<IMembershipServiceResponse> ReadByUserId(IMembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to find a membership by user id.";
                return response;
            }

            try
            {
                // Read the membership record now.
                var memberships = await _uow.memberships.ReadByUserId((Guid)request.Id);

                // Map the repository's domain objects to DTOs for the response to the controller.
                var membershipDtos = new List<IMembershipDto>();
                foreach (var membership in memberships) membershipDtos.Add(_mapper.Map<IMembershipDto>(membership));
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
        /// Delete a membership from the database for a specific group/user id combo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IMembershipServiceResponse> DeleteById(IMembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete a specific membership by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that the membership exists.
                var existing = await _uow.memberships.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership for delete does not exist.";
                    return response;
                }

                // Delete the membership record now.
                var successful = await _uow.memberships.DeleteById((Guid)request.Id);

                // If the membership record couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Membership was not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                _uow.Commit();

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
        public async Task<IMembershipServiceResponse> DeleteByGroupId(IMembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete membership(s) by group id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that at least one membership exists for the group.
                var existingCount = await _uow.memberships.CountByColumnValue("group_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership(s) for delete not found.";
                    return response;
                }

                // Delete the membership record(s) now.
                var successful = await _uow.memberships.DeleteByGroupId((Guid)request.Id);

                // If the membership record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Membership(s) not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                _uow.Commit();

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
        public async Task<IMembershipServiceResponse> DeleteByUserId(IMembershipServiceRequest request)
        {
            var response = new MembershipServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete membership(s) by user id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that at least one membership exists for the user.
                var existingCount = await _uow.memberships.CountByColumnValue("user_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested membership(s) for delete not found.";
                    return response;
                }

                // Delete the membership record(s) now.
                var successful = await _uow.memberships.DeleteByUserId((Guid)request.Id);

                // If the membership record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Membership(s) not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                _uow.Commit();

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
