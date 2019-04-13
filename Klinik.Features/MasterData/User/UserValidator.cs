using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class UserValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_USER";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_USER";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_USER";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public UserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(UserRequest request, out UserResponse response)
        {
            response = new UserResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.OrgID == 0)
                {
                    errorFields.Add("Organization");
                }

                if (String.IsNullOrEmpty(request.Data.UserName) || String.IsNullOrWhiteSpace(request.Data.UserName))
                {
                    errorFields.Add("UserName");
                }

                if (String.IsNullOrEmpty(request.Data.Password) || String.IsNullOrWhiteSpace(request.Data.Password))
                {
                    errorFields.Add("Password");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }
                else if (request.Data.Id == 0)
                {
                    //validate is username exist
                    var qry = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName.Equals(request.Data.UserName) && x.Status == true, includes: x => x.Employee);
                    if (qry != null)
                    {
                        response.Status = false;
                        response.Message = Messages.UsernameAlreadyExist;
                    }
                }
                else if (request.Data.Id == 0)
                {
                    //validate is username exist
                    var qry = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName.Equals(request.Data.EmployeeID) && x.Status == true, includes: x => x.Employee);
                    if (qry != null)
                    {
                        response.Status = false;
                        response.Message = Messages.OneEmpOneUserID;
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
                    response = new UserHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(UserRequest request, out UserResponse response)
        {
            response = new UserResponse();

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
                response = new UserHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}