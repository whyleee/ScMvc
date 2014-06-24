using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace ScMvc
{
    public interface IEditableModel
    {
        Item Item { get; set; }

        bool IsEditMode { get; set; }
    }
}
