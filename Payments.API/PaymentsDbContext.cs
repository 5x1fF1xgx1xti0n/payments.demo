using Microsoft.EntityFrameworkCore;
using Payments.API.Models.Entities;

namespace Payments.API
{
    public class PaymentsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TotalCommission> TotalCommissions { get; set; }


        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasKey(e => e.Id);

            builder.Entity<Account>()
                .HasKey(e => e.Id);

            builder.Entity<Transaction>()
                .HasKey(e => e.Id);

            builder.Entity<TotalCommission>()
                .HasKey(e => e.Id);


            builder.Entity<User>()
                .HasOne(e => e.Account)
                .WithOne(e => e.User)
                .HasForeignKey<Account>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Account>()
                .HasOne(e => e.User)
                .WithOne(e => e.Account)
                .HasForeignKey<User>(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
