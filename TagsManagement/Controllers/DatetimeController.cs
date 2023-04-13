using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace TagsManagement.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DatetimeController : Controller
    {
        [HttpGet("ServerTime")]
        public IActionResult getServerDateTime()
        {
            var usCulture = CultureInfo.CreateSpecificCulture("en-US");
            DateTime serverTime = DateTime.Now;
            string format = "dddd dd/MM/yyy - HH:mm:ss tt";

            TimeZone curTimeZone = TimeZone.CurrentTimeZone;
            string CurTimeZoneName = curTimeZone.StandardName;

            var serverTimeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);


            DateTime utcTime = serverTime.ToUniversalTime();
            string utcFormatedTime = utcTime.ToString(format, usCulture);

            string serverFormatedTime = serverTime.ToString(format, usCulture);

            //string serverFormatedTime = $"dddd dd/MM/yy - HH:mm:ss tt : {serverTime}";

            // convert to GMT:
            //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime euStandardTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, "W. Europe Standard Time");  // ConvertTimeFromUtc
            string euStandardFormatedTime = euStandardTime.ToString(format, usCulture);


            return Ok( new
            {
                serverTimeZone = curTimeZone,
                serverTimeZoneName = CurTimeZoneName,
                serverTimeZoneOffset = serverTimeZoneOffset,
                serverTime = serverFormatedTime,
                euStandardTime = euStandardFormatedTime,
                utcTime = utcFormatedTime
            });
        }
    }
}
