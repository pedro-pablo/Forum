using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ContextoDb _contexto;

        public UsuarioController()
        {
            _contexto = new ContextoDb();
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string urlRetorno)
        {
            ViewData["UrlRetorno"] = urlRetorno;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Cadastro(UsuarioCadastroViewModel cadastroViewModel)
        {
            if (!ModelState.IsValid) return View("Cadastro", cadastroViewModel);

            List<string> resultadoValidacao = Usuario.ValidarCadastro(cadastroViewModel);

            if (resultadoValidacao.Count == 0)
            {
                Usuario usuarioExistente = _contexto.Usuarios.FirstOrDefault(u => u.Nome == cadastroViewModel.Nome);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError(string.Empty, "Já existe um usuário cadastrado com esse nome.");
                    return View("Cadastro", cadastroViewModel);
                }

                Usuario novoUsuario = new Usuario(cadastroViewModel);

                _contexto.Usuarios.Add(novoUsuario);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                foreach (string resultado in resultadoValidacao) 
                    ModelState.AddModelError(string.Empty, resultado);

                return View("Cadastro", cadastroViewModel);
            }

            return RedirectToAction("Login");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View("Login", loginViewModel);

            bool loginOk = false;
            Usuario usuario = _contexto.Usuarios.FirstOrDefault(u => u.Nome == loginViewModel.Nome);
            bool usuarioExiste = usuario != null;

            if (usuarioExiste) loginOk = usuario.ValidarLogin(loginViewModel);

            if (loginOk)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                };

                var userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                await _contexto.SaveChangesAsync();
                if (loginViewModel.UrlRetorno != null) return LocalRedirect(loginViewModel.UrlRetorno);

                return RedirectToAction("Index", "Postagens");
            }

            if (usuarioExiste)
                ModelState.AddModelError(string.Empty,
                    usuario.Desativado
                        ? "Conta desativada permanentemente pelo usuário."
                        : "Nome ou senha incorretos.");
            else
                ModelState.AddModelError(string.Empty, "Nome ou senha incorretos.");


            return View("Login", loginViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Deslogar()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MinhasPostagens()
        {
            Usuario usuario = await Usuario.ObterUsuarioAsync(User, _contexto);
            if (usuario == null) return RedirectToAction("Index", "Home");

            var postagens = (from p in _contexto.Postagens
                where p.Usuario.Id == usuario.Id
                orderby p.Publicacao descending
                select p).ToList();

            ViewData["Postagens"] = postagens;
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MinhaConta(MinhaContaViewModel contaViewModel = null,
            bool? senhaAlterada = null)
        {
            Usuario usuario = await Usuario.ObterUsuarioAsync(User, _contexto);
            if (usuario == null) return RedirectToAction("Index", "Home");

            if (senhaAlterada.HasValue) ViewData["SenhaAlterada"] = senhaAlterada.Value;

            return View(contaViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AlterarSenha(MinhaContaViewModel contaViewModel)
        {
            Usuario usuario = await Usuario.ObterUsuarioAsync(User, _contexto);
            if (usuario == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid || contaViewModel.AlterarSenha == null) return View("MinhaConta", contaViewModel);

            if (!usuario.ValidarSenha(contaViewModel.AlterarSenha.SenhaAtual))
            {
                ModelState.AddModelError(string.Empty, "A senha atual está incorreta.");
                return View("MinhaConta", contaViewModel);
            }

            usuario.AlterarSenha(contaViewModel.AlterarSenha.SenhaNova);
            await _contexto.SaveChangesAsync();

            return RedirectToAction("MinhaConta", new {senhaAlterada = true});
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DesativarConta(MinhaContaViewModel contaViewModel)
        {
            Usuario usuario = await Usuario.ObterUsuarioAsync(User, _contexto);
            if (usuario == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid || contaViewModel.DesativarConta == null) return View("MinhaConta", contaViewModel);

            if (!usuario.ValidarSenha(contaViewModel.DesativarConta.Senha))
            {
                ModelState.AddModelError(string.Empty, "A senha informada está incorreta.");
                return View("MinhaConta", contaViewModel);
            }

            usuario.DesativarConta();
            await _contexto.SaveChangesAsync();

            return RedirectToAction("Deslogar");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _contexto.Dispose();

            base.Dispose(disposing);
        }
    }
}