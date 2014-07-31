using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace ScMvc.Models
{
    public interface IEditableItemModel : IEditableModel
    {
        Item Item { get; set; }
    }
}
