using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using SCTimeSheet_UTIL.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static SCTimeSheet.Controllers.BaseController;

namespace SCTimeSheet.Controllers
{
    public class SettingsController : BaseController
    {

        [NoCache]
        public ActionResult Index()
        {
            //var model3 = DB.Settings;
            //SettingsModel model = new SettingsModel();
            //model.SetValue = model3.Where(x => x.SetCode == "D01").FirstOrDefault().SetValue;
            //return View(model);
            return View();
        }

        [HttpPost]
        [SubmitButton(Name = "action", Argument = "Set")]
        public ActionResult Save(SettingsModel model)
        {
            try
            {

                ModelState.Remove("SetID");
                // model.ValidateModel(ModelState);
                if (ModelState.IsValid)
                {
                    if (model.SetID.Equals(0))
                    {
                        model.SetValue = model.SetValue;
                        model.SetCode = model.SetCode;
                        DB.Settings.Add(model);
                        TempData["Success"] = ResourceMessage.Settings;
                    }
                    else
                    {
                        var existing = DB.Settings.Find(model.SetID);
                        existing.SetValue = model.SetValue;
                        existing.SetCode = model.SetCode;
                        DB.Settings.Attach(existing);
                        DB.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                        TempData["Success"] = ResourceMessage.Settings;
                    }

                    DB.SaveChanges();
                    return RedirectToAction("Index", "Settings");

                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return View("Index", model);
        }


        //[HttpPost]
        //public JsonResult Read()
        //{

        //    try
        //    {
        //        var GetDetails = DB.Database.SqlQuery<SettingsList>(
        //                   @"exec " + Constants.P_GetEmpTimesheet_SettingsAdmin,
        //                   new object[] {
        //                  }).ToList();
        //        //var NewEnryModel = DB.NewEntry.Join(DB.ProjectMaster,x=>x.ProjectID,y=>y.ProjectID,(x,y)=>new { x.InvolveMonth,x.InvolvePercent}) .OrderBy(x => x.ProjectID).ToList();

        //        return Json(new { data = GetDetails });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.ErrorLog(ex);
        //        throw ex;
        //    }
        //}


        //[HttpGet]
        //public ActionResult Edit(int? id)
        //{
        //    try
        //    {

        //        var settinglist = DB.Settings.Where(x => x.SetID == id).FirstOrDefault();
        //        return View("Index", settinglist);

        //    }
        //    catch (Exception ex)
        //    {

        //        LogHelper.ErrorLog(ex);
        //        throw ex;
        //    }
        //}


        //[HttpPost]
        //public ActionResult Delete(int? id)
        //{
        //    try
        //    {
        //        if (id == null)
        //            return Json(new { success = false });

        //        var existingModel = DB.Settings.Where(x => x.SetID == id).FirstOrDefault();
        //        DB.Settings.Attach(existingModel);
        //        DB.Entry(existingModel).State = System.Data.Entity.EntityState.Modified;
        //        DB.SaveChanges();
        //        return Json(new { success = true });

        //    }
        //    catch (Exception ex)
        //    {

        //        LogHelper.ErrorLog(ex);
        //        throw ex;
        //    }
        //}






    }
}