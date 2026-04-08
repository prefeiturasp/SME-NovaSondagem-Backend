using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioRespostaAlunoDapper : IRepositorioRespostaAlunoDapper
{
    private readonly string _connectionString;

    public RepositorioRespostaAlunoDapper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SondagemConnection") 
            ?? throw new ArgumentNullException(nameof(configuration), "A string de conexão SondagemConnection não foi encontrada.");
    }

    public async Task AtualizarCamposAsync(int id, string? turmaId, string? ueId, string? dreId, int? anoLetivo, int? modalidadeId, int? anoTurma, int? generoId, int? raca_id)
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
                   raca_cor_id = @Raca_id,
                   alterado_em = CURRENT_TIMESTAMP
             WHERE id = @Id;";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = id,
            TurmaId = turmaId,
            UeId = ueId,
            DreId = dreId,
            AnoLetivo = anoLetivo,
            ModalidadeId = modalidadeId,
            AnoTurma = anoTurma,
            GeneroId = generoId,
            Raca_id = raca_id
        });
    }
}
