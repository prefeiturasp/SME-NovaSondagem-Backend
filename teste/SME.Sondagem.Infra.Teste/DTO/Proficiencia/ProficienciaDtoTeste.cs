using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.Proficiencia
{
    public class ProficienciaDtoTeste
    {
        [Fact]
        public void Deve_Criar_ProficienciaDto_Com_Propriedades_Padrao()
        {
            var dto = new ProficienciaDto();

            Assert.Null(dto.Id);
            Assert.Equal(string.Empty, dto.Nome);
            Assert.Equal(0, dto.ComponenteCurricularId);
            Assert.Equal(0, dto.ModalidadeId);
            Assert.Null(dto.Modalidade);
            Assert.NotNull(dto.Questionarios);
            Assert.Empty(dto.Questionarios);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Id()
        {
            var dto = new ProficienciaDto();
            var id = 5;

            dto.Id = id;

            Assert.Equal(id, dto.Id);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Nome()
        {
            var dto = new ProficienciaDto();
            var nome = "Proficiência Básica";

            dto.Nome = nome;

            Assert.Equal(nome, dto.Nome);
        }

        [Fact]
        public void Deve_Definir_E_Obter_ComponenteCurricularId()
        {
            var dto = new ProficienciaDto();
            var componenteCurricularId = 10;

            dto.ComponenteCurricularId = componenteCurricularId;

            Assert.Equal(componenteCurricularId, dto.ComponenteCurricularId);
        }

        [Fact]
        public void Deve_Definir_E_Obter_ModalidadeId()
        {
            var dto = new ProficienciaDto();
            var modalidadeId = 3;

            dto.ModalidadeId = modalidadeId;

            Assert.Equal(modalidadeId, dto.ModalidadeId);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Modalidade()
        {
            var dto = new ProficienciaDto();
            var modalidade = "Ensino Fundamental";

            dto.Modalidade = modalidade;

            Assert.Equal(modalidade, dto.Modalidade);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Questionarios()
        {
            var dto = new ProficienciaDto();
            var questionarios = new List<QuestionarioDto>
            {
                new QuestionarioDto { Nome = "Questionário 1" },
                new QuestionarioDto { Nome = "Questionário 2" }
            };

            dto.Questionarios = questionarios;

            Assert.Equal(questionarios, dto.Questionarios);
            Assert.Equal(2, dto.Questionarios.Count);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Vazio()
        {
            var dto = new ProficienciaDto
            {
                Nome = string.Empty
            };

            Assert.Equal(string.Empty, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Com_Espacos_Em_Branco()
        {
            var dto = new ProficienciaDto();
            var nomeComEspacos = "   ";

            dto.Nome = nomeComEspacos;

            Assert.Equal(nomeComEspacos, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Com_Caracteres_Especiais()
        {
            var dto = new ProficienciaDto();
            var nomeComCaracteres = "Proficiência - Nível 1º";

            dto.Nome = nomeComCaracteres;

            Assert.Equal(nomeComCaracteres, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_ComponenteCurricularId_Negativo()
        {
            var dto = new ProficienciaDto();
            var idNegativo = -1;

            dto.ComponenteCurricularId = idNegativo;

            Assert.Equal(idNegativo, dto.ComponenteCurricularId);
        }

        [Fact]
        public void Deve_Aceitar_ComponenteCurricularId_Com_Valor_Alto()
        {
            var dto = new ProficienciaDto();
            var idAlto = int.MaxValue;

            dto.ComponenteCurricularId = idAlto;

            Assert.Equal(idAlto, dto.ComponenteCurricularId);
        }

        [Fact]
        public void Deve_Aceitar_ModalidadeId_Negativo()
        {
            var dto = new ProficienciaDto();
            var idNegativo = -1;

            dto.ModalidadeId = idNegativo;

            Assert.Equal(idNegativo, dto.ModalidadeId);
        }

        [Fact]
        public void Deve_Aceitar_ModalidadeId_Com_Valor_Alto()
        {
            var dto = new ProficienciaDto();
            var idAlto = int.MaxValue;

            dto.ModalidadeId = idAlto;

            Assert.Equal(idAlto, dto.ModalidadeId);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Nula()
        {
            var dto = new ProficienciaDto
            {
                Modalidade = null
            };

            Assert.Null(dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Vazia()
        {
            var dto = new ProficienciaDto();
            var modalidadeVazia = string.Empty;

            dto.Modalidade = modalidadeVazia;

            Assert.Equal(modalidadeVazia, dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Com_Espacos_Em_Branco()
        {
            var dto = new ProficienciaDto();
            var modalidadeComEspacos = "   ";

            dto.Modalidade = modalidadeComEspacos;

            Assert.Equal(modalidadeComEspacos, dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_Questionarios_Vazia()
        {
            var dto = new ProficienciaDto
            {
                Questionarios = new List<QuestionarioDto>()
            };

            Assert.Empty(dto.Questionarios);
        }

        [Fact]
        public void Deve_Adicionar_Questionario_Na_Colecao()
        {
            var dto = new ProficienciaDto();
            var questionario = new QuestionarioDto { Nome = "Questionário Teste" };

            dto.Questionarios.Add(questionario);

            Assert.Single(dto.Questionarios);
            Assert.Contains(questionario, dto.Questionarios);
        }

        [Fact]
        public void Deve_Remover_Questionario_Da_Colecao()
        {
            var questionario = new QuestionarioDto { Nome = "Questionário Teste" };
            var dto = new ProficienciaDto
            {
                Questionarios = new List<QuestionarioDto> { questionario }
            };

            dto.Questionarios.Remove(questionario);

            Assert.Empty(dto.Questionarios);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Id()
        {
            var dto = new ProficienciaDto
            {
                Id = 1
            };

            var novoId = 10;
            dto.Id = novoId;

            Assert.Equal(novoId, dto.Id);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Nome()
        {
            var dto = new ProficienciaDto
            {
                Nome = "Nome inicial"
            };

            var novoNome = "Nome alterado";
            dto.Nome = novoNome;

            Assert.Equal(novoNome, dto.Nome);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_ComponenteCurricularId()
        {
            var dto = new ProficienciaDto
            {
                ComponenteCurricularId = 1
            };

            var novoId = 5;
            dto.ComponenteCurricularId = novoId;

            Assert.Equal(novoId, dto.ComponenteCurricularId);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_ModalidadeId()
        {
            var dto = new ProficienciaDto
            {
                ModalidadeId = 1
            };

            var novoId = 3;
            dto.ModalidadeId = novoId;

            Assert.Equal(novoId, dto.ModalidadeId);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Modalidade()
        {
            var dto = new ProficienciaDto
            {
                Modalidade = "Modalidade inicial"
            };

            var novaModalidade = "Modalidade alterada";
            dto.Modalidade = novaModalidade;

            Assert.Equal(novaModalidade, dto.Modalidade);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Questionarios()
        {
            var dto = new ProficienciaDto
            {
                Questionarios = new List<QuestionarioDto> { new QuestionarioDto { Nome = "Questionário 1" } }
            };

            var novosQuestionarios = new List<QuestionarioDto>
            {
                new QuestionarioDto { Nome = "Questionário A" },
                new QuestionarioDto { Nome = "Questionário B" },
                new QuestionarioDto { Nome = "Questionário C" }
            };
            dto.Questionarios = novosQuestionarios;

            Assert.Equal(3, dto.Questionarios.Count);
        }

        [Fact]
        public void Deve_Definir_Todas_As_Propriedades_Via_Inicializador()
        {
            var id = 10;
            var nome = "Proficiência Avançada";
            var componenteCurricularId = 15;
            var modalidadeId = 2;
            var modalidade = "Ensino Médio";
            var questionarios = new List<QuestionarioDto> { new QuestionarioDto { Nome = "Questionário Principal" } };

            var dto = new ProficienciaDto
            {
                Id = id,
                Nome = nome,
                ComponenteCurricularId = componenteCurricularId,
                ModalidadeId = modalidadeId,
                Modalidade = modalidade,
                Questionarios = questionarios
            };

            Assert.Equal(id, dto.Id);
            Assert.Equal(nome, dto.Nome);
            Assert.Equal(componenteCurricularId, dto.ComponenteCurricularId);
            Assert.Equal(modalidadeId, dto.ModalidadeId);
            Assert.Equal(modalidade, dto.Modalidade);
            Assert.Equal(questionarios, dto.Questionarios);
        }

        [Fact]
        public void Deve_Herdar_De_BaseDto()
        {
            var dto = new ProficienciaDto();

            Assert.IsAssignableFrom<BaseDto>(dto);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Com_Texto_Longo()
        {
            var dto = new ProficienciaDto();
            var nomeLongo = new string('A', 500);

            dto.Nome = nomeLongo;

            Assert.Equal(nomeLongo, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Com_Texto_Longo()
        {
            var dto = new ProficienciaDto();
            var modalidadeLonga = new string('B', 500);

            dto.Modalidade = modalidadeLonga;

            Assert.Equal(modalidadeLonga, dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_Multiplos_Questionarios()
        {
            var dto = new ProficienciaDto();
            var questionarios = new List<QuestionarioDto>
            {
                new QuestionarioDto { Nome = "Questionário 1" },
                new QuestionarioDto { Nome = "Questionário 2" },
                new QuestionarioDto { Nome = "Questionário 3" },
                new QuestionarioDto { Nome = "Questionário 4" },
                new QuestionarioDto { Nome = "Questionário 5" }
            };

            dto.Questionarios = questionarios;

            Assert.Equal(5, dto.Questionarios.Count);
        }
    }
}