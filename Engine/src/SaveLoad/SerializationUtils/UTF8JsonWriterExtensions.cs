using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Civ2engine.SaveLoad.SerializationUtils;

public static class Utf8JsonWriterExtensions
{
    public static T[] Clamp<T>(this T[] array, T ignoreValue = default)
    {
        int i = array.Length - 1;
        for (; i >= 0 && array[i].Equals(ignoreValue); i--)
        {
        }

        if (i == -1)
        {
            return [];
        }
        var res = new T[i +1];
        Array.Copy(array, 0, res, 0, i +1);
        return res;
    }
    public static void WriteNonDefaultFields<T>(this Utf8JsonWriter writer, string objectName, T instance)
    {
        writer.WriteStartObject(objectName);
        WriteObjectContents(writer, instance);
    }

    public static void WriteNonDefaultFields<T>(this Utf8JsonWriter writer, T instance)
    {
        writer.WriteStartObject();
        WriteObjectContents(writer, instance);
    }
    private static void WriteObjectContents(this Utf8JsonWriter writer, object instance)
    {
        var type = instance.GetType();
        foreach (var info in
                 type.GetProperties())
        {
            var typeCode = Type.GetTypeCode(info.PropertyType);
            var defaultValue = GetDefaultValueFor(typeCode);
            var value = info.GetValue(instance);
            if ((defaultValue == null && value != null) || (defaultValue != null && !defaultValue.Equals(value)))
            {
                writer.WritePropertyName(info.Name);
                WriteValue(writer, typeCode, value);
            }
        }
        writer.WriteEndObject();
    }

    private static void WriteValue(Utf8JsonWriter writer, TypeCode typeCode, object? value)
    {
        switch (typeCode)
        {
            case TypeCode.Empty:
                writer.WriteNullValue();
                break;
            case TypeCode.Object:
                if (value is IEnumerable enumerable)
                {
                    writer.WriteStartArray();
                    foreach (var element in enumerable)
                    {
                        if(element.GetType().IsValueType)
                        {
                            WriteValue(writer, Type.GetTypeCode(element.GetType()), element);
                        }
                        else
                        {
                            writer.WriteStartObject();
                            writer.WriteObjectContents(element);
                        }
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteStartObject();
                    writer.WriteObjectContents(value);
                }

                break;
            case TypeCode.DBNull:
                writer.WriteNullValue();
                break;
            case TypeCode.Boolean:
                writer.WriteBooleanValue((bool)value);
                break;
            case TypeCode.Char:
                writer.WriteStringValue(value.ToString());
                break;
            case TypeCode.SByte:
                writer.WriteNumberValue(Convert.ToSByte(value));
                break;
            case TypeCode.Byte:
                writer.WriteNumberValue(Convert.ToByte(value));
                break;
            case TypeCode.Int16:
                writer.WriteNumberValue(Convert.ToInt16(value));
                break;
            case TypeCode.UInt16:
                writer.WriteNumberValue(Convert.ToUInt16(value));
                break;
            case TypeCode.Int32:
                writer.WriteNumberValue(Convert.ToInt32(value));
                break;
            case TypeCode.UInt32:
                writer.WriteNumberValue(Convert.ToUInt32(value));
                break;
            case TypeCode.Int64:
                writer.WriteNumberValue(Convert.ToInt64(value));
                break;
            case TypeCode.UInt64:
                writer.WriteNumberValue(Convert.ToUInt64(value));
                break;
            case TypeCode.Single:
                writer.WriteNumberValue(Convert.ToSingle(value));
                break;
            case TypeCode.Double:
                writer.WriteNumberValue(Convert.ToDouble(value));
                break;
            case TypeCode.Decimal:
                writer.WriteNumberValue(Convert.ToDecimal(value));
                break;
            case TypeCode.DateTime:
                writer.WriteStringValue(value.ToString());
                break;
            case TypeCode.String:
                writer.WriteStringValue(value.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Return the default value for a field, there's probaby a framework method for this... 
    /// </summary>
    /// <param name="typeCode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static object? GetDefaultValueFor(TypeCode typeCode)
    {
        return typeCode switch
        {
            TypeCode.Empty or TypeCode.DBNull => null,
            TypeCode.Object => null,
            TypeCode.Boolean => false,
            TypeCode.Char => '\0',
            TypeCode.SByte => default(sbyte),
            TypeCode.Byte => default(byte),
            TypeCode.Int16 => default(short),
            TypeCode.UInt16 => default(ushort),
            TypeCode.Int32 => default(int),
            TypeCode.UInt32 => default(uint),
            TypeCode.Int64 => default(long),
            TypeCode.UInt64 => default(ulong),
            TypeCode.Single => default(float),
            TypeCode.Double => default(double),
            TypeCode.Decimal => default(decimal),
            TypeCode.DateTime => default(DateTime),
            TypeCode.String => default(string),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}