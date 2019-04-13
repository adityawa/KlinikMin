using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;
using Klinik.Features.MasterData.Poli;

namespace Klinik.Features
{
    public class PoliValidator : BaseFeatures
    {
        private const string ADD_POLI_NAME = "ADD_M_POLI";
        private const string EDIT_POLI_NAME = "EDIT_M_POLI";
        private const string DELETE_POLI_NAME = "DELETE_M_POLI";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PoliValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(PoliRequest request, out PoliResponse response)
        {
            response = new PoliResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.Name == null || String.IsNullOrWhiteSpace(request.Data.Name))
                {
                    errorFields.Add("Poli Name");
                }

                if (request.Data.Type == 0)
                {
                    errorFields.Add("Type");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }
                else if (request.Data.Name.Length > 30)
                {
                    response.Status = false;
                    response.Message = $"Maximum Character for Poli Name is 30";
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_POLI_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_POLI_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new PoliHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(PoliRequest request, out PoliResponse response)
        {
            response = new PoliResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_POLI_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PoliHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
