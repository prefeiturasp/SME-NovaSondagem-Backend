using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.ComponenteCurricular
{
    public class AtualizarComponenteCurricularDtoTeste
    {
        [Fact]
        public void Deve_Criar_AtualizarComponenteCurricularDto_Com_Propriedades_Padrao()
        {
            var dto = new AtualizarComponenteCurricularDto();

            Assert.Equal(string.Empty, dto.Nome);
            Assert.Null(dto.Ano);
            Assert.Null(dto.Modalidade);
            Assert.Equal(0, dto.CodigoEol);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Nome()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var nome = "Matemática";

            dto.Nome = nome;

            Assert.Equal(nome, dto.Nome);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Ano()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var ano = 2024;

            dto.Ano = ano;

            Assert.Equal(ano, dto.Ano);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Modalidade()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var modalidade = "Ensino Fundamental";

            dto.Modalidade = modalidade;

            Assert.Equal(modalidade, dto.Modalidade);
        }

        [Fact]
        public void Deve_Definir_E_Obter_CodigoEol()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var codigoEol = 12345;

            dto.CodigoEol = codigoEol;

            Assert.Equal(codigoEol, dto.CodigoEol);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Vazio()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                Nome = string.Empty
            };

            Assert.Equal(string.Empty, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Com_Espacos_Em_Branco()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var nomeComEspacos = "   ";

            dto.Nome = nomeComEspacos;

            Assert.Equal(nomeComEspacos, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Com_Caracteres_Especiais()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var nomeComCaracteres = "Língua Portuguesa - 1º Ano";

            dto.Nome = nomeComCaracteres;

            Assert.Equal(nomeComCaracteres, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Ano_Nulo()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                Ano = null
            };

            Assert.Null(dto.Ano);
        }

        [Fact]
        public void Deve_Aceitar_Ano_Com_Valor_Negativo()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var anoNegativo = -1;

            dto.Ano = anoNegativo;

            Assert.Equal(anoNegativo, dto.Ano);
        }

        [Fact]
        public void Deve_Aceitar_Ano_Com_Valor_Alto()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var anoAlto = 9999;

            dto.Ano = anoAlto;

            Assert.Equal(anoAlto, dto.Ano);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Nula()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                Modalidade = null
            };

            Assert.Null(dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Vazia()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var modalidadeVazia = string.Empty;

            dto.Modalidade = modalidadeVazia;

            Assert.Equal(modalidadeVazia, dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Com_Espacos_Em_Branco()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var modalidadeComEspacos = "   ";

            dto.Modalidade = modalidadeComEspacos;

            Assert.Equal(modalidadeComEspacos, dto.Modalidade);
        }

        [Fact]
        public void Deve_Aceitar_CodigoEol_Negativo()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var codigoNegativo = -1;

            dto.CodigoEol = codigoNegativo;

            Assert.Equal(codigoNegativo, dto.CodigoEol);
        }

        [Fact]
        public void Deve_Aceitar_CodigoEol_Com_Valor_Alto()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var codigoAlto = int.MaxValue;

            dto.CodigoEol = codigoAlto;

            Assert.Equal(codigoAlto, dto.CodigoEol);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Nome()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                Nome = "Nome inicial"
            };

            var novoNome = "Nome alterado";
            dto.Nome = novoNome;

            Assert.Equal(novoNome, dto.Nome);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Ano()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                Ano = 2023
            };

            var novoAno = 2024;
            dto.Ano = novoAno;

            Assert.Equal(novoAno, dto.Ano);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Modalidade()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                Modalidade = "Modalidade inicial"
            };

            var novaModalidade = "Modalidade alterada";
            dto.Modalidade = novaModalidade;

            Assert.Equal(novaModalidade, dto.Modalidade);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_CodigoEol()
        {
            var dto = new AtualizarComponenteCurricularDto
            {
                CodigoEol = 123
            };

            var novoCodigo = 456;
            dto.CodigoEol = novoCodigo;

            Assert.Equal(novoCodigo, dto.CodigoEol);
        }

        [Fact]
        public void Deve_Definir_Todas_As_Propriedades_Via_Inicializador()
        {
            var nome = "Ciências";
            var ano = 2024;
            var modalidade = "Ensino Médio";
            var codigoEol = 98765;

            var dto = new AtualizarComponenteCurricularDto
            {
                Nome = nome,
                Ano = ano,
                Modalidade = modalidade,
                CodigoEol = codigoEol
            };

            Assert.Equal(nome, dto.Nome);
            Assert.Equal(ano, dto.Ano);
            Assert.Equal(modalidade, dto.Modalidade);
            Assert.Equal(codigoEol, dto.CodigoEol);
        }

        [Fact]
        public void Deve_Aceitar_Nome_Com_Texto_Longo()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var nomeLongo = new string('A', 500);

            dto.Nome = nomeLongo;

            Assert.Equal(nomeLongo, dto.Nome);
        }

        [Fact]
        public void Deve_Aceitar_Modalidade_Com_Texto_Longo()
        {
            var dto = new AtualizarComponenteCurricularDto();
            var modalidadeLonga = new string('B', 500);

            dto.Modalidade = modalidadeLonga;

            Assert.Equal(modalidadeLonga, dto.Modalidade);
        }
    }
}