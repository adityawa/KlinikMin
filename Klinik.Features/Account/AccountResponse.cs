using Klinik.Entities;
using Klinik.Entities.Account;

namespace Klinik.Features
{
    public class AccountResponse : BaseResponse<AccountModel>
    {
        public AccountResponse()
        {
            Entity = new AccountModel();
        }
    }
}