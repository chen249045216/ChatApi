using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class UploadImageInput
    {
        public string ToId { get; set; }
        public List<IFormFile> FormData { get; set; }
    }
}
