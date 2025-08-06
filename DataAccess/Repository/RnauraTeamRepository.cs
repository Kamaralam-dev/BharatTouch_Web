using Dapper;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RnauraTeamRepository
    {
        SqlConnection con;

        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public void UpsertTeam(string loggerEmail, RnauraTeamModel model, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("loggerEmail", loggerEmail);
                _params.Add("TeamId", model.TeamId);
                _params.Add("Name", model.Name);
                _params.Add("Designation", model.Designation);
                _params.Add("About", model.About);
                _params.Add("Image", model.Image);
                _params.Add("DisplayPosition", model.DisplayPosition);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Team_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateTeamPosition(string loggerEmail, RnauraTeamModel model, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@loggerEmail", loggerEmail);
                _params.Add("@TeamId", model.TeamId);
                _params.Add("@DisplayPosition", model.DisplayPosition);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Team_UpdatePosition", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public RnauraTeamModel FetchTeamMemberById(int TeamId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("TeamId", TeamId);
                connection();
                con.Open();
                RnauraTeamModel model = con.Query<RnauraTeamModel>("Rnaura_Team_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraTeamModel> FetchTeamAll(out int totalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraTeamModel> model = con.Query<RnauraTeamModel>("Rnaura_Team_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRow = _params.Get<int>("totalRow");
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteTeamMemberById(string loggerEmail, int TeamId, out int OutputFlag)
        {
            try
            {
                int flag;
                DynamicParameters _params = new DynamicParameters();
                _params.Add("loggerEmail", loggerEmail);
                _params.Add("TeamId", TeamId);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Team_Delete", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
