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
    public class BlogRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertBlog(BlogModel blog, string actionName = "")
        {
            int result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@BlogId", blog.BlogId);
                _params.Add("@UserId", blog.UserId);
                _params.Add("@BlogTitle", blog.BlogTitle);
                _params.Add("@BlogCategory", blog.BlogCategory);
                _params.Add("@BlogUrl", blog.BlogUrl);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Blog_Upsert", _params, commandType: CommandType.StoredProcedure);
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

        public List<BlogModel> GetAllBlogs(int userId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", userId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<BlogModel> BlogList = con.Query<BlogModel>("Blog_FetchUserID", param, commandType: CommandType.StoredProcedure).ToList();                
                con.Close();

                return BlogList.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteBlog(int Id, string actionName = "")
        {
            bool isSuccess = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@BlogId", Id);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Blog_Delete", param, commandType: CommandType.StoredProcedure);
                isSuccess = param.Get<int>("OutFlag") != 9;
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BlogModel GetBlogById(int BlogId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("BlogId", BlogId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                BlogModel BlogList = con.Query<BlogModel>("Blog_Fetch", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return BlogList;
            }
            catch (Exception exe)
            {
                throw exe;
            }

        }
      
    }
}
