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
    public class UserRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertUser(UserModel user, out int newUserId, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", user.UserId);
                _params.Add("@Company", user.Company);
                _params.Add("@CompanyType", user.CompanyType);
                _params.Add("@FirstName", user.FirstName);
                _params.Add("@LastName", user.LastName);
                _params.Add("@Tagline", user.Tagline);
                _params.Add("@EmailID", user.EmailId);
                _params.Add("@Password", user.Password);
                _params.Add("@Phone", user.Phone);
                _params.Add("@Address1", user.Address1);
                _params.Add("@Address2", user.Address2);
                _params.Add("@City", user.City);
                _params.Add("@StateName", user.StateName);
                _params.Add("@Zip", user.Zip);
                _params.Add("@CountryId", user.CountryId);
                _params.Add("@Country", user.Country);
                _params.Add("@Skype", user.Skype);
                _params.Add("@Whatsapp", user.Whatsapp);
                _params.Add("@PortfolioLink", user.PortfolioLink);
                _params.Add("@ResumeLink", user.ResumeLink);
                _params.Add("@ServicesLink", user.ServicesLink);
                _params.Add("@UserType", user.UserType);
                _params.Add("@DisplayName", user.Displayname);
                _params.Add("@ReferredByCode", user.ReferredByCode);
                _params.Add("ActionName", actionName);
                _params.Add("@EmailExist", DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("@NewUserId", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Upsert", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("EmailExist");
                if (result == 0)
                    newUserId = _params.Get<int>("NewUserId");
                else
                    newUserId = 0;

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }
        public int UpsertUser_V2(UserModel user, out int newUserId, out int newOrderId, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("Company", user.Company);
                _params.Add("CompanyType", user.CompanyType);
                _params.Add("FirstName", user.FirstName);
                _params.Add("LastName", user.LastName);
                _params.Add("Tagline", user.Tagline);                
                _params.Add("EmailID", user.EmailId);
                _params.Add("Password", user.Password);
                _params.Add("Phone", user.Phone);
                _params.Add("Address1", user.Address1);
                _params.Add("Address2", user.Address2);
                _params.Add("City", user.City);
                _params.Add("StateName", user.StateName);
                _params.Add("Zip", user.Zip);
                _params.Add("CountryId", user.CountryId);
                _params.Add("Country", user.Country);
                _params.Add("Skype", user.Skype);
                _params.Add("Whatsapp", user.Whatsapp);
                _params.Add("PortfolioLink", user.PortfolioLink);
                _params.Add("ResumeLink", user.ResumeLink);
                _params.Add("ServicesLink", user.ServicesLink);
                _params.Add("UserType", user.UserType);
                _params.Add("DisplayName", user.Displayname);
                //_params.Add("PackageId", user.PackageId);
                _params.Add("ReferredByCode", user.ReferredByCode);
                _params.Add("ActionName", actionName);
                _params.Add("EmailExist", DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("NewUserId", DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("NewOrderId", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Upsert_V2", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("EmailExist");
                if (result == 0)
                    newUserId = _params.Get<int>("NewUserId");
                else
                    newUserId = 0;

                newOrderId = _params.Get<int>("NewOrderId");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public int UpdateUserPackage(UserPackageViewModel model, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", model.UserId);
                _params.Add("PackageId", model.PackageId);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Update_Package", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public int UpdateUserPackage_v1(UserPackageViewModel model, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LoggedUserId", model.LoggedUserId);
                _params.Add("UserId", model.UserId);
                _params.Add("PackageId", model.PackageId);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Update_Package_v1", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public int UpdateUserType(UserModel user, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("UserType", user.UserType);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_UpdateUserType", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public void UpdatePersonalInfo(UserModel user, out int OutFlag, string actionName = "")
        {
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", user.UserId);
                _params.Add("@Company", user.Company);
                _params.Add("@CompanyTypeId", user.CompanyTypeId);
                _params.Add("@CompanyType", user.CompanyType);
                _params.Add("@FirstName", user.FirstName);
                _params.Add("@LastName", user.LastName);
                _params.Add("@CurrentDesignation", user.CurrentDesignation);
                _params.Add("@Tagline", user.Tagline);
                _params.Add("@BirthDate", user.BirthDate);
                _params.Add("@Gender", user.Gender);
                _params.Add("@PortfolioLink", user.PortfolioLink);
                _params.Add("@ResumeLink", user.ResumeLink);
                _params.Add("@ServicesLink", user.ServicesLink);
                _params.Add("@Website", user.Website);
                //_params.Add("@ShowExperience", user.ShowExperience);
                //_params.Add("@ShowEducation", user.ShowEducation);
                //_params.Add("@ShowSocial", user.ShowSocial);
                //_params.Add("@ShowSkill", user.ShowSkill);
                _params.Add("@MenuLink", user.MenuLink);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_PersonalInfo_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutFlag = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        public void UpdateContactInfo(UserModel user, out int OutFlag, string actionName = "")
        {
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", user.UserId);
                _params.Add("@PersonalEmail", user.PersonalEmail);
                _params.Add("@CountryId", user.CountryId);
                _params.Add("@Phone", user.Phone);
                _params.Add("@WhatsAppCountryId", Convert.ToInt32(user.WhatsAppCountryId));
                _params.Add("@Whatsapp", user.Whatsapp);
                _params.Add("@Address1", user.Address1);
                _params.Add("@Address2", user.Address2);
                _params.Add("@City", user.City);
                _params.Add("@StateName", user.StateName);
                _params.Add("@Country", user.Country);
                _params.Add("@Zip", user.Zip);
                _params.Add("@WorkPhoneCountryId", Convert.ToInt32(user.WorkPhoneCountryId));
                _params.Add("@WorkPhone", user.WorkPhone);
                _params.Add("@OtherPhoneCountryId", Convert.ToInt32(user.OtherPhoneCountryId));
                _params.Add("@OtherPhone", user.OtherPhone);
                _params.Add("@OfficeAddress1", user.OfficeAddress1);
                _params.Add("@OfficeAddress2", user.OfficeAddress2);
                _params.Add("@OfficeCity", user.OfficeCity);
                _params.Add("@OfficeStatename", user.OfficeStatename);
                _params.Add("@OfficeCountry", user.OfficeCountry);
                _params.Add("@OfficeZip", user.OfficeZip);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_ContactInfo_Upsert_V1", _params, commandType: CommandType.StoredProcedure);
                OutFlag = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateAboutInfo(UserModel user, out int OutFlag, string actionName = "")
        {
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", user.UserId);
                _params.Add("@AboutDescription", user.AboutDescription);
                _params.Add("@SkillName1", user.SkillName1);
                _params.Add("@SkillName2", user.SkillName2);
                _params.Add("@SkillName3", user.SkillName3);
                _params.Add("@SkillName4", user.SkillName4);
                _params.Add("@SkillName5", user.SkillName5);
                _params.Add("@SkillName6", user.SkillName6);
                _params.Add("@KnowledgePercent1", user.KnowledgePercent1);
                _params.Add("@KnowledgePercent2", user.KnowledgePercent2);
                _params.Add("@KnowledgePercent3", user.KnowledgePercent3);
                _params.Add("@KnowledgePercent4", user.KnowledgePercent4);
                _params.Add("@KnowledgePercent5", user.KnowledgePercent5);
                _params.Add("@KnowledgePercent6", user.KnowledgePercent6);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_AboutInfo_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutFlag = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int AdhaarUpdate(UserModel user, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("AdhaarFrontImgPath", user.AdhaarFrontImgPath);
                _params.Add("AdhaarBackImgPath", user.AdhaarBackImgPath);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_AdharUpdate", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public void InsertNewsLetter(NewsLetterModel model, string actionName = "")
        {
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Email", model.Email);
                _params.Add("ActionName", actionName);
                con.Execute("UserNewsLetter_Insert", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserModel> GetAllUsers(int page, int size, string sortBy, string sortOrder, string searchText, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", page);
                param.Add("size", size);
                param.Add("sortby", sortBy);
                param.Add("sortOrder", sortOrder);
                param.Add("searchText", searchText);
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                IList<UserModel> userList = con.Query<UserModel>("User_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
        public List<UserModel> GetAllUsers(out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                IList<UserModel> userList = con.Query<UserModel>("User_GetALL", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool UpsertUserProfileAndCoverImage(UserModel user, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("ProfileImage", user.ProfileImage);
                _params.Add("CoverImage", user.CoverImage);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Profile_And_Cover_Image_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool UpdateCoverImageProperty(UserModel user, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("BestFitCoverImage", user.BestFitCoverImage);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Update_CoverImage_Property", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool RemoveUserCoverImage(int UserId, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", UserId);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Remove_Cover_Image", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool DeleteUser(int Id, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", Id);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("User_Delete", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Showhideprofilesection(int Id, string type, out int OutFlag, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", Id);
                param.Add("@type", type);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_showhidesection", param, commandType: CommandType.StoredProcedure);
                OutFlag = param.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserModel GetUserById(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserModel userList = con.Query<UserModel>("User_Fetch", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }
        
        public UserModel CheckValidReferalCode(string ReferredByCode, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ReferredByCode", ReferredByCode);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserModel userList = con.Query<UserModel>("Check_Valid_ReferalCode", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public ProfileDetail GetProfileDetail(string UrlCode, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UrlCode", UrlCode);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                ProfileDetail profile = con.Query<ProfileDetail>("User_Profile_Detail", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return profile;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }


        public UserModel AuthenticateUser(UserModel user, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("EmailID", user.EmailId);
                param.Add("Password", user.Password);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserModel userList = con.Query<UserModel>("User_Authenticate_V1", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public UserModel SignWithGoogle(UserModel user, string actionName = "")
        {
            try
            {
                DynamicParameters _param = new DynamicParameters();
                _param.Add("GoogleId", user.GoogleId);
                _param.Add("FirstName", user.FirstName);
                _param.Add("LastName", user.LastName);
                _param.Add("EmailID", user.EmailId);
                _param.Add("ProfileImage", user.ProfileImage);
                _param.Add("IsActive", user.IsActive);
                _param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserModel userList = con.Query<UserModel>("User_Google_SignIn", _param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public UserModel SignWithMicrosoft(UserModel user, string actionName = "")
        {
            try
            {
                DynamicParameters _param = new DynamicParameters();
                _param.Add("MicrosoftId", user.MicrosoftId);
                _param.Add("FirstName", user.FirstName);
                _param.Add("LastName", user.LastName);
                _param.Add("EmailID", user.EmailId);
                _param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserModel userList = con.Query<UserModel>("User_Microsoft_SignIn", _param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool IsExistEmail(string email, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("EmailId", email);
                _params.Add("ActionName", actionName);
                _params.Add("IsExist", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_CheckEmailAvailability", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("IsExist") == 1;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public bool IsExistDisplayName(string DisplayName, string actionName = "")
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
                con.Execute("User_CheckDisplayNameAvailability", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("IsExist") == 1;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public bool UpdatePassword(int userId, string password, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@Password", password);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("User_UpdatePassword", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsVerify(string email, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("EmailId", email);
                _params.Add("ActionName", actionName);
                _params.Add("IsVerified", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Verify_Email", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("IsVerified") == 1;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        
        public int UpdateActiveStatus(UserModel user, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("IsActive", user.IsActive);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_Update_Active_Status_ById", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string ForgetPassword(string email, string actionName = "")
        {
            object result = "";
            try
            {
                connection();
                using (con)
                {
                    con.Open();
                    DynamicParameters _params = new DynamicParameters();
                    _params.Add("EmailId", email);
                    _params.Add("ActionName", actionName);
                    _params.Add("Password", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);
                    con.Execute("Forget_Password", _params, commandType: CommandType.StoredProcedure);
                    result = _params.Get<object>("Password");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.NullToString();
        }
        public int ChangePassword(UserModel user, string actionName = "")
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
                    _params.Add("Password", user.Password);
                    _params.Add("ActionName", actionName);
                    _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    con.Execute("Change_Password", _params, commandType: CommandType.StoredProcedure);
                    result = _params.Get<int>("OutFlag");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public int GetUserIdByCode(string urlCode)
        {
            object result = "";
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UrlCode", urlCode);
                _params.Add("UserId", DbType.String, direction: ParameterDirection.Output);
                con.Execute("User_FetchByCode", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("UserId");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result.ToIntOrZero();
        }

        public bool DeletePersonalFile(int userId, string type, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@Type", type);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("User_PersonalFiles_Delete", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTeam(TeamViewModel model, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", model.UserId);
                _params.Add("@Name", model.Name);
                _params.Add("@Designation", model.Designation);
                _params.Add("@PicturePath", model.PicturePath);
                _params.Add("ActionName", actionName);
                con.Execute("Team_Upsert", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public bool DeleteTeam(int TeamId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@TeamId", TeamId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Team_Delete", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TeamViewModel> GetTeam(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                IList<TeamViewModel> teamList = con.Query<TeamViewModel>("Team_FetchUser", param, commandType: CommandType.StoredProcedure).ToList();
         
                con.Close();

                return teamList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }


        public int ChangeProfileTemplate(UserModel user, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", user.UserId);
                _params.Add("@ProfileTemplateId", user.ProfileTemplateId);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Change_Profile_Template", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public UserModel GetUserByCodeOrName(string codeOrName, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CodeOrName", codeOrName);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserModel userList = con.Query<UserModel>("User_GetByCodeOrName", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public List<UserModel> GetAdminUsers(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<UserModel> userList = con.Query<UserModel>("User_Fetch_Admin", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                con.Close();
                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public int InsertViewHistory(string ipAddress,string browser,string version,string plateForm,string displayName,string state = "", string city = "", string country = "", string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@IPAddress", ipAddress);
                _params.Add("@BrowserName", browser);
                _params.Add("@BrowserVersion", version);
                _params.Add("@PlateForm", plateForm);
                _params.Add("@DisplayName", displayName);
                _params.Add("@State", state);
                _params.Add("@City", city);
                _params.Add("@Country", country);
                _params.Add("ActionName", actionName);
                con.Execute("ViewHistory_Insert", _params, commandType: CommandType.StoredProcedure);
                result = 0;
                con.Close();
            }
            catch (Exception ex)
            {
                result = -1;
                throw ex;
            }
            return result;

        }
        
        public int InsertViewHistory_v1(DeviceInfoModel model, string displayName, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("IPAddress", model.IPAddress);
                _params.Add("BrowserName", model.BrowserName);
                _params.Add("BrowserVersion", model.BrowserVersion);
                _params.Add("PlateForm", model.PlateForm);
                _params.Add("DisplayName", displayName);
                _params.Add("LocationLat", model.LocationLat);
                _params.Add("LocationLon", model.LocationLon);
                _params.Add("ModelName", model.ModelName);
                _params.Add("IsMobile", model.IsMobile);
                _params.Add("City", model.City);
                _params.Add("State", model.State);
                _params.Add("Country", model.Country);
                _params.Add("UserAgent", model.UserAgent);
                _params.Add("ActionName", actionName);
                con.Execute("ViewHistory_Insert_v1", _params, commandType: CommandType.StoredProcedure);
                result = 0;
                con.Close();
            }
            catch (Exception ex)
            {
                result = -1;
                throw ex;
            }
            return result;

        }

        public List<ProfileCompletionEmailModel> GetProfileCompletionUserEmailList()
        {
            try
            {
                connection();
                con.Open();

                IList<ProfileCompletionEmailModel> list = con.Query<ProfileCompletionEmailModel>("Email_CompleteProfile_Request", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return list.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public int UpdateUserEncryptDetail(int userId,string encryptEmailId,string encryptPassword,string EncryptPersonalEmail,string EncryptPhone,string EncryptWhatsapp,string encryptWorkPhone,string encryptOtherPhone)
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", userId);
                _params.Add("EncryptEmailId", encryptEmailId);
                _params.Add("EncryptPassword", encryptPassword);
                _params.Add("EncryptPersonalEmail", EncryptPersonalEmail); //not updating this in proc
                _params.Add("EncryptPhone", EncryptPhone);
                _params.Add("EncryptWhatsapp", EncryptWhatsapp);//not updating this in proc
                _params.Add("EncryptWorkPhone", encryptWorkPhone);
                _params.Add("EncryptOtherPhone", encryptOtherPhone);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("User_EncryptDetail", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        #region User Upi Details
        public bool UpsertUpiDetails(UpiDetailsModel upi, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UpiId", upi.UpiId);
                _params.Add("@PayeeName", upi.PayeeName);
                _params.Add("@QrImage", upi.QrImage);
                _params.Add("@UserId", upi.UserId);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("UpiDetails_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public UpiDetailsModel GetUserUpiDetailByUserId(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UpiDetailsModel upiDetail = con.Query<UpiDetailsModel>("UpiDetails_FetchByUserId", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return upiDetail;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public bool DeleteUserUpiDetailByUserId(int UserId, string actionName = "")
        {
            bool success = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", UserId);
                param.Add("@ActionName", actionName);
                param.Add("@OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("UpiDetails_DeleteByUserId", param, commandType: CommandType.StoredProcedure);
                success = param.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }
        #endregion

        #region Leads
        public bool InsertLead(LeadsViewModel model, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", model.UserId);
                _params.Add("@Name", model.Name);
                _params.Add("@Email", model.Email);
                _params.Add("@PhoneNo", model.PhoneNo);
                _params.Add("@LeadTypeId", model.LeadTypeId);
                _params.Add("@Message", model.Message);
                _params.Add("@Date", model.Date);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Leads_Insert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<LeadsViewModel> FetchLeadsByUserId(int UserId, int page, int size, string sortBy, string sortOrder, string searchText, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("page", page);
                param.Add("size", size);
                param.Add("sortby", sortBy);
                param.Add("sortOrder", sortOrder);
                param.Add("searchText", searchText);
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                IList<LeadsViewModel> leadsList = con.Query<LeadsViewModel>("Leads_FetchAll_ByUserId", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return leadsList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteLeadById(int LeadId, string actionName = "")
        {
            bool isSuccess = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("LeadId", LeadId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Leads_DeleteById", param, commandType: CommandType.StoredProcedure);
                isSuccess = param.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccess;
        }

        #endregion

        #region AviPdf

        public List<AviPdfUserModel> GetAllAviPdfUsers(int page, int size, string sortBy, string sortOrder, string searchText, out int totalRows)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", page);
                param.Add("size", size);
                param.Add("sortby", sortBy);
                param.Add("sortOrder", sortOrder);
                param.Add("searchText", searchText);
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();

                IList<AviPdfUserModel> userList = con.Query<AviPdfUserModel>("Usp_AviPdf_User_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public AviPdfUserModel AviPdfUserLogin(AviPdfUserModel user)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", user.UserId);
                param.Add("UserName", user.UserName);
                param.Add("Email", user.Email);
                param.Add("DeviceID", user.DeviceID);
                param.Add("DeviceType", user.DeviceType);
                param.Add("ProfilePicture", user.ProfilePicture);
                connection();
                con.Open();
                AviPdfUserModel userList = con.Query<AviPdfUserModel>("Usp_AviPdf_USERS", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public AviPdfUserModel AviPdfUserUpdate(AviPdfUserModel user)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Id", user.id);
                param.Add("UserId", user.UserId);
                param.Add("UserName", user.UserName);
                param.Add("Email", user.Email);
                param.Add("DeviceID", user.DeviceID);
                param.Add("DeviceType", user.DeviceType);
                param.Add("ProfilePicture", user.ProfilePicture);
                connection();
                con.Open();
                AviPdfUserModel model = con.Query<AviPdfUserModel>("Usp_AviPdf_User_Update", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public AviPdfUserModel AviPdfUserForgetPassword(AviPdfUserModel user)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Email", user.Email);
                _params.Add("Password", user.Password);
                connection();
                con.Open();
                AviPdfUserModel model = con.Query<AviPdfUserModel>("Usp_AviPdf_User_ForgetPassword", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AviPdfUserDelete(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                connection();
                con.Open();
                con.Execute("Usp_AviPdf_USER_Delete", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AviPdfUserDeleteByUserId(string UserId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Usp_AviPdf_USER_DeleteByUserId", param, commandType: CommandType.StoredProcedure);
                int OutputFlag = param.Get<int>("OutputFlag");
                con.Close();
                return OutputFlag == 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AviPdfUserModel AviPdfUserSignUp(AviPdfUserModel user)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserName", user.UserName);
                _params.Add("Email", user.Email);
                _params.Add("Password", user.Password);
                connection();
                con.Open();
                AviPdfUserModel model = con.Query<AviPdfUserModel>("Usp_AviPdf_USER_Signup", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AviPdfUserModel AviPdfUserLoginWithEmail(AviPdfUserModel user)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Email", user.Email);
                _params.Add("Password", user.Password);
                connection();
                con.Open();
                AviPdfUserModel model = con.Query<AviPdfUserModel>("Usp_AviPdf_USER_Login_With_Email", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AviPdfUserEmailVerification(string Email)
        {
            var result = false;
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Email", Email);
                _params.Add("IsVerify", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Usp_AviPdf_User_VerificationEmail", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("IsVerify") == 1;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool AviPdfUserUpdatePassword(AviPdfUserModel user)
        {
            bool result = false;
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("id", user.id);
                _params.Add("Password", user.Password);
                _params.Add("Output_flag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Usp_AviPdf_User_UpdatePassword", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("Output_flag") == 1;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        #endregion

        #region Temprary
        public UserImagesViewModel GetUserImages(int UserId, string ActionName = "")
        {
            try
            {

                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", UserId);
                _params.Add("ActionName", ActionName);
                connection();

                using (con)
                {
                    con.Open();
                    using (var multi = con.QueryMultiple("Images_fetch_By_UserId", _params, commandType: CommandType.StoredProcedure))
                    {
                        UserModel user = multi.Read<UserModel>().FirstOrDefault();
                        List<TeamViewModel> teamList = multi.Read<TeamViewModel>().ToList();
                        List<ClientTestimonialModel> testimonials = multi.Read<ClientTestimonialModel>().ToList();
                        UpiDetailsModel upiDetail = multi.Read<UpiDetailsModel>().FirstOrDefault();

                        return new UserImagesViewModel
                        {
                            user = user,
                            teams = teamList,
                            clientTestimonials = testimonials,
                            upiDetail = upiDetail
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateUserImages(UserModel user, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", user.UserId);
                _params.Add("ProfileImage", user.ProfileImage);
                _params.Add("CoverImage", user.CoverImage);
                _params.Add("AdhaarFrontImgPath", user.AdhaarFrontImgPath);
                _params.Add("AdhaarBackImgPath", user.AdhaarBackImgPath);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Update_User_Images", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") == 1;
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        
        public bool UpdateUserImagesByType(int userid, int id, string path, string type, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", userid);
                _params.Add("Id", id);
                _params.Add("Path", path);
                _params.Add("Type", type);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Update_User_Images_ByType", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") == 1;
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        #endregion


        public ProfilePermissionViewModel GetProfilePermissionDetails(string urlCode)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UrlCode", urlCode);

                connection();
                con.Open();

                using (var multi = con.QueryMultiple("User_Fetch_PermissionAndDetails", param, commandType: CommandType.StoredProcedure))
                {
                    var model = new ProfilePermissionViewModel
                    {                     
                        SummaryCounts = multi.Read<SummaryCountsViewModel>().FirstOrDefault(),
                        UpiDetails = multi.Read<UpiDetailViewModel>().FirstOrDefault(),
                        SocialMedia = multi.Read<SocialMediaViewModel>().ToList(),
                        OpenWeekDays = multi.Read<OpenWeekDayViewModel>().FirstOrDefault(),
                        OauthTokens = multi.Read<OauthTokenViewModel>().FirstOrDefault(),
                        PackageDetails = multi.Read<PackageDetailsViewModel>().FirstOrDefault(),
                           ProfileDetails = multi.Read<ProfileDetailModel>().FirstOrDefault()
                    };

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Bulk Order

        public bool UpsertBulkOrder(BulkOrderModel model,out int outFlag, out string outMessage)
        {
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("OrderRequestId", model.OrderRequestId == 0 ? (object)null : model.OrderRequestId);
                _params.Add("CompanyName", model.CompanyName);
                _params.Add("Email", model.Email);
                _params.Add("PhoneNo", model.PhoneNo);
                _params.Add("ContactPerson", model.ContactPerson);
                _params.Add("MinOrder", model.MinOrder);
                _params.Add("Message", model.Message);

                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                con.Execute("BulkOrder_Upsert", _params, commandType: CommandType.StoredProcedure);
                outFlag = _params.Get<int>("OutFlag");
                outMessage = _params.Get<string>("OutMessage");
                con.Close();
                return outFlag==0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        public AdminModel GetUserByEmail(string EmailId)
        {
            try
            {
                DynamicParameters _param = new DynamicParameters();
                _param.Add("EmailId", EmailId);
                connection();
                con.Open();
                AdminModel userList = con.Query<AdminModel>("GetUserByEmail", _param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
    }
}
