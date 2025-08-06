using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.ViewModels;

namespace DataAccess.Repository
{
    public class SocialRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertUserSocial(int userId,string socialMedia,string url, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();                
                _params.Add("@UserId", userId);
                _params.Add("@SocialMedia", socialMedia);
                _params.Add("@Url", url);
                _params.Add("ActionName", actionName);
                con.Execute("User_SocialInfo_Upsert", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;

        }

        public List<SocialMediaModel> GetSocialInfo(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<SocialMediaModel> UserSocialList = con.Query<SocialMediaModel>("User_SocialInfo_Fetch", param, commandType: CommandType.StoredProcedure).ToList();                
                con.Close();

                return UserSocialList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteSocialMedia(int userId, string actionName = "")
        {
            bool result = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("User_SocialInfo_Delete", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}
