using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Klinik.Features.MasterData.LabItemCategory
{
    public class LabItemCategoryHandler : BaseFeatures, IBaseFeatures<LabItemCategoryResponse, LabItemCategoryRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LabItemCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit lab item category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemCategoryResponse CreateOrEdit(LabItemCategoryRequest request)
        {
            LabItemCategoryResponse response = new LabItemCategoryResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.LabItemCategoryRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Klinik.Data.DataRepository.LabItemCategory, LabItemCategoryModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.Name = request.Data.Name;
                        qry.LabType = request.Data.LabType;
                        qry.PoliID = request.Data.PoliID;

                        _unitOfWork.LabItemCategoryRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "LabItemCategory", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.EDIT_LAB_ITEM_CATEGORY, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "LabItemCategory");

                            CommandLog(false, ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.EDIT_LAB_ITEM_CATEGORY, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "LabItemCategory");

                        CommandLog(false, ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.EDIT_LAB_ITEM_CATEGORY, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var labItemEntity = Mapper.Map<LabItemCategoryModel, Klinik.Data.DataRepository.LabItemCategory>(request.Data);
                    labItemEntity.CreatedBy = request.Data.Account.UserCode;
                    labItemEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.LabItemCategoryRepository.Insert(labItemEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "LabItemCategory", labItemEntity.Name, labItemEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.ADD_LAB_ITEM_CATEGORY, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "LabItemCategory");

                        CommandLog(false, ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.ADD_LAB_ITEM_CATEGORY, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.EDIT_LAB_ITEM_CATEGORY, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, Constants.Command.ADD_LAB_ITEM_CATEGORY, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get lab item category details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemCategoryResponse GetDetail(LabItemCategoryRequest request)
        {
            LabItemCategoryResponse response = new LabItemCategoryResponse();

            var qry = _unitOfWork.LabItemCategoryRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Klinik.Data.DataRepository.LabItemCategory, LabItemCategoryModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get lab item category list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemCategoryResponse GetListData(LabItemCategoryRequest request)
        {
            List<LabItemCategoryModel> lists = new List<LabItemCategoryModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Klinik.Data.DataRepository.LabItemCategory>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

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
                        case "name":
                            qry = _unitOfWork.LabItemCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.LabItemCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.LabItemCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.LabItemCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.LabItemCategoryRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Klinik.Data.DataRepository.LabItemCategory, LabItemCategoryModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new LabItemCategoryResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove lab item category data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabItemCategoryResponse RemoveData(LabItemCategoryRequest request)
        {
            LabItemCategoryResponse response = new LabItemCategoryResponse();

            try
            {
                var labItemCategory = _unitOfWork.LabItemCategoryRepository.GetById(request.Data.Id);
                if (labItemCategory.ID > 0)
                {
                    labItemCategory.RowStatus = -1;
                    labItemCategory.ModifiedBy = request.Data.Account.UserCode;
                    labItemCategory.ModifiedDate = DateTime.Now;

                    _unitOfWork.LabItemCategoryRepository.Update(labItemCategory);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "LabItemCategory", labItemCategory.Name, labItemCategory.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "LabItemCategory");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "LabItemCategory");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_LAB_ITEM_CATEGORY, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
