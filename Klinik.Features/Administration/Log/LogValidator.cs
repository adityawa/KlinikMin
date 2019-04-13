using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Klinik.Features
{
    public class LogValidator : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LogValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public LogValidator(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public LogResponse Validate(LogRequest request)
        {
            LogResponse response = new LogResponse();
            return response;
        }
    }
}
