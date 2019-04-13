using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class OrganizationValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_ORG";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_ORG";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_ORG";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public OrganizationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(OrganizationRequest request, out OrganizationResponse response)
        {
            response = new OrganizationResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

               
                if (request.Data.OrgCode == null || request.Data.OrgCode.Equals(string.Empty))
                {
                    errorFields.Add("Organization Code");
                }

                if (request.Data.OrgName == null || request.Data.OrgName.Equals(string.Empty))
                {
                    errorFields.Add("Organization Name");
                }
                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {
                    var _cek = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == request.Data.OrgCode, includes: x => x.Clinic);
                    if (_cek != null)
                    {
                        if (_cek.ID > 0)
                        {
                            response.Status = false;
                            response.Message = $"Organization Code {request.Data.OrgCode} was exist. Please use another";
                        }
                    }
                }

                if (request.Data.Id == 0)
                {
                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new OrganizationHandler(_unitOfWork).CreateOrEditOrganization(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(OrganizationRequest request, out OrganizationResponse response)
        {
            response = new OrganizationResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new OrganizationHandler(_unitOfWork).RemoveOrganization(request);
            }
        }
    }
}