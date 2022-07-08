using MEE.Business;
using MEE.Domain.Profiles;
using MEE.Helpers;
using MEE.Models;
using MEE.Models.Account;
using MEE.Models.Hiring;
using MEE.Models.Offer;
using MEE.Models.Paysheet;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using MEE.Models.Candidate;
using System.Threading.Tasks;




namespace MEE.Controllers
{
    public class OffersController : BaseController
    {
        private MMEContext db = new MMEContext();
        HelpersController hc = new HelpersController();



        public ActionResult Index()
        {
            try
            {
                List<OffersData> listOffers = new List<OffersData>();

                IRestResponse response = new ApiUrl().UrlExecute("Offers/List?Nit=" + Session["NIT"].ToString().Trim(), Session["ApiToken"].ToString(), "GET", null);


                // IRestResponse response = Api.ExecuteApiUrl("Offers/List", HttpContext.Session.GetString("ApiToken"), "GET", null);
                if (response.StatusDescription == "OK")
                {
                    // FILTRO DE AREAS
                    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    var FilArea = listOffers
                      .GroupBy(a => a.str_Area)
                      .Select(a => new Filtros
                      {
                          Name = a.Key,
                          Cantidad = a.Count().ToString()
                      })
                      .OrderBy(a => a.Name).ToList();
                    ViewBag.FilArea = FilArea;

                    // FILTRO JORNADAS
                    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    var FilJornada = listOffers
                      .GroupBy(a => a.str_WorkingDay)
                      .Select(a => new Filtros
                      {
                          Name = a.Key,
                          Cantidad = a.Count().ToString()
                      })
                      .OrderBy(a => a.Name).ToList();
                    ViewBag.FilJornada = FilJornada;

                    // FILTRO DEPARTAMENTO
                    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    var FilDepartamento = listOffers
                      .GroupBy(a => a.str_Department)
                      .Select(a => new Filtros
                      {
                          Name = a.Key,
                          Cantidad = a.Count().ToString()
                      })
                      .OrderBy(a => a.Name).ToList();
                    ViewBag.FilDepartamento = FilDepartamento;

                    // FILTRO CIUDAD
                    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    var FilCiudad = listOffers
                      .GroupBy(a => a.str_City)
                      .Select(a => new Filtros
                      {
                          Name = a.Key,
                          Cantidad = a.Count().ToString()
                      })
                      .OrderBy(a => a.Name).ToList();
                    ViewBag.FilCiudad = FilCiudad;

                    // FILTRO CARGO
                    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    var FilCargo = listOffers
                      .GroupBy(a => a.str_Position)
                      .Select(a => new Filtros
                      {
                          Name = a.Key,
                          Cantidad = a.Count().ToString()
                      })
                      .OrderBy(a => a.Name).ToList();
                    ViewBag.FilCargo = FilCargo;

                    // FILTRO PSICOLOGO
                    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    var FilPsicologo = listOffers
                      .GroupBy(a => a.str_Email)
                      .Select(a => new Filtros
                      {
                          Name = a.Key,
                          Cantidad = a.Count().ToString()
                      })
                      .OrderBy(a => a.Name).ToList();
                    ViewBag.str_Email = FilPsicologo;

                    //    listOffers = JsonConvert.DeserializeObject<List<OffersData>>(response.Content);
                    if (listOffers.Count() == 0)
                    {
                        listOffers = JsonConvert.DeserializeObject<List<OffersData>>("[]");
                    }

                    return View(listOffers);
                }
                else
                {
                    return View(JsonConvert.DeserializeObject<List<OffersData>>("[]"));
                }
            }
            catch (Exception ex)
            {
                //           GuardarErrores(ex.Message + ". Error en la carga del metodo Index del controlador Offers", "System", _context);
                return View("Error");
            }
        }



