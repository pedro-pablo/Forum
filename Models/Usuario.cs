using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class Usuario
    {
        public Usuario()
        {
        }

        public Usuario(UsuarioCadastroViewModel cadastroViewModel)
        {
            Preencher(cadastroViewModel);
        }

        public int Id { get; set; }

        public string Nome { get; set; }

        public byte[] Senha { get; set; }

        public byte[] Salt { get; set; }

        public string Email { get; set; }

        public DateTime Criacao { get; private set; }

        public bool Desativado { get; private set; }

        public List<Postagem> Postagens { get; set; }

        public bool ValidarLogin(UsuarioLoginViewModel loginViewModel)
        {
            return ValidarSenha(loginViewModel.Senha) && !Desativado;
        }

        public bool ValidarSenha(string senha)
        {
            byte[] senhaTextoHash = Hashing.GerarHashSenha(senha, Salt);
            bool senhaCorreta = true;
            for (int i = 0; i < Senha.Length; i++)
                if (senhaTextoHash[i] != Senha[i])
                {
                    senhaCorreta = false;
                    break;
                }

            return senhaCorreta;
        }

        public void DesativarConta()
        {
            Desativado = true;
        }

        public void AlterarSenha(string senha)
        {
            Salt = Hashing.GerarSalt();
            Senha = Hashing.GerarHashSenha(senha, Salt);
        }

        public void Preencher(UsuarioCadastroViewModel cadastroViewModel)
        {
            Nome = cadastroViewModel.Nome;
            Email = cadastroViewModel.Email;
            Criacao = DateTime.UtcNow;
            AlterarSenha(cadastroViewModel.Senha);
        }

        public static List<string> ValidarCadastro(UsuarioCadastroViewModel cadastroViewModel)
        {
            List<string> erros = new List<string>();

            if (cadastroViewModel.Senha != cadastroViewModel.SenhaConfirmacao)
                erros.Add("A confirmação de senha está incorreta.");

            if (!cadastroViewModel.Termos) erros.Add("Você deve concordar com os termos do site para criar uma conta.");

            return erros;
        }

        public static async Task<Usuario> ObterUsuarioAsync(ClaimsPrincipal user, ContextoDb db)
        {
            int usuarioId = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier).Value);
            return await db.Usuarios.FindAsync(usuarioId);
        }
    }
}