using FluentValidation;
using SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using System.Net;

namespace SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;

public class VincularBimestresUseCase : IVincularBimestresUseCase
{
    private readonly IRepositorioQuestionarioBimestre _repositorio;
    private readonly IRepositorioQuestionario _repositorioQuestionario;
    private readonly IRepositorioBimestre _repositorioBimestre;
    private readonly IValidator<VincularBimestresDto> _validatorVincular;
    private readonly IValidator<AtualizarVinculosBimestresDto> _validatorAtualizar;

    public VincularBimestresUseCase(
        IRepositorioQuestionarioBimestre repositorio,
        IRepositorioQuestionario repositorioQuestionario,
        IRepositorioBimestre repositorioBimestre,
        IValidator<VincularBimestresDto> validatorVincular,
        IValidator<AtualizarVinculosBimestresDto> validatorAtualizar)
    {
        _repositorio = repositorio;
        _repositorioQuestionario = repositorioQuestionario;
        _repositorioBimestre = repositorioBimestre;
        _validatorVincular = validatorVincular;
        _validatorAtualizar = validatorAtualizar;
    }

    public async Task<bool> ExecutarAsync(VincularBimestresDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validatorVincular.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var questionarioExiste = await _repositorioQuestionario.ObterPorIdAsync(dto.QuestionarioId, cancellationToken);
        if (questionarioExiste == null)
        {
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, dto.QuestionarioId),
                HttpStatusCode.NotFound);
        }

        var bimestresParaVincular = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>();

        foreach (var bimestreId in dto.BimestreIds.Distinct())
        {
            var bimestreExiste = await _repositorioBimestre.ObterPorIdAsync(bimestreId, cancellationToken);
            if (bimestreExiste == null)
            {
                throw new RegraNegocioException(
                    string.Format(MensagemNegocioComuns.BIMESTRE_NAO_ENCONTRADO, bimestreId),
                    HttpStatusCode.NotFound);
            }

            var vinculoExiste = await _repositorio.ExisteVinculoAsync(dto.QuestionarioId, bimestreId, cancellationToken);
            if (!vinculoExiste)
            {
                bimestresParaVincular.Add(new Dominio.Entidades.Questionario.QuestionarioBimestre(
                    dto.QuestionarioId,
                    bimestreId));
            }
        }

        if (bimestresParaVincular.Count == 0)
        {
            throw new RegraNegocioException(
                "Todos os bimestres informados já estão vinculados a este questionário",
                HttpStatusCode.Conflict);
        }

        return await _repositorio.CriarMultiplosAsync(bimestresParaVincular, cancellationToken);
    }

    public async Task<bool> ExecutarAtualizacaoAsync(AtualizarVinculosBimestresDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validatorAtualizar.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var questionarioExiste = await _repositorioQuestionario.ObterPorIdAsync(dto.QuestionarioId!.Value, cancellationToken);
        if (questionarioExiste == null)
        {
            throw new RegraNegocioException(
                string.Format(MensagemNegocioComuns.QUESTIONARIO_NAO_ENCONTRADO, dto.QuestionarioId),
                HttpStatusCode.NotFound);
        }

        if (dto.BimestreIds == null || dto.BimestreIds.Count == 0)
        {
            return await _repositorio.ExcluirPorQuestionarioIdAsync(dto.QuestionarioId.Value, cancellationToken);
        }

        foreach (var bimestreId in dto.BimestreIds.Distinct())
        {
            var bimestreExiste = await _repositorioBimestre.ObterPorIdAsync(bimestreId, cancellationToken);
            if (bimestreExiste == null)
            {
                throw new RegraNegocioException(
                    string.Format(MensagemNegocioComuns.BIMESTRE_NAO_ENCONTRADO, bimestreId),
                    HttpStatusCode.NotFound);
            }
        }

        var vinculosAtuais = await _repositorio.ObterPorQuestionarioIdAsync(dto.QuestionarioId.Value, cancellationToken);
        var bimestresAtuais = vinculosAtuais.Select(v => v.BimestreId).ToList();
        var bimestresNovos = dto.BimestreIds.Distinct().ToList();

        var bimestresParaAdicionar = bimestresNovos.Except(bimestresAtuais).ToList();
        var bimestresParaRemover = bimestresAtuais.Except(bimestresNovos).ToList();

        foreach (var bimestreId in bimestresParaRemover)
        {
            await _repositorio.ExcluirPorQuestionarioEBimestreAsync(dto.QuestionarioId.Value, bimestreId, cancellationToken);
        }

        if (bimestresParaAdicionar.Count > 0)
        {
            var novosVinculos = bimestresParaAdicionar.Select(bimestreId =>
                new Dominio.Entidades.Questionario.QuestionarioBimestre(dto.QuestionarioId.Value, bimestreId)
            ).ToList();

            await _repositorio.CriarMultiplosAsync(novosVinculos, cancellationToken);
        }

        return true;
    }
}