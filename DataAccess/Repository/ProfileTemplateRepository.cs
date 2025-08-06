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
    public class ProfileTemplateRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public List<ProfileTemplateModel> GetAllTemplates(string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<ProfileTemplateModel> list = con.Query<ProfileTemplateModel>("ProfileTemplate_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                con.Close();

                return list.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public ProfileTemplateModel GetTemplateById(int ProfileTemplateId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("ProfileTemplateId", ProfileTemplateId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                ProfileTemplateModel model = con.Query<ProfileTemplateModel>("Profile_Template_FetchByID", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
    }
}
