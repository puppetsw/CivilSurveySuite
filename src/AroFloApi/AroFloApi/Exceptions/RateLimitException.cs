using System;
using System.Runtime.Serialization;
using AroFloApi.Enums;

namespace AroFloApi.Exceptions
{
    [Serializable]
    public class RateLimitException : Exception
    {
        public RateLimitException()
        {
        }

        public RateLimitException(string message, Status statusCode)
            : base(message)
        {
        }

        public RateLimitException(string message, Status statusCode, Exception inner)
            : base(message, inner)
        {
        }

        protected RateLimitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}