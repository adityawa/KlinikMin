using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class OrganizationPrivilegeHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public OrganizationPrivilegeHandler(IUnitOfWork unitOfWork, klinikEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit organization privilege
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationPrivilegeResponse CreateOrEdit(OrganizationPrivilegeRequest request)
        {
            OrganizationPrivilegeResponse response = new OrganizationPrivilegeResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.OrganizationPrivileges.Where(x => x.OrgID == request.Data.OrgID);
                    _context.OrganizationPrivileges.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (long _privId in request.Data.PrivilegeIDs)
                    {
                        var orgpprivilege = new OrganizationPrivilege
                        {
                            OrgID = request.Data.OrgID,
                            PrivilegeID = _privId,
                            CreatedBy = request.Data.Account.UserCode,
                            CreatedDate = DateTime.Now
                        };

                        _context.OrganizationPrivileges.Add(orgpprivilege);
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

                    ErrorLog(ClinicEnums.Module.MASTER_ORGANIZATION_PRIVILEGE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Get list of organization privilege data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationPrivilegeResponse GetListData(OrganizationPrivilegeRequest request)
        {
            var qry = _unitOfWork.OrgPrivRepository.Get(x => x.OrgID == request.Data.OrgID && x.RowStatus == 0);
            OrganizationPrivilegeModel _model = new OrganizationPrivilegeModel();

            if (qry.Count > 0)
                _model.OrgID = qry.FirstOrDefault().OrgID;

            foreach (var item in qry)
            {
                _model.PrivilegeIDs.Add(item.PrivilegeID);
            }

            var response = new OrganizationPrivilegeResponse
            {
                Entity = _model
            };

            return response;
        }
    }
}