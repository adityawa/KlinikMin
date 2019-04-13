using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.MasterData.FamilyRelationship
{
    public class FamilyStatusHandler : BaseFeatures
    {
        public FamilyStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<FamilyRelationshipModel> GetAllFamilyStatus()
        {
            var qry = _unitOfWork.FamilyRelationshipRepository.Get();
            IList<FamilyRelationshipModel> families = new List<FamilyRelationshipModel>();
            foreach (var item in qry)
            {
                var _families = Mapper.Map<Data.DataRepository.FamilyRelationship, FamilyRelationshipModel>(item);
                families.Add(_families);
            }

            return families;
        }
    }
}
