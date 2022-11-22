using API_EventFest.Mappers.sql;
using API_EventFest.Models;
using API_EventFest.Models.Enum;
using API_EventFest.Models.Extensions;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API_EventFest.Mappers {
    public class EventoMapper : BaseMapper {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventoMapper(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration) {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task CriarEvento(Evento evento) {

            try {

                var fotoid = await UploadImagem(evento.Foto);

                cmd.CommandText = @$"INSERT INTO conta (
                                      evento_nome
                                    , evento_descricao
                                    , evento_data
                                    , evento_classificacao
                                    , evento_fotoid
                                    , evento_qtdIngresso
                                    , evento_organizador)
                                VALUES (@nome, @descricao, @data, @classificacao, @fotoid, @qtdIngresso, @organizador)";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@nome", MySqlDbType.VarString);
                cmd.Parameters.Add("@descricao", MySqlDbType.VarString);
                cmd.Parameters.Add("@data", MySqlDbType.Date);
                cmd.Parameters.Add("@classificacao", MySqlDbType.Int16);
                cmd.Parameters.Add("@fotoid", MySqlDbType.Int32);
                cmd.Parameters.Add("@qtdIngresso", MySqlDbType.Int32);
                cmd.Parameters.Add("@organizador", MySqlDbType.VarString);

                cmd.Parameters["@nome"].Value = evento.Nome;
                cmd.Parameters["@descricao"].Value = evento.Descricao;
                cmd.Parameters["@data"].Value = evento.Data;
                cmd.Parameters["@classificacao"].Value = evento.IsLivre ? 0 : evento.Classificao;
                cmd.Parameters["@fotoid"].Value = fotoid;
                cmd.Parameters["@qtdIngresso"].Value = evento.QuantidadeIngressos;
                cmd.Parameters["@organizador"].Value = evento.Organizador;

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

        public async Task DeletarEvento(int eventoid) {

            try {
                cmd.CommandText = @$"SELECT evento_fotoid FROM evento
                                WHERE eventoid = @eventoid";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@eventoid", MySqlDbType.Int32);
                cmd.Parameters["@eventoid"].Value = eventoid;
                var dr = await cmd.ReaderQueryAsync();

                while (await dr.ReadAsync()) {
                    //deletarFoto(fotoid)
                }


                cmd.CommandText = @$"DELETE FROM evento
                                WHERE eventoid = @eventoid";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@eventoid", MySqlDbType.Int32);
                cmd.Parameters["@eventoid"].Value = eventoid;
                await cmd.NonQueryAsync();

            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Evento>> FindEventos(int? eventoid = null) {

            try {
                cmd.CommandText = $@"SELECT 
                            evento_nome,
                            evento_descricao,
                            evento_data,
                            evento_classificacao,
                            foto_url,
                            evento_preco,
                            evento_organizador
                        FROM evento
                        INNER JOIN foto ON evento_fotoid = fotoid
                        WHERE evento_data >= @data
                        {(eventoid != null ? "AND eventoid = @eventoid" : " ")}
                        ";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@data", MySqlDbType.Date);
                cmd.Parameters.Add("@eventoid", MySqlDbType.Int32);
                cmd.Parameters["@data"].Value = DateTime.Now.Date;
                cmd.Parameters["@eventoid"].Value = eventoid;

                await conexao.OpenAsync();
                var dr = await cmd.ExecuteReaderAsync();
                var eventos = new List<Evento>();
                while (await dr.ReadAsync()) {
                    var evento = new Evento();

                    evento.Nome = dr.GetString(0);
                    evento.Descricao = dr.GetString(1);
                    evento.Data = dr.GetDateTime(2);
                    evento.Classificao = (Classificacao)dr.GetInt32(3);
                    evento.Foto = new Foto() { Url = dr.GetString(4) };
                    evento.Preco = dr.GetInt32(5);
                    evento.Organizador = dr.GetString(6);

                    eventos.Add(evento);
                }

                return eventos;
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
            finally { 
                await conexao.CloseAsync();
            }
        }

        public async Task<int> UploadImagem(Foto foto) {

            try {
                await conexao.OpenAsync();

                string Filepath;

                cmd.CommandText = "SELECT fotoid FROM foto ORDER BY fotoid desc LIMIT 1";

                var dr = await cmd.ExecuteReaderAsync();
                int fotoid = 0;
                while (await dr.ReadAsync()) {
                    fotoid = dr.GetInt32(0) + 1;
                }

                if (fotoid == 0) {
                    throw new Exception("ocorreu um erro ao procurar a fotoid");
                }
                Filepath = GetFilePath(fotoid);
                await conexao.CloseAsync();

                if (!Directory.Exists(Filepath)) {
                    Directory.CreateDirectory(Filepath);
                }

                string imagePath = Filepath + "\\imagem.jpg";

                if (File.Exists(imagePath)) {
                    File.Delete(imagePath);
                }

                using (FileStream stream = File.Create(imagePath)) {
                    await foto.arquivo.CopyToAsync(stream);
                    await stream.FlushAsync();
                    cmd.CommandText = "INSERT INTO foto (foto_url) VALUES (@path)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@path", MySqlDbType.VarString);
                    cmd.Parameters["@path"].Value = imagePath;

                    await conexao.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                return fotoid;
            }
            catch (Exception e) {

                throw new Exception(e.Message);
            }
            finally {
                await conexao.CloseAsync();
            }
            
        }

        public async Task<int> FindEventoFotoId(int eventoid) {

            int fotoid = 0;

            cmd.CommandText = "SELECT evento_fotoid FROM evento WHERE eventoid = @eventoid";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@eventoid", MySqlDbType.Int32);
            cmd.Parameters["@eventoid"].Value = eventoid;

            await conexao.OpenAsync();
            var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) {
                fotoid = dr.GetInt32(0);
            }

            if (fotoid == 0) {
                throw new Exception("Verifique se o evento existe, foto não encontrada");
            }

            return fotoid;
        }

        public async Task<string> FindFotoPath(int eventoid) {

            var fotoid = await FindEventoFotoId(eventoid);

            string filePath = GetFilePath(fotoid) + "\\imagem.jpg";

            return filePath;
        }

        private string GetFilePath(int fotoid) {
            return this._webHostEnvironment.WebRootPath + "\\Uploads\\Evento_ilustracao\\" + fotoid.ToString();
        }
    }
}
