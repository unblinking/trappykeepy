using System;
using System.Threading.Tasks;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}