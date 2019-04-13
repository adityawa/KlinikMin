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

namespace Klinik.Features.MasterData.Poli
{
    public class PoliHandler : BaseFeatures, IBaseFeatures<PoliResponse, PoliRequest>
    {
        public PoliHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IList<PoliModel> GetAllPoli(int exId)
        {
            var qry = _unitOfWork.PoliRepository.Get(x => x.ID != exId);
            IList<PoliModel> polies = new List<PoliModel>();
            foreach (var item in qry)
            {
                var _poli = Mapper.Map<Klinik.Data.DataRepository.Poli, PoliModel>(item);
                polies.Add(_poli);
            }

            return polies;
        }

        public PoliResponse CreateOrEdit(PoliRequest request)
        {
            PoliResponse response = new PoliResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PoliRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Klinik.Data.DataRepository.Poli, PoliModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;
                        qry.ID = Convert.ToInt32(request.Data.Id);

                        _unitOfWork.PoliRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Poli", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_POLI, Constants.Command.EDIT_POLI, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Poli");

                            CommandLog(false, ClinicEnums.Module.MASTER_POLI, Constants.Command.EDIT_POLI, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Poli");

                        CommandLog(false, ClinicEnums.Module.MASTER_POLI, Constants.Command.EDIT_POLI, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var roleEntity = Mapper.Map<PoliModel, Klinik.Data.DataRepository.Poli>(request.Data);
                    roleEntity.CreatedBy = request.Data.Account.UserCode;
                    roleEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.PoliRepository.Insert(roleEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Poli", roleEntity.Name, roleEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_POLI, Constants.Command.ADD_NEW_ROLE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Role");

                        CommandLog(false, ClinicEnums.Module.MASTER_ROLE, Constants.Command.ADD_NEW_POLI, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_POLI, Constants.Command.EDIT_POLI, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_POLI, Constants.Command.ADD_NEW_POLI, request.Data.Account, ex);
            }

            return response;
        }

        public PoliResponse GetDetail(PoliRequest request)
        {
            PoliResponse response = new PoliResponse();

            var qry = _unitOfWork.PoliRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Klinik.Data.DataRepository.Poli, PoliModel>(qry.FirstOrDefault());
            }

            return response;
        }

        public PoliResponse GetListData(PoliRequest request)
        {
            List<PoliModel> lists = new List<PoliModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Klinik.Data.DataRepository.Poli>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.Rowstatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "poliname":
                            qry = _unitOfWork.PoliRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "poliname":
                            qry = _unitOfWork.PoliRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PoliRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Klinik.Data.DataRepository.Poli, PoliModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PoliResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PoliResponse RemoveData(PoliRequest request)
        {
            PoliResponse response = new PoliResponse();

            try
            {
                var poli = _unitOfWork.PoliRepository.GetById(request.Data.Id);
                if (poli.ID > 0)
                {
                    poli.Rowstatus = -1;
                    poli.ModifiedBy = request.Data.Account.UserCode;
                    poli.ModifiedDate = DateTime.Now;

                    _unitOfWork.PoliRepository.Update(poli);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Poli", poli.Name, poli.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Poli");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Poli");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_POLI, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

    }
}
