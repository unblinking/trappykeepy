namespace TrappyKeepy.Domain.Models
{
    public class GroupDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
