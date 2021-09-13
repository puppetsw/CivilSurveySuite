using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseAngleTests
    {
        [TestMethod]
        public void TraverseAngleObject_New_SetBearing_Invalid()
        {
            var traverseObject = new TraverseAngleObject();
            traverseObject.Bearing = 400; //Invalid Bearing

            var expected = new Angle();
            Assert.AreEqual(expected.ToDouble(), traverseObject.Bearing);
        }

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
                new TraverseAngleObject { Distance = 10, Bearing = 90 },
                new TraverseAngleObject { Distance = 30, Bearing = 90 }
            };

            var newPointList = PointHelpers.TraverseObjectsToCoordinates(angleList, new Point(0, 0));

            var expectedList = new List<Point>
            {
                new Point(0, 0),
                new Point(0, 30),
                new Point(10, 30),
                new Point(10, 0)
            };

            CollectionAssert.AreEqual(expectedList, newPointList);
        }

        [TestMethod]
        public void Calculate_CoordinatesFromTraverseAngleItemList_ComplexBoundary_WithInternalAndAdjacent()
        {
            var angleList = new List<TraverseAngleObject>
            {
                new TraverseAngleObject { Distance = 60.35 },
                new TraverseAngleObject { Distance = 111.23, Bearing = 98.40, RotationDirection = AngleRotationDirection.Positive }
            };

            var newPointList = PointHelpers.TraverseObjectsToCoordinates(angleList, new Point(0, 0));

            var expectedList = new List<Point>
            {
                new Point(0, 0),
                new Point(0, 60.35),
                new Point(-109.9599, 77.1108)
            };

            //CollectionAssert.AreEqual(expectedList, newPointList);

            Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[0].X, newPointList[0].X, 0.0001));
            Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[0].Y, newPointList[0].Y, 0.0001));

            Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[1].X, newPointList[1].X, 0.0001));
            Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[1].Y, newPointList[1].Y, 0.0001));

            Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[1].X, newPointList[1].X, 0.0001));
            Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[1].Y, newPointList[1].Y, 0.0001));
        }

        [TestMethod]
        public void Calculate_CoordinatesFromTraverseAngleItemList_ComplexBoundary_WithInternalAndAdjacent_FullBoundary()
        {
            var angleList = new List<TraverseAngleObject>
            {
                new TraverseAngleObject { Distance = 60.35 },
                new TraverseAngleObject { Distance = 111.23, Bearing = 98.40, RotationDirection = AngleRotationDirection.Positive },
                new TraverseAngleObject { Distance = 11.51, Bearing = 98.07, },
                new TraverseAngleObject { Distance = 38.15, Bearing = 176.23, RotationDirection = AngleRotationDirection.Positive, },
                new TraverseAngleObject { Distance = 279.55, Bearing = 95.29 },
                //new TraverseAngleItem { Distance = 212.35, InternalAngle = 84 },
                //new TraverseAngleItem { Distance = 66.22, AdjacentAngle = 66.11 },
                //new TraverseAngleItem { Distance = 105.66, InternalAngle = 168.58 }
            };

            var newPointList = PointHelpers.TraverseObjectsToCoordinates(angleList, new Point(0, 0));

            var expectedList = new List<Point>
            {
                new Point(0,0),
                new Point(0, 60.35),
                new Point(-109.9599, 77.1108),
                new Point(-109.8494, 88.6203),
                new Point(-111.8904, 126.7156),
                new Point(164.5528, 168.2770)
            };

            //CollectionAssert.AreEqual(expectedList, newPointList);

            for (int i = 0; i < expectedList.Count - 1; i++)
            {
                Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[i].X, newPointList[i].X, 0.0001));
                Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[i].Y, newPointList[i].Y, 0.0001));
            }
        }

        [TestMethod]
        public void Calculate_CoordinatesFromTraverseAngleItemList_ComplexBoundary_WithInternalAndAdjacent_FullBoundary2()
        {
            var angleList = new List<TraverseAngleObject>
            {
                new TraverseAngleObject { Distance = 32.10 },
                new TraverseAngleObject { Distance = 21.03, Bearing = 89.15, },
                new TraverseAngleObject { Distance = 34.49, Bearing = 86.37, RotationDirection = AngleRotationDirection.Positive, ReferenceDirection = AngleReferenceDirection.Forward },
                new TraverseAngleObject { Distance = 8.94, Bearing = 78.17, },
            };

            var newPointList = PointHelpers.TraverseObjectsToCoordinates(angleList, new Point(0, 0));

            var expectedList = new List<Point>
            {
                new Point(0,0),
                new Point(0, 32.10),
                new Point(21.0282, 31.8247),
                new Point(22.6128, -2.6289),
                new Point(13.7849, -1.2175)
            };

            //CollectionAssert.AreEqual(expectedList, newPointList);

            for (int i = 0; i < expectedList.Count - 1; i++)
            {
                Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[i].X, newPointList[i].X, 0.0001));
                Assert.IsTrue(MathHelpers.NearlyEqual(expectedList[i].Y, newPointList[i].Y, 0.0001));
            }
        }
    }
}