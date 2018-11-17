using SportingTrainingAPI.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportingTrainingAPI.Controllers
{
    [RoutePrefix("api/Usuario")]
    public class UsuarioController : ApiController
    {
        [Route("GravarUsuario")]
        [HttpPost]
        public string GravarUsuario([FromBody]Usuario usuario)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                if (usuario != null)
                {
                    try
                    {
                        var usuarioComMesmoLogin = UsuarioExistente(usuario.Login);
                        if (usuarioComMesmoLogin == null)
                        {
                            usuario.IdUsuario = Guid.NewGuid().ToString();
                            context.Usuarios.Add(usuario);
                            context.SaveChanges();
                            return "Usuário gravado com sucesso!";
                        }
                        else
                        {
                            return "Login inválido!";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erro ao gravar usuário", ex);
                    }
                }
                else
                {
                    return "Usuário inválido!";
                }
            }
        }

        [Route("ExcluirUsuario")]
        [HttpPost]
        public void ExcluirUsuarioPorToken(string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                Usuario usuario = context.LoginsUsuario.FirstOrDefault(x => x.Token == token).Usuario;

                if (usuario != null)
                {
                    context.LoginsUsuario.RemoveRange(usuario.LoginsUsuario);
                    context.Usuarios.Remove(usuario);

                    context.SaveChanges();
                }
            }
        }

        [HttpPost]
        [Route("Logar")]
        public string Logar(string login, string senha)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                try
                {
                    var usuario = context.Usuarios.Where(x => x.Login == login && x.Senha == senha).FirstOrDefault();
                    string token = Guid.NewGuid().ToString();
                    token = token.Replace("{", "").Replace("}", "").Replace("-", "");

                    LoginUsuario novoLogin = new LoginUsuario();
                    novoLogin.Usuario = usuario;
                    novoLogin.Token = token;
                    novoLogin.InicioSessao = DateTime.Now;
                    novoLogin.FimSessao = novoLogin.InicioSessao.AddMinutes(30);
                    context.LoginsUsuario.Add(novoLogin);
                    context.SaveChanges();

                    return "Login efetuado com sucesso!";
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao logar", ex);
                }
            }
        }

        [HttpPost]
        [Route("AtualizarSessao")]
        public string AtualizarSessao(string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                try
                {
                    var login = context.LoginsUsuario.Where(x => x.Token == token).FirstOrDefault();
                    login.InicioSessao = DateTime.Now;
                    login.FimSessao = login.InicioSessao.AddMinutes(30);

                    context.SaveChanges();

                    return "Sessão atualizada com sucesso!";
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar sessão", ex);
                }
            }
        }

        [HttpPost]
        [Route("Deslogar")]
        public string Deslogar(string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                try
                {
                    var login = context.LoginsUsuario.Where(x => x.Token == token).FirstOrDefault();
                    login.FimSessao = DateTime.Now;

                    context.SaveChanges();

                    return "Usuário deslogado com sucesso!";
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao deslogar", ex);
                }
            }
            
        }

        private Usuario UsuarioExistente(string login)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                var usuario = context.Usuarios.Where(x => x.Login == login).FirstOrDefault();
                if (usuario != null)
                {
                    return usuario;
                }
                return null;
            }
        }

        public Usuario UsuarioLogado(string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                var usuario = context.LoginsUsuario.Where(x => x.Token == token && x.FimSessao >= DateTime.Now).Select(x => x.Usuario).FirstOrDefault();
                if (usuario != null)
                {
                    return usuario;
                }
                return null;
            }
        }
    }
}
