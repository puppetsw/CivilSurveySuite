namespace CivilSurveySuite.Common.Models
{
    public sealed class InputServiceOptions
    {
        public InputServiceOptions(string title, string message, string primaryButtonText)
        {
            Title = title;
            Message = message;
            PrimaryButtonText = primaryButtonText;
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public string PrimaryButtonText { get; set; }

    }
}