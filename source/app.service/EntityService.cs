using app.data;
using app.domain.Exceptions;
using app.domain.Model.Collections.Entity;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service.Model.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.service
{
    public class EntityService : IEntityService
    {
        private readonly IEntityRepository _entityRepository;
        public EntityService(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public GenericServiceResponse<int> Create<T>(T entity, string noDublicateColumnName, object noDublicateColumnValue, bool validate) where T : EntityBaseModel
        {
            var response = new GenericServiceResponse<int>();
            try
            {
                response.Model = _entityRepository.Create<T>(entity, noDublicateColumnName, noDublicateColumnValue, validate);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public VoidServiceResponse UpdateBy<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            var response = new VoidServiceResponse();
            try
            {
                _entityRepository.UpdateBy<T>(columns, byColumnName, byColumnValue);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public async Task<VoidServiceResponse> UpdateByAsync<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            var response = new VoidServiceResponse();
            try
            {
                await _entityRepository.UpdateByAsync<T>(columns, byColumnName, byColumnValue);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public VoidServiceResponse UpdateByAll<T>(T entity, string byColumnName, object byColumnValue, bool validate, string noDublicateColumnName, object noDublicateColumnValue) where T : EntityBaseModel
        {
            var response = new VoidServiceResponse();
            try
            {
                _entityRepository.UpdateByAll<T>(entity, byColumnName, byColumnValue, validate, noDublicateColumnName, noDublicateColumnValue);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public VoidServiceResponse DeleteById<T>(int id) where T : EntityBaseModel
        {
            var response = new VoidServiceResponse();
            try
            {
                _entityRepository.DeleteById<T>(id);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public VoidServiceResponse DeleteBy<T>(string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            var response = new VoidServiceResponse();
            try
            {
                _entityRepository.DeleteBy<T>(byColumnName, byColumnValue);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public GenericServiceResponse<List<EnumModel>> ConvertEnumToEnumModelList<T>(List<int> excludeIdList) where T : Enum
        {
            var response = new GenericServiceResponse<List<EnumModel>>();
            try
            {
                List<EnumModel> result = new List<EnumModel>();
                var all = Enum.GetValues(typeof(T));
                foreach (var value in all)
                {
                    int id = (int)value;
                    T enumOwn = (T)value;
                    if (excludeIdList == null || !excludeIdList.Contains(id))
                    {
                        result.Add(new EnumModel() { Id = id, Name = enumOwn.ToString() });
                    }
                }

                response.Model = result;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public GenericServiceResponse<T> GetEntityById<T>(int id) where T : EntityBaseModel
        {
            var response = new GenericServiceResponse<T>();
            try
            {
                response.Model = _entityRepository.GetEntityById<T>(id);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public GenericServiceResponse<T> GetEntityBy<T>(Dictionary<string, object> columns) where T : EntityBaseModel
        {
            var response = new GenericServiceResponse<T>();
            try
            {
                response.Model = _entityRepository.GetEntityBy<T>(columns);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public GenericServiceResponse<EntityCollection<T>> LoadEntitiesByCriteria<T>(BaseCriteriaModel criteriaModel) where T : EntityBaseModel
        {
            var response = new GenericServiceResponse<EntityCollection<T>>();
            try
            {
                response.Model = _entityRepository.LoadEntitiesByCriteria<T>(criteriaModel);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
        public GenericServiceResponse<EntityViewCollection<V>> LoadEntityViewModelsByCriteria<V, E>(BaseCriteriaModel criteriaModel) where V : EntityBaseModel where E : EntityBaseModel
        {
            var response = new GenericServiceResponse<EntityViewCollection<V>>();
            try
            {
                response.Model = _entityRepository.LoadEntityViewModelsByCriteria<V, E>(criteriaModel);
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
    }
}
