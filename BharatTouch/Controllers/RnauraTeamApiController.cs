using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RnauraTeamApiController : ApiController
    {
        RnauraTeamRepository _teamRepo = new RnauraTeamRepository();

        [HttpPost]
        [Route("api/v1/team/upsertTeam")]
        public ResponseModel UpsertTeam([FromBody] RnauraTeamModel model)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                model.Image = Utility.SaveFileFromBase64(model.Image, "/uploads/rnaura/team/");
                _teamRepo.UpsertTeam(loggerEmail, model, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "Team member created successfully", Data = null };
                else if (OutputFlag == 2)
                    return new ResponseModel() { IsSuccess = true, Message = "Team member updated successfully", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "You don't have permission for this action", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/team/updateTeamPosition")]
        public ResponseModel UpdateTeamPosition([FromBody] RnauraTeamModel model)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _teamRepo.UpdateTeamPosition(loggerEmail, model, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "Position updated successfully", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "You don't have permission for this action", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/team/getTeamMemberById/{TeamId}")]
        public ResponseModel GetTeamMemberById(int TeamId)
        {
            try
            {
                RnauraTeamModel model = _teamRepo.FetchTeamMemberById(TeamId);
                return new ResponseModel() 
                {
                    IsSuccess = model != null,
                    Message = model != null ? "Fetched Team Member" : "Team member Not Found",
                    Data = model
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/team/fetchTeamAll")]
        public ResponseModel FetchTeamALl()
        {
            try
            {
                int totalRow;
                var model = _teamRepo.FetchTeamAll(out totalRow);
                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Team list",
                    Data = model,
                    outParam = totalRow
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/team/deleteTeamMember/{TeamId}")]
        public ResponseModel DeleteTeamMember(int TeamId)
        {
            try
            {
                int OutputFlag;

                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _teamRepo.DeleteTeamMemberById(loggerEmail, TeamId, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "Team member deleted successfully.", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "You don't have permission for this action", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }
    }
}
