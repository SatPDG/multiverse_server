using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MultiverseServer.Security.Json
{
    public class JsonValidator
    {
        private JsonValidator()
        {

        }

        public static bool ValidateJsonNotNullOrEmpty(Object obj)
        {
            if (obj != null)
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    if (pi.GetValue(obj) == null)
                    {
                        return false;
                    }
                    if (pi.PropertyType == typeof(string) && string.IsNullOrWhiteSpace(((string)pi.GetValue(obj))))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

    }
}
