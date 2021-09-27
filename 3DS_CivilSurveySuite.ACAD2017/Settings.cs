// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class Settings
    {
        public static int GraphicsSize
        {
            get => Properties.Settings.Default.Graphics_Size;
            set => Properties.Settings.Default.Graphics_Size = value;
        }

        public static int GraphicsTextSize
        {
            get => Properties.Settings.Default.Graphics_Text_Size;
            set => Properties.Settings.Default.Graphics_Text_Size = value;
        }

        public static short TransientColorIndex
        {
            get => Properties.Settings.Default.Transient_ColorIndex;
            set => Properties.Settings.Default.Transient_ColorIndex = value;
        }
    }
}
