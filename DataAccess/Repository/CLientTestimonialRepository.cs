using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Models;

namespace DataAccess.Repository
{
    public class CLientTestimonialRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertClientTestimonial(ClientTestimonialModel model, string actionName = "")
        {
            var result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@Client_Id", model.Client_Id);
                _params.Add("@UserId", model.UserId);
                _params.Add("@ClientName", model.ClientName);
                _params.Add("@Designation", model.Designation);
                _params.Add("@CompanyName", model.CompanyName);
                _params.Add("@PicOfClient", model.PicOfClient);
                _params.Add("@Testimonial", model.Testimonial);
                _params.Add("ActionName", actionName);
                _params.Add("@NewClientId", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Client_Testimonail_Upsert", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("NewClientId");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public List<ClientTestimonialModel> GetClientTestimonialBy_UserId(int userId, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", userId);
                _params.Add("ActionName", actionName);
                _params.Add("totalRow", DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<ClientTestimonialModel> list = con.Query<ClientTestimonialModel>("Client_Testimonail_FetchByUserId", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRows = _params.Get<int>("totalRow");
                con.Close();
                return list;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public ClientTestimonialModel GetClientTestimonialBy_Id(int client_Id, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Client_Id", client_Id);
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                ClientTestimonialModel model = con.Query<ClientTestimonialModel>("Client_Testimonail_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteTestimonialBy_Id(int clientId, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Client_Id", clientId);
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Client_Testimonail_DeleteById", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
