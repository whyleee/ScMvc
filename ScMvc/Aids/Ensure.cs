using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScMvc.Aids
{
    internal class Ensure
    {
        public static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ArgumentNotNullOrEmpty(string argument, string name)
        {
            Ensure.ArgumentNotNull(argument, name);

            if (argument.Length == 0)
            {
                throw new ArgumentException("Can't be empty", name);
            }
        }
    }
}
