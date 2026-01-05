using System.ComponentModel.DataAnnotations;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Extensoes
{
    public class EnumExtensionTeste
    {
        public enum StatusTeste
        {
            [Display(Name = "Ativo", ShortName = "AT", GroupName = "Status Principal")]
            Ativo = 1,

            [Display(Name = "Inativo", ShortName = "IN", GroupName = "Status Principal")]
            Inativo = 2,

            [Display(Name = "Pendente", ShortName = "PD", GroupName = "Status Secundário")]
            Pendente = 3,

            [Display(Name = "Cancelado", ShortName = "CA", GroupName = "Status Secundário")]
            Cancelado = 4
        }

        [Fact]
        public void EhUmDosValores_Deve_retornar_true_quando_valor_esta_na_lista()
        {
            var status = StatusTeste.Ativo;

            var resultado = status.EhUmDosValores(StatusTeste.Ativo, StatusTeste.Inativo);

            Assert.True(resultado);
        }

        [Fact]
        public void EhUmDosValores_Deve_retornar_false_quando_valor_nao_esta_na_lista()
        {
            var status = StatusTeste.Pendente;

            var resultado = status.EhUmDosValores(StatusTeste.Ativo, StatusTeste.Inativo);

            Assert.False(resultado);
        }

        [Fact]
        public void EhUmDosValores_Deve_retornar_true_para_um_unico_valor_correto()
        {
            var status = StatusTeste.Cancelado;

            var resultado = status.EhUmDosValores(StatusTeste.Cancelado);

            Assert.True(resultado);
        }

        [Fact]
        public void EhUmDosValores_Deve_retornar_true_quando_valor_esta_entre_multiplos_valores()
        {
            var status = StatusTeste.Pendente;

            var resultado = status.EhUmDosValores(
                StatusTeste.Ativo, 
                StatusTeste.Inativo, 
                StatusTeste.Pendente, 
                StatusTeste.Cancelado);

            Assert.True(resultado);
        }

        [Theory]
        [InlineData(StatusTeste.Ativo, "AT")]
        [InlineData(StatusTeste.Inativo, "IN")]
        [InlineData(StatusTeste.Pendente, "PD")]
        [InlineData(StatusTeste.Cancelado, "CA")]
        public void ObterNomeCurto_Deve_retornar_nome_curto_do_enum(StatusTeste status, string nomeCurtoEsperado)
        {
            var resultado = status.ObterNomeCurto();

            Assert.Equal(nomeCurtoEsperado, resultado);
        }

        [Theory]
        [InlineData(StatusTeste.Ativo, "Ativo")]
        [InlineData(StatusTeste.Inativo, "Inativo")]
        [InlineData(StatusTeste.Pendente, "Pendente")]
        [InlineData(StatusTeste.Cancelado, "Cancelado")]
        public void ObterNome_Deve_retornar_nome_do_enum(StatusTeste status, string nomeEsperado)
        {
            var resultado = status.ObterNome();

            Assert.Equal(nomeEsperado, resultado);
        }

        [Theory]
        [InlineData(StatusTeste.Ativo, "Status Principal")]
        [InlineData(StatusTeste.Inativo, "Status Principal")]
        [InlineData(StatusTeste.Pendente, "Status Secundário")]
        [InlineData(StatusTeste.Cancelado, "Status Secundário")]
        public void ObterNomeGrupo_Deve_retornar_nome_do_grupo_do_enum(StatusTeste status, string grupoEsperado)
        {
            var resultado = status.ObterNomeGrupo();

            Assert.Equal(grupoEsperado, resultado);
        }

        [Fact]
        public void ObterAtributo_Deve_retornar_atributo_display_corretamente()
        {
            var status = StatusTeste.Ativo;

            var atributo = status.ObterAtributo<DisplayAttribute>();

            Assert.NotNull(atributo);
            Assert.Equal("Ativo", atributo.Name);
            Assert.Equal("AT", atributo.ShortName);
            Assert.Equal("Status Principal", atributo.GroupName);
        }

        [Fact]
        public void ObterAtributo_Deve_retornar_todas_propriedades_do_display_attribute()
        {
            var status = StatusTeste.Cancelado;

            var atributo = status.ObterAtributo<DisplayAttribute>();

            Assert.NotNull(atributo);
            Assert.Equal("Cancelado", atributo.Name);
            Assert.Equal("CA", atributo.ShortName);
            Assert.Equal("Status Secundário", atributo.GroupName);
        }
    }
}