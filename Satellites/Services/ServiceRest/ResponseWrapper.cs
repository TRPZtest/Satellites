using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Satellites.Services.ServiceRest
{
    public class ResponseWrapper<T>
    {        
        public T ResponseDto { get; set; }
        public bool IsSuccesStatusCode { get; set;  }
        public HttpStatusCode StatusCode { get; set; }
    }
}
