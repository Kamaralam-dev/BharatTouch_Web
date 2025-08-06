using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using SautinSoft.Document.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.ModelBinding;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RnauraProjectApiController : ApiController
    {
        RnauraProjectRepository _projectRepo = new RnauraProjectRepository();

        [HttpGet]
        [Route("api/v1/Projects/GetProjectById/{id}")]
        public ResponseModel GetProjectById(int id)
        {
            try
            {
                var model = _projectRepo.GetProjectById(id);
                return new ResponseModel() { IsSuccess = model != null, Message = model != null ? "Operation successful." : "Project not found.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Projects/getProjectByParent/{parentId}")]
        public ResponseModel GetProjectByParent(int parentId)
        {
            try
            {
                int TotalRow;
                var model = _projectRepo.GetProjectsByParent(parentId, out TotalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model, outParam = TotalRow };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Projects/getAllParentProject")]
        public ResponseModel GetAllParentProject()
        {
            try
            {
                int TotalRow;
                var model = _projectRepo.GetAllParentProjects(out TotalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model, outParam = TotalRow };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/Projects/upsertProject")]
        public ResponseModel UpsertProject(RnauraProjectModel model)
        {
            try
            {
                int OutputFlag;
                model.Image = Utility.SaveFileFromBase64(model.Image, "/uploads/rnaura/images/projects/");
                _projectRepo.UpsertProject(model, out OutputFlag);
                return new ResponseModel() { 
                    IsSuccess = true, 
                    Message = OutputFlag == 1 ? "Project updated successfully." : "Project created successfully.", 
                    Data = null 
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/Projects/deleteProjectById/{id}")]
        public ResponseModel DeleteProjectById(int id)
        {
            try
            {
                _projectRepo.DeleteProjectById(id);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }
        
        [HttpPost]
        [Route("api/v1/Projects/deleteProjectByParent/{parentId}")]
        public ResponseModel DeleteProjectByParent(int parentId)
        {
            try
            {
                _projectRepo.DeleteProjectByParentId(parentId); 
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/ProjectsCategory/GetProjectById/{id}")]
        public ResponseModel GetProjectCategoryById(int id)
        {
            try
            {
                var model = _projectRepo.GetProjectCategoryById(id);
                return new ResponseModel() { IsSuccess = model != null, Message = model != null ? "Operation successful." : "Category not found.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/ProjectsCategory/getAllProjectCategories")]
        public ResponseModel GetAllProjectCategories()
        {
            try
            {
                int TotalRow;
                var model = _projectRepo.GetAllProjectCategories(out TotalRow);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model, outParam = TotalRow };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/ProjectsCategory/upsertProjectCategory")]
        public ResponseModel UpsertProjectCategory(RnauraProjectCategoryModel model)
        {
            try
            {
                int OutputFlag;
                _projectRepo.UpsertProjectCategory(model, out OutputFlag);
                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = OutputFlag == 1 ? "Category updated successfully." : "Category created successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/ProjectsCategory/deleteProjectCategoryById/{id}")]
        public ResponseModel DeleteProjectCategoryById(int id)
        {
            try
            {
                _projectRepo.DeleteProjectCategoryById(id);
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }
    }
}