using Alexapps.SkinCare.Entities;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.HomeServices;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.Medications;
using Alexapps.SkinCare.Entities.Notifications;
using Alexapps.SkinCare.Entities.SkinConditions;
using Alexapps.SkinCare.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Alexapps.SkinCare.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class SkinCareDbContext :
    AbpDbContext<SkinCareDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    // 
    public DbSet<Page> Pages { get; set; }
    public DbSet<User> AppUsers { get; set; }
    public DbSet<Otp> Otps { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<DoctorRating> DoctorRatings { get; set; }
    public DbSet<DoctorQualification> DoctorQualifications { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Lab> Labs { get; set; }

    public DbSet<LabBranch> LabBranches { get; set; }
    public DbSet<LabMedicalTest> LabMedicalTests { get; set; }

    public DbSet<LabSchedule> LabSchedules { get; set; }
    public DbSet<HomeService> HomeServices { get; set; }
    public DbSet<HomeServiceProvider> HomeServiceProviders { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<SkinCondition> SkinConditions { get; set; }
    public DbSet<HomeServiceSession> HomeServiceSessions { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    public DbSet<HomeServiceSchedule> HomeServiceSchedules { get; set; }
    public DbSet<DiagnosticSession> DiagnosticSessions { get; set; }
    public DbSet<DiagnosticSessionImage> DiagnosticSessionImages { get; set; }
    public DbSet<DiagnosticSessionMessage> DiagnosticSessionMessages { get; set; }
    public DbSet<DiagnosticSessionTest> DiagnosticSessionTests { get; set; }
    public DbSet<DiagnosticSessionRoom> DiagnosticSessionRooms { get; set; }
    public DbSet<Alexapps.SkinCare.Entities.MedicalTests.TestCategory> TestCategories { get; set; }
    public DbSet<Alexapps.SkinCare.Entities.MedicalTests.MedicalTest> MedicalTests { get; set; }
    public DbSet<Alexapps.SkinCare.Entities.MedicalTests.SampleType> SampleTypes { get; set; }
    public DbSet<Medication> Medications { get; set; }

    public DbSet<DiagnosticSessionTreatmentPlan> DiagnosticSessionTreatmentPlans { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    #endregion

    public SkinCareDbContext(DbContextOptions<SkinCareDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

      
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

    
        builder.Entity<LabBranch>(b => {
            b.ToTable("LabBranches");
            b.ConfigureByConvention();
            b.HasOne(x => x.Lab).WithMany(x => x.Branches).HasForeignKey(x => x.LabId);
        });


        builder.ApplyConfigurationsFromAssembly(typeof(SkinCareDbContext).Assembly);
    }
}



