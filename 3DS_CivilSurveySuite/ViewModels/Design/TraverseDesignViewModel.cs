using _3DS_CivilSurveySuite.Model;
using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.ViewModels.Design
{
    public class TraverseDesignViewModel
    {
        public ObservableCollection<TraverseObject> TraverseItems
        {
            get
            {
                return new ObservableCollection<TraverseObject>()
                {

                    new TraverseObject()
                    {
                        Index = 0,
                        Bearing = 354.5020,
                        Distance = 34.21,
                    },
                    new TraverseObject()
                    {
                        Index = 1,
                        Bearing = 84.5020,
                        Distance = 20.81,
                    },
                    new TraverseObject()
                    {
                        Index = 2,
                        Bearing = 174.5020,
                        Distance = 20.81,
                    }

                };
            }
        }
    }
}
