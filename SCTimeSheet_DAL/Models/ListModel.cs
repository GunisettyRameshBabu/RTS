using SCTimeSheet_UTIL;
using SCTimeSheet_UTIL.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SCTimeSheet_DAL.Models
{
    public class SearchList
    {
        public Int64 ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectGrant { get; set; }
        public decimal? InvPercentage { get; set; }
        public string prgStartDate { get; set; }
        public string prgEndDate { get; set; }
        public string isPM { get; set; }
        public string EmpStartDate { get; set; }
        public string EmpEndDate { get; set; }

    }

    public class TimeSheetList
    {
        public Int64 TsID { get; set; }

        public string ProjectCode { get; set; }

        public Int64 ProjectID { get; set; }

        public string ProjectName { get; set; }

        public string RefNo { get; set; }

        public string InvolveMonth { get; set; }

        public int DaysCount { get; set; }

        public int DaysEditCount { get; set; }

        public decimal InvolvePercent { get; set; }

        public string StatusDesc { get; set; }

        public Int64 ItemNo { get; set; }

        public string ProjectGrant { get; set; }

    }

    public class NewEntryList
    {

        public string ProjectName { get; set; }

        public string InvolveMonth { get; set; }

        public string RefNo { get; set; }

        public int DaysCount { get; set; }

        public int DaysEditCount { get; set; }

        public Int64 ItemNo { get; set; }

        public decimal InvolvePercent { get; set; }

        public string StatusDesc { get; set; }

        public Int64 TsID { get; set; }

        public string Employee { get; set; }

        public Int64 Quart { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Actioner { get; set; }

        public string ActionedOn { get; set; }

        public string ApproveRejectComments { get; set; }

        public string SubmittedOnBehalf { get; set; }

        public string Quard { get; set; }

        public string InvolvementDays1 { get; set; }

        public string InvolvementDays2 { get; set; }

        public string InvolvementDays3 { get; set; }

        public string InvolvementEditDays1 { get; set; }

        public string InvolvementEditDays2 { get; set; }

        public string InvolvementEditDays3 { get; set; }

        public string EmpRemarks { get; set; }

        [Display(Name ="Submitted By")]
        public string SubmittedBy { get; set; }

        public int SequenceNo { get; set; }

        public bool IsEdit1 { get; set; }
        public bool IsEdit2 { get; set; }
        public bool IsEdit3 { get; set; }


    }

    public class MonthList
    {
        public long EmployeeID { get; set; }
        public int Id { get; set; }
        public Int64 ProjectID { get; set; }

        public string ProjectCode { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        [Display(Name = "Grant Type")]
        public string ProjectGrant { get; set; }
        [Display(Name = "% of Involvement")]    //, ResourceType = typeof(ResourceDisplay)
        public decimal InvPercentage { get; set; }

        [Display(Name = "Project Member Name")]
        public string EmpName { get; set; }

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public int? Year { get; set; }

        public string Jan { get; set; }

        public string Feb { get; set; }

        public string Mar { get; set; }

        public string Apr { get; set; }

        public string May { get; set; }

        public string Jun { get; set; }

        public string Jul { get; set; }

        public string Aug { get; set; }

        public string Sep { get; set; }

        public string Oct { get; set; }

        public string Nov { get; set; }

        public string Dec { get; set; }

        public Int64 Status1 { get; set; }
        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; }
    }

    public class MonthList2
    {
        public long EmployeeID { get; set; }
        public int Id { get; set; }
        public Int64 ProjectID { get; set; }

        public string ProjectCode { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        [Display(Name = "Grant Type")]
        public string ProjectGrant { get; set; }
        [Display(Name = "% of Involvement")]    //, ResourceType = typeof(ResourceDisplay)
        public decimal InvPercentage { get; set; }

        [Display(Name = "Project Member Name")]
        public string EmpName { get; set; }

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public List<YearModel> TimeSheet { get; set; }

        public Int64 Status1 { get; set; }
        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; }
    }


    public class YearModel
    {
        public int? Year { get; set; }
        private string jan;
        public string Jan { get { return jan ?? "-"; } set { jan = value; } }

        private string feb;
        public string Feb { get { return feb ?? "-"; } set { feb = value; } }

        private string mar;
        public string Mar { get { return mar ?? "-"; } set { mar = value; } }

        private string apr;
        public string Apr { get { return apr ?? "-"; } set { apr = value; } }

        private string may;
        public string May { get { return may ?? "-"; } set { may = value; } }

        private string jun;
        public string Jun { get { return jun ?? "-"; } set { jun = value; } }

        private string jul;
        public string Jul { get { return jul ?? "-"; } set { jul = value; } }

        private string aug;
        public string Aug { get { return aug ?? "-"; } set { aug = value; } }

        private string sep;
        public string Sep { get { return sep ?? "-"; } set { sep = value; } }

        private string oct;
        public string Oct { get { return oct ?? "-"; } set { oct = value; } }

        private string nov;
        public string Nov { get { return nov ?? "-"; } set { nov = value; } }

        private string dec;
        public string Dec { get { return dec ?? "-"; } set { dec = value; } }
    }
    public class TestClass
    {
        public Int64 ProjectID { get; set; }

        public Int64 EmployeeID { get; set; }
        [Display(Name = "Project Member")]
        public string EmpFirstName { get; set; }
        [Display(Name = "Project Manager")]
        public string IsManager { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
        [Display(Name = "% of Involvement")]
        public decimal InvPercentage { get; set; }

        public List<YearModel> MonthLists { get; set; }
    }

    public class PageMappingList
    {
        public Int64 PageID { get; set; }

        public Int64 RoleID { get; set; }

        public bool IsActive { get; set; }

        public string PageName { get; set; }
    }

    public class ApproveList
    {
        public Int64 TsID { get; set; }

        [Display(Name = "ProjectID", ResourceType = typeof(ResourceDisplay))]
        public Int64? ProjectID { get; set; }

        [Display(Name = "EmpId", ResourceType = typeof(ResourceDisplay))]
        public Int64 EmpId { get; set; }

        public string ProjectName { get; set; }

        public string InvolveMonth { get; set; }

        public string RefNo { get; set; }

        public int DaysCount { get; set; }

        public Int64 ItemNo { get; set; }

        [Required]
        public decimal InvolvePercent { get; set; }

        public string StatusDesc { get; set; }

        public DateTime? ApproveRejectDate { get; set; }

        [Required]
        public string ApproveRejectComments { get; set; }

        public Int64 ApproveRejectUser { get; set; }

        public Int64 ApproveRejectStatus { get; set; }

        public Int64? Status { get; set; }

        public int GrantTypeId { get; set; }

        public int GrantTypeName { get; set; }

        public List<TimeSheetList> Result { get; set; }
    }

    public class ProjectMasterList
    {
        public Int64 ProjectID { get; set; }

        [Display(Name ="S.No")]
        public Int64 ItemNo { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        public string ProjectDesc { get; set; }

        [Display(Name = "Internal Order")]
        public string InternalOrder { get; set; }

        [Display(Name = "Cost Centre")]
        public string CostCentre { get; set; }

        [Display(Name = "Grant")]
        public string ProjectGrant { get; set; }

        [Display(Name = "Research Area")]
        public string ResearchArea { get; set; }

        [Display(Name = "Type of Research")]
        public string TypeofResearch { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Theme")]
        public string Theme { get; set; }

    }


    public class ProjectEmployeeList
    {
        public Int64 ProjectEmpID { get; set; }

        public Int64 ItemNo { get; set; }

        public string ProjectName { get; set; }

        public string Employee { get; set; }

        public string RoleName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Int64? InvPercentage { get; set; }

        public string ProjectGrant { get; set; }

        public bool IsActive { get; set; }
    }

    public class ReportShowListHC
    {
        public string Citizenship { get; set; }

        public decimal? DegreePhd { get; set; } = 0;

        public decimal? DegreeMaster { get; set; } = 0;

        public decimal? DegreeBachelor { get; set; } = 0;

        public decimal? NonDegree { get; set; } = 0;

        public decimal? Technician { get; set; } = 0;

        public decimal? OtherStaff { get; set; } = 0;

        public decimal? Total { get { return DegreeBachelor + DegreePhd + DegreeMaster + NonDegree + OtherStaff + Technician; } }

    }

    public class ReportShowListAgeGroup
    {
        public string Citizenship { get; set; }

        public int? DegreePhd { get; set; } = 0;

        public int? DegreeMaster { get; set; } = 0;

        public int? DegreeBachelor { get; set; } = 0;

        public int? student { get; set; } = 0;

        public int? NonDegree { get; set; } = 0;

        public int? Technician { get; set; } = 0;

        public int? OtherStaff { get; set; } = 0;

        public int? Total { get; set; }

    }

    public class ReportShowListAgeGroupByGender
    {
        public string Citizenship { get; set; }

        public int? DegreePhdMale { get; set; }

        public int? DegreePhdFemale { get; set; }

        public int? DegreeMasterMale { get; set; }

        public int? DegreeMasterFemale { get; set; }

        public int? DegreeBachelorMale { get; set; }

        public int? DegreeBachelorFemale { get; set; }

        public int? studentMale { get; set; }

        public int? studentFemale { get; set; }

        public int? NonDegreeMale { get; set; }

        public int? NonDegreeFemale { get; set; }

        public int? TechnicianMale { get; set; }

        public int? TechnicianFemale { get; set; }

        public int? OtherStaffMale { get; set; }

        public int? OtherStaffFemale { get; set; }

        public int? TotalMale { get; set; }

        public int? TotalFemale { get; set; }

    }

    public class SettingsList
    {
        public Int64 SetID { get; set; }

        public Int64 ItemNo { get; set; }

        public string SetCode { get; set; }

        public Int64 SetValue { get; set; }

    }
    public class ReportInvolvmentPercentgeList
    {
        public string EmployeeName { get; set; }

        public int? Project1 { get; set; }

        public int? Project2 { get; set; }

        public int? Project3 { get; set; }

        public int? Project4 { get; set; }

        public int? Project5 { get; set; }

        public int? Total { get; set; }

    }
    public class ReportInvolvmentPercentgeListDemo
    {
        public Int64 ItemNo { get; set; }
        public string EmployeeName { get; set; }
        public string Project1 { get; set; }
        public string Project2 { get; set; }
        public string Project3 { get; set; }
        public string Project4 { get; set; }
        public string Project5 { get; set; }
        public decimal? Project1Percent { get; set; }
        public decimal? Project2Percent { get; set; }
        public decimal? Project3Percent { get; set; }
        public decimal? Project4Percent { get; set; }
        public decimal? Project5Percent { get; set; }
    }

    public class ReportShowListPostGraduation
    {
        public string Citizenship { get; set; }

        public int? Masterdegree { get; set; }

        public int? phddegree { get; set; }

        public int? Masterdegrees { get; set; }

        public int? phddegrees { get; set; }

        public int? Masterdegreez { get; set; }

        public int? phddegreez { get; set; }

        public int? Total { get; set; }

       


    }

    public class ReportShowListFTE
    {
        public string AreaOfResearch { get; set; }

        public decimal? Masterdegree { get; set; }

        public decimal? phddegree { get; set; }

        public decimal? bachelorDegree { get; set; }

        public decimal? fulltimePGS { get; set; }

        public decimal? nondegree { get; set; }

       
        public decimal? Total { get { return bachelorDegree + phddegree + nondegree + Masterdegree; } }

        public decimal? MasterdegreeFTE { get; set; }

        public decimal? phddegreeFTE { get; set; }

        public decimal? bachelorDegreeFTE { get; set; }

        public decimal? fulltimePGSFTE { get; set; }

        public decimal? nondegreeFTE { get; set; }

       
        public decimal? TotalFTE { get { return bachelorDegreeFTE + phddegreeFTE + nondegreeFTE + MasterdegreeFTE; } }


    }


    public class NewEntryBox
    {
        public List<BoxItems> Items { get; set; }
        public string Month1 { get; set; }
        public string Month2 { get; set; }
        public string Month3 { get; set; }
        public string MonthEdit1 { get; set; }
        public string MonthEdit2 { get; set; }
        public string MonthEdit3 { get; set; }
        public string QID { get; set; }
        public decimal TotalDaysCount1 { get; set; }
        public decimal TotalDaysEditCount1 { get; set; }
        public decimal TotalDaysCount2 { get; set; }
        public decimal TotalDaysEditCount2 { get; set; }
        public decimal TotalDaysCount3 { get; set; }
        public decimal TotalDaysEditCount3 { get; set; }
        public decimal InvolvePercent { get; set; }
        public string CheckMonth { get; set; }

        public decimal Count1 { get; set; }
        public decimal Count2 { get; set; }
        public decimal Count3 { get; set; }

        public bool ShowWarning { get; set; }


        public int CurrentYear { get; set; }

        public void ValidateModel_NewEntry(ModelStateDictionary modelState, Int64 empid, Int64 projectid, DateTime involvmonth, Int64 Dayscount)
        {
            try
            {

                ApplicationDBContext db = new ApplicationDBContext();
                var invmonth = involvmonth.ToString("yyyy/MM /01");

                var GetDetails = db.Database.SqlQuery<ValidationItems>(
                            @"exec " + Constants.P_GetEmpTimesheet_Validation + " @EmpID,@ProjectID,@InvoleMonth",
                            new object[] {
                        new SqlParameter("@EmpID",  empid),
                        new SqlParameter("@ProjectID",  projectid),
                        new SqlParameter("@InvoleMonth",  invmonth)
                            }).FirstOrDefault();

                if (GetDetails != null)
                {
                    var result = ((GetDetails.Dayscount * GetDetails.InvPercentage) / 100);

                    if (Dayscount > result)
                    {
                        modelState.AddModelError("ProjectID", ResourceMessage.DayscountExceed);
                    }
                }
                else
                {
                    if (Dayscount > 22)
                    {
                        modelState.AddModelError("ProjectID", ResourceMessage.DayscountExceed);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class PendingListforApproval
    {
        public Int64 TsID { get; set; }
        public string RefNo { get; set; }
        public int SequenceNo { get; set; }
        public string SubmittedBy { get; set; }
        public string ProjectName { get; set; }

        public string Period { get; set; }
        public string SubmittedOn { get; set; }
        public Int64 Status { get; set; }
        public string ApproveRejectComments { get; set; }
        public string Quart { get; set; }

        public string InvolvementDays1 { get; set; }
        public string InvolvementDays2 { get; set; }
        public string InvolvementDays3 { get; set; }
        public string InvolvementEditDays1 { get; set; }
        public string InvolvementEditDays2 { get; set; }
        public string InvolvementEditDays3 { get; set; }

        public string Month1 { get; set; }

        public string Month2 { get; set; }
        public string Month3 { get; set; }
    }

    public class ApprovedTimesheet
    {
        public Int64 TsID { get; set; }
        public string RefNo { get; set; }
        public string SubmittedBy { get; set; }
        public string ProjectName { get; set; }
        public string Period { get; set; }
        public string Quart { get; set; }
        public string SubmittedOn { get; set; }
        public string StatusDesc { get; set; }
        public string ApproveRejectComments { get; set; }
        public string Actioner { get; set; }
        public string ActionedDate { get; set; }
        public string SubmittedOnBehalf { get; set; }
        public string InvolvementDays1 { get; set; }
        public string InvolvementDays2 { get; set; }
        public string InvolvementDays3 { get; set; }
        public string InvolvementEditDays1 { get; set; }
        public string InvolvementEditDays2 { get; set; }
        public string InvolvementEditDays3 { get; set; }
        public int SequenceNo { get; set; }
        public DateTime? ApproveRejectDate { get; set; }
        public string EmpName { get; set; }
        public bool IsEdit1 { get; set; }
        public bool IsEdit2 { get; set; }
        public bool IsEdit3 { get; set; }
    }
}

public class BoxItems
{
    [Key]
    public string TsID { get; set; }
    public Int64 ProjectID { get; set; }
    public string ProjectName { get; set; }
    public decimal InvolvementDays1 { get; set; }
    public decimal InvolvementDays2 { get; set; }
    public decimal InvolvementDays3 { get; set; }
    public bool IsEdit1 { get; set; }
    public bool IsEdit2 { get; set; }
    public bool IsEdit3 { get; set; }
    [DisplayFormat( DataFormatString = "{0:n2}")]
    [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
    public decimal InvolvementEditDays1 { get; set; }
    [DisplayFormat( DataFormatString = "{0:n2}")]
    [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
    public decimal InvolvementEditDays2 { get; set; }
    [DisplayFormat( DataFormatString = "{0:n2}")]
    [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
    public decimal InvolvementEditDays3 { get; set; }
    public string TotalDaysCount1 { get; set; }
    [DisplayFormat( DataFormatString = "{0:n2}")]
    public string TotalDaysEditCount1 { get; set; }
    public string TotalDaysCount2 { get; set; }
    [DisplayFormat( DataFormatString = "{0:n2}")]
    public string TotalDaysEditCount2 { get; set; }
    public string TotalDaysCount3 { get; set; }
    [DisplayFormat( DataFormatString = "{0:n2}")]
    public string TotalDaysEditCount3 { get; set; }
    public decimal? OldInvolvementEditDays1 { get; set; }
    public decimal? OldInvolvementEditDays2 { get; set; }
    public decimal? OldInvolvementEditDays3 { get; set; }
    public bool IsSubmitted1 { get; set; }
    public bool IsSubmitted2 { get; set; }
    public bool IsSubmitted3 { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}
public class ValidationItems
{
    public Int64 Dayscount { get; set; }
    public Int64? InvPercentage { get; set; }
}
public class NewEntryOnBehalfByProSelect
{
    [Key]
    public Int64 ProjectID { get; set; }
    public Int64 EmployeeID { get; set; }
    public string emp_name { get; set; }
    public string ProjectName { get; set; }
    public int InvolvementDays1 { get; set; }
    public int InvolvementDays2 { get; set; }
    public int InvolvementDays3 { get; set; }
    public int InvolvementEditDays1 { get; set; }
    public int InvolvementEditDays2 { get; set; }
    public int InvolvementEditDays3 { get; set; }
    public string Quarter { get; set; }
}




