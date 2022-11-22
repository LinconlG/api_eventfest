using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace API_EventFest.Models.Extensions {
    public static class MySqlCommandExtensions {

        public async static Task NonQueryAsync(this MySqlCommand thisObj) {
            await thisObj.Connection.OpenAsync();
            await thisObj.ExecuteNonQueryAsync();
            await thisObj.Connection.CloseAsync();
        }

        public async static Task<DbDataReader> ReaderQueryAsync(this MySqlCommand thisObj) {
            await thisObj.Connection.OpenAsync();
            var dr = await thisObj.ExecuteReaderAsync();
            await thisObj.Connection.CloseAsync();

            return dr;
        }
    }
}
