using Resend;

namespace Api.Services;

public interface IEmailService
{
    Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);
}

public class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IResend resend, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _resend = resend;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
    {
        try
        {
            var baseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
            var fromEmail = _configuration["App:FromEmail"] 
              ?? throw new InvalidOperationException("From email is not configured. Set App:FromEmail in configuration.");
            var resetUrl = $"{baseUrl}/reset-password?token={resetToken}";
            
            var htmlContent = GeneratePasswordResetEmailHtml(userName, resetUrl);
            
            var message = new EmailMessage
            {
                From = fromEmail,
                To = new[] { toEmail },
                Subject = "Reset Your Password",
                HtmlBody = htmlContent
            };

            var response = await _resend.EmailSendAsync(message);
            
            // For now, assume success if no exception is thrown
            _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", toEmail);
            return false;
        }
    }

private string GeneratePasswordResetEmailHtml(string userName, string resetUrl)
{
    return $@"
<!DOCTYPE html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Reset your password</title>
  <!--[if gte mso 9]>
  <xml>
    <o:OfficeDocumentSettings>
      <o:AllowPNG/>
      <o:PixelsPerInch>96</o:PixelsPerInch>
    </o:OfficeDocumentSettings>
  </xml>
  <![endif]-->
  <style>
    /* Base resets */
    html, body {{ margin:0 !important; padding:0 !important; height:100% !important; width:100% !important; }}
    * {{ -ms-text-size-adjust:100%; -webkit-text-size-adjust:100%; }}
    table, td {{ mso-table-lspace:0pt !important; mso-table-rspace:0pt !important; }}
    img {{ -ms-interpolation-mode:bicubic; border:0; outline:none; text-decoration:none; }}
    a {{ text-decoration:none; }}
    /* iOS blue links */
    a[x-apple-data-detectors] {{ color:inherit !important; text-decoration:none !important; }}
    /* Gmail dark mode support */
    @media (prefers-color-scheme: dark) {{
      body, .email-bg {{ background:#0b0d12 !important; }}
      .card {{ background:#111827 !important; border-color:#1f2937 !important; }}
      .muted, .footer-text {{ color:#9ca3af !important; }}
      .heading, .body-text {{ color:#e5e7eb !important; }}
      .link {{ color:#93c5fd !important; }}
    }}
    /* Mobile */
    @media screen and (max-width: 600px) {{
      .container {{ width:100% !important; }}
      .px-24 {{ padding-left:16px !important; padding-right:16px !important; }}
      .py-32 {{ padding-top:24px !important; padding-bottom:24px !important; }}
      .btn {{ width:100% !important; display:block !important; }}
    }}
  </style>
</head>
<body class=""email-bg"" style=""background:#f3f4f6; margin:0; padding:0;"">
  <!-- Preheader (hidden preview text) -->
  <div style=""display:none; font-size:1px; color:#f3f4f6; line-height:1px; max-height:0; max-width:0; opacity:0; overflow:hidden;"">
    Reset your password — link expires in 30 minutes.
  </div>

  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"">
    <tr>
      <td align=""center"" style=""padding:32px 12px;"">
        <!-- Card -->
        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" class=""container card"" style=""width:600px; max-width:600px; background:#ffffff; border-radius:12px; border:1px solid #e5e7eb; box-shadow:0 6px 24px rgba(2,6,23,0.08);"">
          <!-- Header / Logo -->
          <tr>
            <td class=""px-24"" style=""padding:28px 24px 0 24px;"">
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td align=""center"">
                    <div class=""muted"" style=""font-family:Segoe UI, Arial, sans-serif; font-size:12px; color:#6b7280;"">Security Notification</div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Hero -->
          <tr>
            <td class=""px-24"" style=""padding:20px 24px 0 24px;"">
              <table role=""presentation"" width=""100%"">
                <tr>
                  <td align=""left"">
                    <h1 class=""heading"" style=""margin:0; font-family:Segoe UI, Arial, sans-serif; font-size:22px; line-height:1.4; color:#111827;"">
                      Reset your password
                    </h1>
                  </td>
                </tr>
                <tr>
                  <td style=""padding-top:8px;"">
                    <p class=""body-text"" style=""margin:0; font-family:Segoe UI, Arial, sans-serif; font-size:15px; line-height:1.7; color:#374151;"">
                      Hi {userName}, we received a request to reset your password. Click the button below to create a new one.
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- CTA Button -->
          <tr>
            <td class=""px-24"" align=""center"" style=""padding:24px 24px 0 24px;"">
              <!--[if mso]>
              <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word""
                href=""{resetUrl}"" style=""height:48px; v-text-anchor:middle; width:280px;"" arcsize=""12%""
                strokecolor=""#3B82F6"" fillcolor=""#3B82F6"">
                <w:anchorlock/>
                <center style=""color:#ffffff; font-family:Segoe UI, Arial, sans-serif; font-size:16px; font-weight:bold;"">
                  Reset Password
                </center>
              </v:roundrect>
              <![endif]-->
              <!--[if !mso]><!-- -->
              <a href=""{resetUrl}"" class=""btn""
                 style=""display:inline-block; background:#3B82F6; color:#ffffff; font-family:Segoe UI, Arial, sans-serif;
                        font-size:16px; font-weight:700; line-height:48px; text-align:center; border-radius:8px;
                        padding:0 28px; border:1px solid #2563EB; box-shadow:0 6px 16px rgba(59,130,246,0.35);"">
                Reset Password
              </a>
              <!--<![endif]-->
            </td>
          </tr>

          <!-- Help text -->
          <tr>
            <td class=""px-24"" style=""padding:20px 24px 0 24px;"">
              <p class=""body-text"" style=""margin:0; font-family:Segoe UI, Arial, sans-serif; font-size:14px; line-height:1.8; color:#374151;"">
                This link will expire in <strong>30 minutes</strong> for your security.
              </p>
            </td>
          </tr>

          <!-- Security Notice -->
          <tr>
            <td class=""px-24"" style=""padding:16px 24px 0 24px;"">
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                     style=""border:1px solid #fde68a; border-radius:8px; background:#fffbeb;"">
                <tr>
                  <td style=""padding:14px 14px 14px 14px;"">
                    <p style=""margin:0; font-family:Segoe UI, Arial, sans-serif; font-size:13px; line-height:1.7; color:#78350f;"">
                      <strong>Didn't request this?</strong> You can safely ignore this email—your password will remain unchanged.
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Fallback URL -->
          <tr>
            <td class=""px-24"" style=""padding:18px 24px 0 24px;"">
              <p class=""muted"" style=""margin:0; font-family:Segoe UI, Arial, sans-serif; font-size:13px; line-height:1.7; color:#6b7280;"">
                Having trouble with the button? Paste this link into your browser:
              </p>
              <p class=""link"" style=""margin:6px 0 0; font-family:Segoe UI, Arial, sans-serif; font-size:13px; line-height:1.6; color:#2563EB; word-break:break-all;"">
                {resetUrl}
              </p>
            </td>
          </tr>

          <!-- Divider -->
          <tr>
            <td style=""padding:24px 24px 0 24px;"">
              <hr style=""border:none; border-top:1px solid #e5e7eb; margin:0;"">
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td class=""px-24 py-32"" style=""padding:18px 24px 28px 24px;"">
              <table role=""presentation"" width=""100%"">
                <tr>
                  <td align=""center"">
                    <p class=""footer-text"" style=""margin:0; font-family:Segoe UI, Arial, sans-serif; font-size:12px; color:#9ca3af; line-height:1.7;"">
                      This is an automated message—please don't reply.
                    </p>
                    <p class=""footer-text"" style=""margin:2px 0 0; font-family:Segoe UI, Arial, sans-serif; font-size:12px; color:#9ca3af; line-height:1.7;"">
                      Need help? Contact our support team.
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

        </table>
        <!-- /Card -->

        <!-- Tiny legal line (optional) -->
        <div style=""font-family:Segoe UI, Arial, sans-serif; font-size:10px; color:#9ca3af; text-align:center; padding:12px 8px;"">
          You're receiving this email because a password reset was requested for your account.
        </div>
      </td>
    </tr>
  </table>
</body>
</html>";
}
}
