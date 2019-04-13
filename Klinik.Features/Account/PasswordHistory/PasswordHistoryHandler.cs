using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    /// <summary>
    /// Password history handler class
    /// </summary>
    public class PasswordHistoryHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public PasswordHistoryHandler(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PasswordHistoryResponse ChangePassword(PasswordHistoryRequest request)
        {
            PasswordHistoryResponse response = new PasswordHistoryResponse();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toBeUpdate = _context.Users.SingleOrDefault(x => x.ID == request.Data.UserID);
                    if (toBeUpdate != null)
                    {
                        toBeUpdate.Password = CommonUtils.Encryptor(request.Data.NewPassword, CommonUtils.KeyEncryptor);
                        _context.SaveChanges();
                    }

                    var _passHistoryEntity = new PasswordHistory
                    {
                        OrganizationID = request.Data.OrganizationID,
                        UserName = request.Data.UserName,
                        Password = CommonUtils.Encryptor(request.Data.Password, CommonUtils.KeyEncryptor),
                        CreatedBy = request.Data.UserName,
                        CreatedDate = DateTime.Now
                    };

                    _context.PasswordHistories.Add(_passHistoryEntity);

                    _context.SaveChanges();

                    transaction.Commit();

                    response.Message = Messages.UserPasswordUpdated;
                }
                catch
                {
                    transaction.Rollback();
                    response.Status = false;
                    response.Message = Messages.GeneralError;
                }
            }

            return response;
        }
    }
}