using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class MasterHandler : BaseFeatures
    {
        public MasterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<GeneralMaster> GetMasterDataByType(string type)
        {
            return _unitOfWork.MasterRepository.Query(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }
    }
}