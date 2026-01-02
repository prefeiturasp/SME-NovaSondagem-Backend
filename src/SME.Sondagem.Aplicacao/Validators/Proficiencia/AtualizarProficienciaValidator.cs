using FluentValidation;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.Validators.Proficiencia
{
    public class AtualizarProficienciaValidator : AbstractValidator<ProficienciaDto>
    {
        public AtualizarProficienciaValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da proficiência é obrigatório")
                .MaximumLength(200).WithMessage("O nome não pode ter mais de 200 caracteres");

            RuleFor(x => x.ComponenteCurricularId)
                .GreaterThan(0).WithMessage("O código do componente curricular deve ser maior que zero");
        }
    }
}
