using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    [Serializable]
    public class UnsupportedDataTypeParserException : ParserException
    {
        private UnsupportedDataTypeParserException(string uiErrorMessage, string errorMessage) : base(uiErrorMessage, errorMessage, null)
        {
        }

        protected UnsupportedDataTypeParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
        }

        public static UnsupportedDataTypeParserException Create(object objValue, string expectedType)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            return new UnsupportedDataTypeParserException(message, $"{message}. Unsupported data type [{objValue.GetType().Name}]");
        }
    }
}
