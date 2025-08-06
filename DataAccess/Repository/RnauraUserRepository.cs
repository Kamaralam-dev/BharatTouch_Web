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
    public class RnauraUserRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public RnauraUserModel loggerDetails(string email)
        {
            try
            {
                DynamicParameters _param = new DynamicParameters();
                _param.Add("LoggerEmail", email);
                connection();
                con.Open();
                RnauraUserModel model = con.Query<RnauraUserModel>("Rnaura_User_LoggerDetails", _param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public RnauraUserModel AuthenticateUser(RnauraUserModel user)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Email", user.Email);
                param.Add("Password", user.Password);
                connection();
                con.Open();
                RnauraUserModel userList = con.Query<RnauraUserModel>("Rnaura_User_Authenticate", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public RnauraUserModel GetUserById(int UserId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", UserId);
                connection();
                con.Open();
                RnauraUserModel model = con.Query<RnauraUserModel>("Rnaura_User_Fetch", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraUserModel> GetAllUsers(out int TotalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraUserModel> models = con.Query<RnauraUserModel>("Rnaura_User_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                TotalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpserUser(RnauraUserModel user, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("FirstName", user.FirstName);
                _params.Add("LastName", user.LastName);
                _params.Add("DateOfBirth", user.DateOfBirth);
                _params.Add("Gender", user.Gender);
                _params.Add("Email", user.Email);
                _params.Add("Password", user.Password);
                _params.Add("PhoneNumber", user.PhoneNumber);
                _params.Add("Address", user.Address);
                _params.Add("City", user.City);
                _params.Add("State", user.State);
                _params.Add("ZipCode", user.ZipCode);
                _params.Add("SuperViserId", user.SuperviserId);
                _params.Add("Position", user.Position);
                _params.Add("DateOfHire", user.DateOfHire);
                _params.Add("UserStatus", user.UserStatus);
                _params.Add("UserTypeId", user.UserTypeId);
                _params.Add("ProfilePicture", user.ProfilePicture);
                _params.Add("TerminationDate", user.TerminationDate);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_User_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteUser(string loggerEmail, int UserId, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LoggerEmail", loggerEmail);
                _params.Add("UserId", UserId);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_User_Delete", _params, commandType: CommandType.StoredProcedure);
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
