using System;
using System.Collections.Generic;
using System.Linq;

namespace ScMvc.Models
{
    public interface IEditableModel
    {
        bool IsEditMode { get; set; }
    }
}
