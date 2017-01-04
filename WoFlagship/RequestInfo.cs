using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WoFlagship
{
    [Serializable]
    public class RequestInfo
    {
        public string RequestUrl { get; set; }
        public string DataString { get; set; }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public RequestInfo Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                var clonedData = formatter.Deserialize(stream) as RequestInfo;
                stream.Close();
                return clonedData;
            }
        }
    }
}
