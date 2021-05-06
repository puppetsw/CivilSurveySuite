namespace _3DS_CivilSurveySuite.Model
{
    public class DMSCalc : DMS
    {
        public static DMSCalc operator +(DMSCalc dms1, DMSCalc dms2)
        {
            var degrees = dms1.Degrees + dms2.Degrees;
            var minutes = dms1.Minutes + dms2.Minutes;
            var seconds = dms1.Seconds + dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (seconds >= 60)
            {
                seconds -= 60;
                minutes++;
            }

            //work out minutes, carry over to degrees
            if (minutes >= 60)
            {
                minutes -= 60;
                degrees++;
            }

            return new DMSCalc() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public static DMSCalc operator -(DMSCalc dms1, DMSCalc dms2)
        {
            var degrees = dms1.Degrees - dms2.Degrees;
            var minutes = dms1.Minutes - dms2.Minutes;
            var seconds = dms1.Seconds - dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (dms1.Seconds < dms2.Seconds)
            {
                minutes--;
                seconds += 60;
            }

            //work out minutes, carry over to degrees
            if (dms1.Minutes < dms2.Minutes)
            {
                degrees--;
                minutes += 60;
            }

            return new DMSCalc() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        
    }
}
