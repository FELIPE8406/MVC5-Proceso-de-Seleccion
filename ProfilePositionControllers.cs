using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Models.Hiring;
using System.Linq;
using RestSharp;
using Microsoft.AspNetCore.Http;
using WebApi.Models.ProfilePosition;
using Microsoft.AspNetCore.Identity;
using WebApi.Models.Candidate;
using WebApi.Models.Account;
using WebApi.Helpers;
using Microsoft.Extensions.Configuration;
using WebApi.Models.SystemResponses;
using WebApi.Models.Offer;
using WebApi.Controllers;

namespace WEBAPIPRUEBAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ProfilePositionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _Configuration;
        public ProfilePositionController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        /// <summary>
        /// Get list Position Profiles by client
        /// </summary>
        /// <remarks>
        /// Get list Position Profiles by client
        /// </remarks>
        [HttpGet]
        public List<PositionProfile> GetPositionProfiles(string cod_cli)
        {
            try
            {
                var Position = _context.PositionProfile.Where(x => x.cod_cli == cod_cli).ToList();
                return Position;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get list Position Profiles by client
        /// </summary>
        /// <remarks>
        /// Get list Position Profiles by client
        /// </remarks>
        [HttpGet]
        [Route("GetPositionProfilesById")]
        public PositionProfile GetPositionProfilesById(int id)
        {
            try
            {
                PositionProfile model = new PositionProfile();
                model = _context.PositionProfile.Where(x => x.Id == id).FirstOrDefault();

                model.id_test = _context.OptionTest.Where(x => x.id_Cargo == id).Select(x => x.id_Opti.ToString()).ToArray();
                model.id_antecedent = _context.OptionAntecedent.Where(x => x.id_Cargo == id).Select(x => x.id_Opti.ToString()).ToArray();
                model.id_rasgosADEP = _context.OptionRasgos.Where(x => x.id_Cargo == id && x.id_Prueba == 1).Select(x => x.id_Opti.ToString()).ToArray();
                model.id_rasgosServicios = _context.OptionRasgos.Where(x => x.id_Cargo == id && x.id_Prueba == 4).Select(x => x.id_Opti.ToString()).ToArray();
                model.id_rasgosVentas = _context.OptionRasgos.Where(x => x.id_Cargo == id && x.id_Prueba == 3).Select(x => x.id_Opti.ToString()).ToArray();

                CargosRespuestaTarea tarea = _context.CargosRespuestaTarea.Where(x => x.Id_Cargo == id).FirstOrDefault();
                if (tarea != null)
                {
                    model.CargosRespuestaTarea = new CargosRespuestaTarea();
                    model.CargosRespuestaTarea.Id = tarea.Id;
                    model.CargosRespuestaTarea.Id_Cargo = tarea.Id_Cargo;
                    model.CargosRespuestaTarea.ALTURAS = tarea.ALTURAS;
                    model.CargosRespuestaTarea.ESPACIOSCONFINADOS = tarea.ESPACIOSCONFINADOS;
                    model.CargosRespuestaTarea.ENERGIASPELIGROSAS = tarea.ENERGIASPELIGROSAS;
                    model.CargosRespuestaTarea.SUSTANCIASPELIGROSAS = tarea.SUSTANCIASPELIGROSAS;
                    model.CargosRespuestaTarea.CALIENTE = tarea.CALIENTE;
                    model.CargosRespuestaTarea.RADIACIONESIONIZANTES = tarea.RADIACIONESIONIZANTES;
                    model.CargosRespuestaTarea.MANIPULACIONALIMENTOS = tarea.MANIPULACIONALIMENTOS;
                    model.CargosRespuestaTarea.MONTACARGA = tarea.MONTACARGA;
                    model.CargosRespuestaTarea.LIBRETAMILITAR = tarea.LIBRETAMILITAR;
                }
                var pregun = _context.CargosConfigurationQuestion.Where(x => x.id_Cargo == id).ToList();
                model.CargosConfigurationQuestionSave = new List<CargosConfigurationQuestion>();
                for (int p = 0; p < pregun.Count(); p++)
                {
                    CargosConfigurationQuestion Quest = new CargosConfigurationQuestion();
                    Quest.Id = pregun[p].Id;
                    Quest.id_Cargo = pregun[p].id_Cargo;
                    Quest.int_State = pregun[p].int_State;
                    Quest.int_Type = pregun[p].int_Type;
                    Quest.str_Description = pregun[p].str_Description;
                    Quest.str_Title = pregun[p].str_Title;
                    Quest.CustList = new List<CargosQuestionItems>();


                    var pregunitem = _context.CargosQuestionOptions.Where(x => x.id_questions == pregun[p].Id).ToList();

                    for (int i = 0; i < pregunitem.Count(); i++)
                    {
                        CargosQuestionItems QuestItem = new CargosQuestionItems();

                        QuestItem.Name = pregunitem[i].str_Options;
                        QuestItem.bln_Excluye = pregunitem[i].bln_Excluye;
                        Quest.CustList.Add(QuestItem);

                    }
                    model.CargosConfigurationQuestionSave.Add(Quest);
                }

                var escola = (from item in _context.OptionEscolaridad.Where(x => x.id_Cargo == id).ToList()
                              join Progra in _context.CargosProgramas.ToList() on item.id_Programa equals Progra.Id
                              join escol in _context.CargosEscolaridad.ToList() on Progra.Id_Escolaridad equals escol.Id
                              select new OptionEscolaridad()
                              {
                                  id = item.id,
                                  id_Cargo = item.id_Cargo,
                                  id_Programa = item.id_Programa,
                                  nom_form = escol.Nombre,
                                  nom_pro = Progra.Name,
                                  blnPrincipal = item.blnPrincipal,
                                  str_StudiesObservation = item.str_StudiesObservation
                              }).ToList();

                model.OptionEscolaridadSave = escola;

                model.CargosLicencia = _context.CargosLicencia.Where(x => x.Id_Cargo == id).FirstOrDefault();



                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///Save Cargos
        /// </summary>
        /// <remarks>
        /// Save Cargos
        /// </remarks>
        [HttpPost("[action]")]
        public async Task<OkObjectResult> SaveCargos(PositionProfile model)
        {
            ResponseJson json = new ResponseJson();
            try
            {



                if (ModelState.IsValid)
                {
                    var Exist = _context.PositionProfile.Where(x => x.cod_car == model.cod_car).FirstOrDefault();
                    if (Exist == null)
                    {
                        _context.PositionProfile.Add(model);
                        var response = await _context.SaveChangesAsync();

                        if (response == 1)
                        {
                            //guardar tests
                            foreach (var test in model.id_test)
                            {
                                var objOpt = new OptionTest();
                                objOpt.id_Cargo = model.Id;
                                objOpt.id_Opti = Convert.ToInt32(test);
                                _context.OptionTest.Add(objOpt);
                                await _context.SaveChangesAsync();
                            }

                            //  guardar antecedentes
                            foreach (var antecedents in model.id_antecedent)
                            {
                                var objOptAnt = new OptionAntecedent();
                                objOptAnt.id_Cargo = model.Id;
                                objOptAnt.id_Opti = Convert.ToInt32(antecedents);
                                _context.OptionAntecedent.Add(objOptAnt);
                                await _context.SaveChangesAsync();
                            }

                            //  guardar rasgos
                            foreach (var rasgosADEP in model.id_rasgosADEP)
                            {
                                var objOptRas = new OptionRasgos();
                                objOptRas.id_Cargo = model.Id;
                                objOptRas.id_Prueba = 1;
                                objOptRas.id_Opti = Convert.ToInt32(rasgosADEP);
                                _context.OptionRasgos.Add(objOptRas);
                                await _context.SaveChangesAsync();
                            }

                            foreach (var id_rasgosServicios in model.id_rasgosServicios)
                            {
                                var objOptRas = new OptionRasgos();
                                objOptRas.id_Cargo = model.Id;
                                objOptRas.id_Prueba = 4;
                                objOptRas.id_Opti = Convert.ToInt32(id_rasgosServicios);
                                _context.OptionRasgos.Add(objOptRas);
                                await _context.SaveChangesAsync();
                            }

                            foreach (var id_rasgosVentas in model.id_rasgosVentas)
                            {
                                var objOptRas = new OptionRasgos();
                                objOptRas.id_Cargo = model.Id;
                                objOptRas.id_Prueba = 3;
                                objOptRas.id_Opti = Convert.ToInt32(id_rasgosVentas);
                                _context.OptionRasgos.Add(objOptRas);
                                await _context.SaveChangesAsync();
                            }

                            //  guardar tareas alto riesgo
                            var CargosRespuestaTarea = new CargosRespuestaTarea();
                            CargosRespuestaTarea.Id_Cargo = model.Id;
                            CargosRespuestaTarea.ALTURAS = model.CargosRespuestaTarea.ALTURAS;
                            CargosRespuestaTarea.ESPACIOSCONFINADOS = model.CargosRespuestaTarea.ESPACIOSCONFINADOS;
                            CargosRespuestaTarea.ENERGIASPELIGROSAS = model.CargosRespuestaTarea.ENERGIASPELIGROSAS;
                            CargosRespuestaTarea.SUSTANCIASPELIGROSAS = model.CargosRespuestaTarea.SUSTANCIASPELIGROSAS;
                            CargosRespuestaTarea.CALIENTE = model.CargosRespuestaTarea.CALIENTE;
                            CargosRespuestaTarea.RADIACIONESIONIZANTES = model.CargosRespuestaTarea.RADIACIONESIONIZANTES;
                            CargosRespuestaTarea.MANIPULACIONALIMENTOS = model.CargosRespuestaTarea.MANIPULACIONALIMENTOS;
                            CargosRespuestaTarea.MONTACARGA = model.CargosRespuestaTarea.MONTACARGA;
                            CargosRespuestaTarea.LIBRETAMILITAR = model.CargosRespuestaTarea.LIBRETAMILITAR;


                            _context.CargosRespuestaTarea.Add(CargosRespuestaTarea);
                            await _context.SaveChangesAsync();
                            if (model.CargosConfigurationQuestionSave != null)
                            {
                                foreach (var Ques in model.CargosConfigurationQuestionSave)
                                {
                                    var obQue = new CargosConfigurationQuestion();
                                    obQue.id_Cargo = model.Id;
                                    obQue.int_State = 1;
                                    obQue.int_Type = Ques.int_Type;
                                    obQue.str_Description = Ques.str_Description;
                                    obQue.str_Title = Ques.str_Title;
                                    _context.CargosConfigurationQuestion.Add(obQue);
                                    await _context.SaveChangesAsync();
                                    if (Ques.CustList != null)
                                    {
                                        foreach (var QuesOpt in Ques.CustList)
                                        {
                                            var obQuesOp = new CargosQuestionOptions();
                                            obQuesOp.id_questions = obQue.Id;
                                            obQuesOp.str_Options = QuesOpt.Name;
                                            obQuesOp.bln_Excluye = QuesOpt.bln_Excluye;
                                            _context.CargosQuestionOptions.Add(obQuesOp);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }

                            }
                            foreach (var Ques in model.OptionEscolaridadSave)
                            {
                                var obQue = new OptionEscolaridad();
                                obQue.id_Cargo = model.Id;
                                obQue.id_Programa = Ques.id_Programa;
                                obQue.str_StudiesObservation = Ques.str_StudiesObservation;
                                obQue.blnPrincipal = Ques.blnPrincipal;
                                _context.OptionEscolaridad.Add(obQue);
                                await _context.SaveChangesAsync();
                            }


                            var CargosLicencia = new CargosLicencia();
                            CargosLicencia.Id_Cargo = model.Id;
                            CargosLicencia.bool_A1 = model.CargosLicencia.bool_A1;
                            CargosLicencia.bool_A2 = model.CargosLicencia.bool_A2;
                            CargosLicencia.bool_B1 = model.CargosLicencia.bool_B1;
                            CargosLicencia.bool_B2 = model.CargosLicencia.bool_B2;
                            CargosLicencia.bool_B3 = model.CargosLicencia.bool_B3;
                            CargosLicencia.bool_C1 = model.CargosLicencia.bool_C1;
                            CargosLicencia.bool_C2 = model.CargosLicencia.bool_C2;
                            CargosLicencia.bool_C3 = model.CargosLicencia.bool_C3;
                            CargosLicencia.bool_Unlicensed = model.CargosLicencia.bool_Unlicensed;





                            _context.CargosLicencia.Add(CargosLicencia);
                            await _context.SaveChangesAsync();


                            json.Succeeded = true;
                            json.Errors.Add(new Errors
                            {
                                Code = model.Id.ToString(),
                                Description = "Guardado exitoso"
                            });
                        }
                        else
                        {
                            json.Succeeded = false;
                            json.Errors.Add(new Errors
                            {
                                Code = "0",
                                Description = "Error al consumir la API"
                            });
                        }
                    }
                    else
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "1",
                            Description = "El Perfil de cargo con código " + model.cod_car + " ya esta homologado en novasoft"
                        });
                    }

                    return Ok(json);
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "1",
                        Description = "Modelo invalido"
                    });
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "1",
                    Description = "error " + ex.Message,
                });
                return Ok(json);
            }
        }


        /// <summary>
        /// Update Estado
        /// </summary>
        /// <remarks>
        /// Update Estado
        /// </remarks>
        [HttpPut("[action]")]
        public async Task<OkObjectResult> PutEstado(PositionProfile model)
        {
            ResponseJson json = new ResponseJson();
            try
            {

                var ProfileOld = _context.PositionProfile.Find(model.Id);
                var ProfileNew = _context.PositionProfile.Find(model.Id);
                if (ProfileOld != null)
                {
                    if (ProfileOld.Est_Per == 0)
                    {
                        ProfileNew.Est_Per = 1;
                    }
                    else
                    {
                        ProfileNew.Est_Per = 0;
                    }

                    _context.Entry(ProfileOld).CurrentValues.SetValues(ProfileNew);
                    var res = await _context.SaveChangesAsync();
                    if (res == 1)
                    {
                        json.Succeeded = true;
                        json.Errors.Add(new Errors
                        {
                            Code = string.Empty,
                            Description = string.Empty
                        });
                    }
                    else
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Error al consumir el API"
                        });
                    }
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "0",
                        Description = "No es posible encontrar la solicitud para realizar la modificación"
                    });
                }
                return Ok(json);

            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "0",
                    Description = ex.Message
                });
                return Ok(json);
            }

        }

        /// <summary>
        /// Update Perfil
        /// </summary>
        /// <remarks>
        /// Update Perfil
        /// </remarks>
        [HttpPut("[action]")]
        public async Task<OkObjectResult> PutPerfil(PositionProfile model)
        {
            ResponseJson json = new ResponseJson();
            try
            {
                _context.Update(model);
                var res = await _context.SaveChangesAsync();

                if (res == 1)
                {
                    _context.Update(model.CargosRespuestaTarea);
                    await _context.SaveChangesAsync();

                    foreach (var itemEsco in model.OptionEscolaridadSave)
                    {
                        if (itemEsco.id != 0)
                        {
                            OptionEscolaridad esco = new OptionEscolaridad()
                            {
                                id = itemEsco.id,
                                blnPrincipal = itemEsco.blnPrincipal,
                                id_Cargo = itemEsco.id_Cargo,
                                id_Programa = itemEsco.id_Programa,
                                str_StudiesObservation = itemEsco.str_StudiesObservation
                            };
                            _context.Update(esco);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            OptionEscolaridad esco = new OptionEscolaridad()
                            {
                                id_Cargo = model.Id,
                                blnPrincipal = itemEsco.blnPrincipal,
                                id_Programa = itemEsco.id_Programa,
                                str_StudiesObservation = itemEsco.str_StudiesObservation
                            };

                            _context.OptionEscolaridad.Add(esco);
                            await _context.SaveChangesAsync();
                        }

                    }

                    if (model.CargosRespuestaTarea.Id != 0)
                    {
                        _context.Update(model.CargosRespuestaTarea);
                        _context.SaveChanges();
                    }
                    if (model.CargosConfigurationQuestionSave != null)
                    {

                        List<CargosConfigurationQuestion> QuesRan = model.CargosConfigurationQuestionSave.Where(x => x.Id != 0).ToList();
                        _context.UpdateRange(QuesRan);
                        var result = await _context.SaveChangesAsync();

                        if (result != 1)
                        {
                            json.Succeeded = false;
                            json.Errors.Add(new Errors
                            {
                                Code = "1",
                                Description = "Error actualizando las preguntas"
                            });
                        }



                        foreach (var Ques in model.CargosConfigurationQuestionSave.Where(x => x.Id == 0).ToList())
                        {
                            var obQue = new CargosConfigurationQuestion();
                            obQue.id_Cargo = model.Id;
                            obQue.int_State = 1;
                            obQue.int_Type = Ques.int_Type;
                            obQue.str_Description = Ques.str_Description;
                            obQue.str_Title = Ques.str_Title;
                            _context.CargosConfigurationQuestion.Add(obQue);
                            await _context.SaveChangesAsync();
                            if (Ques.CustList != null)
                            {
                                foreach (var QuesOpt in Ques.CustList)
                                {
                                    var obQuesOp = new CargosQuestionOptions();
                                    obQuesOp.id_questions = obQue.Id;
                                    obQuesOp.str_Options = QuesOpt.Name;
                                    obQuesOp.bln_Excluye = QuesOpt.bln_Excluye;
                                    _context.CargosQuestionOptions.Add(obQuesOp);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                    }


                    json.Succeeded = true;
                    json.Errors.Add(new Errors
                    {
                        Code = string.Empty,
                        Description = string.Empty
                    });
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "0",
                        Description = "Error al consumir el API"
                    });
                }

                return Ok(json);

            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "0",
                    Description = ex.Message
                });
                return Ok(json);
            }

        }

        /// <summary>
        /// List Profile
        /// </summary>
        /// <remarks>
        /// List Profile
        /// </remarks>
        //    [Authorize(Roles = "Admin, ASeleccion")]
        [HttpGet]
        [Route("ListProfile")]
        public Array ListProfile(string str_NitCompany)
        {
            var response = new List<ListarPerfilResponse>();

            var sql = "SELECT perfil.[Id]" +
                ",[User]" +
                ",[Date]" +
                ",rtrim(cargos.nom_car) as cod_car" +
                ",[str_NitCompany]" +
                ",[str_Area]" +
                ",rtrim(jornadas.descripcion) as str_WorkingDay" +
                ",[str_Skills]" +
                "FROM[WEBAPIPruebas].[Selection].[PositionProfile] perfil " +
                "left join NovasoftPruebas.dbo.rhh_cargos cargos on cargos.cod_car = perfil.cod_car " +
                "left join[WEBAPIPruebas].[dbo].[SelectionJornadas] jornadas on jornadas.id = perfil.str_WorkingDay " +
                "where perfil.str_NitCompany = '" + str_NitCompany + "' ";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListarPerfilResponse model = new ListarPerfilResponse();

                model.Id = (int)rdr["Id"];
                model.User = rdr["User"].ToString();
                model.Date = (DateTime)rdr["Date"];
                model.cod_car = rdr["cod_car"].ToString();
                model.str_NitCompany = rdr["str_NitCompany"].ToString();
                model.str_Area = rdr["str_Area"].ToString();
                //  model.str_WorkingDay = rdr["str_WorkingDay"].ToString();
                model.str_Skills = rdr["str_Skills"].ToString();
                response.Add(model);
            }
            conn.Close();
            return response.ToArray();
        }

        // Pruebas

        /// <summary>
        /// Get Cargos Test
        /// </summary>
        /// <remarks>
        /// Get Cargos Test
        /// </remarks>
        [HttpGet]
        [Route("CargosTest")]
        public List<CargosTest> CargosTest()
        {
            List<CargosTest> test = _context.CargosTest.ToList();
            return test;
        }

        /// <summary>
        /// Save Test
        /// </summary>
        /// <remarks>
        /// Save Test
        /// </remarks>
        [HttpPost("[action]")]
        public async Task<OkObjectResult> SaveTest(CargosTest model)
        {
            ResponseJson json = new ResponseJson();
            try
            {
                if (ModelState.IsValid)
                {
                    CargosTest cargosTest = new CargosTest();
                    cargosTest.Name = model.Name;

                    _context.CargosTest.Add(cargosTest);
                    var response = await _context.SaveChangesAsync();

                    if (response == 1)
                    {
                        json.Succeeded = true;
                        json.Errors.Add(new Errors
                        {
                            Code = model.Id.ToString(),
                            Description = "Guardado exitoso"
                        });
                    }
                    else
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Error al consumir la API"
                        });
                    }
                    return Ok(json);
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "1",
                        Description = "Modelo invalido"
                    });
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "1",
                    Description = "error " + ex.Message,
                });
                return Ok(json);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// Delete
        /// </remarks>
        //  [Authorize(Roles = "Admin, ASeleccion")]
        [HttpDelete("[Action]")]
        public async Task<OkObjectResult> TestDelete(int id)
        {
            ResponseJson json = new ResponseJson();
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _context.CargosTest.FindAsync(id);
                    if (data == null)
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "No existe el id especificado"
                        });
                        return Ok(json);
                    }
                    _context.CargosTest.Remove(data);
                    var response = await _context.SaveChangesAsync();
                    if (response == 1)
                    {
                        json.Succeeded = true;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Prueba eliminada"
                        });
                    }
                    else
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Error al consumir la API"
                        });
                    }
                    return Ok(json);
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "1",
                        Description = "Modelo invalido"
                    });
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "1",
                    Description = "error " + ex.Message,
                });
                return Ok(json);
            }
        }

        //Antecedentes

        /// <summary>
        /// Get Cargos Antecedents
        /// </summary>
        /// <remarks>
        /// Get Cargos Antecedents
        /// </remarks>
        [HttpGet]
        [Route("CargosAntecedents")]
        public List<CargosAntecedent> CargosAntecedent()
        {
            List<CargosAntecedent> test = _context.CargosAntecedent.ToList();
            return test;
        }

        /// <summary>
        /// Save Antecedentes
        /// </summary>
        /// <remarks>
        /// Save Antecedentes
        /// </remarks>
        [HttpPost("[action]")]
        public async Task<OkObjectResult> SaveAntecedents(CargosAntecedent model)
        {
            ResponseJson json = new ResponseJson();
            try
            {
                if (ModelState.IsValid)
                {
                    CargosAntecedent offersAntecedents = new CargosAntecedent();
                    offersAntecedents.Name = model.Name;

                    _context.CargosAntecedent.Add(offersAntecedents);
                    var response = await _context.SaveChangesAsync();

                    if (response == 1)
                    {
                        json.Succeeded = true;
                        json.Errors.Add(new Errors
                        {
                            Code = model.Id.ToString(),
                            Description = "Guardado exitoso"
                        });
                    }
                    else
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Error al consumir la API"
                        });
                    }
                    return Ok(json);
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "1",
                        Description = "Modelo invalido"
                    });
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "1",
                    Description = "error " + ex.Message,
                });
                return Ok(json);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// Delete
        /// </remarks>
        //  [Authorize(Roles = "Admin, ASeleccion")]
        [HttpDelete("[Action]")]
        public async Task<OkObjectResult> AntecedentsDelete(int id)
        {
            ResponseJson json = new ResponseJson();
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _context.CargosAntecedent.FindAsync(id);
                    if (data == null)
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "No existe el id especificado"
                        });
                        return Ok(json);
                    }
                    _context.CargosAntecedent.Remove(data);
                    var response = await _context.SaveChangesAsync();
                    if (response == 1)
                    {
                        json.Succeeded = true;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Antecedente eliminado"
                        });
                    }
                    else
                    {
                        json.Succeeded = false;
                        json.Errors.Add(new Errors
                        {
                            Code = "0",
                            Description = "Error al consumir la API"
                        });
                    }
                    return Ok(json);
                }
                else
                {
                    json.Succeeded = false;
                    json.Errors.Add(new Errors
                    {
                        Code = "1",
                        Description = "Modelo invalido"
                    });
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                json.Succeeded = false;
                json.Errors.Add(new Errors
                {
                    Code = "1",
                    Description = "error " + ex.Message,
                });
                return Ok(json);
            }
        }


        /// <summary>
        /// Salary
        /// </summary>
        /// <remarks>
        /// Salary
        /// </remarks>
        //    [Authorize(Roles = "Admin, ASeleccion")]
        [HttpGet]
        [Route("GetSalary")]
        public Array GetSalary()
        {
            var response = new List<SalaryResponse>();
            var sql = "Select nom_varghis.fec_ini,nom_varghis.fec_fin,nom_varghis.val_var " +
                "from NovasoftPruebas.dbo.[nom_vargen] WITH(NOLOCK) " +
                "inner join NovasoftPruebas.dbo.[nom_varghis] WITH(NOLOCK) on[nom_vargen].[num_var] = [nom_varghis].[num_var] " +
                "where rtrim([nom_varghis].[num_var]) like '60' and year(fec_fin)= year(getdate()) ";
            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new SalaryResponse()
                {
                    fec_ini = rdr["fec_ini"].ToString().Trim(),
                    fec_fin = rdr["fec_fin"].ToString().Trim(),
                    val_var = rdr["val_var"].ToString().Trim(),
                });
            }
            conn.Close();
            return response.ToArray();
        }


        /// <summary>
        /// Competencies
        /// </summary>
        /// <remarks>
        /// Competencies
        /// </remarks>
        //    [Authorize(Roles = "Admin, ASeleccion")]
        [HttpGet]
        [Route("Competencies")]
        public Array Competencies()
        {
            var response = new List<CompetenciasResponse>();
            var sql = "Select * From NovasoftPruebas.dbo.GTH_Competencias order by nom_comp asc";
            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new CompetenciasResponse()
                {
                    cod_comp = rdr["cod_comp"].ToString().Trim(),
                    nom_comp = rdr["nom_comp"].ToString().Trim(),
                    def_comp = rdr["def_comp"].ToString().Trim(),
                });
            }
            conn.Close();
            return response.ToArray();
        }

        /// <summary>
        /// Get Cargos Requerimientos
        /// </summary>
        /// <remarks>
        /// Get Cargos Requerimientos
        /// </remarks>
        [HttpGet]
        [Route("CargosRequerimientos")]
        public List<CargosRequerimientos> CargosRequerimientos()
        {
            List<CargosRequerimientos> requerimientos = _context.CargosRequerimientos.ToList();
            return requerimientos;
        }

        /// <summary>
        /// Consultar Rasgos
        /// </summary>
        /// <remarks>
        /// Consultar Rasgos 
        /// </remarks>
        [HttpGet]
        [Route("CargosRasgos")]
        public List<CargosRasgos> CargosRasgos()
        {
            List<CargosRasgos> cargos = _context.CargosRasgos.ToList();
            return cargos;
        }

        /// <summary>
        /// WorkingDay
        /// </summary>
        /// <remarks>
        /// WorkingDay
        /// </remarks>
        //  [Authorize(Roles = "Admin, ASeleccion")]
        [HttpGet]
        [Route("WorkingDay")]
        public Array WorkingDay()
        {
            var response = new List<JornadasResponse>();
            var sql = "SELECT *   FROM [WEBAPI].[dbo].[SelectionJornadas] order by [id] asc";
            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new JornadasResponse()
                {
                    id = rdr["id"].ToString().Trim(),
                    descripcion = rdr["descripcion"].ToString().Trim(),
                });
            }
            conn.Close();
            return response.ToArray();
        }

        /// <summary>
        /// Get CargosConocimientos
        /// </summary>
        /// <remarks>
        /// Get CargosConocimientos
        /// </remarks>
        [HttpGet]
        [Route("CargosConocimientos")]
        public List<CargosConocimientos> CargosConocimientos(int Id_Area)
        {
            List<CargosConocimientos> conocimientos = (List<CargosConocimientos>)_context.CargosConocimientos.Where(x => x.Id_Area == Id_Area).ToList();
            return conocimientos;
        }





        /// <summary>
        /// Get list Position Profiles by offer
        /// </summary>
        /// <remarks>
        /// Get list Position Profiles by offer
        /// </remarks>
        [HttpGet]
        [Route("GetPositionProfilesOffer")]
        public PositionProfile GetPositionProfilesOffer(int id)
        {
            try
            {
                var Position = _context.PositionProfile.Where(x => x.Id == id).FirstOrDefault();

                return Position;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region Escolaridad Programas




        /// <summary>
        /// Get Competencias
        /// </summary>
        /// <remarks>
        /// Get Competencias
        /// </remarks>
        [HttpGet]
        [Route("GetCompetencias")]
        public List<CargosCompetencias> GetCompetencias()
        {
            List<CargosCompetencias> competencias = (List<CargosCompetencias>)_context.CargosCompetencias.ToList();
            return competencias;
        }

        /// <summary>
        /// Get Escolaridad
        /// </summary>
        /// <remarks>
        /// Get Escolaridad
        /// </remarks>
        [HttpGet]
        [Route("GetEscolaridad")]
        public List<CargosEscolaridad> GetEscolaridad()
        {
            List<CargosEscolaridad> escolaridad = (List<CargosEscolaridad>)_context.CargosEscolaridad.ToList();
            return escolaridad;
        }




        /// <summary>
        /// Get Programas
        /// </summary>
        /// <remarks>
        /// Get Programas
        /// </remarks>
        [HttpGet]
        [Route("GetProgramas")]

        public List<CargosProgramas> GetProgramas(int Id_Escolaridad)
        {
            List<CargosProgramas> programas = (List<CargosProgramas>)_context.CargosProgramas.Where(x => x.Id_Escolaridad == Id_Escolaridad).ToList();
            return programas;
        }

        /// <summary>
        /// Get Programas
        /// </summary>
        /// <remarks>
        /// Get Programas
        /// </remarks>
        [HttpGet]
        [Route("GetProgramasById")]

        public List<CargosProgramas> GetProgramasById(int Id)
        {
            List<CargosProgramas> programas = (List<CargosProgramas>)_context.CargosProgramas.Where(x => x.Id == Id).ToList();
            return programas;
        }


        /// <summary>
        /// Get Programas
        /// </summary>
        /// <remarks>
        /// Get Programas
        /// </remarks>
        [HttpGet]
        [Route("GetLicenciaByCargo")]

        public CargosLicencia GetLicenciaByCargo(int Id)
        {
            CargosLicencia Licencia = (CargosLicencia)_context.CargosLicencia.Where(x => x.Id_Cargo == Id).FirstOrDefault();
            return Licencia;
        }





        #endregion


        #region Tareas alto riesgo


        ///// <summary>
        ///// Get Tareas
        ///// </summary>
        ///// <remarks>
        ///// Get Tareas
        ///// </remarks>
        //[HttpGet]
        //[Route("GetTareas")]
        //public List<CargosTarea> GetTareas()
        //{
        //    List<CargosTarea> tareas = _context.CargosTarea.ToList();
        //    return tareas;
        //}

        /// <summary>
        /// Get Vigencia
        /// </summary>
        /// <remarks>
        /// Get Vigencia
        /// </remarks>
        //[HttpGet]
        //[Route("GetVigencia")]
        //public List<CargosVigencia> GetVigencia(int Id_Tarea)
        //{
        //    List<CargosVigencia> vigencia = (List<CargosVigencia>)_context.CargosVigencia.Where(x => x.Id_Tarea == Id_Tarea).ToList();
        //    return vigencia;
        //}

        /// <summary>
        /// Get Formacion
        /// </summary>
        /// <remarks>
        /// Get Formacion
        /// </remarks>
        //[HttpGet]
        //[Route("GetFormacionTareas")]
        //public List<CargosFormacion> GetFormacionTareas()
        //{
        //    List<CargosFormacion> formacion = (List<CargosFormacion>)_context.CargosFormacion.ToList();
        //    return formacion;
        //}

        #endregion
    }
}






