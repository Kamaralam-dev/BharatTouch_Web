using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Models;

namespace DataAccess.Repository
{
    public class OAuthRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public bool UpsertOAuthTokens(OAuthTokenModel model, string key, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@UserId", model.UserId);
                _params.Add("@Key", key);
                _params.Add("@AccessToken", key == "google" ? model.GoogleAccessToken : model.MicrosoftAccessToken);
                _params.Add("@RefreshToken", key == "google" ? model.GoogleRefreshToken : model.MicrosoftRefreshToken);
                _params.Add("@ActionName", actionName);
                _params.Add("@OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Oauth_Token_Upsert", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("OutFlag") != 9;

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public OAuthTokenModel GetOAuthTokenByUserId(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", UserId);
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                OAuthTokenModel model = con.Query<OAuthTokenModel>("Oauth_Token_FetchByUserID", _params, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
