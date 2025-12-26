using FluentValidation;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Aplicacao.Validators.ComponenteCurricular
{
    public class AtualizarComponenteCurricularValidator : AbstractValidator<AtualizarComponenteCurricularDto>
    {
        public AtualizarComponenteCurricularValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do componente curricular é obrigatório")
                .MaximumLength(200).WithMessage("O nome não pode ter mais de 200 caracteres");

            RuleFor(x => x.CodigoEol)
                .GreaterThan(0).WithMessage("O código EOL deve ser maior que zero");

            RuleFor(x => x.Modalidade)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.Modalidade))
                .WithMessage("A modalidade não pode ter mais de 50 caracteres");
        }
    }
}