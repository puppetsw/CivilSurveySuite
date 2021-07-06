// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.


using Autodesk.Civil.ApplicationServices;

namespace _3DS_CivilSurveySuite_C3DBase21
{
    public static class CivilApplicationManager
    {
        public static CivilDocument ActiveCivilDocument => CivilApplication.ActiveDocument;
    }
}