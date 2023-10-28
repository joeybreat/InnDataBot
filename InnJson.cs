using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace innModel
{

    public class InnJson
    {

            [JsonProperty("items")]
            public Item[] Items { get; set; }
 

        public partial class Item
        {
            [JsonProperty("ЮЛ")]
            public JP jp { get; set; }
        }

        public partial class JP
        {
            [JsonProperty("НаимСокрЮЛ")]
            public string NameJp { get; set; }

            [JsonProperty("Адрес")]
            public Адрес AddressJp { get; set; }
        }

        public partial class Адрес
        {
            [JsonProperty("АдресПолн")]
            public string FullAddressJp { get; set; }
        }

        public partial class Deserialize
        {
            public static InnJson FromJson(string json) => JsonConvert.DeserializeObject<InnJson>(json, InnJson.Converter.Settings);
        }



        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }
    }


}
