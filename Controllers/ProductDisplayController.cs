using cldvPOE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace cldvPOE.Controllers
{
    public class ProductDisplayController : Controller
    {
        public  List<ProductDisplayModel> SelectProducts()
        {
            List<ProductDisplayModel> products = new List<ProductDisplayModel>();

            string con_string = "Integrated Security=SSPI;Persist Security Info=False;User ID=\"\";Initial Catalog=test;Data Source=labVMH8OX\\SQLEXPRESS";
            using (SqlConnection con = new SqlConnection(con_string))
            {
                string sql = "SELECT ProductID, ProductName, ProductPrice, ProductCategory, ProductAvailability FROM productTable";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ProductDisplayModel product = new ProductDisplayModel();
                    product.ProductID = Convert.ToInt32(reader["ProductID"]);
                    product.ProductName = Convert.ToString(reader["ProductName"]);
                    product.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                    product.ProductCategory = Convert.ToString(reader["ProductCategory"]);
                    product.ProductAvailability = Convert.ToBoolean(reader["ProductAvailability"]);

                    products.Add(product);
                }
                reader.Close();
            }
            return products;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var products = ProductDisplayModel.SelectProducts();
            return View(products);
        }
    }
}
