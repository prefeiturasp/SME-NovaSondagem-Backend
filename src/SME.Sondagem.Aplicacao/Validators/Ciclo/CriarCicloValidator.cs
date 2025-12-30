using FluentValidation;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.Validators.Ciclo
{
    public class CriarCicloValidator : AbstractValidator<CicloDto>
    {
        public CriarCicloValidator()
        {
            RuleFor(x => x.DescCiclo)
                .NotEmpty().WithMessage("A descrição do ciclo é obrigatória")
                .MaximumLength(200).WithMessage("A descrição não pode ter mais de 200 caracteres");

            RuleFor(x => x.CodCicloEnsinoEol)
                .GreaterThan(0).WithMessage("O código do ciclo no EOL deve ser maior que zero");
        }
    }
}
