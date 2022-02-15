using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestEntity
{
    public class Test_Table : Basic_Entity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        [MaxLength(20)]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int RoleId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Display(Name = "Token")]
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        [Editable(true)]
        public string Token { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        [Display(Name = "用户真实姓名")]
        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string RealName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [MaxLength(400)]
        [JsonIgnore]
        [Column(TypeName = "nvarchar(400)")]
        public string Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(22)]
        [Column(TypeName = "nvarchar(22)")]
        public string PhoneNo { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Display(Name = "是否可用")]
        [Column(TypeName = "tinyint")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public byte Enable { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        [Editable(true)]
        public string Email { get; set; }
    }
}
