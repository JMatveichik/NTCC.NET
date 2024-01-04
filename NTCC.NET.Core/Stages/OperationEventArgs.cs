using Dispergator.Common.Facility;
using System;

namespace Dispergator.Common.Stages
{
    public delegate void OperationHandler(StageBase sender, OperationEventArgs ea);

    public class OperationEventArgs : StringMessageEventArgs
    {
        
        public OperationEventArgs(string message, StageResult res) : base(message)
        {
            Result = res;
        }

        public StageResult Result
        {
            get;
            private set;
        }
    }
    
}
