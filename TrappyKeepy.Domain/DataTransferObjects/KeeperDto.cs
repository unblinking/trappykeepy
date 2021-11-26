namespace TrappyKeepy.Domain.Models
{
    public class KeeperDto
    {
        public Guid? Id { get; set; }
        public string? Filename { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? DatePosted { get; set; }
        public Guid? UserPosted { get; set; }
        public byte[]? Binarydata { get; set; }
    }
}