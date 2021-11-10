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
        private ObservableCollection<string> _symbols;
        private string _selectedSymbol;
        private string _selectedTrunkSymbol;
        private string _treeCode;
        private int _trunkParameter;
        private int _spreadParameter;

        public ObservableCollection<string> Symbols
        {
            get => _symbols;
            set
            {
                _symbols = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedTreeSymbol
        {
            get => _selectedSymbol;
            set
            {
                _selectedSymbol = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedTrunkSymbol
        {
            get => _selectedTrunkSymbol;
            set
            {
                _selectedTrunkSymbol = value;
                NotifyPropertyChanged();
            }
        }

        public string TreeCode
        {
            get => _treeCode;
            set
            {
                _treeCode = value;
                NotifyPropertyChanged();
            }
        }

        public int TrunkParameter
        {
            get => _trunkParameter;
            set
            {
                _trunkParameter = value;
                NotifyPropertyChanged();
            }
        }

        public int SpreadParameter
        {
            get => _spreadParameter;
            set
            {
                _spreadParameter = value;
                NotifyPropertyChanged();
            }
        }


        public RelayCommand OkCommand => new RelayCommand(Ok, () => true);


        public CogoPointReplaceDuplicateViewModel(ICogoPointReplaceDuplicateService cogoPointReplaceDuplicateService)
        {
            _cogoPointReplaceDuplicateService = cogoPointReplaceDuplicateService;
            Symbols = new ObservableCollection<string>(_cogoPointReplaceDuplicateService.GetCogoPointSymbols());

            TreeCode = _cogoPointReplaceDuplicateService.TreeCode;
            TrunkParameter = _cogoPointReplaceDuplicateService.TrunkParameter;
            SpreadParameter = _cogoPointReplaceDuplicateService.SpreadParameter;


            if (Symbols.Count > 0)
            {
                string tree = _cogoPointReplaceDuplicateService.TreeReplaceSymbol;
                string trunk = _cogoPointReplaceDuplicateService.TrunkReplaceSymbol;

                SelectedTreeSymbol = Symbols.Contains(tree) ? tree : Symbols[0];
                SelectedTrunkSymbol = Symbols.Contains(trunk) ? trunk : Symbols[0];
            }
        }

        private void Ok()
        {
            _cogoPointReplaceDuplicateService.TreeCode = TreeCode;
            _cogoPointReplaceDuplicateService.TrunkParameter = TrunkParameter;
            _cogoPointReplaceDuplicateService.SpreadParameter = SpreadParameter;
            _cogoPointReplaceDuplicateService.TreeReplaceSymbol = SelectedTreeSymbol;
            _cogoPointReplaceDuplicateService.TrunkReplaceSymbol = SelectedTrunkSymbol;
            _cogoPointReplaceDuplicateService.Save();


            _cogoPointReplaceDuplicateService.ReplaceAndDuplicateSymbols();
        }
    }
}