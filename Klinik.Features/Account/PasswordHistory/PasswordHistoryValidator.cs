using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class PasswordHistoryValidator : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public PasswordHistoryValidator(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PasswordHistoryResponse Validate(PasswordHistoryRequest request)
        {
            PasswordHistoryResponse response = new PasswordHistoryResponse();

            if (String.IsNullOrEmpty(request.Data.UserName) || String.IsNullOrWhiteSpace(request.Data.UserName))
                errorFields.Add("User Name");
            if (String.IsNullOrEmpty(request.Data.Password) || String.IsNullOrWhiteSpace(request.Data.Password))
                errorFields.Add("Password");
            if (String.IsNullOrEmpty(request.Data.NewPassword) || String.IsNullOrWhiteSpace(request.Data.NewPassword))
                errorFields.Add("New Password");

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.RequiredFieldsMissing, String.Join(",", errorFields));
            }

            var cekIsExpired = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.Data.UserName);
            if (cekIsExpired == null)
            {
                response.Status = false;
                response.Message = Messages.UserNotFound;
            }
            else
            {
                if (cekIsExpired.Status == false || cekIsExpired.ExpiredDate < DateTime.Now)
                {
                    response.Status = false;
                    response.Message = Messages.CannotChangePasswordForInactiveUser;
                }
            }

            var validateCurrentUser = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.Data.UserName);
            if (validateCurrentUser != null)
            {
                if (request.Data.Password != CommonUtils.Decryptor(validateCurrentUser.Password, CommonUtils.KeyEncryptor))
                {
                    response.Status = false;
                    response.Message = Messages.UpdateFailedUsernamePasswordNotMatch;
                }
            }

            var IsExistPassinHist = _unitOfWork.PasswordHistoryRepository.Get(x => x.OrganizationID == request.Data.OrganizationID && x.UserName == request.Data.UserName).Select(x => x.Password);
            foreach (string p in IsExistPassinHist)
            {
                if (request.Data.NewPassword == CommonUtils.Decryptor(p, CommonUtils.KeyEncryptor))
                {
                    response.Status = false;
                    response.Message = Messages.PasswordHasBeenUsedBefore;
                    break;
                }
            }

            if (request.Data.Password.Equals(request.Data.NewPassword))
            {
                response.Status = false;
                response.Message = Messages.PasswordHasBeenUsedBefore;
            }

            if (response.Status)
            {
                response = new PasswordHistoryHandler(_unitOfWork, _context).ChangePassword(request);
            }

            return response;
        }
    }
}