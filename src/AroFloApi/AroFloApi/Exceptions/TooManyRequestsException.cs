using System;
using System.Runtime.Serialization;
using AroFloApi.Enums;

namespace AroFloApi.Exceptions
{
    [Serializable]
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException()
        {
        }

        public TooManyRequestsException(string message, Status statusCode)
            : base(message)
        {
        }

        public TooManyRequestsException(string message, Status statusCode, Exception inner)
            : base(message, inner)
        {
        }

        protected TooManyRequestsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}