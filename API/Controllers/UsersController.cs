using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController
    {
        private List<AppUser> appUserList;
        private readonly string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";

        public UsersController()
        {

        }

        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            List<AppUser> appUserList = new List<AppUser>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Console.WriteLine( connection.State == ConnectionState.Open); // For test purposes

                // Retrieve the rows that are contains correct language code.
                string stringOfSqlCommandFirst = "SELECT * FROM [dbo].[app_users];";
                
                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    var appUser = new AppUser();

                    appUser.Id = Convert.ToInt32( sqlDataReader["id"]);
                    appUser.UserName = sqlDataReader["username"].ToString();

                    string userInformation = appUser.Id + ". " + appUser.UserName + "\n";

                    Console.WriteLine(userInformation);

                    appUserList.Add( appUser);
                }

                sqlDataReader.Close();
            }                   
            return appUserList;
        }

        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUser(int id)
        {
            var appUser = new AppUser();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                // Retrieve the rows that are contains correct language code.
                string stringOfSqlCommandFirst = "SELECT * FROM [dbo].[app_users] " + "WHERE id = " + id + ";";
                
                SqlCommand sqlCommand = new SqlCommand(stringOfSqlCommandFirst, connection);
                
                sqlCommand.CommandType = CommandType.Text;

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while( sqlDataReader.Read())
                {
                    appUser.Id = Convert.ToInt32( sqlDataReader["id"]);
                    appUser.UserName = sqlDataReader["username"].ToString();

                    string userInformation = appUser.Id + ". " + appUser.UserName + "\n";

                    Console.WriteLine(userInformation);
                }

                sqlDataReader.Close();
            }  

            return appUser;                
        }
    }
}