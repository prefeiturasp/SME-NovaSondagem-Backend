using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionariosUseCase : IObterQuestionariosUseCase
{
    private readonly IRepositorioQuestionario questionarioRepositorio;

    public ObterQuestionariosUseCase(IRepositorioQuestionario questionarioRepositorio)
    {
        this.questionarioRepositorio = questionarioRepositorio;
    }

    public async Task<IEnumerable<QuestionarioDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var questionarios = await questionarioRepositorio.ListarAsync(cancellationToken: cancellationToken);

        return questionarios.Select(p => new QuestionarioDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Tipo = p.Tipo,
            AnoLetivo = p.AnoLetivo,    
            ProficienciaId = p.ProficienciaId,
            SondagemId = p.SondagemId,
            ModalidadeId = p.ModalidadeId,
            SerieAno = p.SerieAno,
            ComponenteCurricularId = p.ComponenteCurricularId,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}