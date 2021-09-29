using app.domain;
using app.domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace app.data
{
    public class BaseRepositoryModel
    {
        protected string _connectionString { get; set; } = null;
        public BaseRepositoryModel(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected T ReadDataFromDataReader<T>(SqlDataReader dr) where T : BaseModel
        {
            if (!dr.HasRows)
                return null;

            T model = Activator.CreateInstance<T>();
            PropertyInfo[] props = model.GetType().GetProperties();

            while (dr.Read())
            {
                foreach (PropertyInfo prop in props)
                {
                    object value = CheckIfDbNull(dr[prop.Name]);
                    //prop.SetValue(model, Convert.ChangeType(value, prop.PropertyType), null);
                    if (prop != null)
                    {
                        Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        prop.SetValue(model, safeValue, null);
                    }
                }
            }

            return model;
        }
        protected List<T> ReadDataListFromDataReader<T>(SqlDataReader dr, Type type) where T : BaseModel
        {
            List<T> list = new List<T>();
            if (!dr.HasRows) return list;

            T model = default(T);

            PropertyInfo[] props = type.GetProperties();
            while (dr.Read())
            {
                model = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in props)
                {
                    object value = CheckIfDbNull(dr[prop.Name]);

                    if (IsNullableType(prop.PropertyType))
                    {
                        if (value == null) continue;
                        var genericType = prop.PropertyType.GetGenericArguments()[0];
                        prop.SetValue(model, Convert.ChangeType(value, genericType), null);
                    }
                    else
                    {
                        prop.SetValue(model, Convert.ChangeType(value, prop.PropertyType), null);
                    }
                }
                list.Add(model);
            }
            return list;
        }



        protected void Open(SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        protected void Close(SqlConnection connection)
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        protected string GetTableName<T>(T entity) where T : EntityBaseModel
        {
            return typeof(T).Name + "Table";
        }
        protected string GetTableName<T>() where T : EntityBaseModel
        {
            return typeof(T).Name + "Table";
        }
        protected string GetTableName(string name)
        {
            return name + "Table";
        }
        protected object CheckNullSetDBNull(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            if (value.GetType() == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
            {
                return DBNull.Value;
            }

            return value;
        }
        protected object CheckIfDbNull(object value) => value == DBNull.Value ? null : value;
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        //protected object GetValueByType(Type dataType, object propertyValue)
        //{
        //    if (propertyValue == DBNull.Value || propertyValue == null)
        //    {
        //        return null;
        //    }
        //    if (dataType.Equals(typeof(DBNull)))
        //    {
        //        return Convert.ToSingle(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(float)))
        //    {
        //        return Convert.ToSingle(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(double)))
        //    {
        //        return Convert.ToDouble(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(decimal)))
        //    {
        //        return Convert.ToDecimal(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(byte)))
        //    {
        //        return Convert.ToByte(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(string)))
        //    {
        //        return Convert.ToString(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(char)))
        //    {
        //        return Convert.ToChar(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(int)))
        //    {
        //        return Convert.ToInt32(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(long)))
        //    {
        //        return Convert.ToInt64(propertyValue);
        //    }
        //    else
        //    if (dataType.Equals(typeof(short)))
        //    {
        //        return Convert.ToInt16(propertyValue);
        //    }

        //    return null;
        //}
        //public static T ChangeType<T>(object value)
        //{
        //    var t = typeof(T);

        //    if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        //    {
        //        if (value == null)
        //        {
        //            return default(T);
        //        }
        //        t = Nullable.GetUnderlyingType(t);
        //    }

        //    return (T)Convert.ChangeType(value, t);
        //}
    }
}
