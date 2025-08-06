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
    public class NfcCardRepository
    {

        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }
        public List<NfcCardColorModel> GetNfcCardColorList(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<NfcCardColorModel> list = con.Query<NfcCardColorModel>("NfcCardColor_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return list;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public NfcCardColorViewModel GetNfcCardColorByUserId(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                NfcCardColorViewModel model = con.Query<NfcCardColorViewModel>("NfcCardColor_FetchByUserId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public NfcCardColorViewModel GetNfcCardPreviewDetailsByUserId(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                NfcCardColorViewModel model = con.Query<NfcCardColorViewModel>("NfcCard_Preview_Details_ByUserId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
        public NfcCardColorViewModel GetNfcCardPreviewDetailsByOrderId(int OrderId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderId", OrderId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                NfcCardColorViewModel model = con.Query<NfcCardColorViewModel>("NfcCard_Preview_Details_ByOrderId", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool UpdateSelectedNfcCardColor(NfcCardColorViewModel model, out int outFlag, string actionName)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("CardColorId", model.CardColorId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_Update_Selected_NfcCardColor", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");

                con.Close();

                return outFlag == 1;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                return false;
            }
        }

        public bool UpdateSelectedNfcCardColor_V3(NfcCardColorViewModel model, out int outFlag, string actionName)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("OrderId", model.OrderId);
                param.Add("NFCCardColorId", model.CardColorId);
                param.Add("NFCCardFinishId", model.CardFinishId);
                param.Add("IncludeMetalCard", model.IncludeMetalCard);
                param.Add("PackageCost", model.PackageCost);
                param.Add("CardType", model.CardType);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_Update_Selected_NfcCard_V3", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");

                con.Close();

                return outFlag == 0;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                return false;
            }
        }
        
        public bool UpdateSelectedNfcCardColor_V2(NfcCardColorViewModel model, out int outFlag, string actionName)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("OrderId", model.OrderId);
                param.Add("NFCCardColorId", model.CardColorId);
                param.Add("NFCCardFinishId", model.CardFinishId);
                param.Add("IncludeMetalCard", model.IncludeMetalCard);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_Update_Selected_NfcCard_V2", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");

                con.Close();

                return outFlag == 0;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                return false;
            }
        }

        public bool UpdateUsersNfcCardDetails(NfcCardColorViewModel model, out int outFlag, string actionName)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("CardColorId", model.CardColorId);
                param.Add("NfcCardLine1", model.NfcCardLine1);
                param.Add("NfcCardLine2", model.NfcCardLine2);
                param.Add("NfcCardLine3", model.NfcCardLine3);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_Update_NfcCardDetails", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");

                con.Close();

                return outFlag == 1;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                return false;
            }
        }

        public bool UpdateUsersNfcCardDetails_v2(NfcCardColorViewModel model, out int outFlag, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("OrderId", model.OrderId);
                param.Add("NfcCardLine1", model.NfcCardLine1);
                param.Add("NfcCardLine2", model.NfcCardLine2);
                param.Add("NfcCardLine3", model.NfcCardLine3);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_Update_NfcCardDetails_V2", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");

                con.Close();

                return outFlag == 0;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                return false;
            }
        }
    }
}
