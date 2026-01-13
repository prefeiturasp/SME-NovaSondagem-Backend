using SME.Sondagem.Infra.Extensions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Extensions
{
    public class DateTimeExtensionsTeste
    {
        [Fact]
        public void InicioMes_deve_retornar_primeiro_dia_do_mes_em_utc()
        {
            var data = new DateTime(2025, 8, 15, 10, 30, 45, DateTimeKind.Local);

            var inicioMes = data.InicioMes();

            Assert.Equal(1, inicioMes.Day);
            Assert.Equal(0, inicioMes.Hour);
            Assert.Equal(0, inicioMes.Minute);
            Assert.Equal(0, inicioMes.Second);
            Assert.Equal(DateTimeKind.Utc, inicioMes.Kind);
        }

        [Fact]
        public void FinalMes_deve_retornar_ultimo_dia_do_mes_as_23_59_59_em_utc()
        {
            var data = new DateTime(2025, 8, 10);

            var finalMes = data.FinalMes();

            Assert.Equal(31, finalMes.Day);
            Assert.Equal(23, finalMes.Hour);
            Assert.Equal(59, finalMes.Minute);
            Assert.Equal(59, finalMes.Second);
            Assert.Equal(DateTimeKind.Utc, finalMes.Kind);
        }

        [Fact]
        public void FinalMes_deve_respeitar_fevereiro_em_ano_bissexto()
        {
            var data = new DateTime(2024, 2, 5);

            var finalMes = data.FinalMes();

            Assert.Equal(29, finalMes.Day);
            Assert.Equal(23, finalMes.Hour);
            Assert.Equal(59, finalMes.Minute);
            Assert.Equal(59, finalMes.Second);
            Assert.Equal(DateTimeKind.Utc, finalMes.Kind);
        }

        [Fact]
        public void InicioMes_e_FinalMes_devem_manter_mes_e_ano_corretos()
        {
            var data = new DateTime(2023, 12, 20);

            var inicioMes = data.InicioMes();
            var finalMes = data.FinalMes();

            Assert.Equal(12, inicioMes.Month);
            Assert.Equal(12, finalMes.Month);
            Assert.Equal(2023, inicioMes.Year);
            Assert.Equal(2023, finalMes.Year);
        }
    }
}
