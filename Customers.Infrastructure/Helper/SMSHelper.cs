using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Customers.Infrastructure.Helper
{
    public static class SMSHelper
    {
        public static string SendMessage(string code)
        {
            // Find your Account SID and Auth Token at twilio.com / console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = "";
            string authToken = "";

            TwilioClient.Init(accountSid, authToken);
            var message = MessageResource.Create(
                body: $"{code} is your OTP to login. OTP is confidential. Please don't share with anyone.",
                from: new Twilio.Types.PhoneNumber(""),
                to: new Twilio.Types.PhoneNumber("")                
            );
                //to: new Twilio.Types.PhoneNumber("")                

            return message.Sid;
        }
    }
}
