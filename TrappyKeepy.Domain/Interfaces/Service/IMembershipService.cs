﻿using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IMembershipService
    {
        Task<MembershipServiceResponse> Create(MembershipServiceRequest request);
        Task<MembershipServiceResponse> ReadAll(MembershipServiceRequest request);
        Task<MembershipServiceResponse> ReadByGroupId(MembershipServiceRequest request);
        Task<MembershipServiceResponse> ReadByUserId(MembershipServiceRequest request);
        Task<MembershipServiceResponse> DeleteById(MembershipServiceRequest request);
        Task<MembershipServiceResponse> DeleteByGroupId(MembershipServiceRequest request);
        Task<MembershipServiceResponse> DeleteByUserId(MembershipServiceRequest request);
    }
}
