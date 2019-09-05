using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class ClampedValueTest
    {
        private const float TestInitialValue = 0;
        private const float TestMinValue = -100;
        private const float TestMaxValue = 100;

        private ClampedValue sut;
        /*
        [SetUp]
        public void SetUp()
        {
            sut = new ClampedValue(TestInitialValue, TestMinValue, TestMaxValue);
        }

        [TestCase(TestMinValue - 1, TestName = "atMinValue{a}", ExpectedResult = TestMinValue)]
        [TestCase(TestMaxValue + 1, TestName = "atMaxValue{a}", ExpectedResult = TestMaxValue)]
        [Test]
        public float TestCtorDoesNotExceedBorder(float valueToInitialize)
        {
            return new ClampedValue(valueToInitialize, TestMinValue, TestMaxValue).Value;
        }

        [TestCase(TestMinValue - 1, TestName = "atMinValue{a}", ExpectedResult = TestMinValue)]
        [TestCase(TestMaxValue + 1, TestName = "atMaxValue{a}", ExpectedResult = TestMaxValue)]
        [Test]
        public float TestAddDoesNotExceedBorder(float valueToAdd)
        {
            sut.Value += valueToAdd;
            return sut.Value;
        }*/
    }
}
