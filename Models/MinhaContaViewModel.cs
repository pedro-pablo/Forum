using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class MinhaContaViewModel
    {
        public AlterarSenhaViewModel AlterarSenha { get; set; }

        public DesativarContaViewModel DesativarConta { get; set; }
    }

    public class AlterarSenhaViewModel
    {
        [Required]
        [Display(Name = "Senha atual")]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }

        [Required]
        [Display(Name = "Senha nova")]
        [MinLength(6, ErrorMessage = "Tamanho mínimo de 6 caracteres.")]
        [DataType(DataType.Password)]
        public string SenhaNova { get; set; }
    }

    public class DesativarContaViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}