﻿using Microsoft.EntityFrameworkCore;

namespace ASP.Data;

public class DataContext : DbContext
{
    public DbSet<Entities.UserData> UsersData { get; private set; }
    public DbSet<Entities.UserRole> UserRoles { get; private set; }
    public DbSet<Entities.UserAccess> UserAccesses { get; private set; }
    
    public DbSet<Entities.Category> Categories { get; private set; }
    
    public DataContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ASP");

        modelBuilder.Entity<Entities.UserAccess>()
            .HasIndex(a => a.Login)
            .IsUnique();
        
        modelBuilder.Entity<Entities.UserAccess>()
            .HasOne(ua => ua.UserData)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .HasPrincipalKey(u => u.Id);

        modelBuilder.Entity<Entities.UserAccess>()
            .HasOne(ua => ua.UserRole)
            .WithMany()
            .HasForeignKey(ua => ua.RoleId); // если имена полей стандартные (Id), то можно не указывать
                                                      // .HasPrincipalKey(r => r.Id);

      modelBuilder.Entity<Entities.Category>()
          .HasIndex(c => c.Slug)
          .IsUnique();
        
        modelBuilder.Entity<Entities.UserRole>().HasData(
            new Entities.UserRole()
            {
                Id = "guest", Description = "solely registered user", CanCreate = 0, CanRead = 0, CanUpdate = 0,
                CanDelete = 0
            },
            new Entities.UserRole()
            {
                Id = "editor", Description = "has authority to edit content", CanCreate = 0, CanRead = 1, CanUpdate = 1,
                CanDelete = 0
            },
            new Entities.UserRole()
                { Id = "admin", Description = "admin of DB", CanCreate = 1, CanRead = 1, CanUpdate = 1, CanDelete = 1 },
            new Entities.UserRole()
            {
                Id = "moderator", Description = "has authority to block", CanCreate = 0, CanRead = 1, CanUpdate = 0,
                CanDelete = 1
            });
    }
}

