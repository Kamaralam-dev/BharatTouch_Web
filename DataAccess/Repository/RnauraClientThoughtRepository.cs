using Dapper;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RnauraClientThoughtRepository
    {
        SqlConnection con;

        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public void UpsertClientThought(string loggerEmail, RnauraClientThoughtModel model, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("loggerEmail", loggerEmail);
                _params.Add("ClientId", model.ClientId);
                _params.Add("Name", model.Name);
                _params.Add("Designation", model.Designation);
                _params.Add("Comment", model.Comment);
                _params.Add("Image", model.Image);
                _params.Add("DisplayPosition", model.DisplayPosition);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_ClientThoughts_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateClientThoughtPosition(string loggerEmail, RnauraClientThoughtModel model, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@loggerEmail", loggerEmail);
                _params.Add("@ClientId", model.ClientId);
                _params.Add("@DisplayPosition", model.DisplayPosition);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_ClientThoughts_UpdatePosition", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RnauraClientThoughtModel FetchClientThoughtById(int ClientId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("ClientId", ClientId);
                connection();
                con.Open();
                RnauraClientThoughtModel model = con.Query<RnauraClientThoughtModel>("Rnaura_ClientThoughts_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraClientThoughtModel> FetchClientThoughtAll(out int totalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraClientThoughtModel> model = con.Query<RnauraClientThoughtModel>("Rnaura_ClientThoughts_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRow = _params.Get<int>("totalRow");
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteClientThoughtById(string loggerEmail, int ClientId, out int OutputFlag)
        {
            try
            {
                int flag;
                DynamicParameters _params = new DynamicParameters();
                _params.Add("loggerEmail", loggerEmail);
                _params.Add("ClientId", ClientId);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_ClientThoughts_Delete", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
