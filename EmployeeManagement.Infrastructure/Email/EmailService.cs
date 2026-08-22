using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace EmployeeManagement.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string email, string firstName, string otp)
    {
        var host = _configuration["SmtpSettings:Host"];

        var port = _configuration.GetValue<int>("SmtpSettings:Port");

        var senderEmail = _configuration["SmtpSettings:SenderEmail"];

        var senderName = _configuration["SmtpSettings:SenderName"];

        var appPassword = _configuration["SmtpSettings:AppPassword"];

        var expiryMinutes = _configuration.GetValue<int>("OtpSettings:ExpiryMinutes");

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,

            Credentials = new NetworkCredential(senderEmail, appPassword)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail!, senderName),

            Subject = "Your Verification Code",

            Body = $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif; color: #333; line-height: 1.6;">
                    <div style="max-width: 600px; margin: 0 auto; padding: 30px;">
                
                        <h2 style="color: #1f2937;">Verify Your Email Address</h2>

                        <p>Hello {firstName},</p>

                        <p>
                            Thank you for registering with the 
                            <strong>Employee Management System</strong>.
                        </p>

                        <p>Your email verification code is:</p>

                        <div style="
                            margin: 25px 0;
                            padding: 15px;
                            background-color: #f3f4f6;
                            border-radius: 8px;
                            text-align: center;
                            font-size: 32px;
                            font-weight: bold;
                            letter-spacing: 8px;
                            color: #111827;">
                            {otp}
                        </div>

                        <p>
                            This verification code is valid for <strong>{expiryMinutes} minutes</strong>.
                        </p>

                        <p>
                            For your security, please do not share this code with anyone.
                        </p>

                        <p>
                            If you did not request this verification code, 
                            you can safely ignore this email.
                        </p>

                        <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">

                        <p style="font-size: 13px; color: #6b7280;">
                            Regards,<br>
                            <strong>Employee Management System</strong>
                        </p>

                    </div>
                </body>
                </html>
                """,
            IsBodyHtml = true
        };
        message.To.Add(email);

        await client.SendMailAsync(message);
    }

    public async Task SendOnboardingEmailAsync(string email, string firstName, string lastName, string? employeeCode = null)
    {
        var host = _configuration["SmtpSettings:Host"];
        var port = _configuration.GetValue<int>("SmtpSettings:Port");
        var senderEmail = _configuration["SmtpSettings:SenderEmail"];
        var senderName = _configuration["SmtpSettings:SenderName"];
        var appPassword = _configuration["SmtpSettings:AppPassword"];

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(senderEmail, appPassword)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail!, senderName),
            Subject = "Welcome to the Employee Management System – Your Account Is Ready",

            Body = $"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Welcome to Employee Management System</title>
                </head>

                <body style="
                    margin: 0;
                    padding: 0;
                    background-color: #f4f6f8;
                    font-family: Arial, Helvetica, sans-serif;
                    color: #333333;
                ">

                    <div style="
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff;
                        border-radius: 10px;
                        padding: 40px;
                        box-shadow: 0 2px 8px rgba(0,0,0,0.08);
                    ">

                        <h2 style="
                            margin-top: 0;
                            color: #1f2937;
                            font-size: 24px;
                        ">
                            Welcome to the Employee Management System!
                        </h2>

                        <p>Hello <strong>{firstName}</strong>,</p>

                        <p>
                            We’re pleased to let you know that your email address has been
                            successfully verified and your employee account is now ready.
                        </p>

                        <div style="
                            margin: 25px 0;
                            padding: 20px;
                            background-color: #f8fafc;
                            border: 1px solid #e5e7eb;
                            border-radius: 8px;
                        ">
                            <h3 style="
                                margin-top: 0;
                                color: #1f2937;
                                font-size: 18px;
                            ">
                                Employee Details
                            </h3>

                            <p style="margin: 8px 0;">
                                <strong>Employee Name:</strong>
                                {firstName} {lastName}
                            </p>

                            <p style="margin: 8px 0;">
                                <strong>Employee Code:</strong>
                                {employeeCode}
                            </p>
                        </div>

                        <p>
                            You can now sign in to the Employee Management System
                            using your registered email address.
                        </p>

                        <h3 style="
                            color: #1f2937;
                            font-size: 18px;
                            margin-top: 30px;
                        ">
                            What you can do next
                        </h3>

                        <ul style="
                            padding-left: 20px;
                            line-height: 1.8;
                        ">
                            <li>Sign in to your account</li>
                            <li>Review your employee profile</li>
                            <li>Verify your personal and employment information</li>
                            <li>Complete any pending onboarding requirements</li>
                            <li>Explore the features available to you based on your role</li>
                        </ul>

                        <div style="
                            margin: 25px 0;
                            padding: 15px;
                            background-color: #fff8e1;
                            border-left: 4px solid #f59e0b;
                            border-radius: 4px;
                        ">
                            <strong>Security Notice</strong>
                            <p style="margin-bottom: 0;">
                                Please keep your account credentials confidential
                                and never share them with anyone.
                            </p>
                        </div>

                        <p>
                            If you have any questions or need assistance during the
                            onboarding process, please contact your HR department
                            or system administrator.
                        </p>

                        <p style="
                            margin-top: 30px;
                            font-size: 16px;
                        ">
                            We’re excited to have you with us and wish you a great start!
                        </p>

                        <hr style="
                            border: none;
                            border-top: 1px solid #e5e7eb;
                            margin: 30px 0;
                        ">

                        <p style="
                            margin-bottom: 0;
                            font-size: 13px;
                            color: #6b7280;
                        ">
                            Regards,<br>
                            <strong>Employee Management System</strong>
                        </p>

                    </div>

                </body>
                </html>
                """,
            IsBodyHtml = true
        };

        message.To.Add(email);

        await client.SendMailAsync(message);
    }
}