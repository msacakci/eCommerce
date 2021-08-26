using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace API.Helpers
{
    public class GetLanguagesHelper
    {
        public GetLanguagesHelper()
        {

        }

        public List<string> GetLanguagesFromDatabase(string connectionString)
        {
            // Step 1: Create an array list that will keep the all of the possible language codes.
            List<string> languageCodes = new List<string>();

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

            return languageCodes;
        }
    }
}