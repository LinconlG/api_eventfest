using API_EventFest.Mappers.sql;
using API_EventFest.Models;

namespace API_EventFest.Mappers {
    public class EventoMapper  : BaseMapper {

        public EventoMapper(IConfiguration configuration) : base(configuration) { }

        public async Task CriarEvento(Evento evento) {

            cmd.CommandText = @$"INSERT INTO conta (
                                      evento_nome
                                    , evento_descricao
                                    , evento_data
                                    , evento_classificacao
                                    , evento_fotoid
                                    , evento_qtdIngresso
                                    , evento_organizador)
                                VALUES (@nome, @descricao, @data, @classificacao, @fotoid, @qtdIngresso, @organizador)";



        }

    }
}
