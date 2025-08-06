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
using DataAccess.ViewModels;

namespace DataAccess.Repository
{
    public class CompanyRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public AdminModel AuthenticateCompanyAdmin(AdminModel model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("EmailId", model.EmailId);
                param.Add("Password", model.Password);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                AdminModel userList = con.Query<AdminModel>("User_Authenticate_Company_Admin", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public bool Company_Contact_Upsert(CompanyViewModel model,out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", model.CompanyId);
                param.Add("CompanyName", model.CompanyName);
                param.Add("Address1", model.Address1);
                param.Add("Address2", model.Address2);
                param.Add("City", model.City);
                param.Add("State", model.State);
                param.Add("Zip", model.Zip);
                param.Add("Country", model.Country);
                param.Add("PhoneCountryId", model.PhoneCountryId);
                param.Add("Phone", model.Phone);
                param.Add("Email", model.Email);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Company_Contact_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();
                return outFlag == 0;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public bool Company_SocialMedia_Upsert(CompanyViewModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", model.CompanyId);
                param.Add("Facebook", model.Facebook);
                param.Add("LinkedIn", model.LinkedIn);
                param.Add("Twitter", model.Twitter);
                param.Add("Instagram", model.Instagram);
                param.Add("Youtube", model.Youtube);
                param.Add("Website", model.Website);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Company_SocialMedia_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();
                return outFlag == 0;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public bool Company_About_Upsert(CompanyViewModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", model.CompanyId);
                param.Add("Tagline", model.Tagline);
                param.Add("AboutDescription", model.AboutDescription);
                param.Add("SkillName1", model.SkillName1);
                param.Add("SkillName2", model.SkillName2);
                param.Add("SkillName3", model.SkillName3);
                param.Add("SkillName4", model.SkillName4);
                param.Add("SkillName5", model.SkillName5);
                param.Add("SkillName6", model.SkillName6);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Company_About_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();
                return outFlag == 0;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public bool Company_CoverImage_Upsert(CompanyViewModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", model.CompanyId);
                param.Add("CoverImagepath", model.CoverImagepath);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Company_CoverImage_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();
                return outFlag == 0;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public int ChangeCompanyAdminPassword(UserModel user, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                using (con)
                {
                    con.Open();
                    DynamicParameters _params = new DynamicParameters();
                    _params.Add("UserId", user.UserId);
                    _params.Add("CompanyId", user.CompanyId);
                    _params.Add("Password", user.Password);
                    _params.Add("ActionName", actionName);
                    _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    con.Execute("Change_CompanyAdmin_Password", _params, commandType: CommandType.StoredProcedure);
                    result = _params.Get<int>("OutFlag");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<UserModel> GetAllUsersByCompany(int CompanyId, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", CompanyId);
                param.Add("ActionName", actionName);
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();

                IList<UserModel> userList = con.Query<UserModel>("User_FetchAll_ByCompany", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool Company_User_Upsert(UserModel model, out int outFlag, out string outMessage,out int OutUserId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("CompanyId", model.CompanyId);
                param.Add("Displayname", model.Displayname);
                param.Add("CurrentDesignation", model.CurrentDesignation);
                param.Add("EmailId", model.EmailId);
                param.Add("CountryId", model.CountryId);
                param.Add("Phone", model.Phone);
                param.Add("FirstName", model.FirstName);
                param.Add("LastName", model.LastName);
                param.Add("IsCompanyAdmin", model.IsCompanyAdmin);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);
                param.Add("OutUserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Company_User_Upsert", param, commandType: CommandType.StoredProcedure);

                OutUserId = param.Get<int>("OutUserId");
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();
                return outFlag == 0;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public List<ViewHistoryViewModel> GetViewHistoryByCompanyUser(int UserId, out int totalRows)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();

                IList<ViewHistoryViewModel> list = con.Query<ViewHistoryViewModel>("ViewHistory_FetchAll_ByCompanyUserId", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return list.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool IsExistCompanyUserDisplayName(string DisplayName, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@DisplayName", DisplayName);
                _params.Add("ActionName", actionName);
                _params.Add("IsExist", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_CheckCompanyUserDisplayNameAvailability", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("IsExist") == 1;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
