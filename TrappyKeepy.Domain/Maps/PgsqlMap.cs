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
                Role = short.Parse($"{reader["role"].ToString()}"),
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
    }
}
