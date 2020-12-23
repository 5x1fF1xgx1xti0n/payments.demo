using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payments.API
{
    public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
    {
        public PaymentsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-T76MGRB;Database=Payments;Trusted_Connection=True;");

            return new PaymentsDbContext(optionsBuilder.Options);
        }
    }
}
