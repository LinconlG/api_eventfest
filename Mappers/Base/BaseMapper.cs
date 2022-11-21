
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace API_EventFest.Mappers.sql {
    public class BaseMapper  {

        public static MySqlConnection conexao { get; set; }
        public static MySqlCommand cmd = new MySqlCommand();

        public BaseMapper(IConfiguration configuration) {
            conexao = new MySqlConnection(configuration.GetConnectionString("eventfest"));
            cmd.Connection = conexao;
        }

    }
}
