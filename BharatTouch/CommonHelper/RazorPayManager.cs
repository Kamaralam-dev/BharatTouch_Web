using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Razorpay.Api;
using BharatTouch.CommonHelper;

namespace BharatTouch.CommonHelper
{
    public class RazorPayManager
    {
        private readonly RazorpayClient _razorpayClient = new RazorpayClient(ConfigValues.RazorPayApiKey, ConfigValues.RazorPayApiSecret);

        public ActionState FetchPaymentDetailsById(string paymentId)
        {
            try
            {
                Payment payment = _razorpayClient.Payment.Fetch(paymentId);
                //string method = payment["method"].ToString();
                //string email = payment["email"].ToString();
                //string contact = payment["contact"].ToString();
                //string amount = payment["amount"].ToString();
                //string status = payment["status"].ToString();
                return new ActionState { Success = true, Message = "Payment Details", Data = payment.Attributes, Type = ActionState.SuccessType };
            }
            catch (Razorpay.Api.Errors.BadRequestError ex)
            {
                Console.WriteLine(ex);
                return new ActionState
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ex,
                    Type = ActionState.ErrorType
                };
            }
            catch (Razorpay.Api.Errors.ServerError ex)
            {
                Console.WriteLine(ex);  
                return new ActionState
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ex,
                    Type = ActionState.ErrorType
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ActionState { Success = false, Message = ex.Message, Data = ex, Type = ActionState.ErrorType };
            }
        }
    }
}