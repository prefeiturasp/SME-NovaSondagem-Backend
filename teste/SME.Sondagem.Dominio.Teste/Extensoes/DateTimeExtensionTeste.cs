using Xunit;

namespace SME.Sondagem.Dominio.Teste.Extensoes
{
    public class DateTimeExtensionTeste
    {
        [Fact]
        public void Local_Deve_retornar_mesma_data()
        {
            var data = new DateTime(2026, 1, 5, 10, 30, 0);

            var resultado = data.Local();

            Assert.Equal(data, resultado);
        }

        [Fact]
        public void HorarioBrasilia_Deve_retornar_horario_com_GMT_menos_3()
        {
            var horaUtc = DateTime.UtcNow;

            var horarioBrasilia = DateTimeExtension.HorarioBrasilia();

            var diferencaEsperada = horaUtc.AddHours(-3);
            Assert.True((horarioBrasilia - diferencaEsperada).TotalSeconds < 1);
        }

        [Theory]
        [InlineData("2026-01-04", "2026-01-04")]
        [InlineData("2026-01-05", "2026-01-04")]
        [InlineData("2026-01-06", "2026-01-04")]
        [InlineData("2026-01-07", "2026-01-04")]
        [InlineData("2026-01-08", "2026-01-04")]
        [InlineData("2026-01-09", "2026-01-04")]
        [InlineData("2026-01-10", "2026-01-04")]
        public void ObterDomingo_Deve_retornar_domingo_da_semana(string dataEntrada, string domingoEsperado)
        {
            var data = DateTime.Parse(dataEntrada);
            var esperado = DateTime.Parse(domingoEsperado).Date;

            var resultado = data.ObterDomingo();

            Assert.Equal(esperado, resultado);
            Assert.Equal(DayOfWeek.Sunday, resultado.DayOfWeek);
        }

        [Theory]
        [InlineData("2026-01-10", "2026-01-10")]
        [InlineData("2026-01-04", "2026-01-10")]
        [InlineData("2026-01-05", "2026-01-10")]
        [InlineData("2026-01-06", "2026-01-10")]
        [InlineData("2026-01-07", "2026-01-10")]
        [InlineData("2026-01-08", "2026-01-10")]
        [InlineData("2026-01-09", "2026-01-10")]
        public void ObterSabado_Deve_retornar_sabado_da_semana(string dataEntrada, string sabadoEsperado)
        {
            var data = DateTime.Parse(dataEntrada);
            var esperado = DateTime.Parse(sabadoEsperado);

            var resultado = data.ObterSabado();

            Assert.Equal(esperado.Date, resultado.Date);
            Assert.Equal(DayOfWeek.Saturday, resultado.DayOfWeek);
        }

        [Theory]
        [InlineData("2026-01-04", true)]
        [InlineData("2026-01-10", true)]
        [InlineData("2026-01-05", false)]
        [InlineData("2026-01-06", false)]
        [InlineData("2026-01-07", false)]
        [InlineData("2026-01-08", false)]
        [InlineData("2026-01-09", false)]
        public void FimDeSemana_Deve_identificar_sabado_e_domingo(string dataEntrada, bool esperado)
        {
            var data = DateTime.Parse(dataEntrada);

            var resultado = data.FimDeSemana();

            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("2026-01-04", true)]
        [InlineData("2026-01-05", false)]
        [InlineData("2026-01-06", false)]
        [InlineData("2026-01-07", false)]
        [InlineData("2026-01-08", false)]
        [InlineData("2026-01-09", false)]
        [InlineData("2026-01-10", false)]
        public void Domingo_Deve_identificar_apenas_domingo(string dataEntrada, bool esperado)
        {
            var data = DateTime.Parse(dataEntrada);

            var resultado = data.Domingo();

            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        [InlineData(5, 1)]
        [InlineData(6, 1)]
        [InlineData(7, 2)]
        [InlineData(8, 2)]
        [InlineData(9, 2)]
        [InlineData(10, 2)]
        [InlineData(11, 2)]
        [InlineData(12, 2)]
        public void Semestre_Deve_retornar_semestre_correto(int mes, int semestreEsperado)
        {
            var data = new DateTime(2026, mes, 1);

            var resultado = data.Semestre();

            Assert.Equal(semestreEsperado, resultado);
        }

        [Theory]
        [InlineData("2026-01-09", 1, "2026-01-08")]
        [InlineData("2026-01-09", 2, "2026-01-07")]
        [InlineData("2026-01-09", 3, "2026-01-06")]
        [InlineData("2026-01-09", 4, "2026-01-05")]
        [InlineData("2026-01-09", 5, "2026-01-04")]
        [InlineData("2026-01-11", 1, "2026-01-08")]
        [InlineData("2026-01-12", 1, "2026-01-11")]
        public void DiaRetroativo_Deve_calcular_dias_uteis_anteriores(string dataEntrada, int diasRetroativos, string dataEsperada)
        {
            var data = DateTime.Parse(dataEntrada);
            var esperado = DateTime.Parse(dataEsperada);

            var resultado = data.DiaRetroativo(diasRetroativos);

            Assert.Equal(esperado.Date, resultado.Date);
        }

        [Fact]
        public void DiaRetroativo_Deve_retornar_data_anterior_quando_zero_dias()
        {
            var data = new DateTime(2026, 1, 9);

            var resultado = data.DiaRetroativo(0);

            Assert.Equal(data.Date, resultado.Date);
        }

        [Fact]
        public void EhAnoAtual_Deve_retornar_true_para_ano_atual()
        {
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var data = new DateTime(anoAtual, 6, 15);

            var resultado = data.EhAnoAtual();

            Assert.True(resultado);
        }

        [Fact]
        public void EhAnoAtual_Deve_retornar_false_para_ano_diferente()
        {
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var data = new DateTime(anoAtual - 1, 6, 15);

            var resultado = data.EhAnoAtual();

            Assert.False(resultado);
        }

        [Fact]
        public void ObterDomingo_Deve_retornar_data_sem_hora()
        {
            var data = new DateTime(2026, 1, 7, 15, 30, 45);

            var resultado = data.ObterDomingo();

            Assert.Equal(TimeSpan.Zero, resultado.TimeOfDay);
        }
    }
}