namespace TrappyKeepy.Domain.Models
{
    public interface IGroupDto
    {
        Guid? Id { get; }
        string? Name { get; }
        string? Description { get; }
        DateTime? DateCreated { get; }
    }

    public class GroupDto : IGroupDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
