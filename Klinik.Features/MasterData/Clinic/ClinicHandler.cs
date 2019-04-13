using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Features.MasterData.Clinic;
using System.Collections.Generic;
using System;
using System.Linq;
using LinqKit;
using Klinik.Resources;

namespace Klinik.Features
{
    public class ClinicHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ClinicHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all clinic
        /// </summary>
        /// <returns></returns>
        public IList<ClinicModel> GetAllClinic()
        {
            var qry = _unitOfWork.ClinicRepository.Get();
            IList<ClinicModel> clinics = new List<ClinicModel>();
            foreach (var item in qry)
            {
                var _clinic = Mapper.Map<Clinic, ClinicModel>(item);
                clinics.Add(_clinic);
            }

            return clinics;
        }

        /// <summary>
        /// Create or edit a clinic
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse CreateOrEdit(ClinicRequest request)
        {
            ClinicResponse response = new ClinicResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.ClinicRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Clinic, ClinicModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;
                        qry.Address = request.Data.Address;
                        qry.LegalNumber = request.Data.LegalNumber;
                        qry.LegalDate = request.Data.LegalDate;
                        qry.ContactNumber = request.Data.ContactNumber;
                        qry.Email = request.Data.Email;
                        qry.Lat = request.Data.Lat;
                        qry.Long = request.Data.Long;
                        qry.CityID = request.Data.CityId;
                        qry.ClinicType = request.Data.ClinicType;
                        qry.ModifiedDate = DateTime.Now;
                        qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";

                        _unitOfWork.ClinicRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Clinic", qry.Name, qry.Code);

                            CommandLog(true, ClinicEnums.Module.MASTER_CLINIC, Constants.Command.EDIT_CLINIC, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Clinic");

                            CommandLog(false, ClinicEnums.Module.MASTER_CLINIC, Constants.Command.EDIT_CLINIC, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Clinic");

                        CommandLog(false, ClinicEnums.Module.MASTER_CLINIC, Constants.Command.EDIT_CLINIC, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var clinicEntity = Mapper.Map<ClinicModel, Clinic>(request.Data);
                    clinicEntity.CreatedBy = request.Data.Account.UserCode;
                    clinicEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.ClinicRepository.Insert(clinicEntity);

                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Clinic", clinicEntity.Name, clinicEntity.Code);

                        CommandLog(true, ClinicEnums.Module.MASTER_CLINIC, Constants.Command.ADD_NEW_CLINIC, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Clinic");

                        CommandLog(false, ClinicEnums.Module.MASTER_CLINIC, Constants.Command.ADD_NEW_CLINIC, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_CLINIC, Constants.Command.EDIT_CLINIC, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_CLINIC, Constants.Command.ADD_NEW_CLINIC, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse GetDetail(ClinicRequest request)
        {
            ClinicResponse response = new ClinicResponse();

            var qry = _unitOfWork.ClinicRepository.Query(x => x.ID == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Clinic, ClinicModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get employee list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse GetListData(ClinicRequest request)
        {
            List<ClinicModel> lists = new List<ClinicModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Clinic>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

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
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Code));
                            break;
                        case "name":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Code));
                            break;
                        case "name":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ClinicRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var clinicData = Mapper.Map<Clinic, ClinicModel>(item);
                long _cityId = clinicData.CityId ?? 0;
                long _clinicTypeId = clinicData.ClinicType ?? 0;
                var getCityDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.ID == _cityId && x.Type == ClinicEnums.MasterTypes.City.ToString());
                var getClinicTypeDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.ID == _clinicTypeId && x.Type == ClinicEnums.MasterTypes.ClinicType.ToString());
                if (getCityDesc != null)
                    clinicData.CityDesc = getCityDesc.Name ?? "";
                if (getClinicTypeDesc != null)
                    clinicData.ClinicTypeDesc = getClinicTypeDesc.Name ?? "";

                lists.Add(clinicData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ClinicResponse
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
        public ClinicResponse RemoveData(ClinicRequest request)
        {
            ClinicResponse response = new ClinicResponse();
            try
            {
                var clinic = _unitOfWork.ClinicRepository.GetById(request.Data.Id);
                if (clinic.ID > 0)
                {
                    clinic.RowStatus = -1;
                    clinic.ModifiedBy = request.Data.Account.UserCode;
                    clinic.ModifiedDate = DateTime.Now;

                    _unitOfWork.ClinicRepository.Update(clinic);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Clinic", clinic.Name, clinic.Code);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Clinic");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Clinic");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_CLINIC, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}