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
        private List<string> languageCodes;
        private readonly string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";
        string activeLanguage;
        
        public ProductsController()
        {
            // Step 1: Create an array list that will keep the all of the possible language codes.
            languageCodes = new List<string>();

            // Step 2: Read the ref_languages table to see available languages and add them to the array list.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Console.Write("Available languages: ");

                string stringOfSqlCommand = "SELECT * FROM [dbo].[ref_languages]";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);

                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while(sqlDataReader.Read())
                {
                    languageCodes.Add( sqlDataReader["code"].ToString());
                    Console.Write( sqlDataReader["name"].ToString() + " | ");
                }

                Console.Write("\n" + "---------------------------------------------" + "\n");
                sqlDataReader.Close();
            }

            // Step 3: Set first element of the languageCodes as default language 
            activeLanguage = languageCodes[0];
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

        [HttpGet("{languageCode}")]
        public ActionResult<IEnumerable<Product>> GetProductsWithSpecificLanguage(string languageCode)
        {
            activeLanguage = languageCode;

            if(!languageCodes.Contains(activeLanguage))
            {
                Console.WriteLine("Language code error");
                return null;
            }        
            else
            {
                return GetProducts();
            }     
        }
    }

}