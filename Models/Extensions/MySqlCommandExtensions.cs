using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace API_EventFest.Models.Extensions {
    public static class MySqlCommandExtensions {

        public async static Task NonQueryAsync(this MySqlCommand thisObj, List<(string name, object value)> parametros = null) {

            thisObj.Parameters.Clear();
            foreach (var item in parametros) {
                int type = 0;
                switch (Type.GetTypeCode(item.value.GetType())) {
                    case TypeCode.String:
                        type = 15;
                        break;
                    case TypeCode.Byte:
                        type = 1;
                        break;
                    case TypeCode.DateTime:
                        type = 12;
                        break;
                    case TypeCode.Int32:
                        type = 3;
                        break;
                }

                if (type == 0) {
                    throw new Exception($"Tipo de dado do {item.name} não tratado");
                }

                thisObj.Parameters.Add($"@{item.name}", (MySqlDbType)type);
                thisObj.Parameters[$"@{item.name}"].Value = item.value;
            }

            await thisObj.Connection.OpenAsync();
            await thisObj.ExecuteNonQueryAsync();
            await thisObj.Connection.CloseAsync();
        }

        public async static Task<DbDataReader> ReaderQueryAsync(this MySqlCommand thisObj, List<(string name, object value)> parametros = null) {

            thisObj.Parameters.Clear();
            foreach (var item in parametros) {
                int type = 0;
                switch (Type.GetTypeCode(item.value.GetType())) {
                    case TypeCode.String:
                        type = 15;
                        break;
                    case TypeCode.Byte:
                        type = 1;
                        break;
                    case TypeCode.DateTime:
                        type = 12;
                        break;
                    case TypeCode.Int32:
                        type = 3;
                        break;
                }

                if (type == 0) {
                    throw new Exception($"Tipo de dado do {item.name} não tratado");
                }

                thisObj.Parameters.Add($"@{item.name}", (MySqlDbType)type);
                thisObj.Parameters[$"@{item.name}"].Value = item.value;
            }

            await thisObj.Connection.OpenAsync();
            var dr = await thisObj.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(dr);
            await thisObj.Connection.CloseAsync();

            return dt.CreateDataReader();
        }
    }
}
