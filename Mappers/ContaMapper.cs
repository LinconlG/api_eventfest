using API_EventFest.Mappers.sql;
using API_EventFest.Models;
using MySql.Data.MySqlClient;

namespace API_EventFest.Mappers {
    public class ContaMapper : BaseMapper {

        public ContaMapper(IConfiguration configuration) : base(configuration) { }

        public async Task CriarConta(Conta conta) {
            try {

                if (conta.Nome.Split(" ").Length <= 1) {
                    throw new Exception("Nome inválido");
                }

                cmd.CommandText = @$"INSERT INTO conta (
                                        conta_nome
                                    , conta_email
                                    , conta_senha
                                    , conta_cpf
                                    , conta_telefone
                                    , conta_nascimento)
                                VALUES (@nome, @email, @senha, @cpf, @telefone, @nascimento)";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@nome", MySqlDbType.VarString);
                cmd.Parameters.Add("@email", MySqlDbType.VarString);
                cmd.Parameters.Add("@senha", MySqlDbType.VarString);
                cmd.Parameters.Add("@cpf", MySqlDbType.VarString);
                cmd.Parameters.Add("@telefone", MySqlDbType.VarString);
                cmd.Parameters.Add("@nascimento", MySqlDbType.Date);
                cmd.Parameters["@nome"].Value = conta.Nome;
                cmd.Parameters["@email"].Value = conta.Email;
                cmd.Parameters["@senha"].Value = conta.Senha;
                cmd.Parameters["@cpf"].Value = conta.CPF;
                cmd.Parameters["@telefone"].Value = conta.Telefone;
                cmd.Parameters["@nascimento"].Value = conta.DataNascimento;


                await conexao.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception g) {

                throw new Exception(g.Message);
            }
            finally {
                await conexao.CloseAsync();
            }
        }
    }
}