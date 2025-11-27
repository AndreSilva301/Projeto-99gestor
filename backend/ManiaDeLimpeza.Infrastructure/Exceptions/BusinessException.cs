using System.Runtime.Serialization;

namespace ManiaDeLimpeza.Infrastructure.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public string? ErrorCode { get; }

        public BusinessException() : base("A business rule was violated.") { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException) { }

        public BusinessException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorCode = info.GetString(nameof(ErrorCode));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ErrorCode), ErrorCode);
        }
    }
}
