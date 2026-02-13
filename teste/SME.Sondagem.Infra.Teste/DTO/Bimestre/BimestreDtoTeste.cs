using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.Bimestre
{
    public class BimestreDtoTeste
    {
        [Fact]
        public void Deve_Criar_BimestreDto_Com_Propriedades_Padrao()
        {
            var dto = new BimestreDto();

            Assert.Null(dto.Id);
            Assert.Equal(0, dto.CodBimestreEnsinoEol);
            Assert.Equal(string.Empty, dto.Descricao);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Id()
        {
            var dto = new BimestreDto();
            var id = 5;

            dto.Id = id;

            Assert.Equal(id, dto.Id);
        }

        [Fact]
        public void Deve_Definir_E_Obter_CodBimestreEnsinoEol()
        {
            var dto = new BimestreDto();
            var codigo = 1;

            dto.CodBimestreEnsinoEol = codigo;

            Assert.Equal(codigo, dto.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Deve_Definir_E_Obter_Descricao()
        {
            var dto = new BimestreDto();
            var descricao = "1º Bimestre";

            dto.Descricao = descricao;

            Assert.Equal(descricao, dto.Descricao);
        }

        [Fact]
        public void Deve_Aceitar_CodBimestreEnsinoEol_Negativo()
        {
            var dto = new BimestreDto();
            var codigoNegativo = -1;

            dto.CodBimestreEnsinoEol = codigoNegativo;

            Assert.Equal(codigoNegativo, dto.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Deve_Aceitar_CodBimestreEnsinoEol_Com_Valor_Alto()
        {
            var dto = new BimestreDto();
            var codigoAlto = int.MaxValue;

            dto.CodBimestreEnsinoEol = codigoAlto;

            Assert.Equal(codigoAlto, dto.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Deve_Aceitar_Descricao_Vazia()
        {
            var dto = new BimestreDto
            {
                Descricao = string.Empty
            };

            Assert.Equal(string.Empty, dto.Descricao);
        }

        [Fact]
        public void Deve_Aceitar_Descricao_Com_Espacos_Em_Branco()
        {
            var dto = new BimestreDto();
            var descricaoComEspacos = "   ";

            dto.Descricao = descricaoComEspacos;

            Assert.Equal(descricaoComEspacos, dto.Descricao);
        }

        [Fact]
        public void Deve_Aceitar_Descricao_Com_Caracteres_Especiais()
        {
            var dto = new BimestreDto();
            var descricaoComCaracteres = "1º Bimestre - 2024/2025";

            dto.Descricao = descricaoComCaracteres;

            Assert.Equal(descricaoComCaracteres, dto.Descricao);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_Descricao()
        {
            var dto = new BimestreDto
            {
                Descricao = "Descrição inicial"
            };

            var novaDescricao = "Descrição alterada";
            dto.Descricao = novaDescricao;

            Assert.Equal(novaDescricao, dto.Descricao);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_CodBimestreEnsinoEol()
        {
            var dto = new BimestreDto
            {
                CodBimestreEnsinoEol = 1
            };

            var novoCodigo = 4;
            dto.CodBimestreEnsinoEol = novoCodigo;

            Assert.Equal(novoCodigo, dto.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Deve_Definir_Todas_As_Propriedades_Via_Inicializador()
        {
            var id = 10;
            var codigo = 2;
            var descricao = "2º Bimestre";

            var dto = new BimestreDto
            {
                Id = id,
                CodBimestreEnsinoEol = codigo,
                Descricao = descricao
            };

            Assert.Equal(id, dto.Id);
            Assert.Equal(codigo, dto.CodBimestreEnsinoEol);
            Assert.Equal(descricao, dto.Descricao);
        }

        [Fact]
        public void Deve_Herdar_De_BaseDto()
        {
            var dto = new BimestreDto();

            Assert.IsAssignableFrom<BaseDto>(dto);
        }

        [Fact]
        public void Deve_Aceitar_Descricao_Com_Texto_Longo()
        {
            var dto = new BimestreDto();
            var descricaoLonga = new string('A', 500);

            dto.Descricao = descricaoLonga;

            Assert.Equal(descricaoLonga, dto.Descricao);
        }
    }
}