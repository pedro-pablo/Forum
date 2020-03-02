using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class UsuarioLoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obrigatório.")]
        [MinLength(5, ErrorMessage = "Tamanho mínimo de 5 caracteres.")]
        [MaxLength(15, ErrorMessage = "Tamanho máximo de 15 caracteres")]
        [Display(Name = "Nome de usuário")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [MinLength(6, ErrorMessage = "Tamanho mínimo de 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        public string UrlRetorno { get; set; }
    }
}