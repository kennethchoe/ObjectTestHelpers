using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace ObjectTestHelpers
{
    public class SampleValueSetter
    {
        private int _id;
        private bool _processFields;
        private bool _processProperties;

        public void AssignSampleValues(object obj, int seed, bool processFields = true,
                                        bool processProperties = true)
        {
            _id = seed;
            _processFields = processFields;
            _processProperties = processProperties;

            AssignSampleValuesInternal(obj);
        }

        private void AssignSampleValuesInternal(object obj)
        {

            if ((_processFields))
                foreach (var fieldInfo in obj.GetType().GetFields())
                    AssignSampleValueToField(obj, fieldInfo);

            if ((_processProperties))
                foreach (var prop in obj.GetType().GetProperties())
                    AssignSampleValueToProperty(obj, prop);
        }

        private bool IsSimpleType(Type type)
        {
            return (type.IsValueType || type.Equals(typeof (string)));
        }

        private bool IsListOfSimpleType(object childValue)
        {
            var list = childValue as IList;
            if (list == null) return false;

            if (list.Count == 0) return true;

            return IsSimpleType(list[0].GetType());
        }

        private void HandleListOfSimpleType(object childValue)
        {
            var list = (IList) childValue;

            for (var i = 0; i < list.Count; i++)
            {
                list[i] = BuildTypedSampleValue(list[i].GetType());
            }
        }

        private void ProcessCompositeChild(object childValue)
        {
            if (childValue == null)
                return;

            var dict = childValue as IDictionary;
            if (dict != null)
            {
                foreach (var child in dict.Values)
                    AssignSampleValuesInternal(child);

                return;
            }

            var enumerable = childValue as IEnumerable;
            if (enumerable != null)
            {
                foreach (var child in enumerable)
                    AssignSampleValuesInternal(child);

                return;
            }

            AssignSampleValuesInternal(childValue);
        }

        public void AssignSampleValueToField(object obj, FieldInfo fieldInfo, int? seed = null)
        {
            if (seed != null) _id = seed.Value;

            var childValue = fieldInfo.GetValue(obj);

            if (IsSimpleType(fieldInfo.FieldType))
            {
                var value = BuildTypedSampleValue(fieldInfo.FieldType);
                fieldInfo.SetValue(obj, value);
            }
            else if (IsListOfSimpleType(childValue))
                HandleListOfSimpleType(childValue);
            else
                ProcessCompositeChild(childValue); 
        }

        public void AssignSampleValueToProperty(object obj, PropertyInfo propInfo, int? seed = null)
        {
            if (seed != null) _id = seed.Value;

            MakeRoomForArray(obj, propInfo);

            var childValue = propInfo.GetValue(obj, null);

            if (IsSimpleType(propInfo.PropertyType))
            {
                if (!propInfo.CanWrite) return;
                var value = BuildTypedSampleValue(propInfo.PropertyType);
                propInfo.SetValue(obj, value, null);
            }
            else if (IsListOfSimpleType(childValue))
                HandleListOfSimpleType(childValue);
            else
                ProcessCompositeChild(childValue);
        }

        private void MakeRoomForArray(object obj, PropertyInfo propInfo)
        {
            // TODO: how can we do this in generic way?
            if (propInfo.PropertyType.Name == "String[]")
                if (propInfo.GetValue(obj, null) == null)
                    propInfo.SetValue(obj, new[] { "" }, null);
        }

        private object BuildTypedSampleValue(Type fieldType)
        {
            object value;

            _id++;

            if (fieldType.IsAssignableFrom(typeof(bool)))
            {
                value = _id % 2 == 1;

            }
            else if (fieldType.IsAssignableFrom(typeof(DateTime)))
            {
                value = new DateTime(2012, 1, 1).AddDays(_id);

            }
            else if (fieldType.IsAssignableFrom(typeof(long)))
            {
                value = Convert.ToInt64(_id);

            }
            else if (fieldType.IsAssignableFrom(typeof(int)))
            {
                value = _id;

            }
            else if (fieldType.IsAssignableFrom(typeof(byte)))
            {
                value = Convert.ToByte(_id);

            }
            else if (fieldType.IsAssignableFrom(typeof(Int64)))
            {
                value = _id;

            }
            else if (fieldType.IsAssignableFrom(typeof(decimal)))
            {
                value = new decimal(_id);

            }
            else if (fieldType.IsAssignableFrom(typeof(string)))
            {
                var stringValue = _id.ToString(CultureInfo.InvariantCulture);
                value = stringValue.Substring(stringValue.Length - 1, 1);
            }
            else if (fieldType.IsEnum)
            {
                var list = Enum.GetValues(fieldType);
                var index = _id%(list.Length);
                value = list.GetValue(index);
            }
            else if (IsNullableEnum(fieldType))
            {
                var list = Enum.GetValues(Nullable.GetUnderlyingType(fieldType));
                var index = _id % (list.Length + 1);
                value = index == 0 ? null : list.GetValue(index - 1);
            }
            else if (fieldType.Name == "Guid")
            {
                value = Guid.Parse(string.Format("00000000-0000-0000-0000-{0,12:x12}", _id));
            }
            else
            {
                throw new Exception("Unknown property type: " + Convert.ToString(fieldType));
            }


            return value;
        }

        private bool IsNullableEnum(Type fieldType)
        {
            Type u = Nullable.GetUnderlyingType(fieldType);
            return (u != null) && u.IsEnum;
        }
    }
}