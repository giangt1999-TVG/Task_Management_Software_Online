using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tms_api.Models;

namespace tms_api.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<ActivityDetail> ActivityDetail { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectMember> ProjectMember { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<TaskLabel> TaskLabel { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Label> Label { get; set; }
        public virtual DbSet<Lists> Lists { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<UserNotification> UserNotification { get; set; }
        public virtual DbSet<Checklist> Checklist { get; set; }
        public virtual DbSet<TaskPriority> TaskPriority { get; set; }
        public virtual DbSet<TaskStatuses> TaskStatus { get; set; }
        public virtual DbSet<Dependency> Dependency { get; set; }
        public virtual DbSet<TaskDependency> TaskDependency { get; set; }
        public virtual DbSet<FileAttachment> FileAttachment { get; set; }
        public virtual DbSet<FbUserToken> FbUserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.FullName)
                    .HasMaxLength(250);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.HasIndex(e => e.RollNumber)
                    .IsUnique();

                entity.Property(e => e.AvatarUrl)
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserToken");
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

                entity.Property(e => e.ActivityDetailId).HasColumnName("ActivityDetailID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectId).HasColumnName("ObjectID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.ActivityDetail)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.ActivityDetailId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_ActivityActivityDetail");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserActivity");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskActivity");
            });

            modelBuilder.Entity<ActivityDetail>(entity =>
            {
                entity.Property(e => e.ActivityDetailId).HasColumnName("ActivityDetailID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasIndex(e => e.ProjectCode)
                    .IsUnique();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.Property(e => e.TaskId).HasColumnName("TaskID");
                entity.HasKey(e => e.TaskId);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.ListId).HasColumnName("ListID");

                entity.Property(e => e.TaskPriorityId).HasColumnName("TaskPriorityID");

                entity.Property(e => e.TaskStatusId).HasColumnName("TaskStatusID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.List)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_ListTask");

                entity.HasOne(d => d.TaskPriority)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.TaskPriorityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskPriorityTask");

                entity.HasOne(d => d.TaskStatus)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.TaskStatusId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskStatusTask");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserTask");
            });

            modelBuilder.Entity<TaskLabel>(entity =>
            {
                entity.Property(e => e.TaskLabelId).HasColumnName("TaskLabelID");

                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LabelId).HasColumnName("LabelID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskLabel)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Task_TaskLabel");

                entity.HasOne(d => d.Label)
                    .WithMany(p => p.TaskLabel)
                    .HasForeignKey(d => d.LabelId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Label_TaskLabel");
            });

            modelBuilder.Entity<ProjectMember>(entity =>
            {
                entity.Property(e => e.ProjectMemberId).HasColumnName("ProjectMemberID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectMembers)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Project_ProjectMember");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProjectMembers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_User_ProjectMember");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.AttachFile)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.Property(e => e.Content).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskComment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserComment");
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.Property(e => e.LabelId).HasColumnName("LabelID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Label)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_ProjectLabel");
            });

            modelBuilder.Entity<Lists>(entity =>
            {
                entity.Property(e => e.ListId).HasColumnName("ListID");
                entity.HasKey(e => e.ListId);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.List)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_ProjectList");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Property(e => e.UserNotificationId).HasColumnName("UserNotificationID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.UserNotification)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Notification_UserNotification");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotification)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_User_UserNotification");
            });

            modelBuilder.Entity<Checklist>(entity =>
            {
                entity.Property(e => e.ChecklistId).HasColumnName("ChecklistID");

                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Checklist)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskChecklist");
            });

            modelBuilder.Entity<TaskPriority>(entity =>
            {
                entity.Property(e => e.TaskPriorityId).HasColumnName("TaskPriorityID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<TaskStatuses>(entity =>
            {
                entity.Property(e => e.TaskStatusId).HasColumnName("TaskStatusID");
                entity.HasKey(e => e.TaskStatusId);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<Dependency>(entity =>
            {
                entity.Property(e => e.DependencyId).HasColumnName("DependencyID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<TaskDependency>(entity =>
            {
                entity.Property(e => e.TaskDependencyId).HasColumnName("TaskDependencyID");

                entity.Property(e => e.DependencyId).HasColumnName("DependencyID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Dependency)
                    .WithMany(p => p.TaskDependencies)
                    .HasForeignKey(d => d.DependencyId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskDependency");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskDependencies)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskTaskDependency");
            });

            modelBuilder.Entity<FileAttachment>(entity =>
            {
                entity.Property(e => e.FileAttachmentId).HasColumnName("FileAttachmentID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.FileAttachments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TaskFileAttachment");
            });

            modelBuilder.Entity<FbUserToken>(entity =>
            {
                entity.Property(e => e.FbUserTokenId).HasColumnName("FbUserTokenID");

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FbUserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserFbUserToken");
            });
        }

    }
}
