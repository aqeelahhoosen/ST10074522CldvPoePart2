using cldvPOE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace cldvPOE.Controllers
{
    public class ProductController : Controller
    {
        private readonly string _connectionString;

        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Server=tcp:clddevsqlserver.database.windows.net,1433;Initial Catalog=cldv-POE-DB;Persist Security Info=False;User ID=AqeelahHoosen;Password=Belltone92;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");
        }

        public List<ProductTable> GetAllProducts()
        {
            List<ProductTable> products = new List<ProductTable>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
                                // Map other properties
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductTable model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Products (ProductName, ProductDescription, Price, Category, Availability, ImageUrl) " +
                                   "VALUES (@ProductName, @ProductDescription, @Price, @Category, @Availability, @ImageUrl)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", model.ProductName);
                        command.Parameters.AddWithValue("@ProductDescription", model.ProductDescription);
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@Category", model.Category);
                        command.Parameters.AddWithValue("@Availability", model.Availability);
                        command.Parameters.AddWithValue("@ImageUrl", model.ImageUrl);
                        // Set other parameter values
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
