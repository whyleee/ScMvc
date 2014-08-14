using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScMvc.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RenderingDataSourceAttribute : Attribute
    {
        public RenderingDataSourceAttribute(RenderingDataSource dataSource)
        {
            DataSource = dataSource;
        }

        public RenderingDataSource DataSource { get; private set; }
    }

    public enum RenderingDataSource
    {
        RenderingItem,
        ContextItem
    }
}
