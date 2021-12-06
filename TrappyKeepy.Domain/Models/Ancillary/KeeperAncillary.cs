namespace TrappyKeepy.Domain.Models
{
    public interface IKeeperDto
    {
        Guid? Id { get; }
        string? Filename { get; }
        string? ContentType { get; }
        string? Description { get; }
        string? Category { get; }
        DateTime? DatePosted { get; }
        Guid? UserPosted { get; }
    }

    public class KeeperDto : IKeeperDto
    {
        public Guid? Id { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? DatePosted { get; set; }
        public Guid? UserPosted { get; set; }
    }
}
