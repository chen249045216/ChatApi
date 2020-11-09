using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Common
{
    public class MessageResult
    {
        public ResponseType Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }

        public static MessageResult Instance
        {
            get { return new MessageResult(); }
        }
        public MessageResult Ok(string msg, object data)
        {
            this.Code = ResponseType.Success;
            this.Msg = msg;
            this.Data = data;
            return this;
        }

        public MessageResult Error(string msg, object data)
        {
            this.Code = ResponseType.Error;
            this.Msg = msg;
            this.Data = data;
            return this;
        }
    }
}
