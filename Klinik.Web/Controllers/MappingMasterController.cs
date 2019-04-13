using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MappingMaster;
using Klinik.Entities.MasterData;

using Klinik.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class MappingMasterController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private klinikEntities _context;

        public MappingMasterController(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: MappingMaster
        #region ::Organization Role::
        public ActionResult OrgPrivilegeList()
        {
            OrganizationPrivilegeModel opmodel = new OrganizationPrivilegeModel();
            if (Request.QueryString["orgid"] != null)
            {
                var reqOrg = new OrganizationRequest
                {
                    Data = new OrganizationModel
                    {
                        Id = Convert.ToInt64(Request.QueryString["orgid"].ToString())
                    }
                };

                var respOrg = new OrganizationResponse();
                respOrg = new OrganizationHandler(_unitOfWork).GetDetailOrganizationById(reqOrg);
                opmodel.OrganizationName = respOrg.Entity.OrgName;
                opmodel.OrgID = respOrg.Entity.Id;

                var _request = new OrganizationPrivilegeRequest
                {
                    Data = new OrganizationPrivilegeModel
                    {
                        OrgID = Convert.ToInt64(Request.QueryString["orgid"].ToString())
                    }
                };
                //get Privilege Ids for organization
                var selPrivileges = new OrganizationPrivilegeHandler(_unitOfWork, _context).GetListData(_request);
                if (selPrivileges.Entity.PrivilegeIDs != null && selPrivileges.Entity.PrivilegeIDs.Count > 0)
                    opmodel.PrivilegeIDs = selPrivileges.Entity.PrivilegeIDs;
            }

            return View(opmodel);
        }

        [HttpPost]
        public JsonResult CreateOrganizationPrivilege()
        {
            OrganizationPrivilegeResponse response = new OrganizationPrivilegeResponse();
            OrganizationPrivilegeModel _model = new OrganizationPrivilegeModel();
            if (Request.Form["OrgId"] != null)
                _model.OrgID = Convert.ToInt64(Request.Form["OrgId"].ToString());
            if (Request.Form["Privileges"] != null)
                _model.PrivilegeIDs = JsonConvert.DeserializeObject<List<long>>(Request.Form["Privileges"]);
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new OrganizationPrivilegeRequest
            {
                Data = _model
            };

            new OrganizationPrivilegeValidator(_unitOfWork, _context).Validate(request, out response);
            return Json(new { Status = response.Status, Message = response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetOrganizationPrivilegeData()
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
        #endregion

        #region ::Role Privilege::
        public ActionResult RolePrivilegeList()
        {
            RolePrivilegeModel rpmodel = new RolePrivilegeModel();
            if (Request.QueryString["roleid"] != null)
            {
                var reqOrg = new RoleRequest
                {
                    Data = new RoleModel
                    {
                        Id = Convert.ToInt64(Request.QueryString["roleid"].ToString())
                    }
                };

                var respOrg = new RoleResponse();
                respOrg = new RoleHandler(_unitOfWork).GetDetail(reqOrg);
                rpmodel.RoleDesc = respOrg.Entity.RoleName;
                rpmodel.RoleID = respOrg.Entity.Id;

                var _request = new RolePrivilegeRequest
                {
                    Data = new RolePrivilegeModel
                    {
                        RoleID = Convert.ToInt64(Request.QueryString["roleid"].ToString())
                    }
                };
                //get Privilege Ids for organization
                var selPrivileges = new RolePrivilegeHandler(_unitOfWork, _context).GetListData(_request);
                if (selPrivileges.Entity.PrivilegeIDs != null && selPrivileges.Entity.PrivilegeIDs.Count > 0)
                    rpmodel.PrivilegeIDs = selPrivileges.Entity.PrivilegeIDs;
            }

            return View(rpmodel);
        }

        [HttpPost]
        public JsonResult CreateRolePrivilege()
        {
            RolePrivilegeResponse response = new RolePrivilegeResponse();
            RolePrivilegeModel _model = new RolePrivilegeModel();
            if (Request.Form["RoleId"] != null)
                _model.RoleID = Convert.ToInt64(Request.Form["RoleId"].ToString());
            if (Request.Form["Privileges"] != null)
                _model.PrivilegeIDs = JsonConvert.DeserializeObject<List<long>>(Request.Form["Privileges"]);
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new RolePrivilegeRequest
            {
                Data = _model
            };

            new RolePrivilegeValidator(_unitOfWork, _context).Validate(request, out response);
            return Json(new { Status = response.Status, Message = response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRolePrivilegeData(string roleid)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var _model = new RolePrivilegeModel
            {
                RoleID = long.Parse(roleid)
            };

            var request = new RolePrivilegeRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = _model
            };

            var response = new RolePrivilegeHandler(_unitOfWork, _context).GetPrivilegeBasedOnOrganization(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::USERROLE::
        public ActionResult UserRoleList()
        {
            UserRoleModel rpmodel = new UserRoleModel();
            if (Request.QueryString["userid"] != null)
            {
                var reqOrg = new UserRequest
                {
                    Data = new UserModel
                    {
                        Id = Convert.ToInt64(Request.QueryString["userid"].ToString())
                    }
                };

                var respUser = new UserResponse();
                respUser = new UserHandler(_unitOfWork).GetDetail(reqOrg);
                rpmodel.UserName = respUser.Entity.UserName;
                rpmodel.UserID = respUser.Entity.Id;

                var _request = new UserRoleRequest
                {
                    Data = new UserRoleModel
                    {
                        UserID = Convert.ToInt64(Request.QueryString["userid"].ToString())
                    }
                };

                //get Privilege Ids for organization
                var selRoles = new UserRoleHandler(_unitOfWork, _context).GetListData(_request);
                if (selRoles.Entity.RoleIds != null && selRoles.Entity.RoleIds.Count > 0)
                    rpmodel.RoleIds = selRoles.Entity.RoleIds;
            }

            return View(rpmodel);
        }

        [HttpPost]
        public JsonResult CreateUserRole()
        {
            UserRoleResponse response = new UserRoleResponse();
            UserRoleModel _model = new UserRoleModel();
            if (Request.Form["UserID"] != null)
                _model.UserID = Convert.ToInt64(Request.Form["UserID"].ToString());
            if (Request.Form["Roles"] != null)
                _model.RoleIds = JsonConvert.DeserializeObject<List<long>>(Request.Form["Roles"]);
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new UserRoleRequest
            {
                Data = _model
            };

            new UserRoleValidator(_unitOfWork, _context).Validate(request, out response);
            return Json(new { Status = response.Status, Message = response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUserRoleData(string userid)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var _model = new UserRoleModel
            {
                UserID = long.Parse(userid)
            };

            var request = new UserRoleRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = _model
            };

            var response = new UserRoleHandler(_unitOfWork, _context).GetRoleBasedOnOrganization(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
        #endregion

       
    }
}