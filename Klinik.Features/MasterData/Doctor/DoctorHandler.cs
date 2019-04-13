using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class DoctorHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public DoctorHandler(IUnitOfWork unitOfWork, klinikEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Get all Doctor
        /// </summary>
        /// <returns></returns>
        public IList<DoctorModel> GetAllDoctor()
        {
            var qry = _unitOfWork.DoctorRepository.Get();
            IList<DoctorModel> Doctors = new List<DoctorModel>();
            foreach (var item in qry)
            {
                var _Doctor = Mapper.Map<Doctor, DoctorModel>(item);
                Doctors.Add(_Doctor);
            }

            return Doctors;
        }

        /// <summary>
        /// Create or edit a Doctor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse CreateOrEdit(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();
            try
            {
                string type = request.Data.TypeID == 0 ? Messages.Doctor : Messages.Paramedic;
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.DoctorRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Doctor, DoctorModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;
                        qry.Address = request.Data.Address;
                        qry.KTPNumber = request.Data.KTPNumber;
                        qry.STRNumber = request.Data.STRNumber;
                        qry.STRValidFrom = request.Data.STRValidFrom;
                        qry.STRValidTo = request.Data.STRValidTo;
                        qry.Email = request.Data.Email;
                        qry.HPNumber = request.Data.HPNumber;
                        qry.Remark = request.Data.Remark;
                        qry.SpecialistID = request.Data.SpecialistID;
                        qry.TypeID = request.Data.TypeID;
                        qry.ModifiedDate = DateTime.Now;
                        qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";

                        _unitOfWork.DoctorRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, type, qry.Name, qry.Code);

                            CommandLog(true, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, type);

                            CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, type);

                        CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    return CreateDoctor(request.Data);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_DOCTOR, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Create new doctor automatically create employee and user also
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private DoctorResponse CreateDoctor(DoctorModel requestData)
        {
            DoctorResponse response = new DoctorResponse();
            using (var transaction = _context.Database.BeginTransaction())
            {
                string type = requestData.TypeID == 0 ? Messages.Doctor : Messages.Paramedic;
                int _resultAffected = 0;
                try
                {
                    // create employee
                    Employee employeeEntity = Mapper.Map<DoctorModel, Employee>(requestData);
                    employeeEntity.CreatedBy = requestData.Account.UserCode;
                    employeeEntity.CreatedDate = DateTime.Now;

                    // create user
                    User userEntity = Mapper.Map<DoctorModel, User>(requestData);
                    userEntity.Password = CommonUtils.Encryptor(requestData.Password, CommonUtils.KeyEncryptor);
                    userEntity.Status = true;
                    userEntity.Employee = employeeEntity;
                    userEntity.CreatedBy = requestData.Account.UserCode;
                    userEntity.CreatedDate = DateTime.Now;
                    userEntity.ExpiredDate = DateTime.Now.AddDays(100);

                    Doctor doctorEntity = Mapper.Map<DoctorModel, Doctor>(requestData);
                    doctorEntity.Employee = employeeEntity;
                    doctorEntity.CreatedBy = requestData.Account.UserCode;
                    doctorEntity.CreatedDate = DateTime.Now;

                    _context.Users.Add(userEntity);
                    _context.Doctors.Add(doctorEntity);

                    _resultAffected = _context.SaveChanges();

                    if (_resultAffected > 0)
                    {
                        CommandLog(true, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_DOCTOR, requestData.Account, doctorEntity);

                        response.Message = string.Format(Messages.ObjectHasBeenAdded, type, requestData.Name, requestData.Id);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.Status = false;
                    response.Message = string.Format(Messages.AddObjectFailed, type);

                    ErrorLog(ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_DOCTOR, requestData.Account, ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse GetDetail(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();

            var qry = _unitOfWork.DoctorRepository.Query(x => x.ID == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Doctor, DoctorModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get list data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse GetListData(DoctorRequest request)
        {
            return GetListData(request, true);
        }

        /// <summary>
        /// Get employee list of data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isDoctor"></param>
        /// <returns></returns>
        public DoctorResponse GetListData(DoctorRequest request, bool isDoctor)
        {
            List<DoctorModel> lists = new List<DoctorModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Doctor>(true);

            // add default filter to show doctor or paramedic list            
            searchPredicate = searchPredicate.And(p => p.TypeID == (isDoctor ? 0 : 1) && p.RowStatus != -1);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Code.Contains(request.SearchValue) || p.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Code));
                            break;
                        case "name":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Code));
                            break;
                        case "name":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.DoctorRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                DoctorModel doctorData = Mapper.Map<Doctor, DoctorModel>(item);
                doctorData.TypeName = isDoctor ? Messages.Doctor : Messages.Paramedic;

                if (isDoctor)
                {
                    var doctor = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Value == doctorData.SpecialistID.ToString() && x.Type == ClinicEnums.MasterTypes.DoctorType.ToString());
                    if (doctor != null)
                        doctorData.SpecialistName = doctor.Name;
                }
                else
                {
                    var paramedic = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Value == doctorData.SpecialistID.ToString() && x.Type == ClinicEnums.MasterTypes.ParamedicType.ToString());
                    if (paramedic != null)
                        doctorData.SpecialistName = paramedic.Name;
                }

                lists.Add(doctorData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new DoctorResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove employee data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse RemoveData(DoctorRequest request)
        {
            return RemoveData(request, true);
        }

        /// <summary>
        /// Remove employee data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isDoctor"></param>
        /// <returns></returns>
        public DoctorResponse RemoveData(DoctorRequest request, bool isDoctor)
        {
            DoctorResponse response = new DoctorResponse();
            try
            {
                string type = isDoctor ? Messages.Doctor : Messages.Paramedic;
                var doctor = _unitOfWork.DoctorRepository.GetById(request.Data.Id);
                if (doctor.ID > 0)
                {
                    doctor.RowStatus = -1;
                    doctor.ModifiedBy = request.Data.Account.UserCode;
                    doctor.ModifiedDate = DateTime.Now;

                    _unitOfWork.DoctorRepository.Update(doctor);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, type, doctor.Name, doctor.Code);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, type);
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, type);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_DOCTOR, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}