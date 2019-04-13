using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MappingMaster;
using Klinik.Resources;
using System;
using System.Collections.Generic;

namespace Klinik.Features
{
    /// <summary>
    /// Account handler class
    /// </summary>
    public class AccountHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public AccountHandler(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Clinic enum
        /// </summary>
        public string ClinicEnum { get; private set; }

        /// <summary>
        /// Authenticate user based on request values
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse AuthenticateUser(AccountRequest request)
        {
            AccountResponse response = new AccountResponse();

            //get Org ID
            var _getOrganization = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == request.Data.Organization);
            if (_getOrganization != null)
            {
                var _getByUname = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.Data.UserName && x.Status == true && x.ExpiredDate > DateTime.Now && x.OrganizationID == _getOrganization.ID);
                if (_getByUname != null)
                {
                    var _decryptedPassword = CommonUtils.Decryptor(_getByUname.Password, CommonUtils.KeyEncryptor);
                    if (_decryptedPassword == request.Data.Password)
                    {
                        response.Entity.UserName = _getByUname.UserName;
                        response.Entity.UserID = _getByUname.ID;
                        response.Entity.EmployeeID = _getByUname.EmployeeID ?? 0;
                        response.Entity.Organization = _getOrganization.OrgCode;
                        response.Entity.ClinicID = _getOrganization.KlinikID ?? 0;
                        var _getRoles = _unitOfWork.UserRoleRepository.Get(x => x.UserID == response.Entity.UserID);

                        foreach (var role in _getRoles)
                        {
                            response.Entity.Roles.Add(role.RoleID);
                        }

                        var _getRolePrivileges = _unitOfWork.RolePrivRepository.Get(x => response.Entity.Roles.Contains(x.RoleID));

                        foreach (var rp in _getRolePrivileges)
                        {
                            response.Entity.Privileges.PrivilegeIDs.Add(rp.PrivilegeID);
                        }

                        CommandLog(true, ClinicEnums.Module.LOGIN, Constants.Command.LOGIN_TO_SYSTEM, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = Messages.InvalidPassword;

                        CommandLog(false, ClinicEnums.Module.LOGIN, Constants.Command.LOGIN_TO_SYSTEM, request.Data);
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = Messages.InvalidUsernamePassword;

                    CommandLog(false, ClinicEnums.Module.LOGIN, Constants.Command.LOGIN_TO_SYSTEM, request.Data);
                }
            }
            else
            {
                response.Status = false;
                response.Message = Messages.InvalidOrganizationCode;

                CommandLog(false, ClinicEnums.Module.LOGIN, Constants.Command.LOGIN_TO_SYSTEM, request.Data);
            }

            return response;
        }

        /// <summary>
        /// Set user reset password code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse SetResetPasswordCode(AccountRequest request)
        {
            // init response
            AccountResponse response = new AccountResponse();

            // get employee by its email
            var employee = _unitOfWork.EmployeeRepository.GetFirstOrDefault(x => x.Email == request.Data.Email);
            if (employee != null)
            {
                try
                {
                    // get user based on employee ID
                    var user = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.EmployeeID == employee.ID);

                    // set reset password code
                    user.ResetPasswordCode = request.Data.ResetPasswordCode;

                    // update user                    
                    _unitOfWork.UserRepository.Update(user);

                    // save it
                    int resultAffected = _unitOfWork.Save();

                    // update response                    
                    response.Message = Messages.DataUpdated;
                }
                catch
                {
                    response.Status = false;
                    response.Message = Messages.GeneralError;
                }
            }
            else
            {
                response.Status = false;
                response.Message = Messages.InvalidUsernamePassword;
            }

            return response;
        }

        /// <summary>
        /// Validate user reset password code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse ValidateResetPasswordCode(AccountRequest request)
        {
            // init response
            AccountResponse response = new AccountResponse();

            // check if user with passed reset password code exist
            var user = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.ResetPasswordCode == request.Data.ResetPasswordCode);
            if (user == null)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }

            return response;
        }

        /// <summary>
        /// Update user password 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse UpdateUserPassword(AccountRequest request)
        {
            // init response
            AccountResponse response = new AccountResponse();

            // get user by its reset password code
            var user = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.ResetPasswordCode == request.Data.ResetPasswordCode);
            if (user != null)
            {
                try
                {
                    // update password
                    user.Password = CommonUtils.Encryptor(request.Data.Password, CommonUtils.KeyEncryptor);

                    // clear the resert password code
                    user.ResetPasswordCode = null;

                    // update user
                    _unitOfWork.UserRepository.Update(user);

                    // save it
                    int resultAffected = _unitOfWork.Save();

                    // update response                    
                    response.Message = Messages.UserPasswordUpdated;
                }
                catch
                {
                    response.Status = false;
                    response.Message = Messages.GeneralError;
                }
            }
            else
            {
                response.Status = false;
                response.Message = Messages.InvalidResetPasswordCode;
            }

            return response;
        }
    }
}