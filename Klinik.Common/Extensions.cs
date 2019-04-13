using System;

namespace Klinik.Common
{
    public static class Extensions
    {
        public static string GetAllMessages(this Exception ex, string separator = "\r\nInnerException: ")
        {
            if (ex.InnerException == null)
                return ex.Message;

            return ex.Message + separator + GetAllMessages(ex.InnerException, separator);
        }
    }
}
