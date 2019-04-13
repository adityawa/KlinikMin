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
    public class OrganizationHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public OrganizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get orginazation data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse GetListData(OrganizationRequest request)
        {
            List<OrganizationData> lists = new List<OrganizationData>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Organization>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.OrgCode.Contains(request.SearchValue) || p.OrgName.Contains(request.SearchValue) || p.Clinic.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "orgcode":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.OrgCode), includes: x => x.Clinic);
                            break;
                        case "orgname":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.OrgName), includes: x => x.Clinic);
                            break;
                        default:
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID), includes: x => x.Clinic);
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "orgcode":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.OrgCode), includes: x => x.Clinic);
                            break;
                        case "orgname":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.OrgName), includes: x => x.Clinic);
                            break;
                        default:
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID), includes: x => x.Clinic);
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, null, includes: x => x.Clinic);
            }

            foreach (var item in qry)
            {
                var orData = Mapper.Map<Organization, OrganizationData>(item);
                lists.Add(orData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new OrganizationResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Get organization data
        /// </summary>
        /// <returns></returns>
        public List<OrganizationModel> GetOrganizationList()
        {
            List<OrganizationModel> lists = new List<OrganizationModel>();
            var qry = _unitOfWork.OrganizationRepository.Get(null, null, includes: x => x.Clinic);
            foreach (var item in qry)
            {
                lists.Add(Mapper.Map<Organization, OrganizationModel>(item));
            }

            return lists;
        }

        /// <summary>
        /// Create or edit an organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse CreateOrEditOrganization(OrganizationRequest request)
        {
            OrganizationResponse response = new OrganizationResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.OrganizationRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Organization, OrganizationModel>(qry);

                        // update data
                        qry.OrgCode = request.Data.OrgCode;
                        qry.OrgName = request.Data.OrgName;
                        qry.KlinikID = request.Data.KlinikID;
                        qry.ModifiedBy = request.Data.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.OrganizationRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Organization", qry.OrgName, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.EDIT_ORG, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Orgnization");

                            CommandLog(false, ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.EDIT_ORG, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Orgnization");

                        CommandLog(false, ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.EDIT_ORG, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var OrganizationEntity = Mapper.Map<OrganizationModel, Organization>(request.Data);
                    OrganizationEntity.CreatedBy = request.Data.Account.UserName;
                    OrganizationEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.OrganizationRepository.Insert(OrganizationEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Orgnization", OrganizationEntity.OrgName, OrganizationEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.ADD_NEW_ORG, request.Data.Account, OrganizationEntity);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Orgnization");

                        CommandLog(false, ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.ADD_NEW_ORG, request.Data.Account, OrganizationEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.EDIT_ORG, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_ORGANIZATION, Constants.Command.ADD_NEW_ORG, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get organization details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse GetDetailOrganizationById(OrganizationRequest request)
        {
            OrganizationResponse response = new OrganizationResponse();

            var qry = _unitOfWork.OrganizationRepository.Query(x => x.ID == request.Data.Id, null, includes: x => x.Clinic);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Organization, OrganizationData>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Remove organizationd data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse RemoveOrganization(OrganizationRequest request)
        {
            OrganizationResponse response = new OrganizationResponse();

            try
            {
                var organization = _unitOfWork.OrganizationRepository.GetById(request.Data.Id);
                if (organization.ID > 0)
                {
                    organization.RowStatus = -1;
                    organization.ModifiedBy = request.Data.Account.UserCode;
                    organization.ModifiedDate = DateTime.Now;

                    _unitOfWork.OrganizationRepository.Update(organization);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Orgnization", organization.OrgName, organization.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Orgnization");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Orgnization");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}