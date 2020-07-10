using System;

using NUnit.Framework;
using Testadal.Tests.Helpers;

namespace Testadal.Tests
{
    public class BaseTest
    {
        [TearDown]
        public void TearDown()
        {
            DataContextProvider.DeleteDataContext((Type)TestContext.CurrentContext.Test.Arguments[0]);
        }
    }
}
