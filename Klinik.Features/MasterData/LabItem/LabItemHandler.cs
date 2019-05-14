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
using System.Linq.Expressions;

namespace Klinik.Features.MasterData.LabItem
{
    public class LabItemHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LabItemHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit lab item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemResponse CreateOrEdit(LabItemRequest request)
        {
            LabItemResponse response = new LabItemResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.LabItemRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Klinik.Data.DataRepository.LabItem, LabItemModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.Code = request.Data.Code;
                        qry.Name = request.Data.Name;
                        qry.LabItemCategoryID = request.Data.LabItemCategoryID;
                        qry.Normal = request.Data.Normal;
                        qry.Price = request.Data.Price;

                        _unitOfWork.LabItemRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "LabItem", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.EDIT_LAB_ITEM, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "LabItem");

                            CommandLog(false, ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.EDIT_LAB_ITEM, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "LabItem");

                        CommandLog(false, ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.EDIT_LAB_ITEM, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var labItemEntity = Mapper.Map<LabItemModel, Klinik.Data.DataRepository.LabItem>(request.Data);
                    labItemEntity.CreatedBy = request.Data.Account.UserCode;
                    labItemEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.LabItemRepository.Insert(labItemEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "LabItem", labItemEntity.Name, labItemEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.ADD_LAB_ITEM, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "LabItem");

                        CommandLog(false, ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.ADD_LAB_ITEM, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.EDIT_LAB_ITEM, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_LAB_ITEM, Constants.Command.ADD_LAB_ITEM, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get lab item details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemResponse GetDetail(LabItemRequest request)
        {
            LabItemResponse response = new LabItemResponse();

            var qry = _unitOfWork.LabItemRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Klinik.Data.DataRepository.LabItem, LabItemModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get lab item list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemResponse GetListData(LabItemRequest request, Expression<Func<Klinik.Data.DataRepository.LabItem, bool>> searchCriteria = null)
        {
            List<LabItemModel> lists = new List<LabItemModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Klinik.Data.DataRepository.LabItem>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (searchCriteria != null)
            {
                searchPredicate = searchPredicate.And(searchCriteria);
            }

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Name.Contains(request.SearchValue) || p.Code.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;
                        case "code":
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Code));
                            break;
                        case "price":
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Price));
                            break;

                        default:
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;
                        case "code":
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Code));
                            break;
                        case "price":
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Price));
                            break;
                        default:
                            qry = _unitOfWork.LabItemRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.LabItemRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Klinik.Data.DataRepository.LabItem, LabItemModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new LabItemResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove lab item data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemResponse RemoveData(LabItemRequest request)
        {
            LabItemResponse response = new LabItemResponse();

            try
            {
                var labItem = _unitOfWork.LabItemRepository.GetById(request.Data.Id);
                if (labItem.ID > 0)
                {
                    labItem.RowStatus = -1;
                    labItem.ModifiedBy = request.Data.Account.UserCode;
                    labItem.ModifiedDate = DateTime.Now;

                    _unitOfWork.LabItemRepository.Update(labItem);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "LabItem", labItem.Name, labItem.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "LabItem");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "LabItem");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_LAB_ITEM, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
