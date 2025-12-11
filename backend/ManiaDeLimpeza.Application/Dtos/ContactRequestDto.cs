namespace ManiaDeLimpeza.Application.Dtos;
public class ContactRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public ContactInterest Interest { get; set; }
    public string Message { get; set; } = string.Empty;
}

public enum ContactInterest
{
    ConsultaGeral,
    Suporte,
    SuporteTecnico,
    Vendas,
    Financeiro,
    Feedback,
    Reclamacao,
    SolicitarFuncionalidade,
    Parceria,
    ProblemaNaConta,
    Agendamento,
    Cancelamento,
    SolicitarOrcamento
}