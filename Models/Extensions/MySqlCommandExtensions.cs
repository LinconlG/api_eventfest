using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;

namespace API_EventFest.Models.Extensions {
    public static class MySqlCommandExtensions {

        public async static Task NonQueryAsync(this MySqlCommand thisObj) {
            await thisObj.Connection.OpenAsync();
            await thisObj.ExecuteNonQueryAsync();
            await thisObj.Connection.CloseAsync();

        }

    }
}
