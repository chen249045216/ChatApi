using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApi.Common
{
    [AuthorizeAttribute]
    public class ChatApiController : ControllerBase
    {
        ////[Route("Success")]
        [NonAction]
        public ResponseContent Success(string msg, object data = null)
        {
            return ResponseContent.Instance.OK(msg, data);
        }
        [NonAction]
        public ResponseContent Success(object data = null)
        {
            return ResponseContent.Instance.OK(data);
        }
        ////[Route("Error")]
        [NonAction]
        public ResponseContent Error(string msg, ResponseType type = ResponseType.Error)
        {
            return ResponseContent.Instance.Error(msg, type);
        }
        public string CurrentUserId
        {
            get
            {

                return (HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Jti)
                    ?? HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).ToString();

            }
        }
        [NonAction]
        protected void ValidateInput()
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage);
            }
        }

    }
}
