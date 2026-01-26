using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioQuestionarioBimestreTeste : RepositorioBaseTeste
    {
        private static Questionario CriarQuestionario(string sufixo = "1")
        {
            return new Questionario(
                nome: $"Questionário {sufixo}",
                tipo: TipoQuestionario.SondagemEscrita,
                anoLetivo: 2024,
                componenteCurricularId: 1,
                proficienciaId: 1,
                sondagemId: 1,
                modalidadeId: 1,
                serieAno: 1
            );
        }

        private static Bimestre CriarBimestre(int codEol, string descricao = null)
        {
            return new Bimestre(codEol, descricao ?? $"Bimestre {codEol}");
        }

        private static QuestionarioBimestre CriarVinculoValido(int questionarioId, int bimestreId)
        {
            return new QuestionarioBimestre(questionarioId, bimestreId);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarApenasNaoExcluidosOrdenados()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_DeveRetornarApenasNaoExcluidosOrdenados));

            var q1 = CriarQuestionario("1");
            var q2 = CriarQuestionario("2");
            var q3 = CriarQuestionario("3");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);

            context.Questionarios.AddRange(q1, q2, q3);
            context.Bimestres.AddRange(b1, b2);
            await context.SaveChangesAsync();

            var v1 = CriarVinculoValido((int)q2.Id, (int)b2.Id);
            var v2 = CriarVinculoValido((int)q1.Id, (int)b1.Id);
            var v3 = CriarVinculoValido((int)q3.Id, (int)b1.Id);
            v3.Excluido = true;

            context.QuestionariosBimestres.AddRange(v1, v2, v3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterTodosAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal((int)q1.Id, result[0].QuestionarioId);
            Assert.Equal((int)q2.Id, result[1].QuestionarioId);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaVaziaQuandoNaoHouverVinculos()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_DeveRetornarListaVaziaQuandoNaoHouverVinculos));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterTodosAsync()).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveOrdenarPorQuestionarioIdEDepoisPorBimestreId()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_DeveOrdenarPorQuestionarioIdEDepoisPorBimestreId));

            var q1 = CriarQuestionario("1");
            var q2 = CriarQuestionario("2");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);
            var b3 = CriarBimestre(3);

            context.Questionarios.AddRange(q1, q2);
            context.Bimestres.AddRange(b1, b2, b3);
            await context.SaveChangesAsync();

            var v1 = CriarVinculoValido((int)q1.Id, (int)b3.Id);
            var v2 = CriarVinculoValido((int)q1.Id, (int)b1.Id);
            var v3 = CriarVinculoValido((int)q1.Id, (int)b2.Id);
            var v4 = CriarVinculoValido((int)q2.Id, (int)b1.Id);

            context.QuestionariosBimestres.AddRange(v1, v2, v3, v4);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterTodosAsync()).ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal((int)q1.Id, result[0].QuestionarioId);
            Assert.Equal((int)b1.Id, result[0].BimestreId);
            Assert.Equal((int)q1.Id, result[1].QuestionarioId);
            Assert.Equal((int)b2.Id, result[1].BimestreId);
            Assert.Equal((int)q1.Id, result[2].QuestionarioId);
            Assert.Equal((int)b3.Id, result[2].BimestreId);
            Assert.Equal((int)q2.Id, result[3].QuestionarioId);
        }

        [Fact]
        public async Task ObterPorQuestionarioIdAsync_DeveRetornarVinculosDoQuestionario()
        {
            var context = CriarContexto(nameof(ObterPorQuestionarioIdAsync_DeveRetornarVinculosDoQuestionario));

            var q1 = CriarQuestionario("1");
            var q2 = CriarQuestionario("2");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);

            context.Questionarios.AddRange(q1, q2);
            context.Bimestres.AddRange(b1, b2);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var v1 = CriarVinculoValido(questionarioId, (int)b1.Id);
            var v2 = CriarVinculoValido(questionarioId, (int)b2.Id);
            var v3 = CriarVinculoValido((int)q2.Id, (int)b1.Id);

            context.QuestionariosBimestres.AddRange(v1, v2, v3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterPorQuestionarioIdAsync(questionarioId)).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, v => Assert.Equal(questionarioId, v.QuestionarioId));
        }

        [Fact]
        public async Task ObterPorQuestionarioIdAsync_DeveRetornarListaVaziaQuandoNaoHouverVinculos()
        {
            var context = CriarContexto(nameof(ObterPorQuestionarioIdAsync_DeveRetornarListaVaziaQuandoNaoHouverVinculos));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterPorQuestionarioIdAsync(999)).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public async Task ObterPorQuestionarioIdAsync_DeveOrdenarPorBimestreId()
        {
            var context = CriarContexto(nameof(ObterPorQuestionarioIdAsync_DeveOrdenarPorBimestreId));

            var q1 = CriarQuestionario("1");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);
            var b3 = CriarBimestre(3);

            context.Questionarios.Add(q1);
            context.Bimestres.AddRange(b1, b2, b3);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var v1 = CriarVinculoValido(questionarioId, (int)b3.Id);
            var v2 = CriarVinculoValido(questionarioId, (int)b1.Id);
            var v3 = CriarVinculoValido(questionarioId, (int)b2.Id);

            context.QuestionariosBimestres.AddRange(v1, v2, v3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterPorQuestionarioIdAsync(questionarioId)).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal((int)b1.Id, result[0].BimestreId);
            Assert.Equal((int)b2.Id, result[1].BimestreId);
            Assert.Equal((int)b3.Id, result[2].BimestreId);
        }

        [Fact]
        public async Task ObterPorBimestreIdAsync_DeveRetornarVinculosDoBimestre()
        {
            var context = CriarContexto(nameof(ObterPorBimestreIdAsync_DeveRetornarVinculosDoBimestre));

            var q1 = CriarQuestionario("1");
            var q2 = CriarQuestionario("2");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);

            context.Questionarios.AddRange(q1, q2);
            context.Bimestres.AddRange(b1, b2);
            await context.SaveChangesAsync();

            var bimestreId = (int)b1.Id;
            var v1 = CriarVinculoValido((int)q1.Id, bimestreId);
            var v2 = CriarVinculoValido((int)q2.Id, bimestreId);
            var v3 = CriarVinculoValido((int)q1.Id, (int)b2.Id);

            context.QuestionariosBimestres.AddRange(v1, v2, v3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = (await repo.ObterPorBimestreIdAsync(bimestreId)).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, v => Assert.Equal(bimestreId, v.BimestreId));
        }

        [Fact]
        public async Task ExisteVinculoAsync_DeveRetornarTrueQuandoVinculoExiste()
        {
            var context = CriarContexto(nameof(ExisteVinculoAsync_DeveRetornarTrueQuandoVinculoExiste));

            var q1 = CriarQuestionario("1");
            var b1 = CriarBimestre(1);

            context.Questionarios.Add(q1);
            context.Bimestres.Add(b1);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var bimestreId = (int)b1.Id;
            var vinculo = CriarVinculoValido(questionarioId, bimestreId);

            context.QuestionariosBimestres.Add(vinculo);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExisteVinculoAsync(questionarioId, bimestreId);

            Assert.True(result);
        }

        [Fact]
        public async Task ExisteVinculoAsync_DeveRetornarFalseQuandoVinculoNaoExiste()
        {
            var context = CriarContexto(nameof(ExisteVinculoAsync_DeveRetornarFalseQuandoVinculoNaoExiste));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExisteVinculoAsync(999, 999);

            Assert.False(result);
        }

        [Fact]
        public async Task ExisteVinculoAsync_DeveRetornarFalseQuandoVinculoEstaExcluido()
        {
            var context = CriarContexto(nameof(ExisteVinculoAsync_DeveRetornarFalseQuandoVinculoEstaExcluido));

            var q1 = CriarQuestionario("1");
            var b1 = CriarBimestre(1);

            context.Questionarios.Add(q1);
            context.Bimestres.Add(b1);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var bimestreId = (int)b1.Id;
            var vinculo = CriarVinculoValido(questionarioId, bimestreId);
            vinculo.Excluido = true;

            context.QuestionariosBimestres.Add(vinculo);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExisteVinculoAsync(questionarioId, bimestreId);

            Assert.False(result);
        }

        [Fact]
        public async Task CriarMultiplosAsync_DevePersistirVinculos()
        {
            var context = CriarContexto(nameof(CriarMultiplosAsync_DevePersistirVinculos));

            var q1 = CriarQuestionario("1");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);
            var b3 = CriarBimestre(3);

            context.Questionarios.Add(q1);
            context.Bimestres.AddRange(b1, b2, b3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var vinculos = new List<QuestionarioBimestre>
            {
                CriarVinculoValido((int)q1.Id, (int)b1.Id),
                CriarVinculoValido((int)q1.Id, (int)b2.Id),
                CriarVinculoValido((int)q1.Id, (int)b3.Id)
            };

            var result = await repo.CriarMultiplosAsync(vinculos);

            Assert.True(result);
            Assert.Equal(3, await context.QuestionariosBimestres.CountAsync());
        }

        [Fact]
        public async Task CriarMultiplosAsync_DeveRetornarFalseQuandoListaVazia()
        {
            var context = CriarContexto(nameof(CriarMultiplosAsync_DeveRetornarFalseQuandoListaVazia));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.CriarMultiplosAsync(new List<QuestionarioBimestre>());

            Assert.False(result);
        }

        [Fact]
        public async Task CriarMultiplosAsync_DeveRetornarFalseQuandoListaNull()
        {
            var context = CriarContexto(nameof(CriarMultiplosAsync_DeveRetornarFalseQuandoListaNull));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.CriarMultiplosAsync(null!);

            Assert.False(result);
        }

        [Fact]
        public async Task ExcluirPorQuestionarioIdAsync_DeveMarcarTodosComoExcluido()
        {
            var context = CriarContexto(nameof(ExcluirPorQuestionarioIdAsync_DeveMarcarTodosComoExcluido));

            var q1 = CriarQuestionario("1");
            var q2 = CriarQuestionario("2");
            var b1 = CriarBimestre(1);
            var b2 = CriarBimestre(2);

            context.Questionarios.AddRange(q1, q2);
            context.Bimestres.AddRange(b1, b2);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var v1 = CriarVinculoValido(questionarioId, (int)b1.Id);
            var v2 = CriarVinculoValido(questionarioId, (int)b2.Id);
            var v3 = CriarVinculoValido((int)q2.Id, (int)b1.Id);

            context.QuestionariosBimestres.AddRange(v1, v2, v3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExcluirPorQuestionarioIdAsync(questionarioId);

            Assert.True(result);

            context.Entry(v1).Reload();
            context.Entry(v2).Reload();
            context.Entry(v3).Reload();

            Assert.True(v1.Excluido);
            Assert.True(v2.Excluido);
            Assert.False(v3.Excluido);
        }

        [Fact]
        public async Task ExcluirPorQuestionarioIdAsync_DeveRetornarFalseQuandoNaoHouverVinculos()
        {
            var context = CriarContexto(nameof(ExcluirPorQuestionarioIdAsync_DeveRetornarFalseQuandoNaoHouverVinculos));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExcluirPorQuestionarioIdAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task ExcluirPorQuestionarioEBimestreAsync_DeveMarcarVinculoComoExcluido()
        {
            var context = CriarContexto(nameof(ExcluirPorQuestionarioEBimestreAsync_DeveMarcarVinculoComoExcluido));

            var q1 = CriarQuestionario("1");
            var b1 = CriarBimestre(1);

            context.Questionarios.Add(q1);
            context.Bimestres.Add(b1);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var bimestreId = (int)b1.Id;
            var vinculo = CriarVinculoValido(questionarioId, bimestreId);

            context.QuestionariosBimestres.Add(vinculo);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExcluirPorQuestionarioEBimestreAsync(questionarioId, bimestreId);

            Assert.True(result);

            context.Entry(vinculo).Reload();
            Assert.True(vinculo.Excluido);
        }

        [Fact]
        public async Task ExcluirPorQuestionarioEBimestreAsync_DeveRetornarFalseQuandoVinculoNaoExiste()
        {
            var context = CriarContexto(nameof(ExcluirPorQuestionarioEBimestreAsync_DeveRetornarFalseQuandoVinculoNaoExiste));
            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExcluirPorQuestionarioEBimestreAsync(999, 999);

            Assert.False(result);
        }

        [Fact]
        public async Task ExcluirPorQuestionarioEBimestreAsync_DeveRetornarFalseQuandoVinculoJaEstaExcluido()
        {
            var context = CriarContexto(nameof(ExcluirPorQuestionarioEBimestreAsync_DeveRetornarFalseQuandoVinculoJaEstaExcluido));

            var q1 = CriarQuestionario("1");
            var b1 = CriarBimestre(1);

            context.Questionarios.Add(q1);
            context.Bimestres.Add(b1);
            await context.SaveChangesAsync();

            var questionarioId = (int)q1.Id;
            var bimestreId = (int)b1.Id;
            var vinculo = CriarVinculoValido(questionarioId, bimestreId);
            vinculo.Excluido = true;

            context.QuestionariosBimestres.Add(vinculo);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionarioBimestre(context);

            var result = await repo.ExcluirPorQuestionarioEBimestreAsync(questionarioId, bimestreId);

            Assert.False(result);
        }
    }
}