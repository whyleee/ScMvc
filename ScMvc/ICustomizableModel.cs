using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScMvc
{
    public interface ICustomizableModel
    {
        string Class { get; set; }

        IDictionary<string, object> Attributes { get; set; }
    }
}
