// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace _3DS_CivilSurveySuite.UI.Services
{
    /// <summary>
    /// Interface IDialogService
    /// </summary>
    public interface IDialogService<T>
    {
        bool? DialogResult { get; set; }

        T ResultObject { get; set; }

        bool? ShowDialog();
    }
}