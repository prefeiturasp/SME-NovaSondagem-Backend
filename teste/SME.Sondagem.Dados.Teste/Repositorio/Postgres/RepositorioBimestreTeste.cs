using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioBimestreTeste : RepositorioBaseTeste
    {
        #region Helpers

        private static Bimestre CriarBimestre(
            int id,
            bool excluido = false,
            string descricao = "1º Bimestre")
        {
            var bimestre = new Bimestre(
                codBimestreEnsinoEol: 1,
                descricao: descricao
            );

            typeof(Bimestre).GetProperty("Id")!.SetValue(bimestre, id);
            typeof(Bimestre).GetProperty("Excluido")!.SetValue(bimestre, excluido);

            return bimestre;
        }

        private static SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem CriarSondagem(int id, string descricao = "Sondagem Teste")
        {
            var sondagem = new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(descricao, DateTime.UtcNow);
            typeof(SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem).GetProperty("Id")!.SetValue(sondagem, id);
            return sondagem;
        }

        private static Questionario CriarQuestionario(int id, int sondagemId, string nome = "Questionario Teste")
        {
            var questionario = new Questionario(
                nome,
                TipoQuestionario.SondagemLeitura,
                anoLetivo: DateTime.UtcNow.Year,
                componenteCurricularId: 1,
                proficienciaId: 1,
                sondagemId: sondagemId
            );

            typeof(Questionario).GetProperty("Id")!.SetValue(questionario, id);
            return questionario;
        }

        private static QuestionarioBimestre CriarQuestionarioBimestre(int id, int questionarioId, int bimestreId, bool excluido = false)
        {
            var qb = new QuestionarioBimestre(questionarioId, bimestreId);
            typeof(QuestionarioBimestre).GetProperty("Id")!.SetValue(qb, id);
            typeof(QuestionarioBimestre).GetProperty("Excluido")!.SetValue(qb, excluido);
            return qb;
        }

        private static SondagemPeriodoBimestre CriarPeriodoBimestre(int id, int sondagemId, int bimestreId, bool excluido = false)
        {
            var periodo = new SondagemPeriodoBimestre(sondagemId, bimestreId, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
            typeof(SondagemPeriodoBimestre).GetProperty("Id")!.SetValue(periodo, id);
            typeof(SondagemPeriodoBimestre).GetProperty("Excluido")!.SetValue(periodo, excluido);
            return periodo;
        }

        #endregion

        #region Construtor

        [Fact]
        public void Construtor_deve_lancar_excecao_quando_contexto_nulo()
        {
            Assert.Throws<NullReferenceException>(() => new RepositorioBimestre(null!,null!,null!));
        }

        #endregion

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_deve_retornar_apenas_bimestres_nao_excluidos_ordenados_por_descricao()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_deve_retornar_apenas_bimestres_nao_excluidos_ordenados_por_descricao));

            context.Bimestres.AddRange(
                CriarBimestre(1, descricao: "2º Bimestre"),
                CriarBimestre(2, descricao: "1º Bimestre"),
                CriarBimestre(3, excluido: true)
            );

            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = (await repo.ListarAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal("2º Bimestre", resultado[0].Descricao);
            Assert.Equal("1º Bimestre", resultado[1].Descricao);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_bimestre_quando_existir()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_bimestre_quando_existir));

            context.Bimestres.Add(CriarBimestre(1));
            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado!.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_excluido()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_excluido));

            context.Bimestres.Add(CriarBimestre(1, excluido: true));
            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.Null(resultado);
        }

        #endregion

        #region CriarAsync

        [Fact]
        public async Task CriarAsync_deve_persistir_e_retornar_id()
        {
            var context = CriarContexto(nameof(CriarAsync_deve_persistir_e_retornar_id));
            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var bimestre = new Bimestre(1, "1º Bimestre");

            var id = await repo.SalvarAsync(bimestre);

            Assert.True(id > 0);
            Assert.Single(context.Bimestres);
        }

        #endregion

        #region AtualizarAsync

        [Fact]
        public async Task AtualizarAsync_deve_retornar_false_quando_nao_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_retornar_false_quando_nao_existir));
            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var bimestre = CriarBimestre(99);

            var resultado = await repo.SalvarAsync(bimestre);

            Assert.Equivalent(resultado,0);
        }

        [Fact]
        public async Task AtualizarAsync_deve_atualizar_campos_de_auditoria_quando_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_atualizar_campos_de_auditoria_quando_existir));

            var bimestre = CriarBimestre(1);
            context.Bimestres.Add(bimestre);
            await context.SaveChangesAsync();

            bimestre.AlteradoEm = DateTime.UtcNow;
            bimestre.AlteradoPor = "TESTE";
            bimestre.AlteradoRF = "123";

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.SalvarAsync(bimestre);

            Assert.Equivalent(resultado, 1);
        }

        #endregion

        #region ExcluirAsync

        [Fact]
        public async Task ExcluirAsync_deve_retornar_true_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_retornar_true_quando_nao_existir));
            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.RemoverLogico(1) == 0;

            Assert.True(resultado);
        }

        [Fact]
        public async Task ExcluirAsync_deve_marcar_bimestre_como_excluido()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_marcar_bimestre_como_excluido));

            var bimestre = CriarBimestre(1);
            context.Bimestres.Add(bimestre);
            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.RemoverLogico(1)>0;

            var bimestreExcluido = await context.Bimestres
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == 1);

            Assert.True(resultado);
            Assert.NotNull(bimestreExcluido);
            Assert.True(bimestreExcluido!.Excluido);
        }

        #endregion

        #region ObterBimestresPorQuestionarioIdAsync

        [Fact]
        public async Task ObterBimestresPorQuestionarioIdAsync_deve_retornar_vazio_quando_nao_existirem_bimestres_associados()
        {
            var context = CriarContexto(nameof(ObterBimestresPorQuestionarioIdAsync_deve_retornar_vazio_quando_nao_existirem_bimestres_associados));

            var sondagem = CriarSondagem(1);
            var questionario = CriarQuestionario(10, sondagemId: sondagem.Id);

            context.Sondagens.Add(sondagem);
            context.Questionarios.Add(questionario);

            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.ObterBimestresPorQuestionarioIdAsync(questionario.Id);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterBimestresPorQuestionarioIdAsync_deve_retornar_vazio_quando_nao_existirem_periodos_para_os_bimestres()
        {
            var context = CriarContexto(nameof(ObterBimestresPorQuestionarioIdAsync_deve_retornar_vazio_quando_nao_existirem_periodos_para_os_bimestres));

            var sondagem = CriarSondagem(1);
            var questionario = CriarQuestionario(10, sondagemId: sondagem.Id);
            var bimestre = CriarBimestre(5);

            // associa o questionário ao bimestre, mas não cria SondagemPeriodoBimestre
            var qb = CriarQuestionarioBimestre(100, questionario.Id, bimestre.Id);

            context.Sondagens.Add(sondagem);
            context.Questionarios.Add(questionario);
            context.Bimestres.Add(bimestre);
            context.QuestionariosBimestres.Add(qb);

            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = await repo.ObterBimestresPorQuestionarioIdAsync(questionario.Id);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterBimestresPorQuestionarioIdAsync_deve_retornar_periodos_com_bimestre_incluido()
        {
            var context = CriarContexto(nameof(ObterBimestresPorQuestionarioIdAsync_deve_retornar_periodos_com_bimestre_incluido));

            var sondagem = CriarSondagem(1);
            var questionario = CriarQuestionario(10, sondagemId: sondagem.Id);
            var bimestre = CriarBimestre(5);

            var qb = CriarQuestionarioBimestre(100, questionario.Id, bimestre.Id);
            var periodo = CriarPeriodoBimestre(200, sondagem.Id, bimestre.Id);

            context.Sondagens.Add(sondagem);
            context.Questionarios.Add(questionario);
            context.Bimestres.Add(bimestre);
            context.QuestionariosBimestres.Add(qb);
            context.SondagemPeriodosBimestre.Add(periodo);

            await context.SaveChangesAsync();

            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioBimestre(context, auditoriaMock.Object, contextoBase);

            var resultado = (await repo.ObterBimestresPorQuestionarioIdAsync(questionario.Id)).ToList();

            Assert.Single(resultado);
            Assert.Equal(periodo.Id, resultado[0].Id);
            Assert.NotNull(resultado[0].Bimestre);
            Assert.Equal(bimestre.Id, resultado[0].Bimestre.Id);
        }

        #endregion
    }
}
