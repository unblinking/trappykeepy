using System.Text;
using Npgsql;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Maps
{
    public class PgsqlReaderMap
    {
        public User User(NpgsqlDataReader reader)
        {
            return new User()
            {
                Id = Guid.Parse($"{reader["id"].ToString()}"),
                Name = $"{reader["name"].ToString()}",
                Password = $"{reader["password"].ToString()}",
                Email = $"{reader["email"].ToString()}",
                Role = $"{reader["role"].ToString()}",
                DateCreated = DateTime.Parse($"{reader["date_created"].ToString()}"),
                DateActivated = reader["date_activated"] is not DBNull ? DateTime.Parse($"{reader["date_activated"].ToString()}") : null,
                DateLastLogin = reader["date_last_login"] is not DBNull ? DateTime.Parse($"{reader["date_last_login"].ToString()}") : null,
            };
        }

        public Keeper Keeper(NpgsqlDataReader reader)
        {
            return new Keeper()
            {
                Id = Guid.Parse($"{reader["id"].ToString()}"),
                Filename = $"{reader["filename"].ToString()}",
                Description = reader["description"] is not DBNull ? $"{reader["description"].ToString()}" : null,
                Category = reader["category"] is not DBNull ? $"{reader["category"].ToString()}" : null,
                DatePosted = DateTime.Parse($"{reader["date_posted"].ToString()}"),
                UserPosted = Guid.Parse($"{reader["user_posted"].ToString()}"),
            };
        }

        public Filedata Filedata(NpgsqlDataReader reader)
        {
            return new Filedata()
            {
                KeeperId = Guid.Parse($"{reader["keeper_id"].ToString()}"),
                BinaryData = (byte[])reader["binary_data"],
            };
        }

        public Group Group(NpgsqlDataReader reader)
        {
            return new Group()
            {
                Id = Guid.Parse($"{reader["id"].ToString()}"),
                Name = $"{reader["name"].ToString()}",
                Description = reader["description"] is not DBNull ? $"{reader["description"].ToString()}" : null,
                DateCreated = DateTime.Parse($"{reader["date_created"].ToString()}"),
            };
        }

        public Membership Membership(NpgsqlDataReader reader)
        {
            return new Membership()
            {
                Id = Guid.Parse($"{reader["id"].ToString()}"),
                GroupId = Guid.Parse($"{reader["group_id"].ToString()}"),
                UserId = Guid.Parse($"{reader["user_id"].ToString()}"),
            };
        }

        public Permit Permit(NpgsqlDataReader reader)
        {
            return new Permit()
            {
                Id = Guid.Parse($"{reader["id"].ToString()}"),
                KeeperId = Guid.Parse($"{reader["keeper_id"].ToString()}"),
                UserId = reader["user_id"] is not DBNull ? Guid.Parse($"{reader["user_id"].ToString()}") : null,
                GroupId = reader["group_id"] is not DBNull ? Guid.Parse($"{reader["group_id"].ToString()}") : null,
            };
        }
    }
}
