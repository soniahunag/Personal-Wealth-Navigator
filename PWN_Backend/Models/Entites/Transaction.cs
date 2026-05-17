using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWN_Backend.Models.Entites
{
    [Table("Transactions", Schema = "dbo")] // 強制對應到 dbo 綱要下的 Transactions 表
    public class Transaction
    {
        [Key] // 指定主鍵
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Type { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")] // 確保資料型別與 MSSQL 完全一致
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        public DateTime TxnDate { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        // 關聯屬性 (導覽屬性) - 這能讓 EF Core 自動幫你處理 Join 邏輯
        public string UserId { get; set; } = string.Empty;

        // 股票交易欄位
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string? Symbol { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Price { get; set; }
      

    }
}
