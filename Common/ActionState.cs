using Newtonsoft.Json;
using System;
using System.Net;
using System.Web.Mvc;

#if CLIENT_WEB
namespace guestACCESS.ClientWeb
#elif NGX_WEB
namespace NGX.Web
#else
namespace guestACCESS.Web
#endif
{
    [Serializable]
    public class ActionState
    {
        public const string SuccessType = "success";
        public const string ErrorType = "error";
        public const string InfoType = "info";

        public ActionState()
        {
            Success = true;
        }

        string _msg;
        public string Message
        {
            get { return _msg; }
            set { _msg = value; }
        }

        bool _success;
        public bool Success
        {
            get { return _success; }
            set
            {
                _success = value;
                _type = value ? SuccessType : ErrorType;
            }
        }

        string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        object _data;
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public ActionResult ToActionResult(HttpStatusCode statusCode)
        {
            return new ActionStateResult(statusCode, this);
        }
    }

    public class ActionStateResult : ActionResult
    {
        public HttpStatusCode StatusCode
        {
            get;
            private set;
        }

        public ActionState State
        {
            get;
            private set;
        }

        public ActionStateResult(HttpStatusCode statusCode, ActionState state)
        {
            StatusCode = statusCode;
            State = state;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = "application/json";
            response.StatusCode = (int)StatusCode;
            response.Write(JsonConvert.SerializeObject(State));
        }
    }
}