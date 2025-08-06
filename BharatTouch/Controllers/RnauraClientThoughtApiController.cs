using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BharatTouch.CommonHelper;
using System.Web.Http.Cors;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RnauraClientThoughtApiController : ApiController
    {
        RnauraClientThoughtRepository _clientThoughtRepo = new RnauraClientThoughtRepository();

        [HttpPost]
        [Route("api/v1/clientThoughts/upsertClientThought")]
        public ResponseModel UpsertClientThought([FromBody] RnauraClientThoughtModel model)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                model.Image = Utility.SaveFileFromBase64(model.Image, "/uploads/rnaura/team/");
                _clientThoughtRepo.UpsertClientThought(loggerEmail, model, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "Client Thought created successfully", Data = null };
                else if (OutputFlag == 2)
                    return new ResponseModel() { IsSuccess = true, Message = "Client Thought updated successfully", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "You don't have permission for this action", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/clientThoughts/updateClientThoughtPosition")]
        public ResponseModel UpdateClientThoughtPosition([FromBody] RnauraClientThoughtModel model)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _clientThoughtRepo.UpdateClientThoughtPosition(loggerEmail, model, out OutputFlag);
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
        [Route("api/v1/clientThoughts/getClientThoughtById/{ClientId}")]
        public ResponseModel GetClientThoughtsById(int ClientId)
        {
            try
            {
                RnauraClientThoughtModel model = _clientThoughtRepo.FetchClientThoughtById(ClientId);
                return new ResponseModel()
                {
                    IsSuccess = model != null,
                    Message = model != null ? "Fetched client thought" : "Client thought Not Found",
                    Data = model
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/clientThoughts/fetchClientThoughtsAll")]
        public ResponseModel FetchClientThoughtALl()
        {
            try
            {
                int totalRow;
                var model = _clientThoughtRepo.FetchClientThoughtAll(out totalRow);
                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Client thoughts list",
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
        [Route("api/v1/clientThoughts/deleteClientThought/{ClientId}")]
        public ResponseModel DeleteClientThought(int ClientId)
        {
            try
            {
                int OutputFlag;

                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _clientThoughtRepo.DeleteClientThoughtById(loggerEmail, ClientId, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "Client thought deleted successfully.", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "You don't have permission for this action", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }
    }
}
