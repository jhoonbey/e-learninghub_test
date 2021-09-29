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
    public interface IEntityService
    {
        GenericServiceResponse<int> Create<T>(T entity, string noDublicateColumnName, object noDublicateColumnValue, bool validate) where T : EntityBaseModel;
        VoidServiceResponse UpdateBy<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel;
        Task<VoidServiceResponse> UpdateByAsync<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel;
        VoidServiceResponse UpdateByAll<T>(T entity, string byColumnName, object byColumnValue, bool validate, string noDublicateColumnName, object noDublicateColumnValue) where T : EntityBaseModel;
        VoidServiceResponse DeleteById<T>(int id) where T : EntityBaseModel;
        VoidServiceResponse DeleteBy<T>(string byColumnName, object byColumnValue) where T : EntityBaseModel;

        //VoidServiceResponse UpdateById<T>(T entity, Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel;
        //Task<VoidServiceResponse> UpdateByIdAsync<T>(T entity, Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel;
        GenericServiceResponse<List<EnumModel>> ConvertEnumToEnumModelList<T>(List<int> excludeIdList) where T : Enum;

        GenericServiceResponse<T> GetEntityById<T>(int id) where T : EntityBaseModel;
        GenericServiceResponse<T> GetEntityBy<T>(Dictionary<string, object> columns) where T : EntityBaseModel;
        GenericServiceResponse<EntityCollection<T>> LoadEntitiesByCriteria<T>(BaseCriteriaModel criteriaModel) where T : EntityBaseModel;
        GenericServiceResponse<EntityViewCollection<V>> LoadEntityViewModelsByCriteria<V, E>(BaseCriteriaModel criteriaModel) where V : EntityBaseModel where E : EntityBaseModel;
    }
}