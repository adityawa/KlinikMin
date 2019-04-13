using System;

namespace Klinik.Entities.Account
{
    public class PasswordHistoryModel : AccountModel
    {
        public long OrganizationID { get; set; }
        public String NewPassword { get; set; }
    }
}