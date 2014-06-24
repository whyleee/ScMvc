using Sitecore.Pipelines.RenderField;

namespace ScMvc
{
    public class ViewModelRenderFieldArgs : RenderFieldArgs
    {
        public object Model { get; set; }

        public string DefaultText { get; set; }
    }
}