using System.Management.Automation;

namespace PSBase
{
    public abstract class HighImpactCmdletBase<T> : CmdletBase<T> where T : class, ICmdletHandler
    {
        [Parameter(DontShow = true)]
        public SwitchParameter YesIReallyKnowWhatImDoing { get; set; }

        protected abstract string Warning { get; }

        protected bool IssueWarning()
        {
            if (!YesIReallyKnowWhatImDoing)
                ThrowTerminatingError("I'm not convinced you should be running this command. ", ErrorCategory.PermissionDenied);

            return ShouldContinue(Warning, "***** Hey, you! *****");
        }
    }
}