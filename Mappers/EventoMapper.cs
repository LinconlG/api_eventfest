using API_EventFest.Mappers.sql;
using API_EventFest.Models;
using API_EventFest.Models.Enum;
using API_EventFest.Models.Extensions;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;

namespace API_EventFest.Mappers {
    public class EventoMapper : BaseMapper {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventoMapper(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration) {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task CriarEvento(Evento evento) {

            try {

                var fotoid = await FindEventoFotoId(evento.EventoID);

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

                await cmd.NonQueryAsync();
            }
            catch (Exception g) {

                throw new Exception(g.Message);
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

                cmd.CommandText = @$"DELETE FROM evento
                                WHERE eventoid = @eventoid";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@eventoid", MySqlDbType.Int32);
                cmd.Parameters["@eventoid"].Value = eventoid;
                await cmd.NonQueryAsync();

                while (await dr.ReadAsync()) {
                    await DeletarFoto(dr.GetInt32(0));
                }
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Evento>> FindEventos(int? eventoid = null) {

            try {
                cmd.CommandText = $@"SELECT 
                            eventoid,
                            evento_nome,
                            evento_descricao,
                            evento_data,
                            evento_classificacao,
                            fotoid,
                            foto_url,
                            evento_preco,
                            evento_organizador
                        FROM evento
                        INNER JOIN foto ON evento_fotoid = fotoid
                        WHERE evento_data >= @data
                        {(eventoid != null ? "AND eventoid = @eventoid" : " ")}
                        ";

                var parametro = new List<(string, object)> {
                ("data", DateTime.Now),
                ("eventoid", eventoid)
                };

                var dr = await cmd.ReaderQueryAsync(parametro);
                var eventos = new List<Evento>();
                while (await dr.ReadAsync()) {
                    var evento = new Evento();

                    evento.EventoID = dr.GetInt32(0);
                    evento.Nome = dr.GetString(1);
                    evento.Descricao = dr.GetString(2);
                    evento.Data = dr.GetDateTime(3);
                    evento.Classificao = (Classificacao)dr.GetInt32(4);
                    evento.Foto = new Foto(await FindFotoPath(evento.EventoID), dr.GetInt32(5), dr.GetString(6));
                    evento.Preco = dr.GetDecimal(7);
                    evento.Organizador = dr.GetString(8);

                    eventos.Add(evento);
                }

                return eventos;
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public async Task EditarEvento(Evento evento) {

            cmd.CommandText = $@"UPDATE evento
                                SET   evento_nome = @nome
                                    , evento_descricao = @descricao
                                    , evento_data = @data
                                    , evento_fotoid = @fotoid
                                WHERE eventoid = @eventoid
                                ";

            var parametros = new List<(string, object)> {
                ("nome", evento.Nome),
                ("descricao", evento.Descricao),
                ("data", evento.Data),
                ("fotoid", evento.Foto.FotoID),
                ("eventoid", evento.EventoID)
            };//mudar endereço tambem

            await cmd.NonQueryAsync(parametros);
        }

        public async Task<int> UploadFoto(IFormFile foto) {

            try {

                cmd.CommandText = "SELECT fotoid FROM foto ORDER BY fotoid desc LIMIT 1";

                var dr = await cmd.ReaderQueryAsync();
                int fotoid = 0;
                while (await dr.ReadAsync()) {
                    fotoid = dr.GetInt32(0) + 1;
                }

                if (fotoid == 0) {
                    throw new Exception("ocorreu um erro ao procurar a fotoid");
                }
                var Filepath = GetFilePath(fotoid);

                if (!Directory.Exists(Filepath)) {
                    Directory.CreateDirectory(Filepath);
                }

                var imagePath = Filepath + "\\imagem.jpg";

                if (File.Exists(imagePath)) {
                    File.Delete(imagePath);
                }

                using (FileStream stream = File.Create(imagePath)) {
                    await foto.CopyToAsync(stream);
                    await stream.FlushAsync();
                    cmd.CommandText = "INSERT INTO foto (foto_url) VALUES (@path)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@path", MySqlDbType.VarString);
                    cmd.Parameters["@path"].Value = imagePath;

                    await cmd.ReaderQueryAsync();
                }

                return fotoid;
            }
            catch (Exception e) {

                throw new Exception(e.Message);
            }     
        }

        public async Task DeletarFoto(int fotoid) {

            string directory = GetFilePath(fotoid);
            string imagePath = directory + "\\imagem.jpg";

            if (File.Exists(imagePath)) {
                File.Delete(imagePath);
            }
            if (Directory.Exists(directory)) {
                Directory.Delete(directory);
            }

            cmd.CommandText = @$"DELETE FROM foto
                                WHERE fotoid = @fotoid";

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@fotoid", MySqlDbType.Int32);
            cmd.Parameters["@fotoid"].Value = fotoid;
            await cmd.NonQueryAsync();
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
