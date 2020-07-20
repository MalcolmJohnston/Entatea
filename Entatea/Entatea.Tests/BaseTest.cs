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
            DataContextProvider.DeleteDataContext((Type)TestContext.CurrentContext.Test.Arguments[0]);
        }
    }
}
