namespace TrappyKeepy.Domain.Models
{
    public interface IMembershipDto
    {
        Guid? Id { get; }
        Guid? GroupId { get; }
        Guid? UserId { get; }
    }

    public class MembershipDto : IMembershipDto
    {
        public Guid? Id { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }
    }
}
