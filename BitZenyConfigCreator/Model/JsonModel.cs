using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BitZenConfigCreator.Model
{
    class JsonModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("dirs")]
        public List<CryptDir> CD { get; set; }

        [JsonProperty("seed")]
        public List<string> Node { get; set; }
    }

    class CryptDir
    {
        [JsonProperty("OS")]
        public string os { get; set; }

        [JsonProperty("Dir")]
        public string dir { get; set; }
    }
}
