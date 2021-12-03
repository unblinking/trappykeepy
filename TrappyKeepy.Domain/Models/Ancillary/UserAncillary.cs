namespace TrappyKeepy.Domain.Models
{
    public interface IUserDto
    {
        Guid? Id { get; }
        string? Name { get; }
        string? Password { get; }
        string? Email { get; }
        string? Role { get; }
        DateTime? DateCreated { get; }
        DateTime? DateActivated { get; }
        DateTime? DateLastLogin { get; }
    }

    public class UserDto : IUserDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateActivated { get; set; }
        public DateTime? DateLastLogin { get; set; }
    }

    public interface IUserSessionDto
    {
        string Email { get; }
        string Password { get; }
    }

    public class UserSessionDto : IUserSessionDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserSessionDto(string e, string p)
        {
            Email = e;
            Password = p;
        }
    }
}
