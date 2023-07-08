using System.Globalization;

namespace Application.Exceptions
{
    public class CustomUnauthorizedAccessException : Exception
    {
        public string MessageCode { get; set; }
        public string MessageText { get; set; }

        /// <summary>
        /// init
        /// </summary>
        public CustomUnauthorizedAccessException() : base()
        {
            this.MessageCode = "SYS0004W";
            this.MessageText = "No access to this page.";
        }

        /// <summary>
        /// throw new CustomUnauthorizedAccessException
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="messageText"></param>
        /// <param name="args"></param>
        public CustomUnauthorizedAccessException(string messageCode, string messageText, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, messageCode, messageText, args))
        {
            this.MessageCode = !string.IsNullOrEmpty(messageCode) ? messageCode : "SYS0004W";
            this.MessageText = !string.IsNullOrEmpty(messageText) ? messageText : "No access to this page.";
        }
    }
}