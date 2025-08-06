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
using DocumentFormat.OpenXml.Spreadsheet;

namespace DataAccess.Repository
{
    public class ScheduleAndMeetingRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        #region Schedule Open Days
        public bool UpsertScheduleOpenDays(ScheduleOpenDayModel model, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", model.UserId);
                _params.Add("@Dates", model.multipleOpenDays);
                _params.Add("@ActionName", actionName);
                _params.Add("@OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("OpenDays_Upsert", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag") != 9;

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public List<ScheduleOpenDayModel> GetAllScheduleOpenDays(int UserId, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                param.Add("TotalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();

                IList<ScheduleOpenDayModel> meetingList = con.Query<ScheduleOpenDayModel>("OpenDays_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("TotalRow");
                con.Close();

                return meetingList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public ScheduleOpenDayModel GetScheduleOpenDaysByUserIdAndDate(ScheduleOpenDayModel model,  string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", model.UserId);
                param.Add("Date", model.Date);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                ScheduleOpenDayModel openDay = con.Query<ScheduleOpenDayModel>("OpenDays_Fetch_By_UserId_And_Date", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return openDay;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public ScheduleOpenDayModel GetScheduleOpenDayById(int dayId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("DayId", dayId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                ScheduleOpenDayModel model = con.Query<ScheduleOpenDayModel>("OpenDays_FetchById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }

        public bool DeleteScheduleOpenDayById(int dayId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("DayId", dayId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("OpenDays_DeleteById", param, commandType: CommandType.StoredProcedure);
                int outputFlag = param.Get<int>("OutFlag");
                con.Close();
                return outputFlag == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Schedule Week Days
        public bool UpsertScheduleOpenWeekDays(ScheduleOpenWeekDayModel model, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", model.UserId);
                _params.Add("Sun", model.Sun);
                _params.Add("Mon", model.Mon);
                _params.Add("Tue", model.Tue);
                _params.Add("Wed", model.Wed);
                _params.Add("Thu", model.Thu);
                _params.Add("Fri", model.Fri);
                _params.Add("Sat", model.Sat);
                _params.Add("@ActionName", actionName);
                _params.Add("@OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Schedule_Open_Week_Days_Upsert", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag") != 9;

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ScheduleOpenWeekDayModel GetScheduleOpenWeekDays(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", UserId);
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                ScheduleOpenWeekDayModel model = con.Query<ScheduleOpenWeekDayModel>("Schedule_Open_Week_Days_FetchByUserId", _params, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
