using System;
using System.Collections.Generic;
using System.Linq;

namespace ScMvc.Models
{
    public interface ITemplatedModel
    {
        string Template { get; set; }
    }
}
