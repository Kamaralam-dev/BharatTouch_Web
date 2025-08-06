using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.ViewModels;

namespace DataAccess.Repository
{
    public class ProfessionalRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertUserProfessional(UserProfessionalModel UserProfessional, out int newUserProfessionalId, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@ProfessionId", UserProfessional.ProfessionId);
                _params.Add("@UserId", UserProfessional.UserId);
                _params.Add("@Title", UserProfessional.Title);
                _params.Add("@Company", UserProfessional.Company);
                _params.Add("@Website", UserProfessional.Website);
                _params.Add("@StartDate", UserProfessional.StartDate);
                _params.Add("@EndDate", UserProfessional.EndDate);
                _params.Add("@IsCurrent", UserProfessional.IsCurrent);
                _params.Add("@Address1", UserProfessional.Address1);
                _params.Add("@Address2", UserProfessional.Address2);
                _params.Add("@City", UserProfessional.City);
                _params.Add("@StateName", UserProfessional.StateName);
                _params.Add("@Zip", UserProfessional.Zip);
                _params.Add("@Country", UserProfessional.Country);
                _params.Add("@Phone", UserProfessional.Phone);
                _params.Add("@NewProfessionId", DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("UserProfssional_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag");

                if (result == 0)
                    newUserProfessionalId = _params.Get<int>("@NewProfessionId");
                else
                    newUserProfessionalId = 0;

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public List<UserProfessionalModel> GetAllUserProfessionals(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<UserProfessionalModel> UserProfessionalList = con.Query<UserProfessionalModel>("UserProfssional_FetchUserID", param, commandType: CommandType.StoredProcedure).ToList();                
                con.Close();

                return UserProfessionalList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteUserProfessional(int Id, string actionName = "")
        {
            bool result = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProfessionId", Id);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("UserProfessional_Delete", param, commandType: CommandType.StoredProcedure);
                con.Close();
                result = param.Get<int>("OutFlag") != 9;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public UserProfessionalModel GetUserProfessionalById(int ProfessionId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ProfessionId", ProfessionId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserProfessionalModel UserProfessionalList = con.Query<UserProfessionalModel>("UserProfessional_Fetch", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return UserProfessionalList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }
      
    }
}
