using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Klinik.Entities.MasterData;
using Klinik.Data.DataRepository;

namespace Klinik.Features.MasterData.EmployeeStatus
{
    public class EmployeeStatusHandler : BaseFeatures
    {
        public EmployeeStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<EmployeeStatusModel> GetAllEmployeeStatus()
        {
            var qry = _unitOfWork.EmployeeStatusRepository.Get();
            IList<EmployeeStatusModel> empstatus = new List<EmployeeStatusModel>();
            foreach (var item in qry)
            {
                var _status = Mapper.Map<EmployeeStatu, EmployeeStatusModel>(item);
                empstatus.Add(_status);
            }

            return empstatus;
        }
    }
}
