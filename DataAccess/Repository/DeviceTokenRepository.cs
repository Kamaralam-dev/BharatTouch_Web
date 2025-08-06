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

namespace DataAccess.Repository
{
    public class DeviceTokenRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public bool UpsertDeviceToken(DeviceTokenModel model, out int OutFlag, string actionName = "")
        {
            var result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", model.UserId);
                _params.Add("Device_ID", model.Device_Id);
                _params.Add("DeviceToken", model.Device_Token);
                _params.Add("DeviceDescription", model.DeviceDescription);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Upsert_DeviceToken", _params, commandType: CommandType.StoredProcedure);

                OutFlag = _params.Get<int>("OutFlag");
                result = OutFlag != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public bool RemoveDeviceToken(DeviceTokenModel model, out int OutFlag, string actionName = "")
        {
            var result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", model.UserId);
                _params.Add("Device_ID", model.Device_Id);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Remove_DeviceToken", _params, commandType: CommandType.StoredProcedure);

                OutFlag = _params.Get<int>("OutFlag");
                result = OutFlag == 1;
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
