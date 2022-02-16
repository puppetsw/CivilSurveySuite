// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="CogoPointReplaceDuplicateViewModel"/>.
    /// </summary>
    /// <remarks>
    /// This class will handle the replacement of tree CogoPoints allowing for
    /// duplication so tree spreads and trunks can be scaled correctly.
    /// </remarks>
    public class CogoPointReplaceDuplicateViewModel : ViewModelBase
    {
        private readonly ICogoPointReplaceDuplicateService _cogoPointReplaceDuplicateService;
        private string _findCode;
        private string _replaceCode;
        private string _duplicateCode;
        private int _foundCount;
        private bool _shouldApplyDescriptionKey;
        private bool _shouldOverwriteStyle;
        private bool _shouldReplaceCode;
        private bool _shouldDuplicateApplyDescriptionKey;
        private bool _shouldDuplicateOverwriteStyle;
        private ObservableCollection<string> _symbols;
        private string _replaceSymbol;
        private string _duplicateSymbol;
        private bool _shouldDuplicateCode;
        private string _foundCountString;

        public string FindCode
        {
            get => _findCode;
            set
            {
                _findCode = value;
                NotifyPropertyChanged();
            }
        }

        public int FoundCount
        {
            get => _foundCount;
            set
            {
                _foundCount = value;
                NotifyPropertyChanged();
            }
        }

        public string ReplaceCode
        {
            get => _replaceCode;
            set
            {
                _replaceCode = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldApplyDescriptionKey
        {
            get => _shouldApplyDescriptionKey;
            set
            {
                _shouldApplyDescriptionKey = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldOverwriteStyle
        {
            get => _shouldOverwriteStyle;
            set
            {
                _shouldOverwriteStyle = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldReplaceCode
        {
            get => _shouldReplaceCode;
            set
            {
                _shouldReplaceCode = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldDuplicateCode
        {
            get => _shouldDuplicateCode;
            set
            {
                _shouldDuplicateCode = value;
                NotifyPropertyChanged();
            }
        }

        public string DuplicateCode
        {
            get => _duplicateCode;
            set
            {
                _duplicateCode = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldDuplicateApplyDescriptionKey
        {
            get => _shouldDuplicateApplyDescriptionKey;
            set
            {
                _shouldDuplicateApplyDescriptionKey = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldDuplicateOverwriteStyle
        {
            get => _shouldDuplicateOverwriteStyle;
            set
            {
                _shouldDuplicateOverwriteStyle = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> Symbols
        {
            get => _symbols;
            set
            {
                _symbols = value;
                NotifyPropertyChanged();
            }
        }

        public string ReplaceSymbol
        {
            get => _replaceSymbol;
            set
            {
                _replaceSymbol = value;
                NotifyPropertyChanged();
            }
        }

        public string DuplicateSymbol
        {
            get => _duplicateSymbol;
            set
            {
                _duplicateSymbol = value;
                NotifyPropertyChanged();
            }
        }

        public string FoundCountString
        {
            get => _foundCountString;
            set
            {
                _foundCountString = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand FindReplaceCommand => new RelayCommand(FindReplace, () => true);

        public RelayCommand FindCodeCommand => new RelayCommand(FindCount, () => true);

        private void FindCount()
        {
            _cogoPointReplaceDuplicateService.FindCode = FindCode;
            _cogoPointReplaceDuplicateService.Find();
            FoundCount = _cogoPointReplaceDuplicateService.FoundCount;
            FoundCountString = $"{FoundCount} CogoPoints Found";
        }

        public CogoPointReplaceDuplicateViewModel(ICogoPointReplaceDuplicateService cogoPointReplaceDuplicateService)
        {
            _cogoPointReplaceDuplicateService = cogoPointReplaceDuplicateService;
            Symbols = new ObservableCollection<string>(_cogoPointReplaceDuplicateService.GetCogoPointSymbols());

            if (Symbols.Count > 0)
            {
                ReplaceSymbol = Symbols[0];
                DuplicateSymbol = Symbols[0];
            }
        }

        private void FindReplace()
        {
            _cogoPointReplaceDuplicateService.FindCode = FindCode;
            _cogoPointReplaceDuplicateService.ReplaceCodeText = ReplaceCode;
            _cogoPointReplaceDuplicateService.DuplicateCodeText = DuplicateCode;
            _cogoPointReplaceDuplicateService.ShouldDuplicateCode = ShouldDuplicateCode;
            _cogoPointReplaceDuplicateService.ShouldOverwriteStyle = ShouldOverwriteStyle;
            _cogoPointReplaceDuplicateService.ShouldReplaceCode = ShouldReplaceCode;
            _cogoPointReplaceDuplicateService.ShouldApplyDescriptionKey = ShouldApplyDescriptionKey;
            _cogoPointReplaceDuplicateService.ShouldDuplicateOverwriteStyle = ShouldDuplicateOverwriteStyle;
            _cogoPointReplaceDuplicateService.ShouldDuplicateApplyDescriptionKey = ShouldDuplicateApplyDescriptionKey;
            _cogoPointReplaceDuplicateService.ReplaceSymbol = ReplaceSymbol;
            _cogoPointReplaceDuplicateService.DuplicateSymbol = DuplicateSymbol;
            _cogoPointReplaceDuplicateService.Save();
            _cogoPointReplaceDuplicateService.ReplaceDuplicate();
        }
    }
}