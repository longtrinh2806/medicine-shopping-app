using Customer.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customers>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Coupon>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Membership>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Address>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Customers>()
                .HasOne<Membership>(c => c.Membership)
                .WithOne(m => m.Customers)
                .HasForeignKey<Membership>(a => a.CustomerId);

            modelBuilder.Entity<Customers>()
                .HasMany(c => c.Coupon)
                .WithOne(c => c.Customers)
                .HasForeignKey(c => c.CustomerId);

            modelBuilder.Entity<Customers>()
                .HasOne(c => c.Address)
                .WithOne(a => a.Customers)
                .HasForeignKey<Address>(a => a.CustomerId);

            modelBuilder.Entity<Customers>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Customers>()
                .HasIndex(c => c.PhoneNumber)
                .IsUnique();
        }
    }
}
