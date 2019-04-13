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
using System.Data.SqlTypes;
using System.Data.Entity;

namespace Klinik.Features
{
    public class EmployeeHandler : BaseFeatures, IBaseFeatures<EmployeeResponse, EmployeeRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// 

        public EmployeeHandler(IUnitOfWork unitOfWork, klinikEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit employee
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse CreateOrEdit(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();

          

            var _empTypeDesc = _unitOfWork.FamilyRelationshipRepository.GetFirstOrDefault(x => x.ID == request.Data.EmpType) == null ? "" : _unitOfWork.FamilyRelationshipRepository.GetFirstOrDefault(x => x.ID == request.Data.EmpType).Code;
            var _statusDesc = _unitOfWork.EmployeeStatusRepository.GetFirstOrDefault(x => x.ID == request.Data.EmpStatus) == null ? "" : _unitOfWork.EmployeeStatusRepository.GetFirstOrDefault(x => x.ID == request.Data.EmpStatus).Code;
            if (request.Data.EmpTypeDesc == string.Empty || request.Data.EmpTypeDesc == null)
                request.Data.EmpTypeDesc = _empTypeDesc;

            using (var transaction = _context.Database.BeginTransaction())
            {
                int _resultAffected = 0;
                //cek employee assignment, if empid exist with joint date
                long _emplId = GetEmployeeNo(request.Data.EmpID);
                if (_emplId == 0 && !String.IsNullOrEmpty(request.Data.LastEmpId))
                    _emplId = GetEmployeeNo(request.Data.LastEmpId);

                if (request.Data.IsFromAPI)
                {
                    //convert EmpType, Reff Emp ID & EmpStatus to Id
                    request.Data.EmpType = _context.FamilyRelationships.SingleOrDefault(x => x.Code == request.Data.EmpTypeDesc).ID;
                    request.Data.Status = _context.EmployeeStatus.SingleOrDefault(x => x.Code == request.Data.EmpStatusDesc).ID;
                    if (request.Data.ReffEmpID != string.Empty)
                        request.Data.ReffEmpID = _context.Employees.SingleOrDefault(x => x.EmpID == request.Data.ReffEmpID).ID.ToString();
                }

                try
                {
                    var _qry = _context.Employees.SingleOrDefault(x => x.EmpID == request.Data.EmpID);
                    if (_qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Employee, EmployeeModel>(_qry);
                        // update data
                        _qry.EmpName = request.Data.EmpName;
                        _qry.BirthDate = request.Data.Birthdate;
                        _qry.Gender = request.Data.Gender;
                        _qry.Email = CommonUtils.Encryptor( request.Data.Email, CommonUtils.KeyEncryptor);
                        _qry.EmpType = request.Data.EmpType;
                        _qry.ReffEmpID = request.Data.ReffEmpID;
                        _qry.Status = request.Data.EmpStatus;
                        _qry.KTPNumber = request.Data.KTPNumber;
                        _qry.HPNumber = CommonUtils.Encryptor( request.Data.HPNumber, CommonUtils.KeyEncryptor);
                        _qry.ReffEmpID = request.Data.ReffEmpID;
                        _qry.LastEmpID = request.Data.LastEmpId;
                        _qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";
                        _qry.ModifiedDate = DateTime.Now;
                        _resultAffected = _context.SaveChanges();

                        if (_resultAffected > 0)
                            CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, request.Data, _oldentity);

                        var _qryEmpAss = _context.EmployeeAssignments.FirstOrDefault(x => x.EmployeeID == _emplId && x.StartDate == request.Data.StartDate);
                        if (_qryEmpAss != null)
                        {
                            _resultAffected = 0;
                            _qryEmpAss.StartDate = request.Data.StartDate;
                            _qryEmpAss.EndDate = request.Data.EndDate.Value.ToString().Contains("1/1/0001") ? (DateTime)SqlDateTime.Null : request.Data.EndDate.Value;
                            _qryEmpAss.Department = request.Data.Department;
                            _qryEmpAss.Region = request.Data.Region;
                            _qryEmpAss.BusinessUnit = request.Data.BussinesUnit;
                            _qryEmpAss.EmpStatus = request.Data.EmpStatus;
                            _qryEmpAss.LastEmpID = request.Data.LastEmpId;
                            _qryEmpAss.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";
                            _qryEmpAss.ModifiedDate = DateTime.Now;
                            _resultAffected = _context.SaveChanges();

                            var newEmployeeAssignment = new EmployeeAssignment
                            {
                                EmployeeID = _qry.ID,
                                BusinessUnit = request.Data.BussinesUnit,
                                Department = request.Data.Department,
                                StartDate = request.Data.StartDate,
                                EndDate = request.Data.EndDate,
                                Region = request.Data.Region,
                                EmpStatus = request.Data.EmpStatus,
                                LastEmpID = request.Data.LastEmpId
                            };
                            if (_resultAffected > 0)
                                CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.EDIT_EMPLOYEEASSIGNMENT, request.Data.Account, newEmployeeAssignment, _qryEmpAss);
                        }


                        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Employee", _qry.EmpName, _qry.EmpID);

                    }
                    else
                    {
                        //cek dulu dgn Last Employee ID
                        var oldEmployee = _context.Employees.SingleOrDefault(x => x.EmpID == request.Data.LastEmpId);
                        if (oldEmployee != null)
                        {
                            long _newEmpId = 0;

                            //insert new in employee & employee assignment
                            if (request.Data.EmpTypeDesc.Trim() != Constants.Command.EmployeeRelationshipCode)
                            {
                                request.Data.EmpID = string.Format("{0}-{1}", request.Data.ReffEmpID, request.Data.EmpTypeDesc);

                            }

                            var _employeeEntity = Mapper.Map<EmployeeModel, Employee>(request.Data);
                            _employeeEntity.CreatedDate = DateTime.Now;
                            _employeeEntity.CreatedBy = request.Data.Account.UserCode;

                            _context.Employees.Add(_employeeEntity);
                            _resultAffected = _context.SaveChanges();
                            if (_resultAffected > 0)
                            {
                                CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.ADD_NEW_EMPLOYEE, request.Data.Account, _employeeEntity);
                                _newEmpId = _employeeEntity.ID;
                            }

                            if (request.Data.EmpTypeDesc.Trim() == Constants.Command.EmployeeRelationshipCode)
                            {
                                var _employeeAssignmentEntity = new EmployeeAssignment
                                {
                                    EmployeeID = _employeeEntity.ID,
                                    BusinessUnit = request.Data.BussinesUnit,
                                    Department = request.Data.Department,
                                    StartDate = request.Data.StartDate,
                                    EndDate = request.Data.EndDate,
                                    Region = request.Data.Region,
                                    EmpStatus = request.Data.EmpStatus,
                                    LastEmpID = request.Data.LastEmpId,
                                    Grade = request.Data.Grade,
                                    CreatedDate = DateTime.Now,
                                    CreatedBy = request.Data.Account.UserCode
                                };

                                _context.EmployeeAssignments.Add(_employeeAssignmentEntity);

                                _resultAffected = _context.SaveChanges();
                                if (_resultAffected > 0)
                                    CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.ADD_EMPLOYEEASSIGNMENT, request.Data.Account, _employeeAssignmentEntity);
                            }

                            oldEmployee.Status = _context.EmployeeStatus.FirstOrDefault(x => x.Code == Constants.Command.NotActiveCode).ID;
                            _context.SaveChanges();

                            //get all dependent
                            var _getAllDependant = _context.Employees.Where(x => x.ReffEmpID == oldEmployee.ID.ToString());
                            foreach (var item in _getAllDependant)
                            {
                                item.ReffEmpID = _newEmpId.ToString();
                                item.EmpID = $"{_newEmpId}-{item.EmployeeStatu.Code}";
                                _context.SaveChanges();
                            }

                            //update employee assignment
                            var oldEmpAssignment = _context.EmployeeAssignments.SingleOrDefault(x => x.EmployeeID == _emplId);
                            if (oldEmpAssignment.ID > 0)
                            {
                                oldEmpAssignment.EndDate = request.Data.StartDate;
                                oldEmpAssignment.EmpStatus = _context.EmployeeStatus.FirstOrDefault(x => x.Code == Constants.Command.NotActiveCode.ToString()).ID;
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            if (request.Data.EmpTypeDesc.Trim() != Constants.Command.EmployeeRelationshipCode)
                            {
                                request.Data.EmpID = string.Format("{0}-{1}", request.Data.ReffEmpID, request.Data.EmpTypeDesc);
                            }

                            var _employeeEntity = Mapper.Map<EmployeeModel, Employee>(request.Data);

                            _employeeEntity.CreatedDate = DateTime.Now;
                            _employeeEntity.CreatedBy = request.Data.Account.UserCode;
                            _context.Employees.Add(_employeeEntity);
                            _resultAffected = _context.SaveChanges();
                            if (_resultAffected > 0)
                                CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.ADD_NEW_EMPLOYEE, request.Data.Account, _employeeEntity);
                            if (request.Data.EmpTypeDesc.Trim() == Constants.Command.EmployeeRelationshipCode)
                            {
                                var _employeeAssignmentEntity = new EmployeeAssignment
                                {
                                    EmployeeID = _employeeEntity.ID,
                                    BusinessUnit = request.Data.BussinesUnit,
                                    Department = request.Data.Department,
                                    StartDate = request.Data.StartDate,
                                    EndDate = request.Data.EndDate,
                                    Region = request.Data.Region,
                                    EmpStatus = request.Data.EmpStatus,
                                    LastEmpID = request.Data.LastEmpId,
                                    Grade = request.Data.Grade,
                                    CreatedDate = DateTime.Now,
                                    CreatedBy = request.Data.Account.UserCode
                                };

                                _context.EmployeeAssignments.Add(_employeeAssignmentEntity);

                                _resultAffected = _context.SaveChanges();
                                if (_resultAffected > 0)
                                    CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.ADD_EMPLOYEEASSIGNMENT, request.Data.Account, _employeeAssignmentEntity);
                            }
                        }

                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Employee", request.Data.EmpName, request.Data.EmpID);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    if (request.Data != null && request.Data.Id > 0)
                        ErrorLog(ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, ex);
                    else
                        ErrorLog(ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.ADD_NEW_EMPLOYEE, request.Data.Account, ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Get all employee list
        /// </summary>
        /// <returns></returns>
        public IList<EmployeeModel> GetAllEmployee()
        {
            var qry = _unitOfWork.EmployeeRepository.Get();
            IList<EmployeeModel> employees = new List<EmployeeModel>();
            foreach (var item in qry)
            {
                var _emp = Mapper.Map<Employee, EmployeeModel>(item);
                employees.Add(_emp);
            }

            return employees;
        }

        public IList<EmployeeModel> GetActiveEmployee()
        {
            //Get from EmployeeAssignment
            DateTime minTime = (DateTime)SqlDateTime.Null;
            var _idActiveEmp = _unitOfWork.EmployeeAssignmentRepository.Get(x => (x.EndDate == null || x.EndDate == minTime) && x.EmployeeStatu.Status == "A").Select(x => x.EmployeeID);
            var qry = _unitOfWork.EmployeeRepository.Get(x => _idActiveEmp.Contains(x.ID));
            IList<EmployeeModel> employees = new List<EmployeeModel>();
            foreach (var item in qry)
            {
                var _activeEmp = Mapper.Map<Employee, EmployeeModel>(item);
                employees.Add(_activeEmp);
            }

            return employees;
        }

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse GetDetail(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();

            var qry = _unitOfWork.EmployeeRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Employee, EmployeeModel>(qry.FirstOrDefault());
                //get from employee assignment
                var qryEmpAssignment = _unitOfWork.EmployeeAssignmentRepository.GetFirstOrDefault(x => x.EmployeeID == response.Entity.Id);
                if (qryEmpAssignment != null)
                {
                    response.Entity.Department = qryEmpAssignment.Department;
                    response.Entity.BussinesUnit = qryEmpAssignment.BusinessUnit;
                    response.Entity.Region = qryEmpAssignment.Region;
                    response.Entity.StartDate = qryEmpAssignment.StartDate == null ? Convert.ToDateTime("0001-01-01") : qryEmpAssignment.StartDate.Value;
                    response.Entity.EndDate = qryEmpAssignment.EndDate == null ? Convert.ToDateTime("0001-01-01") : qryEmpAssignment.EndDate.Value;
                    response.Entity.Grade = qryEmpAssignment.Grade;
                    response.Entity.LastEmpId = qryEmpAssignment.LastEmpID;
                }
            }

            return response;
        }

