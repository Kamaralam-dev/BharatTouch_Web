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
    public class CountryRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public List<CountryModel> GetCountries(string actionName = "")
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("ActionName", actionName);
                connection();
                con.Open();
                IList<CountryModel> countries = con.Query<CountryModel>("Country_Fetch", _params, commandType: CommandType.StoredProcedure).ToList();                
                con.Close();
                con.Dispose();

                return countries.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool SoftDeleteCountry(int countryId, int deletedBy, out int OutFlag, out string OutMessage)
        {
            try
            {

                DynamicParameters param = new DynamicParameters();
                param.Add("CountryId", countryId);
                param.Add("DeletedBy", deletedBy);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection();
                con.Open();
                con.Execute("Country_Delete", param, commandType: CommandType.StoredProcedure);

                OutFlag = param.Get<int>("OutFlag");
                OutMessage = param.Get<string>("OutMessage");

                con.Close();
                con.Dispose();
                return OutFlag == 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while performing soft delete on the country."+ex.Message, ex);
            }
        }

        public CountryModel GetCountryById(int countryId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CountryId", countryId);
                connection();
                con.Open();
                CountryModel model = con.Query<CountryModel>("Country_FetchById", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                con.Dispose();

                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool UpsertCountry(CountryModel model, out int outFlag, out string outMessage)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("CountryId", model.CountryId == 0 ? (int?)null : model.CountryId);
                param.Add("Country", model.Country);
                param.Add("Abbreviation", model.Abbreviation);
                param.Add("CountryCode", model.CountryCode);
                param.Add("NumberCode", model.NumberCode);
                param.Add("MinNumberLength", model.MinNumberLength);
                param.Add("MaxNumberLength", model.MaxNumberLength);
                param.Add("CreatedBy", model.CreatedBy);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("OutMessage", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                
                connection();
                con.Open();
                con.Execute("Country_Upsert", param, commandType: CommandType.StoredProcedure);

                outFlag = param.Get<int>("OutFlag");
                outMessage = param.Get<string>("OutMessage");

                con.Close();
                con.Dispose();

                return outFlag == 0;
            }
            catch (Exception ex)
            {
                // Handle the exception
                outFlag = 1;
                outMessage = $"An error occurred: {ex.Message}";
                return false;
            }
        }

    }
}
