using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace CareFusion.Dispensing.Data.Entities
{
    internal static class LinqEntityExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, bool excludePrimaryKeyColumn = false)
        {
            if (collection == null)
                return null;

            // get table attribute
            TableAttribute[] tableAttributes = typeof(T).GetCustomAttributes<TableAttribute>(true);
            if (tableAttributes == null || tableAttributes.Length <= 0)
                return null;
            TableAttribute tableAttribute = tableAttributes[0];

            // get column properties
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();
            properties = properties.Where(p =>
                {
                    ColumnAttribute[] attributes = p.GetCustomAttributes<ColumnAttribute>(true);
                    if (attributes.Length > 0)
                    {
                        // Exclude primary key column if specified. Allows database to use default.
                        if (excludePrimaryKeyColumn && attributes[0].IsPrimaryKey)
                            return false;

                        return true;
                    }
                    return false;
                }).ToArray();

            // create data table
            DataTable table = CreateDataTable<T>(tableAttribute.Name, properties);

            // fill data table
            foreach (T item in collection)
            {
                DataRow row = table.NewRow();
                FillDataRow(row, item, properties);
                table.Rows.Add(row);
            }

            return table;
        }

        private static DataTable CreateDataTable<T>(string tableName, IEnumerable<PropertyInfo> properties)
        {
            DataTable dt = new DataTable(tableName);

            foreach (PropertyInfo property in properties)
            {
                Type type = property.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = type.GetGenericArguments()[0];

                dt.Columns.Add(property.Name, type);
            }

            return dt;
        }

        private static DataRow FillDataRow<T>(DataRow dataRow, T item, IEnumerable<PropertyInfo> properties)
        {
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(item, null);

                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    value == null)
                    value = DBNull.Value;

                dataRow[property.Name] = value;
            }

            return dataRow;
        }
    }
}
