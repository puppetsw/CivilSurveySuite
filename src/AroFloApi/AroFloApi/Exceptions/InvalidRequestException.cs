using System;
using System.Runtime.Serialization;
using AroFloApi.Enums;

namespace AroFloApi.Exceptions
{
    [Serializable]
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException()
        {
        }

        public InvalidRequestException(string message, Status statusCode)
            : base(message)
        {
        }

        public InvalidRequestException(string message, Status statusCode, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}