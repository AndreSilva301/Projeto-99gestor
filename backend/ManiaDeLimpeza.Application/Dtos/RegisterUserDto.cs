using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos
{
    public class RegisterUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;   
        public string Company { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        
    }
}
