namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface IProficienciaUseCase
{
    Task<IEnumerable<object>> ObterProficienciasAsync();
}
