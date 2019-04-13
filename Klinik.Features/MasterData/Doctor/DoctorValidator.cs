using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Klinik.Features
{
    public class DoctorValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_DOCTOR";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_DOCTOR";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_DOCTOR";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public DoctorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DoctorValidator(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public DoctorResponse Validate(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                response = ValidateForDelete(request);
            }
            else
            {
                bool isHavePrivilege = true;

                if (String.IsNullOrEmpty(request.Data.Code) || String.IsNullOrWhiteSpace(request.Data.Code))
                {
                    request.Data.Code = "D" + new Random().Next(1000, 9999);
                }

                if (request.Data.STRValidFrom != null && request.Data.STRValidTo != null)
                {
                    if (request.Data.STRValidTo < request.Data.STRValidFrom)
                    {
                        errorFields.Add(Messages.STRValidToInvalid);
                    }
                }

                if (String.IsNullOrEmpty(request.Data.Name) || String.IsNullOrWhiteSpace(request.Data.Name))
                {
                    errorFields.Add("Doctor Name");
                }

                if (!String.IsNullOrEmpty(request.Data.Email))
                {
                    if (!Regex.IsMatch(request.Data.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}"))
                        errorFields.Add("Email");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {
                    string privilegeName = request.Data.TypeID == 0 ? ADD_PRIVILEGE_NAME : "ADD_M_PARAMEDIC";

                    isHavePrivilege = IsHaveAuthorization(privilegeName, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    string privilegeName = request.Data.TypeID == 0 ? EDIT_PRIVILEGE_NAME : "EDIT_M_PARAMEDIC";

                    isHavePrivilege = IsHaveAuthorization(privilegeName, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new DoctorHandler(_unitOfWork, _context).CreateOrEdit(request);
                }
            }

            return response;
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private DoctorResponse ValidateForDelete(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();
            string privilegeName = request.Data.TypeID == 0 ? DELETE_PRIVILEGE_NAME : "DELETE_M_PARAMEDIC";
            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(privilegeName, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new DoctorHandler(_unitOfWork).RemoveData(request, request.Data.TypeID == 0);
            }

            return response;
        }
    }
}
