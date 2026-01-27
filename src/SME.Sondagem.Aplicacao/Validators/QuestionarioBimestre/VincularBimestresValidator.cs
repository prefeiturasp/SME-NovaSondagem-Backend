using FluentValidation;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.Aplicacao.Validators.QuestionarioBimestre;

public class VincularBimestresValidator : AbstractValidator<VincularBimestresDto>
{
    public VincularBimestresValidator()
    {
        RuleFor(x => x.QuestionarioId)
            .GreaterThan(0).WithMessage("O ID do questionário deve ser maior que zero");

        RuleFor(x => x.BimestreIds)
            .NotEmpty().WithMessage("Deve haver pelo menos um bimestre para vincular")
            .Must(ids => ids.All(id => id > 0)).WithMessage("Todos os IDs de bimestre devem ser maiores que zero");
    }
}