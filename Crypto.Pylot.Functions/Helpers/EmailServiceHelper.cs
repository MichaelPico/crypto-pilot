using System;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Crypto.Pylot.Functions.Models.Options;
using Microsoft.Extensions.Options;

namespace Crypto.Pylot.Functions.Helpers
{
    public class EmailServiceHelper
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderAddress;

        public EmailServiceHelper(IOptions<EmailServiceOptions> options)
        {
            var opts = options.Value;
            _emailClient = new EmailClient(opts.ConnectionString);
            _senderAddress = opts.SenderAddress;
        }

        public async Task SendCryptoAlertAsync(
            string recipientEmail,
            string coinName,
            string ticker,
            double targetPrice,
            string currency,
            double currentPrice,
            DateTime timestamp)
        {
            var subject = "🚨 Crypto Alert!";
            var plainText = $"{coinName} ({ticker}) has reached your target price of {targetPrice} {currency}.\n" +
                            $"Current Price: {currentPrice} {currency}\n" +
                            $"Time: {timestamp:yyyy-MM-dd HH:mm:ss}";

            var html = $@"
                <html>
                <body>
                    <h2>🚨 Crypto Alert!</h2>
                    <p><strong>{coinName} ({ticker})</strong> has reached your target price of <strong>{targetPrice} {currency}</strong>.</p>
                    <p>Current Price: <strong>{currentPrice} {currency}</strong></p>
                    <p>Time: {timestamp:yyyy-MM-dd HH:mm:ss}</p>
                </body>
                </html>";

            var emailContent = new EmailContent(subject)
            {
                PlainText = plainText,
                Html = html
            };

            var message = new EmailMessage(_senderAddress, recipientEmail, emailContent);

            await _emailClient.SendAsync(Azure.WaitUntil.Completed, message);
        }
    }
}
