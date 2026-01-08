using Microsoft.AspNetCore.Http;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using System;
using System.Collections.Generic;
using System.Text;

using SME.Sondagem.Infra.Dtos;

namespace SME.Sondagem.Dados.Repositorio.Postgres
{
    public class RepositorioRespostaAluno : RepositorioBase<RespostaAluno>, IRepositorioRespostaAluno
    {
        public RepositorioRespostaAluno(SondagemDbContext context) : base(context) { }
    }
}
