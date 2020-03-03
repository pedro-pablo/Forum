using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    public class PostagensController : Controller
    {
        private readonly ContextoDb _contexto;

        public PostagensController()
        {
            _contexto = new ContextoDb();
        }

        [HttpGet]
        public IActionResult Index(int pagina = 1)
        {
            pagina = pagina <= 0 ? 1 : pagina;

            var postagensPagina = _contexto.Postagens
                .Include("Arquivos")
                .Include("Usuario")
                .Where(p => !p.Comentario)
                .OrderByDescending(p => p.Id)
                .ToList();

            ViewData["Sucesso"] = TempData["Sucesso"];
            ViewData["PostagensPagina"] = postagensPagina;
            ViewData["PaginaAtual"] = pagina;

            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Detalhes(int postagemId)
        {
            if (postagemId == 0) return RedirectToAction("Index");

            Postagem postagem = await _contexto.Postagens
                .Include("Arquivos")
                .Include("Comentarios")
                .Include("Usuario")
                .FirstOrDefaultAsync(s => s.Id == postagemId);

            if (postagem == null || postagem.Comentario) return RedirectToAction("Index");

            ViewData["Detalhes"] = true;
            return View(postagem);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Enviar(PostagemNovaViewModel dadosPostagem)
        {
            if (!ModelState.IsValid) return View("Index", dadosPostagem);

            Usuario usuario = await Usuario.ObterUsuarioAsync(User, _contexto);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado. Tente sair e fazer login novamente.");
                return View("Index", dadosPostagem);
            }

            List<string> errosValidacaoArquivos = new List<string>(6);
            foreach (var arquivoFormulario in dadosPostagem.Arquivos)
                Arquivo.ValidarArquivoFormulario(arquivoFormulario, errosValidacaoArquivos);

            if (errosValidacaoArquivos.Count > 0)
            {
                errosValidacaoArquivos.ForEach(erro => ModelState.AddModelError(string.Empty, erro));
                return View("Index", dadosPostagem);
            }

            Postagem postagem;
            try
            {
                postagem = await Postagem.PreencherAsync(dadosPostagem, null, usuario);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível registrar a postagem.\n" + e.Message);
                return View("Index", dadosPostagem);
            }

            await _contexto.Postagens.AddAsync(postagem);

            await _contexto.SaveChangesAsync();

            TempData["Sucesso"] = true;
            return RedirectToAction("Index");
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Comentar(PostagemNovaViewModel dadosComentario)
        {
            ViewData["Comentario"] = dadosComentario.Texto;

            if (!ModelState.IsValid) return RedirectToAction("Detalhes", dadosComentario.PostagemOriginalId);

            Postagem postagemOriginal = await _contexto.Postagens.FindAsync(dadosComentario.PostagemOriginalId);
            if (postagemOriginal == null) return RedirectToAction("Detalhes", dadosComentario.PostagemOriginalId);

            Usuario usuario = await Usuario.ObterUsuarioAsync(User, _contexto);

            Postagem comentario = await Postagem.PreencherAsync(dadosComentario, postagemOriginal, usuario);
            comentario.Comentario = true;

            _contexto.Postagens.Add(comentario);
            await _contexto.SaveChangesAsync();

            return RedirectToAction("Detalhes", new {postagemId = dadosComentario.PostagemOriginalId});
        }

        public async Task<int> ObterQuantidadeComentarios(int postagemId)
        {
            return await (from c in _contexto.Postagens
                where c.PostagemOriginal.Id == postagemId
                select c).CountAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _contexto.Dispose();

            base.Dispose(disposing);
        }
    }
}