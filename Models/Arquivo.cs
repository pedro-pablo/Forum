using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Forum.Models
{
    public class Arquivo
    {
        [NotMapped] public const int TamanhoMaximoBytes = 5242880;

        [NotMapped] public const int LarguraThumbnail = 200;

        [NotMapped] public static readonly Dictionary<string, string> FormatosPermitidos =
            new Dictionary<string, string>
            {
                {"image/png", "png"},
                {"image/jpeg", "jpg"},
                {"image/gif", "gif"},
                {"video/mp4", "mp4"},
                {"video/webm", "webm"},
                {"audio/mp3", "mp3"},
                {"audio/ogg", "ogg"},
                {"audio/webm", "webm"}
            };

        public Guid Id { get; set; }

        public string NomeEnviado { get; set; }

        public string NomeThumbnail { get; set; }

        public string CaminhoThumbnail { get; set; }

        public string CaminhoCompleto { get; set; }

        public string NomeArquivo { get; set; }

        public Postagem Postagem { get; set; }

        public bool Removido { get; set; }

        public byte[] Hash { get; set; }

        public TipoArquivo Tipo { get; set; }

        public static void ValidarArquivoFormulario(IFormFile arquivo, List<string> listaErros)
        {
            if (listaErros == null)
                throw new ArgumentNullException(nameof(listaErros), "Lista de erros deve ser inicializada.");

            if (!FormatosPermitidos.ContainsKey(arquivo.ContentType))
                listaErros.Add($"Arquivo \"{arquivo.FileName}\" não é permitido.");

            if (arquivo.Length > TamanhoMaximoBytes)
                listaErros.Add($"Arquivo \"{arquivo.FileName}\" excede o limite de tamanho.");

            //TODO: Adicionar validação da hash do arquivo enviado
        }
    }

    public enum TipoArquivo
    {
        Audio,
        Imagem,
        Video
    }
}