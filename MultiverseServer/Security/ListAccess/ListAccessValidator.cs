using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MultiverseServer.Security.ListAccess
{
    public class ListAccessValidator
    {
        public const int MAX_COUNT = 20;

        private ListAccessValidator()
        {

        }

        public static bool NormalizeListAccess(Object obj)
        {
            // Make sur that a count and offset field is present and that count is lesser or equal to the maximum
            bool countIsPresent = false;
            bool offsetIsPresent = false;

            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                if (pi.Name.Equals("count") && pi.GetType() == typeof(int))
                {
                    countIsPresent = true;
                }
                else if (pi.Name.Equals("offset") && pi.GetType() == typeof(int))
                {
                    offsetIsPresent = true;
                    object val = pi.GetValue(obj);
                    int ival = (int)val;
                    if (ival > MAX_COUNT)
                    {
                        pi.SetValue(obj, MAX_COUNT);
                    }
                }
            }
            return countIsPresent && offsetIsPresent;
        }
    }
}
