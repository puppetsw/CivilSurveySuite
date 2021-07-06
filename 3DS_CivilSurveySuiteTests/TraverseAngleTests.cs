using System;
using System.Collections.Generic;
using System.Diagnostics;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//UnitOfWork_StateUnderTest_ExpectedBehavior
namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseAngleTests
    {
        [TestMethod]
        public void Add_InternalAngle90ToBearing45_Calculate()
        {
            var previousBearing = new Angle(45.0000);
            var internalAngleToAdd = new Angle(90);

            Angle result = previousBearing + internalAngleToAdd;

            Assert.AreEqual(135, result.ToDouble());
        }

        [TestMethod]
        public void Add_InternalAngle90ToBearing315_Calculate()
        {
            var previousBearing = new Angle(315.0000);
            var internalAngleToAdd = new Angle(90);

            Angle result = previousBearing + internalAngleToAdd;

            Assert.AreEqual(45, result.ToDouble());
        }

        [TestMethod]
        public void Add_AdjacentAngle89ToBearing360_Calculate()
        {
            var previousBearing = new Angle(360);
            var adjacentAngleToAdd = new Angle(89);

            var dms180 = new Angle(180);

            Angle internalAngle = dms180 - adjacentAngleToAdd;

            Angle result = previousBearing + internalAngle;

            Assert.AreEqual(91, result.ToDouble());
        }

        [TestMethod]
        public void Add_AdjacentAngle89ToBearing45_Calculate()
        {
            var previousBearing = new Angle(45);
            var adjacentAngleToAdd = new Angle(89);

            var dms180 = new Angle(180);

            Angle internalAngle = dms180 - adjacentAngleToAdd;

            Angle result = previousBearing + internalAngle;

            Assert.AreEqual(136, result.ToDouble());
        }

        [TestMethod]
        public void Calculate_CoordinatesFromTraverseAngleItemList()
        {
            var angleList = new List<TraverseAngleObject>
            {
                new TraverseAngleObject { Distance = 30 },
                new TraverseAngleObject { Distance = 10, InternalAngle = 90 },
                new TraverseAngleObject { Distance = 30, InternalAngle = 90 }
            };

            var newPointList = TraverseAngleCoordinates(angleList);

            Assert.AreEqual(new Coordinate(0, 0), newPointList[0]);
            Assert.AreEqual(new Coordinate(0, 30), newPointList[1]);
            Assert.AreEqual(new Coordinate(10, 30), newPointList[2]);
            Assert.AreEqual(new Coordinate(10, 0), newPointList[3]);
        }

        [TestMethod]
        public void Calculate_CoordinatesFromTraverseAngleItemList_ComplexBoundary_WithInternalAndAdjacent()
        {
            var angleList = new List<TraverseAngleObject>
            {
                new TraverseAngleObject { Distance = 60.35 },
                new TraverseAngleObject { Distance = 111.23, AdjacentAngle = 98.40 }
            };

            var newPointList = TraverseAngleCoordinates(angleList);

            Assert.AreEqual(new Coordinate(0, 0), newPointList[0]);
            Assert.AreEqual(new Coordinate(0, 60.35), newPointList[1]);
            Assert.AreEqual(new Coordinate(-109.9599, 77.1108), newPointList[2]);
        }

        [TestMethod]
        public void Calculate_CoordinatesFromTraverseAngleItemList_ComplexBoundary_WithInternalAndAdjacent_FullBoundary()
        {
            var angleList = new List<TraverseAngleObject>
            {
                new TraverseAngleObject { Distance = 60.35 },
                new TraverseAngleObject { Distance = 111.23, AdjacentAngle = 98.40 },
                new TraverseAngleObject { Distance = 11.51, InternalAngle = 98.07 },
                new TraverseAngleObject { Distance = 38.15, AdjacentAngle = 176.23 },
                new TraverseAngleObject { Distance = 279.55, InternalAngle = 95.29 },
                //new TraverseAngleItem { Distance = 212.35, InternalAngle = 84 },
                //new TraverseAngleItem { Distance = 66.22, AdjacentAngle = 66.11 },
                //new TraverseAngleItem { Distance = 105.66, InternalAngle = 168.58 }
            };

            var newPointList = TraverseAngleCoordinates(angleList);

            Assert.AreEqual(new Coordinate(0, 0).ToString(), newPointList[0].ToString());
            Assert.AreEqual(new Coordinate(0, 60.35).ToString(), newPointList[1].ToString());
            Assert.AreEqual(new Coordinate(-109.9599, 77.1108).ToString(), newPointList[2].ToString());
            Assert.AreEqual(new Coordinate(-109.8494, 88.6203).ToString(), newPointList[3].ToString());
            Assert.AreEqual(new Coordinate(-111.8903, 126.7157).ToString(), newPointList[4].ToString());
            Assert.AreEqual(new Coordinate(164.5529, 168.2771), newPointList[5]);
        }

        [TestMethod]
        public void SetProperty_InternalAngleWithExistingAdjacentAngle_SetAdjacentAngleToEmpty()
        {
            var angle = new TraverseAngleObject { AdjacentAngle = 45 };
            Assert.AreEqual(new Angle(45).ToDouble(), angle.AdjacentAngle);

            angle.InternalAngle = 135;
            Assert.AreEqual(new Angle(135).ToDouble(), angle.InternalAngle);
            Assert.AreEqual(true, angle.DMSAdjacentAngle.IsEmpty);
        }

        private static List<Coordinate> TraverseAngleCoordinates(IReadOnlyList<TraverseAngleObject> angleList)
        {
            var newPointList = new List<Coordinate>();
            var basePoint = new Coordinate(0, 0);
            newPointList.Add(basePoint);

            var lastBearing = new Angle(0);
            for (var i = 0; i < angleList.Count; i++)
            {
                TraverseAngleObject item = angleList[i];
                Angle nextBearing = lastBearing - new Angle(180);

                if (i == 0)
                {
                    nextBearing = new Angle(0);
                }
                else if (!item.DMSInternalAngle.IsEmpty)
                {
                    nextBearing -= item.DMSInternalAngle;
                }
                else if (!item.DMSAdjacentAngle.IsEmpty)
                {
                    nextBearing += item.DMSAdjacentAngle;
                }

                double dec = DMSToDecimalDegrees(nextBearing);
                double rad = DecimalDegreesToRadians(dec);

                double departure = item.Distance * Math.Sin(rad);
                double latitude = item.Distance * Math.Cos(rad);

                double newX = Math.Round(newPointList[i].X + departure, 4);
                double newY = Math.Round(newPointList[i].Y + latitude, 4);

                newPointList.Add(new Coordinate(newX, newY));

                lastBearing = nextBearing;
            }

            return newPointList;
        }

        [DebuggerDisplay("X = {X}, Y = {Y}")]
        private struct Coordinate
        {
            public double X;
            public double Y;

            public Coordinate(double x, double y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"X:{X},Y:{Y}";
            }
        }

        private static double DMSToDecimalDegrees(Angle dms)
        {
            if (dms == null)
                return 0;

            double minutes = (double) dms.Minutes / 60;
            double seconds = (double) dms.Seconds / 3600;

            double decimalDegree = dms.Degrees + minutes + seconds;

            return decimalDegree;
        }

        private static double DecimalDegreesToRadians(double decimalDegrees)
        {
            return decimalDegrees * (Math.PI / 180);
        }
    }
}