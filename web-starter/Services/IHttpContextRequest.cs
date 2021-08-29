using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public interface IHttpContextRequest
    {
        string Body { get; set; }
        string Path { get; set; }
    }
}
