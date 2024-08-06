using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Customers.Infrastructure.Utils
{
    public static class DateUtils
    {       
        public static string FormatDate(DateTime? date)
        {
            if (date.HasValue) 
            {
                return date.Value.ToString("MM/dd/yyyy").Replace("-","/");
            }
            else
            {
                return string.Empty; 
            }
        }
        public static string FormatDateTime(DateTime? datetime)
        {
            if (datetime.HasValue)
            {
                return datetime.Value.ToString("MM/dd/yyyy hh:mm:ss tt").Replace("-", "/");
            }
            else
            {
                return string.Empty; 
            }
        }
    }
}
 