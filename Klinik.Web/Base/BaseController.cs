using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Features;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Klinik.Web
{
    public abstract class BaseController : Controller
    {
        #region ::Properties::
        public IUnitOfWork _unitOfWork;
        public klinikEntities _context;

        protected AccountModel Account
        {
            get
            {
                return Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"];
            }
        }

        protected HttpStatusCodeResult BadRequestResponse
        {
            get
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public BaseController(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        protected long GetClinicID()
        {
            if (Session["UserLogon"] != null)
            {
                AccountModel account = (AccountModel)Session["UserLogon"];
                return account.ClinicID;
            }

            return -1;
        }

        protected bool IsHaveAuthorization(string privilege_name)
        {
            AccountModel account = (AccountModel)Session["UserLogon"];

            var privilegeNameList = _unitOfWork.PrivilegeRepository.Get(x => account.Privileges.PrivilegeIDs.Contains(x.ID));

            bool isAuthorized = privilegeNameList.Any(x => x.Privilege_Name == privilege_name);

            return isAuthorized;
        }

        protected int GetUserPoliID()
        {
            AccountModel account = (AccountModel)Session["UserLogon"];
            var privilegeNameList = _unitOfWork.PrivilegeRepository.Get(x => account.Privileges.PrivilegeIDs.Contains(x.ID));

            if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION")) { return 1; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_UMUM")) { return 2; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_GIGI")) { return 3; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_INTERNIS")) { return 4; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_KULIT")) { return 5; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_MATA")) { return 6; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_THT")) { return 7; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_ANAK")) { return 8; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_SYARAF")) { return 9; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_RADIOLOGI")) { return 10; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_LABORATORIUM")) { return 11; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_FARMASI")) { return 12; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_REKAMMEDIS")) { return 13; }
            else if (privilegeNameList.Any(x => x.Privilege_Name == "VIEW_REGISTRATION_KASIR")) { return 14; }
            else { return 1; }
        }

        #region ::Dropdown Methods::
        protected List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _clinicLists = new List<SelectListItem>();
            foreach (var item in new ClinicHandler(_unitOfWork).GetAllClinic())
            {
                _clinicLists.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _clinicLists;
        }

        protected List<SelectListItem> BindDropDownOrganization()
        {
            List<SelectListItem> _organizationLists = new List<SelectListItem>();
            foreach (var item in new OrganizationHandler(_unitOfWork).GetOrganizationList())
            {
                _organizationLists.Add(new SelectListItem
                {
                    Text = item.OrgName,
                    Value = item.Id.ToString()
                });
            }

            return _organizationLists;
        }

        protected List<SelectListItem> BindDropDownEmployee()
        {
            List<SelectListItem> _employeeLists = new List<SelectListItem>();
            foreach (var item in new EmployeeHandler(_unitOfWork).GetAllEmployee())
            {
                _employeeLists.Add(new SelectListItem
                {
                    Text = item.EmpName,
                    Value = item.Id.ToString()
                });
            }

            return _employeeLists;
        }

        protected List<SelectListItem> BindDropDownDoctorType()
        {
            return GetGeneralMasterByType(Constants.MasterType.DOCTOR);
        }

        protected List<SelectListItem> BindDropDownParamedicType()
        {
            return GetGeneralMasterByType(Constants.MasterType.PARAMEDIC);
        }

        protected List<SelectListItem> BindDropDownCityType()
        {
            return GetGeneralMasterByType(Constants.MasterType.CITY);
        }

        protected List<SelectListItem> BindDropDownClinicType()
        {
            return GetGeneralMasterByType(Constants.MasterType.CLINIC);
        }

        protected List<SelectListItem> BindDropDownDayType()
        {
            return GetGeneralMasterByType(Constants.MasterType.DAY);
        }

        protected List<SelectListItem> BindDropDownDepartmentType()
        {
            return GetGeneralMasterByType(Constants.MasterType.DEPARTMENT);
        }

        protected List<SelectListItem> BindDropDownEmploymentType()
        {
            return GetGeneralMasterByType(Constants.MasterType.EMPLOYMENT);
        }

        protected List<SelectListItem> GetGeneralMasterByType(string type)
        {
            List<SelectListItem> _dataList = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(type).ToList())
            {
                _dataList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _dataList;
        }
        #endregion
    }
}