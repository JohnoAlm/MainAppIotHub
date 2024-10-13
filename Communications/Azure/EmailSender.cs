using Azure.Communication.Email;
using Azure;

namespace Communications.Azure;

public class EmailSender
{
    private readonly EmailClient _emailClient;
    private readonly string _senderAddress; 

    public EmailSender(string connectionString = "endpoint=https://cs-18a9688d.europe.communication.azure.com/;accesskey=4AX3B372EbB6hcl12OMyYgHEDCdj8q8vB45x5oFolWlplnoVWhcXJQQJ99AJACULyCpqXhATAAAAAZCSlBJB", string senderAddress = "DoNotReply@61cfe3b2-395a-4595-8849-b97ad636885c.azurecomm.net")
    {
        _emailClient = new EmailClient(connectionString);
        _senderAddress = senderAddress; 
    }

    public void SendEmail(string toAddress, string subject, string body, string bodyPlainText)
    {
        EmailSendOperation emailSendOperation = _emailClient.Send(
            WaitUntil.Completed,
            senderAddress: _senderAddress,
            recipientAddress: toAddress,
            subject: subject,
            htmlContent: body,
            plainTextContent: bodyPlainText);  
    }
}
