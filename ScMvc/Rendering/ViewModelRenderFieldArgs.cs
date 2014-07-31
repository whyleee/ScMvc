using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering
{
    public class ViewModelRenderFieldArgs : RenderFieldArgs
    {
        public object Model { get; set; }

        public string DefaultText { get; set; }
    }
}