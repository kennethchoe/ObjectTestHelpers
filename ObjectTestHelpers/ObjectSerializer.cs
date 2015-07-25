using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectTestHelpers
{
    public class ObjectSerializer
    {
        public List<string> SerializeForComparison(object obj)
        {
            var result = new List<string>();
            SerializeForComparisonRecursively("", result, obj);
            return result;
        }

        private void SerializeForComparisonRecursively(string pathToContainer, List<string> result, object value)
        {
            if (value == null)
            {
                result.Add(pathToContainer + " - null");
                return;
            }

            if (IsSimpleType(value.GetType()))
            {
                result.Add(pathToContainer + " - " + value);
                return;
            }

            if (IsDictionary(value))
            {
                var dict = (IDictionary)value;

                if (dict.Count == 0)
                    result.Add(pathToContainer + " - no elements found");
                else
                {
                    foreach (var key in dict.Keys)
                    {
                        SerializeForComparisonRecursively(pathToContainer + " " + key, result, dict[key]);
                    }
                }
                return;
            }

            if (IsList(value))
            {
                var list = (IList)value;

                if (list.Count == 0)
                    result.Add(pathToContainer + " - no elements found");
                else
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        SerializeForComparisonRecursively(pathToContainer + " " + i, result, list[i]);
                    }
                }
                return;
            }

            LoopThroughProperties(pathToContainer, result, value);
            LoopThroughFields(pathToContainer, result, value);
        }

        private void LoopThroughProperties(string pathToContainer, List<string> result, object obj)
        {
            var propertyInfos = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in propertyInfos)
            {
                PropertyInfo info = propertyInfo;

                if (!info.CanRead) continue;

                SerializeForComparisonRecursively(pathToContainer + "/" + propertyInfo.Name, result, info.GetValue(obj, null));
            }
        }

        private void LoopThroughFields(string pathToContainer, List<string> result, object obj)
        {
            var fieldInfos = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var fieldInfo in fieldInfos)
            {
                FieldInfo info = fieldInfo;
                SerializeForComparisonRecursively(pathToContainer + "/" + fieldInfo.Name, result, info.GetValue(obj));
            }
        }

        private bool IsDictionary(object value)
        {
            var dict = value as IDictionary;
            if (dict == null) return false;

            return true;
        }

        private bool IsSimpleType(Type type)
        {
            return (type.IsValueType || type.Equals(typeof(string)));
        }

        private bool IsList(object childValue)
        {
            var list = childValue as IList;
            if (list == null) return false;

            return true;
        }
    }
}
