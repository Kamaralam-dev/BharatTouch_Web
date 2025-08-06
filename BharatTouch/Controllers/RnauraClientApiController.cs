using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RnauraClientApiController : ApiController
    {
        RnauraClientRepository _clientRepo = new RnauraClientRepository();

        [HttpGet]
        [Route("api/v1/clients/getClientById/{id}")]
        public ResponseModel GelClientById(int id)
        {
            try
            {
                var client = _clientRepo.GetClientById(id);
                return new ResponseModel()
                {
                    IsSuccess = client != null,
                    Message = client != null ? "Operation successful." : "Client not found.",
                    Data = client
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/clients/getAllClients")]
        public ResponseModel GetAllClients()
        {
            try
            {
                int TotalRow = 0;
                var clients = _clientRepo.GetAllClients(out TotalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = clients };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/clients/insertClient")]
        public ResponseModel InsertClient([FromBody] RnauraClientModel client)
        {
            try
            {
                int newClientId;
                _clientRepo.ClientInsert(client, out newClientId);
                return new ResponseModel()
                {
                    IsSuccess = newClientId.ToIntOrZero() != 0,
                    Message = newClientId.ToIntOrZero() != 0 ? "Request has sent." : "Something went wrong",
                    Data = null
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/clients/deleteClientById/{id}")]
        public ResponseModel DeleteClient(int id)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _clientRepo.DeleteClient(loggerEmail, id, out OutputFlag);
                if (OutputFlag != 0)
                    return new ResponseModel() { IsSuccess = true, Message = "Client deleted successfully.", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "User not authorized.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }
    }
}