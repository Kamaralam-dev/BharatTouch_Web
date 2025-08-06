using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using SautinSoft.Document.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.ModelBinding;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RnauraUserApiController : ApiController
    {
        RnauraUserRepository _userRepo = new RnauraUserRepository();

        [HttpPost]
        [Route("api/v1/Users/authenticate")]
        public ResponseModel Authenticate([FromBody] RnauraUserModel user)
        {
            try
            {
                var model = _userRepo.AuthenticateUser(user);
                return new ResponseModel()
                {
                    IsSuccess = model != null,
                    Message = model != null ? "User Authenticated." : "Email & Password is wrong.",
                    Data = model
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Users/getUserById/{id}")]
        public ResponseModel GetUserById(int id)
        {
            try
            {
                var model = _userRepo.GetUserById(id);
                return new ResponseModel() { IsSuccess = model != null, Message = model != null ? "Operation successful." : "User not found.", Data = model };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Users/getAllUsers")]
        public ResponseModel GetAllUsers()
        {
            try
            {
                int TotalRow;
                var model = _userRepo.GetAllUsers(out TotalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model, outParam = TotalRow };
            }   
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Users/getLoggerDetails")]
        public ResponseModel GetLoggerDetails()
        {
            try
            {
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if(loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                var model = _userRepo.loggerDetails(loggerEmail);
                if (model != null)
                    return new ResponseModel() { IsSuccess = true, Message = "Operation successful", Data = model };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "Something went wrong.", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/Users/upsertUser")]
        public ResponseModel UpsertUser([FromBody] RnauraUserModel user)
        {
            try
            {
                int OutputFlag;
                user.ProfilePicture = Utility.SaveFileFromBase64(user.ProfilePicture, "/uploads/rnaura/images/users/");

                _userRepo.UpserUser(user, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "User created successfully.", Data = null };
                else if (OutputFlag == 2)
                    return new ResponseModel() { IsSuccess = true, Message = "User updated successfully.", Data = null };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "Email already exist.", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/Users/deleteUserById/{id}")]
        public ResponseModel DeleteUser(int id)
        {
            try
            {
                int OutputFlag;
                string loggerEmail = Request.Headers.GetValues("rnaura-loggerEmail").First().NullToString();
                if (loggerEmail == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Logger email is missing", Data = null };

                _userRepo.DeleteUser(loggerEmail, id, out OutputFlag);
                if (OutputFlag == 1)
                    return new ResponseModel() { IsSuccess = true, Message = "User deleted successfully.", Data = null };
                else 
                    return new ResponseModel() { IsSuccess = false, Message = "User not authorized", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }
    }
}