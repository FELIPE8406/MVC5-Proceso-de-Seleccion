using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using WebApi.Models;
using WebApi.Models.Candidate;



namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Candidate
        [HttpGet]
        public Array Candidate()
        {
            try
            {


                var response = new List<Candidate>();
                var sql = "MI_SP_CandidateGetAll";

                SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = sql;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response.Add(new Candidate()
                    {

                        Id = (Int32)rdr["Id"],
                        cod_emp = rdr["cod_emp"].ToString(),
                        nom_emp = rdr["nom_emp"].ToString(),
                        ap1_emp = rdr["ap1_emp"].ToString(),
                        ap2_emp = rdr["ap2_emp"].ToString(),
                        tip_ide = rdr["tip_ide"].ToString(),
                        pai_exp = rdr["pai_exp"].ToString(),
                        ciu_exp = rdr["ciu_exp"].ToString(),
                        fec_nac = rdr["fec_nac"].ToString(),
                        cod_pai = rdr["cod_pai"].ToString(),
                        cod_dep = rdr["cod_dep"].ToString(),
                        cod_ciu = rdr["cod_ciu"].ToString(),
                        sex_emp = (Int32)rdr["sex_emp"],
                        cla_lib = (Int32)rdr["cla_lib"],
                        dim_lib = (Int32)rdr["dim_lib"],
                        gru_san = rdr["gru_san"].ToString(),
                        fac_rhh = rdr["fac_rhh"].ToString(),
                        est_civ = (Int32)rdr["est_civ"],
                        nac_emp = (Int32)rdr["nac_emp"],
                        dir_res = rdr["dir_res"].ToString(),
                        tel_res = rdr["tel_res"].ToString(),
                        pai_res = rdr["pai_res"].ToString(),
                        nom_pai = rdr["nom_pai"].ToString(),
                        dpt_res = rdr["dpt_res"].ToString(),
                        nom_dep = rdr["nom_dep"].ToString(),
                        ciu_res = rdr["ciu_res"].ToString(),
                        nom_ciu = rdr["nom_ciu"].ToString(),
                        e_mail = rdr["e_mail"].ToString(),
                        tel_cel = rdr["tel_cel"].ToString(),
                        dpt_exp = rdr["dpt_exp"].ToString(),
                        Niv_aca = rdr["Niv_aca"].ToString(),
                        des_est = rdr["des_est"].ToString(),
                        barrio = rdr["barrio"].ToString(),
                        nom1_emp = rdr["nom1_emp"].ToString(),
                        nom2_emp = rdr["nom2_emp"].ToString(),
                        e_mail_alt = rdr["e_mail_alt"].ToString(),
                        asp_sal = (Int32)rdr["asp_sal"],
                        foto = rdr["foto"].ToString()
                    });
                }
                conn.Close();

                return response.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public async Task<ActionResult<Candidate>> CandidateEmail(string e_mail)
        {
            try
            {


                var response = new Candidate();
                var sql = "MI_SP_CandidateGetByEmail";

                SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = sql;
                cmd.Parameters.Add("@e_mail", System.Data.SqlDbType.VarChar, 100).Value = e_mail;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response.Id = (int)rdr["Id"];
                    response.cod_emp = rdr["cod_emp"].ToString();
                    response.nom_emp = rdr["nom_emp"].ToString();
                    response.ap1_emp = rdr["ap1_emp"].ToString();
                    response.ap2_emp = rdr["ap2_emp"].ToString();
                    response.tip_ide = rdr["tip_ide"].ToString();
                    response.pai_exp = rdr["pai_exp"].ToString();
                    response.ciu_exp = rdr["ciu_exp"].ToString();
                    response.fec_nac = rdr["fec_nac"].ToString();
                    response.cod_pai = rdr["cod_pai"].ToString();
                    response.cod_dep = rdr["cod_dep"].ToString();
                    response.cod_ciu = rdr["cod_ciu"].ToString();
                    response.sex_emp = (int)rdr["sex_emp"];
                    response.cla_lib = (int)rdr["cla_lib"];
                    response.dim_lib = (int)rdr["dim_lib"];
                    response.gru_san = rdr["gru_san"].ToString();
                    response.fac_rhh = rdr["fac_rhh"].ToString();
                    response.est_civ = (int)rdr["est_civ"];
                    response.des_civ = rdr["des_civ"].ToString();
                    response.nac_emp = (int)rdr["nac_emp"];
                    response.dir_res = rdr["dir_res"].ToString();
                    response.tel_res = rdr["tel_res"].ToString();
                    response.pai_res = rdr["pai_res"].ToString();
                    response.nom_pai = rdr["nom_pai"].ToString();
                    response.dpt_res = rdr["dpt_res"].ToString();
                    response.nom_dep = rdr["nom_dep"].ToString();
                    response.ciu_res = rdr["ciu_res"].ToString();
                    response.nom_ciu = rdr["nom_ciu"].ToString();
                    response.e_mail = rdr["e_mail"].ToString();
                    response.tel_cel = rdr["tel_cel"].ToString();
                    response.dpt_exp = rdr["dpt_exp"].ToString();
                    response.Niv_aca = rdr["Niv_aca"].ToString();
                    response.des_est = rdr["des_est"].ToString();
                    response.barrio = rdr["barrio"].ToString();
                    response.nom1_emp = rdr["nom1_emp"].ToString();
                    response.nom2_emp = rdr["nom2_emp"].ToString();
                    response.e_mail_alt = rdr["e_mail_alt"].ToString();
                    response.asp_sal = (Int32)rdr["asp_sal"];
                    response.foto = rdr["foto"].ToString();
                }
                conn.Close();

                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public async Task<ActionResult<Candidate>> CandidateCod_emp(string cod_emp)
        {
            var response = new Candidate();
            var sql = "MI_SP_CandidateGetBycod_emp";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            cmd.Parameters.Add("@cod_emp", System.Data.SqlDbType.VarChar, 100).Value = cod_emp;

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Id = (int)rdr["Id"];
                response.cod_emp = rdr["cod_emp"].ToString();
                response.nom_emp = rdr["nom_emp"].ToString();
                response.ap1_emp = rdr["ap1_emp"].ToString();
                response.ap2_emp = rdr["ap2_emp"].ToString();
                response.tip_ide = rdr["tip_ide"].ToString();
                response.pai_exp = rdr["pai_exp"].ToString();
                response.ciu_exp = rdr["ciu_exp"].ToString();
                response.fec_nac = rdr["fec_nac"].ToString();
                response.cod_pai = rdr["cod_pai"].ToString();
                response.cod_dep = rdr["cod_dep"].ToString();
                response.cod_ciu = rdr["cod_ciu"].ToString();
                response.sex_emp = (int)rdr["sex_emp"];
                response.cla_lib = (int)rdr["cla_lib"];
                response.dim_lib = (int)rdr["dim_lib"];
                response.gru_san = rdr["gru_san"].ToString();
                response.fac_rhh = rdr["fac_rhh"].ToString();
                response.est_civ = (int)rdr["est_civ"];
                response.des_civ = rdr["des_civ"].ToString();
                response.nac_emp = (int)rdr["nac_emp"];
                response.dir_res = rdr["dir_res"].ToString();
                response.tel_res = rdr["tel_res"].ToString();
                response.pai_res = rdr["pai_res"].ToString();
                response.nom_pai = rdr["nom_pai"].ToString();
                response.dpt_res = rdr["dpt_res"].ToString();
                response.nom_dep = rdr["nom_dep"].ToString();
                response.ciu_res = rdr["ciu_res"].ToString();
                response.nom_ciu = rdr["nom_ciu"].ToString();
                response.e_mail = rdr["e_mail"].ToString();
                response.tel_cel = rdr["tel_cel"].ToString();
                response.dpt_exp = rdr["dpt_exp"].ToString();
                response.Niv_aca = rdr["Niv_aca"].ToString();
                response.des_est = rdr["des_est"].ToString();
                response.barrio = rdr["barrio"].ToString();
                response.nom1_emp = rdr["nom1_emp"].ToString();
                response.nom2_emp = rdr["nom2_emp"].ToString();
                response.e_mail_alt = rdr["e_mail_alt"].ToString();
                response.asp_sal = (int)rdr["asp_sal "];
                response.foto = rdr["foto"].ToString();
            }
            conn.Close();

            return response;
        }


        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public async Task<ActionResult<CandidateExtra>> CandidateExtra(string cod_emp)
        {
            var response = new CandidateExtra();
            var sql = @"select cod_emp,str_Titu,str_Desc,int_Eps,int_Pension
                        from Bolsa_empleo..CandidateExtra
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.cod_emp = rdr["cod_emp"].ToString();
                response.str_Titu = rdr["str_Titu"].ToString();
                response.str_Desc = rdr["str_Desc"].ToString();
                response.int_Eps = (Int32)rdr["int_Eps"];
                response.int_Pension = (Int32)rdr["int_Pension"];

            }
            conn.Close();

            return response;
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public List<ExperienciaLaboral> ExperienciaLaboral(string cod_emp)
        {
            var response = new List<ExperienciaLaboral>();
            var sql = @"select Id,cod_emp,nom_empr,nom_car,area_exp,des_fun,log_car,tpo_exp,mot_ret,jefe_inm,num_tel,sal_bas,fec_ing,fec_ter
                        from Bolsa_empleo..ExperienciaLaboral
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new ExperienciaLaboral()
                {

                    Id = (int)rdr["Id"],
                    cod_emp = rdr["cod_emp"].ToString(),
                    nom_empr = rdr["nom_empr"].ToString(),
                    nom_car = rdr["nom_car"].ToString(),
                    area_exp = rdr["area_exp"].ToString(),
                    des_fun = rdr["des_fun"].ToString(),
                    log_car = rdr["log_car"].ToString(),
                    tpo_exp = rdr["tpo_exp"].ToString(),
                    mot_ret = rdr["mot_ret"].ToString(),
                    jefe_inm = rdr["jefe_inm"].ToString(),
                    num_tel = rdr["num_tel"].ToString(),
                    sal_bas = rdr["sal_bas"].ToString(),
                    fec_ing = rdr["fec_ing"].ToString(),
                    fec_ter = rdr["fec_ter"].ToString()
                    //response.status = rdr["ap1_emp"];
                    //response.novasoft = (int)rdr["ap1_emp"];
                });
            }
            conn.Close();

            return response;
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public List<Studies> Studies(string cod_emp)
        {
            var response = new List<Studies>();
            var sql = @"select Id,cod_emp,cod_est,nom_est,cod_ins,ano_est,sem_apr,hor_est,gra_son,fec_gra
                        from Bolsa_empleo..Studies
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new Studies()
                {
                    Id = (Int32)rdr["Id"],
                    cod_emp = rdr["cod_emp"].ToString(),
                    cod_est = rdr["cod_est"].ToString(),
                    nom_est = rdr["nom_est"].ToString(),
                    cod_ins = rdr["cod_ins"].ToString(),
                    ano_est = (Int32)rdr["ano_est"],
                    sem_apr = (Int32)rdr["sem_apr"],
                    hor_est = (Int32)rdr["hor_est"],
                    gra_son = rdr["gra_son"].ToString(),
                    fec_gra = rdr["fec_gra"].ToString()
                    //response.status = rdr["ap1_emp"];
                    //response.novasoft = (int)rdr["ap1_emp"];
                });
            }
            conn.Close();

            return response;
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public List<References> References(string cod_emp)
        {
            var response = new List<References>();
            var sql = @"SELECT Id,cod_emp,num_ref,tip_ref,nom_ref,ocu_ref,ent_ref,parent,tel_ref,cel_ref,dir_ref,status,novasoft
                        FROM Bolsa_empleo.dbo.[References]
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new References()
                {
                    Id = (Int32)rdr["Id"],
                    cod_emp = rdr["cod_emp"].ToString(),
                    num_ref = (Int32)rdr["num_ref"],
                    tip_ref = rdr["tip_ref"].ToString(),
                    nom_ref = rdr["nom_ref"].ToString(),
                    ocu_ref = rdr["ocu_ref"].ToString(),
                    ent_ref = rdr["ent_ref"].ToString(),
                    parent = rdr["parent"].ToString(),
                    tel_ref = rdr["tel_ref"].ToString(),
                    cel_ref = rdr["cel_ref"].ToString(),
                    dir_ref = rdr["dir_ref"].ToString()
                    //response.status = rdr["ap1_emp"];
                    //response.novasoft = (int)rdr["ap1_emp"];
                });
            }
            conn.Close();

            return response;
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public List<Language> Language(string cod_emp)
        {
            var response = new List<Language>();
            var sql = @"select Id,cod_emp,cod_idi,cod_calif,cod_hab
                        from Bolsa_empleo..Language
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new Language()
                {
                    Id = (Int32)rdr["Id"],
                    cod_emp = rdr["cod_emp"].ToString(),
                    cod_idi = rdr["cod_idi"].ToString(),
                    cod_calif = rdr["cod_calif"].ToString(),
                    cod_hab = rdr["cod_hab"].ToString()

                    //response.status = rdr["ap1_emp"];
                    //response.novasoft = (int)rdr["ap1_emp"];
                });
            }
            conn.Close();

            return response;
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public List<Familiares> Family(string cod_emp)
        {
            var response = new List<Familiares>();
            var sql = @"select Id,cod_emp,ap1_fam,ap2_fam,nom_fam,tip_fam,gra_san,tip_ide,num_ced,fec_nac,sex_fam,est_civ,niv_est,ind_comp,
                        ocu_fam,sal_bas,ind_sub,ind_pro,ind_conv,sex_famANT,edad_ec,Ind_disc_ec,Ind_Dep_Ret
                        from Bolsa_empleo..Familiares
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new Familiares()
                {


                    Id = (Int32)rdr["Id"],
                    cod_emp = rdr["cod_emp"].ToString(),
                    ap1_fam = rdr["ap1_fam"].ToString(),
                    ap2_fam = rdr["ap2_fam"].ToString(),
                    nom_fam = rdr["nom_fam"].ToString(),
                    tip_fam = rdr["tip_fam"].ToString(),
                    gra_san = rdr["gra_san"].ToString(),
                    tip_ide = rdr["tip_ide"].ToString(),
                    num_ced = rdr["num_ced"].ToString(),
                    fec_nac = rdr["fec_nac"].ToString(),
                    sex_fam = rdr["sex_fam"].ToString(),
                    est_civ = rdr["est_civ"].ToString(),
                    niv_est = rdr["niv_est"].ToString(),
                    ind_comp = rdr["ind_comp"].ToString(),
                    ocu_fam = rdr["ocu_fam"].ToString(),
                    sal_bas = (Int32)rdr["sal_bas"],
                    ind_sub = rdr["ind_sub"].ToString(),
                    ind_pro = rdr["ind_pro"].ToString(),
                    ind_conv = rdr["ind_conv"].ToString(),
                    sex_famANT = (Int32)rdr["sex_famANT"],
                    edad_ec = (Int32)rdr["edad_ec"],
                    Ind_disc_ec = (Int32)rdr["Ind_disc_ec"],
                    Ind_Dep_Ret = (Int32)rdr["Ind_Dep_Ret"]
                    //status = (Int32)rdr["ap1_emp"],
                    //novasoft = rdr["ap1_emp"].ToString(),

                    //response.status = rdr["ap1_emp"];
                    //response.novasoft = (int)rdr["ap1_emp"];
                });
            }
            conn.Close();

            return response;
        }

        // GET: api/Candidate/7
        [HttpGet("[Action]")]
        public List<Habilidades> Habilidades(string cod_emp)
        {
            var response = new List<Habilidades>();
            var sql = @"select Id,str_Habi,str_Desc,cod_emp 
                        from Bolsa_empleo..habilidades
                        where cod_emp='" + cod_emp + "'";

            SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = sql;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                response.Add(new Habilidades()
                {


                    Id = (Int32)rdr["Id"],
                    cod_emp = rdr["cod_emp"].ToString(),
                    str_Habi = rdr["str_Habi"].ToString(),
                    str_Desc = rdr["str_Desc"].ToString()

                });
            }
            conn.Close();

            return response;
        }
        [HttpPost("[action]")]
        public string NewCandidate(InputModel model)
        {
           //IRestResponse response = new ApiUrlController().UrlExecute("https://localhost:44359/api/user/CreateNewCandidate", "", "POST", JsonConvert.SerializeObject(model));
            IRestResponse response = new ApiUrlController().UrlExecute("https://empleospruebas.misionempresarial.com/api/user/CreateNewCandidate", "", "POST", JsonConvert.SerializeObject(model));

            if (response.StatusDescription == "OK")
            {
                return response.Content;
            }
            else
            {
                var jsonresponse = new
                {
                    state = false,
                    message = "Error al consumir el Login PE",
                };

                return JsonConvert.SerializeObject(jsonresponse);
            }

            
        }
    }
}
