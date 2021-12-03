using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IPermitService
    {
        Task<IPermitServiceResponse> Create(IPermitServiceRequest request);
        Task<IPermitServiceResponse> ReadAll(IPermitServiceRequest request);
        Task<IPermitServiceResponse> ReadByKeeperId(IPermitServiceRequest request);
        Task<IPermitServiceResponse> ReadByUserId(IPermitServiceRequest request);
        Task<IPermitServiceResponse> ReadByGroupId(IPermitServiceRequest request);
        Task<IPermitServiceResponse> DeleteById(IPermitServiceRequest request);
        Task<IPermitServiceResponse> DeleteByKeeperId(IPermitServiceRequest request);
        Task<IPermitServiceResponse> DeleteByUserId(IPermitServiceRequest request);
        Task<IPermitServiceResponse> DeleteByGroupId(IPermitServiceRequest request);
    }
}
