using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecureOPS.Applications.DTOs
{
    public class UserDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address is mandatory.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A role must be assigned.")]
        [RegularExpression("^(L1 Analyst|L2 Analyst|Responder|Hunter|Manager|Admin)$",
    ErrorMessage = "Role must be: L1 Analyst, L2 Analyst, Responder, Hunter, Manager, or Admin.")]
        public string Role { get; set; }
    }
}
