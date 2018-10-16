using System.Management.Automation;
using JetBrains.Annotations;
using PSBase;

namespace SmugUp
{
    [PublicAPI]
    [Cmdlet(VerbsData.Publish, "ToSmugMug")]
    public class PublishToSmugMug : CmdletBase<ISmugMugPublisher>
    {
        [Parameter]
        public string Directory { get; set; }

        [Parameter]
        public SwitchParameter ConvertStarsToTags { get; set; }

        protected override void ProcessRecord()
        {
            var publisher = GetRootObject();
            base.ProcessRecord();
        }
    }
}
