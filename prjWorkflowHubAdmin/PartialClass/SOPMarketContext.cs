using Microsoft.EntityFrameworkCore;

namespace prjWorkflowHubAdmin.ContextModels
{
    public partial class SOPMarketContext : DbContext
    {
        public SOPMarketContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration Config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(Config.GetConnectionString("SOPMarket"));
            }
        }
    }
}
