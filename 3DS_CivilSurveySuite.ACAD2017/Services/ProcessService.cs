// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Diagnostics;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.ACAD2017.Services
{
    public class ProcessService : IProcessService
    {
        public void Start(string fileName)
        {
            _ = Process.Start(fileName);
        }
    }
}