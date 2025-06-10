using System.Net.Mail;
using System.Net;

namespace FaceRecognitionServer
{
    // Static utility class responsible for sending emails using Gmail's SMTP server
    public static class EmailSender
    {
        /// <summary>
        /// Sends an email asynchronously using Gmail SMTP over STARTTLS.
        /// </summary>
        /// <param name="recipientEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        public static async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            // Sender Gmail credentials (should be moved to a secure location like environment variables or app config)
            var senderEmail = "facerecognitionsoftware1@gmail.com";
            var appPassword = "oenb ajdj cqgm zlyc"; // App-specific password for Gmail account

            // SMTP (Simple Mail Transfer Protocol) client setup:
            // - smtp.gmail.com is Gmail's SMTP server
            // - Port 587 uses STARTTLS: plain connection upgraded to encrypted
            // - EnableSsl = true enables STARTTLS (not legacy SSL)
            // - Credentials are manually set
            using var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",          // SMTP server address
                Port = 587,                       // Standard port for STARTTLS
                EnableSsl = true,                 // Enables STARTTLS encryption
                UseDefaultCredentials = false,    // Do not use Windows credentials
                Credentials = new NetworkCredential(senderEmail, appPassword) // Auth info
            };

            // Compose the email message with subject and body
            using var message = new MailMessage(
                from: senderEmail,
                to: recipientEmail,
                subject: subject,
                body: body
            );

            try
            {
                // Send the message asynchronously using SMTP protocol
                await smtpClient.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
                throw;
            }
        }
    }

    /*
    This program sends emails using the SMTP (Simple Mail Transfer Protocol), which is the standard protocol 
    for sending messages between email clients and servers. Specifically, it connects to Gmail's SMTP server 
    on port 587 using STARTTLS — a security layer that upgrades a plain TCP connection to encrypted TLS after 
    an initial handshake. 

    When the program runs, it authenticates with Gmail using an app-specific password, composes the message, 
    and securely sends it to the recipient. Under the hood, the SMTP server and client exchange a series of commands:
    first EHLO, then STARTTLS, then AUTH LOGIN for authentication, followed by MAIL FROM, RCPT TO, and DATA to send 
    the email body, ending with QUIT. All of this is handled by .NET's SmtpClient class, which abstracts the 
    low-level protocol steps and ensures the message is securely delivered.

    FLOW DIAGRAM: Message exchange between your program and smtp.gmail.com
    -----------------------------------------------------------------------
    Your Program                            Gmail SMTP Server (smtp.gmail.com)
    -----------------------------------------------------------------------
        |  — TCP connect to port 587   —>   |
        |  <— 220 Service Ready           — |
        |  — EHLO                        —> |
        |  <— 250-Supported features     — |
        |  — STARTTLS                    —> |   // Request upgrade to TLS
        |  <— 220 Ready to start TLS     — |
        |========= TLS handshake (encrypted from here) =========|
        |  — EHLO again (encrypted)       —> |
        |  — AUTH LOGIN (base64 creds)    —> |
        |  <— 235 Auth successful        — |
        |  — MAIL FROM:<sender>           —> |
        |  — RCPT TO:<recipient>          —> |
        |  — DATA                         —> |
        |  — (send email content + '.')   —> |
        |  <— 250 OK                      — |
        |  — QUIT                         —> |
        |  <— 221 Bye                     — |
    -----------------------------------------------------------------------
*/

}
