using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool IsSystemAdmin(User user)
        => user.Profile == UserProfile.SystemAdmin;

        public bool IsCompanyAdmin(User user, int companyId)
            => user.Profile == UserProfile.SystemAdmin && user.CompanyId == companyId;

        public bool IsAdminOrSysAdmin(int? companyId = null)
        {
            if (Profile == UserProfile.SystemAdmin)
                return true;

            if (Profile == UserProfile.SystemAdmin && companyId.HasValue && CompanyId == companyId)
                return true;

            return false;
        }
    }
}
