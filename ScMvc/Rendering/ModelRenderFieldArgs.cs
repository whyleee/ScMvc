using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering
{
    public class ModelRenderFieldArgs : RenderFieldArgs
    {
        public object Model { get; set; }
    }
}