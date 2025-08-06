using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BharatTouch.CommonHelper;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using BharatTouch.CommonHelper;
using BharatTouch.Controllers;

namespace BharatTouch.Controllers
{
    public class CardThemeController : Controller
    {
        #region Declaration
        //string defaultCardTemplate = "_defaultCardPreview";
        string defaultCardTemplate = "_profileDefaultCard";
        CardThemeRepository _cardthemeRepo = new CardThemeRepository();
        #endregion

        // GET: CardTheme

        [AuthenticateAdmin]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CardPreview(int UserId)
        {

            var model = _cardthemeRepo.FetchCardThemeSettingByUserId(UserId, "BharatTouch/EditProfile/CardPreview");
            if (model == null)
            {
                UserCardThemeSettingViewModel obj = new UserCardThemeSettingViewModel();
                obj.UserId = UserId;
                return PartialView(defaultCardTemplate, obj);
            }

            //return PartialView(model.TemplateView, model);
            return PartialView(defaultCardTemplate, model);

        }

        public ActionResult BindCardTemplates()
        {
            int totalRows = 0;
            var result = _cardthemeRepo.FetchAllCardTemplates(Utility.StartIndex(), Utility.PageSize(), Utility.SortBy(), Utility.SortDesc(), Utility.FilterText(), out totalRows, "BharatTouch/ThemeCardIndex/BindCardTemplates");
            return Json(new { recordsFiltered = totalRows, recordsTotal = totalRows, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpsertCardTemplate(CardTemplateModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (string.IsNullOrWhiteSpace(fileExtension))
                        {
                            char[] splitImg = { '/' };
                            string[] getExtention = file.ContentType.Split(splitImg);
                            fileExtension = "." + getExtention[1];
                        }
                        string fileName = Guid.NewGuid().ToString("N") + Path.GetFileNameWithoutExtension(file.FileName) + fileExtension; ;
                        string dbPath = "/Uploads/CardThemeTemplates";
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                        {
                            var folderPath = Server.MapPath("~" + dbPath);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            var serverPath = Path.Combine(folderPath, fileName);
                            file.SaveAs(serverPath);
                            model.ImageUrl = dbPath + "/" + fileName;
                        }
                        else
                        {
                            return new ActionState { Message = "Failed!", Data = "Only jpg, jpeg, png files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        }
                    }
                }

                var result = _cardthemeRepo.UpsertCardTemplate(model, "BharatTouch/ThemeCardIndex/UpsertCardTemplate");

                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var isInserted = model.TemplateId == 0 ? "inserted" : "updated";

                return new ActionState { 
                    Message = "Done!", 
                    Data = $"Card template {isInserted} successfully.", 
                    Success = true, 
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult DeleteCardTemplateById(int TemplateId)
        {
            try
            {
                bool result = _cardthemeRepo.DeleteCardTemplateById(TemplateId, "BharatTouch/ThemeCardIndex/DeleteCardTemplateById");
                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Message = "Done!", Data = "Card template deleted successfully.", Success = true, Type = ActionState.SuccessType}.ToActionResult(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenCardTemplatesModel(int TemplateId)
        {
            CardTemplateModel model = new CardTemplateModel();
            if(TemplateId.ToIntOrZero() > 0)
            {
                model = _cardthemeRepo.FetchCardTemplateById(TemplateId.ToIntOrZero(), "BharatTouch/ThemeCardIndex/OpenCardTemplatesModel");
            }

            return PartialView("_CardTemplateFormModel", model);
        }

        [HttpPost]
        public ActionResult UpsertUserCardThemeSetting(CardThemeSettingModel model, string actionName)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (string.IsNullOrWhiteSpace(fileExtension))
                        {
                            char[] splitImg = { '/' };
                            string[] getExtention = file.ContentType.Split(splitImg);
                            fileExtension = "." + getExtention[1];
                        }
                        string fileName = Guid.NewGuid().ToString("N") + Path.GetFileNameWithoutExtension(file.FileName) + fileExtension; ;
                        string dbPath = "/Uploads/CardThemeBgImage";
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                        {
                            var folderPath = Server.MapPath("~" + dbPath);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            var serverPath = Path.Combine(folderPath, fileName);
                            file.SaveAs(serverPath);
                            model.BackgroundImg = dbPath + "/" + fileName;
                        }
                        else
                        {
                            return new ActionState { Message = "Failed!", Data = "Only jpg, jpeg, png files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        }
                    }
                }

                if(model.TemplateId.ToIntOrZero() == 0)
                {
                    model.TemplateId = 1;
                }

                var result = _cardthemeRepo.UpsertCardThemeSetting(model, actionName);

                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }


                return new ActionState
                {
                    Message = "Done!",
                    Data = "Theme setting updated successfully.",
                    Success = true,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult RemoveCardThemeBgImg(CardThemeSettingModel model, string actionName)
        {
            try
            {
                string bgPath = model.BackgroundImg;
                model.BackgroundImg = "";
                var result = _cardthemeRepo.UpsertCardThemeSetting(model, actionName);

                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                string filepath = Server.MapPath(bgPath);
                FileInfo file = new FileInfo(filepath);
                if (file.Exists) 
                {
                    file.Delete();
                }


                return new ActionState
                {
                    Message = "Done!",
                    Data = "Card background image removed successfully.",
                    Success = true,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenSelectCardThemeModel(int UserId, string searchText)
        {
            var model = _cardthemeRepo.FetchCardThemeSettingByUserId(UserId, "BharatTouch/EditProfile/OpenSelectCardThemeModel");
            if(model == null)
            {
                var obj = new UserCardThemeSettingViewModel();
                obj.UserId = UserId;
                obj.TemplateId = 1;
                var modelObj = Tuple.Create(searchText, obj);
                return PartialView("_SelectCardThemeFormModel", modelObj);
            }
            return PartialView("_SelectCardThemeFormModel", Tuple.Create(searchText, model));
        }

        [HttpPost]
        public ActionResult FetchMoreThemeCards(PaginationModel model, string actionName)
        {
            try
            {
                int totalRows = 0;
                var data = _cardthemeRepo.FetchAllCardTemplates(model.Page.ToIntOrZero(), model.Size.ToIntOrZero(), model.SortBy.NullToString(), model.SortOrder.NullToString(), model.SearchText.NullToString(), out totalRows, actionName);
                return new ActionState { Success = true, Message = "Done!", Data = data, OptionalValue = totalRows.NullToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
    }
}