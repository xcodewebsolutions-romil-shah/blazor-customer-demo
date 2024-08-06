using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Customers.Admin.Components.Pages;
using System.Diagnostics;
//using Customers.User.Components.Pages.Shared;

namespace Customers.Admin.Services
{
    public class ErrorDialogService(DialogService DialogService)
    {
        public async Task ShowError(Exception ex)
        {
            if (ex.StackTrace == null)
            {
                await DialogService.Alert($"Meesage : {ex.Message}", "Error");
                return;
            }

            await DialogService.OpenAsync<Error>("Error", new Dictionary<string, object>
            {
                {"ErrMessage", ex.Message},
                {"StackTrace" , ex.StackTrace}
            }, new DialogOptions()
            {
                ShowClose = false,
                ShowTitle = false,
                Style = "width:400px;",
                CloseDialogOnEsc = false,
                CloseDialogOnOverlayClick = false
            });            
        }
    }
}
