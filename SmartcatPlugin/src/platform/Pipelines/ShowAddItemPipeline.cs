using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace SmartcatPlugin.Pipelines
{
    public class ShowSmartcatModalPipeline : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue();
        }
    }
}