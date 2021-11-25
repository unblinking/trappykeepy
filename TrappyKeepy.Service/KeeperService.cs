using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    public class KeeperService : IKeeperService
    {
        private string connectionString;

        public KeeperService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public KeeperService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<KeeperServiceResponse> Create(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<KeeperServiceResponse> ReadAll(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<KeeperServiceResponse> ReadById(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<KeeperServiceResponse> UpdateById(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<KeeperServiceResponse> DeleteById(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}