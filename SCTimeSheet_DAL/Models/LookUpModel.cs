using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    public class ProjectLookUp
    {
        public Int64 ProjectID { get; set; }
        public string ProjectName { get; set; }
        public bool IsActive { get; set; }
    }

    public class StatusLookUp
    {
        public Int64 StatusID { get; set; }
        public string StatusDesc { get; set; }
    }

    public class EmployeeLookUp
    {
        public Int64 EmployeeID { get; set; }
        public string EmpName { get; set; }
    }
    public class MasterLookUp
    {
        public Int64 MstID { get; set; }
        public string MstCode { get; set; }
    }
    public class ProjectLookUpAdmin
    {
        public Int64 ProjectID { get; set; }
        public string ProjectName { get; set; }
    }
    public class RoleLookup
    {
        public Int64 RoleID { get; set; }
        public string RoleName { get; set; }
    }
    public class ReportLookup
    {
        public Int64 ReportID { get; set; }
        public string ReportName { get; set; }
    }
    public class ResearchLookup
    {
        public Int64 RsID { get; set; }
        public string RsDesc { get; set; }
    }
    public class CostCenterLookup
    {
        public Int64 CostID { get; set; }
        public string CostName { get; set; }
    }
}
