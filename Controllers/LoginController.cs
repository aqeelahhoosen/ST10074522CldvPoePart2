using cldvPOE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace cldvPOE.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Server=tcp:clddevsqlserver.database.windows.net,1433;Initial Catalog=cldv-POE-DB;Persist Security Info=False;User ID=AqeelahHoosen;Password=Belltone92;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", model.UserFirstName);
                        command.Parameters.AddWithValue("@Password", model.Password);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Set authentication cookie or token
                                // Redirect to appropriate page based on user role
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid username or password.");
                            }
                        }
                    }
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            // Clear authentication cookie or token
            return RedirectToAction("Login");
        }
    }
}
