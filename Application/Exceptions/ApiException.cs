using System.Globalization;

namespace Application.Exceptions
{
    public class ApiException : Exception
    {
        public string MessageCode { get; set; } = default!;
        public string MessageText { get; set; } = default!;
        public ApiException() : base()
        {
        }

        public ApiException(string messageText) : base(messageText)
        {
            this.MessageText = messageText;
        }

        public ApiException(string messageCode, string messageText, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, messageCode, messageText, args))
        {
            this.MessageCode = messageCode;
            this.MessageText = messageText;
        }
    }
}