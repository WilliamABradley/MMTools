using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MMTools.FFProbeConfig.Stream
{
    public class FFStreamConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<FFStream> Results = new List<FFStream>();
            JArray array = JArray.Load(reader);
            foreach (var token in array)
            {
                var type = token.Children<JProperty>()
                    .FirstOrDefault(child => child.Name == "codec_type")
                    .Value.Value<string>();

                switch (type)
                {
                    case "audio":
                        Results.Add(token.ToObject<FFAudioStream>());
                        break;

                    case "video":
                        Results.Add(token.ToObject<FFVideoStream>());
                        break;

                    default:
                        Results.Add(token.ToObject<FFStream>());
                        break;
                }
            }
            return Results;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
