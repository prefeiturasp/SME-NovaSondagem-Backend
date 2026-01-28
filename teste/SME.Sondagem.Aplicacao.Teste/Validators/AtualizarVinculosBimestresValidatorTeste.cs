using FluentValidation.TestHelper;
using SME.Sondagem.Aplicacao.Validators.QuestionarioBimestre;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Validators
{
    public class AtualizarVinculosBimestresValidatorTeste
    {
        private readonly AtualizarVinculosBimestresValidator _validator;

        public AtualizarVinculosBimestresValidatorTeste()
        {
            _validator = new AtualizarVinculosBimestresValidator();
        }

        [Fact]
        public void Deve_Ter_Erro_Quando_QuestionarioId_For_Null()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = null,
                BimestreIds = new List<int> { 1 }
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.QuestionarioId)
                .WithErrorMessage("O ID do questionário é obrigatório");
        }

        [Fact]
        public void Deve_Ter_Erro_Quando_QuestionarioId_For_Zero()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 0,
                BimestreIds = new List<int> { 1 }
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.QuestionarioId)
                .WithErrorMessage("O ID do questionário deve ser maior que zero");
        }

        [Fact]
        public void Deve_Ter_Erro_Quando_QuestionarioId_For_Negativo()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = -5,
                BimestreIds = new List<int> { 1 }
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.QuestionarioId);
        }

        [Fact]
        public void Deve_Ter_Erro_Quando_BimestreIds_Contem_Zero()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 1,
                BimestreIds = new List<int> { 1, 0 }
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.BimestreIds)
                .WithErrorMessage("Todos os IDs de bimestre devem ser maiores que zero");
        }

        [Fact]
        public void Deve_Ter_Erro_Quando_BimestreIds_Contem_Numero_Negativo()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 1,
                BimestreIds = new List<int> { -1, 2 }
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.BimestreIds);
        }

        [Fact]
        public void Nao_Deve_Ter_Erro_Quando_Dto_For_Valido()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2, 3 }
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Nao_Deve_Ter_Erro_Quando_BimestreIds_Estiver_Vazio()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int>()
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}