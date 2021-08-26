using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";

        public AccountController()
        {

        }

        [HttpPost("register")]
        public ActionResult<AppUser> Register(RegisterDto registerDto)
        {
            using var hmac = new HMACSHA512();

            // Check username is already exists or not
            if( UserExists(registerDto.Username))
            {
                BadRequest("Username is taken");
            }

            // Create the user
            var user = new AppUser
            {
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            Console.WriteLine(user.PasswordHash);

            // Add user to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string stringOfSqlCommandFirst = "INSERT INTO [dbo].[app_users] ([username], [password_hash] ,[password_salt])" + "\n" 
                    +  "VALUES ('" + user.UserName + "', '" + user.PasswordHash + "', '" + user.PasswordSalt + "');";
                

                Console.WriteLine(stringOfSqlCommandFirst);
                
                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }

            //Report the new user to the console
            Console.WriteLine("Username: " + user.UserName);
            Console.WriteLine("PasswordHash: " + user.PasswordHash);

            //Return the new user
            return user;

        }

        private bool UserExists( string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Retrieve the rows that are contains correct language code.
                string stringOfSqlCommandFirst = "SELECT * FROM [dbo].[app_users] " + "WHERE username = '" + username + "';";
                
                Console.WriteLine(stringOfSqlCommandFirst);

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    if(( sqlDataReader["username"].ToString()) == username)
                    {
                        sqlDataReader.Close();
                        return true;
                    }
                }
                sqlDataReader.Close();
            }

            return false;
        }
    }
}