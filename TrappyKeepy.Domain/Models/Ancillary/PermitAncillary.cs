namespace TrappyKeepy.Domain.Models
{
    public interface IPermitDto
    {
        Guid? Id { get; }
        Guid? KeeperId { get; }
        Guid? UserId { get; }
        Guid? GroupId { get; }
    }


    public class PermitDto : IPermitDto
    {
        public Guid? Id { get; set; }
        public Guid? KeeperId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }
    }
}
