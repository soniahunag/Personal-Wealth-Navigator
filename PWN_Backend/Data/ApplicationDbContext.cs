using Microsoft.EntityFrameworkCore;
using PWN_Backend.Models.Entites;

namespace PWN_Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        //負責將你的 C# Model 映射到資料庫 Table
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // 定義 DbSet，這代表資料庫中的 Table
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<MortgagePlan> MortgagePlans { get; set; } = null!;
        public DbSet<MarketDataCache> MarketDataCaches { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 如果你的 Table 有特殊的複合主鍵或額外設定，寫在這裡
            // 這裡我們維持簡潔，因為大部分設定已在 Model 使用 Data Annotations 完成
        }

    }
}
