using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Administration;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class LogHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LogHandler(IUnitOfWork unitOfWork, klinikEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Get list data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LogResponse GetListData(LogRequest request)
        {
            return GetListData(request, true);
        }

        /// <summary>
        /// Get log list of data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isLog"></param>
        /// <returns></returns>
        public LogResponse GetListData(LogRequest request, bool isLog)
        {
            List<LogModel> lists = new List<LogModel>();
            List<Log> qry = null;
            var searchPredicate = PredicateBuilder.New<Log>(true);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.CreatedBy.Contains(request.SearchValue) || p.Command.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "Module":
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Module));
                            break;
                        case "Command":
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Command));
                            break;
                        case "StartStr":
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.CreatedDate));
                            break;
                        default:
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "Module":
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.CreatedBy));
                            break;
                        case "Command":
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.CreatedBy));
                            break;
                        case "StartStr":
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.CreatedDate));
                            break;
                        default:
                            qry = _unitOfWork.LogRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.LogRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                LogModel LogData = Mapper.Map<Log, LogModel>(item);
                lists.Add(LogData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new LogResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Get list data
        /// </summary>
        /// <param name="logID"></param>
        /// <returns></returns>
        public LogResponse GetDataByID(int logID)
        {
            LogResponse response = new LogResponse();
            var log = _unitOfWork.LogRepository.GetById(logID);
            if (log != null)
            {
                var logModel = Mapper.Map<Log, LogModel>(log);
                response.Data.Add(logModel);
            }

            return response;
        }
    }
}