using Dapper;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RnauraProjectRepository
    {
        SqlConnection con;

        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        public RnauraProjectModel GetProjectById(int id)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Id", id);
                connection();
                con.Open();
                RnauraProjectModel model = con.Query<RnauraProjectModel>("Rnaura_Project_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraProjectModel> GetProjectsByParent(int ParentId, out int TotalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("ParentId", ParentId);
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraProjectModel> models = con.Query<RnauraProjectModel>("Rnaura_Project_FetchByParent", _params, commandType: CommandType.StoredProcedure).ToList();
                TotalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraProjectModel> GetAllParentProjects(out int totalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraProjectModel> models = con.Query<RnauraProjectModel>("Rnaura_Project_Parent_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpsertProject(RnauraProjectModel project, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Id", project.Id);
                _params.Add("ProjectTitle", project.ProjectTitle);
                _params.Add("Description", project.Description);
                _params.Add("Image", project.Image);
                _params.Add("IsActive", project.IsActive);
                _params.Add("IsParent", project.IsParent);
                _params.Add("CategoryId", project.CategoryId);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Project_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteProjectById(int ProjectId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("Id", ProjectId);
                connection();
                con.Open();
                con.Execute("Rnaura_Project_DeleteById", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteProjectByParentId(int ParentId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("ParentId", ParentId);
                connection();
                con.Open();
                con.Execute("Rnaura_Project_DeleteByParent", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RnauraProjectCategoryModel GetProjectCategoryById(int CategoryId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("CategoryId", CategoryId);
                connection();
                con.Open();
                RnauraProjectCategoryModel model = con.Query<RnauraProjectCategoryModel>("Rnaura_Project_Category_FetchById", _params, commandType: CommandType.StoredProcedure).FirstOrDefault();
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RnauraProjectCategoryModel> GetAllProjectCategories(out int totalRow)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                List<RnauraProjectCategoryModel> models = con.Query<RnauraProjectCategoryModel>("Rnaura_Project_Category_FetchAll", _params, commandType: CommandType.StoredProcedure).ToList();
                totalRow = _params.Get<int>("totalRow");
                con.Close();
                return models;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpsertProjectCategory(RnauraProjectCategoryModel category, out int OutputFlag)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("CategoryId", category.CategoryId);
                _params.Add("CategoryName", category.CategoryName);
                _params.Add("IsActive", category.IsActive);
                _params.Add("OutputFlag", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection();
                con.Open();
                con.Execute("Rnaura_Project_Category_Upsert", _params, commandType: CommandType.StoredProcedure);
                OutputFlag = _params.Get<int>("OutputFlag");
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteProjectCategoryById(int CategoryId)
        {
            try
            {
                DynamicParameters _params = new DynamicParameters();
                _params.Add("CategoryId", CategoryId);
                connection();
                con.Open();
                con.Execute("Rnaura_Project_Category_DeleteById", _params, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
