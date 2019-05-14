using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Features;
using Klinik.Features.Laboratorium;
using Klinik.Features.Loket;
using Klinik.Features.MasterData.LabItem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
namespace Klinik.Web.Controllers
{
    public class LabController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private klinikEntities _context;
        private ClinicHandler _clinicHandler;
        #region ::DROPDOWN::
        private List<SelectListItem> BindDropDownDokter()
        {
            IList<DoctorModel> Doctors = new DoctorHandler(_unitOfWork).GetAllDoctor();
            List<SelectListItem> _doctorList = new List<SelectListItem>();

            _doctorList.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = "0"
            });

            foreach (var item in Doctors)
            {
                _doctorList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _doctorList;
        }

        protected List<SelectListItem> BindLabCategory(string _poliNm)
        {
            List<SelectListItem> _dataList = new List<SelectListItem>();
            foreach (var item in new LabHandler(_unitOfWork).GetLaboratoriumCategory(_poliNm).ToList())
            {
                _dataList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _dataList;
        }

        private List<SelectListItem> BindDropDownStatus()
        {

            List<SelectListItem> _status = new List<SelectListItem>();
            _status.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = "-1"
            });

            _status.Insert(1, new SelectListItem
            {
                Text = "New",
                Value = "0"
            });

            _status.Insert(2, new SelectListItem
            {
                Text = "Process",
                Value = "1"
            });

            _status.Insert(3, new SelectListItem
            {
                Text = "Hold",
                Value = "2"
            });

            _status.Insert(4, new SelectListItem
            {
                Text = "Finish",
                Value = "3"
            });

            return _status;
        }

        private List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _authorizedClinics = new List<SelectListItem>();
            if (Session["UserLogon"] != null)
            {
                var _account = (AccountModel)Session["UserLogon"];

                var _getClinics = _clinicHandler.GetAllClinic(_account.ClinicID);
                foreach (var item in _getClinics)
                {
                    _authorizedClinics.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
            }

            return _authorizedClinics;
        }
        #endregion

        #region ::CHEKCKLIST TABLE::
        [HttpPost]
        public ActionResult GetListLabItem()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LabItemRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,


            };


            Expression<Func<LabItem, bool>> _serachCriteria = x => x.LabItemCategory.LabType == Constants.NameConstant.Laboratorium;
            var response = new LabItemHandler(_unitOfWork).GetListData(request, _serachCriteria);

            return Json(new { data = response.Data.OrderBy(x => x.LabItemCategoryID), recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLabItemForInput()
        {

            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LabRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,

                PageSize = _pageSize,
                Skip = _skip,
            };

            if (request.Data == null)
                request.Data = new FormExamineLabModel();
            request.Data.LoketData = new LoketModel
            {
                Id = Convert.ToInt64(Session["QueuePoliId"])
            };


            var response = new LabHandler(_unitOfWork, _context).GetLabForInput(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public LabController(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public LabController(IUnitOfWork unitOfWork, klinikEntities context, ClinicHandler clinicHandler)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _clinicHandler = clinicHandler;
        }
        // GET: Lab
        [CustomAuthorize("VIEW_QUEUE_LAB")]
        public ActionResult ListQueueLaboratorium()
        {
            ViewBag.Status = BindDropDownStatus();
            ViewBag.Clinics = BindDropDownClinic();
            return View();
        }

        [HttpPost]
        public ActionResult GetListQueue(string clinics, string status)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LoketRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new LoketModel { ClinicID = Convert.ToInt32(clinics), Status = Convert.ToInt32(status) }

            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new LabHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddLabItem()
        {
            LabResponse response = new LabResponse();
            FormExamineLabModel _model = new FormExamineLabModel();
            if (Request.Form["FormMedicalID"] != null)
                _model.FormMedicalID = Convert.ToInt64(Request.Form["FormMedicalID"].ToString());
            if (Request.Form["LabItems"] != null)
                _model.LabItemsId = JsonConvert.DeserializeObject<List<int>>(Request.Form["LabItems"]);
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            if (Session["QueuePoliId"] != null)
            {
                if (_model.LoketData == null)
                    _model.LoketData = new LoketModel();
                _model.LoketData.Id = Convert.ToInt64(Session["QueuePoliId"].ToString());
            }

            var request = new LabRequest
            {
                Data = _model
            };

            new LabValidator(_unitOfWork, _context).Validate(request, out response);
            return Json(new { Status = response.Status, Message = response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddLabResult()
        {
            LabResponse response = new LabResponse();
            FormExamineLabModel _model = new FormExamineLabModel();
            if (Request.Form["FormMedicalID"] != null)
                _model.FormMedicalID = Convert.ToInt64(Request.Form["FormMedicalId"].ToString());
            if (Request.Form["LabResults"] != null)
                _model.LabItemCollsJs = JsonConvert.DeserializeObject<List<LabItemResultJS>>(Request.Form["LabResults"]);
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            _model.FormExamine = new FormExamineModel
            {
                DoctorID = Request.Form["Doctor"] == null ? 0 : Convert.ToInt32(Request.Form["Doctor"].ToString()),
                Result = Request.Form["Conclusion"]
            };

            if (Session["QueuePoliId"] != null)
            {
                if (_model.LoketData == null)
                    _model.LoketData = new LoketModel();
                _model.LoketData.Id = Convert.ToInt64(Session["QueuePoliId"].ToString());
            }

            var request = new LabRequest
            {
                Data = _model
            };

            new LabValidator(_unitOfWork, _context).ValidateAddResult(request, out response);
            return Json(new { Status = response.Status, Message = response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_Lab_Item")]
        public ActionResult CreateItemLab()
        {
            LabResponse response = new LabResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new LabRequest
                {
                    Data = new FormExamineLabModel
                    {
                        LoketData = new LoketModel
                        {
                            Id = long.Parse(Request.QueryString["id"].ToString()),
                        },

                    }
                };
                if (Session["UserLogon"] != null)
                    request.Data.Account = (AccountModel)Session["UserLogon"];

                LabResponse resp = new LabHandler(_unitOfWork).GetDetailPatient(request.Data.LoketData.Id);
                FormExamineLabModel _model = resp.Entity;
                Session["QueuePoliId"] = resp.Entity.LoketData.Id;
                _model.LabItemsId = LabHandler.GetSelectedLabItem(request.Data.LoketData.Id);
                return View(_model);
            }
            ViewBag.LabCategory = BindLabCategory(Constants.NameConstant.Laboratorium);
            return View();
        }
        [CustomAuthorize("ADD_LAB_RESULT")]
        public ActionResult InputLabResult()
        {
            LabResponse response = new LabResponse();
            var _model = new FormExamineLabModel { };
            if (Request.QueryString["id"] != null)
            {
                var request = new LabRequest
                {
                    Data = new FormExamineLabModel
                    {
                        LoketData = new LoketModel
                        {
                            Id = long.Parse(Request.QueryString["id"].ToString()),
                        },

                    }
                };
                if (Session["UserLogon"] != null)
                    request.Data.Account = (AccountModel)Session["UserLogon"];

                LabResponse resp = new LabHandler(_unitOfWork).GetDetailPatient(request.Data.LoketData.Id);
                _model = resp.Entity;
                LabResponse resp2 = new LabHandler(_unitOfWork).GetDataExamine(request.Data.LoketData.Id);
                if (_model.FormExamine == null)
                    _model.FormExamine = new FormExamineModel();
                _model.FormExamine.Result = resp2.Entity.FormExamine.Result;
                _model.FormExamine.DoctorID = resp2.Entity.FormExamine.DoctorID;
                Session["QueuePoliId"] = resp.Entity.LoketData.Id;
            }
            ViewBag.DoctorList = BindDropDownDokter();
            return View(_model);
        }
    }
}