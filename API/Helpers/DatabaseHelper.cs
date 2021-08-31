using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers
{
    public class DatabaseHelper
    {
        public DatabaseHelper()
        {

        }

        public Task<bool> InsertToDatabase(string connectionString, string stringOfSqlCommand)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {   
                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }

            return Task.FromResult(true);
        }

        public ProductDto GetProductDtoFromProductsTable(string connectionString, int productId)
        {
            ProductDto productDto = new ProductDto
            {
                Id = -1,
                ProductCategoryId = -1
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommand = "SELECT * FROM [dbo].[products] WHERE id = " + productId + ";";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    productDto.Id = Convert.ToInt32( sqlDataReader["id"]);
                    productDto.ProductCategoryId = Convert.ToInt32( sqlDataReader["product_category_id"]);
                }
                sqlDataReader.Close();
            }

            if(productDto.Id == -1)
            {
                return null;
            }
            else
            {
                return productDto;
            }
        }

        public Product GetProductFromDatabase( string connectionString, ProductDto productDto, string activeLanguage)
        {
            var product = new Product()
            {
                Id = -1,
                Description = "",
                ProductType = ""
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommand = "SELECT * FROM [dbo].[product_translations] WHERE language_code = '"+ activeLanguage + "' AND product_id = " + productDto.Id + ";";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    product.Id = Convert.ToInt32( sqlDataReader["product_id"]);
                    product.Description = sqlDataReader["description"].ToString();
                }
                sqlDataReader.Close();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommand = "SELECT * FROM [dbo].[product_category_translations] WHERE language_code = '"+ activeLanguage + "' AND product_category_id = " + productDto.ProductCategoryId + ";";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    product.ProductType = sqlDataReader["description"].ToString();
                }
                sqlDataReader.Close();
            }

            if(product.Id == -1)
            {
                return null;
            }
            else
            {
                string productInfo = product.Id + ". " + product.Description + " - " + product.ProductType  + "\n";
                Console.WriteLine(productInfo);
                return product;
            }
        }

        public List<ProductDto> GetListOfProductDtos(string connectionString)
        {
            List<ProductDto> productDtoList = new List<ProductDto>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommand = "SELECT * FROM [dbo].[products];";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    ProductDto productDto = new ProductDto();

                    productDto.Id = Convert.ToInt32( sqlDataReader["id"]);
                    productDto.ProductCategoryId = Convert.ToInt32( sqlDataReader["product_category_id"]);

                    productDtoList.Add(productDto);
                }
                sqlDataReader.Close();
            }
            return productDtoList;
        }

        public List<Product> GetListOfProducts(string connectionString, List<ProductDto> productDtoList, string activeLanguage)
        {
            List<Product> productList = new List<Product>();

            for(int index = 0; index < productDtoList.Count; index++)
            {
                Product product = GetProductFromDatabase(connectionString, productDtoList[index], activeLanguage);
                productList.Add(product);
            }

            return productList;
        }
    }
}