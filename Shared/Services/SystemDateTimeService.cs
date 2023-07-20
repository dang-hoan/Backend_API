using Application.Interfaces;
using System.Globalization;

namespace Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;

        public bool IsCorrectFormat(string datetime, string format, out DateTime result)
        {
            if (DateTime.TryParseExact(datetime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}