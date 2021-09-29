using app.domain.Exceptions;
using app.domain.Languages;
using app.domain.Model.Collections.Entity;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace app.data
{
    public class EntityRepository : BaseRepositoryModel, IEntityRepository
    {
        public EntityRepository(string connectionString) : base(connectionString)
        {
        }

        public int Create<T>(T entity, string noDublicateColumnName, object noDublicateColumnValue, bool validate) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    //db validations
                    if (validate)
                    {
                        if (CheckExistBy<T>(connection, noDublicateColumnName, noDublicateColumnValue, false))
                            throw new BusinessException(noDublicateColumnName, noDublicateColumnValue + Lang.ErrorIsBusyText);
                    }

                    entity.CreateDate = DateTime.UtcNow;
                    entity.IsDeleted = false;

                    return CreateEntity(connection, entity);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public void UpdateBy<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    UpdateBy<T>(connection, columns, byColumnName, byColumnValue);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public Task UpdateByAsync<T>(Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    UpdateBy<T>(connection, columns, byColumnName, byColumnValue);

                    return Task.Delay(0);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public void UpdateByAll<T>(T entity, string byColumnName, object byColumnValue, bool validate, string noDublicateColumnName, object noDublicateColumnValue) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    //db validations
                    if (validate)
                    {
                        if (CheckExistByNotThis<T>(connection, noDublicateColumnName, noDublicateColumnValue, byColumnName, byColumnValue, false))
                            throw new BusinessException(noDublicateColumnName, noDublicateColumnValue + Lang.ErrorIsBusyText);
                    }

                    UpdateByAll<T>(connection, entity, byColumnName, byColumnValue);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public T DeleteById<T>(int id) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);
                    return DeleteById<T>(connection, id);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public T DeleteBy<T>(string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);
                    return DeleteBy<T>(connection, byColumnName, byColumnValue);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }

        public T GetEntityById<T>(int id) where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);
                    return GetEntityById<T>(connection, id, false);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public T GetEntityBy<T>(Dictionary<string, object> columns, string ascOrDesc = "asc") where T : EntityBaseModel
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);
                    return GetEntityBy<T>(connection, columns, ascOrDesc, false);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }
        }
        public EntityCollection<T> LoadEntitiesByCriteria<T>(BaseCriteriaModel criteriaModel) where T : EntityBaseModel
        {
            EntityCollection<T> entityCollection = new EntityCollection<T>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    Type type = typeof(T);

                    // Collect criterias ===========================
                    //Keyword
                    string Keyword = string.Empty;
                    if (type == typeof(User))
                    {
                        Keyword = string.IsNullOrEmpty(criteriaModel.Keyword) ? string.Empty : @" AND ( Username LIKE @Username OR Fullname LIKE @Fullname ) ";
                    }
                    if (type == typeof(Course))
                    {
                        Keyword = string.IsNullOrEmpty(criteriaModel.Keyword) ? string.Empty :
                            @" AND ( Name LIKE @Name OR Description LIKE @Description OR WhatObjectives LIKE @WhatObjectives OR 
                                     WhatSkills LIKE @WhatSkills OR WhoShouldTake LIKE @WhoShouldTake OR RejectReason LIKE @RejectReason) ";
                    }

                    //IntCriteria
                    string IntCriteria = string.Empty;
                    if (type == typeof(User))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( Role = @Role )";
                    }
                    else
                    if (type == typeof(SubCategory))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( CategoryId = @CategoryId )";
                    }
                    else
                    if (type == typeof(Course))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( UserId = @UserId )";
                    }
                    else
                    if (type == typeof(Video))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( CourseId = @CourseId )";
                    }
                    else
                    if (type == typeof(Section))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( CourseId = @CourseId )";
                    }


                    //IntCriteria2
                    string IntCriteria2 = string.Empty;
                    if (type == typeof(Course))
                    {
                        IntCriteria2 = !criteriaModel.IntCriteria2.HasValue ? string.Empty : @" AND ( Status = @Status )";
                    }


                    //Dates
                    string MinCreateDate = (criteriaModel.MinCreateDate == null || criteriaModel.MinCreateDate.Value == DateTime.MinValue)
                        ? string.Empty : " AND CreateDate >= @MinCreateDate ";
                    string MaxCreateDate = (criteriaModel.MaxCreateDate == null || criteriaModel.MaxCreateDate.Value == DateTime.MinValue)
                        ? string.Empty : " AND CreateDate <= @MaxCreateDate ";


                    //OrderBy =============================
                    string orderBys = string.Empty;
                    if (criteriaModel.OrderByModels != null && criteriaModel.OrderByModels.Count > 0)
                    {
                        orderBys = " ORDER BY ";
                        foreach (var item in criteriaModel.OrderByModels)
                        {
                            orderBys += $" { item.ColumnName } { item.OrderBy.ToString() } , ";
                        }
                    }


                    // Get count ===============================================================================================================
                    if (criteriaModel.WillCount)
                    {
                        string sqlCount = $"SELECT COUNT(*) FROM { GetTableName<T>() } " +
                                          $" WHERE IsDeleted = 0 " + Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate;

                        using (var cmd = new SqlCommand(sqlCount, connection))
                        {
                            cmd.CommandType = CommandType.Text;

                            if (!string.IsNullOrEmpty(Keyword))
                            {
                                if (type == typeof(User))
                                {
                                    cmd.Parameters.AddWithValue("@Username", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@Fullname", "%" + criteriaModel.Keyword + "%");
                                }
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@Name", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@Description", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@WhatObjectives", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@WhatSkills", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@WhoShouldTake", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@RejectReason", "%" + criteriaModel.Keyword + "%");
                                }
                            }

                            if (!string.IsNullOrEmpty(IntCriteria))
                            {
                                if (type == typeof(User))
                                {
                                    cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(SubCategory))
                                {
                                    cmd.Parameters.AddWithValue("@CategoryId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Video))
                                {
                                    cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Section))
                                {
                                    cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                                }
                            }

                            if (!string.IsNullOrEmpty(IntCriteria2))
                            {
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@Status", criteriaModel.IntCriteria2.Value);
                                }
                            }

                            if (!string.IsNullOrEmpty(MinCreateDate)) { cmd.Parameters.AddWithValue("@MinCreateDate", criteriaModel.MinCreateDate); }
                            if (!string.IsNullOrEmpty(MaxCreateDate)) { cmd.Parameters.AddWithValue("@MaxCreateDate", criteriaModel.MaxCreateDate); }

                            entityCollection.AllCount = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }


                    // Get list ===============================================================================================================
                    string sql = string.Empty;
                    if (criteriaModel.Top > 0)
                    {
                        sql = $"SELECT TOP {criteriaModel.Top} * FROM { GetTableName<T>() } " +
                              $"WHERE  IsDeleted = 0 { Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate } " +
                              orderBys.TrimEnd(new char[] { ',', ' ' });
                    }
                    else
                        sql = @"SELECT SOD.* FROM  
                                            ( 
                                                SELECT  *, 
                                                        ROW_NUMBER() OVER (ORDER BY CreateDate DESC) AS RowNum " +
                                                    $"FROM    { GetTableName<T>() } " +
                                                    $"WHERE   IsDeleted = 0 { Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate } " +

                                                ") AS SOD WHERE SOD.RowNum BETWEEN @RowStart AND @RowEnd ORDER BY SOD.Id DESC";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        //Keyword
                        if (!string.IsNullOrEmpty(Keyword))
                        {
                            if (type == typeof(User))
                            {
                                cmd.Parameters.AddWithValue("@Username", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@Fullname", "%" + criteriaModel.Keyword + "%");
                            }
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@Name", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@Description", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@WhatObjectives", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@WhatSkills", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@WhoShouldTake", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@RejectReason", "%" + criteriaModel.Keyword + "%");
                            }
                        }

                        //IntCriteria
                        if (!string.IsNullOrEmpty(IntCriteria))
                        {
                            if (type == typeof(User))
                            {
                                cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(SubCategory))
                            {
                                cmd.Parameters.AddWithValue("@CategoryId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@UserId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Video))
                            {
                                cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Section))
                            {
                                cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                            }
                        }

                        //IntCriteria2
                        if (!string.IsNullOrEmpty(IntCriteria2))
                        {
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@Status", criteriaModel.IntCriteria2.Value);
                            }
                        }


                        //Dates
                        if (!string.IsNullOrEmpty(MinCreateDate)) { cmd.Parameters.AddWithValue("@MinCreateDate", criteriaModel.MinCreateDate); }
                        if (!string.IsNullOrEmpty(MaxCreateDate)) { cmd.Parameters.AddWithValue("@MaxCreateDate", criteriaModel.MaxCreateDate); }

                        if (criteriaModel.Top <= 0)
                        {
                            cmd.Parameters.AddWithValue("@RowStart", ((criteriaModel.PageNumber - 1) * criteriaModel.RowsPerPage) + 1);
                            cmd.Parameters.AddWithValue("@RowEnd", criteriaModel.RowsPerPage * criteriaModel.PageNumber);
                        }


                        using (var dr = cmd.ExecuteReader())
                        {
                            entityCollection.Items = ReadDataListFromDataReader<T>(dr, typeof(T));
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }

            return entityCollection;
        }
        public EntityCollection<T> LoadTopEntitiesByCriteria<T>(BaseCriteriaModel criteriaModel) where T : EntityBaseModel
        {
            EntityCollection<T> entityCollection = new EntityCollection<T>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    Type type = typeof(T);

                    // Collect criterias ===========================
                    //Keyword
                    string Keyword = string.Empty;
                    if (type == typeof(User))
                    {
                        Keyword = string.IsNullOrEmpty(criteriaModel.Keyword) ? string.Empty : @" AND ( Username LIKE @Username OR Fullname LIKE @Fullname ) ";
                    }
                    if (type == typeof(Course))
                    {
                        Keyword = string.IsNullOrEmpty(criteriaModel.Keyword) ? string.Empty :
                            @" AND ( Name LIKE @Name OR Description LIKE @Description OR WhatObjectives LIKE @WhatObjectives OR 
                                     WhatSkills LIKE @WhatSkills OR WhoShouldTake LIKE @WhoShouldTake OR RejectReason LIKE @RejectReason) ";
                    }

                    //IntCriteria
                    string IntCriteria = string.Empty;
                    if (type == typeof(User))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( Role = @Role )";
                    }
                    else
                    if (type == typeof(SubCategory))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( CategoryId = @CategoryId )";
                    }
                    else
                    if (type == typeof(Course))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( UserId = @UserId )";
                    }
                    else
                    if (type == typeof(Video))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( CourseId = @CourseId )";
                    }
                    else
                    if (type == typeof(Section))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( CourseId = @CourseId )";
                    }

                    //IntCriteria2
                    string IntCriteria2 = string.Empty;
                    if (type == typeof(Course))
                    {
                        IntCriteria2 = !criteriaModel.IntCriteria2.HasValue ? string.Empty : @" AND ( Status = @Status )";
                    }


                    //Dates
                    string MinCreateDate = (criteriaModel.MinCreateDate == null || criteriaModel.MinCreateDate.Value == DateTime.MinValue)
                        ? string.Empty : " AND CreateDate >= @MinCreateDate ";
                    string MaxCreateDate = (criteriaModel.MaxCreateDate == null || criteriaModel.MaxCreateDate.Value == DateTime.MinValue)
                        ? string.Empty : " AND CreateDate <= @MaxCreateDate ";


                    //Top
                    string Top = string.Empty;
                    if (criteriaModel.Top == 0)
                    {
                        Top = !criteriaModel.IntCriteria2.HasValue ? string.Empty : $" TOP Status = { criteriaModel.Top } ";
                    }


                    // Get count ============================================================================================================
                    if (criteriaModel.WillCount)
                    {
                        string sqlCount = $"SELECT COUNT(*) FROM { GetTableName<T>() } " +
                                          $" WHERE IsDeleted = 0 " + Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate;

                        using (var cmd = new SqlCommand(sqlCount, connection))
                        {
                            cmd.CommandType = CommandType.Text;

                            if (!string.IsNullOrEmpty(Keyword))
                            {
                                if (type == typeof(User))
                                {
                                    cmd.Parameters.AddWithValue("@Username", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@Fullname", "%" + criteriaModel.Keyword + "%");
                                }
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@Name", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@Description", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@WhatObjectives", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@WhatSkills", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@WhoShouldTake", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@RejectReason", "%" + criteriaModel.Keyword + "%");
                                }
                            }

                            if (!string.IsNullOrEmpty(IntCriteria))
                            {
                                if (type == typeof(User))
                                {
                                    cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(SubCategory))
                                {
                                    cmd.Parameters.AddWithValue("@CategoryId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Video))
                                {
                                    cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Section))
                                {
                                    cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                                }
                            }

                            if (!string.IsNullOrEmpty(IntCriteria2))
                            {
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@Status", criteriaModel.IntCriteria2.Value);
                                }
                            }

                            if (!string.IsNullOrEmpty(MinCreateDate)) { cmd.Parameters.AddWithValue("@MinCreateDate", criteriaModel.MinCreateDate); }
                            if (!string.IsNullOrEmpty(MaxCreateDate)) { cmd.Parameters.AddWithValue("@MaxCreateDate", criteriaModel.MaxCreateDate); }

                            entityCollection.AllCount = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }


                    // Get list ===========================
                    string sql = @"SELECT SOD.* FROM  
                                                  ( SELECT  *, 
                                                            ROW_NUMBER() OVER (ORDER BY CreateDate DESC) AS RowNum " +
                                                    $"FROM    { GetTableName<T>() } " +
                                                    $"WHERE   IsDeleted = 0 { Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate } " +
                                                          ") AS SOD WHERE SOD.RowNum BETWEEN @RowStart AND @RowEnd ORDER BY SOD.Id DESC";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (!string.IsNullOrEmpty(Keyword))
                        {
                            if (type == typeof(User))
                            {
                                cmd.Parameters.AddWithValue("@Username", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@Fullname", "%" + criteriaModel.Keyword + "%");
                            }
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@Name", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@Description", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@WhatObjectives", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@WhatSkills", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@WhoShouldTake", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@RejectReason", "%" + criteriaModel.Keyword + "%");
                            }
                        }

                        if (!string.IsNullOrEmpty(IntCriteria))
                        {
                            if (type == typeof(User))
                            {
                                cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(SubCategory))
                            {
                                cmd.Parameters.AddWithValue("@CategoryId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@UserId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Video))
                            {
                                cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Section))
                            {
                                cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                            }
                        }


                        if (!string.IsNullOrEmpty(IntCriteria2))
                        {
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@Status", criteriaModel.IntCriteria2.Value);
                            }
                        }


                        if (!string.IsNullOrEmpty(MinCreateDate)) { cmd.Parameters.AddWithValue("@MinCreateDate", criteriaModel.MinCreateDate); }
                        if (!string.IsNullOrEmpty(MaxCreateDate)) { cmd.Parameters.AddWithValue("@MaxCreateDate", criteriaModel.MaxCreateDate); }

                        cmd.Parameters.AddWithValue("@RowStart", ((criteriaModel.PageNumber - 1) * criteriaModel.RowsPerPage) + 1);
                        cmd.Parameters.AddWithValue("@RowEnd", criteriaModel.RowsPerPage * criteriaModel.PageNumber);

                        using (var dr = cmd.ExecuteReader())
                        {
                            entityCollection.Items = ReadDataListFromDataReader<T>(dr, typeof(T));
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }

            return entityCollection;
        }
        public EntityViewCollection<V> LoadEntityViewModelsByCriteria<V, E>(BaseCriteriaModel criteriaModel) where V : EntityBaseModel where E : EntityBaseModel
        {
            EntityViewCollection<V> entityCollection = new EntityViewCollection<V>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    Type type = typeof(V);

                    // Collect criterias ===========================
                    string Keyword = string.Empty;
                    if (type == typeof(User))
                    {
                        Keyword = string.IsNullOrEmpty(criteriaModel.Keyword) ? string.Empty : @" AND ( M.Username LIKE @Username OR M.Fullname LIKE @Fullname )";
                    }

                    string IntCriteria = string.Empty;
                    if (type == typeof(User))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( M.Role = @Role )";
                    }
                    else
                    if (type == typeof(SubCategory))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( M.CategoryId = @CategoryId )";
                    }
                    else
                    if (type == typeof(Course))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( M.UserId = @UserId )";
                    }
                    if (type == typeof(Video))
                    {
                        IntCriteria = !criteriaModel.IntCriteria.HasValue ? string.Empty : @" AND ( M.CourseId = @CourseId )";
                    }

                    string IntCriteria2 = string.Empty;
                    if (type == typeof(Course))
                    {
                        IntCriteria2 = !criteriaModel.IntCriteria2.HasValue ? string.Empty : @" AND ( M.Status = @Status )";
                    }

                    string MinCreateDate = (criteriaModel.MinCreateDate == null || criteriaModel.MinCreateDate.Value == DateTime.MinValue)
                        ? string.Empty : " AND M.CreateDate >= @MinCreateDate ";
                    string MaxCreateDate = (criteriaModel.MaxCreateDate == null || criteriaModel.MaxCreateDate.Value == DateTime.MinValue)
                        ? string.Empty : " AND M.CreateDate <= @MaxCreateDate ";

                    // Get count ===========================
                    if (criteriaModel.WillCount)
                    {
                        string sqlCount = $" SELECT COUNT(*) FROM { GetTableName<E>() } AS M " +
                                          $" WHERE M.IsDeleted = 0 " + Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate;

                        using (var cmd = new SqlCommand(sqlCount, connection))
                        {
                            cmd.CommandType = CommandType.Text;

                            if (!string.IsNullOrEmpty(Keyword))
                            {
                                if (type == typeof(User))
                                {
                                    cmd.Parameters.AddWithValue("@Username", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@Fullname", "%" + criteriaModel.Keyword + "%");
                                    cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                                }
                            }

                            if (!string.IsNullOrEmpty(IntCriteria))
                            {
                                if (type == typeof(User))
                                {
                                    cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(SubCategory))
                                {
                                    cmd.Parameters.AddWithValue("@CategoryId", criteriaModel.IntCriteria.Value);
                                }
                                else
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", criteriaModel.IntCriteria.Value);
                                }
                                if (type == typeof(Video))
                                {
                                    cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                                }
                            }


                            if (!string.IsNullOrEmpty(IntCriteria2))
                            {
                                if (type == typeof(Course))
                                {
                                    cmd.Parameters.AddWithValue("@Status", criteriaModel.IntCriteria2.Value);
                                }
                            }


                            if (!string.IsNullOrEmpty(MinCreateDate)) { cmd.Parameters.AddWithValue("@MinCreateDate", criteriaModel.MinCreateDate); }
                            if (!string.IsNullOrEmpty(MaxCreateDate)) { cmd.Parameters.AddWithValue("@MaxCreateDate", criteriaModel.MaxCreateDate); }

                            entityCollection.AllCount = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }

                    //join part =============================
                    string joinColumns = string.Empty;
                    if (criteriaModel.LeftJoinModels != null && criteriaModel.LeftJoinModels.Count > 0)
                    {
                        foreach (var item in criteriaModel.LeftJoinModels)
                        {
                            joinColumns += item.Alias + "." + item.TakeHelperColumn + "  AS " + item.AsResultColumn + ", ";
                        }
                    }

                    //join tables =============================
                    string joinTables = string.Empty;
                    if (criteriaModel.LeftJoinModels != null && criteriaModel.LeftJoinModels.Count > 0)
                    {
                        foreach (var item in criteriaModel.LeftJoinModels)
                        {
                            joinTables += $" LEFT JOIN { GetTableName(item.HelperEntityName)  } AS { item.Alias } on {item.Alias}.{item.JoinHelperColumn} = M.{item.JoinMainColumn }  ";
                        }
                    }

                    // Get list ===========================
                    string sql = @"SELECT SOD.* FROM  
                                                  ( SELECT  M.*, " +
                                                                    joinColumns
                                                                   +
                                                             @"ROW_NUMBER() OVER (ORDER BY M.Id DESC) AS RowNum " +
                                                    $" FROM    { GetTableName<E>() } AS M "
                                                    +
                                                    joinTables
                                                    +
                                                    $" WHERE   M.IsDeleted = 0 { Keyword + IntCriteria + IntCriteria2 + MinCreateDate + MaxCreateDate } " +
                                                          ") AS SOD WHERE SOD.RowNum BETWEEN @RowStart AND @RowEnd ORDER BY SOD.Id DESC";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (!string.IsNullOrEmpty(Keyword))
                        {
                            if (type == typeof(User))
                            {
                                cmd.Parameters.AddWithValue("@Username", "%" + criteriaModel.Keyword + "%");
                                cmd.Parameters.AddWithValue("@Fullname", "%" + criteriaModel.Keyword + "%");
                            }
                        }

                        if (!string.IsNullOrEmpty(IntCriteria))
                        {
                            if (type == typeof(User))
                            {
                                cmd.Parameters.AddWithValue("@Role", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(SubCategory))
                            {
                                cmd.Parameters.AddWithValue("@CategoryId", criteriaModel.IntCriteria.Value);
                            }
                            else
                            if (type == typeof(Course))
                            {
                                cmd.Parameters.AddWithValue("@UserId", criteriaModel.IntCriteria.Value);
                            }
                            if (type == typeof(Video))
                            {
                                cmd.Parameters.AddWithValue("@CourseId", criteriaModel.IntCriteria.Value);
                            }
                        }

                        if (!string.IsNullOrEmpty(MinCreateDate)) { cmd.Parameters.AddWithValue("@MinCreateDate", criteriaModel.MinCreateDate); }
                        if (!string.IsNullOrEmpty(MaxCreateDate)) { cmd.Parameters.AddWithValue("@MaxCreateDate", criteriaModel.MaxCreateDate); }

                        cmd.Parameters.AddWithValue("@RowStart", ((criteriaModel.PageNumber - 1) * criteriaModel.RowsPerPage) + 1);
                        cmd.Parameters.AddWithValue("@RowEnd", criteriaModel.RowsPerPage * criteriaModel.PageNumber);

                        using (var dr = cmd.ExecuteReader())
                        {
                            entityCollection.Items = ReadDataListFromDataReader<V>(dr, typeof(V));
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Close(connection);
                }
            }

            return entityCollection;
        }

        //repo
        protected int CreateEntity<T>(SqlConnection connnection, T entity) where T : EntityBaseModel
        {
            List<PropertyInfo> props = entity.GetType().GetProperties().ToList();

            //read columns and values
            string columns = string.Empty;
            string values = string.Empty;
            foreach (var prop in props)
            {
                if (prop.Name != "Id")
                {
                    columns += prop.Name + ", ";
                    values += "@" + prop.Name + ", ";
                }
            }

            string sql = $"INSERT INTO { GetTableName(entity) } ( { columns.TrimEnd(new char[] { ',', ' ' }) } ) VALUES ( { values.TrimEnd(new char[] { ',', ' ' }) } ); SELECT SCOPE_IDENTITY();";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                foreach (var prop in props)
                {
                    if (prop.Name != "Id")
                    {
                        cmd.Parameters.AddWithValue("@" + prop.Name, CheckNullSetDBNull(prop.GetValue(entity, null)));
                    }
                }
                return Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }
        }
        protected void UpdateBy<T>(SqlConnection connnection, Dictionary<string, object> columns, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            string sqlPart = string.Empty;
            foreach (KeyValuePair<string, object> pair in columns)
            {
                sqlPart += pair.Key + "  = @" + pair.Key + ", ";
            }

            string sql = $"UPDATE { GetTableName<T>() } SET " + sqlPart.TrimEnd(new char[] { ',', ' ' }) + $" WHERE { byColumnName } = @{byColumnName } ";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                foreach (KeyValuePair<string, object> pair in columns)
                {
                    cmd.Parameters.AddWithValue("@" + pair.Key, CheckNullSetDBNull(pair.Value));
                }
                cmd.Parameters.AddWithValue("@" + byColumnName, CheckNullSetDBNull(byColumnValue));
                cmd.ExecuteNonQuery();
            }
        }
        protected void UpdateByAll<T>(SqlConnection connnection, T entity, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            List<PropertyInfo> props = entity.GetType().GetProperties().ToList();

            //read columns and values
            string columns = string.Empty;
            foreach (var prop in props)
            {
                if (prop.Name != "Id" && prop.Name != "CreateDate" && prop.Name != "IsDeleted")
                {
                    columns += prop.Name + " =  @" + prop.Name + ", ";
                }
            }

            string sql = $"UPDATE { GetTableName(entity) } SET " + columns.TrimEnd(new char[] { ',', ' ' }) + $" WHERE { byColumnName } = @{byColumnName } ";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                foreach (var prop in props)
                {
                    if (prop.Name != "Id" && prop.Name != "CreateDate" && prop.Name != "IsDeleted")
                    {
                        cmd.Parameters.AddWithValue("@" + prop.Name, CheckNullSetDBNull(prop.GetValue(entity, null)));
                    }
                }
                cmd.Parameters.AddWithValue("@" + byColumnName, CheckNullSetDBNull(byColumnValue));
                cmd.ExecuteNonQuery();
            }
        }
        protected T GetEntityById<T>(SqlConnection connnection, int id, bool isDeleted) where T : EntityBaseModel
        {
            string sql = $"SELECT TOP 1 * FROM { GetTableName<T>() } WHERE Id = @Id AND IsDeleted = @IsDeleted";

            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                using (var dr = cmd.ExecuteReader())
                {
                    return ReadDataFromDataReader<T>(dr);
                }
            }
        }
        protected T GetEntityBy<T>(SqlConnection connnection, Dictionary<string, object> columns, string ascOrDesc, bool isDeleted) where T : EntityBaseModel
        {
            string sqlPart = string.Empty;
            foreach (KeyValuePair<string, object> pair in columns)
            {
                sqlPart += " " + pair.Key + " = @" + pair.Key + " AND";
            }

            string sql = $"SELECT TOP 1 * FROM { GetTableName<T>() } WHERE " + sqlPart + " IsDeleted = @IsDeleted order by Id " + ascOrDesc;
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                foreach (KeyValuePair<string, object> pair in columns)
                {
                    cmd.Parameters.AddWithValue("@" + pair.Key, CheckNullSetDBNull(pair.Value));
                }
                cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                using (var dr = cmd.ExecuteReader())
                {
                    return ReadDataFromDataReader<T>(dr);
                }
            }
        }
        protected T GetEntityByAndBy<T>(SqlConnection connnection, string byColumn1, string byColumn2, string value1, string value2, bool isDeleted) where T : EntityBaseModel
        {
            string sql = $"SELECT TOP 1 * FROM { GetTableName<T>() } WHERE " + byColumn1 + " = @" + value1 + " AND " + byColumn2 + " = @" + value2 + " AND IsDeleted = @IsDeleted";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@" + byColumn1, CheckNullSetDBNull(value1));
                cmd.Parameters.AddWithValue("@" + byColumn2, CheckNullSetDBNull(value2));
                cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                using (var dr = cmd.ExecuteReader())
                {
                    return ReadDataFromDataReader<T>(dr);
                }
            }
        }
        protected List<T> LaodAllEntities<T>(SqlConnection connnection, bool isDeleted) where T : EntityBaseModel
        {
            string sql = $"SELECT * FROM { GetTableName<T>() } WHERE IsDeleted = @IsDeleted";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                using (var dr = cmd.ExecuteReader())
                {
                    return ReadDataListFromDataReader<T>(dr, typeof(T));
                }
            }
        }
        protected T DeleteBy<T>(SqlConnection connnection, string byColumnName, object byColumnValue) where T : EntityBaseModel
        {
            string sql = $"UPDATE { GetTableName<T>() } SET IsDeleted = @IsDeleted " + $" WHERE { byColumnName } = @{byColumnName } ";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IsDeleted", true);
                cmd.Parameters.AddWithValue("@" + byColumnName, CheckNullSetDBNull(byColumnValue));
                using (var dr = cmd.ExecuteReader())
                {
                    return ReadDataFromDataReader<T>(dr);
                }
            }
        }
        protected T DeleteById<T>(SqlConnection connnection, int id) where T : EntityBaseModel
        {
            string sql = $"UPDATE { GetTableName<T>() } SET IsDeleted = @IsDeleted WHERE Id = @Id ";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IsDeleted", true);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var dr = cmd.ExecuteReader())
                {
                    return ReadDataFromDataReader<T>(dr);
                }
            }
        }
        private bool CheckExistBy<T>(SqlConnection connnection, string byColumnName, object byColumnValue, bool isDeleted) where T : EntityBaseModel
        {
            string sql = $"SELECT TOP 1 * FROM { GetTableName<T>() } WHERE " + byColumnName + " = @" + byColumnName + " AND IsDeleted = @IsDeleted";
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@" + byColumnName, CheckNullSetDBNull(byColumnValue));
                cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                        return true;
                    else
                        return false;
                }
            }
        }
        private bool CheckExistByNotThis<T>(SqlConnection connnection, string byColumnName, object byColumnValue, string notThisName, object notThisValue, bool isDeleted) where T : EntityBaseModel
        {
            string sql = $"SELECT TOP 1 * FROM { GetTableName<T>() } WHERE " + byColumnName + " = @" + byColumnName + " AND IsDeleted = @IsDeleted AND " + notThisName + " != @" + notThisName;
            using (var cmd = new SqlCommand(sql, connnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@" + byColumnName, CheckNullSetDBNull(byColumnValue));
                cmd.Parameters.AddWithValue("@" + notThisName, CheckNullSetDBNull(notThisValue));
                cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                        return true;
                    else
                        return false;
                }
            }
        }

        //public Task UpdateByIdAsync<T>(T entity, Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            Open(connection);

        //            //validation is here

        //            UpdateById<T>(connection, entity, columns, id);

        //            return Task.Delay(0);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            Close(connection);
        //        }
        //    }
        //}
        //public Task UpdateByIdAsync<T>(Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel
        //{
        //    //DB Validations

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            Open(connection);

        //            //validation is here

        //            UpdateById<T>(connection, columns, id);

        //            return Task.Delay(0);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            Close(connection);
        //        }
        //    }
        //}

        //public void UpdateById<T>(T entity, Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel
        //{
        //    //DB Validations
        //    if (typeof(T) == typeof(User))
        //    {
        //        //Validator.Validate((User)(object)entity, "create");
        //    }
        //    else
        //    if (typeof(T) == typeof(Issue))
        //    {
        //        Validator.Validate((Issue)(object)entity, "create");
        //    }

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            Open(connection);

        //            //validation is here

        //            UpdateById<T>(connection, entity, GetTableName<T>(), columns, id);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            Close(connection);
        //        }
        //    }
        //}
        //public void UpdateById<T>(Dictionary<string, object> columns, int id, bool validate) where T : EntityBaseModel
        //{
        //    //DB Validations

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            Open(connection);

        //            //validation is here

        //            UpdateById<T>(connection, columns, id);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            Close(connection);
        //        }
        //    }
        //}

        //


        //protected T UpdateById<T>(SqlConnection connnection, T entity, string tableName, string columnName, object columnValue, int id) where T : EntityBaseModel
        //{
        //    string sql = $"UPDATE { tableName } SET " + columnName + " = @" + columnValue + " WHERE Id = @Id ";
        //    using (var cmd = new SqlCommand(sql, connnection))
        //    {
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@" + columnName, CheckNullSetDBNull(columnValue));
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        using (var dr = cmd.ExecuteReader())
        //        {
        //            return ReadDataFromDataReader<T>(dr);
        //        }
        //    }

        //}


        //public Task UpdateById<T>(T entity, string columnName, object columnValue, int id, bool validate) where T : EntityBaseModel
        //{
        //    //EntityBaseModel baseModel;

        //    //DB Validations
        //    if (typeof(T) == typeof(User))
        //    {
        //        //Validator.Validate((User)(object)entity, "create");
        //    }
        //    else
        //    if (typeof(T) == typeof(Issue))
        //    {
        //        Validator.Validate((Issue)(object)entity, "create");
        //    }

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            Open(connection);

        //            //validation is here

        //            UpdateById<T>(connection, entity, GetTableName<T>(), columnName, columnValue, id);

        //            return Task.Delay(0);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            Close(connection);
        //        }
        //    }
        //}
        //protected void UpdateById<T>(SqlConnection connnection, T entity, Dictionary<string, object> columns, int id) where T : EntityBaseModel
        //{
        //    string sqlPart = string.Empty;
        //    foreach (KeyValuePair<string, object> pair in columns)
        //    {
        //        sqlPart += pair.Key + "  = @" + pair.Key + ", ";
        //    }

        //    string sql = $"UPDATE { GetTableName(entity) } SET " + sqlPart.TrimEnd(new char[] { ',', ' ' }) + " WHERE Id = @Id ";
        //    using (var cmd = new SqlCommand(sql, connnection))
        //    {
        //        cmd.CommandType = CommandType.Text;

        //        foreach (KeyValuePair<string, object> pair in columns)
        //        {
        //            cmd.Parameters.AddWithValue("@" + pair.Key, CheckNullSetDBNull(pair.Value));
        //        }
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
        //protected void UpdateById<T>(SqlConnection connnection, Dictionary<string, object> columns, int id) where T : EntityBaseModel
        //{
        //    string sqlPart = string.Empty;
        //    foreach (KeyValuePair<string, object> pair in columns)
        //    {
        //        sqlPart += pair.Key + "  = @" + pair.Key + ", ";
        //    }

        //    string sql = $"UPDATE { GetTableName<T>() } SET " + sqlPart.TrimEnd(new char[] { ',', ' ' }) + " WHERE Id = @Id ";
        //    using (var cmd = new SqlCommand(sql, connnection))
        //    {
        //        cmd.CommandType = CommandType.Text;

        //        foreach (KeyValuePair<string, object> pair in columns)
        //        {
        //            cmd.Parameters.AddWithValue("@" + pair.Key, CheckNullSetDBNull(pair.Value));
        //        }
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
    }
}
