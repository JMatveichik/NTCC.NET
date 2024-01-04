using System;

namespace Dispergator.Common.Stages
{
    public class StringMessageEventArgs : EventArgs
    {
        public StringMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message
        {
            get;
            private set;
        }

    }
}
