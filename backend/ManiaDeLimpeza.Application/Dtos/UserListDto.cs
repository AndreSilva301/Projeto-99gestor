using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Application.Dtos;
public class UserListDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public UserProfile Profile { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive => Profile != UserProfile.Inactive;
}    