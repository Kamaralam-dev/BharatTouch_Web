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
    public class RnauraClientRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public RnauraClientModel GetClientById(int clientId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("clientId", clientId);
                connection();
                con.Open();
                RnauraClientModel model = con.Query<RnauraClientModel>("Rnaura_Client_Fetch", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraClientModel> GetAllClients(out int TotalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraClientModel>  models = con.Query<RnauraClientModel>("Rnaura_Client_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                TotalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void ClientInsert(RnauraClientModel client, out int newClientId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Name", client.Name);
                _params.Add("Email", client.Email);
                _params.Add("PhoneNo", client.PhoneNo);
                _params.Add("AboutProject", client.AboutProject);
                _params.Add("NewClientId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Client_Insert", _params, commandType: CommandType.StoredProcedure);
                newClientId = _params.Get<int>("NewClientId");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteClient(string loggerEmail, int id, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LoggerEmail", loggerEmail);
                _params.Add("clientId", id);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Client_Delete", _params, commandType: CommandType.StoredProcedure);
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
