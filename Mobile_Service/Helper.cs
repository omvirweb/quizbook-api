using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Mobile_Service
{
    public static class Helper
    {
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                T tempT = new T();
                var tType = tempT.GetType();
                List<T> list = new List<T>();
                foreach (var row in table.Rows.Cast<DataRow>())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        var propertyInfo = tType.GetProperty(prop.Name);
                        var rowValue = row[prop.Name];
                        var t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                        try
                        {
                            object safeValue = (rowValue == null || DBNull.Value.Equals(rowValue)) ? null : Convert.ChangeType(rowValue, t);
                            propertyInfo.SetValue(obj, safeValue, null);

                        }
                        catch 
                        {
                            //this write exception to my logger
                            //_logger.Error(ex.Message);
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
    }

    public class Student
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }
    }
}