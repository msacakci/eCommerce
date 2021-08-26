using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;
 

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private List<string> languageCodes;
        private readonly string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";
        string activeLanguage;
        
        public ProductsController()
        {
            adjustLanguageCodes();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            List<Product> productList = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Console.WriteLine( connection.State == ConnectionState.Open); // For test purposes

                // Retrieve the rows that are contains correct language code.
                string stringOfSqlCommandFirst = "SELECT * FROM [dbo].[product_translations] WHERE language_code = '"+ activeLanguage + "';";
                
                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    var product = new Product();

                    product.Id = Convert.ToInt32( sqlDataReader["product_id"]);
                    product.Description = sqlDataReader["description"].ToString();

                    string productInformation = product.Id + ". " + product.Description + "\n";

                    Console.WriteLine(productInformation);

                    productList.Add( product);
                }

                sqlDataReader.Close();
            }                   
            return productList;

        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = new Product();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommandFirst = "SELECT * FROM [dbo].[product_translations] WHERE language_code = '"+ activeLanguage + "' AND product_id = " + id + ";";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    product.Id = Convert.ToInt32( sqlDataReader["product_id"]);
                    product.Description = sqlDataReader["description"].ToString();

                    string productInformation = product.Id + ". " + product.Description + "\n";

                    Console.WriteLine(productInformation);
                }

                sqlDataReader.Close();

            }
            return product;

        }

        // [HttpGet("{languageCode}")]
        // public ActionResult<IEnumerable<Product>> GetProductsWithSpecificLanguage(string languageCode)
        // {
        //     activeLanguage = languageCode;

        //     if(!languageCodes.Contains(activeLanguage))
        //     {
        //         Console.WriteLine("Language code error");
        //         return null;
        //     }        
        //     else
        //     {
        //         return GetProducts();
        //     }     
        // }

        private void adjustLanguageCodes()
        {
            languageCodes = new List<string>();
            GetLanguagesHelper getLanguagesHelper = new GetLanguagesHelper();

            languageCodes = getLanguagesHelper.GetLanguagesFromDatabase(connectionString);

            activeLanguage = languageCodes[0];
        }
    }

}