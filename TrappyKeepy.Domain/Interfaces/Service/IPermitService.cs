using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IPermitService
    {
        Task<PermitServiceResponse> Create(PermitServiceRequest request);
        Task<PermitServiceResponse> ReadAll(PermitServiceRequest request);
        Task<PermitServiceResponse> ReadByKeeperId(PermitServiceRequest request);
        Task<PermitServiceResponse> ReadByUserId(PermitServiceRequest request);
        Task<PermitServiceResponse> ReadByGroupId(PermitServiceRequest request);
        Task<PermitServiceResponse> DeleteById(PermitServiceRequest request);
        Task<PermitServiceResponse> DeleteByKeeperId(PermitServiceRequest request);
        Task<PermitServiceResponse> DeleteByUserId(PermitServiceRequest request);
        Task<PermitServiceResponse> DeleteByGroupId(PermitServiceRequest request);
    }
}
