using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using Google.Apis.Logging;
using Newtonsoft.Json;
using Razorpay.Api;
using BharatTouch.CommonHelper;
//using WebSocketSharp;
//using WebSocketSharp.Server;

namespace BharatTouch.Controllers
{
    public class PaymentOrderResponse
    {
        public Decimal Amount { get; set; }
        public string OrderId { get; set; }
        public string Error { get; set; }
        public bool success { get; set; }
    }
    public class TestController : Controller
    {

        private readonly RazorpayClient _razorpayClient;

        public TestController()
        {
            //_razorpayClient = new RazorpayClient("rzp_test_SZItrD86m81mu1", "77awe3d4vCqfpbrs9yPGycbl"); // Replace with your actual keys
            _razorpayClient = new RazorpayClient(ConfigValues.RazorPayApiKey, ConfigValues.RazorPayApiSecret); // Replace with your actual keys
        }
        //private static ConcurrentDictionary<string, WebSocket> _connectedClients = new ConcurrentDictionary<string, WebSocket>();
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();

        UserRepository _userRepo = new UserRepository();
        public ActionResult Index()
        {
            //int visitorCount = GetVisitorCount();
            //ViewBag.VisitorCount = visitorCount;
            return View();
        }

        public ActionResult TestPage()
        {
            decimal amount = 1000; // Example amount

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount); // Amount in paise
            options.Add("currency", "INR");
            options.Add("receipt", $"order_{Guid.NewGuid().ToString("N").Substring(0, 30)}"); //generate unique receipt id

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Order order = _razorpayClient.Order.Create(options);
                return View(new PaymentOrderResponse { Amount = amount, OrderId = order["id"] });

            }
            catch (Exception ex)
            {
                //handle exception
                return View(new PaymentOrderResponse { Error = ex.Message});


            }
        }

        [HttpPost]
        public ActionResult VerifyPayment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>
        {
           { "razorpay_order_id", razorpay_order_id },
           { "razorpay_payment_id", razorpay_payment_id },
            { "razorpay_signature", razorpay_signature }
        };


            try
            {
                string expectedSignature = GenerateSHA256Hash(razorpay_order_id + "|" + razorpay_payment_id, "77awe3d4vCqfpbrs9yPGycbl");

                if (expectedSignature != razorpay_signature)
                {
                    //Signature Verified
                    return Json(new { success = false, error = "Invalid Signature" });

                }

                return Json(new { success = true });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });

            }
        }

        private string GenerateSHA256Hash(string input, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

                return BitConverter.ToString(hash).Replace("-", "").ToLower();

            }
        }



        //private int GetVisitorCount()
        //{
        //    int count = 0;

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();
        //        var query = "SELECT Count FROM VisitorCount WHERE Id = 1";
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            count = (int)cmd.ExecuteScalar();
        //        }
        //    }

        //    return count;
        //}
    }

    //    public class VisitorCounter : WebSocketBehavior
    //    {
    //        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
    //        private static ConcurrentDictionary<string, int> _userSessions = new ConcurrentDictionary<string, int>();
    //        private static int totalConnections = 0;

    //        protected override void OnOpen()
    //        {
    //            string page = this.Context.QueryString["page"];
    //            //string identity = this.Context.QueryString["identity"];


    //            var identity = Context.QueryString["identity"] ?? Guid.NewGuid().ToString();
    //            totalConnections++;

    //            // If user doesn't have active session, increment and add
    //            if (!_userSessions.ContainsKey(identity))
    //            {
    //                _userSessions.TryAdd(identity, 1);
    //                IncrementVisitorCount();
    //            }
    //            else
    //            {
    //                _userSessions[identity]++;
    //            }
    //;
    //            // Broadcast updated visitor count to all connected clients
    //            BroadcastVisitorCount();
    //        }

    //        protected override void OnClose(CloseEventArgs e)
    //        {
    //            var identity = Context.QueryString["identity"];

    //            totalConnections--;

    //            //Remove user if its the last connection
    //            if (identity != null)
    //            {
    //                if (_userSessions.ContainsKey(identity))
    //                {
    //                    _userSessions[identity]--;
    //                    if (_userSessions[identity] == 0)
    //                    {
    //                        int value;
    //                        _userSessions.TryRemove(identity, out value);
    //                        DecrementVisitorCount();
    //                    }
    //                }
    //            }

    //            BroadcastVisitorCount();
    //        }


    //        private void BroadcastVisitorCount()
    //        {
    //            int visitorCount = GetVisitorCount();
    //            //Send(visitorCount.ToString());
    //            var model = new UserModel();
    //            model.FirstName = "Firoz";
    //            //model.UserId = Sessions.Count;        // visitor count from session
    //            model.UserId = GetVisitorCount();       // visitor count from database query
    //            model.CountryId = totalConnections;
    //            Sessions.Broadcast(JsonConvert.SerializeObject(model));
    //        }

    //        private int GetVisitorCount()
    //        {
    //            int count = 0;

    //            using (SqlConnection conn = new SqlConnection(connectionString))
    //            {
    //                conn.Open();
    //                var query = "SELECT Count FROM VisitorCount WHERE Id = 1";
    //                using (SqlCommand cmd = new SqlCommand(query, conn))
    //                {
    //                    count = (int)cmd.ExecuteScalar();
    //                }
    //            }

    //            return count;
    //        }
    //        private void IncrementVisitorCount()
    //        {
    //            using (SqlConnection conn = new SqlConnection(connectionString))
    //            {
    //                conn.Open();
    //                var query = "UPDATE VisitorCount SET Count = Count + 1 WHERE Id = 1";
    //                using (SqlCommand cmd = new SqlCommand(query, conn))
    //                {
    //                    cmd.ExecuteNonQuery();
    //                }
    //            }
    //        }
    //        private void DecrementVisitorCount()
    //        {
    //            using (SqlConnection conn = new SqlConnection(connectionString))
    //            {
    //                conn.Open();
    //                var query = "UPDATE VisitorCount SET Count = Count - 1 WHERE Id = 1";
    //                using (SqlCommand cmd = new SqlCommand(query, conn))
    //                {
    //                    cmd.ExecuteNonQuery();
    //                }
    //            }
    //        }
    //    }

    //    public class WebSocketServerManager
    //    {
    //        private WebSocketServer _server;
    //        private string _url = ConfigValues.WebSocketUrl;

    //        public void StartServer()
    //        {
    //            _server = new WebSocketServer(_url); // in development we can also pass only port in this then 
    //            _server.AddWebSocketService<VisitorCounter>("/visitor");
    //            _server.Start();
    //        }

    //        public void StopServer()
    //        {
    //            if (_server != null)
    //            {
    //                _server.Stop();
    //                _server.RemoveWebSocketService("/visitor"); // Clean up the service
    //                _server = null;
    //            }
    //        }
    //    }
}