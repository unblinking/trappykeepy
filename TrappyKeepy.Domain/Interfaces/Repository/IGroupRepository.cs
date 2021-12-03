using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task<Group> Create(Group group);
        Task<List<Group>> ReadAll();
        Task<Group> ReadById(Guid id);
        Task<bool> UpdateById(Group group);
        Task<bool> DeleteById(Guid id);
        Task<int> CountByColumnValue(string column, string value);
    }
}
