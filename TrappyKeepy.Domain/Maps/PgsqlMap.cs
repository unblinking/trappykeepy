using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace TrappyKeepy.Domain
{
    public class PgsqlMap
    {
        public User User(NpgsqlDataReader reader)
        {
            return new User(){
                Id = Guid.Parse($"{reader["id"].ToString()}"),
                Name = $"{reader["name"].ToString()}",
                Password = $"{reader["password"].ToString()}",
                Email = $"{reader["email"].ToString()}",
                DateCreated = DateTime.Parse($"{reader["date_created"].ToString()}"),
                DateActivated = reader["date_activated"] is not DBNull ? DateTime.Parse($"{reader["date_activated"].ToString()}") : null,
                DateLastLogin = reader["date_last_login"] is not DBNull ? DateTime.Parse($"{reader["date_last_login"].ToString()}") : null,
            };
        }
    }
}
