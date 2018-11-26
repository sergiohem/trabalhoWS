using Newtonsoft.Json;
using SportingTrainingAPI.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        private class InscricaoJson
        {
            public string NomeAtleta { get; set; }
            public string NomeEvento { get; set; }
            public string ModalidadeEvento { get; set; }
            public string SituacaoEvento { get; set; }
            public DateTime DataEvento { get; set; }
            public string StatusInscricao { get; set; }
            public DateTime DataInscricao { get; set; }
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
        public string ExcluirAtleta([FromBody]SimuladorFormularioDeAtleta formulario, string token)
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
        public string InscreverAtletaEmEvento([FromBody]SimuladorFormularioDeAtleta formulario, string token)
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
                            novaInscricao.Atleta = context.Atletas.FirstOrDefault(x => x.IdAtleta == formulario.IdAtleta);
                            novaInscricao.Evento = context.Eventos.FirstOrDefault(x => x.IdEvento == formulario.IdEventoInscricao);
                            novaInscricao.InscricaoAtleta = "Ativa";
                            novaInscricao.DataInscricao = DateTime.Now;
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
        public string CancelarInscricaoAtletaEmEvento([FromBody]SimuladorFormularioDeAtleta formulario, string token)
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

        [Route("VisualizarInscricoesAtleta")]
        [HttpPost]
        public HttpResponseMessage VisualizarInscricoesAtleta([FromBody]SimuladorFormularioDeAtleta filtro, string token)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);

                Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Atleta")
                {
                    List<EventoAtleta> listaDeInscricoes = BuscarInscricoesAtleta(filtro);
                    List<InscricaoJson> inscricoesEmJson = new List<InscricaoJson>();

                    foreach (var inscricao in listaDeInscricoes)
                    {
                        InscricaoJson inscricaoJson = new InscricaoJson
                        {
                            NomeAtleta = inscricao.Atleta.Nome,
                            NomeEvento = inscricao.Evento.Nome,
                            ModalidadeEvento = inscricao.Evento.Modalidade,
                            SituacaoEvento = inscricao.Evento.Situacao,
                            DataEvento = inscricao.Evento.Data,
                            StatusInscricao = inscricao.InscricaoAtleta,
                            DataInscricao = inscricao.DataInscricao
                        };

                        inscricoesEmJson.Add(inscricaoJson);
                    }

                    response.Content = new StringContent(JsonConvert.SerializeObject(inscricoesEmJson));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");       
                }
                else
                {
                    response.Content = new StringContent("Usuário inválido!");
                }
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        private List<EventoAtleta> BuscarInscricoesAtleta(SimuladorFormularioDeAtleta filtro)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                var consultaInscricoes = context.EventosAtletas.Include("Atleta").Include("Evento").Where(x => x.IdAtleta == filtro.IdAtleta).AsQueryable();

                if (!string.IsNullOrEmpty(filtro.ModalidadeBusca))
                {
                    consultaInscricoes = consultaInscricoes.Where(x => x.Evento.Modalidade.Contains(filtro.ModalidadeBusca));
                }
                if (!string.IsNullOrEmpty(filtro.SituacaoBusca))
                {
                    consultaInscricoes = consultaInscricoes.Where(x => x.Evento.Situacao.ToUpper() == filtro.SituacaoBusca.ToUpper());
                }
                if (!string.IsNullOrEmpty(filtro.DataInicioBusca))
                {
                    DateTime dataInicioBusca = DateTime.Parse(filtro.DataInicioBusca);
                    consultaInscricoes = consultaInscricoes.Where(x => x.DataInscricao >= dataInicioBusca);
                }
                if (!string.IsNullOrEmpty(filtro.DataFimBusca))
                {
                    DateTime dataFimBusca = DateTime.Parse(filtro.DataFimBusca);
                    consultaInscricoes = consultaInscricoes.Where(x => x.DataInscricao <= dataFimBusca);
                }

                return consultaInscricoes.ToList();
            }
        }
    }
}
