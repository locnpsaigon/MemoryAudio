using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Context
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DisplayStatus> DisplayStatuses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsStatus> NewsStatuses { get; set; }
        public DbSet<NewsType> NewsTypes { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        public DBContext() : base("name=MemoryAudioConnectionString")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(r => r.Roles)
                .WithMany(u => u.Users)
                .Map(m => {
                    m.ToTable("UserRoles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<DisplayStatus>().ToTable("DisplayStatuses");
            modelBuilder.Entity<Brand>().ToTable("Brands");
            modelBuilder.Entity<News>().ToTable("News");
            modelBuilder.Entity<NewsStatus>().ToTable("NewsStatuses");
            modelBuilder.Entity<NewsType>().ToTable("NewsTypes");
            modelBuilder.Entity<Statistic>().ToTable("Statistics");
            base.OnModelCreating(modelBuilder);
        }
    }
}