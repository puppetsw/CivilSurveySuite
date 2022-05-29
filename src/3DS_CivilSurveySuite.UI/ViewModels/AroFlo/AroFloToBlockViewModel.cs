using System.Collections.ObjectModel;
using System.Windows.Input;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;

namespace _3DS_CivilSurveySuite.UI.ViewModels.AroFlo
{
    public class AroFloToBlockViewModel : ObservableObject
    {
        private readonly IBlockService _blockService;
        private readonly IMessageBoxService _messageBoxService;
        private ObservableCollection<AcadBlock> _blocks;
        private string _projectNumberTag;
        private string _addressTag;
        private string _suburbStatePostcodeTag;
        private AcadBlock _selectedAcadBlock;

        public AcadBlock SelectedAcadBlock
        {
            get => _selectedAcadBlock;
            set => SetProperty(ref _selectedAcadBlock, value);
        }

        public ObservableCollection<AcadBlock> Blocks
        {
            get => _blocks;
            set => SetProperty(ref _blocks, value);
        }

        public string ProjectNumberTag
        {
            get => _projectNumberTag;
            set => SetProperty(ref _projectNumberTag, value);
        }

        public string AddressTag
        {
            get => _addressTag;
            set => SetProperty(ref _addressTag, value);
        }

        public string SuburbStatePostcodeTag
        {
            get => _suburbStatePostcodeTag;
            set => SetProperty(ref _suburbStatePostcodeTag, value);
        }


        public ICommand LoadBlocksCommand { get; private set; }

        public ICommand OkCommand { get; private set; }

        public AroFloToBlockViewModel(IBlockService blockService, IMessageBoxService messageBoxService)
        {
            _blockService = blockService;
            _messageBoxService = messageBoxService;

            InitCommands();
        }

        private void InitCommands()
        {
            LoadBlocksCommand = new RelayCommand(GetBlocks);
        }

        public void GetBlocks()
        {
            Blocks = new ObservableCollection<AcadBlock>(_blockService.GetBlocks());
        }
    }
}
