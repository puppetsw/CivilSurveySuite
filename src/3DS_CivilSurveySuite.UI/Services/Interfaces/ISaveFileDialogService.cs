// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace _3DS_CivilSurveySuite.UI.Services.Interfaces
{
    public interface ISaveFileDialogService
    {
        bool? ShowDialog();

        string FileName { get; set; }

        string DefaultExt { get; set; }

        string Filter { get; set; }
    }
}