using FluentValidation;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.Validators.Bimestre
{
    public class AtualizarBimestreValidator : AbstractValidator<BimestreDto>
    {
        public AtualizarBimestreValidator()
        {
            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição do bimestre é obrigatória")
                .MaximumLength(200).WithMessage("A descrição não pode ter mais de 200 caracteres");

            RuleFor(x => x.CodBimestreEnsinoEol)
                .GreaterThan(0).WithMessage("O código do bimestre no EOL deve ser maior que zero");
        }
    }
}
