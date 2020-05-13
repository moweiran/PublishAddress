using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ConsoleApp1.DbHelper
{
    public class SqlBulkCopyHelper
    {
        /// <summary>
        /// 批量新增处理方法
        /// 保证传入的泛型类和数据库结构一致（区分大小写）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void CommonBulkCopy<T>(List<T> list, SqlTransaction trans)
        {
            if (trans != null)
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(trans.Connection, SqlBulkCopyOptions.Default, trans))
                {
                    using (var reader = list.GetDataReader<T>())
                    {
                        sqlBulkCopy.DestinationTableName = typeof(T).Name;
                        sqlBulkCopy.BatchSize = 100;
                        sqlBulkCopy.BulkCopyTimeout = 600;
                        foreach (var property in reader.properties)
                        {
                            sqlBulkCopy.ColumnMappings.Add(property.Name, property.Name);
                        }
                        sqlBulkCopy.WriteToServer(reader);
                    }
                }
            }
            else
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(DBHelper.ConnectionString, SqlBulkCopyOptions.Default))
                {
                    using (var reader = list.GetDataReader<T>())
                    {
                        sqlBulkCopy.DestinationTableName = typeof(T).Name;
                        sqlBulkCopy.BatchSize = 100;
                        sqlBulkCopy.BulkCopyTimeout = 600;
                        foreach (var property in reader.properties)
                        {
                            sqlBulkCopy.ColumnMappings.Add(property.Name, property.Name);
                        }
                        sqlBulkCopy.WriteToServer(reader);
                    }
                }
            }
        }
    }
}
