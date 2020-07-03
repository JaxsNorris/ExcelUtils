using Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    [Serializable]
    public class ParserException : Exception
    {
        public string UiErrorMessage { get; private set; }

        protected ParserException(string uiErrorMessage, string errorMessage, Exception? innerException) : base(errorMessage, innerException)
        {
            UiErrorMessage = uiErrorMessage;
        }

        protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            UiErrorMessage = info.GetStringOrEmpty(nameof(UiErrorMessage));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue(nameof(UiErrorMessage), UiErrorMessage, typeof(string));
        }

        public static ParserException CreateUnsupportedValueException(object objValue, string expectedType)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            return new ParserException(message, $"{message}. Unsupported value of {objValue}", null);
        }

        public static ParserException CreateFromException(object objValue, string expectedType, Exception innerException)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            return new ParserException(message, $"{message}. {innerException.Message}", innerException);
        }

        public static ParserException CreateWithEnrichedMessage(object objValue, string expectedType, string? additionalUiMessage, string errorMessage)
        {
            var message = GetUiErrorMessage(objValue, expectedType);
            if (!string.IsNullOrWhiteSpace(additionalUiMessage))
            {
                message = $"{message} {additionalUiMessage}";
            }
            return new ParserException(message, $"{message}. {errorMessage}", null);
        }

        protected static string GetUiErrorMessage(object objValue, string expectedType)
        {
            return $"Failed to parse {objValue} into a valid {expectedType}";
        }
    }
}
