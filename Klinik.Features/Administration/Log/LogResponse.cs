using Klinik.Entities;
using Klinik.Entities.Administration;
using System.Collections.Generic;

namespace Klinik.Features
{
    public class LogResponse : BaseResponse<LogModel>
    {
        public LogResponse()
        {
            Data = new List<LogModel>();
        }
    }
}
