using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class MathUtilsTest
    {
        [TestCase(0, TestName = "with0{a}", ExpectedResult = 0)]
        [TestCase(90, TestName = "withPositiveAngle{a}", ExpectedResult = 90)]
        [TestCase(179, TestName = "withPositiveAngleJustBeforeRange{a}", ExpectedResult = 179)]
        [TestCase(180, TestName = "withPositiveAngleJustAfterRange{a}", ExpectedResult = -180)]
        [TestCase(359, TestName = "withPositiveAngleJustBeforeFullCircle{a}", ExpectedResult = -1)]
        [TestCase(360, TestName = "withPositiveAngleAtFullCircle{a}", ExpectedResult = 0)]
        [TestCase(540, TestName = "withPositiveAngleAtOneAnfHalfCircles{a}", ExpectedResult = -180)]
        [TestCase(720, TestName = "withPositiveAngleAtTwoCircles{a}", ExpectedResult = 0)]
        [TestCase(-90, TestName = "withNegativeAngle{a}", ExpectedResult = -90)]
        [TestCase(-180, TestName = "withNegativeAngleJustBeforeRange{a}", ExpectedResult = -180)]
        [TestCase(-181, TestName = "withNegativeAngleJustAfterRange{a}", ExpectedResult = 179)]
        [TestCase(-359, TestName = "withNegativeAngleJustBeforeFullCircle{a}", ExpectedResult = 1)]
        [TestCase(-360, TestName = "withNegativeAngleAtFullCircle{a}", ExpectedResult = 0)]
        [TestCase(-540, TestName = "withNegativeAngleAtOneAnfHalfCircles{a}", ExpectedResult = -180)]
        [TestCase(-720, TestName = "withNegativeAngleAtTwoCircles{a}", ExpectedResult = 0)]
        [Test]
        public float TestToSignedAngle(float angle)
        {
            return MathUtils.ToSignedAngle(angle);
        }

        [TestCase(2, 1, TestName = "withPositiveValueCapped{a}", ExpectedResult = 1)]
        [TestCase(-2, 1, TestName = "withNegativeValueCapped{a}", ExpectedResult = -1)]
        [TestCase(0.5f, 1, TestName = "withPositiveValueNotCapped{a}", ExpectedResult = 0.5f)]
        [TestCase(-0.5f, 1, TestName = "withNegativeValueNotCapped{a}", ExpectedResult = -0.5f)]
        [Test]
        public float TestAbsMin(float value, float minValue)
        {
            return MathUtils.AbsMin(value, minValue);
        }

        [TestCase(0.5f, 1, TestName = "withPositiveValueCapped{a}", ExpectedResult = 1)]
        [TestCase(-0.5f, 1, TestName = "withNegativeValueCapped{a}", ExpectedResult = -1)]
        [TestCase(2, 1, TestName = "withPositiveNotValueCapped{a}", ExpectedResult = 2)]
        [TestCase(-2, 1, TestName = "withNegativeNotValueCapped{a}", ExpectedResult = -2)]
        [Test]
        public float TestAbsMax(float value, float maxValue)
        {
            return MathUtils.AbsMax(value, maxValue);
        }
    }
}
