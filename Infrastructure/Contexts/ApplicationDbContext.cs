using Application.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.Booking;
using Domain.Entities.BookingDetail;
using Domain.Entities.Customer;
using Domain.Entities.Employee;
using Domain.Entities.EmployeeService;
using Domain.Entities.FeedbackFileUpload;
using Domain.Entities.Feedback;
using Domain.Entities.Reply;
using Domain.Entities.Service;
using Domain.Entities.ServiceImage;
using Domain.Entities.View.ViewCustomerBookingHistory;
using Domain.Entities.View.ViewCustomerFeedbackReply;
using Domain.Entities.View.ViewCustomerReviewHistory;
using Domain.Entities.WorkShift;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.EnumMasterData;

namespace Infrastructure.Contexts
{
    public class ApplicationDbContext : AudtableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService) : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        private DbSet<AppUser> AppUsers { get; set; } = default!;
        private DbSet<AppRole> AppRoles { get; set; } = default!;
        private DbSet<AppRoleClaim> AppRoleClaims { get; set; } = default!;
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeService> EmployeeServices { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<FeedbackFileUpload> FeedbackFileUploads { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceImage> ServiceImages { get; set; }
        public virtual DbSet<WorkShift> WorkShifts { get; set; }
        public virtual DbSet<ViewCustomerBookingHistory> ViewCustomerBookingHistories { get; set; }
        public virtual DbSet<ViewCustomerFeedbackReply> ViewCustomerFeedbackReplies { get; set; }
        public virtual DbSet<ViewCustomerReviewHistory> ViewCustomerReviewHistories { get; set; }
        public virtual DbSet<EnumMasterData> EnumMasterDatas { get; set; }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = string.IsNullOrEmpty(_currentUserService.UserName) ? "System" : _currentUserService.UserName;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = string.IsNullOrEmpty(_currentUserService.UserName) ? "System" : _currentUserService.UserName;
                        break;
                }
            }

            return await base.SaveChangesAsync(_currentUserService.UserName, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", "Identity");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");
            });

            builder.Entity<AppRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });
            builder.Entity<ViewCustomerBookingHistory>(entity =>
            {
                entity.ToView("View_CustomerBookingHistory");
                entity.HasNoKey();
            });

            builder.Entity<ViewCustomerFeedbackReply>(entity =>
            {
                entity.ToView("View_CustomerFeedbackReply");
                entity.HasNoKey();
            });

            builder.Entity<ViewCustomerReviewHistory>(entity =>
            {
                entity.ToView("View_CustomerReviewHistory");
                entity.HasNoKey();
            });
        }
    }
}