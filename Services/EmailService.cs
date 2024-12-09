using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OnlineLearningPlatform.Services
{
    public class EmailService
    {
        private readonly string _sendGridApiKey;
        private readonly SendGridClient _client;

        public EmailService(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
            _client = new SendGridClient(_sendGridApiKey);
        }

        public async Task SendCourseCreationEmail(string instructorEmail, string courseName)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("prakharkr.singh14365@gmail.com", "Online Learning Platform"),
                Subject = "New Course Created Successfully",
                PlainTextContent = $"Your course '{courseName}' has been created successfully.",
                HtmlContent = $@"
                    <h2>Course Created Successfully</h2>
                    <p>Your course '<strong>{courseName}</strong>' has been created successfully.</p>
                    <p>You can now start adding content to your course.</p>"
            };
            msg.AddTo(new EmailAddress(instructorEmail));

            await _client.SendEmailAsync(msg);
        }

        public async Task SendEnrollmentEmail(string studentEmail, string courseName)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("prakharkr.singh14365@gmail.com", "Online Learning Platform"),
                Subject = $"Course Enrollment Complete!",
                PlainTextContent = $"You have successfully enrolled in {courseName}.",
                HtmlContent = $@"
                    <h2>Welcome to {courseName}!</h2>
                    <p>You have successfully enrolled in the course.</p>
                    <p>You can start learning right away by accessing your course materials.</p>"
            };
            msg.AddTo(new EmailAddress(studentEmail));

            await _client.SendEmailAsync(msg);
        }
        public async Task SendCourseCreationNotifEmail(string instructorName, string studentEmail, string courseName)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("prakharkr.singh14365@gmail.com", "Online Learning Platform"),
                Subject = "New Course",
                PlainTextContent = $"A new course was added by '{instructorName}'",
                HtmlContent = $@"
                    <h2>New Course '{courseName}'</h2>
                    <p>New course '<strong>{courseName}</strong>' has been created by '{instructorName}'.</p>
                    <p>You can now start adding content to your course.</p>"
            };
            msg.AddTo(new EmailAddress(studentEmail));

            await _client.SendEmailAsync(msg);
        }
    }
}