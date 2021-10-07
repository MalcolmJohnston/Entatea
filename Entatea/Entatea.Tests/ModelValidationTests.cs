using System;

using Entatea.Tests.Entities;
using NUnit.Framework;

namespace Entatea.Tests.ModelValidation
{
    public class ModelValidationTests
    {
        [Test]
        public void Sequential_Partition_Key_Must_Be_Only_Key()
        {
            // Act / Assert
            Assert.Throws<ArgumentException>(() => ClassMapper.GetClassMap<InvalidPartition>());
        }
    }
}
