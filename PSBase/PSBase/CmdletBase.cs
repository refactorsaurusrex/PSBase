using System;
using System.Management.Automation;

namespace PSBase
{
    public abstract class CmdletBase<T> : PSCmdlet where T : class, ICmdletHandler
    {
        private readonly Random _random = new Random();
        private PSApplication _app;

        protected T GetRootObject() => _app.Register<T>();

        protected override void BeginProcessing() => _app = PSApplication.Create();

        protected string ResolvePath(string path) => GetUnresolvedProviderPathFromPSPath(path);

        protected override void EndProcessing() => _app.Dispose();

        protected void WriteInformation(string message)
        {
            WriteInformation(new InformationRecord(message, ""));
        }

        protected void WriteError(string error, ErrorCategory category)
        {
            var errorRecord = new ErrorRecord(new Exception(error), category.ToString(), category, null);
            WriteError(errorRecord);
        }

        protected void WriteUnspecifiedError(Exception exception)
        {
            WriteError(new ErrorRecord(exception, exception.GetType().Name, ErrorCategory.NotSpecified, null));
        }

        protected void ThrowTerminatingError(string message, ErrorCategory category)
        {
            var errorRecord = new ErrorRecord(new Exception(message), category.ToString(), category, null);
            ThrowTerminatingError(errorRecord);
        }

        protected void WriteInvalidOperationException(string message)
        {
            var ex = new PSInvalidOperationException(message);
            WriteError(ex.ErrorRecord);
        }

        protected void WriteInvalidOperationException(InvalidOperationException inner)
        {
            var ex = new PSInvalidOperationException(inner.Message, inner);
            WriteError(ex.ErrorRecord);
        }

        protected void WriteArgumentException(string message, string paramName)
        {
            var ex = new PSArgumentException(message, paramName);
            WriteError(ex.ErrorRecord);
        }

        protected void ShowProgress(ProgressRecord progressRecord, ProgressReporter progressReporter)
        {
            foreach (var progressInfo in progressReporter.GetConsumingEnumerable())
            {
                progressRecord.CurrentOperation = progressInfo.CurrentOperation;
                progressRecord.StatusDescription = $"Completed {progressInfo.CompletedItems} of {progressInfo.TotalItems} items";
                progressRecord.PercentComplete = progressInfo.PercentComplete();
                WriteProgress(progressRecord);

                if (!progressInfo.VerboseOutput.IsNullOrEmpty())
                    WriteVerbose(progressInfo.VerboseOutput);
            }
        }

        protected void HideProgress(ProgressRecord progressRecord)
        {
            progressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progressRecord);
        }

        protected ProgressRecord CreateProgressRecord(string activity, string statusDescription) => new ProgressRecord(_random.Next(), activity, statusDescription);
    }
}