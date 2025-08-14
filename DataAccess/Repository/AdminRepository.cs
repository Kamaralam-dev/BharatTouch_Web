using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dapper;
using DataAccess.AdminApiDto;
using DataAccess.Models;
using DataAccess.ViewModels;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DataAccess.Repository
{
    public class AdminRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public AdminModel Authenticate_Admin(AdminModel model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("EmailId", model.EmailId);
                param.Add("Password", model.Password);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                AdminModel userList = con.Query<AdminModel>("User_Authenticate_Admin", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public List<UserModel> GetAllUsers_Admin(int page, int size, string sortBy, string sortOrder, string searchText, out int totalRows, string actionName = "")
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

        public UserCompleteProfileViewModel UserCompleteProfile(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                connection();
                using (con)
                {
                    con.Open();
                    using (var multi = con.QueryMultiple("Complete_Profile_Details_ByUserId", param, commandType: CommandType.StoredProcedure))
                    {
                        UserCompleteProfileViewModel model = multi.Read<UserCompleteProfileViewModel>().FirstOrDefault();
                        model.Package = multi.Read<UserPackageViewModel>().FirstOrDefault();
                        model.SocialMedia = multi.Read<SocialMediaModel>().ToList();
                        model.Educations = multi.Read<UserEducationModel>().ToList();
                        model.Experiencee = multi.Read<UserProfessionalModel>().ToList();
                        model.TrainingCertification = multi.Read<UserCertificationModel>().ToList();
                        model.ClientTestimonials = multi.Read<ClientTestimonialModel>().ToList();
                        model.Teams = multi.Read<TeamViewModel>().ToList();
                        model.YoutubeVideos = multi.Read<YouTubeModel>().ToList();
                        model.UpiDetails = multi.Read<UpiDetailsModel>().FirstOrDefault();
                        model.Blogs = multi.Read<BlogModel>().ToList();
                        model.MeetingWeedDays = multi.Read<ScheduleOpenWeekDayModel>().FirstOrDefault();

                        return model;
                    }
                }
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteUser_Admin(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", UserId);
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

        public List<UserHistoryModel> GetAllUserHistory_Admin(int UserId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("UserId", UserId);
                connection();
                con.Open();

                IList<UserHistoryModel> userList = con.Query<UserHistoryModel>("User_ViewHistory", param, commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<UserHistoryModel> PaginatedUserHistory(PaginationModel model, int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("page", model.Page);
                param.Add("size", model.Size);
                param.Add("sortby", model.SortBy);
                param.Add("sortOrder", model.SortOrder);
                param.Add("searchText", model.SearchText);
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                IList<UserHistoryModel> userList = con.Query<UserHistoryModel>("Paginated_User_ViewHistory", param, commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<UserModel> UserReferredByCode(string ReferalCode)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ReferalCode", ReferalCode);
                connection();
                con.Open();

                IList<UserModel> userList = con.Query<UserModel>("User_ReferredByCode", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }


        public List<dynamic> PaginatedReferredUsers(PaginationModel model, string ReferalCode, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ReferalCode", ReferalCode);
                param.Add("@page", model.Page);
                param.Add("@size", model.Size);
                param.Add("@sortby", model.SortBy);
                param.Add("@sortOrder", model.SortOrder);
                param.Add("@searchText", model.SearchText);
                param.Add("@ActionName", actionName);
                connection();
                con.Open();
                IList<dynamic> list = con.Query<dynamic>("Paginated_Referred_Users", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();
                return list.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #region order

        public List<OrderAdminViewModel> GetAllOrder_Admin()
        {
            try
            {
                connection();
                con.Open();
                IList<OrderAdminViewModel> order = con.Query<OrderAdminViewModel>("Orders_FetchAll", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return order.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public OrderAdminViewModel GetOrderByOrderId(int OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", OrderId);
                connection();
                con.Open();
                OrderAdminViewModel model = con.Query<OrderAdminViewModel>("Orders_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool OrderPrinting_Admin(int OrderId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", OrderId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Printing", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OrderShippingCompanyViewModel> GetAllOrderShppingCompany_Admin(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<OrderShippingCompanyViewModel> order = con.Query<OrderShippingCompanyViewModel>("Order_Fetch_ShippingCompany", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return order.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }


        public bool SaveOrUpdateOrderShipping_Admin(OrderShippingModel model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("ShippingCompanyid", model.ShippingCompanyid);
                param.Add("Trackingnumber", model.Trackingnumber);
                param.Add("Shippingdate", model.Shippingdate);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Shipping", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OrderShippingModel GetIsShippedById_Admin(int OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", OrderId);
                connection();
                con.Open();
                var order = con.Query<OrderShippingModel>("Order_Fetch_IsShipped_By_OrderId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                con.Close();

                return order;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #endregion

        public bool SaveOrUpdateUserByAdmin_Admin(UserByAdminModel model, out int outFlag, out string outMessage, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("FirstName", model.FirstName);
                param.Add("LastName", model.LastName);
                param.Add("NfcCardColorId", model.NfcCardColorId);
                param.Add("NfcCardFinishId", model.NfcCardFinishId);
                param.Add("NfcCardLine1", model.NfcCardLine1);
                param.Add("NfcCardLine2", model.NfcCardLine2);
                param.Add("NfcCardLine3", model.NfcCardLine3);
                param.Add("EmailID", model.EmailID);
                param.Add("PaymentMethodId", model.paymentmethodid);
                param.Add("DisplayName", model.DisplayName);
                param.Add("PackageCost", model.PackageCost);
                param.Add("ReferralDiscount", model.ReferralDiscount);
                param.Add("DiscountCouponId", model.DiscountCouponIdText.ToIntOrZero());
                param.Add("CouponDiscount", model.CouponDiscount);
                param.Add("Tax", model.Tax);
                param.Add("TotalAmount", model.TotalAmount);
                param.Add("IsSelfPick", model.IsSelfPick);
                param.Add("CreatedBy", model.CreatedBy);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutPutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                param.Add("ActionName", actionName);

                connection();
                con.Open();
                con.Execute("User_Upsert_ByAdmin", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutPutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<NfcCardColorViewModel> GetAllNfcCardColor_Admin(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<NfcCardColorViewModel> color = con.Query<NfcCardColorViewModel>("NfcCardColor_FetchAll", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return color.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }


        public List<NfcCardColorViewModel> GetAllNfcCardFinishColor_Admin(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<NfcCardColorViewModel> color = con.Query<NfcCardColorViewModel>("NfcCardFinish_FetchAll", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return color.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #region BulkOrder

        public List<BulkOrderViewModel> GetAllBulkOrder_Admin(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<BulkOrderViewModel> order = con.Query<BulkOrderViewModel>("BulkOrder_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return order.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #endregion

        #region Company

        public List<CompanyViewModel> GetAllCompany_Admin(out int totalRows)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();

                IList<CompanyViewModel> data = con.Query<CompanyViewModel>("Company_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();

                return data.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public CompanyViewModel GetCompanyById_Admin(int companyId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", companyId);
                connection();
                con.Open();
                var result = con.Query<CompanyViewModel>("Company_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CompanyDelete_Admin(int companyId, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CompanyId", companyId);
                param.Add("outFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("outMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Company_Delete", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("outFlag");
                outMessage = param.Get<string>("outMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveOrUpdateCompany_Admin(CompanyModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("CompanyId", model.CompanyId);
                param.Add("CompanyName", model.CompanyName);
                param.Add("CompanyDisplayName", model.CompanyDisplayName);
                param.Add("AdminFirstname", model.AdminFirstname);
                param.Add("AdminLastname", model.AdminLastname);
                param.Add("AdminEmail", model.AdminEmail);
                param.Add("AdminDisplayName", model.AdminDisplayName);
                param.Add("Address1", model.Address1);
                param.Add("Address2", model.Address2);
                param.Add("City", model.City);
                param.Add("State", model.State);
                param.Add("Zip", model.Zip);
                param.Add("Country", model.Country);
                param.Add("Phone", model.Phone);
                param.Add("Email", model.Email);
                param.Add("Facebook", model.Facebook);
                param.Add("LinkedIn", model.LinkedIn);
                param.Add("Twitter", model.Twitter);
                param.Add("Instagram", model.Instagram);
                param.Add("Youtube", model.Youtube);
                param.Add("Website", model.Website);
                param.Add("Tagline", model.Tagline);
                param.Add("HasBackgroundImage", model.HasBackgroundImage);
                param.Add("HasCompanyVideos", model.HasCompanyVideos);
                param.Add("HasCompanyImages", model.HasCompanyImages);
                param.Add("AboutDescription", model.AboutDescription);
                param.Add("SkillName1", model.SkillName1);
                param.Add("SkillName2", model.SkillName2);
                param.Add("SkillName3", model.SkillName3);
                param.Add("SkillName4", model.SkillName4);
                param.Add("SkillName5", model.SkillName5);
                param.Add("SkillName6", model.SkillName6);
                param.Add("IsActive", model.IsActive);
                param.Add("CreatedBy", model.CreatedBy);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Company_Upsert", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Mobile app login

        public AdminModel AuthenticateAdminMobile(AuthenticateUserDto model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("EmailId", model.EmailId);
                param.Add("Password", model.Password);
                param.Add("DeviceId", model.DeviceId);
                param.Add("FirebaseToken", model.FirebaseToken);
                param.Add("DeviceType", model.DeviceType);
                param.Add("PhoneMake", model.PhoneMake);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                AdminModel userList = con.Query<AdminModel>("User_Authenticate_Admin_Mobile", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public AuthenticateAppUserViewModel AuthenticateAppUser(AuthenticateAppUserDto model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("EmailId", model.EmailId);
                param.Add("Password", model.Password);
                param.Add("DeviceId", model.DeviceId);
                param.Add("FirebaseToken", model.FirebaseToken);
                param.Add("DeviceType", model.DeviceType);
                param.Add("PhoneMake", model.PhoneMake);
                //param.Add("ActionName", actionName);
                connection();
                con.Open();
                AuthenticateAppUserViewModel userList = con.Query<AuthenticateAppUserViewModel>("User_Authenticate_App", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return userList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public List<DeviceTokenViewModel> GetAllDeviceToken()
        {
            try
            {
                connection();
                con.Open();

                IList<DeviceTokenViewModel> tokenList = con.Query<DeviceTokenViewModel>("UserDeviceToken_GetAll", commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return tokenList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #endregion

        #region Blog

        public List<BT_BlogViewModel> GetAllBT_Blogs_Admin()
        {
            try
            {
                connection();
                con.Open();

                IList<BT_BlogViewModel> data = con.Query<BT_BlogViewModel>("BT_Blogs_GetAll", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return data.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public BT_BlogViewModel GetBT_BlogsById_Admin(int blogId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("BlogId", blogId);
                connection();
                con.Open();

                var data = con.Query<BT_BlogViewModel>("BT_Blogs_GetById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                con.Close();

                return data;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public BT_BlogViewModel GetBT_BlogsBySlug(string slug)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Slug", slug);
                connection();
                con.Open();

                var data = con.Query<BT_BlogViewModel>("BT_Blog_GetBlogBySlug", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                con.Close();

                return data;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool BlogDelete_Admin(int blogId, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("BlogId", blogId);
                param.Add("outFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("outMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("BT_Blog_Delete", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("outFlag");
                outMessage = param.Get<string>("outMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveOrUpdateBT_Blog_Admin(BT_BlogModel model, out int outFlag, out string outMessage, out int outBlogId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("BlogId", model.BlogId);
                param.Add("BlogTitle", model.BlogTitle);
                param.Add("BlogDescription", model.BlogDescription);
                param.Add("BlogKeywords", model.BlogKeywords);
                param.Add("BlogTagLine", model.BlogTagLine);
                param.Add("IsActive", model.IsActive);
                param.Add("CreatedBy", model.CreatedBy);

                param.Add("outFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("outMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);
                param.Add("outBlogId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("BT_Blog_Upsert", param, commandType: CommandType.StoredProcedure);
                outBlogId = param.Get<int>("outBlogId");
                outFlag = param.Get<int>("outFlag");
                outMessage = param.Get<string>("outMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Lead

        public bool Lead_ConvertFromOrder(int orderId, int createdBy, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("OrderId", orderId);
                param.Add("CreatedBy", createdBy);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Lead_ConvertFromOrder", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool Lead_ConvertFromBulkOrder(int orderRequestId, int createdBy, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("OrderRequestId", orderRequestId);
                param.Add("CreatedBy", createdBy);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Lead_ConvertFromBulkOrder", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LeadsAdminViewModel> GetAllLeads_Admin(string actionName)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<LeadsAdminViewModel> dt = con.Query<LeadsAdminViewModel>("Lead_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return dt.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteLeads_Admin(int LeadId, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("LeadId", LeadId);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Lead_Delete", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LeadsAdminViewModel GetLeadById_Admin(int LeadId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("LeadId", LeadId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                var data = con.Query<LeadsAdminViewModel>("Lead_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                con.Close();

                return data;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<LeadStatusViewModel> GetAllLeadsStatus_Admin()
        {
            try
            {
                connection();
                con.Open();
                IList<LeadStatusViewModel> dt = con.Query<LeadStatusViewModel>("Lead_FetchStatus", commandType: CommandType.StoredProcedure).ToList();

                con.Close();

                return dt.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool Lead_CreateManual(LeadAdminModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("LeadId", model.LeadId);
                param.Add("Firstname", model.Firstname);
                param.Add("Lastname", model.Lastname);
                param.Add("Email", model.Email);
                param.Add("Phone", model.Phone);
                param.Add("CountryId", model.CountryId);
                param.Add("CurrentStatusId", model.CurrentStatusId);
                param.Add("Source", model.Source);
                param.Add("AssignedTo", model.AssignedTo);
                param.Add("CreatedBy", model.CreatedBy);
                param.Add("Company", model.Company);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Lead_CreateManual", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LeadCommunicationViewModel> GetLeadCommunicationByLeadId_Admin(int LeadId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("LeadId", LeadId);
                connection();
                con.Open();

                IList<LeadCommunicationViewModel> dt = con.Query<LeadCommunicationViewModel>("LeadCommunication_FetchLeadById", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return dt.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<LeadCommunicationTypeViewModel> GetLeadCommunicationType_Admin()
        {
            try
            {
                connection();
                con.Open();

                IList<LeadCommunicationTypeViewModel> dt = con.Query<LeadCommunicationTypeViewModel>("LeadCommunicationType_Fetch",  commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return dt.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool LeadCommunication_Upsert(LeadCommunicationModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("Id", model.Id);
                param.Add("LeadId", model.LeadId);
                param.Add("CommunicationTypeId", model.CommunicationTypeId);
                param.Add("Subject", model.Subject);
                param.Add("Message", model.Message);
                param.Add("CommunicatedBy", model.CommunicatedBy);
                param.Add("CommunicatedOn", model.CommunicatedOn);
                param.Add("NextFollowUpDate", model.NextFollowUpDate);
                param.Add("FollowUpCommunicationTypeId", model.FollowUpCommunicationTypeId);
                param.Add("CreatedBy", model.CreatedBy);

                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("LeadCommunication_Upsert", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteLeadCommunication(int id, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Id", id);
                param.Add("outFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("outMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("DeleteLeadCommunication", param, commandType: CommandType.StoredProcedure);
                outFlag = param.Get<int>("outFlag");
                outMessage = param.Get<string>("outMessage");
                con.Close();
                return outFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
