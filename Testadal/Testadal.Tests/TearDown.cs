using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Testadal.Tests.Helpers;

namespace Testadal.Tests
{
    [TestFixture]
    public partial class DataContextTests
    {
        /// <summary>
        /// Tear down routine for each Test Fixture
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Type type = (Type)TestContext.CurrentContext.Test.Arguments[0];
            DataContextProvider.DeleteDataContext(type);
        }
    }
}
