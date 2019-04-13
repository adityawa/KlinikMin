using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class DoctorRequest : BaseRequest<DoctorModel>
    {
        public DoctorRequest()
        {
        }

        public DoctorRequest(string doctorID)
        {
            Data = new DoctorModel { Id = long.Parse(doctorID) };
        }

        public DoctorRequest(long doctorID)
        {
            Data = new DoctorModel { Id = doctorID };
        }
    }
}