        [HttpGet]
        public ActionResult ListData()
        {
            try
            {
                if (!SessionApi())
                {
                    //     GuardarErrores("Error en la carga del metodo Index del controlador Offers", "System", _context);
                    return View("Error");
                }
                List<Offers> listOffers = new List<Offers>();


                IRestResponse response = new ApiUrl().UrlExecute("Offers/List?Nit=" + Session["NIT"].ToString().Trim(), Session["ApiToken"].ToString(), "GET", null);

                if (response.StatusDescription == "OK")
                {
                    listOffers = JsonConvert.DeserializeObject<List<Offers>>(response.Content);
                    if (listOffers.Count() == 0)
                    {
                        listOffers = JsonConvert.DeserializeObject<List<Offers>>("[]");
                    }
                    return Json(listOffers);
                    //   return View();
                }
                else
                {
                    return Json(JsonConvert.DeserializeObject<List<OffersData>>("[]"));
                }
                return Json(listOffers);
            }
            catch (Exception ex)
            {
                // GuardarErrores(ex.Message + ". Error en la carga del metodo Index del controlador Offers", "System", _context);
                return Json(JsonConvert.DeserializeObject<List<OffersData>>("[]"));
            }
        }

        public ActionResult CandidatesOffers(int id)
        {
            try
            {

                List<CandidatesOffers> CandidatesOffers = new List<CandidatesOffers>();
                List<Candidate> Candidates = new List<Candidate>();
                Offers Offer = new Offers();
                IRestResponse response = new ApiUrl().UrlExecute("Offers/GetCandidateOffers?id=" + id, Session["ApiToken"].ToString(), "GET", null);
             //   IRestResponse responseOffer = Api.ExecuteApiUrl("Offers?id=" + id, GlobalJson.Token, "GET", null);
                IRestResponse responseOffer = new ApiUrl().UrlExecute("Offers?id=" + id, Session["ApiToken"].ToString(), "GET", null);
                IRestResponse responseAllCandidate = new ApiUrl().UrlExecute("Candidates", Session["ApiToken"].ToString(), "GET", null);


                if (response.StatusDescription == "OK")
                {
                    CandidatesOffers = JsonConvert.DeserializeObject<List<CandidatesOffers>>(response.Content);
                    Candidates = JsonConvert.DeserializeObject<List<Candidate>>(responseAllCandidate.Content);
                    Offer = JsonConvert.DeserializeObject<Offers>(responseOffer.Content);

    

                    var ListApli = (from item in Candidates
                                    join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                    where CandOfer.int_Estado == 1
                                    select new CandidatesOffersAuxi()
                                    {
                                        Id = item.Id,
                                        cod_emp = item.cod_emp,
                                        nom_emp = item.nom_emp,
                                        str_Email = CandOfer.str_Email,
                                        int_Offer = CandOfer.int_Offer,
                                        Antecedentes = CandOfer.Antecedentes,
                                        Pruebas = CandOfer.Pruebas,
                                        Entrevista = CandOfer.Entrevista,
                                        int_Estado = CandOfer.int_Estado,
                                        int_Riesgo = CandOfer.int_Riesgo
                                    }).ToList();
                    var ListSele = (from item in Candidates
                                    join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                    where CandOfer.int_Estado == 2
                                    select new CandidatesOffersAuxi()
                                    {
                                        Id = item.Id,
                                        cod_emp = item.cod_emp,
                                        nom_emp = item.nom_emp,
                                        str_Email = CandOfer.str_Email,
                                        int_Offer = CandOfer.int_Offer,
                                        Antecedentes = CandOfer.Antecedentes,
                                        Pruebas = CandOfer.Pruebas,
                                        Entrevista = CandOfer.Entrevista,
                                        int_Estado = CandOfer.int_Estado,
                                        int_Riesgo = CandOfer.int_Riesgo
                                    }).ToList();
                    var ListRech = (from item in Candidates
                                    join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                    where CandOfer.int_Estado == 3
                                    select new CandidatesOffersAuxi()
                                    {
                                        Id = item.Id,
                                        cod_emp = item.cod_emp,
                                        nom_emp = item.nom_emp,
                                        str_Email = CandOfer.str_Email,
                                        int_Offer = CandOfer.int_Offer,
                                        Antecedentes = CandOfer.Antecedentes,
                                        Pruebas = CandOfer.Pruebas,
                                        Entrevista = CandOfer.Entrevista,
                                        int_Estado = CandOfer.int_Estado,
                                        int_Riesgo = CandOfer.int_Riesgo
                                    }).ToList();
                    var ListEntre = (from item in Candidates
                                     join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                     where CandOfer.int_Estado == 4
                                     select new CandidatesOffersAuxi()
                                     {
                                         Id = item.Id,
                                         cod_emp = item.cod_emp,
                                         nom_emp = item.nom_emp,
                                         str_Email = CandOfer.str_Email,
                                         int_Offer = CandOfer.int_Offer,
                                         Antecedentes = CandOfer.Antecedentes,
                                         Pruebas = CandOfer.Pruebas,
                                         Entrevista = CandOfer.Entrevista,
                                         int_Estado = CandOfer.int_Estado,
                                         int_Riesgo = CandOfer.int_Riesgo
                                     }).ToList();
                    var ListFina = (from item in Candidates
                                    join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                    where CandOfer.int_Estado == 6
                                    select new CandidatesOffersAuxi()
                                    {
                                        Id = item.Id,
                                        cod_emp = item.cod_emp,
                                        nom_emp = item.nom_emp,
                                        str_Email = CandOfer.str_Email,
                                        int_Offer = CandOfer.int_Offer,
                                        Antecedentes = CandOfer.Antecedentes,
                                        Pruebas = CandOfer.Pruebas,
                                        Entrevista = CandOfer.Entrevista,
                                        int_Estado = CandOfer.int_Estado,
                                        int_Riesgo = CandOfer.int_Riesgo
                                    }).ToList();
                    var ListPosi = (from item in Candidates
                                    join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                    where CandOfer.int_Estado == 8
                                    select new CandidatesOffersAuxi()
                                    {
                                        Id = item.Id,
                                        cod_emp = item.cod_emp,
                                        nom_emp = item.nom_emp,
                                        str_Email = CandOfer.str_Email,
                                        int_Offer = CandOfer.int_Offer,
                                        Antecedentes = CandOfer.Antecedentes,
                                        Pruebas = CandOfer.Pruebas,
                                        Entrevista = CandOfer.Entrevista,
                                        int_Estado = CandOfer.int_Estado,
                                        int_Riesgo = CandOfer.int_Riesgo
                                    }).ToList();
                    var ListContr = (from item in Candidates
                                     join CandOfer in CandidatesOffers on item.e_mail equals CandOfer.str_Email
                                     where CandOfer.int_Estado == 7
                                     select new CandidatesOffersAuxi()
                                     {
                                         Id = item.Id,
                                         cod_emp = item.cod_emp,
                                         nom_emp = item.nom_emp,
                                         str_Email = CandOfer.str_Email,
                                         int_Offer = CandOfer.int_Offer,
                                         Antecedentes = CandOfer.Antecedentes,
                                         Pruebas = CandOfer.Pruebas,
                                         Entrevista = CandOfer.Entrevista,
                                         int_Estado = CandOfer.int_Estado,
                                         int_Riesgo = CandOfer.int_Riesgo
                                     }).ToList();





                    ViewBag.CandOfferApli = ListApli;
                    ViewBag.CandOfferSele = ListSele;
                    ViewBag.CandOfferRech = ListRech;
                    ViewBag.CandOfferEntre = ListEntre;
                    ViewBag.CandOfferFina = ListFina;
                    ViewBag.CandOfferPosi = ListPosi;
                    ViewBag.CandOfferContr = ListContr;

                    ViewBag.int_Offer = id;
                    ViewBag.Offer = Offer;

                    return View();
                }
                else
                {
                    return View(JsonConvert.DeserializeObject<List<OffersCandidate>>("[]"));
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }



        [HttpGet()]
        public async Task<ActionResult> InfoCandidateGesti(string e_mail,string id)
        {
            try
            {
                Candidate Data = new Candidate();

                IRestResponse responseCandidate = new ApiUrl().UrlExecute("Candidates/CandidateEmail?e_mail=" + e_mail, Session["ApiToken"].ToString(), "GET", null);


                if (responseCandidate.StatusDescription == "OK")
                {
                    Data = JsonConvert.DeserializeObject<Candidate>(responseCandidate.Content);
                    IRestResponse responseCandidateExtra = new ApiUrl().UrlExecute("Candidates/CandidateExtra?cod_emp=" + Data.cod_emp, Session["ApiToken"].ToString(), "GET", null);

                    Data.Extra = JsonConvert.DeserializeObject<CandidateExtra>(responseCandidateExtra.Content);

                    IRestResponse response = new ApiUrl().UrlExecute("Offers/GetCandidateOffers?id=" + id, Session["ApiToken"].ToString(), "GET", null);
                    var cand = JsonConvert.DeserializeObject<List<OffersCandidate>>(response.Content);
                    Data.OffersCandidate= cand.Where(x=>x.str_Email==e_mail).FirstOrDefault();

                    int intEdad = DateTime.Now.Year - Convert.ToDateTime(Data.fec_nac).Year;

                    if (DateTime.Now > Convert.ToDateTime(Data.fec_nac).AddYears(intEdad))
                    {
                        intEdad--;
                    }
             //       Data.Extra.str_Edad = intEdad.ToString();
                    return Json(Data, JsonRequestBehavior.AllowGet);
                    //    return Json(Data);
                }

                else
                {
                    //ViewBag.Concept = JsonConvert.DeserializeObject<List<rhh_tbfondos>>("[]");
                    return Json(JsonConvert.DeserializeObject<List<Candidate>>("[]"));

                }
                // return View();

            }
            catch (Exception ex)
            {
                Alerts("Error al cargar la pantalla de conceptos : " + ex.Message, NotificationTypes.error);
                return Json("Error");
            }

        }



        public ActionResult GetTrackingCand(string e_mail, int int_offer)
        {
            try
            {
                List<Tracking> Data = new List<Tracking>();

                IRestResponse responseCandidateSegui = new ApiUrl().UrlExecute("Tracking/TrackingByCandAndConcept?e_mail=" + e_mail + "&int_offer="+ int_offer, Session["ApiToken"].ToString(), "GET", null);

                if (responseCandidateSegui.StatusDescription == "OK")
                {
                    Data = JsonConvert.DeserializeObject<List<Tracking>>(responseCandidateSegui.Content);

                    return Json(Data, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(JsonConvert.DeserializeObject<List<Candidate>>("[]"));
                }


            }
            catch (Exception ex)
            {
                Alerts("Error al cargar la pantalla de conceptos : " + ex.Message, NotificationTypes.error);
                return Json("Error");
            }

        }

        [HttpGet]
        public async Task<ActionResult> CandidateInterview(string e_mail, int int_offer)
        {
            try
            {
                Candidate Data = new Candidate();

                IRestResponse responseCandidate = new ApiUrl().UrlExecute("Candidates/CandidateEmail?e_mail=" + e_mail, Session["ApiToken"].ToString(), "GET", null);


                if (responseCandidate.StatusDescription == "OK")
                {
                    Data = JsonConvert.DeserializeObject<Candidate>(responseCandidate.Content);

                    IRestResponse responseCandidateExtra = new ApiUrl().UrlExecute("Candidates/CandidateExtra?cod_emp=" + Data.cod_emp, Session["ApiToken"].ToString(), "GET", null);


                    Data.Extra = JsonConvert.DeserializeObject<CandidateExtra>(responseCandidateExtra.Content);
                    Offers DataOffer = new Offers();

                    IRestResponse response = new ApiUrl().UrlExecute("Offers?id=" + int_offer, Session["ApiToken"].ToString(), "GET", null);

                    DataOffer = JsonConvert.DeserializeObject<Offers>(response.Content);

                    ViewBag.Offer = DataOffer;
                    ViewBag.e_mail = e_mail;
                    ViewBag.int_offer = int_offer;
                }

                return View(Data);
            }
            catch (Exception ex)
            {
                Alerts("Error al cargar la pantalla : " + ex.Message, NotificationTypes.error);
                return Json("Error");
            }
        }

        [HttpPost]
        public ActionResult PostCandidateInterview(OffersCandidateInterview data)
        {
            try
            {
                string email_Resp = User.Identity.Name;

                data.Email_Resp = email_Resp;

                IRestResponse responseTrack = new ApiUrl().UrlExecute("Offers/SaveCandidateEntrevista", Session["ApiToken"].ToString(), "POST", JsonConvert.SerializeObject(data));


                if (responseTrack.StatusDescription == "OK")
                {

                    return RedirectToAction("CandidatesOffers", "Offers", new { id = data.int_Offer });
                    //  return RedirectToAction("InfoCandidate", "Candidate", new { e_mail = data.Email_emp, id_Tipo = "1", int_offer = data.int_Offer });
                }
                else
                {
                    return RedirectToAction("CandidatesOffers", "Offers", new { id = data.int_Offer });
                 //   Alerts("No se guardo la información", NotificationTypes.error);
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }


        // [HttpPost]
        public ActionResult CambioEstado(int id, int estado,int offer)
        {
            SessionApi();
            try
            {
                if (!SessionApi())
                {
                    //    GuardarErrores("Error en la generación de las variables de sesion.", "System", _context);
                    return View("Error");
                }

      
                IRestResponse response = new ApiUrl().UrlExecute("Offers/EstadoOffersCandidate?id=" + id + "&estado=" + estado, Session["ApiToken"].ToString(), "PUT", null);
  

                if (response.StatusDescription == "OK")
                {
                    if (estado == 7)
                    {
                        Alerts("Candidato contratado exitosamente", NotificationTypes.success);
                    }
                    else if (estado == 3)
                    {
                        Alerts("Candidato rechazado", NotificationTypes.success);
                    }
                    //   return RedirectToAction("MisOfertas", "OffersCandidate");

                    return RedirectToAction("CandidatesOffers", "Offers", new { id = offer });

                }
                else
                {
                    //      GuardarErrores("No se ha cargado la página de eliminación .", "System", _context);
                    //    return RedirectToAction("MisOfertas", "OffersCandidate");
                    return RedirectToAction("CandidatesOffers", "Offers", new { id = offer });
                }
            }
            catch (Exception ex)
            {
                //      GuardarErrores("No se ha cargado la página de eliminación.", "System", _context);
                return View("Error");
            }
        }




        #region Generar Tokens
        /// <summary>
        /// Validar, regenerar y permitir creación de las variables
        /// </summary>
        /// <remarks>
        /// Retorna un bool indicando la creación de los tokens de ser necesarios.
        /// </remarks>
        public bool SessionApi()
        {
            try
            {
                if (Session["NIT"] == null || Session["ApiToken"] == null || Session["GlpiUserJson"] == null)
                {
                    if (Session["NIT"] == null)
                    {
                        SessionCompanies();
                    }
                    var data = new ApiUrl().GetApiToken();
                    if (data.Data.Result == 1)
                    {
                        Session["ApiToken"] = data.Data.Msg;
                        if (User.IsInRole("Company") || User.IsInRole("AteUsuario") || User.IsInRole("AdminCustomer"))
                        {
                            UserGetJsonGlpi();
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Generar y refrescar la variable de sesion NIT
        /// </summary>
        /// <remarks>
        /// Asigna los velores de la variable de session nit segun la compañia que este en el sistema
        /// </remarks>
        public void SessionCompanies()
        {
            List<Domain.Companies.CompanyView> _Companies = new List<Domain.Companies.CompanyView>();
            _Companies = (from cu in db.CompaniesUsers
                          join c in db.Companies on cu.CompanyId equals c.Id
                          where cu.UserId == User.Identity.Name && c.IsActive == true
                          orderby c.Name
                          select new Domain.Companies.CompanyView()
                          {
                              Id = c.NIT,
                              Name = c.Name,
                          }).ToList();

            //al hacer login revisa si hay un nit de lo contrario escoge la posicion cero de la lista y carga este nit por defecto
            if (Session["NIT"] == null)
            {
                Session["NIT"] = _Companies[0].Id;
                ViewBag.Nom_Cli = _Companies[0].Name;
            }
        }
        /// <summary>
        /// Generar y refrescar la variable de sesion GlpiUserJson
        /// </summary>
        /// <remarks>
        /// Metodo privado para obtener la informacio nde usuario de GLPI y si el usuario no existe se crea para siempre tener el json en la varaible de session
        /// </remarks>
        private dynamic UserGetJsonGlpi()
        {
            var json = new DataResponseAjax { Result = 0, Msg = "" };
            //Se solicita a la API la informacion del usuario contenida en GLPI
            var data = JObject.Parse(new ApiUrl().UrlExecute("GLPI/UserGetId/" + Session["NIT"], Session["ApiToken"].ToString(), "GET", null).Content);
            //si el cliente no existe en GLPI se crea 
            if (Convert.ToInt32(data.SelectToken("count")) == 0)
            {
                MailAddress email = new MailAddress(User.Identity.GetUserName().Trim());
                if (email.Host == "misionempresarial.com")
                {
                    json.Msg = "NO encontramos información en SOFIA, el cliente debe loguearse en el portal con su usuario y contraseña y acceder a la sección PQRSF para que el sistema pueda crear una cuenta automaticamente";
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Se solicita a la API los datos registrados del cliente en NOVASOFT para crear el cliente en GLPI
                    data = JObject.Parse(new ApiUrl().UrlExecute("Customers/" + Session["NIT"], Session["ApiToken"].ToString(), "GET", null).Content);

                    UserModel userModel = new UserModel
                    {
                        name = data.cod_cli.ToString().Trim(),
                        realname = data.nom_cli.ToString().ToUpper().Trim(),
                        firstname = data.rep_leg.ToString().ToUpper().Trim(),
                        _default_email = 0,
                        _useremails = new UserModelEmails { id = "0", email = User.Identity.GetUserName().Trim() },
                        phone = data.te1_cli.ToString().Trim(),
                        address = data.di1_cli.ToString().Trim()
                    };
                    //se crea el usuario en GLPI
                    data = JObject.Parse(new ApiUrl().UrlExecute("GLPI/UserCreate/", Session["ApiToken"].ToString(), "PUT", JsonConvert.SerializeObject(userModel)).Content);

                    UserModelProfile userModelProfile = new UserModelProfile
                    {
                        users_id = data.id,
                        profiles_id = 1,
                        entities_id = 0,
                        is_recursive = 1
                    };
                    //Se le setea el perfil y permisos de un usuario comun recien creado
                    data = JObject.Parse(new ApiUrl().UrlExecute("GLPI/UserSetProfile/", Session["ApiToken"].ToString(), "POST", JsonConvert.SerializeObject(userModelProfile)).Content);

                    //Se solicita a la API la informacion del usuario recien creado contenida en GLPI
                    data = JObject.Parse(new ApiUrl().UrlExecute("GLPI/UserGetId/" + Session["NIT"], Session["ApiToken"].ToString(), "GET", null).Content);
                }
            }
            //se guarda el json de informacion del usuario en una variable de session
            Session["GlpiUserJson"] = JsonConvert.SerializeObject(data.SelectToken("data[0]"));
            json.Result = 1;
            json.Msg = "Información cargada correctamente";
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public void Lists()
        {
            //Llamamos los datos que necesitamos
            IRestResponse RespoPais = new ApiUrl().UrlExecute("Selection/Paises", Session["ApiToken"].ToString(), "GET", null);
            IRestResponse RespoCargos = new ApiUrl().UrlExecute("Selection/Cargos?Nit=" + Session["NIT"].ToString(), Session["ApiToken"].ToString(), "GET", null);
            IRestResponse RespoTipContrato = new ApiUrl().UrlExecute("Selection/TipoContrato", Session["ApiToken"].ToString(), "GET", null);
            IRestResponse RespoModelProceso = new ApiUrl().UrlExecute("Selection/ModelProceso", Session["ApiToken"].ToString(), "GET", null);
            IRestResponse response = new ApiUrl().UrlExecute("Paysheet/DateCutAndAgreement?nit=" + Session["NIT"].ToString(), Session["ApiToken"].ToString(), "GET", null);
            IRestResponse RespoOption = new ApiUrl().UrlExecute("OptionProcessPosition", Session["ApiToken"].ToString(), "GET", null);

            var ListAgr = JsonConvert.DeserializeObject<List<AgreementCutDate>>(response.Content);
            List<PaisesResponse> Paises = JsonConvert.DeserializeObject<List<PaisesResponse>>(RespoPais.Content);
            List<Cargo> Cargos = JsonConvert.DeserializeObject<List<Cargo>>(RespoCargos.Content);
            List<TipContrato> TipContrato = JsonConvert.DeserializeObject<List<TipContrato>>(RespoTipContrato.Content);
            List<ModelProceso> ModelProceso = JsonConvert.DeserializeObject<List<ModelProceso>>(RespoModelProceso.Content);
            List<OptionProcessPosition> optionProcessPositions = JsonConvert.DeserializeObject<List<OptionProcessPosition>>(RespoOption.Content);

            ListAgr.Insert(0, new AgreementCutDate { strConvenio = "", strNombreConvenio = "Selecciona tu convenio" });
            var NewListAgr = ListAgr.Select(x => new
            {
                x.strConvenio,
                x.strNombreConvenio
            }).Distinct();

            List<SelectListItem> List_cod_obj_cont = new List<SelectListItem>();
            List_cod_obj_cont.Add(new SelectListItem() { Text = "Selecciona un objeto de contrato", Value = "" });
            List_cod_obj_cont.Add(new SelectListItem() { Text = "Incapacidades ", Value = "1" });
            List_cod_obj_cont.Add(new SelectListItem() { Text = "Incremento Temporal de la Producción", Value = "2" });
            List_cod_obj_cont.Add(new SelectListItem() { Text = "Licencia de Maternidad", Value = "3" });
            List_cod_obj_cont.Add(new SelectListItem() { Text = "Licencia No Remunerada", Value = "4" });
            List_cod_obj_cont.Add(new SelectListItem() { Text = " Otro ", Value = "5" });

            List<SelectListItem> List_cod_cla_sol = new List<SelectListItem>();
            List_cod_cla_sol.Add(new SelectListItem() { Text = "Selecciona un tipo de solicitud ", Value = "" });
            List_cod_cla_sol.Add(new SelectListItem() { Text = " No Aplica ", Value = "0" });
            List_cod_cla_sol.Add(new SelectListItem() { Text = " Genérico ", Value = "01" });
            List_cod_cla_sol.Add(new SelectListItem() { Text = " Especializado ", Value = "02" });
            List_cod_cla_sol.Add(new SelectListItem() { Text = " Urgente ", Value = "03" });


            ViewBag.Agreements = NewListAgr;
            ViewBag.listPaises = Paises;
            ViewBag.Cargos = Cargos;
            ViewBag.TipContrato = TipContrato;
            ViewBag.ModelProceso = ModelProceso;
            ViewBag.List_cod_obj_cont = List_cod_obj_cont;
            ViewBag.List_cod_cla_sol = List_cod_cla_sol;
            ViewBag.List_Options = optionProcessPositions;


        }
        public JsonResult getDepartamentos(string IdPais)
        {
            SessionApi();
            IRestResponse responseDepartamentos = new ApiUrl().UrlExecute("Selection/Departamentos?cod_pai=" + IdPais, Session["ApiToken"].ToString(), "GET", null);
            List<DepartamentosResponse> listDepartamentos = JsonConvert.DeserializeObject<List<DepartamentosResponse>>(responseDepartamentos.Content);
            ViewBag.listDepartamentos = listDepartamentos;
            return Json(listDepartamentos);
        }

        public JsonResult getCiudades(string IdPais, string IdDepartamento)
        {
            SessionApi();
            IRestResponse responseCiudades = new ApiUrl().UrlExecute("Selection/Ciudades?cod_pai=" + IdPais + "&cod_dep=" + IdDepartamento, Session["ApiToken"].ToString(), "GET", null);
            List<CiudadesResponse> listCiudades = JsonConvert.DeserializeObject<List<CiudadesResponse>>(responseCiudades.Content);
            ViewBag.listCiudades = listCiudades;
            return Json(listCiudades);
        }


    }
}
