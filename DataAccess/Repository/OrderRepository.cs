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
using DocumentFormat.OpenXml.Drawing.Charts;

namespace DataAccess.Repository
{
    public class OrderRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public void UpsertOrder(OrderModel model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("ShippingAddress1", model.ShippingAddress1);
                param.Add("ShippingAddress2", model.ShippingAddress2);
                param.Add("ShippingCity", model.ShippingCity);
                param.Add("ShippingState", model.ShippingState);
                param.Add("ShippingCountry", model.ShippingCountry);
                param.Add("ShippingZip", model.ShippingZip);
                param.Add("BillingAddress1", model.BillingAddress1);
                param.Add("BillingAddress2", model.BillingAddress2);
                param.Add("BillingCity", model.BillingCity);
                param.Add("BillingState", model.BillingState);
                param.Add("BillingCountry", model.BillingCountry);
                param.Add("BillingZip", model.BillingZip);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Address", param, commandType: CommandType.StoredProcedure);
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpsertOrder_v2(OrderModel model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("ShippingAddress1", model.ShippingAddress1);
                param.Add("ShippingAddress2", model.ShippingAddress2);
                param.Add("ShippingCity", model.ShippingCity);
                param.Add("ShippingState", model.ShippingState);
                param.Add("ShippingCountry", model.ShippingCountry);
                param.Add("ShippingZip", model.ShippingZip);
                param.Add("BillingAddress1", model.BillingAddress1);
                param.Add("BillingAddress2", model.BillingAddress2);
                param.Add("BillingCity", model.BillingCity);
                param.Add("BillingState", model.BillingState);
                param.Add("BillingCountry", model.BillingCountry);
                param.Add("BillingZip", model.BillingZip);
                param.Add("IsSelfPick", model.IsSelfPick);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Order_Upsert_Address_V2", param, commandType: CommandType.StoredProcedure);
                var result = param.Get<int>("OutFlag");
                con.Close();
                return result == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public int CreateNewCardOrder(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                param.Add("NewOrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Order_NewCard_Create", param, commandType: CommandType.StoredProcedure);
                var newOrderId = param.Get<int>("NewOrderId");
                con.Close();
                return newOrderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public int UpdateOrderWithRazorPayDetails(OrderModel model, string actionName = "")
        {
            int result = 1;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", model.OrderId);
                param.Add("UserId", model.UserId);
                param.Add("RazorpayOrderId", model.RazorpayOrderId);
                param.Add("RazorpayOrderDate", model.RazorpayOrderDate);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Order_Update_Razorpay_OrderDetails", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("OutFlag");
                con.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public OrderViewModel GetOrderByUserId(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                OrderViewModel model = con.Query<OrderViewModel>("Order_Fetch_By_UserId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
        public OrderViewModel GetOrderByOrderId(int OrderId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", OrderId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                OrderViewModel model = con.Query<OrderViewModel>("Order_Fetch_By_OrderId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public OrderViewModel GetOrderAmountDetialsForFinalPayment(int OrderId, int UserId, out int OutFlag, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", OrderId);
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                OrderViewModel model = con.Query<OrderViewModel>("Order_Fetch_DetailsForPayment", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                OutFlag = param.Get<int>("OutFlag");
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
