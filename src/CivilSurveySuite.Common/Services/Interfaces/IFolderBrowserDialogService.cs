﻿namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface IFolderBrowserDialogService
    {
        bool? ShowDialog();

        string SelectedPath { get; set; }

        string Description { get; set; }
    }
}