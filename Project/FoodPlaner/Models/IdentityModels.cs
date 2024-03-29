﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodPlaner.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? Age { get; set; }
        public int? Weight { get; set; }
        public float? BodyIndex { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
           FoodPlaner.Migrations.Configuration>("DefaultConnection"));
        }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Review>().HasRequired(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId).WillCascadeOnDelete(false);
            //modelBuilder.Entity<Recipe>().HasRequired(r => r.User).WithMany(u => u.Recipes).HasForeignKey(r => r.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Review>().HasRequired(r => r.Recipe).WithMany(r => r.Reviews).HasForeignKey(r => r.RecipeId).WillCascadeOnDelete(false);
        }
    }
}