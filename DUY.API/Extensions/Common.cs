
using System.Text.Json;
using System.Text.Json.Serialization;

namespace C.Tracking.Extensions
{
    public static class Common
    {
        public enum TableName
        {
            Transfers,
            Contracts,
            Category_Store,
            Customer,
            User, 
            Song,
            Config
        }
        public enum Status : byte
        {
            contract_Nhap = 1, // lwu nháp 

        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        }
    }



}