        /// <summary>
        /// Get employee list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse GetListData(EmployeeRequest request)
        {
            List<EmployeeModel> lists = new List<EmployeeModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Employee>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.EmpID.Contains(request.SearchValue) || p.EmpName.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "empid":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpID), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;
                        case "empname":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpName), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;

                        default:

                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID), p => p.EmployeeAssignments, p => p.FamilyRelationship, prop => prop.EmployeeStatu);
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "empid":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EmpID), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;
                        case "empname":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EmpName), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;

                        default:
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, null, includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Employee, EmployeeModel>(item);
                long _idEmp = prData.Id ?? 0;
                var _dept = _unitOfWork.EmployeeAssignmentRepository.GetFirstOrDefault(x => x.EmployeeID == _idEmp) == null ? "" : _unitOfWork.EmployeeAssignmentRepository.GetFirstOrDefault(x => x.EmployeeID == _idEmp).Department;
                prData.Department = _dept;
                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new EmployeeResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove employee data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse RemoveData(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();
            int result_affected = 0;

            #region ::OLD::
            //try
            //{
            //    var isExist = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
            //    if (isExist.id > 0)
            //    {
            //        _unitOfWork.EmployeeRepository.Delete(isExist.id);
            //        int resultAffected = _unitOfWork.Save();
            //        if (resultAffected > 0)
            //        {
            //            response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Employee", isExist.EmpName, isExist.id);
            //        }
            //        else
            //        {
            //            response.Status = false;
            //            response.Message = string.Format(Messages.RemoveObjectFailed, "Employee");
            //        }
            //    }
            //    else
            //    {
            //        response.Status = false;
            //        response.Message = string.Format(Messages.RemoveObjectFailed, "Employee");
            //    }
            //}
            //catch
            //{
            //    response.Status = false;
            //    response.Message = Messages.GeneralError;
            //}
            #endregion

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var employee = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
                    if (employee == null)
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Employee");
                    }
                    else
                    {
                        if (employee.FamilyRelationship != null &&
                            request.Data.EmpID != null &&
                            !string.IsNullOrEmpty(employee.FamilyRelationship.Name) &&
                            employee.FamilyRelationship.Name.ToLower() == "employee")
                        {
                            //remove first in Employee Assignment
                            long _empId = GetEmployeeNo(request.Data.EmpID);
                            var empAssignment = _unitOfWork.EmployeeAssignmentRepository.GetFirstOrDefault(x => x.EmployeeID == _empId);
                            if (empAssignment != null)
                            {
                                _context.EmployeeAssignments.Attach(empAssignment);
                                empAssignment.RowStatus = -1;
                                empAssignment.ModifiedBy = request.Data.Account.UserCode;
                                empAssignment.ModifiedDate = DateTime.Now;

                                result_affected = _context.SaveChanges();

                                if (result_affected > 0)
                                    CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.DELETE_EMPLOYEE, request.Data.Account, null, empAssignment);
                            }
                        }

                        _context.Employees.Attach(employee);

                        employee.RowStatus = -1;
                        employee.ModifiedBy = request.Data.Account.UserCode;
                        employee.ModifiedDate = DateTime.Now;

                        result_affected = _context.SaveChanges();

                        if (result_affected > 0)
                            CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.DELETE_EMPLOYEE, request.Data.Account, null, employee);

                        transaction.Commit();
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Employee", request.Data.EmpName, request.Data.EmpID);
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    ErrorLog(ClinicEnums.Module.MASTER_EMPLOYEE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
                }
            }

            return response;
        }

        private long GetEmployeeNo(string employeeCd)
        {
            long _emplId = 0;
            var _qry = _unitOfWork.EmployeeRepository.GetFirstOrDefault(x => x.EmpID == employeeCd);
            if (_qry != null)
            {
                _emplId = _qry.ID;
            }
            return _emplId;
        }
    }
}