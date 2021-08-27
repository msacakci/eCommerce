using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
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
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            using var hmac = new HMACSHA512();

            // Check username is already exists or not
            Console.WriteLine(await UserExists(registerDto.Username));
            if( await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }

            // Create the user
            var user = new AppUser
            {
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            Console.WriteLine(user.PasswordHash);
            Console.WriteLine(hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)));

            string stringOfSqlCommand = "INSERT INTO [dbo].[app_users] ([username], [password_hash] ,[password_salt])" + "\n" 
                    +  "VALUES ('" + user.UserName + "', '" + user.PasswordHash + "', '" + user.PasswordSalt + "');";

            // Add user to the database
            DatabaseHelper databaseHelper = new DatabaseHelper();
            await databaseHelper.InsertToDatabase(connectionString, stringOfSqlCommand);

            //Report the new user to the console
            Console.WriteLine("Username: " + user.UserName);
            Console.WriteLine("PasswordHash: " + user.PasswordHash);

            //Return the new user
            return user;

        }

        private Task<bool> UserExists( string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Retrieve the rows that are contains correct language code.
                string stringOfSqlCommand = "SELECT * FROM [dbo].[app_users] " + "WHERE username = '" + username.ToLower() + "';";

                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommand, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    Console.WriteLine(( sqlDataReader["username"].ToString()).ToLower() + " - " + username.ToLower());
                    if( string.Equals(( sqlDataReader["username"].ToString()).ToLower(), username.ToLower()))
                    {
                        Console.WriteLine("Checkpoint");
                        sqlDataReader.Close();
                        return Task.FromResult(true);
                    }
                }
                sqlDataReader.Close();
            }

            return Task.FromResult(false);
        }
    }
}