using SCTimeSheet_DAL.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace SCTimeSheet_DAL
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base("ApplicationDBContext")
        {
            Database.CommandTimeout = 300;
        }

        #region Save Changes

        public override int SaveChanges()
        {
            try
            {
                if (base.Database.CurrentTransaction != null)
                {
                    SetEntityBase();
                    return base.SaveChanges();
                }
                else
                {
                    using (var transaction = base.Database.BeginTransaction())
                    {
                        int result = 0;
                        try
                        {
                            SetEntityBase();
                            result = base.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetEntityBase()
        {
            try
            {
                string userName = HttpContext.Current?.User?.Identity?.Name ?? "";
                Int64 userId = User.Where(x => x.Email == userName).Select(x => x.UserID).FirstOrDefault();

                var added = from e in this.ChangeTracker.Entries()
                            where e.State == EntityState.Added
                            select e;
                foreach (var entry in added)
                {
                    if (entry.Entity.GetType().IsSubclassOf(typeof(EntityBase)))
                    {
                        HandleAdded(entry, userId);
                    }
                }

                var changed = from e in this.ChangeTracker.Entries()
                              where e.State == EntityState.Modified
                              select e;
                foreach (var entry in changed)
                {
                    if (entry.Entity.GetType().IsSubclassOf(typeof(EntityBase)))
                    {
                        HandleUpdated(entry, userId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HandleAdded(DbEntityEntry entry, Int64 userId)
        {
            try
            {
                var baseEntity = entry.Entity as EntityBase;
                baseEntity.CreatedBy = userId;
                baseEntity.CreatedDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HandleUpdated(DbEntityEntry entry, Int64 userId)
        {
            try
            {
                var baseEntity = entry.Entity as EntityBase;

                baseEntity.ModifiedBy = userId;
                baseEntity.ModifiedDate = DateTime.Now;

                this.Entry(baseEntity).Property(i => i.CreatedBy).IsModified = false;
                this.Entry(baseEntity).Property(i => i.CreatedDate).IsModified = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Log

        public DbSet<ErrorLogModel> ErrorLog { get; set; }

        #endregion

        #region Masters

        public DbSet<UserModel> User { get; set; }
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<PageModel> Page { get; set; }
        public DbSet<StatusModel> Status { get; set; }
        public DbSet<PageMappingModel> PageMapping { get; set; }
        public DbSet<ProjectMasterModel> ProjectMaster { get; set; }
        public DbSet<AutoGenModel> AutoGen { get; set; }
        public DbSet<EmployeeModel> Employee { get; set; }
        public DbSet<QuartModel> Quarter { get; set; }
        public DbSet<QuarterListModel> QuarterList { get; set; }
        public DbSet<WorkDaysModel>Workdays { get; set; }
        public DbSet<ProjectEmployeesModel> ProjectEmployee { get; set; }
        public DbSet<ProjectGrantModel> ProjectGrant { get; set; }
        public DbSet<MasterDataModel> MasterData { get; set; }
        public DbSet<ReportModel> Report { get; set; }
        public DbSet<ResearchModel>ResearchMaster { get; set; }
        public DbSet<SettingsModel> Settings { get; set; }

        public DbSet<TimeSheetLockModel> TimeSheetLocks { get; set; }

        public DbSet<ContactUsModel> ContactUs { get; set; }

        public DbSet<AuditTimeSheetInfo> AuditTimeSheetInfo { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<CountryMapping> CountryMappings { get; set; }

        public DbSet<EmployeeAdditionalDetails> EmployeeAdditionalDetails { get; set; }
        public DbSet<EmailTemplates> EmailTemplates { get; set; }
        #endregion

        #region New Entry

        public DbSet<NewEntryModel> EmpTimeSheet { get; set; }
        public DbSet<NewEntryByProjectSelection> NewEntryViaProjectSelect { get; set; }
        //public DbSet<PendingListforTSApproval> PendingListforTSApproval { get; set; }
        #endregion
    }
}
