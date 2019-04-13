using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Features;
using Klinik.Features.MasterData.Clinic;

using Klinik.Features.MasterData.EmployeeStatus;
using Klinik.Features.MasterData.FamilyRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Klinik.Data.DataRepository;
namespace Klinik.Web.Controllers
{
    public class MasterDataController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private klinikEntities _context;

        public MasterDataController(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region ::MISC::
        private List<SelectListItem> BindDropDownRoleList(int orgId)
        {
            List<OrganizationRole> orgRoleList = _context.OrganizationRoles.Where(x => x.OrgID == orgId).ToList();
            List<SelectListItem> _roleList = new List<SelectListItem>();

            foreach (var item in orgRoleList)
            {
                _roleList.Add(new SelectListItem
                {
                    Text = item.RoleName,
                    Value = item.ID.ToString()
                });
            }

            return _roleList;
        }

        private List<SelectListItem> BindDropDownKlinik()
        {
            List<SelectListItem> _clinicLists = new List<SelectListItem>();
            foreach (var item in new ClinicHandler(_unitOfWork).GetAllClinic())
            {
                _clinicLists.Insert(0, new SelectListItem
                {
                    Text = "",
                    Value = "0"
                });
                _clinicLists.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _clinicLists;
        }

        private List<SelectListItem> BindDropDownOrganization()
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

        private List<SelectListItem> BindDropDownEmployee()
        {
            List<SelectListItem> _employeeLists = new List<SelectListItem>();
            _employeeLists.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
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

        private List<SelectListItem> BindDropDownEmployeeReff()
        {
            List<SelectListItem> _employeeActiveLists = new List<SelectListItem>();
            _employeeActiveLists.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
            foreach (var item in new EmployeeHandler(_unitOfWork).GetActiveEmployee())
            {
                _employeeActiveLists.Add(new SelectListItem
                {
                    Text = $"{item.EmpID} - {item.EmpName}",
                    Value = item.Id.ToString()
                });
            }

            return _employeeActiveLists;
        }

        private List<SelectListItem> BindDropDownEmployementType()
        {
            List<SelectListItem> _empTypes = new List<SelectListItem>();

            foreach (var item in new FamilyStatusHandler(_unitOfWork).GetAllFamilyStatus().ToList())
            {
                _empTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _empTypes;
        }

        private List<SelectListItem> BindDropDownEmployeeStatus()
        {
            List<SelectListItem> _empStatus = new List<SelectListItem>();
            _empStatus.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
            foreach (var item in new EmployeeStatusHandler(_unitOfWork).GetAllEmployeeStatus())
            {
                _empStatus.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.Id.ToString()
                });
            }

            return _empStatus;
        }

        private List<SelectListItem> BindDropDownCity()
        {
            List<SelectListItem> _cities = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.City.ToString()).ToList())
            {
                _cities.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _cities;
        }

        private List<SelectListItem> BindDropDownDoctorType()
        {
            List<SelectListItem> _doctorTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.DoctorType.ToString()).ToList())
            {
                _doctorTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _doctorTypes;
        }

