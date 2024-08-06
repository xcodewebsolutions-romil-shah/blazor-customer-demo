// Services/DateTimeService.cs
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public class DateTimeService
{
    private readonly IJSRuntime _jsRuntime;

    public DateTimeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> ConvertToTimeZoneAsync(DateTime utcDateTime, string timeZone)
    {
        return await _jsRuntime.InvokeAsync<string>("momentInterop.convertToTimeZone", utcDateTime, timeZone);
    }
}
