using SportingTrainingAPI.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportingTrainingAPI.Controllers
{
    [RoutePrefix("api/Atleta")]
    public class AtletaController : ApiController
    {
        public class SimuladorFormularioDeAtleta
        {
            public string IdAtleta { get; set; }
            public int IdEventoInscricao { get; set; }
            public int IdEventoAtleta { get; set; }
            public string SituacaoBusca { get; set; }
            public string ModalidadeBusca { get; set; }
            public string DataInicioBusca { get; set; }
            public string DataFimBusca { get; set; }
        }

        private UsuarioController usuarioController = new UsuarioController();

        [Route("GravarAtleta")]
        [HttpPost]
        public string GravarAtleta([FromBody]Atleta atleta, string token)
        {
            if (atleta != null)
            {
                using (SportingTrainingEntities context = new SportingTrainingEntities())
                {
                    try
                    {
                        Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                        if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Atleta")
                        {
                            atleta.IdAtleta = Guid.NewGuid().ToString();
                            atleta.IdUsuario = usuarioSessao.IdUsuario;
                            context.Atletas.Add(atleta);
                            context.SaveChanges();
                            return "Seu cadastro foi realizado com sucesso!";
                        }
                        else
                        {
                            return "Usuário de sessão inválido!";
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Entidade do tipo \"{0}\" com estado \"{1}\" tem os seguintes erros de validação:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                System.Diagnostics.Debug.WriteLine("- Propriedade: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
            }
            else
            {
                return "Dados inválidos!";
            }
        }

        [Route("AlterarAtleta")]
        [HttpPost]
        public string AlterarAtleta([FromBody]Atleta atleta, string token)
        {
            if (atleta != null)
            {
                using (SportingTrainingEntities context = new SportingTrainingEntities())
                {
                    try
                    {
                        Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                        if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Atleta")
                        {
                            Atleta atletaNoBanco = context.Atletas.FirstOrDefault(x => x.IdAtleta == atleta.IdAtleta);
                            atletaNoBanco.Nome = atleta.Nome;
                            atletaNoBanco.Endereco = atleta.Endereco;
                            atletaNoBanco.DataNascimento = atleta.DataNascimento;
                            atletaNoBanco.TipoSanguineo = atleta.TipoSanguineo;
                            atletaNoBanco.PlanoDeSaude = atleta.PlanoDeSaude;
                            atletaNoBanco.Cpf = atleta.Cpf;
                            atletaNoBanco.Nacionalidade = atleta.Nacionalidade;
                            atletaNoBanco.Equipe = atleta.Equipe;

                            context.SaveChanges();
                            return "Seu cadastro foi alterado com sucesso!";
                        }
                        else
                        {
                            return "Usuário de sessão inválido!";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erro ao alterar cadastro", ex);
                    }
                }
            }
            else
            {
                return "Dados inválidos!";
            }
        }

        [Route("ExcluirAtleta")]
        [HttpPost]
        public string ExcluirAtleta(SimuladorFormularioDeAtleta formulario, string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                try
                {
                    Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                    if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Atleta")
                    {
                        Atleta atleta = context.Atletas.FirstOrDefault(x => x.IdAtleta == formulario.IdAtleta);
                        if (atleta != null)
                        {
                            //antes dessa parte, lembrar de cancelar inscrição do atleta, se ele tiver inscrito em algum evento
                            if (atleta.EventosAtleta.Count > 0)
                            {
                                context.EventosAtletas.RemoveRange(atleta.EventosAtleta);
                            }
                            context.Atletas.Remove(atleta);
                            usuarioController.Deslogar(token);

                            context.SaveChanges();

                            usuarioController.ExcluirUsuarioPorToken(token);                         

                            return "Atleta excluído com sucesso!";
                        }
                        else
                        {
                            return "Atleta não pode ser excluído";
                        }
                    }
                    else
                    {
                        return "Usuário de sessão inválido!";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao excluir atleta!", ex);
                }
            }
        }

        [Route("InscreverAtletaEmEvento")]
        [HttpPost]
        public string InscreverAtletaEmEvento(SimuladorFormularioDeAtleta formulario, string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                try
                {
                    Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                    if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Atleta")
                    {
                        if (!string.IsNullOrEmpty(formulario.IdAtleta) && formulario.IdEventoInscricao != 0)
                        {
                            EventoAtleta novaInscricao = new EventoAtleta();
                            novaInscricao.IdAtleta = formulario.IdAtleta;
                            novaInscricao.IdEvento = formulario.IdEventoInscricao;
                            novaInscricao.InscricaoAtleta = "Ativa";
                            novaInscricao.DataInscricao = DateTime.Now;
                            //teste
                            context.EventosAtletas.Add(novaInscricao);

                            context.SaveChanges();

                            return "Inscrição realizada com sucesso!";
                        }
                        else
                        {
                            return "Dados inválidos para inscrição!";
                        }
                    }
                    else
                    {
                        return "Usuário de sessão inválido!";
                    }
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Entidade do tipo \"{0}\" com estado \"{1}\" tem os seguintes erros de validação:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("- Propriedade: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }

        [Route("CancelarInscricaoAtletaEmEvento")]
        [HttpPost]
        public string CancelarInscricaoAtletaEmEvento(SimuladorFormularioDeAtleta formulario, string token)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                try
                {
                    Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                    if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Atleta")
                    {
                        EventoAtleta inscricao = context.EventosAtletas.FirstOrDefault(x => x.IdEventoAtleta == formulario.IdEventoAtleta);

                        if (inscricao != null)
                        {
                            inscricao.InscricaoAtleta = "Cancelada";

                            context.SaveChanges();

                            return "Inscrição cancelada com sucesso!";
                        }
                        else
                        {
                            return "Dados inválidos para cancelamento de inscrição!";
                        }
                    }
                    else
                    {
                        return "Usuário de sessão inválido!";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cancelar inscrição de atleta no evento!", ex);
                }
            }
        }
    }
}
