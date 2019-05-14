using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Administration;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Features.Loket;
using LinqKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Klinik.Features
{
    /// <summary>
    /// Abstract class of base features
    /// </summary>
    public abstract class BaseFeatures
    {
        public static IUnitOfWork _unitOfWork { get; set; }
        public klinikEntities _context;
        public IList<string> errorFields = new List<string>();

        /// <summary>
        /// Contructor
        /// </summary>
        public BaseFeatures()
        {
        }

        /// <summary>
        /// Constructor with parameter
        /// </summary>
        /// <param name="unitOfWork"></param>
        public BaseFeatures(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate the privilege based on its name
        /// </summary>
        /// <param name="privilege_name"></param>
        /// <param name="PrivilegeIds"></param>
        /// <returns></returns>
        public bool IsHaveAuthorization(string privilege_name, List<long> PrivilegeIds)
        {
            bool IsAuthorized = false;
            var _getPrivilegeName = _unitOfWork.PrivilegeRepository.Get(x => PrivilegeIds.Contains(x.ID));

            foreach (var item in _getPrivilegeName)
            {
                if (privilege_name == item.Privilege_Name)
                    IsAuthorized = true;
            }

            return IsAuthorized;
        }

        public DateTime reformatDate(string dtStr)
        {
            string[] arrDates = dtStr.Split('/');
            return new DateTime(Convert.ToInt16(arrDates[2]), Convert.ToInt16(arrDates[1]), Convert.ToInt16(arrDates[0]));
        }

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="module"></param>
        /// <param name="status"></param>
        /// <param name="command"></param>
        /// <param name="account"></param>        
        /// <param name="ex"></param>
        public static void ErrorLog(ClinicEnums.Module module, string command, AccountModel account, Exception ex)
        {
            CommandLog(false, module, command, account, ex.GetAllMessages());
        }

        /// <summary>
        /// Log any executed command by user
        /// </summary>
        /// <param name="module"></param>
        /// <param name="status"></param>
        /// <param name="command"></param>
        /// <param name="account"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        public static void CommandLog(bool status, ClinicEnums.Module module, string command, AccountModel account, object newValue = null, object oldValue = null)
        {
            try
            {
                var log = new LogModel
                {
                    Start = DateTime.Now,
                    Module = module.ToString(),
                    Status = status ? "SUCCESS" : "ERROR",
                    Command = command,
                    UserName = account.UserName,
                    Organization = account.Organization,
                    OldValue = oldValue is null ? null : JsonConvert.SerializeObject(oldValue),
                    NewValue = newValue is null ? null : JsonConvert.SerializeObject(newValue),
                    CreatedBy = account.UserCode,
                    CreatedDate = DateTime.Now
                };

                var _entity = Mapper.Map<LogModel, Log>(log);

                using (var context = new klinikEntities())
                {
                    context.Logs.Add(_entity);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.GetAllMessages(), EventLogEntryType.Error, 101, 1);
                }
            }
        }

        public List<LoketModel> GetbaseLoketData(LoketRequest request, Expression<Func<QueuePoli, bool>> searchCriteria = null)
        {

            List<LoketModel> lists = new List<LoketModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<QueuePoli>(true);
            searchPredicate = searchPredicate.And(searchCriteria);

            if (request.Data.PoliToID != 0)
            {
                searchPredicate = searchPredicate.And(p => p.PoliTo == request.Data.PoliToID);
            }

            if (request.Data.ClinicID != 0)
            {
                searchPredicate = searchPredicate.And(p => p.ClinicID == request.Data.ClinicID);
            }

            if (request.Data.Status != -1)
            {
                searchPredicate = searchPredicate.And(p => p.Status == request.Data.Status);
            }

            if (request.Data.strIsPreExamine != string.Empty)
            {
                bool _isAlreadyPreExamine = Convert.ToBoolean(request.Data.strIsPreExamine);
                searchPredicate = searchPredicate.And(p => p.IsPreExamine == _isAlreadyPreExamine);
            }

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Patient.Name.Contains(request.SearchValue) ||
                 p.Doctor.Name.Contains(request.SearchValue) || p.Patient.MRNumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "patientname":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Patient.Name));
                            break;
                        case "doctorstr":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Doctor.Name));
                            break;
                        case "transactiondatestr":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.TransactionDate));
                            break;
                        default:
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "patientname":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Patient.Name));
                            break;
                        case "doctorstr":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Doctor.Name));
                            break;
                        case "transactiondatestr":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.TransactionDate));
                            break;
                        default:
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                LoketModel lokmdl = Mapper.Map<QueuePoli, LoketModel>(item);
                if (item.Type == (int)RegistrationTypeEnum.MCU)
                {
                    lokmdl.SortNumberCode = "M-" + string.Format("{0:D3}", item.SortNumber);
                }
                else
                {
                    lokmdl.SortNumberCode = item.Poli1.Code.Trim() + "-" + string.Format("{0:D3}", item.SortNumber);
                }
                lists.Add(lokmdl);
            }
            DateTime _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime _end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            lists = lists.Where(x => x.TransactionDate >= _start && x.TransactionDate <= _end).ToList();
            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            return data;
        }
    }
}