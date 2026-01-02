using FluentValidation;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Validators.Questao
{
    public class CriarQuestaoValidator : AbstractValidator<QuestaoDto>
    {
        public CriarQuestaoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da questão é obrigatório")
                .MaximumLength(200).WithMessage("O nome não pode ter mais de 200 caracteres");

            RuleFor(x => x.QuestionarioId)
                .GreaterThan(0).WithMessage("O código do questionário deve ser maior que zero");
        }
    }
}
