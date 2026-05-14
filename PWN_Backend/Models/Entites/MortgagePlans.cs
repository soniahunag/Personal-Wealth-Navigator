using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWN_Backend.Models.Entites
{
    [Table("MortgagePlans", Schema = "dbo")]
    public class MortgagePlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PlanName { get; set; } = "預設房貸方案";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalLoanAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingPrincipal { get; set; }

        [Column(TypeName = "decimal(5,4)")] // 例如 0.0230 代表 2.3%
        public decimal InterestRate { get; set; }

        public int LoanTermYears { get; set; }

        public DateTime StartDate { get; set; }

        public int GracePeriodMonths { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 關聯 User
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
