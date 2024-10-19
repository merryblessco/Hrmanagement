namespace HRbackend.Lib
{
    public static class EmailTemplates
    {
        // Welcome email template
        public const string WelcomeEmail = @"
        Hi {FullName},

        Welcome to [CompanyName]! We're excited to have you on board. Please log into your account to complete the onboarding process and explore our services.
        Passcode: {Passcode}

        Best regards,
        [CompanyName] Team
        ";

        // Password reset email template
        public const string PasswordResetEmail = @"
        Hi {FirstName},

        We received a request to reset your password. Click the link below to reset it:
        {ResetLink}

        If you didn't request this, please ignore this email.

        Best regards,
        [CompanyName] Support Team
        ";

        // Account verification email template
        public const string AccountVerificationEmail = @"
        Hi {FirstName},

        Thank you for creating an account with [CompanyName]. Please verify your email address by clicking the link below:
        {VerificationLink}

        Best regards,
        [CompanyName] Team
        ";

        // Invoice email template
        public const string InvoiceEmail = @"
        Hi {FirstName},

        Your invoice for the recent purchase is ready. You can view the details and download the invoice using the link below:
        {InvoiceLink}

        Thank you for your business!

        Best regards,
        [CompanyName] Billing Team
        ";

        // New feature announcement email template
        public const string FeatureAnnouncementEmail = @"
        Hi {FirstName},

        We are excited to announce a new feature on [CompanyName]! You can now {FeatureDescription}. Log into your account to check it out!

        Best regards,
        [CompanyName] Product Team
        ";
    }
}
