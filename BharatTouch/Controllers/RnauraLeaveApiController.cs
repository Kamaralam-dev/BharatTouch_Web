using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.ModelBinding;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RnauraLeaveApiController : ApiController
    {
        RnauraLeaveRepository _leaveRepo = new RnauraLeaveRepository();

        #region Leaves
        [HttpGet]
        [Route("api/v1/leaves/getAllLeaveTypes")]
        public ResponseModel GetAllLeaveTypes()
        {
            try
            {
                int totalRow;
                var model = _leaveRepo.GetAllLeaveTypes(out totalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model, outParam = totalRow };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/leaves/getLeaveById/{id}")]
        public ResponseModel GetLeaveById(int id)
        {
            try
            {
                var model = _leaveRepo.GetLeaveById(id);
                return new ResponseModel()
                {
                    IsSuccess = model != null,
                    Message = model != null ? "Operation successful." : "Leave not found.",
                    Data = model
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/leaves/getAllLeaves")]
        public ResponseModel GetAllLeaves()
        {
            try
            {
                int totalRow;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                var model = _leaveRepo.GetAllLeaves(loggerEmail, out totalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model, outParam = totalRow };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        
        [HttpPost]
        [Route("api/v1/leaves/upsertLeave")]
        public ResponseModel UpsertLeave()
        {
            try
            {
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                int OutputFlag;
                string OutputMessage;
                var req = HttpContext.Current.Request;

                RnauraLeaveModel model = new RnauraLeaveModel();
                model.LeaveId = Convert.ToInt32(req.Form["LeaveId"].ToIntOrNull());
                model.UserId = Convert.ToInt32(req.Form["UserId"].ToIntOrNull());
                model.LeaveTypeId = Convert.ToInt32(req.Form["LeaveTypeId"].ToIntOrNull());
                var startDate = req.Form["StartDate"];
                if (!string.IsNullOrEmpty(startDate))
                {
                    model.StartDate = DateTime.Parse(startDate);
                }

                var endDate = req.Form["EndDate"];
                if (!string.IsNullOrEmpty(endDate))
                {
                    model.EndDate = DateTime.Parse(endDate);
                }
                model.Reason = req.Form["Reason"].NullToString();
                if (req.Files.Count > 0)
                    model.Document = Utility.SaveFile("/uploads/rnaura/documents/leave/");
                else
                    model.Document = req.Form["Document"].NullToString();
                model.ApprovalStatus = req.Form["ApprovalStatus"].NullToString();
                model.CommentByManager = req.Form["CommentByManager"].NullToString();

                _leaveRepo.UpsertLeave(loggerEmail, model, out OutputFlag, out OutputMessage);
                return new ResponseModel()
                {
                    IsSuccess = OutputFlag == 1,
                    Message = OutputMessage.NullToString() != "" ? OutputMessage : "Something went wrong.",
                    Data = null
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/leaves/deleteLeave/{id}")]
        public ResponseModel DeleteLeave(int id)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _leaveRepo.DeleteLeave(loggerEmail, id, out OutputFlag);
                return new ResponseModel()
                {
                    IsSuccess = OutputFlag == 1,
                    Message = OutputFlag == 1 ? "Operation successful." : "User not authorized",
                    Data = null
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Holidays
        [HttpGet]
        [Route("api/v1/holidays/getHolidayById/{holidayId}")]
        public ResponseModel GetHolidayById(int holidayId)
        {
            try
            {
                var model = _leaveRepo.GetHolidayById(holidayId);
                return new ResponseModel()
                {
                    IsSuccess = model != null,
                    Message = model != null ? "Operation successful." : "Holiday not found.",
                    Data = model
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/holidays/getAllHolidays")]
        public ResponseModel GetAllHolidays(int page = 0, int size = 10, string sortby = "Date", string sortOrder = "desc", string searchText = "")
        {
            try
            {
                int totalRows = 0;
                var model = _leaveRepo.GetAllHolidays(page, size, sortby, sortOrder, searchText, out totalRows);
                return new ResponseModel() { IsSuccess = true, Message = "Holidays", Data = model, outParam = totalRows };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/holidays/getHolidaysByYear/{Year}")]
        public ResponseModel GetHolidaysByYear(int Year)
        {
            try
            {
                int totalRows = 0;
                var model = _leaveRepo.GetHolidaysByYear(Year, out totalRows);
                return new ResponseModel() { IsSuccess = true, Message = "Holidays", Data = model, outParam = totalRows };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/holidays/upsertHoliday")]
        public ResponseModel UpsertHoliday([FromBody] RnauraHolidayModel model)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _leaveRepo.UpsertHoliday(loggerEmail, model, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "holiday created successfully!", Data = null };
                else if (OutputFlag == 2)
                    return new ResponseModel() { IsSuccess = true, Message = "holiday updated successfully!", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "User not authorized!", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/holidays/deleteHoliday/{holidayId}")]
        public ResponseModel DeleteHoliday(int holidayId)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _leaveRepo.DeleteHoliday(loggerEmail, holidayId, out OutputFlag);

                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "Holiday deleted successfully!", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "User not authorized!", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion
    }
}