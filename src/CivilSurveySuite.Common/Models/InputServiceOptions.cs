// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace CivilSurveySuite.Shared.Models
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