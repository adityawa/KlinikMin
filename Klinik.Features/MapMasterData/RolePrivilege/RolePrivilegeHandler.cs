using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class RolePrivilegeHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public RolePrivilegeHandler(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RolePrivilegeResponse CreateOrEdit(RolePrivilegeRequest request)
        {
            RolePrivilegeResponse response = new RolePrivilegeResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.RolePrivileges.Where(x => x.RoleID == request.Data.RoleID);
                    _context.RolePrivileges.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (long _privid in request.Data.PrivilegeIDs)
                    {
                        var rolepprivilege = new RolePrivilege
                        {
                            RoleID = request.Data.RoleID,
                            PrivilegeID = _privid,
                            CreatedBy = request.Data.Account.UserCode,
                            CreatedDate = DateTime.Now
                        };

                        _context.RolePrivileges.Add(rolepprivilege);
                    }

                    int resultAffected = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = true;
                    response.Message = Messages.DataSaved;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    ErrorLog(ClinicEnums.Module.MASTER_ROLE_PRIVILEGE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Get list of role privilege data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RolePrivilegeResponse GetListData(RolePrivilegeRequest request)
        {
            var qry = _unitOfWork.RolePrivRepository.Get(x => x.RoleID == request.Data.RoleID && x.RowStatus == 0);
            RolePrivilegeModel _model = new RolePrivilegeModel();

            if (qry.Count > 0)
                _model.RoleID = qry.FirstOrDefault().RoleID;

            foreach (var item in qry)
            {
                _model.PrivilegeIDs.Add(item.PrivilegeID);
            }

            var response = new RolePrivilegeResponse
            {
                Entity = _model
            };

            return response;
        }

        /// <summary>
        /// Get privilege based on organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationPrivilegeResponse GetPrivilegeBasedOnOrganization(RolePrivilegeRequest request)
        {
            var _orgId = _unitOfWork.RoleRepository.GetById(request.Data.RoleID) == null ? 0 : _unitOfWork.RoleRepository.GetById(request.Data.RoleID).OrgID;

            List<OrganizationPrivilegeModel> lists = new List<OrganizationPrivilegeModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<OrganizationPrivilege>(true);
            searchPredicate = searchPredicate.And(x => x.OrgID == _orgId);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Privilege.Privilege_Name.Contains(request.SearchValue) || p.Privilege.Privilege_Desc.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "privilevename":
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Privilege.Privilege_Name));
                            break;

                        default:
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "privilevename":
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Privilege.Privilege_Name));
                            break;

                        default:
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<OrganizationPrivilege, OrganizationPrivilegeModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new OrganizationPrivilegeResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }
    }
}