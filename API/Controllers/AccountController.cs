using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";
        private readonly ITokenService _tokenService;

        public AccountController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public ActionResult<UserDto> Register(RegisterDto registerDto)
        {

            // Check username is already exists or not
            Console.WriteLine( UserExists(registerDto.Username));
            if( UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }

            // Create the user
            using var hmac = new HMACSHA512();
            AppUser user = new AppUser
            {
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
            };

            string stringOfSqlCommand = "INSERT INTO [dbo].[app_users] ([username], [password_hash], [password_salt])" + "\n" 
                    +  "VALUES ('" + user.UserName + "', '" + BitConverter.ToString(user.PasswordHash).Replace("-","") + "', '" + BitConverter.ToString(user.PasswordSalt).Replace("-","") + "');";


            // Add user to the database
            DatabaseHelper databaseHelper = new DatabaseHelper();
            databaseHelper.InsertToDatabase(connectionString, stringOfSqlCommand);

            //Return the new user
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public ActionResult<UserDto> Login(LoginDto loginDto)
        {
            var user = GetUserFromDatabase(loginDto.Username);

            if(user == null)
            {
                return Unauthorized("Invalid username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int index = 0; index < computeHash.Length; index++)
            {
                if(computeHash[index] != user.PasswordHash[index])
                {
                    // Console.WriteLine("1400 -- " + BitConverter.ToString(computeHash).Replace("-",""));
                    // Console.WriteLine("1401 -- " + BitConverter.ToString(user.PasswordHash).Replace("-",""));

                    return Unauthorized("Invalid password");
                }
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private bool UserExists( string username)
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
                    if( string.Equals(( sqlDataReader["username"].ToString()).ToLower(), username.ToLower()))
                    {
                        Console.WriteLine("Checkpoint");
                        sqlDataReader.Close();
                        return true;
                    }
                }
                sqlDataReader.Close();
            }

            return false;
        }

        private AppUser GetUserFromDatabase( string username)
        {
            var user = new AppUser
            {
                Id = -1,
                UserName = null,
                PasswordHash = null,
                PasswordSalt = null
            };

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
                    user.Id = Convert.ToInt32( sqlDataReader["id"]);

                    user.UserName = sqlDataReader["username"].ToString();     

                    user.PasswordHash = StringToByteArray(sqlDataReader["password_hash"].ToString());

                    user.PasswordSalt = StringToByteArray(sqlDataReader["password_salt"].ToString());
                }
                sqlDataReader.Close();
            }

            if(user.Id == -1)
            {
                return null;
            }
            else
            {
                return user;
            }
        }

        private byte[] StringToByteArray(string hex) 
        {
            return Enumerable.Range(0, hex.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                            .ToArray();
        }
    }
}