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
using DocumentFormat.OpenXml.Spreadsheet;

namespace DataAccess.Repository
{
    public class PaymentRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertOrderPayment(PaymentModel model, Boolean WantMetalCard, out int NewInvoiceId, string actionName = "")
        {
            int result = 0;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("PaymentMethodId", model.PaymentMethodId);
                param.Add("PaymentAmount", model.PaymentAmount);
                param.Add("TransactionId", model.TransactionId);
                param.Add("PaymentStatus", model.PaymentStatus);
                param.Add("WantMetalCard", WantMetalCard);
                param.Add("RazorPayPaymentMethod", model.RazorPayPaymentMethod);
                param.Add("DiscountCoupon", model.DiscountCoupon);
                param.Add("ShippingAmount", model.ShippingAmount);
                param.Add("IsByHandPickup", model.IsByHandPickup);
                param.Add("Tax", model.Tax);
                param.Add("ActionName", actionName);
                param.Add("NewPaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("NewInvoiceId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Payment_v5", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("NewPaymentId");
                NewInvoiceId = param.Get<int>("NewInvoiceId");
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public int UpsertOrderPayment_v6(PaymentModel model, out int NewInvoiceId, string actionName = "")
        {
            int result = 0;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("PaymentMethodId", model.PaymentMethodId);
                param.Add("PaymentAmount", model.PaymentAmount);
                param.Add("RazorPayTransactionId", model.RazorPayTransactionId);
                param.Add("RazorPayPaymentStatus", model.RazorPayPaymentStatus);
                param.Add("RazorPayJson", model.RazorPayJson);
                param.Add("RazorPayPaymentMethod", model.RazorPayPaymentMethod);
                param.Add("DiscountCoupon", model.DiscountCoupon);
                param.Add("CouponDiscount", model.CouponDiscount);
                param.Add("ActionName", actionName);
                param.Add("NewPaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("NewInvoiceId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Payment_v6", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("NewPaymentId");
                NewInvoiceId = param.Get<int>("NewInvoiceId");
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public int UpsertOrderPayment_v7(PaymentModel model, out int NewInvoiceId, string actionName = "")
        {
            int result = 0;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("PaymentMethodId", model.PaymentMethodId);
                param.Add("PaymentAmount", model.PaymentAmount);
                param.Add("RazorPayTransactionId", model.RazorPayTransactionId);
                param.Add("RazorPayPaymentStatus", model.RazorPayPaymentStatus);
                param.Add("RazorPayJson", model.RazorPayJson);
                param.Add("RazorPayPaymentMethod", model.RazorPayPaymentMethod);
                param.Add("DiscountCoupon", model.DiscountCoupon);
                param.Add("CouponDiscount", model.CouponDiscount);
                param.Add("ShippingAmount", model.ShippingAmount);
                param.Add("Tax", model.Tax);
                param.Add("GSTno", model.GstNo);
                param.Add("ActionName", actionName);
                param.Add("NewPaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("NewInvoiceId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Payment_v7", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("NewPaymentId");
                NewInvoiceId = param.Get<int>("NewInvoiceId");
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public PaymentModel GetPaymentByUserId(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                PaymentModel model = con.Query<PaymentModel>("Payment_Fetch_By_UserId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
        
        public dynamic GetPaymentById(int PaymentId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("PaymentId", PaymentId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                dynamic model = con.Query<dynamic>("Payment_Fetch_By_Id", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<dynamic> GetUsersPaymentsList(PaginationModel pagination, int userId, string actionName = "")
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
                List<dynamic> model = con.Query<dynamic>("Paginated_User_Payments", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #region Invoice

        public InvoiceViewModel FetchInvoiceById(int InvoiceId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("InvoiceId", InvoiceId);
                connection();
                con.Open();
                InvoiceViewModel model = con.Query<InvoiceViewModel>("Invoice_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        #endregion


        #region DiscountCoupons
        public DiscountCouponModel CheckValidDiscountCoupon(string Code, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Code", Code);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                DiscountCouponModel model = con.Query<DiscountCouponModel>("Check_Discount_Coupon_IsValid ", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }


        public int UpsertDiscountCoupon(DiscountCouponModel model, string actionName = "")
        {
            int result = 0;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("DiscountCouponId", model.DiscountCouponId);
                param.Add("CouponName", model.CouponName);
                param.Add("Code", model.Code);
                param.Add("StartDate", model.StartDate);
                param.Add("EndDate", model.EndDate);
                param.Add("PercentageOff", model.PercentageOff);
                param.Add("AmountOff", model.AmountOff);
                param.Add("IsActive", model.IsActive);
                param.Add("CreatedBy", model.CreatedBy);
                param.Add("ActionName", actionName);
                param.Add("NewDiscountCouponId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Discount_Coupon_Upsert", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("NewDiscountCouponId");
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public DiscountCouponModel GetDiscountCouponById(int DiscountCouponId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("DiscountCouponId", DiscountCouponId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                DiscountCouponModel model = con.Query<DiscountCouponModel>("Discount_Coupon_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<DiscountCouponModel> DiscountCouponFetchAll(PaginationModel pagination, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", pagination.Page);
                param.Add("size", pagination.Size);
                param.Add("sortby", pagination.SortBy);
                param.Add("sortOrder", pagination.SortOrder);
                param.Add("searchText", pagination.SearchText);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<DiscountCouponModel> userList = con.Query<DiscountCouponModel>("Discount_Coupon_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return userList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public int DeleteDiscountCoupon(int DiscountCouponId, string actionName = "")
        {
            int result = 0;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("DiscountCouponId", DiscountCouponId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Discount_Coupon_DeleteById", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("OutFlag");
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion
    }
}
