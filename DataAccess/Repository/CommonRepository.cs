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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace DataAccess.Repository
{
    public class CommonRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public List<BusinessTypeModel> GetBusinessTypeList(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<BusinessTypeModel> list = con.Query<BusinessTypeModel>("BusinessType_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return list;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<BusinessTypeModel> GetBusinessTypeParentList(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<BusinessTypeModel> list = con.Query<BusinessTypeModel>("BusinessType_FetchParents", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return list;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<BusinessTypeModel> GetBusinessTypeListBtParentId(int parentId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ParentId", parentId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                List<BusinessTypeModel> list = con.Query<BusinessTypeModel>("BusinessType_Fetch_ByParents", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return list;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool SoftDeleteBusinessTypeById(int businessTypeId, int deletedBy, out int OutFlag, out string OutMessage)
        {
            try
            {

                DynamicParameters param = new DynamicParameters();
                param.Add("BusinessTypeId", businessTypeId);
                param.Add("DeletedBy", deletedBy);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("BusinessType_SoftDelete", param, commandType: CommandType.StoredProcedure);

                OutFlag = param.Get<int>("OutFlag");
                OutMessage = param.Get<string>("OutMessage");

                con.Close();
                return OutFlag == 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while performing soft delete on the business type.", ex);
            }
        }

        public BusinessTypeViewModel GetBusinessTypeById(int businessTypeId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("BusinessTypeId", businessTypeId);
                connection();
                con.Open();
                BusinessTypeViewModel list = con.Query<BusinessTypeViewModel>("BusinessType_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return list;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool UpsertBusinessType(BusinessTypeModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("BusinessTypeId", model.BusinessTypeId == 0 ? (int?)null : model.BusinessTypeId);
                param.Add("BusinessType", model.BusinessType);
                param.Add("ParentId", model.ParentId == 0 ? (int?)null : model.ParentId);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("BusinessType_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();

                return outFlag == 0;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                outMessage = $"An error occurred: {ex.Message}";
                return false;
            }
        }

    }
}
