# Yusr Email Service

To use this service and send an email message, here is a full sample program:
```cs
using Yusr.Email.Abstractions.Primitives;
using Yusr.Email;
using System.Collections.Generic;

// 1. Prepare attachments (optional)
var fileBytes = System.IO.File.ReadAllBytes(@"C:\path\to\file.pdf");
var attachments = new List<byte[]> { fileBytes };

// 2. Create the email message
var email = new EmailMessage(
    senderEmail: "your-email@gmail.com",
    senderName: "Your Name",
    senderAppKey: "your-gmail-app-password", // Use a Gmail App Password
    receiversEmailsList: new[] { "recipient@example.com", "another@example.com" },
    subject: "Your Subject",
    body: "This is the body content. It will be wrapped in <h2> tags by the service.",
    filesBytes: attachments
);

// 3. Send the email using the concrete EmailService
var emailService = new EmailService();
await emailService.SendAsync(email);

```
