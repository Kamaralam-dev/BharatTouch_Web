using Dapper;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RnauraLeaveRepository
    {
        SqlConnection con;

        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        #region Leaves
        public RnauraLeaveModel GetLeaveById(int id)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LeaveId", id);
                connection();
                con.Open();
                RnauraLeaveModel model = con.Query<RnauraLeaveModel>("Rnaura_Leave_Fetch", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraLeaveModel> GetAllLeaves(string loggerEmail, out int TotalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("loggerEmail", loggerEmail);
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraLeaveModel> models = con.Query<RnauraLeaveModel>("Rnaura_Leave_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                TotalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraLeaveTypesModel> GetAllLeaveTypes(out int totalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraLeaveTypesModel> models = con.Query<RnauraLeaveTypesModel>("Rnaura_LeaveType_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpsertLeave(string loggerEmail, RnauraLeaveModel leave, out int OutputFlag, out string OutputMessage)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LoggerEmail", loggerEmail);
                _params.Add("LeaveId", leave.LeaveId);
                _params.Add("UserId", leave.UserId);
                _params.Add("LeaveTypeId", leave.LeaveTypeId);
                _params.Add("StartDate", leave.StartDate);
                _params.Add("EndDate", leave.EndDate);
                _params.Add("Reason", leave.Reason);
                _params.Add("Document", leave.Document);
                _params.Add("ApprovalStatus", leave.ApprovalStatus);
                _params.Add("CommentByManager", leave.CommentByManager);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                _params.Add("OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);
                connection();
                con.Open();
                con.Execute("Rnaura_Leave_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                OutputMessage = _params.Get<string>("OutputMessage");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteLeave(string loggerEmail, int leaveId, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LoggerEmail", loggerEmail);
                _params.Add("LeaveId", leaveId);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Leave_Delete", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Holidays
        public RnauraHolidayModel GetHolidayById(int HolidayId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("HolidayId", HolidayId);
                connection();
                con.Open();
                RnauraHolidayModel model = con.Query<RnauraHolidayModel>("Rnaura_Holiday_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraHolidayModel> GetAllHolidays(int page, int size, string sortBy, string sortOrder, string searchText, out int totalRows)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", page);
                param.Add("size", size);
                param.Add("sortby", sortBy);
                param.Add("sortOrder", sortOrder);
                param.Add("searchText", searchText);
                param.Add("totalrow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();

                IList<RnauraHolidayModel> holidayList = con.Query<RnauraHolidayModel>("Rnaura_Holiday_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalrow");
                con.Close();

                return holidayList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public List<RnauraHolidayModel> GetHolidaysByYear(int Year, out int totalRows)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Year", Year);
                param.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                IList<RnauraHolidayModel> holidayList = con.Query<RnauraHolidayModel>("Rnaura_Holiday_fetchByYear", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalRow");
                con.Close();
                return holidayList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public void UpsertHoliday(string loggerEmail, RnauraHolidayModel model, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("loggerEmail", loggerEmail);
                _params.Add("HolidayId", model.HolidayId);
                _params.Add("Name", model.Name);
                _params.Add("Description", model.Description);
                _params.Add("Date", model.Date);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Holiday_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteHoliday(string loggerEmail, int HolidayId, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("LoggerEmail", loggerEmail);
                _params.Add("HolidayId", HolidayId);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Holiday_Delete", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
