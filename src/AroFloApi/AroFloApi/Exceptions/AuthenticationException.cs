using System;
using System.Runtime.Serialization;
using AroFloApi.Enums;

namespace AroFloApi.Exceptions
{
    [Serializable]
    public class AuthenticationException : Exception
    {
        public AuthenticationException()
        {
        }

        public AuthenticationException(string message, Status statusCode)
            : base(message)
        {
        }

        public AuthenticationException(string message, Status statusCode, Exception inner)
            : base(message, inner)
        {
        }

        protected AuthenticationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}