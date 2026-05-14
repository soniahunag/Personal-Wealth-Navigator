using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWN_Backend.Models.Entites
{
    [Table("MarketDataCache", Schema = "dbo")]
    public class MarketDataCache
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Symbol { get; set; } = "0050.TW"; // 預設 0050

        [Required]
        public DateTime DataDate { get; set; } // 收盤日期

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdjClose { get; set; } // 調整後收盤價（含息）

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
