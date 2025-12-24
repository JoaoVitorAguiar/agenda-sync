using AgendaSync.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaSync.Data;

public class AgendaSyncDbContext : DbContext
{
    public AgendaSyncDbContext(DbContextOptions<AgendaSyncDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");

            entity.Property(u => u.ExternalSubject)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("external_subject");

            entity.Property(u => u.ExternalRefreshToken)
                .HasColumnName("external_refresh_token")
                .HasColumnType("text");

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasIndex(u => u.ExternalSubject)
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
