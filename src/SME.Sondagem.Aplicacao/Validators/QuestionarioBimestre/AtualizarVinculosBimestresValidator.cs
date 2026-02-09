using FluentValidation;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.Aplicacao.Validators.QuestionarioBimestre;

public class AtualizarVinculosBimestresValidator : AbstractValidator<AtualizarVinculosBimestresDto>
{
    public AtualizarVinculosBimestresValidator()
    {
        RuleFor(x => x.QuestionarioId)
            .NotNull().WithMessage("O ID do questionário é obrigatório")
            .GreaterThan(0).WithMessage("O ID do questionário deve ser maior que zero");

        RuleFor(x => x.BimestreIds)
            .NotNull().WithMessage("A lista de bimestres não pode ser nula")
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("Todos os IDs de bimestre devem ser maiores que zero");
    }
}