namespace ManiaDeLimpeza.Application.Dtos;
public class LeadCaptureRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsValid(out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            errorMessage = "O nome é obrigatório.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Phone))
        {
            errorMessage = "O telefone é obrigatório.";
            return false;
        }
        errorMessage = null;
        return true;
    }
}