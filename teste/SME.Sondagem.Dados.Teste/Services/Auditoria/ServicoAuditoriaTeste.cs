using Moq;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Services.Auditoria;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Services.Auditoria
{
    public class ServicoAuditoriaTeste
    {
        private readonly Mock<IRepositorioAuditoria> repositorioMock;
        private readonly ContextoFake contexto;

        public ServicoAuditoriaTeste()
        {
            repositorioMock = new Mock<IRepositorioAuditoria>();

            contexto = new ContextoFake();
            contexto.AdicionarVariaveis(new Dictionary<string, object>
        {
            { "NomeUsuario", "Usuario Teste" },
            { "RF", "123456" },
            { "Administrador", "true" }
        });
        }

        [Fact]
        public async Task AuditarAsync_DeveSalvarAuditoria_ComPerfilNulo()
        {
            contexto.AdicionarVariaveis(new Dictionary<string, object>
             {
                 { "NomeUsuario", "Usuario Teste" },
                 { "RF", "123456" },
                 { "PerfilUsuario", string.Empty },
                 { "Administrador", "true" }
             });

            var service = new ServicoAuditoria(repositorioMock.Object, contexto);

            await service.AuditarAsync("Questao", 10, "CRIAR");

            repositorioMock.Verify(r =>
                r.Salvar(It.Is<SME.Sondagem.Dominio.Entidades.Auditoria>(a =>
                    a.Entidade == "questao" &&
                    a.Chave == 10 &&
                    a.Acao == "CRIAR" &&
                    a.Usuario == "Usuario Teste" &&
                    a.RF == "123456" &&
                    a.Administrador == "true" &&
                    a.Perfil == null &&
                    a.Id.HasValue
                )),
                Times.Once);
        }

        [Fact]
        public async Task AuditarAsync_DeveSalvarAuditoria_ComPerfilPreenchido()
        {
            var perfilGuid = Guid.NewGuid();

            contexto.AdicionarVariaveis(new Dictionary<string, object>
            {
                { "NomeUsuario", "Usuario Teste" },
                { "RF", "654321" },
                { "PerfilUsuario", perfilGuid.ToString() },
                { "Administrador", "false" }
            });

            var service = new ServicoAuditoria(repositorioMock.Object, contexto);

            await service.AuditarAsync("Aluno", 99, "ALTERAR");

            repositorioMock.Verify(r =>
                r.Salvar(It.Is<SME.Sondagem.Dominio.Entidades.Auditoria>(a =>
                    a.Entidade == "aluno" &&
                    a.Chave == 99 &&
                    a.Acao == "ALTERAR" &&
                    a.Perfil == perfilGuid
                )),
                Times.Once);
        }

        [Fact]
        public async Task AuditarMultiplosAsync_DeveSalvarMultiplasAuditorias()
        {
            var perfilGuid = Guid.NewGuid();

            contexto.AdicionarVariaveis(new Dictionary<string, object>
            {
                { "NomeUsuario", "Usuario Multiplo" },
                { "RF", "999999" },
                { "PerfilUsuario", perfilGuid.ToString() },
                { "Administrador", "true" }
            });

            var service = new ServicoAuditoria(repositorioMock.Object, contexto);
            var chaves = new List<long> { 1, 2, 3 };

            await service.AuditarMultiplosAsync("Resposta", chaves, "EXCLUIR");

            repositorioMock.Verify(r =>
                r.SalvarMultiplos(It.Is<IEnumerable<SME.Sondagem.Dominio.Entidades.Auditoria>>(lista =>
                    lista.Count() == 3 &&
                    lista.All(a =>
                        a.Entidade == "resposta" &&
                        a.Acao == "EXCLUIR" &&
                        a.Usuario == "Usuario Multiplo" &&
                        a.Perfil == perfilGuid &&
                        a.Id.HasValue
                    )
                )),
                Times.Once);
        }
    }
}
