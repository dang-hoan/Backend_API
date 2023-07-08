using System.Globalization;

namespace Application.Exceptions
{
    public class CustomExpiresAccessException : Exception
    {
        public string MessageCode { get; set; }
        public string MessageText { get; set; }

        /// <summary>
        /// init
        /// </summary>
        public CustomExpiresAccessException() : base()
        {
            this.MessageCode = "SYS0005W";
            this.MessageText = "Tokens have expired.";
        }

        /// <summary>
        /// throw new CustomExpiresAccessException
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="messageText"></param>
        /// <param name="args"></param>
        public CustomExpiresAccessException(string messageCode, string messageText, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, messageCode, messageText, args))
        {
            this.MessageCode = !string.IsNullOrEmpty(messageCode) ? messageCode : "SYS0005W";
            this.MessageText = !string.IsNullOrEmpty(messageText) ? messageText : "Tokens have expired.";
        }
    }
}