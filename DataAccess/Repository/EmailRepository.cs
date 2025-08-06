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
using DataAccess.ViewModels;

namespace DataAccess.Repository
{
    public class EmailRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public bool EmailUpsert(EmailModel model, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Id", model.Id);
                param.Add("Subject", model.Subject);
                param.Add("ToEmail", model.ToEmail);
                param.Add("Body", model.Body);
                param.Add("CC", model.CC);
                param.Add("BCC", model.BCC);
                param.Add("AttachmentPaths", model.AttachmentPaths);
                param.Add("IsSuccess", model.IsSuccess);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Email_Upsert", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<EmailModel> GetAllFailedEmails()
        {
            try
            {
                connection();
                con.Open();
                List<EmailModel> emails = con.Query<EmailModel>("Emails_Failed_FetchAll", commandType: CommandType.StoredProcedure).ToList();
                con.Close();
                return emails;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
    }
}
