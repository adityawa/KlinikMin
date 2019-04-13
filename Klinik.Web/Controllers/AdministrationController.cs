using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using System.Web.Mvc;
using System.Linq;
using System;
using Klinik.Features;
using Klinik.Entities.Administration;

namespace Klinik.Web.Controllers
{
    public class AdministrationController : BaseController
    {
        public AdministrationController(IUnitOfWork unitOfWork, klinikEntities context) :
            base(unitOfWork, context)
        {
        }

        [CustomAuthorize("VIEW_LOG")]
        public ActionResult Logging()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAllLogs()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LogRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new LogHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogDetailOldValue(int id)
        {
            LogResponse response = new LogHandler(_unitOfWork).GetDataByID(id);
            var data = response.Data[0].OldValue;

            return Json(new { Data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogDetailNewValue(int id)
        {
            LogResponse response = new LogHandler(_unitOfWork).GetDataByID(id);
            var data = response.Data[0].NewValue;

            return Json(new { Data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}