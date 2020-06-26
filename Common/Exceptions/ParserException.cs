using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    [Serializable]
    public class ParserException : Exception
    {
        public string? Address { get; set; }
        public string? UiErrorMessage { get; set; }

        private ParserException(string address, string uiErrorMessage, string errorMessage, Exception? innerException) : base(errorMessage, innerException)
        {
            Address = address;
            UiErrorMessage = uiErrorMessage;
        }

        protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Address = info.GetString(nameof(Address));
            UiErrorMessage = info.GetString(nameof(UiErrorMessage));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue(nameof(Address), Address, typeof(string));
            info.AddValue(nameof(UiErrorMessage), UiErrorMessage, typeof(string));
        }

        public static ParserException CreateUnsupportedDataTypeException(string address, object objValue, string expectedType)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            return new ParserException(address, message, $"{message}. Unsupported data type [{objValue.GetType()}]", null);
        }

        public static ParserException CreateFromException(string address, object objValue, string expectedType, Exception innerException)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            return new ParserException(address, message, $"{message}. {innerException.Message}", innerException);
        }

        public static ParserException CreateWithEnrichedMessage(string address, object objValue, string expectedType, string? additionalUiMessage, string errorMessage)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            if (!string.IsNullOrWhiteSpace(additionalUiMessage))
            {
                message = $"{message} {additionalUiMessage}";
            }
            return new ParserException(address, message, $"{message}. {errorMessage}", null);
        }

        private static string GetUiErrorMessage(object objValue, string expectedType)
        {
            return $"Failed to parse {objValue} into a valid {expectedType}";
        }
    }
}
