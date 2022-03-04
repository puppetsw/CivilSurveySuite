// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface IFolderBrowserDialogService
    {
        bool? ShowDialog();

        string SelectedPath { get; set; }

        string Description { get; set; }
    }
}