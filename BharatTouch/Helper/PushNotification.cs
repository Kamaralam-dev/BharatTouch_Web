using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Google.Apis.Auth.OAuth2;
using DataAccess.ApiHelper;

namespace GroopGo.Api.WebHelper
{
    public class PushNotification
    {
        /*carrentals-f82e5 -- this is project id from google service account*/
        private static string scopes = "https://www.googleapis.com/auth/firebase.messaging";
        private static string baseAddress = "https://fcm.googleapis.com/v1/projects/bharattouch-62578/messages:send";

        public static ResponseModel Send(List<string> fcmTokens, string notifyTitle, string notifyMessage)
        {
            //----------Generating Bearer token for FCM---------------
            //Download from Firebase Console ServiceAccount
            string fileName = System.Web.Hosting.HostingEnvironment.MapPath("~/FirebaseTokenHelper/bharattouch-62578-firebase-adminsdk-fbsvc-596a07cc26.json"); // 08 aug 2025 added
           // string fileName = System.Web.Hosting.HostingEnvironment.MapPath("~/FirebaseTokenHelper/bharattouch-62578-firebase-adminsdk-fbsvc-3c19f91889.json");
            // string fileName = System.Web.Hosting.HostingEnvironment.MapPath("~/FirebaseTokenHelper/eagle-eye-car-rentals-3987c-firebase-adminsdk-kbob7-c3c938d21e.json");


            var bearertoken = ""; // Bearer Token in this variable
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {

                bearertoken = GoogleCredential
                  .FromStream(stream) // Loads key file
                  .CreateScoped(scopes) // Gathers scopes requested
                  .UnderlyingCredential // Gets the credentials
                  .GetAccessTokenForRequestAsync().Result; // Gets the Access Token

            }

            ///--------Calling FCM-----------------------------
            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearertoken);


            //---------------Assigning Of data To Model --------------
            ResponseModel apiResponse = new ResponseModel();

            foreach (string fcmToken in fcmTokens)
            {
                Root rootObj = new Root();
                rootObj.message = new Message();

                rootObj.message.token = fcmToken;
                /*beliw is the dinesh device's token for test*/
                // "e2mWSmkmT8eToJ41LTVMHQ:APA91bFTi2vrCYFcKj1aDGfw2gZ9jtkyfupn-pGLA15-AWtCF_sjU3NAgw9YFFrHeTE2HDdYB7SfoVWJVphx9w-UFzEhNX80I8lL-Kfu15PxmvNC3QlJx-cchzmiB2eF4jN4wj9m63nB"; //FCM Token id

                rootObj.message.data = new Data();
                //rootObj.message.data.title = "Data Title";
                //rootObj.message.data.body = "Data Body";
                //rootObj.message.data.key_1 = "Sample Key";
                //rootObj.message.data.key_2 = "Sample Key2";
                rootObj.message.notification = new Notification();
                rootObj.message.notification.title = notifyTitle;
                rootObj.message.notification.body = notifyMessage;

                //Set notification sound for Android
                rootObj.message.android = new Android();
                rootObj.message.android.notification = new AndroidNotification();
                rootObj.message.android.notification.sound = "default"; // You can specify a custom sound file here

                // Set notification sound for iOS
                rootObj.message.apns = new Apns();
                rootObj.message.apns.headers = new ApnsHeaders();
                rootObj.message.apns.payload = new payload();
                rootObj.message.apns.payload.aps = new aps();
                rootObj.message.apns.payload.aps.sound = "default";
                rootObj.message.apns.payload.aps.badge = 1;


                /*-------------Convert Model To JSON ----------------------*/
                var jsonObj = new JavaScriptSerializer().Serialize(rootObj);
                var data = new StringContent(jsonObj, Encoding.UTF8, "application/json");
                data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                /*----------------- Calling The FCM httpv1 API------------------*/
                var response = client.PostAsync(baseAddress, data).Result;

                /*---------- Deserialize Json Response from API ----------------------------------*/
                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                var responseObj = new JavaScriptSerializer().DeserializeObject(jsonResponse);
                apiResponse.IsSuccess = response.IsSuccessStatusCode;
                apiResponse.Message = response.IsSuccessStatusCode ? "Notification sent" : "Error on sending notification";
            }

            return apiResponse;
        }

    }

    public class Data
    {

        public string body
        {
            get;
            set;
        }

        public string title
        {
            get;
            set;
        }

        public string key_1
        {
            get;
            set;
        }

        public string key_2
        {
            get;
            set;
        }

    }

    public class Message
    {

        public string token
        {
            get;
            set;
        }

        public Data data
        {
            get;
            set;
        }

        public Notification notification
        {
            get;
            set;
        }
        public Android android { get; set; } // Added Android property
        public Apns apns { get; set; } // Added Apns property for iOS
    }

    public class Notification
    {

        public string title
        {
            get;
            set;
        }

        public string body
        {
            get;
            set;
        }

    }

    public class Root
    {

        public Message message
        {
            get;
            set;
        }

    }

    public class Android
    {
        public AndroidNotification notification { get; set; }
    }

    public class AndroidNotification
    {
        public string sound { get; set; }
    }

    public class Apns
    {
        public ApnsHeaders headers { get; set; }
        public payload payload { get; set; }
    }

    public class ApnsHeaders
    {
        // public string sound { get; set; }
    }

    public class payload
    {
        public aps aps { get; set; }
    }

    public class aps
    {
        public string sound { get; set; }
        public int badge { get; set; }

    }
}