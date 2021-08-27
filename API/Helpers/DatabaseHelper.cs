using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

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
    }
}