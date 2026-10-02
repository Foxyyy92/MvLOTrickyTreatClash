using Newtonsoft.Json;
using Quantum;
using System;
using System.Reflection;
using UnityEngine;

namespace NSMB.Quantum {
    // This is bullshit. Why does JsonConvert.SerializeObject(QEnum) produce a STRING, but JsonConvert.DeserializeObject(QEnum) expect a VALUE?
    public class QuantumQEnumConverter : JsonConverter {

        [RuntimeInitializeOnLoadMethod]
        public static void Register() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                Converters = { new QuantumQEnumConverter() }
            };
        }

        public override bool CanConvert(Type objectType) {
            if (!objectType.IsGenericType) {
                return false;
            }

            var def = objectType.GetGenericTypeDefinition();

            return def == typeof(QEnum8<>) || def == typeof(QEnum16<>) 
                || def == typeof(QEnum32<>) || def == typeof(QEnum64<>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) {
                writer.WriteNull();
                return;
            }

            var valueField = value.GetType().GetField("Value", BindingFlags.Public | BindingFlags.Instance);
            var rawValue = valueField.GetValue(value);

            writer.WriteValue(rawValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            var enumType = objectType.GetGenericArguments()[0];
            var valueField = objectType.GetField("Value", BindingFlags.Public | BindingFlags.Instance);

            object rawValue;

            if (reader.TokenType == JsonToken.String) {
                // Enum as string
                var parsedEnum = Enum.Parse(enumType, (string) reader.Value);

                rawValue = Convert.ChangeType(
                    parsedEnum,
                    valueField.FieldType
                );
            } else {
                // Enum as raw value
                rawValue = Convert.ChangeType(
                    reader.Value,
                    valueField.FieldType
                );
            }

            var result = Activator.CreateInstance(objectType);
            valueField.SetValue(result, rawValue);

            return result;
        }
    }
}