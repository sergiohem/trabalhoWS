using Newtonsoft.Json;
using SportingTrainingAPI.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace SportingTrainingAPI.Controllers
{
    [RoutePrefix("api/Evento")]
    public class EventoController : ApiController
    {
        private UsuarioController usuarioController = new UsuarioController();

        public class SimuladorFormularioDeEvento
        {
            public int IdEvento { get; set; }
            public string MotivoCancelamento { get; set; }
            public string SituacaoBusca { get; set; }
            public string EstadoBusca { get; set; }
            public string ModalidadeBusca { get; set; }
            public string DataInicioBusca { get; set; }
            public string DataFimBusca { get; set; }
        }

        [Route("ListarEventos")]
        [HttpPost]
        public HttpResponseMessage ListarEventos([FromBody]SimuladorFormularioDeEvento filtro)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                List<Evento> listaDeEventos = BuscarEventos(filtro);
                response.Content = new StringContent(JsonConvert.SerializeObject(listaDeEventos));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [Route("GravarEvento")]
        [HttpPost]
        public string GravarEvento([FromBody]Evento evento, string token)
        {
            if (evento != null)
            {
                using (SportingTrainingEntities context = new SportingTrainingEntities())
                {
                    try
                    {
                        Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                        if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Administrador")
                        {
                            context.Eventos.Add(evento);
                            context.SaveChanges();
                            return "Evento cadastrado com sucesso!";
                        }
                        else
                        {
                            return "Usuário de sessão inválido!";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erro ao gravar evento", ex);
                    }
                }
            }
            else
            {
                return "Evento inválido!";
            }
        }

        [Route("AlterarEvento")]
        [HttpPost]
        public string AlterarEvento([FromBody]Evento evento, string token)
        {
            if (evento != null)
            {
                using (SportingTrainingEntities context = new SportingTrainingEntities())
                {
                    try
                    {
                        Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                        if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Administrador")
                        {
                            Evento eventoNoBanco = context.Eventos.FirstOrDefault(x => x.IdEvento == evento.IdEvento);
                            eventoNoBanco.Nome = evento.Nome;
                            eventoNoBanco.Descricao = evento.Descricao;
                            eventoNoBanco.Data = evento.Data;
                            eventoNoBanco.Hora = evento.Hora;
                            eventoNoBanco.Local = evento.Local;
                            eventoNoBanco.Estado = evento.Estado;
                            eventoNoBanco.Modalidade = evento.Modalidade;
                            eventoNoBanco.ValorInscricao = evento.ValorInscricao;
                            eventoNoBanco.TerminoInscricoes = evento.TerminoInscricoes;
                            eventoNoBanco.TempoLimiteEvento = evento.TempoLimiteEvento;
                            eventoNoBanco.Responsavel = evento.Responsavel;
                            eventoNoBanco.Percurso = evento.Percurso;
                            eventoNoBanco.Situacao = evento.Situacao;
                            eventoNoBanco.MotivoCancelamento = eventoNoBanco.MotivoCancelamento;

                            context.SaveChanges();
                            return "Evento alterado com sucesso!";
                        }
                        else
                        {
                            return "Usuário de sessão inválido!";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erro ao alterar evento", ex);
                    }
                }
            }
            else
            {
                return "Evento inválido!";
            }
        }

        [Route("CancelarEvento")]
        [HttpPost]
        public string CancelarEvento([FromBody]SimuladorFormularioDeEvento cancelamento, string token)
        {
            if (cancelamento.IdEvento != 0)
            {
                if (!string.IsNullOrEmpty(cancelamento.MotivoCancelamento))
                {
                    using (SportingTrainingEntities context = new SportingTrainingEntities())
                    {
                        try
                        {
                            Usuario usuarioSessao = usuarioController.UsuarioLogado(token);
                            if (usuarioSessao != null && usuarioSessao.TipoUsuario == "Administrador")
                            {
                                Evento eventoNoBanco = context.Eventos.FirstOrDefault(x => x.IdEvento == cancelamento.IdEvento);
                                eventoNoBanco.Situacao = "Cancelado";
                                eventoNoBanco.MotivoCancelamento = cancelamento.MotivoCancelamento;

                                context.SaveChanges();
                                return "Evento cancelado com sucesso!";
                            }
                            else
                            {
                                return "Usuário de sessão inválido!";
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Erro ao cancelar evento", ex);
                        }
                    }
                }
                else
                {
                    return "Informe um motivo para cancelar o evento.";
                }
            }
            else
            {
                return "Evento inválido!";
            }
        }

        private List<Evento> BuscarEventos(SimuladorFormularioDeEvento filtro)
        {
            using (SportingTrainingEntities context = new SportingTrainingEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;

                var consultaEventos = context.Eventos.ToList().AsQueryable();
                if (!string.IsNullOrEmpty(filtro.EstadoBusca))
                {
                    consultaEventos = consultaEventos.Where(x => x.Estado == filtro.EstadoBusca);
                }
                if (!string.IsNullOrEmpty(filtro.ModalidadeBusca))
                {
                    consultaEventos = consultaEventos.Where(x => x.Modalidade.Contains(filtro.ModalidadeBusca));
                }
                if (!string.IsNullOrEmpty(filtro.SituacaoBusca))
                {
                    consultaEventos = consultaEventos.Where(x => x.Situacao.ToUpper() == filtro.SituacaoBusca.ToUpper());
                }
                if (!string.IsNullOrEmpty(filtro.DataInicioBusca))
                {
                    DateTime dataInicioBusca = DateTime.Parse(filtro.DataInicioBusca);
                    consultaEventos = consultaEventos.Where(x => x.Data >= dataInicioBusca);
                }
                if (!string.IsNullOrEmpty(filtro.DataFimBusca))
                {
                    DateTime dataFimBusca = DateTime.Parse(filtro.DataFimBusca);
                    consultaEventos = consultaEventos.Where(x => x.Data <= dataFimBusca);
                }

                return consultaEventos.ToList();
            }
        }
    }
}