        private List<SelectListItem> BindDropDownParamedicType()
        {
            List<SelectListItem> _paramedicTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.ParamedicType.ToString()).ToList())
            {
                _paramedicTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _paramedicTypes;
        }

        private List<SelectListItem> BindDropDownClinicType()
        {
            List<SelectListItem> _clinicTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.ClinicType.ToString()).ToList())
            {
                _clinicTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _clinicTypes;
        }

        private List<SelectListItem> BindDropDownMenu()
        {
            List<SelectListItem> _menus = new List<SelectListItem>();
            _menus.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = ""
            });
            foreach (var item in new MenuHandler(_unitOfWork).GetVisibleMenu().ToList())
            {
                _menus.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.Id.ToString()
                });
            }

            return _menus;
        }

        private List<SelectListItem> BindDropDownPoliType()
        {
            List<SelectListItem> _PoliTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.PoliType.ToString()).ToList())
            {
                _PoliTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _PoliTypes;
        }


        #endregion

        // GET: MasterData
        public ActionResult Index()
        {
            return View();
        }

        #region ::ORGANIZATION::
        [CustomAuthorize("VIEW_M_ORG")]
        public ActionResult OrganizationList() => View();

        [HttpPost]
        public ActionResult CreateOrEditOrganization(OrganizationModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new OrganizationRequest
            {
                Data = _model,
            };

            OrganizationResponse _response = new OrganizationResponse();

            new OrganizationValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.clinics = BindDropDownKlinik();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_ORG", "EDIT_M_ORG")]
        public ActionResult CreateOrEditOrganization()
        {
            OrganizationResponse _response = new OrganizationResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new OrganizationRequest
                {
                    Data = new OrganizationModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                OrganizationResponse resp = new OrganizationHandler(_unitOfWork).GetDetailOrganizationById(request);
                OrganizationModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownKlinik();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownKlinik();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetOrganizationData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new OrganizationRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new OrganizationHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterOrganisasi(int id)
        {
            OrganizationResponse _response = new OrganizationResponse();
            var request = new OrganizationRequest
            {
                Data = new OrganizationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new OrganizationValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRIVILEGE::
        [CustomAuthorize("VIEW_M_PRIVILEGE")]
        public ActionResult PrivilegeList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditPrivilege(PrivilegeModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new PrivilegeRequest
            {
                Data = _model
            };

            PrivilegeResponse _response = new PrivilegeResponse();

            new PrivilegeValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Menu = BindDropDownMenu();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRIVILEGE", "EDIT_M_PRIVILEGE")]
        public ActionResult CreateOrEditPrivilege()
        {
            PrivilegeResponse _response = new PrivilegeResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PrivilegeRequest
                {
                    Data = new PrivilegeModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PrivilegeResponse resp = new PrivilegeHandler(_unitOfWork).GetDetail(request);
                PrivilegeModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Menu = BindDropDownMenu();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Menu = BindDropDownMenu();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetPrivilegeData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PrivilegeRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PrivilegeHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterPrivilege(int id)
        {
            PrivilegeResponse _response = new PrivilegeResponse();
            var request = new PrivilegeRequest
            {
                Data = new PrivilegeModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PrivilegeValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::ROLE::
        [CustomAuthorize("VIEW_M_ROLE")]
        public ActionResult RoleList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditRole(RoleModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new RoleRequest
            {
                Data = _model
            };

            RoleResponse _response = new RoleResponse();

            new RoleValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_ROLE", "EDIT_M_ROLE")]
        public ActionResult CreateOrEditRole()
        {
            RoleResponse _response = new RoleResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new RoleRequest
                {
                    Data = new RoleModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                RoleResponse resp = new RoleHandler(_unitOfWork).GetDetail(request);
                RoleModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetRoleData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new RoleRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new RoleHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterRole(int id)
        {
            RoleResponse _response = new RoleResponse();
            var request = new RoleRequest
            {
                Data = new RoleModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new RoleValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::USER::
        [CustomAuthorize("VIEW_M_USER")]
        public ActionResult UserList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditUser(UserModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new UserRequest
            {
                Data = _model
            };

            UserResponse _response = new UserResponse();

            new UserValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            var tempOrgList = BindDropDownOrganization();
            ViewBag.Organisasi = tempOrgList;
            ViewBag.Employees = BindDropDownEmployee();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;
            ViewBag.RoleList = BindDropDownRoleList(int.Parse(tempOrgList[0].Value));

            return View();
        }

        [CustomAuthorize("ADD_M_USER", "EDIT_M_USER")]
        public ActionResult CreateOrEditUser()
        {
            UserResponse _response = new UserResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new UserRequest
                {
                    Data = new UserModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString()),

                    }
                };

                UserResponse resp = new UserHandler(_unitOfWork).GetDetail(request);
                UserModel _model = resp.Entity;
                ViewBag.Response = _response;
                var tempOrgList = BindDropDownOrganization();
                ViewBag.Organisasi = tempOrgList;
                ViewBag.Employees = BindDropDownEmployee();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                ViewBag.RoleList = BindDropDownRoleList(int.Parse(tempOrgList[0].Value));
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                var tempOrgList = BindDropDownOrganization();
                ViewBag.Organisasi = tempOrgList;
                ViewBag.Employees = BindDropDownEmployee();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.RoleList = BindDropDownRoleList(int.Parse(tempOrgList[0].Value));
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetUserData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new UserRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new UserHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterUser(int id)
        {
            UserResponse _response = new UserResponse();
            var request = new UserRequest
            {
                Data = new UserModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new UserValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleList(int orgId)
        {
            // prevent circular reference
            _context.Configuration.ProxyCreationEnabled = false;

            List<OrganizationRole> orgRoleList = _context.OrganizationRoles.Where(x => x.OrgID == orgId).ToList();

            return Json(orgRoleList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::EMPLOYEE::
        [CustomAuthorize("VIEW_M_EMPLOYEE")]
        public ActionResult EmployeeList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditEmployee(EmployeeModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new EmployeeRequest
            {
                Data = _model
            };

            EmployeeResponse _response = new EmployeeValidator(_unitOfWork, _context).Validate(request);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.EmpTypes = BindDropDownEmployementType();
            ViewBag.EmpStatus = BindDropDownEmployeeStatus();
            ViewBag.EmpActive = BindDropDownEmployeeReff();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_EMPLOYEE", "EDIT_M_EMPLOYEE")]
        public ActionResult CreateOrEditEmployee()
        {
            EmployeeResponse _response = new EmployeeResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new EmployeeRequest
                {
                    Data = new EmployeeModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                EmployeeResponse resp = new EmployeeHandler(_unitOfWork).GetDetail(request);
                EmployeeModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.EmpTypes = BindDropDownEmployementType();
                ViewBag.EmplStatus = BindDropDownEmployeeStatus();
                ViewBag.EmpActive = BindDropDownEmployeeReff();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.EmpTypes = BindDropDownEmployementType();
                ViewBag.EmplStatus = BindDropDownEmployeeStatus();
                ViewBag.EmpActive = BindDropDownEmployeeReff();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetEmployee()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new EmployeeRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new EmployeeHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterEmployee(int id)
        {
            var request = new EmployeeRequest
            {
                Data = new EmployeeModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            EmployeeResponse _response = new EmployeeValidator(_unitOfWork, _context).Validate(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::CLINIC::
        [CustomAuthorize("VIEW_M_CLINIC")]
        public ActionResult ClinicList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditClinic(ClinicModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ClinicRequest
            {
                Data = _model
            };

            ClinicResponse _response = new ClinicResponse();

            new ClinicValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Cities = BindDropDownCity();
            ViewBag.ClinicTypes = BindDropDownClinicType();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_CLINIC", "EDIT_M_CLINIC")]
        public ActionResult CreateOrEditClinic()
        {
            ClinicResponse _response = new ClinicResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ClinicRequest
                {
                    Data = new ClinicModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ClinicResponse resp = new ClinicHandler(_unitOfWork).GetDetail(request);
                ClinicModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Cities = BindDropDownCity();
                ViewBag.ClinicTypes = BindDropDownClinicType();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Cities = BindDropDownCity();
                ViewBag.ClinicTypes = BindDropDownClinicType();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetClinic()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ClinicRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ClinicHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterClinic(int id)
        {
            ClinicResponse _response = new ClinicResponse();
            var request = new ClinicRequest
            {
                Data = new ClinicModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ClinicValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::POLI::
        [CustomAuthorize("VIEW_M_POLI")]
        public ActionResult PoliList()
        {
            return View();
        }

      
        #endregion
    }
}