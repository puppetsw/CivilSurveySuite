using _3DS_CivilSurveySuite.Models;
using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class TraverseDesignViewModel
    {
        public ObservableCollection<TraverseItem> TraverseItems
        {
            get
            {
                return new ObservableCollection<TraverseItem>()
                {

                    new TraverseItem()
                    {
                        Index = 0,
                        Bearing = 354.5020,
                        Distance = 34.21,
                    },
                    new TraverseItem()
                    {
                        Index = 1,
                        Bearing = 84.5020,
                        Distance = 20.81,
                    },
                    new TraverseItem()
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
