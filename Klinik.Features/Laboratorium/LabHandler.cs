using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Features.Loket;
using Klinik.Features.MasterData.Poli;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Laboratorium
{
    public class LabHandler : BaseFeatures
    {
        public LabHandler(IUnitOfWork unitOfWork, klinikEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public LabResponse CreateOrEdit(LabRequest request)
        {
            int result = 0;
            var response = new LabResponse { };
            var _getDataLabPoli = _unitOfWork.LabItemRepository.Get(x => x.RowStatus == 0);

            #region ::DELETE FIRST::
            var _deleteFormExamineLab = _unitOfWork.FormExamineLabRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID && x.LabType == Constants.NameConstant.Laboratorium);
            foreach (var item1 in _deleteFormExamineLab)
            {
                _unitOfWork.FormExamineLabRepository.Delete(item1.ID);
            }

            var poliId = PoliHandler.GetPoliIDBasedOnName(Constants.NameConstant.Laboratorium);
            var _deleteFormExamine = _unitOfWork.FormExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID && x.PoliID == poliId);
            if (_deleteFormExamine != null)
            {
                _unitOfWork.FormExamineRepository.Delete(_deleteFormExamine.ID);
            }

            int deleteResult = _unitOfWork.Save();
            #endregion

            //insert to Form Examine   
            try
            {
                var _formExamine = new FormExamine
                {
                    FormMedicalID = request.Data.FormMedicalID,
                    PoliID = PoliHandler.GetPoliIDBasedOnName(Constants.NameConstant.Laboratorium),
                    TransDate = DateTime.Now,
                    CreatedBy = request.Data.Account.UserName,
                    CreatedDate = DateTime.Now
                };

                _unitOfWork.FormExamineRepository.Insert(_formExamine);

                foreach (var _id in request.Data.LabItemsId)
                {

                    var _formExamineLabEntity = new FormExamineLab
                    {
                        FormMedicalID = request.Data.FormMedicalID,
                        LabType = _getDataLabPoli.Where(x => x.ID == _id).FirstOrDefault().LabItemCategory.LabType,
                        LabItemID = (Int32)_id,
                        CreatedBy = request.Data.Account.UserName,
                        CreatedDate = DateTime.Now
                    };

                    _unitOfWork.FormExamineLabRepository.Insert(_formExamineLabEntity);

                }

                result = _unitOfWork.Save();
                if (result > 0)
                {
                    var _editQueuePoli = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
                    if (_editQueuePoli != null)
                    {
                        _editQueuePoli.Status = (int)RegistrationStatusEnum.Hold;
                        _editQueuePoli.ModifiedBy = request.Data.Account.UserName;
                        _editQueuePoli.ModifiedDate = DateTime.Now;
                        _unitOfWork.RegistrationRepository.Update(_editQueuePoli);
                        _unitOfWork.Save();
                    }
                }
                response.Status = true;
                response.Message = string.Format(Messages.LabItemAdded, result, request.Data.FormMedicalID);
            }
            catch (Exception ex)
            {

                response.Status = false;
                response.Message = Messages.GeneralError;
            }

            return response;
        }

        public LabResponse GetDetail(LabRequest request)
        {
            throw new NotImplementedException();
        }

        public LabResponse GetDetailPatient(long IdQueuePoli)
        {
            var qry_poli = _unitOfWork.RegistrationRepository.GetById(IdQueuePoli);
            var LabResponse = new LabResponse { };

            if (qry_poli != null)
            {
                if (LabResponse.Entity == null)
                    LabResponse.Entity = new FormExamineLabModel();
                LabResponse.Entity.PatientData = Mapper.Map<Patient, PatientModel>(qry_poli.Patient);
                LabResponse.Entity.FormMedicalID = qry_poli.FormMedicalID.Value;
                if (LabResponse.Entity.LoketData == null)
                    LabResponse.Entity.LoketData = new LoketModel();
                LabResponse.Entity.LoketData.Id = IdQueuePoli;
            }
            return LabResponse;
        }

        public static List<Int32> GetSelectedLabItem(long IdQueue)
        {
            List<Int32> labItemIds = new List<Int32>();
            var _getFormMedical = _unitOfWork.RegistrationRepository.GetById(IdQueue);
            if (_getFormMedical != null)
            {
                var qryLabItems = _unitOfWork.FormExamineLabRepository.Get(x => x.FormMedicalID == _getFormMedical.FormMedicalID);
                foreach (var item in qryLabItems)
                {
                    labItemIds.Add(item.LabItemID ?? 0);
                }
            }
            return labItemIds;
        }

        public LoketResponse GetListData(LoketRequest request)
        {
            var _laboratoriumId = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == Constants.NameConstant.Laboratorium);
            if (_laboratoriumId == null)
                _laboratoriumId = new Poli { ID = 0 };
            Expression<Func<QueuePoli, bool>> _serachCriteria = x => x.PoliTo == _laboratoriumId.ID;

            List<LoketModel> lists = base.GetbaseLoketData(request, _serachCriteria);
            int totalRequest = lists.Count();
            var response = new LoketResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = lists
            };

            return response;
        }

        public LabResponse RemoveData(LabRequest request)
        {
            throw new NotImplementedException();
        }

        public LabResponse GetLabForInput(LabRequest request)
        {
            List<FormExamineLabModel> lists = new List<FormExamineLabModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<FormExamineLab>(true);

            var _getQueuePoliData = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
            searchPredicate = searchPredicate.And(x => x.FormMedicalID == _getQueuePoliData.FormMedicalID && x.LabType == Constants.NameConstant.Laboratorium);
            qry = _unitOfWork.FormExamineLabRepository.Get(searchPredicate, null);

            foreach (var item in qry)
            {
                var prData = Mapper.Map<FormExamineLab, FormExamineLabModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists;

            var response = new LabResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public LabResponse CreateLabResult(LabRequest request)
        {
            var response = new LabResponse { };
            FormExamineLabModel model = new FormExamineLabModel();
            if (model.LabItemColls == null)
                model.LabItemColls = new List<FormExamineLabModel>();
            try
            {
                foreach (var item in request.Data.LabItemCollsJs.Where(x => x.name.StartsWith("Result_")))
                {
                    string[] temp = item.name.Split('_');
                    if (temp[0].ToLower() == "result")
                    {
                        //pertama Id FormExLab kedua Lab Item
                        string[] _ids = temp[1].Split('|');

                        long _Id = Convert.ToInt64(_ids[0]);
                        int _LabItemID = Convert.ToInt32(_ids[1]);
                        string _result = item.value;
                        string _resultIndicator = request.Data.LabItemCollsJs.Where(x => x.name == "ResultIndicator_" + temp[1]).FirstOrDefault().value;

                        var _existing = _unitOfWork.FormExamineLabRepository.Get(x => x.ID == _Id && x.LabItemID == _LabItemID).FirstOrDefault();
                        if (_existing != null)
                        {
                            _existing.Result = _result;
                            _existing.ResultIndicator = _resultIndicator;
                            _existing.ModifiedBy = request.Data.Account.UserName;
                            _existing.ModifiedDate = DateTime.Now;
                            _unitOfWork.FormExamineLabRepository.Update(_existing);
                        }


                    }
                }

                var _existingExam = _unitOfWork.FormExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID);
                if (_existingExam != null)
                {
                    _existingExam.Result = request.Data.FormExamine.Result;
                    _existingExam.DoctorID = request.Data.FormExamine.DoctorID;
                    _existingExam.ModifiedBy = request.Data.Account.UserName;
                    _existingExam.ModifiedDate = DateTime.Now;
                    _unitOfWork.FormExamineRepository.Update(_existingExam);

                }


                var _editQueuePoli = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
                if (_editQueuePoli != null)
                {
                    _editQueuePoli.Status = (int)RegistrationStatusEnum.Finish;
                    _editQueuePoli.ModifiedBy = request.Data.Account.UserName;
                    _editQueuePoli.ModifiedDate = DateTime.Now;
                    _unitOfWork.RegistrationRepository.Update(_editQueuePoli);

                }

                int result_affected = _unitOfWork.Save();
                if (result_affected > 0)
                {
                    response.Status = true;
                    response.Message = Messages.LabResultUpdated;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }
            return response;
        }

        public LabResponse GetDataExamine(long poliID)
        {
            var response = new LabResponse { };
            var _getformMedicalId = _unitOfWork.RegistrationRepository.GetById(poliID);
            var _qry = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == _getformMedicalId.FormMedicalID).FirstOrDefault();
            if (_qry != null)
            {
                if (response.Entity == null)
                    response.Entity = new FormExamineLabModel();
                response.Entity.FormExamine = Mapper.Map<FormExamine, FormExamineModel>(_qry);
            }

            return response;
        }

        #region ::Binding::
        public IList<LabItemCategoryModel> GetLaboratoriumCategory(string poliName)
        {
            var _qryPoli = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == poliName);
            IList<LabItemCategoryModel> labCategories = new List<LabItemCategoryModel>();
            var qry = _unitOfWork.LabItemCategoryRepository.Get(x => x.PoliID == _qryPoli.ID);
            foreach (var item in qry)
            {
                var _item = Mapper.Map<LabItemCategory, LabItemCategoryModel>(item);
                labCategories.Add(_item);
            }
            return labCategories;
        }
        #endregion
    }
}
