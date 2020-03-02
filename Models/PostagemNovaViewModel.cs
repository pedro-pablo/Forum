using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Forum.Models
{
    public class PostagemNovaViewModel
    {
        [Display(Name = "Título (opcional)")]
        [DataType(DataType.Text)]
        public string Titulo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obrigatório.")]
        [DataType(DataType.MultilineText)]
        [StringLength(2500, ErrorMessage = "Tamanho máximo de 2500 caracteres.")]
        public string Texto { get; set; }

        public int PostagemOriginalId { get; set; }

        public List<IFormFile> Arquivos { get; set; } = new List<IFormFile>(3);
    }
}