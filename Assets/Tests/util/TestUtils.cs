using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class TestUtils
    {
        public static TestCaseData CreateTestCaseData(string name, params object[] args)
        {
            return new TestCaseData(args).SetName(name + "{a}");
        }
    }
}
