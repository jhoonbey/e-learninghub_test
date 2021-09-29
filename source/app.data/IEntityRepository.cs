using app.domain.Model.Collections.Entity;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.data
{
    public interface IEntityRepository : IRepository
    {
        int Create<T>(T entity, string noDublicateColumnName, object noDublicateColumnValue, bool validate) where T : EntityBaseModel;
        void UpdateBy<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel;
        Task UpdateByAsync<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel;
        void UpdateByAll<T>(T entity, string byColumnName, object byColumnValue, bool validate, string noDublicateColumnName, object noDublicateColumnValue) where T : EntityBaseModel;
        T DeleteById<T>(int id) where T : EntityBaseModel;
        T DeleteBy<T>(string byColumnName, object byColumnValue) where T : EntityBaseModel;

        //Task UpdateByIdAsync<T>(T entity, Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel;
        //void UpdateById<T>(T entity, Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel;
        //Task UpdateByIdAsync<T>(Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel;
        //void UpdateById<T>(Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel;
        //void UpdateBy<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue, bool validate) where T : EntityBaseModel;

        T GetEntityById<T>(int id) where T : EntityBaseModel;
        T GetEntityBy<T>(Dictionary<string, object> columns, string ascOrDesc = "asc") where T : EntityBaseModel;
        EntityCollection<T> LoadEntitiesByCriteria<T>(BaseCriteriaModel criteriaModel) where T : EntityBaseModel;
        EntityViewCollection<V> LoadEntityViewModelsByCriteria<V, E>(BaseCriteriaModel criteriaModel) where V : EntityBaseModel where E : EntityBaseModel;
    }
}
