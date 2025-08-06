using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Models;
using DataAccess.ViewModels;

namespace DataAccess.Repository
{
    public class PackagesRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public UserPackageViewModel GetPackageByUserId(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserPackageViewModel model = con.Query<UserPackageViewModel>("User_Package_GetByUserId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public UserPackageViewModel GetPackageByCodeOrName(string code, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CodeOrName", code);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserPackageViewModel model = con.Query<UserPackageViewModel>("User_Package_GetByCodeOrName", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<UserPackageViewModel> GetAllPackages()
        {
            try
            {
                connection();
                con.Open();
                List<UserPackageViewModel> model = con.Query<UserPackageViewModel>("Packages_FetchAll", commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeletePackage(int packageId, int  deletedBy, out int outFlag,out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("PackageId", packageId);
                param.Add("DeletedBy", deletedBy);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Packages_Delete", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserPackageViewModel GetPackageById(int packageId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("PackageId", packageId);
                connection();
                con.Open();
                UserPackageViewModel model = con.Query<UserPackageViewModel>("Packages_ById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool UpsertPackage(UserPackageViewModel model, out int outFlag, out string outMessage, out int outPackageId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("PackageId", model.PackageId == 0 ? (int?)null : model.PackageId);
                param.Add("PackageName", model.PackageName);
                param.Add("PackageDescription", model.PackageDescription);
                param.Add("PackageCost", model.PackageCost);
                param.Add("AllowedImages", model.AllowedImages);
                param.Add("AllowedVideos", model.AllowedVideos);
                param.Add("AllowContactUsSection", model.AllowContactUsSection);
                param.Add("AllowCalendarSection", model.AllowCalendarSection);
                param.Add("AllowBlogSection", model.AllowBlogSection);
                param.Add("AllowTeamSection", model.AllowTeamSection);
                param.Add("AllowUploadDetailSection", model.AllowUploadDetailSection);
                param.Add("AllowTestimonialSection", model.AllowTestimonialSection);
                param.Add("AllowTrainingCertificateSection", model.AllowTrainingCertificateSection);
                param.Add("AllowProfileViewAnalytics", model.AllowProfileViewAnalytics);
                param.Add("AllowSocialMedia", model.AllowSocialMedia);
                param.Add("AllowAdhaarCard", model.AllowAdhaarCard);
                param.Add("AllowedEducationSection", model.AllowedEducationSection);
                param.Add("AllowedExperienceSection", model.AllowedExperienceSection);
                param.Add("AllowedPaymentSection", model.AllowedPaymentSection);
                param.Add("AllowedProfileTemplateSection", model.AllowedProfileTemplateSection);
                param.Add("CreatedBy", model.CreatedBy);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                param.Add("OutPackageId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Packages_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                outPackageId = param.Get<int>("OutPackageId");

                con.Close();

                return outFlag == 0;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1; outPackageId = 0;
                outMessage = $"An error occurred: {ex.Message}";
                return false;
            }
        }

        public List<UserModel> GetUserHavePackage(int packageId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("PackageId", packageId);
                connection();
                con.Open();
                List<UserModel> model = con.Query<UserModel>("GetUser_HavePackage", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }


        public List<dynamic> PaginatedPackageUsers(PaginationModel pagination, int packageId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", pagination.Page);
                param.Add("size", pagination.Size);
                param.Add("sortby", pagination.SortBy);
                param.Add("sortOrder", pagination.SortOrder);
                param.Add("searchText", pagination.SearchText);
                param.Add("PackageId", packageId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<dynamic> model = con.Query<dynamic>("Paginated_Pacakage_Users", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<UserPackageHistoryViewModel> GetUserPackagehistoryByUserId(int userId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                connection();
                con.Open();
                List<UserPackageHistoryViewModel> model = con.Query<UserPackageHistoryViewModel>("User_Packages_histry_FetchBy_UserId", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<UserPackageHistoryViewModel> PaginatedUserPlanChangeHistory(PaginationModel pagination, int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", pagination.Page);
                param.Add("size", pagination.Size);
                param.Add("sortby", pagination.SortBy);
                param.Add("sortOrder", pagination.SortOrder);
                param.Add("searchText", pagination.SearchText);
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<UserPackageHistoryViewModel> model = con.Query<UserPackageHistoryViewModel>("Paginated_User_PlanChange_History", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
    }
}
