

using Shopping.Models;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Data{
    public class DataContextEF : DbContext{
        private readonly IConfiguration _config;
        public DataContextEF(IConfiguration config){
            _config = config;
        }
        public virtual DbSet<User>? Users {get;set;}
        public virtual DbSet<Auth>? Auth {get;set;}



        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured){
                optionsBuilder.UseSqlServer(_config.GetConnectionString("Default"),
                optionsBuilder => optionsBuilder.EnableRetryOnFailure());

            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SHOPPING");
            modelBuilder.Entity<User>()
                .ToTable("Users", "SHOPPING")
                .HasKey(u => u.UserId);
                
            modelBuilder.Entity<Auth>()
                .ToTable("Auth", "SHOPPING")
                .HasKey(u => u.Email);

            }

    }
}