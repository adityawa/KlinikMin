using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Entities.MappingMaster;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using Klinik.Resources;

namespace Klinik.Features
{
    public class UserRoleHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public UserRoleHandler(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit user role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserRoleResponse CreateOrEdit(UserRoleRequest request)
        {
            UserRoleResponse response = new UserRoleResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.UserRoles.Where(x => x.UserID == request.Data.UserID);
                    _context.UserRoles.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (long _roleid in request.Data.RoleIds)
                    {
                        var _userrole = new UserRole
                        {
                            UserID = request.Data.UserID,
                            RoleID = _roleid,
                            CreatedBy = request.Data.Account.UserCode,
                            CreatedDate = DateTime.Now
                        };

                        _context.UserRoles.Add(_userrole);
                    }

                    int resultAffected = _context.SaveChanges();
                    transaction.Commit();

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
        /// Get list of user role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserRoleResponse GetListData(UserRoleRequest request)
        {
            var qry = _unitOfWork.UserRoleRepository.Get(x => x.UserID == request.Data.UserID && x.RowStatus == 0);
            UserRoleModel _model = new UserRoleModel();

            if (qry.Count > 0)
                _model.UserID = qry.FirstOrDefault().UserID;

            foreach (var item in qry)
            {
                _model.RoleIds.Add(item.RoleID);
            }

            var response = new UserRoleResponse
            {
                Entity = _model
            };

            return response;
        }

        /// <summary>
        /// Get user role based on organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RoleResponse GetRoleBasedOnOrganization(UserRoleRequest request)
        {
            var _orgId = _unitOfWork.UserRepository.GetById(request.Data.UserID) == null ? 0 : _unitOfWork.UserRepository.GetById(request.Data.UserID).OrganizationID;

            List<RoleModel> lists = new List<RoleModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<OrganizationRole>(true);
            searchPredicate = searchPredicate.And(x => x.OrgID == _orgId);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.RoleName.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "rolename":
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.RoleName));
                            break;

                        default:
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "rolename":
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.RoleName));
                            break;

                        default:
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.RoleRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<OrganizationRole, RoleModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new RoleResponse
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