using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.Converters
{
    public static class JsonHelper
    {
        public static bool TryParseJson<T>(string jsonData)
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema parsedSchema = generator.Generate(typeof(T));
            try
            {
                JObject jObject = JObject.Parse(jsonData);

                if (jObject.IsValid(parsedSchema))
                    return true;
                else
                    return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
