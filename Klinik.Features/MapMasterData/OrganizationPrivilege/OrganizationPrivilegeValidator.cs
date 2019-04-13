using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class OrganizationPrivilegeValidator : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public OrganizationPrivilegeValidator(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(OrganizationPrivilegeRequest request, out OrganizationPrivilegeResponse response)
        {
            response = new OrganizationPrivilegeResponse();

            if (request.Data.OrgID == 0)
            {
                errorFields.Add("Organization");
            }

            if (request.Data.PrivilegeIDs.Count == 0)
            {
                errorFields.Add("Privileges");
            }

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }

            if (response.Status)
            {
                response = new OrganizationPrivilegeHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}