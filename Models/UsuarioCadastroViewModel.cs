using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class UsuarioCadastroViewModel : UsuarioLoginViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório.")]
        [MinLength(6, ErrorMessage = "Tamanho mínimo de 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        public string SenhaConfirmacao { get; set; }

        [MaxLength(254, ErrorMessage = "Tamanho máximo de 254 caracteres.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de e-mail inválido.")]
        [Display(Name = "E-mail (opcional)")]
        public string Email { get; set; }

        public bool Termos { get; set; }
    }
}