using System;
using System.Runtime.Serialization;

namespace STM.Common.Exceptions
{
    [Serializable]
    public class StmSchemaValidationException : Exception
    {
        public string ValidationMessage { get; private set; }

        public StmSchemaValidationException(string validationMessage) : base(validationMessage)
        {
            ValidationMessage = validationMessage;
        }

        public StmSchemaValidationException(string validationMessage, Exception ex)
            : base(validationMessage, ex)
        {
            ValidationMessage = validationMessage;

        }

        protected StmSchemaValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
