using FluentValidation;
using SME.Sondagem.Infrastructure.Dtos.Questao;

namespace SME.Sondagem.Aplicacao.Validators.Questao
{
    public class AtualizarQuestaoValidator : AbstractValidator<QuestaoDto>
    {
        public AtualizarQuestaoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da questão é obrigatório")
                .MaximumLength(200).WithMessage("O nome não pode ter mais de 200 caracteres");

            RuleFor(x => x.QuestionarioId)
                .GreaterThan(0).WithMessage("O código do questionário deve ser maior que zero");
        }
    }
}
