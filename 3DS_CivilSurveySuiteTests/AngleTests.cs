using System;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class AngleTests
    {
        [TestMethod]
        public void Angle_ParseTest_Double()
        {
            // Need to test multiple parses.
            // These bearings have given me the most trouble.
            var bearing1 = 176.23;
            var bearing2 = 95.29;
            var bearing3 = 359.5959;

            var angle1 = new Angle(bearing1);
            var angle2 = new Angle(bearing2);
            var angle3 = new Angle(bearing3);

            var expectedAngle1 = new Angle { Degrees = 176, Minutes = 23 };
            var expectedAngle2 = new Angle { Degrees = 95, Minutes = 29 };
            var expectedAngle3 = new Angle { Degrees = 359, Minutes = 59, Seconds = 59 };

            Assert.AreEqual(expectedAngle1, angle1);
            Assert.AreEqual(expectedAngle2, angle2);
            Assert.AreEqual(expectedAngle3, angle3);
        }

        [TestMethod]
        public void Angle_ParseTest_String()
        {
            // Need to test multiple parses.
            var bearing1 = "176.23";
            var bearing2 = "95.29";
            var bearing3 = "359.5959";

            var angle1 = new Angle(bearing1);
            var angle2 = new Angle(bearing2);
            var angle3 = new Angle(bearing3);

            var expectedAngle1 = new Angle { Degrees = 176, Minutes = 23 };
            var expectedAngle2 = new Angle { Degrees = 95, Minutes = 29 };
            var expectedAngle3 = new Angle { Degrees = 359, Minutes = 59, Seconds = 59 };

            Assert.AreEqual(expectedAngle1, angle1);
            Assert.AreEqual(expectedAngle2, angle2);
            Assert.AreEqual(expectedAngle3, angle3);
        }

        [TestMethod]
        public void Angle_TryParseTest_ValidBearing()
        {
            var inputBearing = 356.0010;

            var resultBool = Angle.TryParse(inputBearing, out Angle angle);

            var expectedAngle = new Angle(356.0010);

            Assert.AreEqual(true, resultBool);
            Assert.AreEqual(expectedAngle, angle);
        }

        [TestMethod]
        public void Angle_TryParseTest_InvalidBearing_ShouldBeFalse()
        {
            var inputBearing = 456.9080;

            var resultBool = Angle.TryParse(inputBearing, out Angle angle);

            Assert.AreEqual(false, resultBool);
            Assert.AreEqual(null, angle);
        }

        [TestMethod]
        public void Angle_TryParseTest_NullBearing_ShouldBeFalse()
        {
            double? inputBearing = null;

            var resultBool = Angle.TryParse(inputBearing, out Angle angle);

            Assert.AreEqual(false, resultBool);
            Assert.AreEqual(null, angle);
        }

        [TestMethod]
        public void Angle_IsValidTest_ShouldBeTrue()
        {
            //angle problems 176.23//95.29
            var inputAngle = new Angle(95.29);
            var expected = true;
            var result = Angle.IsValid(inputAngle);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_IsValidTest_ShouldBeFalse()
        {
            var inputAngle = new Angle(999.9999);
            var expected = false;
            var result = Angle.IsValid(inputAngle);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_ToStringTest()
        {
            var angle = new Angle(359.5959);
            var expectedString = "359°59'59\"";
            var result = angle.ToString();

            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void Angle_ToStringTest_SecondsLessThan10()
        {
            var angle1 = new Angle(90.0009);
            var result = angle1.ToString();

            var expected = "90°00'09\"";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_ToDoubleTest()
        {
            var angle = new Angle(359.5959);
            var expectedString = 359.5959;
            var result = angle.ToDouble();

            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void Angle_AddTest()
        {
            var angle1 = new Angle(45);
            var angle2 = new Angle(45);

            var result = angle1 + angle2;

            var expected = new Angle(90);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_AddTest_Int()
        {
            var angle1 = new Angle(45);

            var result = angle1 + 45;

            var expected = new Angle(90);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_AddTest_SecondsGreaterThan60()
        {
            var angle1 = new Angle(90.0040);
            var angle2 = new Angle(90.0040);

            var result = angle1 + angle2;
            var expected = new Angle(180.0120);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_SubtractTest()
        {
            var angle1 = new Angle(270);
            var angle2 = new Angle(90);

            var result = angle1 - angle2;

            var expected = new Angle(180);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_SubtractTest_Int()
        {
            var angle1 = new Angle(270);
            var angle2 = 90;

            var result = angle1 - angle2;

            var expected = new Angle(180);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Angle_EqualsTest()
        {
            var angle1 = new Angle(270);
            var angle2 = new Angle(270);

            Assert.AreEqual(angle1, angle2);
        }

        [TestMethod]
        public void Angle_EqualsTest_ShouldBeFalse()
        {
            var angle = new Angle(0);
            Assert.IsFalse(angle.Equals(null));
        }

        [TestMethod]
        public void Angle_Constructor_InvalidString_ExpectedZeroAngle()
        {
            var angle = new Angle("expectedfail");
            var expected = new Angle();

            Assert.AreEqual(expected, angle);
        }

        [TestMethod]
        public void Angle_Construct_Degrees_Minutes_Seconds()
        {
            var angle = new Angle(90, 30, 30);
            var expectedString = "90°30\'30\"";
            
            Assert.AreEqual(expectedString, angle.ToString());
        }

        [TestMethod]
        public void Angle_CounterClockwise_To_Clockwise()
        {
            var angle = new Angle(270);
            var expectedAngle = new Angle(180);

            Assert.AreEqual(expectedAngle, angle.ToClockwise());
        }

        [TestMethod]
        public void Angle_Clockwise_To_CounterClockwise()
        {
            var angle = new Angle(180);
            var expectedAngle = new Angle(270);

            Assert.AreEqual(expectedAngle, angle.ToCounterClockwise());
        }

        [TestMethod]
        public void Angle_Flip_180Degrees()
        {
            var angle = new Angle(0);
            var expectedAngle = new Angle(180);

            Assert.AreEqual(expectedAngle, angle.Flip());
        }

        [TestMethod]
        public void Angle_Subtract_90_PossibleIssue()
        {
            var angle = new Angle(209, 09, 22);
            angle -= 90;

            Assert.AreEqual(new Angle(119, 09, 22), angle);
        }

        [TestMethod]
        public void Angle_Add_90_PossibleIssue()
        {
            var angle = new Angle(209, 9, 22);
            angle += 90;

            Assert.AreEqual(new Angle(299, 9, 22), angle);
        }
    }
}