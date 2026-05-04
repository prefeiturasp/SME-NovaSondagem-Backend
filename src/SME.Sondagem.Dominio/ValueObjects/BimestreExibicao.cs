namespace SME.Sondagem.Dominio.ValueObjects;

/// <summary>
/// Representa um bimestre já adaptado para exibição no relatório consolidado.
/// Desacoplado da entidade de banco — pode ter descrição diferente da original
/// dependendo das regras de negócio da modalidade.
/// </summary>
public record BimestreExibicao(int Id, string Descricao);
