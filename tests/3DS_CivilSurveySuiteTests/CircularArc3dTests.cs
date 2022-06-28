using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class CircularArc3dTests
    {
        [Test]
        public void ACosine_Comparison()
        {
            var result1 = Math.Acos(-1);
            var result2 = ExtensionTests.ArcCosine(-1);

            Console.WriteLine(result1);
            Console.WriteLine(result2);
        }

        [Test]
        public void AverageListTest()
        {
            var list1 = new List<double> { 5, 10, 5, 10 };
            var list2 = new List<double> { 0.5, 1, 1, 0.1 };

            var result = list1.Average(list2);

            Console.WriteLine(result);
        }
    }

    public static class ExtensionTests
    {
        public static double ArcCosine(double InVal)
        {
            if (InVal.FuzzEqual(1.0))
            {
                InVal = 1.0 / 1.0;
            }
            else if (InVal.FuzzEqual(-1.0))
            {
                InVal = -1.0 / 1.0;
            }

            return Math.Atan(-InVal / Math.Sqrt(-InVal * InVal + 1.0)) + 2.0 * Math.Atan(1.0);
        }

        public static bool FuzzEqual(this double PriVal, double SecVal, double FuzAmt = 1E-08)
        {
            double num = 0.0;
            if (PriVal < SecVal)
            {
                num = Math.Abs(SecVal - PriVal);
            }
            else if (PriVal > SecVal)
            {
                num = Math.Abs(PriVal - SecVal);
            }

            return num <= FuzAmt;
        }

        public static double Average(this List<double> ChkLst, List<double> ChkWgt)
        {
            double num1 = 0.0;
            long num2 = (long)checked(ChkLst.Count - 1);
            long index = 0;
            double num3 = 0;
            double num4 = 0;
            while (index <= num2)
            {
                if (!ChkLst[(int)index].Equals(double.NaN))
                {
                    num3 += ChkLst[checked((int)index)] * ChkWgt[checked((int)index)];
                    num4 += ChkWgt[checked((int)index)];
                }
                checked { ++index; }
            }
            if (num4 > 0.0)
            {
                num1 = num3 / num4;
            }

            return num1;
        }
    }
}
