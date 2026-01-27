using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioQuestionarioTeste : RepositorioBaseTeste
    {
        private static Questionario CriarQuestionarioValido(string nome = "Questionário")
        {
            return new Questionario(
                nome,
                TipoQuestionario.SondagemEscrita,
                anoLetivo: 2024,
                componenteCurricularId: 1,
                proficienciaId: 1,
                sondagemId: 1,
                modalidadeId: 1,
                serieAno: 5
            );
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarApenasNaoExcluidosOrdenados()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_DeveRetornarApenasNaoExcluidosOrdenados));

            var q1 = CriarQuestionarioValido("B");
            var q2 = CriarQuestionarioValido("A");
            var q3 = CriarQuestionarioValido("Excluido");
            q3.Excluido = true;

            context.Questionarios.AddRange(q1, q2, q3);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionario(context);

            var result = (await repo.ObterTodosAsync()).ToList();

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarQuestionarioQuandoExiste()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_DeveRetornarQuestionarioQuandoExiste));
            var q = CriarQuestionarioValido();

            context.Questionarios.Add(q);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionario(context);

            var result = await repo.ObterPorIdAsync(q.Id);

            Assert.NotNull(result);
            Assert.Equal(q.Id, result!.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNullQuandoNaoExiste()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_DeveRetornarNullQuandoNaoExiste));
            var repo = new RepositorioQuestionario(context);

            var result = await repo.ObterPorIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CriarAsync_DevePersistirERetornarId()
        {
            var context = CriarContexto(nameof(CriarAsync_DevePersistirERetornarId));
            var repo = new RepositorioQuestionario(context);

            var q = CriarQuestionarioValido();

            var id = await repo.CriarAsync(q);

            Assert.True(id > 0);
            Assert.Single(context.Questionarios);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarQuandoExiste()
        {
            var context = CriarContexto(nameof(AtualizarAsync_DeveAtualizarQuandoExiste));
            var q = CriarQuestionarioValido();

            context.Questionarios.Add(q);
            await context.SaveChangesAsync();

            q.AlteradoEm = DateTime.Now;
            q.AlteradoPor = "teste";
            q.AlteradoRF = "123";

            var repo = new RepositorioQuestionario(context);

            var result = await repo.AtualizarAsync(q);

            Assert.True(result);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalseQuandoNaoExiste()
        {
            var context = CriarContexto(nameof(AtualizarAsync_DeveRetornarFalseQuandoNaoExiste));
            var repo = new RepositorioQuestionario(context);

            var q = CriarQuestionarioValido();

            var result = await repo.AtualizarAsync(q);

            Assert.False(result);
        }

        [Fact]
        public async Task ExcluirAsync_DeveMarcarComoExcluidoQuandoExiste()
        {
            var context = CriarContexto(nameof(ExcluirAsync_DeveMarcarComoExcluidoQuandoExiste));
            var q = CriarQuestionarioValido();

            context.Questionarios.Add(q);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestionario(context);

            var result = await repo.ExcluirAsync(q.Id);

            Assert.True(result);
            Assert.True(q.Excluido);
        }

        [Fact]
        public async Task ExcluirAsync_DeveRetornarFalseQuandoNaoExiste()
        {
            var context = CriarContexto(nameof(ExcluirAsync_DeveRetornarFalseQuandoNaoExiste));
            var repo = new RepositorioQuestionario(context);

            var result = await repo.ExcluirAsync(999);

            Assert.False(result);
        }
    }
}
