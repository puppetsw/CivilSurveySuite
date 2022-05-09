using System;
using System.Runtime.Serialization;

namespace AroFloApi.Exceptions
{
    [Serializable]
    public class LoginFailedException : Exception
    {
        public LoginFailedException()
        {
        }

        public LoginFailedException(string message, Status statusCode)
            : base(message)
        {
        }

        public LoginFailedException(string message, Status statusCode, Exception inner)
            : base(message, inner)
        {
        }

        protected LoginFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}