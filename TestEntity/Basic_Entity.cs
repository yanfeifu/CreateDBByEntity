using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestEntity
{
    public class Basic_Entity
    {
        [Key]
        [Display(Name = "ID")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; }

        [Display(Name = "CreateTime")]
        [MaxLength(200)]
        [Column(TypeName = "datetime")]
        [Editable(true)]
        public DateTime CreateTime { get; set; }

        [Display(Name = "UpdateTime")]
        [MaxLength(200)]
        [Column(TypeName = "datetime")]
        [Editable(true)]
        public DateTime UpdateTime { get; set; }

        [Display(Name = "DeleteTime")]
        [MaxLength(200)]
        [Column(TypeName = "datetime")]
        [Editable(true)]
        public DateTime DeleteTime { get; set; }
    }
}
