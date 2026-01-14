namespace BancoApi.DTOs;

public record CriarContaRequest(string Nome);

public record DepositoRequest(Guid Id, decimal Valor);

public record TransferenciaRequest(Guid IdOrigem, Guid IdDestino, decimal Valor);

public record ContaResponse(Guid Id, string Nome, decimal Saldo);

public record ExtratoResponse(string Nome, decimal Saldo, List<string> UltimasTransacoes);

public record TransacaoResponse(bool Sucesso, string Mensagem);