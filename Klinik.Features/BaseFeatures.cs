using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
    }
}