using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
 

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ProductsController : ControllerBase
    {
        //private readonly List<Product> _content;

        // public ProductsController(List<Product> content)
        // {
        //     _content = content;
        // }

        public ProductsController()
        {
            //_content = content;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            Console.WriteLine("Hello");

            List<Product> productList = new List<Product>();

            string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Console.WriteLine( connection.State == ConnectionState.Open);

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM [dbo].[products]", connection);
                sqlCommand.CommandType = CommandType.Text;
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    var product = new Product();

                    product.Id = Convert.ToInt32( sqlDataReader["id"]);
                    product.ProductName = sqlDataReader["product_name"].ToString();
                    product.ProductTypeId = Convert.ToInt32( sqlDataReader["product_type_id"]);
                    product.Price = Convert.ToInt32( sqlDataReader["price"]);

                    Console.WriteLine(product.ProductName);

                    productList.Add( product);
                }
            }        
            return productList;

        }
    }
}