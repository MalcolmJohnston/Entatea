using System;

using NUnit.Framework;
using Entatea.Tests.Helpers;

namespace Entatea.Tests
{
    public class BaseTest
    {
        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Test.Arguments.Length > 0)
            {
                DataContextTestHelper.DeleteDataContext((Type)TestContext.CurrentContext.Test.Arguments[0]);
            }
        }
    }
}
