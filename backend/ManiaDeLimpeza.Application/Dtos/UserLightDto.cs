using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Application.Dtos;
public class UserLightDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public UserProfile Profile { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive => Profile != UserProfile.Inactive;

    public UserLightDto() { }

    public UserLightDto(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
        Profile = user.Profile;
        CreatedDate = user.CreatedDate;
    }
}    