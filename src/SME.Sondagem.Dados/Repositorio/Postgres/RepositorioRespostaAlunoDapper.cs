using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Integracao;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioRespostaAlunoDapper : IRepositorioRespostaAlunoDapper
{
    private readonly string _connectionString;

    public RepositorioRespostaAlunoDapper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SondagemConnection") 
            ?? throw new ArgumentNullException(nameof(configuration), "A string de conexão SondagemConnection não foi encontrada.");
    }

    public async Task AtualizarCamposAsync(ContextoRespostaAlunoDto contexto)
    {
        const string query = @"
            UPDATE resposta_aluno
               SET turma_id = @TurmaId,
                   ue_id = @UeId,
                   dre_id = @DreId,
                   ano_letivo = @AnoLetivo,
                   modalidade_id = @ModalidadeId,
                   ano_turma = @AnoTurma, 
                   genero_sexo_id = @GeneroId,
                   raca_cor_id = @RacaId,
                   pap = @Pap,
                   aee = @Aee,
                   deficiente = @Deficiente,
                   alterado_em = CURRENT_TIMESTAMP
             WHERE id = @RespostaId;";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(query, contexto);
    }
}
