namespace Yusr.Email
{
    public class EmailMessage
    {  
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = "YusrBus";
        public string SenderAppKey { get; set; } = string.Empty;
        public string[] ReceiversEmailsList { get; set; } = [];

        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public List<byte[]> FilesBytes { get; set; } = new List<byte[]>();

        public EmailMessage()
        {
            
        }

        public EmailMessage(string senderEmail, string senderName, string senderAppKey, string[] receiversEmailsList, string subject, string body, List<byte[]> filesBytes)
        {
            SenderEmail = senderEmail;
            SenderName = senderName;
            SenderAppKey = senderAppKey;
            ReceiversEmailsList = receiversEmailsList;
            Subject = subject;
            Body = body;
            FilesBytes = filesBytes;
        }
    }
}
