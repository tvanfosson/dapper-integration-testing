using System;
using System.Collections.Generic;
using System.Linq;

using DapperTesting.Core.Data;

using FakeItEasy;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperTesting.Core.Tests.Data
{
    [TestClass()]
    public class DapperPostRepositoryTests : TestBase
    {
        private DapperPostRepositoryTestContext _c;

        [TestMethod()]
        public void CreateTest()
        {
            Assert.Fail();
        }

        [TestInitialize]
        public void Init()
        {
            Start();

            _c = new DapperPostRepositoryTestContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_c != null)
            {
                _c.Dispose();
            }

            End();
        }

        private class DapperPostRepositoryTestContext : TestContextBase
        {
            private const string ConnectionStringName = "UserConnectionString";

            public IPostRepository GetRepository()
            {
                var connectionFactory = A.Fake<IConnectionFactory>();

                A.CallTo(() => connectionFactory.Create(ConnectionStringName)).ReturnsLazily(f => GetConnection());

                return new DapperPostRepository(connectionFactory, ConnectionStringName);
            }
        }
    }
}
