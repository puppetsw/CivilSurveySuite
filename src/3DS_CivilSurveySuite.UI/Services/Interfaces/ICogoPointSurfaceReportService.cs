// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using System.Data;
using _3DS_CivilSurveySuite.UI.Models;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointSurfaceReportService
    {
        DataTable DataTable { get; }

        ObservableCollection<ReportCivilSurfaceOptions> CivilSurfaceOptions { get; }

        ObservableCollection<ReportCivilAlignmentOptions> CivilAlignmentOptions { get; }

        ObservableCollection<ReportCivilPointGroupOptions> CivilPointGroupOptions { get; }

        void GenerateReport();
    }

    public class ReportCivilSurfaceOptions
    {
        public CivilSurface CivilSurface { get; set; }

        public CivilSurfaceProperties CivilSurfaceProperties { get; set; }
    }

    public class CivilSurfaceProperties : ObservableObject
    {
        private bool _interpolateLevels;
        private double _interpolateMaximumDistance;
        private bool _showCutFill;
        private bool _invertCutFill;

        public bool InterpolateLevels
        {
            get => _interpolateLevels;
            set => SetProperty(ref _interpolateLevels, value);
        }

        public double InterpolateMaximumDistance
        {
            get => _interpolateMaximumDistance;
            set => SetProperty(ref _interpolateMaximumDistance, value);
        }

        public bool ShowCutFill
        {
            get => _showCutFill;
            set => SetProperty(ref _showCutFill, value);
        }

        public bool InvertCutFill
        {
            get => _invertCutFill;
            set => SetProperty(ref _invertCutFill, value);
        }
    }


    public class ReportCivilAlignmentOptions
    {
        public CivilAlignment CivilAlignment { get; set; }

        public CivilAlignmentProperties CivilAlignmentProperties { get; set;}
    }


    public class CivilAlignmentProperties : ObservableObject
    {
        private bool _roundChainageValues;
        private double _roundChainageValuesAmount;

        public bool RoundChainageValues
        {
            get => _roundChainageValues;
            set => SetProperty(ref _roundChainageValues, value);
        }

        public double RoundChainageValuesAmount
        {
            get => _roundChainageValuesAmount;
            set => SetProperty(ref _roundChainageValuesAmount, value);
        }
    }

    public class ReportCivilPointGroupOptions
    {
        public CivilPointGroup CivilPointGroup { get; set; }

        public CivilPointGroupProperties CivilPointGroupProperties { get; set; }
    }

    public class CivilPointGroupProperties : ObservableObject
    {
        private bool _allowDuplicates;

        public bool AllowDuplicates
        {
            get => _allowDuplicates;
            set => SetProperty(ref _allowDuplicates, value);
        }
    }

}
