using app.domain.Model.Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace app.data
{
    public partial class PromoCodeRepository : BaseRepositoryModel, IPromoCodeRepository
    {
        public PromoCodeRepository(string connectionString) : base(connectionString)
        {
        }

        public string Generate()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    Open(connection);

                    const string sql = @"DECLARE @promoCode varchar(10) = CONVERT(varchar(7), left(newid(),7));
                                         
                                         IF NOT EXISTS(SELECT TOP 1 * FROM PromoCodeTable WHERE Code = @promoCode)
                                         SELECT @promoCode as Code
                                         else
                                         SELECT '' as Code";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                if (dr.Read())
                                {
                                    return dr["Code"].ToString();
                                }
                            }
                        }
                    }

                    return null;
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
    }
}
