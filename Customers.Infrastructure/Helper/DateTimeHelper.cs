// Services/DateTimeService.cs
using Microsoft.JSInterop;
using Customers.Infrastructure.Dtos;
using System;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Helper;
public class DateTimeHelper
{
    private readonly IJSRuntime _jsRuntime;

    public DateTimeHelper(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<DateTime?> ConvertToLocalTimeZone(DateTime? utcDateTime)
    {
        try
        {
            if(utcDateTime == null)
            {
                return null;
            }
            var utcDateTimeString = utcDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var localDateTimeStringTask = await _jsRuntime.InvokeAsync<string>("convertToLocalTimeZone", utcDateTimeString);
            return DateTime.Parse(localDateTimeStringTask);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }   
}
