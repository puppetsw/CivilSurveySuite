using System.Collections.ObjectModel;

namespace CivilSurveySuite.Shared.Models
{
    public class AcadBlock : ObservableObject
    {
        private string _objectId;
        private string _name;

        public string ObjectId
        {
            get => _objectId;
            set => SetProperty(ref _objectId, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<AcadBlockAttribute> Attributes { get; set; }

        public AcadBlock()
        {
            Attributes = new ObservableCollection<AcadBlockAttribute>();
        }
    }

    public class AcadBlockAttribute : ObservableObject
    {
        private string _tag;
        private string _text;

        public string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}
