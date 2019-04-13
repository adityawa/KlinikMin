using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Features;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PasswordHistoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private klinikEntities _context;

        public PasswordHistoryController(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // GET: PasswordHistory
        public ActionResult ChangePassword()
        {
            if (Session["UserLogon"] != null)
            {
                AccountModel acc = (AccountModel)Session["UserLogon"];
                PasswordHistoryModel _model = new PasswordHistoryModel();
                _model.UserName = acc.UserName;
                _model.UserID = acc.UserID;
                return View(_model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult ChangeUserPassword(PasswordHistoryModel _model)
        {
            PasswordHistoryRequest request = new PasswordHistoryRequest
            {
                Data = new PasswordHistoryModel
                {
                    UserID = _model.UserID,
                    UserName = _model.UserName,
                    Password = _model.Password,
                    NewPassword = _model.NewPassword
                }
            };

            PasswordHistoryResponse response = new PasswordHistoryResponse();
            response = new PasswordHistoryValidator(_unitOfWork, _context).Validate(request);
            ViewBag.Response = $"{response.Status};{response.Message}";
            return View("ChangePassword");
        }
    }
}