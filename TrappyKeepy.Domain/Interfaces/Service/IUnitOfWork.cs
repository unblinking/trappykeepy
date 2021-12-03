namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IFiledataRepository filedatas { get; }
        IGroupRepository groups { get; }
        IKeeperRepository keepers { get; }
        IMembershipRepository memberships { get; }
        IPermitRepository permits { get; }
        IUserRepository users { get; }
        void Begin();
        void Commit();
        void Rollback();
    }
}
