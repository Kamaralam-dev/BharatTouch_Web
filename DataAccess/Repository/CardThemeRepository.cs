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
    public class CardThemeRepository
    {
        public SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        #region Templates
        public bool UpsertCardTemplate(CardTemplateModel model,  string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("TemplateId", model.TemplateId);
                _params.Add("Name", model.Name);
                _params.Add("TemplateView", model.TemplateView?.Trim());
                _params.Add("ImageUrl", model.ImageUrl);
                _params.Add("Types", model.Types);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Card_Template_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<CardTemplateModel> FetchAllCardTemplates(int page, int size, string sortBy, string sortOrder, string searchText, out int totalRows, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("page", page);
                param.Add("size", size);
                param.Add("sortby", sortBy);
                param.Add("sortOrder", sortOrder);
                param.Add("searchText", searchText);
                param.Add("totalrow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("ActionName", actionName);
                connection();
                con.Open();

                IList<CardTemplateModel> listmodel = con.Query<CardTemplateModel>("Card_Template_FetchAll", param, commandType: CommandType.StoredProcedure).ToList();
                totalRows = param.Get<int>("totalrow");
                con.Close();

                return listmodel.ToList();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public CardTemplateModel FetchCardTemplateById(int TemplateId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("TemplateId", TemplateId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                CardTemplateModel model = con.Query<CardTemplateModel>("Card_Template_FetchById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public bool DeleteCardTemplateById(int TemplateId, string actionName = "")
        {
            bool result = false;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("TemplateId", TemplateId);
                param.Add("ActionName", actionName);
                param.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Card_Template_DeleteById", param, commandType: CommandType.StoredProcedure);
                result = param.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion

        #region Theme Settings
        public bool UpsertCardThemeSetting(CardThemeSettingModel model, string actionName = "")
        {
            bool result = false;
            try
            {
                connection();
                con.Open();
                DynamicParameters _params = new DynamicParameters();
                _params.Add("TemplateId", model.TemplateId);
                _params.Add("UserId", model.UserId);
                _params.Add("BackgroundImg", model.BackgroundImg);
                _params.Add("ActionName", actionName);
                _params.Add("OutFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("Card_Theme_Setting_Upsert", _params, commandType: CommandType.StoredProcedure);
                result = _params.Get<int>("OutFlag") != 9;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public UserCardThemeSettingViewModel FetchCardThemeSettingByUserId(int UserId, string actionName = "")
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("UserId", UserId);
                param.Add("ActionName", actionName);
                connection();
                con.Open();
                UserCardThemeSettingViewModel model = con.Query<UserCardThemeSettingViewModel>("Card_Theme_Setting_FetchByUserId", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
        #endregion
    }
}
