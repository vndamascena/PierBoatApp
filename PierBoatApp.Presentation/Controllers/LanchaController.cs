using PierBoatApp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using PierBoatApp.Data.Repositories;
using PierBoatApp.Presentation.Models.Lancha;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Reflection;

namespace PierBoatApp.Presentation.Controllers
{
    [Authorize]
    public class LanchaController : Controller
    {
        public IActionResult Cadastrar()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Cadastrar(CadastrarViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //capturar o usuário autenticado
                    //no arquivo de cookie do Asp.Net
                    //var usuario = JsonConvert.DeserializeObject
                    //<Usuario>(User.Identity.Name);
                    //capturando os dados da conta
                    var lancha = new Lancha
                    {
                        Id = Guid.NewGuid(),
                        Nome = model.Nome,
                        Telefone = model.Telefone,
                        Data = model.Data.Value,
                        Observacao = model.Observacao,
                        Periodo = model.Periodo.Value,



                    };
                    //gravar a conta no banco de dados
                    var lanchaRepository = new LanchaRepository();
                    lanchaRepository.Add(lancha);
                    TempData["MensagemSucesso"] = "Cliente agendado com sucesso!";
                    ModelState.Clear(); //limpar os campos do formulário
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro no preechimento, verifique os campos e realize o cadastro";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram erros no preenchimento do formulário de cadastro,por favor verifique.";
            }

            return View();
        }

        public IActionResult ConsultaAgendamento()
        {
            var model = new ConsultaViewModel();

            try
            {


                DateTime dataAtual = DateTime.Today;

                // Definir a data de início como o dia atual
                model.DataInicio = dataAtual;

                // Definir a data de fim como o último dia do mês atual
                DateTime ultimoDiaDoMes = new DateTime(dataAtual.Year, dataAtual.Month, DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month));
                model.DataFim = ultimoDiaDoMes;


                //consultando as contas do usuário no banco de dados
                var lanhcaRepository = new LanchaRepository();
                ViewBag.Lancha = lanhcaRepository.GetAgendamento(model.DataInicio.Value, model.DataFim.Value);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = e.Message;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult ConsultaAgendamento(ConsultaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime dataInicio = model.DataInicio ?? DateTime.MinValue; // Converter para DateTime não nullable
                    DateTime dataFim = model.DataFim ?? DateTime.MaxValue; // Converter para DateTime não nullable   

                    //consultar as contas do usuário
                    var lanchaRepository = new LanchaRepository();
                    var lancha = lanchaRepository.GetAgendamento(model.DataInicio.Value, model.DataFim.Value);

                    if (lancha.Count > 0) //a consulta obteve resultados
                    {
                        ViewBag.Lancha = lancha; //enviando a lista de contas para a página
                    }
                    else
                    {
                        TempData["MensagemAlerta"] = "Nenhuma conta foi encontrada para o período de datas selecionado.";
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram erros no preenchimento do formulário de consulta, por favor verifique.";
            }

            return View();        
    
        
        }


      


        public IActionResult Editar(Guid id)
        {
            // Obter a lancha para edição com base no ID fornecido
            var lanchaRepository = new LanchaRepository();
            var lancha = lanchaRepository.GetById(id);

            // Verificar se a lancha existe
            

            // Preencher o ViewModel de edição com os dados da lancha
            var model = new EditarViewModel
            {
                
                Nome = lancha.Nome,
                Telefone = lancha.Telefone,
                Data = lancha.Data,
                Observacao = lancha.Observacao,
                Periodo = lancha.Periodo
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Editar(EditarViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obter a lancha existente com base no ID fornecido
                    var lanchaRepository = new LanchaRepository();
                    var lancha = lanchaRepository.GetById(model.Id);

                   

                    // Atualizar os dados da lancha com os novos dados do ViewModel
                    lancha.Nome = model.Nome;
                    lancha.Telefone = model.Telefone;
                    lancha.Data = model.Data;
                    lancha.Observacao = model.Observacao;
                    lancha.Periodo = model.Periodo;

                    // Salvar as alterações no banco de dados
                    lanchaRepository.Update(lancha);

                    TempData["MensagemSucesso"] = "Cliente editado com sucesso!";
                    return RedirectToAction("ConsultaAgendamento"); // Redirecionar para a página de consulta após a edição
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro na edição, verifique os campos e tente novamente";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram erros no preenchimento do formulário de edição, por favor verifique.";
            }

            // Se ocorrer um erro, retornar à página de edição com os dados preenchidos
            return View(model);
        }



        
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Obter a lancha para exclusão com base no ID fornecido
                var lanchaRepository = new LanchaRepository();
                var lancha = lanchaRepository.GetById(id);

                

                // Excluir a lancha do banco de dados
                lanchaRepository.Delete(lancha);

                TempData["MensagemSucesso"] = "Agendamento excluída com sucesso!";
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao excluir a agendamento: " + e.Message;
            }

            return RedirectToAction("ConsultaAgendamento");
        }


    }
}



