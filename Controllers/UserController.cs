using cldvPOE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;

namespace cldvPOE.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Server=tcp:clddevsqlserver.database.windows.net,1433;Initial Catalog=cldv-POE-DB;Persist Security Info=False;User ID=AqeelahHoosen;Password=Belltone92;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");
        }

        public IActionResult Profile()
        {
            int userId = GetCurrentUserId();
            UserTable user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE UserId = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserTable
                            {
                                UserID = (int)reader["UserId"],
                                UserFirstName = (string)reader["Username"],
                                UserEmail = (string)reader["Email"],
                                UserPassword = (string)reader["Password"],
                                Role = (string)reader["Role"]
                                // Map other user properties
                            };
                        }
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(UserTable model)
        {
            if (ModelState.IsValid)
            {
                int userId = GetCurrentUserId();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET Username = @Username, Email = @Email WHERE UserId = @UserId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", model.UserFirstName);
                        command.Parameters.AddWithValue("@Email", model.UserEmail);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Profile");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserTable model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Username, Email, Password, Role) " +
                                   "VALUES (@Username, @Email, @Password, @Role)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", model.UserFirstName);
                        command.Parameters.AddWithValue("@Email", model.UserEmail);
                        command.Parameters.AddWithValue("@Password", model.UserPassword);
                        command.Parameters.AddWithValue("@Role", model.Role);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Login", "Login");
            }

            return View(model);
        }

        private int GetCurrentUserId()
        {
            // Implement logic to get the current user's ID from authentication
            // Return the user ID
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.Parse(userId);
            }
            return 0;
        }
    }
}
