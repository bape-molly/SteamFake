using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;


public class UserManager
{
    private string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";

    public void RegisterUser(User user)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("INSERT INTO Users (Username, Password, Role, Status, Image, RegistrationDate) VALUES (@Username, @Password, @Role, @Status, @Image, @RegistrationDate)", connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@Status", user.Status);
            command.Parameters.AddWithValue("@Image", user.Image);
            command.Parameters.AddWithValue("@RegistrationDate", user.RegistrationDate);
            command.ExecuteNonQuery();
        }
    }

    public User LoginUser(string username, string password)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Users WHERE Username = @Username AND Password = @Password", connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User
                    {
                        Id = reader.GetInt32("Id"),
                        Username = reader.GetString("Username"),
                        Password = reader.GetString("Password"),
                        Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString("Role"),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                        Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString("Image"),
                        RegistrationDate = reader.GetDateTime("RegistrationDate")
                    };
                }
            }
        }
        return null;
    }


    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Users", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32("Id"),
                        Username = reader.GetString("Username"),
                        Password = reader.GetString("Password"),
                        Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString("Role"),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                        Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString("Image"),
                        RegistrationDate = reader.GetDateTime("RegistrationDate")
                    });
                }
            }
        }
        return users;
    }

    public User GetUserById(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User
                    {
                        Id = reader.GetInt32("Id"),
                        Username = reader.GetString("Username"),
                        Password = reader.GetString("Password"),
                        Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString("Role"),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                        Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString("Image"),
                        RegistrationDate = reader.GetDateTime("RegistrationDate")
                    };
                }
            }
        }
        return null;
    }

    public void UpdateUser(User user)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("UPDATE Users SET Username = @Username, Password = @Password, Status = @Status, Image = @Image WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Status", user.Status);
            command.Parameters.AddWithValue("@Image", user.Image);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteUser(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }

    public bool UsernameExists(string username)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            string checkQuery = "SELECT COUNT(*) FROM users WHERE Username = @Username";
            using (MySqlCommand cmd = new MySqlCommand(checkQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                return userCount > 0;
            }
        }
    }

}
