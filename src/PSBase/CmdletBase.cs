using System;
using System.Management.Automation;
using JetBrains.Annotations;

namespace PSBase
{
    [PublicAPI]
    public abstract class CmdletBase<THandler, TApp> : PSCmdlet 
        where THandler : class, ICmdletHandler
        where TApp : PSApplicationBase, new()
    {
        private readonly Random _random = new Random();
        private TApp _app;

        protected THandler GetRootObject() => _app.Register<THandler>();

        protected override void BeginProcessing() => _app = new TApp();

        protected string ResolvePath(string path) => GetUnresolvedProviderPathFromPSPath(path);

        protected override void EndProcessing() => _app.Dispose();

        protected void WriteInformation(string message, string source = "")
        {
            WriteInformation(new InformationRecord(message, source));
        }

        protected void WriteError(string error, ErrorCategory category = ErrorCategory.NotSpecified, object targetObject = null)
        {
            var errorRecord = new ErrorRecord(new Exception(error), category.ToString(), category, targetObject);
            WriteError(errorRecord);
        }

        protected void WriteException(Exception exception, ErrorCategory errorCategory = ErrorCategory.NotSpecified, object targetObject = null)
        {
            WriteError(new ErrorRecord(exception, exception.GetType().Name, errorCategory, targetObject));
        }

        protected void ThrowTerminatingError(string message, ErrorCategory category = ErrorCategory.NotSpecified, string errorId = null, object targetObject = null)
        {
            var errorRecord = new ErrorRecord(new Exception(message), errorId ?? category.ToString(), category, targetObject);
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