using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Ats.Data.Models
{
    public partial class AtsContext : DbContext
    {
        public AtsContext()
        {
        }

        public AtsContext(DbContextOptions<AtsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<AnnouncementDefinition> AnnouncementDefinitions { get; set; }
        public virtual DbSet<AnnouncementType> AnnouncementTypes { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.\\;Database=Ats;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(e => e.PkId);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Announcements)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Announcements_AnnouncementTypes");
            });

            modelBuilder.Entity<AnnouncementDefinition>(entity =>
            {
                entity.HasKey(e => e.PkId)
                    .HasName("PK_Pages");

                entity.Property(e => e.ClickCssSelector)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.RowCssSelector)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.AnnouncementDefinitions)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnouncementDefinitions_AnnouncementTypes");
            });

            modelBuilder.Entity<AnnouncementType>(entity =>
            {
                entity.HasKey(e => e.PkId);

                entity.Property(e => e.PkId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.HasKey(e => e.PkId);

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.HasKey(e => e.PkId);

                entity.Property(e => e.ExceptionDateTime).HasColumnType("datetime");

                entity.Property(e => e.InnerException).HasMaxLength(4000);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.SpecialMessage).HasMaxLength(4000);

                entity.Property(e => e.StackTrace)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}