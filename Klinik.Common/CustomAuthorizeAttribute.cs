using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Klinik.Common
{
    /// <summary>
    /// Custom authorize attribute class
    /// </summary>
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _privilege_names;
        private klinikEntities _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="privilege_name"></param>
        public CustomAuthorizeAttribute(params string[] privilege_name)
        {
            this._privilege_names = privilege_name;

            _context = new klinikEntities();
        }

        /// <summary>
        /// Override the onAuthorization
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var account = (AccountModel)filterContext.HttpContext.Session["UserLogon"];
            if (account == null)
            {
                // redirect to login page if user not logged in  
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } });
            }
            else
            {
                bool IsAuthorized = false;
                List<long> PrivilegeIds = account.Privileges.PrivilegeIDs;
                var _getPrivilegeName = _context.Privileges.Where(x => PrivilegeIds.Contains(x.ID)).Select(x => x.Privilege_Name);

                var cek_authorizes = _getPrivilegeName.Where(p => _privilege_names.Contains(p.ToString()));
                if (cek_authorizes.Any())
                {
                    IsAuthorized = true;
                }

              

                if (!IsAuthorized)
                {
                    this.HandleUnauthorizedRequest(filterContext);
                }
            }
        }
        /// <summary>
        /// Override the Handle of unauthorized request
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Unauthorized" } });
        }
    }
}



