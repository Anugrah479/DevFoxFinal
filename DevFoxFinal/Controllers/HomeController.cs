using DevFoxFinal.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DevFoxFinal.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        Devfox1Entities1 db = new Devfox1Entities1();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MasterList()
        {
            //var list = db.tbl_course.ToList();
            //return View(list);

            return View(db.tbl_course.OrderByDescending(x => x.ID).ToList());
        }

        //public string uploadfile(HttpPostedFileBase file)
        //{
        //    Random r = new Random();
        //    string path = "-1";
        //    int random = r.Next();
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        string extension = Path.GetExtension(file.FileName);
        //        if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
        //        {
        //            try
        //            {
        //                path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
        //                file.SaveAs(path);
        //                path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
        //                //ViewBag.Message = "File uploaded Successfully";
        //            }
        //            catch (Exception ex)
        //            {
        //                path = "-1";
        //            }

        //        }
        //        else
        //        {
        //            Response.Write("<script>alert('only jpg,jpeg or png formats are acceptable...');</script>");
        //        }
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert(Please select a file');</script>");
        //        path = "-1";
        //    }


        //    return path;

        //}
        [HttpGet]
        public ActionResult GetDurationsAndFeesForCourse1(string course)
        {
            if (!string.IsNullOrEmpty(course))
            {
                var durationsAndFees1 = db.tbl_course
                    .Where(c => c.courses == course)
                    .Select(c => new { Duration = c.Duration, Fee = c.Fees })
                    .Distinct()
                    .ToList();

                return Json(durationsAndFees1, JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }
        public ActionResult StuEnquiry()
        {
            var uniqueCourses = db.tbl_course.Select(c => c.courses).Distinct().ToList();
            ViewBag.CoursesList = new SelectList(uniqueCourses);
            return View();
        }
        [HttpPost]
        public ActionResult StuEnquiry(tbl_Enquiry en)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Enquiry.Add(en);
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Enquirylist");
            }
            return View();
        }

        public ActionResult Enquirylist()
        {
            return View(db.tbl_Enquiry.OrderByDescending(x => x.Enquiryno).ToList());
        }
        public ActionResult CourseMaster()
        {
            return View();
        }



        [HttpPost]
        public ActionResult CourseMaster(tbl_course course, HttpPostedFileBase imgfile)
        {


            if (imgfile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(imgfile.FileName);
                string extension = Path.GetExtension(imgfile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                course.File = "~/CourseImg/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/CourseImg/"), fileName);
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (imgfile.ContentLength <= 1000000)
                    {

                        db.tbl_course.Add(course);


                        if (db.SaveChanges() > 0)
                        {

                            imgfile.SaveAs(fileName);
                            //ViewBag.msg = "Data Saved";
                            ModelState.Clear();
                            return RedirectToAction("MasterList");

                        }


                        else
                        {
                            ViewBag.msg = "Size is not valid";
                        }
                    }
                }
            }
            else if (imgfile == null)
            {
                db.tbl_course.Add(course);

                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("MasterList");
            }
            return View();
            //return RedirectToAction("MasterList");
            // TempData["msg"] = "Data Added Successfully";


            //string fileName = Path.GetFileNameWithoutExtension(imgfile.FileName);
            //string extension = Path.GetExtension(imgfile.FileName);
            //fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
            //fileName = Path.Combine(Server.MapPath("~/CourseImg/"), fileName);
            //course.File = "~/CourseImg/" + fileName;
            //if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg")
            //{
            //    if (imgfile.ContentLength <= 1000000)
            //    {
            //        db.Entry(course).State = EntityState.Modified;
            //        if (db.SaveChanges() > 0)
            //        {
            //            imgfile.SaveAs(fileName);
            //            TempData["msg"] = "Master Updated";
            //            return RedirectToAction("MasterList");
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.msg = "File Size must be equal or less than 1 mb";
            //    }
            //    }
            //    else
            //    {
            //        ViewBag.msg = "Invalid File Type";
            //    }



            //string fileName = Path.GetFileNameWithoutExtension(imgfile.FileName);
            //string extension = Path.GetExtension(imgfile.FileName);
            //fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
            //course.File = "~/CourseImg/" + fileName;
            //fileName = Path.Combine(Server.MapPath("~/CourseImg/"), fileName);
            //imgfile.SaveAs(fileName);

            //db.tbl_course.Add(course);
            //db.SaveChanges();
            //return RedirectToAction("MasterList");

        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var list = db.tbl_course.Find(id);
            Session["imgpath"] = list.File;
            if (list == null)
            {
                return HttpNotFound();
            }
            return View(list);
            ////var list = db.tbl_course.ToList(


        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase imgfile, tbl_course course)
        {
            if (ModelState.IsValid)
            {
                if (imgfile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imgfile.FileName);
                    string extension = Path.GetExtension(imgfile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    course.File = "~/CourseImg/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/CourseImg/"), fileName);
                    imgfile.SaveAs(fileName);
                    string oldimgpath = Request.MapPath(Session["imgPath"].ToString());
                    //db.tbl_course.Add(course);
                    db.Entry(course).State = EntityState.Modified;
                    db.SaveChanges();
                    if (System.IO.File.Exists(oldimgpath))
                    {
                        System.IO.File.Delete(oldimgpath);
                    }
                    TempData["msg"] = "Data Updated";
                    return RedirectToAction("MasterList");

                    //string fileName = Path.GetFileNameWithoutExtension(imgfile.FileName);
                    //string extension = Path.GetExtension(imgfile.FileName);
                    //fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    //fileName = Path.Combine(Server.MapPath("~/CourseImg/"), fileName);
                    //course.File = "~/CourseImg/" + fileName;
                    //if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg")
                    //{
                    //    if (imgfile.ContentLength <= 1000000)
                    //    {
                    //        db.Entry(course).State = EntityState.Modified;
                    //        if (db.SaveChanges() > 0)
                    //        {
                    //            imgfile.SaveAs(fileName);
                    //            TempData["msg"] = "Master Updated";
                    //            return RedirectToAction("MasterList");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ViewBag.msg = "File Size must be equal or less than 1 mb";
                    //    }
                    //    }
                    //    else
                    //    {
                    //        ViewBag.msg = "Invalid File Type";
                    //    }
                }
                else
                {
                    course.File = Session["imgpath"].ToString();
                    db.Entry(course).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        //db.Entry(course).State = EntityState.Modified;
                        TempData["msg"] = "Data Updated";
                        return RedirectToAction("MasterList");
                    }


                }


            }
            return View();
        }
        public ActionResult Details(int id)
        {
            var listid = db.tbl_course.SingleOrDefault(x => x.ID == id);
            return View(listid);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var list = db.tbl_course.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            string currentImg = Request.MapPath(list.File);

            db.Entry(list).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(currentImg))
                {
                    System.IO.File.Delete(currentImg);
                }
                TempData["msg"] = "Data Deleted";
                return RedirectToAction("MasterList");
            }
            return View();
        }

        //public JsonResult GetCategories()

        //{
        //   // tbl_course model = db.tbl_course.Select(x => x.courses).Include(x => x.FirstOrDefault();
        //    return Json();
        //}
        //public ActionResult GetFeesAndDuration(string courseName)
        //{
        //    var course = db.tbl_course.FirstOrDefault(c => c.courses == courseName);

        //    if (course != null)
        //    {
        //        var feesAndDuration = new { Fees = course.Fees, Duration = course.Duration };
        //        return Json(feesAndDuration, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(null);
        //}

        //public ActionResult Studentregistration()
        //{
        //    var courseNames = db.tbl_course.Select(c => c.courses).Distinct().ToList();
        //    ViewBag.CourseNames = new SelectList(courseNames);



        //    return View();
        //    //var items = db.tbl_course.ToList();
        //    //ViewBag.Item = new SelectList(items, "ID", "courses").FirstOrDefault();
        //    //return View();

        //}
        //[HttpPost]
        //public ActionResult Studentregistration(tbl_studentregisteration reg)
        //{
        //    if (ModelState.IsValid)
        //    {

        //    }
        //    return View();
        //}
        [HttpGet]
        public ActionResult Studentregistration(int id = 0)
        {

            tbl_studentregisteration reg = new tbl_studentregisteration();
            var lastreg = db.tbl_studentregisteration.OrderByDescending(x => x.ID).FirstOrDefault();
            if (id != 0)
            {
                reg = db.tbl_studentregisteration.Where(x => x.ID == id).FirstOrDefault<tbl_studentregisteration>();
            }
            else if (lastreg == null)
            {
                reg.uniqueID = "ENQ ID000001";
            }
            else
            {
                reg.uniqueID = "ENQ ID" + (Convert.ToInt32(lastreg.uniqueID.Substring(9, lastreg.uniqueID.Length - 9)) + 1).ToString("D6");
            }
            var uniqueCourses = db.tbl_course.Select(c => c.courses).Distinct().ToList();
            ViewBag.CoursesList = new SelectList(uniqueCourses);


            return View(reg);
        }
        [HttpPost]
        public ActionResult SaveStudentRegistration(tbl_studentregisteration reg)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Assuming 'db' is your DbContext instance (Devfox1Entities1)
                    //reg.Registrationdate = DateTime.Today.Date;
                    //reg.Bod = DateTime.Today.Date;

                    db.tbl_studentregisteration.Add(reg);
                    db.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("StudentList");
                    // Redirect to a success page or another appropriate action
                    //return Json(new { success = true, redirectUrl = Url.Action("RegistrationSuccess", "Home") });
                }
                catch (Exception ex)
                {
                    // Handle exceptions or errors here, if needed
                    ViewBag.Error = "An error occurred while saving the registration: " + ex.Message;
                    // You can choose to log the exception details for debugging or other purposes
                    // LogException(ex);

                    return Json(new { success = false, errorMessage = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();

            return Json(new { success = false, errors });
        }
        [HttpGet]
        public ActionResult GetDurationsAndFeesForCourse(string course)
        {
            if (!string.IsNullOrEmpty(course))
            {
                var durationsAndFees = db.tbl_course
                    .Where(c => c.courses == course)
                    .Select(c => new { Duration = c.Duration, Fee = c.Fees })
                    .Distinct()
                    .ToList();

                return Json(durationsAndFees, JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }
        public ActionResult StudentList()
        {
            List<tbl_studentregisteration> yourList = db.tbl_studentregisteration.ToList();

            // Add serial number to each entity
            for (int i = 0; i < yourList.Count; i++)
            {
                yourList[i].Serialno = (i + 1).ToString();
            }

            return View(yourList);


            // return View(db.tbl_studentregisteration.OrderByDescending(x => x.ID).ToList());
        }


    }


}



