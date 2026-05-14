
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PWN_Backend.Models.Entites
{
    [Table("Users", Schema = "dbo")]
    public class User
    {
        [Key]
        [Required]
        [StringLength(255)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Pswd { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 導覽屬性 (Navigation Property)
        // 方便你之後透過 user.Transactions 直接取得該使用者的所有收支紀錄
        //當你執行 context.Users.Include(u => u.Transactions) 時，EF Core 會自動幫你做 Left Join，非常方便
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
