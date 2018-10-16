﻿using System.Collections.Concurrent;

namespace PSBase
{
    public class ProgressReporter : BlockingCollection<ProgressInfo>
    {
        public void UpdateProgress(string currentOperation, int totalItems, int completedItems, string verboseOutput = "")
        {
            var progress = new ProgressInfo(currentOperation, totalItems, completedItems, verboseOutput);
            Add(progress);
        }
    }
}