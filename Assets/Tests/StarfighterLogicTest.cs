using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class StarfighterLogicTest
    {
        private const float TestStartTime = 10;
        private const float TestFireRate = 0.5f;
        private const int TestAddedQuadrantsEachHorizontalDirection = 4;
        private const int TestAddedQuadrantsEachVerticalDirection = 3;
        private const int TestQuadrantSize = 100;
        private const float TestMoveBorderSoftDistance = 200;
        private const float TestMoveBorderHardDistance = 100;
        private const float TestSoftBorderX = 250; //(TestAddedQuadrantsEachHorizontalDirection + 0.5f) * TestQuadrantSize - TestMoveBorderSoftDistance;
        private const float TestHardBorderX = 350; //(TestAddedQuadrantsEachHorizontalDirection + 0.5f) * TestQuadrantSize - TestMoveBorderHardDistance;
        private const float TestSoftBorderY = 150; //(TestAddedQuadrantsEachVerticalDirection + 0.5f) * TestQuadrantSize - TestMoveBorderSoftDistance;
        private const float TestHardBorderY = 250; //(TestAddedQuadrantsEachVerticalDirection + 0.5f) * TestQuadrantSize - TestMoveBorderHardDistance;

        // sidewards move speed = manouverability * 100 * 100 / throttle = 1000 in one second to overshoot any borders
        private const float TestThrottle = 100;
        private const float TestManouverabilityAt100 = 10;
        private const float TestDeltaTime = 1;

        private StarfighterLogic sut;

        [SetUp]
        public void SetUp()
        {
            sut = new StarfighterLogic();
        }

        [Test]
        public void TestCanFireAtTheBeginning()
        {
            Assert.IsTrue(sut.CanFire(0, 1));
        }

        [Test]
        public void TestCanNotFireBelowCooldown()
        {
            sut.Fire(TestStartTime);

            // You can't fire before 12
            Assert.IsFalse(sut.CanFire(TestStartTime + 1, TestFireRate));
        }

        [Test]
        public void TestCanFireAboveCooldown()
        {
            sut.Fire(TestStartTime);

            // You can fire with 12s onwards
            Assert.IsTrue(sut.CanFire(TestStartTime + 2, TestFireRate));
        }

        public static IEnumerable StopsOnHardBorderTestCases
        {
            get
            {
                yield return TestUtils.CreateTestCaseData("FromWithinSoftBorderLeft", (TestSoftBorderX + 10) * Vector3.left, 1 * Vector3.left).Returns(TestHardBorderX * Vector3.left);
                yield return TestUtils.CreateTestCaseData("FromWithinSoftBorderRight", (TestSoftBorderX + 10) * Vector3.right, 1 * Vector3.right).Returns(TestHardBorderX * Vector3.right);
                yield return TestUtils.CreateTestCaseData("FromWithinSoftBorderDown", (TestSoftBorderY + 10) * Vector3.down, 1 * Vector3.down).Returns(TestHardBorderY * Vector3.down);
                yield return TestUtils.CreateTestCaseData("FromWithinSoftBorderUp", (TestSoftBorderY + 10) * Vector3.up, 1 * Vector3.up).Returns(TestHardBorderY * Vector3.up);
                yield return TestUtils.CreateTestCaseData("FromWithinSoftBorderUpLeft", new Vector3(-(TestSoftBorderX + 10), TestSoftBorderY + 10, 0), new Vector3(-1, 1,0).normalized).Returns(new Vector3(-TestHardBorderX, TestHardBorderY, 0));
                yield return TestUtils.CreateTestCaseData("FromWithinSoftBorderDownRight", new Vector3(TestSoftBorderX + 10, -(TestSoftBorderY + 10), 0), new Vector3(1, -1, 0).normalized).Returns(new Vector3(TestHardBorderX, -TestHardBorderY, 0));
                yield return TestUtils.CreateTestCaseData("FromOutsideSoftBorderLeft", (TestSoftBorderX - 10) * Vector3.left, 1 * Vector3.left).Returns(TestHardBorderX * Vector3.left);
                yield return TestUtils.CreateTestCaseData("FromOutsideSoftBorderRight", (TestSoftBorderX - 10) * Vector3.right, 1 * Vector3.right).Returns(TestHardBorderX * Vector3.right);
                yield return TestUtils.CreateTestCaseData("FromOutsideSoftBorderDown", (TestSoftBorderY - 10) * Vector3.down, 1 * Vector3.down).Returns(TestHardBorderY * Vector3.down);
                yield return TestUtils.CreateTestCaseData("FromOutsideSoftBorderUp", (TestSoftBorderY - 10) * Vector3.up, 1 * Vector3.up).Returns(TestHardBorderY * Vector3.up);
                yield return TestUtils.CreateTestCaseData("FromOutsideSoftBorderUpLeft", new Vector3(-(TestSoftBorderX - 10), TestSoftBorderY - 10, 0), new Vector3(-1, 1, 0).normalized).Returns(new Vector3(-TestHardBorderX, TestHardBorderY, 0));
                yield return TestUtils.CreateTestCaseData("FromOutsideSoftBorderDownRight", new Vector3(TestSoftBorderX - 10, -(TestSoftBorderY - 10), 0), new Vector3(1, -1, 0).normalized).Returns(new Vector3(TestHardBorderX, -TestHardBorderY, 0));
            }
        }

        /*[TestCaseSource("StopsOnHardBorderTestCases")]
        public Vector3 TestStopsOnHardBorder(Vector3 position, Vector3 moveAxis)
        {
            return sut.GetPosition(position, moveAxis, TestThrottle, TestManouverabilityAt100,
                TestAddedQuadrantsEachHorizontalDirection, TestAddedQuadrantsEachVerticalDirection, TestQuadrantSize,
                TestMoveBorderSoftDistance, TestMoveBorderHardDistance, TestDeltaTime);
        }*/
    }
}
