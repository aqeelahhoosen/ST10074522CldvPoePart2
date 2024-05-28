using cldvPOE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;

namespace cldvPOE.Controllers
{
    public class TransactionController : Controller
    {
        private readonly string _connectionString;

        public TransactionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Server=tcp:clddevsqlserver.database.windows.net,1433;Initial Catalog=cldv-POE-DB;Persist Security Info=False;User ID=AqeelahHoosen;Password=Belltone92;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");
        }

        public IActionResult PlaceOrder(int productId)
        {
            List<ProductTable> products = new List<ProductTable>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products WHERE ProductId = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProductTable product = new ProductTable
                            {
                                ProductID = (int)reader["ProductId"],
                                ProductName = (string)reader["ProductName"],
                                ProductDescription = (string)reader["ProductDescription"],
                                Price = (decimal)reader["Price"],
                                Category = (string)reader["Category"],
                                Availability = (bool)reader["Availability"],
                                ImageUrl = (string)reader["ImageUrl"]
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            if (products.Count == 0)
            {
                return NotFound();
            }

            return View(products);
        }

        [HttpPost]
        public IActionResult PlaceOrder(TransactionTable model)
        {
            if (ModelState.IsValid)
            {
                int userId = GetCurrentUserId();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Transactions (UserId, ProductId, TransactionDate, Quantity, TotalAmount) " +
                                   "VALUES (@UserId, @ProductId, @TransactionDate, @Quantity, @TotalAmount)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@ProductId", model.ProductID);
                        command.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                        command.Parameters.AddWithValue("@Quantity", model.Quantity);
                        command.Parameters.AddWithValue("@TotalAmount", model.Quantity * GetProductPrice(model.ProductID));
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("OrderHistory");
            }
            return View(model);
        }

        public IActionResult OrderHistory()
        {
            int userId = GetCurrentUserId();
            List<TransactionTable> orders = new List<TransactionTable>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Transactions WHERE UserId = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TransactionTable order = new TransactionTable
                            {
                                TransactionID = (int)reader["TransactionId"],
                                UserID = (int)reader["UserId"],
                                ProductID = (int)reader["ProductId"],
                                Date = (DateTime)reader["TransactionDate"],
                                Quantity = (int)reader["Quantity"],
                                TotalAmount = (decimal)reader["TotalAmount"]
                            };
                            orders.Add(order);
                        }
                    }
                }
            }

            return View(orders);
        }

        public IActionResult ProcessOrders()
        {
            List<TransactionTable> orders = new List<TransactionTable>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Transactions";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TransactionTable order = new TransactionTable
                            {
                                TransactionID = (int)reader["TransactionId"],
                                UserID = (int)reader["UserId"],
                                ProductID = (int)reader["ProductId"],
                                Date = (DateTime)reader["TransactionDate"],
                                Quantity = (int)reader["Quantity"],
                                TotalAmount = (decimal)reader["TotalAmount"]
                            };
                            orders.Add(order);
                        }
                    }
                }
            }

            return View(orders);
        }

        private int GetCurrentUserId()
        {
            // Implement logic to get the current user's ID from authentication
            // Return the user ID
            {
                if (User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim != null)
                    {
                        if (int.TryParse(userIdClaim.Value, out int userId))
                        {
                            return userId;
                        }
                    }
                }

                return 0;
            }
        }

        private decimal GetProductPrice(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Price FROM Products WHERE ProductId = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    return (decimal)command.ExecuteScalar();
                }
            }
        }
    }
}
