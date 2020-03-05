using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class ContactUsController : BaseController
    {
        // GET: ContactUs
        public ActionResult Index()
        {
            return View();
        }

        // GET: ContactUs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ContactUs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactUs/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContactUsModel contactUsModel = new ContactUsModel();
                    contactUsModel.Contact = collection["models[0].Contact"];
                    contactUsModel.Title = collection["models[0].Title"];
                    contactUsModel.Name = collection["models[0].Name"];
                    contactUsModel.Email = collection["models[0].Email"];
                   
                    DB.ContactUs.Add(contactUsModel);
                    DB.SaveChanges();
                    return RedirectToAction("Index");
                }

                //The model is invalid - render the current view to show any validation errors
                return View("Index");
                
            }
            catch
            {
                return View();
            }
        }

        // GET: ContactUs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ContactUs/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, FormCollection collection)
        {
            ContactUsModel contactUsModel = new ContactUsModel();
            try
            {


                contactUsModel.Id = Convert.ToInt32(collection["models[0].Id"]);
                contactUsModel.Contact = collection["models[0].Contact"];
                contactUsModel.Title = collection["models[0].Title"];
                contactUsModel.Name = collection["models[0].Name"];
                contactUsModel.Email = collection["models[0].Email"];
                // TODO: Add update logic here
                if (collection != null && ModelState.IsValid)
                {

                    var item = DB.ContactUs.FirstOrDefault(x => x.Id == contactUsModel.Id);
                    if (item != null)
                    {
                        DB.Entry(item).CurrentValues.SetValues(contactUsModel);
                        DB.SaveChanges();
                        return RedirectToAction("Index");
                    }
                   
                }


                return Json(new[] { contactUsModel }.ToDataSourceResult(request, ModelState));

            }
            catch
            {
                return Json(new[] { contactUsModel }.ToDataSourceResult(request, ModelState));
            }
        }

        // GET: ContactUs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ContactUs/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                var id = Convert.ToInt32(collection["models[0].Id"]);

                var item = DB.ContactUs.FirstOrDefault(x => x.Id == id);
                DB.ContactUs.Remove(item);
                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Index");
            }
        }

        [HttpPost]
        public JsonResult GetContactDetails([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
              
                DataSourceResult result = DB.ContactUs.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

    }
}
