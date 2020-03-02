using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Forum.Models
{
    public class Postagem
    {
        private Postagem()
        {
        }

        public int Id { get; set; }

        [NotMapped] public string IdFormatado => Id.ToString().PadLeft(7, '0');

        public string Titulo { get; set; }

        public string Texto { get; set; }

        public DateTime? Publicacao { get; set; }

        public Usuario Usuario { get; set; }

        public List<Arquivo> Arquivos { get; set; } = new List<Arquivo>(3);

        public List<Postagem> Comentarios { get; set; }

        public Postagem PostagemOriginal { get; set; }

        public bool Comentario { get; set; }

        public static async Task<Postagem> PreencherAsync(PostagemNovaViewModel novaViewModel,
            Postagem postagemOriginal, Usuario usuario)
        {
            Postagem novaPostagem = new Postagem
            {
                Titulo = novaViewModel.Titulo,
                Texto = novaViewModel.Texto,
                PostagemOriginal = postagemOriginal,
                Usuario = usuario,
                Publicacao = DateTime.UtcNow
            };

            // Vincula os arquivos de uma postagem original (ignorando arquivos enviados em comentários)
            if (postagemOriginal == null)
                foreach (var arquivo in novaViewModel.Arquivos)
                    if (Arquivo.FormatosPermitidos.ContainsKey(arquivo.ContentType))
                        await novaPostagem.VincularArquivo(arquivo);

            return novaPostagem;
        }

        private async Task VincularArquivo(IFormFile arquivoFormulario)
        {
            byte[] conteudoArquivo = new byte[arquivoFormulario.Length];
            await arquivoFormulario.OpenReadStream().ReadAsync(conteudoArquivo, 0, (int) arquivoFormulario.Length);

            Guid arquivoId = Guid.NewGuid();
            string nomeArquivo = arquivoId.ToString() + '.' + Arquivo.FormatosPermitidos[arquivoFormulario.ContentType];
            string nomeThumbnail = arquivoId + ".bmp";

            Arquivo novoArquivo = new Arquivo
            {
                Id = arquivoId,
                NomeEnviado = arquivoFormulario.FileName,
                NomeArquivo = nomeArquivo,
                NomeThumbnail = nomeThumbnail,
                CaminhoThumbnail = $"{Environment.CurrentDirectory}\\wwwroot\\userfiles\\thumbnails\\{nomeThumbnail}",
                CaminhoCompleto = $"{Environment.CurrentDirectory}\\wwwroot\\userfiles\\{nomeArquivo}",
                Postagem = this,
                Hash = Hashing.GerarHashArquivo(conteudoArquivo),
                Removido = false
            };

            if (arquivoFormulario.ContentType.Contains("video"))
                novoArquivo.Tipo = TipoArquivo.Video;
            else if (arquivoFormulario.ContentType.Contains("audio"))
                novoArquivo.Tipo = TipoArquivo.Audio;
            else
                novoArquivo.Tipo = TipoArquivo.Imagem;

            Arquivos.Add(novoArquivo);

            // Salva o arquivo e a thumbnail no diretório de arquivos de usuário no servidor
            using (var streamNovoArquivo = File.Create(novoArquivo.CaminhoCompleto))
            {
                await arquivoFormulario.CopyToAsync(streamNovoArquivo);
                if (novoArquivo.Tipo == TipoArquivo.Imagem)
                    using (Image imagemOriginal = Image.FromStream(streamNovoArquivo))
                    {
                        Image thumbnail = imagemOriginal.GetThumbnailImage(Arquivo.LarguraThumbnail,
                            (int) (imagemOriginal.Height / (float) imagemOriginal.Width * Arquivo.LarguraThumbnail),
                            null, IntPtr.Zero);
                        thumbnail.Save(novoArquivo.CaminhoThumbnail);
                    }

                await streamNovoArquivo.FlushAsync();
            }
        }
    }
}