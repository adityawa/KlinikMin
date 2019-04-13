using Klinik.Data;
using Klinik.Entities.MasterData;
using Klinik.Features;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Klinik.Common;
using System.Net;
using Klinik.Data.DataRepository;

namespace Klinik.WebApi.Controllers
{
    /// <summary>
    /// Employee controller
    /// </summary>
    public class EmployeeController : ApiController
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public EmployeeController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Get employee list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Get employee by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="model"></param>
        public HttpResponseMessage Post([FromBody]EmployeeModel model)
        {
            var request = new EmployeeRequest
            {
                Data = model
            };

            EmployeeResponse _response = new EmployeeValidator(_unitOfWork, _context).Validate(request, true);
            if (_response.Status == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, _response.Message);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, _response.Message);
            }
        }

        /// <summary>
        /// Delete certain employee
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
        }
    }
}
