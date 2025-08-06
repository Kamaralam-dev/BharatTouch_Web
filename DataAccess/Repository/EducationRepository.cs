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
    public class EducationRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertUserEducation(UserEducationModel UserEducation, out int newUserEducationId, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@EducationId", UserEducation.EducationId);
                _params.Add("@UserId", UserEducation.UserId);
                _params.Add("@Institute", UserEducation.Institute);
                _params.Add("@University", UserEducation.University);
                _params.Add("@Degree", UserEducation.Degree);
                _params.Add("@Marks", UserEducation.Marks);
                _params.Add("@PassYear", UserEducation.PassYear);
                _params.Add("@Specialization", UserEducation.Specialization);
                _params.Add("@StartDate", UserEducation.StartDate);
                _params.Add("@EndDate", UserEducation.EndDate);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("@NewEducationId", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("UserEducation_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag");

                if (result == 0)
                    newUserEducationId = _params.Get<int>("@NewEducationId");
                else
                    newUserEducationId = 0;

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public List<UserEducationModel> GetAllUserEducations(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<UserEducationModel> UserEducationList = con.Query<UserEducationModel>("UserEducation_FetchUserID", param, commandType: CommandType.StoredProcedure).ToList();                
                con.Close();

                return UserEducationList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteUserEducation(int Id, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@EducationId", Id);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("UserEducation_Delete", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserEducationModel GetUserEducationById(int EducationId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("EducationId", EducationId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserEducationModel UserEducationList = con.Query<UserEducationModel>("UserEducation_Fetch", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return UserEducationList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }
      
    }
}
