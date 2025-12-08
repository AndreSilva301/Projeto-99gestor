using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CompanyId is required")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(100)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Profile is required")]
        public UserProfile Profile { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsSystemAdmin()
             => Profile == UserProfile.SystemAdmin;

        public bool IsCompanyAdmin(int companyId)
            => Profile == UserProfile.Admin && CompanyId == companyId;

        public bool IsAdminOrSysAdmin(int companyId)
        {
            if (Profile == UserProfile.SystemAdmin)
                return true;

            if (Profile == UserProfile.Admin && CompanyId == companyId)
                return true;

            return false;
        }
    }
}
