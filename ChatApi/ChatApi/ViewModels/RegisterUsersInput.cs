using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class RegisterUsersInput
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [MaxLength(12, ErrorMessage = "用户名最多12位")]
        [MinLength(3, ErrorMessage = "用户名最少3位")]
        public string username { get; set; }
        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(5, ErrorMessage = "密码最少5位")]
        [MaxLength(12, ErrorMessage = "密码最多12位")]
        public string password { get; set; }
    }
}
