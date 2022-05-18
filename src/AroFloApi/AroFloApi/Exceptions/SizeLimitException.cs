using System;
using System.Runtime.Serialization;
using AroFloApi.Enums;

namespace AroFloApi.Exceptions
{
    [Serializable]
    public class SizeLimitException : Exception
    {
        public SizeLimitException()
        {
        }

        public SizeLimitException(string message, Status statusCode)
            : base(message)
        {
        }

        public SizeLimitException(string message, Status statusCode, Exception inner)
            : base(message, inner)
        {
        }

        protected SizeLimitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}