using System.ComponentModel.DataAnnotations;

namespace PENTDRIVEApi.DTOs
{
    public class UsuarioRegistroRequest
    {
        public string Nome { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string CNPJ { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
                       ErrorMessage = "A senha deve ter no mínimo 6 caracteres, incluindo uma letra maiúscula, um número e um caractere especial (@$!%*?&).")]
        public string Senha { get; set; } = string.Empty;
    }
}