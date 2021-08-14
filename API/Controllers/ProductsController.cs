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
        private static readonly string[] languageCodes = new[]
        {
            "en", "tr"
        }; 

        string activeLanguage;
        int activeLanguageID;

        // public ProductsController(List<Product> content)
        // {
        //     _content = content;
        // }

        public ProductsController()
        {
            activeLanguage = languageCodes[0];
            activeLanguageID = 1;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";

            string product_name_label = "";
            string product_type_label = "";
            string price_label = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommand = "SELECT * FROM [dbo].[products_table_labels] WHERE language_id = " + activeLanguageID;

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);

                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while(sqlDataReader.Read())
                {
                    product_name_label = sqlDataReader["product_name_label"].ToString();
                    product_type_label = sqlDataReader["product_type_label"].ToString();
                    price_label = sqlDataReader["price_label"].ToString();
                }
            }


            List<Product> productList = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Console.WriteLine( connection.State == ConnectionState.Open); // For test purposes

                string stringOfSqlCommandFirst = "SELECT * FROM [dbo].[products]";
                
                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);

                
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

                    string productInformation = product.Id + ". " + "\n" +
                                                product_name_label + product.ProductName + "\n" +
                                                product_type_label + product.ProductTypeId + "\n" +
                                                price_label + product.Price + "\n";

                    Console.WriteLine(productInformation);

                    productList.Add( product);
                }
            }                   
            return productList;

        }
    }
}