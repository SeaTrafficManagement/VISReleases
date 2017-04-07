using System;
using System.Runtime.Serialization;

namespace EfEnumToLookup.LookupGenerator
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EnumGeneratorException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public EnumGeneratorException() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public EnumGeneratorException(string message): base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EnumGeneratorException(string message, Exception innerException): base(message, innerException) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        public EnumGeneratorException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
