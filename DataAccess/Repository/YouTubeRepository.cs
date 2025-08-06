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
    public class YouTubeRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertYouTube(YouTubeModel YouTube, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@YouTubeId", YouTube.YouTubeId);
                _params.Add("@UserId", YouTube.UserId);
                _params.Add("@YouTubeTitle", YouTube.YouTubeTitle);                
                _params.Add("@YouTubeUrl", YouTube.YouTubeUrl);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("YouTube_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                result = -1;
                //throw ex;
            }
            return result;

        }

        public List<YouTubeModel> GetAllYouTubes(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);

                connection();
                con.Open();
                IList<YouTubeModel> YouTubeList = con.Query<YouTubeModel>("YouTube_FetchUserID", param, commandType: CommandType.StoredProcedure).ToList();                
                con.Close();

                return YouTubeList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteYouTube(int Id, string actionName = "")
        {
            bool result = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@YouTubeId", Id);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("YouTube_Delete", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public YouTubeModel GetYouTubeById(int YouTubeId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("YouTubeId", YouTubeId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                YouTubeModel YouTubeList = con.Query<YouTubeModel>("YouTube_Fetch", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return YouTubeList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }
      
    }
}
