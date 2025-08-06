using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Models;

namespace DataAccess.Repository
{
    public class CertificationRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public int UpsertUserCertification(UserCertificationModel model, string actionName = "")
        {
            var result = 0;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@CertificationId", model.CertificationId);
                _params.Add("@UserId", model.UserId);
                _params.Add("@CertificationName", model.CertificationName);
                _params.Add("@IssuingOrganization", model.IssuingOrganization);
                _params.Add("@OrganizationURL", model.OrganizationURL);
                _params.Add("@IssueDate", model.IssueDate);
                _params.Add("@ExpirationDate", model.ExpirationDate);
                _params.Add("@CertifcateNumber", model.CertifcateNumber);
                _params.Add("@CertificateURL", model.CertificateURL);
                _params.Add("@Description", model.Description);
                _params.Add("@CertificateFile", model.CertificateFile);
                _params.Add("@ActionName", actionName);
                _params.Add("@NewCertificationId", DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Certification_Upsert", _params, commandType: CommandType.StoredProcedure);

                result = _params.Get<int>("NewCertificationId");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public List<UserCertificationModel> GetUserCertificationBy_UserId(int userId, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("UserId", userId);
                _params.Add("ActionName", actionName);
                _params.Add("totalRow", DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<UserCertificationModel> list = con.Query<UserCertificationModel>("Certification_FetchByUserId", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRows = _params.Get<int>("totalRow");
                con.Close();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserCertificationModel GetUserCertificationBy_Id(int certificationId, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("CertificationId", certificationId);
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                UserCertificationModel model = con.Query<UserCertificationModel>("Certification_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteUserCertificationBy_Id(int certificationId, string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("CertificationId", certificationId);
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                con.Execute("Certification_DeleteById", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
