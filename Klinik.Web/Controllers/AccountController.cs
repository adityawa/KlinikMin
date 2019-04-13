using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Features;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private IUnitOfWork _unitOfWork;
        private klinikEntities _context;

        public AccountController(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            // generate reset code
            string resetCode = Guid.NewGuid().ToString();

            // define request
            AccountRequest request = new AccountRequest
            {
                Data = new AccountModel
                {
                    Email = EmailID,
                    ResetPasswordCode = resetCode
                }
            };

            // set reset password code
            var response = new AccountHandler(_unitOfWork, _context).SetResetPasswordCode(request);
            if (response.Status)
            {
                // send verification email
                SendVerificationLinkEmail(EmailID, resetCode);

                ViewBag.Message = Messages.ResetPasswordLinkSent;
            }
            else
            {
                ViewBag.Message = string.Format(Messages.EmployeeWithEmailNotExist, EmailID);
            }

            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            // validate
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Login", "Account");
            }

            // define request
            AccountRequest request = new AccountRequest
            {
                Data = new AccountModel
                {
                    ResetPasswordCode = id
                }
            };

            // validate reset password code
            var response = new AccountHandler(_unitOfWork, _context).ValidateResetPasswordCode(request);
            if (response.Status)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                // define request
                AccountRequest request = new AccountRequest
                {
                    Data = new AccountModel
                    {
                        Password = model.NewPassword,
                        ResetPasswordCode = model.ResetCode
                    }
                };

                // update user password 
                var response = new AccountHandler(_unitOfWork, _context).UpdateUserPassword(request);
                if (response.Status)
                {
                    ViewBag.Message = Messages.UserPasswordUpdated;
                }
                else
                {
                    ViewBag.Message = Messages.UserPasswordUpdateFailed;
                }

                ViewBag.Status = response.Status.ToString();
                ViewBag.Message = message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SignIn(AccountModel _model)
        {
            AccountRequest request = new AccountRequest
            {
                Data = new AccountModel
                {
                    UserName = _model.UserName,
                    Password = _model.Password,
                    Organization = _model.Organization
                }
            };

            AccountResponse response = new AccountResponse();
            new AccountValidator(_unitOfWork).Validate(request, out response);
            if (response.Status)
            {
                Session["UserLogon"] = response.Entity;

                if (response.Entity.Privileges.PrivilegeIDs != null)
                {
                    IList<MenuModel> Menu = new MenuHandler(_unitOfWork).GetMenuBasedOnPrivilege(response.Entity.Privileges.PrivilegeIDs);
                    Session["AuthMenu"] = Menu;
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Status = response.Status.ToString();
                ViewBag.Message = response.Message.ToString();
                return View("Login");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        public void remove_Anonymous_Cookies()
        {
            try
            {
                if (Request.Cookies["WebTime"] != null)
                {
                    var option = new HttpCookie("WebTime");
                    option.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(option);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string resetCode)
        {
            try
            {
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/Account/ResetPassword/" + resetCode);

                var emailSender = System.Configuration.ConfigurationManager.AppSettings["SmtpSender"].ToString();
                var emailSenderPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpSenderPassword"].ToString();
                var emailSenderDisplayName = System.Configuration.ConfigurationManager.AppSettings["SmtpSenderDisplay"].ToString();
                var smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"].ToString();
                var smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"].ToString());

                var fromEmail = new MailAddress(emailSender, emailSenderDisplayName);
                var toEmail = new MailAddress(emailID);

                string body = string.Format(Messages.EmailBodyResetPassword, link);

                var smtp = new SmtpClient
                {
                    Host = smtpHost,
                    Port = smtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, emailSenderPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = Messages.ResetPassword,
                    Body = body,
                    IsBodyHtml = true
                })

                    smtp.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}