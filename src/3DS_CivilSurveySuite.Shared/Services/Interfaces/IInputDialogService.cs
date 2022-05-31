// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Shared.Models;

namespace _3DS_CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IInputDialogService
    {
        bool? DialogResult { get; set; }

        string ResultString { get; set; }

        bool? ShowDialog();

        void AssignOptions(InputServiceOptions options);
    }
}