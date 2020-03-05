namespace SCTimeSheet_UTIL
{
    public class Constants
    {
        #region Session Name

        public const string SessionUserID = "UserID";
        public const string SessionRoleID = "RoleID";
        public const string SessionPageAccess = "PageAccess";
        public const string SessionEmpID = "EmployeeID";
        public const string SessionEmpName = "EmployeeName";

        #endregion

        #region Access

        public const string Access = "Accessible";

        #endregion  
        
        #region Stored Procedure

        public const string P_GetEmpTimesheet = "P_GetEmpTimesheet";
        public const string P_GetEmpTimesheet_NewEntry = "P_GetEmpTimesheet_NewEntry";
        public const string P_ProjectList_Employee = "P_ProjectList_Employee";
        public const string P_GetEmpTimesheet_NewEntryPOPUP = "P_GetEmpTimesheet_NewEntryPOPUP";
        public const string P_GetEmpTimesheet_Approve = "P_GetEmpTimesheet_Approve";
        public const string P_Employee_ProjectList = "P_Employee_ProjectList";
        public const string P_GetEmpTimesheet_Approve_Read = "P_GetEmpTimesheet_Approve_Read";
        public const string P_GetNewEntryDefault = "P_GetNewEntryDefault";
        public const string P_GetEmpTimesheet_Validation = "P_GetEmpTimesheet_Validation";
        public const string P_GetEmpTimesheet_ProjectMasterEntry = "P_GetEmpTimesheet_ProjectMasterEntry";
        public const string P_GetEmpTimesheet_ProjectEmployeeAssignEntry = "P_GetEmpTimesheet_ProjectEmployeeAssignEntry";
        public const string P_Grant_ProjectList = "P_Grant_ProjectList";
        public const string P_Report_RDManPowerHC = "P_Report_RDManPowerHC";
        public const string P_Report_RDManPowerHCFTE = "P_Report_RDManPowerHC_FTE";
        public const string P_Report_RDManpowerByAgeGroup = "P_Report_RDManpowerByAgeGroup";
        public const string P_Report_RDManpowerAgeGroupBYGender = "P_Report_RDManpowerAgeGroupBYGender";
        public const string P_GetEmpTimesheet_SettingsAdmin = "P_GetEmpTimesheet_SettingsAdmin";
        public const string P_Report_TotalInvolvmentPercentageDemo = "P_Report_TotalInvolvmentPercentageDemo";
        public const string P_Report_PostGraduation_Student = "P_Report_PostGraduation_Student";
        public const string P_Report_AreaOfResearch_FTE = "P_Report_AreaOfResearch_FTE";
        public const string P_GetProjectList = "P_GetProjectList";
        public const string P_GetMasterListOfTotalProjectInvolvement= "P_GetMasterListOfTotalProjectInvolvement";
        public const string P_GetMasterListOfTotalProjectHistoryInvolvement = "P_GetMasterListOfTotalProjectHistoryInvolvement";
        public const string P_GetMasterListOfTotalProjectHistoryInvolvementAuditLog = "P_GetMasterListOfTotalProjectInvolvementAuditLog";
        



        //New Procedures
        public const string P_GetEmpTimesheet_NewEntry_Test = "P_GetEmpTimesheet_NewEntry_Test";
        public const string P_GetNewEntryDefault_Test = "P_GetNewEntryDefault_Test";
        public const string P_GetEmpTimesheetBy_Grant = "P_GetEmpTimesheetBy_Grant";

        public const string P_GetNewEntryOnBehalfByProjectSelection = "P_GetNewEntryOnBehalfByProjectSelection";

        public const string P_GetEmpTimesheet_Approve_Test = "P_GetEmpTimesheet_Approve_Test";
        public const string P_GetManager_ProjectList = "P_GetManager_ProjectList";
        public const string P_GetEmp_Project_Details = "P_GetEmp_Project_Details";
        public const string P_GetEmp_Project_Details_Edit = "P_GetEmp_Project_Details_Edit";
        public const string P_UpdateEmployeeList = "P_UpdateEmployeeList";
        public const string P_GetEmployeeSearch = "P_GetEmployeeSearch";
        public const string P_InsertProjectEmployee = "P_InsertProjectEmployee";
        public const string P_GetProjectDate = "P_GetProjectDate";
        public const string P_UpdateProjectList = "P_UpdateProjectList";
        public const string P_GetProject_Details_Edit = "P_GetProject_Details_Edit";
        public const string P_CheckNewEntrySubmit = "P_CheckNewEntrySubmit";
        public const string P_GetEmpProjectTS = "P_GetEmpProjectTS";



        public const string P_GetNewEntryOnBehalfByEmployeeSelection = "P_GetNewEntryOnBehalfByEmployeeSelection";  //ak
        public const string P_EmployeeList_ProjectManager = "P_EmployeeList_ProjectManager";    //ak
        public const string P_GetEmp_PendingTimesheet = "P_GetEmp_PendingTimesheet";    //ak
        public const string P_GetEmpPendingTimesheet_POPUP = "P_GetEmpPendingTimesheet_POPUP";  //ak
        public const string P_GetEmp_ActionedTimesheet = "P_GetEmp_ActionedTimesheet";  //ak
        public const string P_UpdateInvolvementPercentage = "P_UpdateInvolvementPercentage";
        #endregion

        #region Pages
        public const string PageManagerDashboard = "managerdashboard";
        public const string PageEmployeeDashboard = "employeedashboard";
        public const string PageNewEntry = "newentry";
        public const string PageNewEntryBehalf = "newentryonbehalfone";
        public const string PageAdminDashboard = "admindashboard";
        public const string PageProjectMaster = "projectmaster";
        public const string PageProjectEmployee = "projectemployee";
        public const string PageReport = "report";
        public const string PageStatisticsReport = "statisticsreport";
        public const string PageFullTimesheetReport = "fulltimesheetreport";
        public const string PageSettings = "settings";
        public const string PageTimesheetApproval = "timesheetapproval";
        public const string PageProjectMainList = "projectmain";
        public const string PageMasterListOfInvolvement = "masterlistofinvolvement";
        public const string AdminTimeSheetLock = "admintimesheetlock";
        public const string SendEmail = "sendemail";
        public const string PageContactUs = "contactus";
        public const string PageCommonMaster = "commonmaster";
        public const string Users = "users";
        #endregion
    }
}
