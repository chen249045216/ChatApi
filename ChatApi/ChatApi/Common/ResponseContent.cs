using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Common
{
    public class ResponseContent
    {
        public bool Status { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        //public string Message { get; set; }
        public object Data { get; set; }

        public ResponseContent OK()
        {
            this.Status = true;
            this.Code = (int)ResponseType.Success;
            return this;
        }
        public ResponseContent OK(string message = null)
        {
            this.Status = true;
            this.Msg = message;
            this.Code = (int)ResponseType.Success;
            return this;
        }
        public ResponseContent OK(string message = null, object data = null)
        {
            this.Status = true;
            this.Msg = message;
            this.Data = data;
            this.Code = (int)ResponseType.Success;
            return this;
        }
        
        public ResponseContent OK(object data)
        {
            this.Status = true;
            //this.Msg = "成功";
            this.Data = data;
            this.Code = (int)ResponseType.Success;
            return this;
        }
        public ResponseContent Error(string message = null, ResponseType type = ResponseType.ServerError)
        {
            this.Status = false;
            this.Code = (int)type;
            this.Msg = message;
            return this;
        }
        public ResponseContent Set(ResponseType responseType)
        {
            bool? b = null;
            return this.Set(responseType, b);
        }
        public ResponseContent Set(ResponseType responseType, bool? status)
        {
            return this.Set(responseType, null, status);
        }
        public ResponseContent Set(ResponseType responseType, string msg)
        {
            bool? b = null;
            return this.Set(responseType, msg, b);
        }
        public ResponseContent Set(ResponseType responseType, string msg, bool? status)
        {
            if (status != null)
            {
                this.Status = (bool)status;
            }
            this.Code = (int)ResponseType.Success;
            if (!string.IsNullOrEmpty(msg))
            {
                Msg = msg;
                return this;
            }
            Msg = responseType.ToString();
            return this;
        }

        public static ResponseContent Instance
        {
            get { return new ResponseContent(); }
        }
    }
    public enum ResponseType
    {
        Success = 0,
        Error = 1,
        ServerError = 2,
        LoginExpiration = 302,
        ParametersLack = 303,
        TokenExpiration,
        Other
    }
}
