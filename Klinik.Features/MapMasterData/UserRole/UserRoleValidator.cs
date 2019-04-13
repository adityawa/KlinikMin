using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class UserRoleValidator : BaseFeatures
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public UserRoleValidator(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(UserRoleRequest request, out UserRoleResponse response)
        {
            response = new UserRoleResponse();

            if (request.Data.UserID == 0)
            {
                errorFields.Add("User Name");
            }
            if (request.Data.RoleIds.Count == 0)
            {
                errorFields.Add("Role");
            }

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }

            if (response.Status)
            {
                response = new UserRoleHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}