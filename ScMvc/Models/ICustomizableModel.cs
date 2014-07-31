using System;
using System.Collections.Generic;
using System.Linq;

namespace ScMvc.Models
{
    public interface ICustomizableModel
    {
        string Class { get; set; }

        IDictionary<string, object> Attributes { get; set; }
    }
}
