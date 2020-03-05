using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet.Common;
using SCTimeSheet.Models;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
namespace SCTimeSheet.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index()
        {
            ViewBag.Reportlist = DropdownList.ReportList();
            return View();
        }


        [HttpPost]
        public JsonResult Generate([DataSourceRequest]DataSourceRequest request, int reportid, DateTime startDate, DateTime endDate)
        {
            try
            {
                object[] reportParams = new object[] {
                        new SqlParameter("@StartDate",startDate),
                        new SqlParameter("@EndDate",endDate)
                           };
                if (reportid == 11)
                {
                    List<ReportShowListHC> GetDetails = GetRDManPowerHC(startDate, endDate);
                    //List<ReportShowListHC> GetDetails = DB.Database.SqlQuery<ReportShowListHC>(
                    //       @"exec " + Constants.P_Report_RDManPowerHC + " @StartDate,@EndDate", reportParams).ToList();

                    //return Json(new { data = GetDetails });

                    return Json(GetDetails.ToDataSourceResult(request));
                }
                if (reportid == 10017)
                {
                    List<ReportShowListHC> GetDetails = GetRDManPowerHCFTE(startDate, endDate);

                    return Json(GetDetails.ToDataSourceResult(request));

                }
                if (reportid == 12)
                {
                    List<ReportShowListAgeGroup> GetDetails = GetRDManPowerHCAgeGroups(startDate, endDate);
                    //List<ReportShowListAgeGroup> GetDetails = DB.Database.SqlQuery<ReportShowListAgeGroup>(
                    //       @"exec " + Constants.P_Report_RDManpowerByAgeGroup + " @StartDate,@EndDate", reportParams).ToList();

                    return Json(GetDetails.ToDataSourceResult(request));
                }
                if (reportid == 13)
                {
                    List<ReportShowListAgeGroupByGender> GetDetails = GetRDManPowerHCByGender(startDate, endDate);
                    //List<ReportShowListAgeGroupByGender> GetDetails = DB.Database.SqlQuery<ReportShowListAgeGroupByGender>(
                    //       @"exec " + Constants.P_Report_RDManpowerAgeGroupBYGender + " @StartDate,@EndDate", reportParams).ToList();

                    return Json(GetDetails.ToDataSourceResult(request));
                }

                if (reportid == 16)
                {
                    List<ReportShowListPostGraduation> GetDetails = DB.Database.SqlQuery<ReportShowListPostGraduation>(
                           @"exec " + Constants.P_Report_PostGraduation_Student + " @StartDate,@EndDate", reportParams).ToList();

                    return Json(new { data = GetDetails });
                }

                if (reportid == 18)
                {
                    List<ReportShowListFTE> GetDetails = GetAreaOfResearchFTE(startDate, endDate);


                    //List<ReportShowListFTE> GetDetails = DB.Database.SqlQuery<ReportShowListFTE>(
                    //       @"exec " + Constants.P_Report_AreaOfResearch_FTE + " @StartDate,@EndDate", reportParams).ToList();

                    return Json(GetDetails.ToDataSourceResult(request));
                }

                if (reportid == 10016)
                {
                    List<ReportInvolvmentPercentgeListDemo> GetDetails = DB.Database.SqlQuery<ReportInvolvmentPercentgeListDemo>(
                           @"exec " + Constants.P_Report_TotalInvolvmentPercentageDemo + " @StartDate,@EndDate", reportParams).ToList();

                    //List<ReportInvolvmentPercentgeListDemo> Students = new List<ReportInvolvmentPercentgeListDemo>();
                    //ListtoDataTable lsttodt = new ListtoDataTable();
                    //DataTable dt = lsttodt.ToDataTable(GetDetails);
                    //DataTable dtReturn = GetInversedDataTable(dt, "Projects", "EmployeeName",
                    //                         "InvolvmentPercentage", "", true);

                    //  return Json(new { data = GetDetails });

                    return Json(GetDetails.ToDataSourceResult(request));

                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return Json(new { data = 0 });
        }

        private List<ReportShowListFTE> GetAreaOfResearchFTE(DateTime startDate, DateTime endDate)
        {
            DateTime invStartMonth = new DateTime(startDate.Year, startDate.Month, 01);
            DateTime invEndMonth = new DateTime(endDate.Year, endDate.Month, 01);
            List<long> researches = ConfigurationManager.AppSettings["TypeEmpResearchRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();

            var fteData = (from rm in DB.ResearchMaster
                           join pm in DB.ProjectMaster on rm.RsID equals pm.ResearchArea
                           join pe in DB.ProjectEmployee on pm.ProjectID equals pe.ProjectID
                           join ed in DB.Employee on pe.EmployeeID equals ed.EmployeeID
                           join md in DB.MasterData on ed.Qualification equals md.MstID
                           join et in DB.EmpTimeSheet on new { pe.EmployeeID, pm.ProjectID } equals new { EmployeeID = et.EmpId, et.ProjectID } 
                           //into ets
                           //from et in ets.DefaultIfEmpty()
                           where
                           (pm.StartDate >= startDate || (pm.StartDate <= startDate && pm.EndDate >= startDate)) && (pm.EndDate >= endDate || (pm.EndDate <= endDate && pm.StartDate >= startDate)) &&
                          pe.EndDate >= startDate &&
                           pm.IsRDProject.Value == 1 && (et.InvolveMonth >= invStartMonth && et.InvolveMonth <= invEndMonth)
                          //orderby rm.RsDesc
                          && researches.Contains(pe.RefRole) 
                          //&& et.DaysEditCount > 0
                           select new { rm, pm, pe, ed, md, et }).ToList();

            int noOfMonths = (((invEndMonth.Year - invStartMonth.Year) * 12) + invEndMonth.Month - invStartMonth.Month) + 1;

            var fteGroupResult = (from x in fteData
                                  orderby x.rm.RsDesc
                                  group x by new { x.rm.RsDesc, x.pm.ProjectID, x.md.MstName, x.pe.EmployeeID } into gcs

                                  select gcs).ToList();

            List<FTEReport> fteSum = new List<FTEReport>();
            foreach (var item in fteGroupResult)
            {
                fteSum.Add(new FTEReport { Sum = item.ToList().Sum(x => x.et.InvolvePercent) / (noOfMonths * 100), EmpId = item.Key.EmployeeID, ResearchType = item.Key.RsDesc, Qualification = item.Key.MstName });
            }

            var rdEmps = (from rm in DB.ResearchMaster
                          join pm in DB.ProjectMaster on rm.RsID equals pm.ResearchArea
                          join pe in DB.ProjectEmployee on pm.ProjectID equals pe.ProjectID
                          join e in DB.Employee on pe.EmployeeID equals e.EmployeeID
                          join md in DB.MasterData on e.Qualification equals md.MstID
                          join et in DB.EmpTimeSheet on new { pe.EmployeeID, pm.ProjectID } equals new { EmployeeID = et.EmpId, et.ProjectID }

                          where
                          (pm.StartDate >= startDate || (pm.StartDate <= startDate && pm.EndDate >= startDate)) && (pm.EndDate >= endDate || (pm.EndDate <= endDate && pm.StartDate >= startDate)) &&
                          pe.EndDate >= startDate &&
                          pm.IsRDProject == 1
                          && researches.Contains(pe.RefRole) && (et.InvolveMonth >= invStartMonth && et.InvolveMonth <= invEndMonth)
                          select new { rm, pm, pe, e, md }).ToList();

            List<long> rdGroupEmps = (from x in rdEmps
                                      orderby x.rm.RsDesc, x.pe.EmployeeID
                                      group x by new { x.pe.EmployeeID } into gcs

                                      select gcs.Key.EmployeeID).ToList();

            List<FTEReport> emps = new List<FTEReport>();
            foreach (long item in rdGroupEmps)
            {
                FTEReport res = rdEmps.Where(x => x.e.EmployeeID == item).Select(x => new FTEReport { ResearchType = x.rm.RsDesc, Qualification = x.md.MstName }).FirstOrDefault();
                emps.Add(res);
            }
            var typeQualification = Convert.ToInt64(ConfigurationManager.AppSettings["TypeQualification"]);
            List<MasterDataModel> qualifications = DB.MasterData.Where(x => x.MstTypeID == typeQualification).ToList();
            IEnumerable<string> availableResearchTypes = fteSum.Select(x => x.ResearchType).Distinct().ToList();
           // IEnumerable<string> availableResearchTypes = DB.ResearchMaster.Select(x => x.RsDesc).OrderBy(x => x).ToList();
            if (!availableResearchTypes.Any())
            {
                availableResearchTypes = emps.Select(x => x.ResearchType).Distinct().ToList();
            }
            List<ReportShowListFTE> GetDetails = new List<ReportShowListFTE>();
           
            foreach (string item in availableResearchTypes)
            {

                ReportShowListFTE reportShowListFTE = new ReportShowListFTE
                {
                    AreaOfResearch = item,
                    bachelorDegreeFTE = Math.Round((from x in fteSum
                                                    join q in qualifications on x.Qualification equals q.MstName
                                                    where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                                    && x.ResearchType == item
                                                    select x.Sum.Value).Sum(), 2),
                    MasterdegreeFTE = Math.Round((from x in fteSum
                                                  join q in qualifications on x.Qualification equals q.MstName
                                                  where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"]) && x.ResearchType == item
                                                  select x.Sum.Value).Sum(), 2),
                    nondegreeFTE = Math.Round((from x in fteSum
                                               join q in qualifications on x.Qualification equals q.MstName
                                               where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"]) && x.ResearchType == item
                                               select x.Sum.Value).Sum(), 2),
                    phddegreeFTE = Math.Round((from x in fteSum
                                               join q in qualifications on x.Qualification equals q.MstName
                                               where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"]) && x.ResearchType == item
                                               select x.Sum.Value).Sum(), 2),
                    bachelorDegree = (from x in emps
                                      join q in qualifications on x.Qualification equals q.MstName
                                      where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"]) && x.ResearchType == item
                                      select x).Count(),
                    Masterdegree = (from x in emps
                                    join q in qualifications on x.Qualification equals q.MstName
                                    where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"]) && x.ResearchType == item
                                    select x).Count(),
                    nondegree = (from x in emps
                                 join q in qualifications on x.Qualification equals q.MstName
                                 where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"]) && x.ResearchType == item
                                 select x).Count(),
                    phddegree = (from x in emps
                                 join q in qualifications on x.Qualification equals q.MstName
                                 where q.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"]) && x.ResearchType == item
                                 select x).Count(),


                };

                GetDetails.Add(reportShowListFTE);
            }

            if (GetDetails.Any())
            {
                GetDetails.Add(new ReportShowListFTE()
                {
                    AreaOfResearch = "Total",
                    bachelorDegreeFTE = GetDetails.Select(x => x.bachelorDegreeFTE).Sum(),
                    fulltimePGSFTE = GetDetails.Select(x => x.fulltimePGSFTE).Sum(),
                    MasterdegreeFTE = GetDetails.Select(x => x.MasterdegreeFTE).Sum(),
                    nondegreeFTE = GetDetails.Select(x => x.nondegreeFTE).Sum(),
                    phddegreeFTE = GetDetails.Select(x => x.phddegreeFTE).Sum(),
                    //TotalFTE = GetDetails.Select(x => x.TotalFTE).Sum(),
                    bachelorDegree = GetDetails.Select(x => x.bachelorDegree).Sum(),
                    fulltimePGS = GetDetails.Select(x => x.fulltimePGS).Sum(),
                    Masterdegree = GetDetails.Select(x => x.Masterdegree).Sum(),
                    nondegree = GetDetails.Select(x => x.nondegree).Sum(),
                    phddegree = GetDetails.Select(x => x.phddegree).Sum(),
                    //Total = GetDetails.Select(x => x.Total).Sum()

                });
            }

            return GetDetails;
        }

        private List<ReportShowListHC> GetRDManPowerHCFTE(DateTime startDate, DateTime endDate)
        {
            DateTime invStartMonth = new DateTime(startDate.Year, startDate.Month, 01);
            DateTime invEndMonth = new DateTime(endDate.Year, endDate.Month, 01);
            List<long> researches = ConfigurationManager.AppSettings["TypeEmpResearchRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> otherTypes = ConfigurationManager.AppSettings["TypeEmpOtherRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> technicians = ConfigurationManager.AppSettings["TypeEmpTechnicianRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var fteData = (from pm in DB.ProjectMaster
                           join pe in DB.ProjectEmployee on pm.ProjectID equals pe.ProjectID
                           join ed in DB.Employee on pe.EmployeeID equals ed.EmployeeID
                           join md in DB.MasterData on ed.Qualification equals md.MstID
                           join et in DB.EmpTimeSheet on new { pe.EmployeeID, pm.ProjectID } equals new { EmployeeID = et.EmpId, et.ProjectID } 
                           //into ets
                           //from et in ets.DefaultIfEmpty()
                           where
                          (pm.StartDate >= startDate || (pm.StartDate <= startDate && pm.EndDate >= startDate)) && (pm.EndDate >= endDate || (pm.EndDate <= endDate && pm.StartDate >= startDate)) &&
                          pe.EndDate >= startDate &&
                           pm.IsRDProject.Value == 1 && (et.InvolveMonth >= invStartMonth && et.InvolveMonth <= invEndMonth)
                           //&& et.DaysEditCount > 0
                           // && ResearchId.Contains(pe.RefRole)
                           //orderby rm.RsDesc
                           select new { pm, pe, ed, md, et }).ToList();

            var otherTypesData = fteData.Where(x => otherTypes.Contains(x.pe.RefRole)).ToList();
            var techniciansData = fteData.Where(x => technicians.Contains(x.pe.RefRole)).ToList();
            fteData = fteData.Where(x => researches.Contains(x.pe.RefRole)).ToList();

            int noOfMonths = (((invEndMonth.Year - invStartMonth.Year) * 12) + invEndMonth.Month - invStartMonth.Month) + 1;

            var fteGroupResult = (from x in fteData

                                  group x by new { x.pm.ProjectID, x.md.MstName, x.pe.EmployeeID } into gcs

                                  select gcs).ToList();

            var otherFteGroupResult = (from x in otherTypesData

                                       group x by new { x.pm.ProjectID, x.pe.EmployeeID } into gcs

                                       select gcs).ToList();

            var technicianFteGroupResult = (from x in techniciansData

                                            group x by new { x.pm.ProjectID, x.pe.EmployeeID } into gcs

                                            select gcs).ToList();
            var typeEmpRole = Convert.ToInt64(ConfigurationManager.AppSettings["TypeEmpRole"]);

            List<MasterDataModel> empRoleList = DB.MasterData.Where(x => x.MstTypeID == typeEmpRole).ToList();

            List<FTEReport> fteSum = new List<FTEReport>();
            foreach (var item in fteGroupResult)
            {
                fteSum.Add(new FTEReport { Sum = item.ToList().Sum(x => x.et.InvolvePercent) / (noOfMonths * 100), EmpId = item.Key.EmployeeID, Qualification = item.Key.MstName, EmpRole = item.FirstOrDefault()?.pe.RefRole });
            }

            List<FTEReport> otherSum = new List<FTEReport>();
            foreach (var item in otherFteGroupResult)
            {
                otherSum.Add(new FTEReport { Sum = item.ToList().Sum(x => x.et.InvolvePercent) / (noOfMonths * 100), EmpId = item.Key.EmployeeID });
            }

            List<FTEReport> techniciansSum = new List<FTEReport>();
            foreach (var item in technicianFteGroupResult)
            {
                techniciansSum.Add(new FTEReport { Sum = item.ToList().Sum(x => x.et.InvolvePercent) / (noOfMonths * 100), EmpId = item.Key.EmployeeID });
            }
            //List<ReportShowListHC> GetDetails = DB.Database.SqlQuery<ReportShowListHC>(
            //       @"exec " + Constants.P_Report_RDManPowerHCFTE + " @StartDate,@EndDate", reportParams).ToList();
            List<ReportShowListHC> GetDetails = new List<ReportShowListHC>();
            var typeQualification = Convert.ToInt64(ConfigurationManager.AppSettings["TypeQualification"]);
            List<MasterDataModel> qualifications = DB.MasterData.Where(x => x.MstTypeID == typeQualification).ToList();
            GetDetails.Add(new ReportShowListHC()
            {
                Citizenship = "Total R&D Manpower FTE",
                DegreeBachelor = Math.Round((from x in fteSum
                                             join y in qualifications on x.Qualification equals y.MstName
                                             where y.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                             select x.Sum.Value).Sum(), 2),
                DegreeMaster = Math.Round((from x in fteSum
                                           join y in qualifications on x.Qualification equals y.MstName
                                           where y.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                           select x.Sum.Value).Sum(), 2),
                DegreePhd = Math.Round((from x in fteSum
                                        join y in qualifications on x.Qualification equals y.MstName
                                        where y.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                        select x.Sum.Value).Sum(), 2),
                NonDegree = Math.Round((from x in fteSum
                                        join y in qualifications on x.Qualification equals y.MstName
                                        where y.MstID == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                        select x.Sum.Value).Sum(), 2),

                Technician = Math.Round(techniciansSum.Sum(x => x.Sum.Value), 2),
                OtherStaff = Math.Round(otherSum.Sum(x => x.Sum.Value), 2),

            });
            return GetDetails;
        }

        private List<ReportShowListHC> GetRDManPowerHC(DateTime startDate, DateTime endDate)
        {
            List<ReportShowListHC> reportShowLists = new List<ReportShowListHC>();
            DateTime invStartMonth = new DateTime(startDate.Year, startDate.Month, 01);
            DateTime invEndMonth = new DateTime(endDate.Year, endDate.Month, 01);
            List<long> researches = ConfigurationManager.AppSettings["TypeEmpResearchRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> otherTypes = ConfigurationManager.AppSettings["TypeEmpOtherRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> technicians = ConfigurationManager.AppSettings["TypeEmpTechnicianRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
           
            var hcData = (from pm in DB.ProjectMaster
                          join pe in DB.ProjectEmployee on pm.ProjectID equals pe.ProjectID
                          join ed in DB.Employee on pe.EmployeeID equals ed.EmployeeID
                          join rd in DB.MasterData on pe.RefRole equals rd.MstID
                          join md in DB.MasterData on ed.Qualification equals md.MstID 
                          join et in DB.EmpTimeSheet on new { pe.EmployeeID, pm.ProjectID } 
                          equals new { EmployeeID = et.EmpId, et.ProjectID }

                          where
                         (pm.StartDate >= startDate || (pm.StartDate <= startDate && pm.EndDate >= startDate)) && (pm.EndDate >= endDate || (pm.EndDate <= endDate && pm.StartDate >= startDate)) &&
                          pe.EndDate >= startDate &&
                         pm.IsRDProject.Value == 1 
                         && (et.InvolveMonth.Year >= startDate.Year && et.InvolveMonth.Year <= endDate.Year)
                         && et.DaysEditCount > 0
                          select new { pm, pe, ed, md }).ToList();

            long prCountryCode = DB.Countries.Where(x => x.CountryCode == "PR").Select(x => x.CountryID).FirstOrDefault();
            long sGCode = DB.Countries.Where(x => x.CountryCode == "SG").Select(x => x.CountryID).FirstOrDefault();
            var lstOtherCountries = new List<long> { sGCode, prCountryCode };
            
            var otherTypesData = hcData.Where(x => otherTypes.Contains(x.pe.RefRole)).ToList();
            var techniciansData = hcData.Where(x => technicians.Contains(x.pe.RefRole)).ToList();
            hcData = hcData.Where(x => researches.Contains(x.pe.RefRole)).ToList();


            var hcGroupData = (from x in hcData
                               orderby x.pe.EmployeeID
                               group x by new { x.pe.EmployeeID } into gcs

                               select gcs).ToList();
            List<long> empList = hcGroupData.Select(x => x.Key.EmployeeID).Distinct().ToList();


            var  groupOthers = (from x in otherTypesData

                              where !empList.Contains(x.pe.EmployeeID)
                              orderby x.pe.EmployeeID
                              group x by x.ed.EmployeeID into gcs
                              select gcs).ToList();

            otherTypesData.Clear();
            foreach (var item in groupOthers)
            {
                otherTypesData.Add(item.ToList().FirstOrDefault());
            }
            empList.AddRange(otherTypesData.Select(x => x.ed.EmployeeID).Distinct().ToList());

            var groupTecnical = (from x in techniciansData
                                 where !empList.Contains(x.pe.EmployeeID)
                                 orderby x.pe.EmployeeID
                                 group x by x.ed.EmployeeID into grp
                                 select grp).ToList();
            techniciansData.Clear();
            foreach (var item in groupTecnical)
            {
                techniciansData.Add(item.ToList().FirstOrDefault());
            }
            reportShowLists.Add(new ReportShowListHC()
            {
                Citizenship = "i) Singapore Citizens",
                DegreeBachelor = (from x in hcData
                                  where x.ed.Nationality == sGCode && x.ed.PermenantResidence == sGCode
                                  && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                  select x.ed.EmployeeID).Distinct().Count(),
                DegreeMaster = (from x in hcData
                                where x.ed.Nationality == sGCode && x.ed.PermenantResidence == sGCode
                                && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                select x.ed.EmployeeID).Distinct().Count(),
                DegreePhd = (from x in hcData
                             where x.ed.Nationality == sGCode && x.ed.PermenantResidence == sGCode
                             && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                             select x.ed.EmployeeID).Distinct().Count(),
                NonDegree = (from x in hcData
                             where x.ed.Nationality == sGCode && x.ed.PermenantResidence == sGCode
                             && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                             select x.ed.EmployeeID).Distinct().Count(),
                OtherStaff = ((from x in otherTypesData
                              where x.ed.Nationality == sGCode && x.ed.PermenantResidence == sGCode

                              select x.ed.EmployeeID).Distinct().Count())  ,
                Technician = (from x in techniciansData
                              where x.ed.Nationality == sGCode && x.ed.PermenantResidence == sGCode

                              select x.ed.EmployeeID).Distinct().Count(),


            });

            reportShowLists.Add(new ReportShowListHC()
            {
                Citizenship = "ii) Permanent Residents (PRs) with Citizenship From",
                Technician = null,
                OtherStaff = null,
                NonDegree = null,
                DegreePhd = null,
                DegreeMaster = null,
                DegreeBachelor = null
            });

            var citigenships = (from x in DB.CountryMappings
                                join y in DB.Countries on x.CountryId equals y.CountryID
                                where y.CountryCode != "SG"
                                orderby x.MapOrder
                                select new { x, y }).Distinct().ToList();

            foreach (var item in citigenships.OrderBy(x => x.x.MapOrder).Select(x => new { x.x.CountrySet , x.y.CountryID }).Distinct())
            {
               // var countryId = citigenships.Where(x => x.x.CountrySet == item.CountrySet).Select(x => x.x.CountryId).FirstOrDefault();
                reportShowLists.Add(new ReportShowListHC()
                {
                    Citizenship = item.CountrySet,
                    DegreeBachelor = (from x in hcData
                                      //join y in citigenships on x.ed.PermenantResidence equals y.y.CountryID
                                      where x.ed.Nationality == item.CountryID && x.ed.PermenantResidence == prCountryCode
                                      && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                      select x.ed.EmployeeID).Distinct().Count(),
                    DegreeMaster = (from x in hcData
                                    //join y in citigenships on x.ed.PermenantResidence equals y.y.CountryID
                                    where x.ed.Nationality == item.CountryID && x.ed.PermenantResidence == prCountryCode
                                    && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"]) 
                                    select x.ed.EmployeeID).Distinct().Count(),
                    DegreePhd = (from x in hcData
                                 //join y in citigenships on x.ed.PermenantResidence equals y.y.CountryID
                                 where x.ed.Nationality == item.CountryID && x.ed.PermenantResidence == prCountryCode
                                 && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"]) 
                                 select x.ed.EmployeeID).Distinct().Count(),
                    NonDegree = (from x in hcData
                                 //join y in citigenships on x.ed.PermenantResidence equals y.y.CountryID
                                 where x.ed.Nationality == item.CountryID && x.ed.PermenantResidence == prCountryCode
                                 && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"]) 
                                 select x.ed.EmployeeID).Distinct().Count(),
                    OtherStaff = (from x in otherTypesData
                                  //join y in citigenships on x.ed.PermenantResidence equals y.y.CountryID
                                  where x.ed.Nationality == item.CountryID && x.ed.PermenantResidence == prCountryCode

                                  select x.ed.EmployeeID).Distinct().Count() ,
                    Technician = (from x in techniciansData
                                  //join y in citigenships on x.ed.PermenantResidence equals y.y.CountryID
                                  where x.ed.Nationality == item.CountryID && x.ed.PermenantResidence == prCountryCode
                                  select x.ed.EmployeeID).Distinct().Count(),
                });
            }
            reportShowLists.Add(new ReportShowListHC()
            {
                Citizenship = "iii) Foreign Citizens with Citizenship From",
                Technician = null,
                OtherStaff = null,
                NonDegree = null,
                DegreePhd = null,
                DegreeMaster = null,
                DegreeBachelor = null
            });
            
            foreach (string item in citigenships.OrderBy(x => x.x.MapOrder).Select(x => x.x.CountrySet).Distinct())
            {
                var countryId = citigenships.Where(x => x.x.CountrySet == item).Select(x => x.x.CountryId).FirstOrDefault();
                reportShowLists.Add(new ReportShowListHC()
                {
                    Citizenship = item,
                    DegreeBachelor = (from x in hcData
                                      join y in citigenships on x.ed.Nationality equals y.y.CountryID
                                      where y.x.CountrySet == item && x.ed.PermenantResidence == countryId
                                      && !lstOtherCountries.Contains(x.ed.Nationality.Value)
                                      && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"]) && x.ed.Nationality != 1
                                      select x.ed.EmployeeID).Distinct().Count(),
                    DegreeMaster = (from x in hcData
                                    join y in citigenships on x.ed.Nationality equals y.y.CountryID

                                    where y.x.CountrySet == item && x.ed.PermenantResidence == countryId
                                    && !lstOtherCountries.Contains(x.ed.Nationality.Value)
                                  && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"]) && x.ed.Nationality != 1
                                    select x.ed.EmployeeID).Distinct().Count(),
                    DegreePhd = (from x in hcData
                                 join y in citigenships on x.ed.Nationality equals y.y.CountryID

                                 where y.x.CountrySet == item && x.ed.PermenantResidence == countryId
                                 && !lstOtherCountries.Contains(x.ed.Nationality.Value)
                            && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"]) && x.ed.Nationality != 1
                                 select x.ed.EmployeeID).Distinct().Count(),
                    NonDegree = (from x in hcData
                                 join y in citigenships on x.ed.Nationality equals y.y.CountryID

                                 where y.x.CountrySet == item && x.ed.PermenantResidence == countryId
                                 && !lstOtherCountries.Contains(x.ed.Nationality.Value)
                            && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"]) && x.ed.Nationality != 1
                                 select x.ed.EmployeeID).Distinct().Count(),
                    OtherStaff = (from x in otherTypesData
                                  join y in citigenships on x.ed.Nationality equals y.y.CountryID

                                  where y.x.CountrySet == item && x.ed.PermenantResidence == countryId
                                  && !lstOtherCountries.Contains(x.ed.Nationality.Value)
                                  select x.ed.EmployeeID).Distinct().Count() 
                                 ,
                    Technician = (from x in techniciansData
                                  join y in citigenships on x.ed.Nationality equals y.y.CountryID

                                  where y.x.CountrySet == item && x.ed.PermenantResidence == countryId
                                  && !lstOtherCountries.Contains(x.ed.Nationality.Value)
                                  select x.ed.EmployeeID).Distinct().Count(),
                });
            }

            reportShowLists.Add(new ReportShowListHC()
            {
                Citizenship = "Total R&D Manpower=(i+ii+iii)",
                DegreeBachelor = reportShowLists.Sum(x => x.DegreeBachelor),
                DegreeMaster = reportShowLists.Sum(x => x.DegreeMaster),
                DegreePhd = reportShowLists.Sum(x => x.DegreePhd),
                NonDegree = reportShowLists.Sum(x => x.NonDegree),
                OtherStaff = reportShowLists.Sum(x => x.OtherStaff),
                Technician = reportShowLists.Sum(x => x.Technician)

            });
            return reportShowLists;
        }

        private List<ReportShowListAgeGroup> GetRDManPowerHCAgeGroups(DateTime startDate, DateTime endDate)
        {
            List<ReportShowListAgeGroup> reportShowLists = new List<ReportShowListAgeGroup>();
            DateTime invStartMonth = new DateTime(startDate.Year, startDate.Month, 01);
            DateTime invEndMonth = new DateTime(endDate.Year, endDate.Month, 01);
            List<long> researches = ConfigurationManager.AppSettings["TypeEmpResearchRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> otherTypes = ConfigurationManager.AppSettings["TypeEmpOtherRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> technicians = ConfigurationManager.AppSettings["TypeEmpTechnicianRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var hcData = (from pm in DB.ProjectMaster
                          join pe in DB.ProjectEmployee on pm.ProjectID equals pe.ProjectID
                          join ed in DB.Employee on pe.EmployeeID equals ed.EmployeeID
                          join md in DB.MasterData on ed.Qualification equals md.MstID
                          join et in DB.EmpTimeSheet on new { pe.EmployeeID, pm.ProjectID } equals new { EmployeeID = et.EmpId, et.ProjectID }
                          where
                         (pm.StartDate >= startDate || (pm.StartDate <= startDate && pm.EndDate >= startDate)) && (pm.EndDate >= endDate || (pm.EndDate <= endDate && pm.StartDate >= startDate)) &&
                          pe.EndDate >= startDate &&
                          pm.IsRDProject.Value == 1 && (et.InvolveMonth >= invStartMonth && et.InvolveMonth <= invEndMonth)
                          && et.DaysEditCount > 0
                          // && ResearchId.Contains(pe.RefRole)
                          //orderby rm.RsDesc
                          select new { pm, pe, ed, md }).ToList();

            var otherTypesData = hcData.Where(x => otherTypes.Contains(x.pe.RefRole)).ToList();
            var techniciansData = hcData.Where(x => technicians.Contains(x.pe.RefRole)).ToList();
            hcData = hcData.Where(x => researches.Contains(x.pe.RefRole)).ToList();

            var hcGroupData = (from x in hcData
                               orderby x.pe.EmployeeID
                               group x by new { x.pe.EmployeeID } into gcs

                               select gcs).ToList();


            List<long> empList = hcGroupData.Select(x => x.Key.EmployeeID).Distinct().ToList();

            

            otherTypesData = (from x in otherTypesData

                              where !empList.Contains(x.pe.EmployeeID)
                              orderby x.pe.EmployeeID

                              select x).ToList();
            empList.AddRange(otherTypesData.Select(x => x.ed.EmployeeID).Distinct().ToList());

            techniciansData = (from x in techniciansData
                               where !empList.Contains(x.pe.EmployeeID)
                               orderby x.pe.EmployeeID

                               select x).ToList();


            List<KeyValuePair<int, int>> ages = new List<KeyValuePair<int, int>>
            {
                new KeyValuePair<int, int>(0, 24),
                new KeyValuePair<int, int>(25, 34),
                new KeyValuePair<int, int>(35, 44),
                new KeyValuePair<int, int>(45, 54),
                new KeyValuePair<int, int>(55, 64),
                new KeyValuePair<int, int>(55, 64),
                new KeyValuePair<int, int>(65, 999)
            };
            long sGCode = DB.Countries.Where(x => x.CountryCode == "SG").Select(x => x.CountryID).FirstOrDefault();
            foreach (KeyValuePair<int, int> item in ages)
            {
                reportShowLists.Add(new ReportShowListAgeGroup()
                {
                    Citizenship = item.Key == 0 ? "i) Under 25 years old" : item.Key == 25 ? "ii) Between 25-34 years old" : item.Key == 35 ? "iii) Between 35-44 years old" : item.Key == 45 ? "iv) Between 45-54 years old" : item.Key == 55 ? "v) Between 55-64 years old" : "vi) 65 years and above",
                    DegreeBachelor = null,
                    DegreeMaster = null,
                    DegreePhd = null,
                    NonDegree = null,
                    OtherStaff = null,
                    student = null,
                    Technician = null

                });
                ReportShowListAgeGroup singapore = new ReportShowListAgeGroup()
                {
                    Citizenship = "Singapore Citizens",
                    DegreeBachelor = (from x in hcData
                                      where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                     && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                      select x.ed.EmployeeID).Distinct().Count(),
                    DegreeMaster = (from x in hcData
                                    where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                   && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                    select x.ed.EmployeeID).Distinct().Count(),
                    DegreePhd = (from x in hcData
                                 where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                 select x.ed.EmployeeID).Distinct().Count(),
                    NonDegree = (from x in hcData
                                 where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                 select x.ed.EmployeeID).Distinct().Count(),
                    OtherStaff = (from x in otherTypesData
                                  where x.ed.Nationality == sGCode
                                 && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                  select x.ed.EmployeeID).Distinct().Count(),

                    Technician = (from x in techniciansData
                                  where x.ed.Nationality == sGCode
                                 && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                  select x.ed.EmployeeID).Distinct().Count(),
                };
                singapore.Total = singapore.DegreeBachelor + singapore.DegreePhd + singapore.DegreeMaster + singapore.NonDegree + singapore.Technician + singapore.OtherStaff;
                reportShowLists.Add(singapore);
                ReportShowListAgeGroup otherCountry = new ReportShowListAgeGroup()
                {
                    Citizenship = "PRs & Foreign Citizens",
                    DegreeBachelor = (from x in hcData
                                      where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                     && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                      select x.ed.EmployeeID).Distinct().Count(),
                    DegreeMaster = (from x in hcData
                                    where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                   && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                    select x.ed.EmployeeID).Distinct().Count(),
                    DegreePhd = (from x in hcData
                                 where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                 select x.ed.EmployeeID).Distinct().Count(),
                    NonDegree = (from x in hcData
                                 where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                 select x.ed.EmployeeID).Distinct().Count(),
                    OtherStaff = (from x in otherTypesData
                                  where x.ed.Nationality != sGCode
                                 && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                  select x.ed.EmployeeID).Distinct().Count(),

                    Technician = (from x in techniciansData
                                  where x.ed.Nationality != sGCode
                                 && CalculateAge(x.ed.EmpDOB.Value) >= item.Key && CalculateAge(x.ed.EmpDOB.Value) <= item.Value
                                  select x.ed.EmployeeID).Distinct().Count(),


                };
                otherCountry.Total = otherCountry.DegreeBachelor + otherCountry.DegreePhd + otherCountry.DegreeMaster + otherCountry.NonDegree + otherCountry.Technician + otherCountry.OtherStaff;
                reportShowLists.Add(otherCountry);
                reportShowLists.Add(new ReportShowListAgeGroup()
                {
                    Citizenship = item.Key == 0 ? "Total(i)" : item.Key == 25 ? "Total(ii)" : item.Key == 35 ? "Total(iii)" : item.Key == 45 ? "Total(iv)" : item.Key == 55 ? "Total(v)" : "Total(vi)",
                    DegreeBachelor = singapore.DegreeBachelor + otherCountry.DegreeBachelor,
                    DegreeMaster = singapore.DegreeMaster + otherCountry.DegreeMaster,
                    DegreePhd = singapore.DegreePhd + otherCountry.DegreePhd,
                    NonDegree = singapore.NonDegree + otherCountry.NonDegree,
                    OtherStaff = singapore.OtherStaff + otherCountry.OtherStaff,

                    Technician = singapore.Technician + otherCountry.Technician,
                    Total = singapore.Total + otherCountry.Total

                });
            }

            reportShowLists.Add(new ReportShowListAgeGroup()
            {
                Citizenship = "Total",
                DegreeMaster = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.DegreeMaster).Sum(),
                DegreePhd = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.DegreePhd).Sum(),
                NonDegree = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.NonDegree).Sum(),
                OtherStaff = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.OtherStaff).Sum(),
                student = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.student).Sum(),
                Technician = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.Technician).Sum(),
                Total = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.Total).Sum(),
                DegreeBachelor = reportShowLists.Where(x => x.Citizenship.StartsWith("Total")).Select(x => x.DegreeBachelor).Sum(),

            });

            return reportShowLists;
        }

        private List<ReportShowListAgeGroupByGender> GetRDManPowerHCByGender(DateTime startDate, DateTime endDate)
        {
            List<ReportShowListAgeGroupByGender> reportShowLists = new List<ReportShowListAgeGroupByGender>();
            DateTime invStartMonth = new DateTime(startDate.Year, startDate.Month, 01);
            DateTime invEndMonth = new DateTime(endDate.Year, endDate.Month, 01);
            List<long> researches = ConfigurationManager.AppSettings["TypeEmpResearchRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> otherTypes = ConfigurationManager.AppSettings["TypeEmpOtherRolesRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            List<long> technicians = ConfigurationManager.AppSettings["TypeEmpTechnicianRole"].Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var hcData = (from pm in DB.ProjectMaster
                          join pe in DB.ProjectEmployee on pm.ProjectID equals pe.ProjectID
                          join ed in DB.Employee on pe.EmployeeID equals ed.EmployeeID
                          join md in DB.MasterData on ed.Qualification equals md.MstID
                          join et in DB.EmpTimeSheet on new { pe.EmployeeID, pm.ProjectID } equals new { EmployeeID = et.EmpId, et.ProjectID }

                          where
                          (pm.StartDate >= startDate || (pm.StartDate <= startDate && pm.EndDate >= startDate)) && (pm.EndDate >= endDate || (pm.EndDate <= endDate && pm.StartDate >= startDate)) &&
                          pe.EndDate >= startDate &&
                          pm.IsRDProject.Value == 1 && (et.InvolveMonth >= invStartMonth && et.InvolveMonth <= invEndMonth)
                          && et.DaysEditCount > 0
                          // && ResearchId.Contains(pe.RefRole)
                          //orderby rm.RsDesc
                          select new { pm, pe, ed, md }).ToList();

            var otherTypesData = hcData.Where(x => otherTypes.Contains(x.pe.RefRole)).ToList();
            var techniciansData = hcData.Where(x => technicians.Contains(x.pe.RefRole)).ToList();
            hcData = hcData.Where(x => researches.Contains(x.pe.RefRole)).ToList();

            var hcGroupData = (from x in hcData
                               orderby x.pe.EmployeeID
                               group x by new { x.pe.EmployeeID } into gcs

                               select gcs).ToList();


            List<long> empList = hcGroupData.Select(x => x.Key.EmployeeID).Distinct().ToList();

            
            otherTypesData = (from x in otherTypesData

                              where !empList.Contains(x.pe.EmployeeID)
                              orderby x.pe.EmployeeID

                              select x).ToList();

            empList.AddRange(otherTypesData.Select(x => x.ed.EmployeeID).Distinct().ToList());


            techniciansData = (from x in techniciansData
                               where !empList.Contains(x.pe.EmployeeID)
                               orderby x.pe.EmployeeID

                               select x).ToList();
            long sGCode = DB.Countries.Where(x => x.CountryCode == "SG").Select(x => x.CountryID).FirstOrDefault();
            var male = ConfigurationManager.AppSettings["Male"];
            var female = ConfigurationManager.AppSettings["FeMale"];
            ReportShowListAgeGroupByGender singapore = new ReportShowListAgeGroupByGender()
            {
                Citizenship = "Total Singapore Citizens",
                DegreeBachelorFemale = (from x in hcData
                                        where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                       && x.ed.EmpGender == female
                                        select x.ed.EmployeeID).Distinct().Count(),
                DegreeBachelorMale = (from x in hcData
                                      where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                     && x.ed.EmpGender == male
                                      select x.ed.EmployeeID).Distinct().Count(),
                DegreeMasterFemale = (from x in hcData
                                      where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                     && x.ed.EmpGender == female
                                      select x.ed.EmployeeID).Distinct().Count(),
                DegreeMasterMale = (from x in hcData
                                    where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                   && x.ed.EmpGender == male
                                    select x.ed.EmployeeID).Distinct().Count(),
                DegreePhdFemale = (from x in hcData
                                   where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                  && x.ed.EmpGender == female
                                   select x.ed.EmployeeID).Distinct().Count(),
                DegreePhdMale = (from x in hcData
                                 where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                && x.ed.EmpGender == male
                                 select x.ed.EmployeeID).Distinct().Count(),
                NonDegreeFemale = (from x in hcData
                                   where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                  && x.ed.EmpGender == female
                                   select x.ed.EmployeeID).Distinct().Count(),
                NonDegreeMale = (from x in hcData
                                 where x.ed.Nationality == sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                && x.ed.EmpGender == male
                                 select x.ed.EmployeeID).Distinct().Count(),
                OtherStaffFemale = (from x in otherTypesData
                                    where x.ed.Nationality == sGCode
                                   && x.ed.EmpGender == female
                                    select x.ed.EmployeeID).Distinct().Count(),
                OtherStaffMale = (from x in otherTypesData
                                  where x.ed.Nationality == sGCode
                                 && x.ed.EmpGender == male
                                  select x.ed.EmployeeID).Distinct().Count(),
                TechnicianFemale = (from x in techniciansData
                                    where x.ed.Nationality == sGCode
                                   && x.ed.EmpGender == female
                                    select x.ed.EmployeeID).Distinct().Count(),
                TechnicianMale = (from x in techniciansData
                                  where x.ed.Nationality == sGCode
                                 && x.ed.EmpGender == male
                                  select x.ed.EmployeeID).Distinct().Count()

            };

            singapore.TotalFemale = singapore.DegreeBachelorFemale + singapore.DegreeMasterFemale + singapore.DegreePhdFemale + singapore.NonDegreeFemale + singapore.TechnicianFemale + singapore.OtherStaffFemale;
            singapore.TotalMale = singapore.DegreeBachelorMale + singapore.DegreeMasterMale + singapore.DegreePhdMale + singapore.NonDegreeMale + singapore.TechnicianMale + singapore.OtherStaffMale;
            reportShowLists.Add(singapore);

            ReportShowListAgeGroupByGender otherPR = new ReportShowListAgeGroupByGender()
            {
                Citizenship = "Total otherPR Citizens",
                DegreeBachelorFemale = (from x in hcData
                                        where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                       && x.ed.EmpGender == female
                                        select x.ed.EmployeeID).Distinct().Count(),
                DegreeBachelorMale = (from x in hcData
                                      where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["BachelorId"])
                                     && x.ed.EmpGender == male
                                      select x.ed.EmployeeID).Distinct().Count(),
                DegreeMasterFemale = (from x in hcData
                                      where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                     && x.ed.EmpGender == female
                                      select x.ed.EmployeeID).Distinct().Count(),
                DegreeMasterMale = (from x in hcData
                                    where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["MasterId"])
                                   && x.ed.EmpGender == male
                                    select x.ed.EmployeeID).Distinct().Count(),
                DegreePhdFemale = (from x in hcData
                                   where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                  && x.ed.EmpGender == female
                                   select x.ed.EmployeeID).Distinct().Count(),
                DegreePhdMale = (from x in hcData
                                 where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["PhdId"])
                                && x.ed.EmpGender == male
                                 select x.ed.EmployeeID).Distinct().Count(),
                NonDegreeFemale = (from x in hcData
                                   where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                  && x.ed.EmpGender == female
                                   select x.ed.EmployeeID).Distinct().Count(),
                NonDegreeMale = (from x in hcData
                                 where x.ed.Nationality != sGCode && x.ed.Qualification == Convert.ToInt64(ConfigurationManager.AppSettings["NonDegreeId"])
                                && x.ed.EmpGender == male
                                 select x.ed.EmployeeID).Distinct().Count(),
                OtherStaffFemale = (from x in otherTypesData
                                    where x.ed.Nationality != sGCode
                                   && x.ed.EmpGender == female
                                    select x.ed.EmployeeID).Distinct().Count(),
                OtherStaffMale = (from x in otherTypesData
                                  where x.ed.Nationality != sGCode
                                 && x.ed.EmpGender == male
                                  select x.ed.EmployeeID).Distinct().Count(),
                TechnicianFemale = (from x in techniciansData
                                    where x.ed.Nationality != sGCode
                                   && x.ed.EmpGender == female
                                    select x.ed.EmployeeID).Distinct().Count(),
                TechnicianMale = (from x in techniciansData
                                  where x.ed.Nationality != sGCode
                                 && x.ed.EmpGender == male
                                  select x.ed.EmployeeID).Distinct().Count()

            };

            otherPR.TotalFemale = otherPR.DegreeBachelorFemale + otherPR.DegreeMasterFemale + otherPR.DegreePhdFemale + otherPR.NonDegreeFemale + otherPR.TechnicianFemale + otherPR.OtherStaffFemale;
            otherPR.TotalMale = otherPR.DegreeBachelorMale + otherPR.DegreeMasterMale + otherPR.DegreePhdMale + otherPR.NonDegreeMale + otherPR.TechnicianMale + otherPR.OtherStaffMale;
            reportShowLists.Add(otherPR);

            reportShowLists.Add(new ReportShowListAgeGroupByGender()
            {
                Citizenship = "Total",
                DegreeBachelorFemale = singapore.DegreeBachelorFemale + otherPR.DegreeBachelorFemale,
                OtherStaffMale = singapore.OtherStaffMale + otherPR.OtherStaffMale,
                DegreeBachelorMale = singapore.DegreeBachelorMale + otherPR.DegreeBachelorMale,
                DegreeMasterFemale = singapore.DegreeMasterFemale + otherPR.DegreeMasterFemale,
                DegreeMasterMale = singapore.DegreeMasterMale + otherPR.DegreeMasterMale,
                DegreePhdFemale = singapore.DegreePhdFemale + otherPR.DegreePhdFemale,
                DegreePhdMale = singapore.DegreePhdMale + otherPR.DegreePhdMale,
                NonDegreeFemale = singapore.NonDegreeFemale + otherPR.NonDegreeFemale,
                NonDegreeMale = singapore.NonDegreeMale + otherPR.NonDegreeMale,
                OtherStaffFemale = singapore.OtherStaffFemale + otherPR.OtherStaffFemale,
                TechnicianFemale = singapore.TechnicianFemale + otherPR.TechnicianFemale,
                TechnicianMale = singapore.TechnicianMale + otherPR.TechnicianMale,
                TotalFemale = singapore.TotalFemale + otherPR.TotalFemale,
                TotalMale = singapore.TotalMale + otherPR.TotalMale
            });

            return reportShowLists;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
            {
                age = age - 1;
            }

            return age;
        }
    }
}