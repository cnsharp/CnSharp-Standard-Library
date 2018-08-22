using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CnSharp.Extensions
{
    public static class MappingHelper
    {
        public static void AssignTo(this object @from, object to, params string[] skips)
        {
            var type = @from.GetType();
            var toType = to.GetType();
            var toProps = toType.GetProperties();

            foreach (var property in type.GetProperties())
            {
                if (skips != null && skips.Contains(property.Name))
                    continue;

                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                    continue;

                if (property.CanRead)
                {
                    foreach (var toProp in toProps)
                    {

                        if (toProp.Name == property.Name && toProp.CanWrite)
                        {
                            toProp.SetValue(to, property.GetValue(@from, null), null);
                            break;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> ToEntities<T>(this IDataReader dr) where T :  class
        {
            var props = typeof (T).GetProperties();
            var dict = new Dictionary<int, PropertyInfo>();

            for (var i = 0; i < dr.FieldCount; i++)
            {
                var name = dr.GetName(i);
                var propertyInfo = (props.FirstOrDefault(p => String.Compare(name, p.Name, StringComparison.OrdinalIgnoreCase) == 0));
                 if(propertyInfo != null)
                     dict.Add(i,propertyInfo);
            }

            while (dr.Read())
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var pair in dict)
                {
                    pair.Value.SetValue(obj,Convert.ChangeType(dr.GetValue(pair.Key),pair.Value.PropertyType),null);
                }
                yield return obj;
            }
        } 
    }
}
