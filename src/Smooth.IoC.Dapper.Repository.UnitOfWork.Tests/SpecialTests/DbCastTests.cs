﻿using System.Data;
using FakeItEasy;
using NUnit.Framework;
using Smooth.IoC.Repository.UnitOfWork.Tests.TestHelpers;
using Smooth.IoC.UnitOfWork;

namespace Smooth.IoC.Repository.UnitOfWork.Tests.SpecialTests
{
    [TestFixture]
    public class DbCastTests
    {
        [Test, Category("Integration")]
        public static void ISession_Is_IDbConnectionAndIsConnected()
        {
            using (var session = new TestSessionMemory(A.Fake<IDbFactory>()))
            {
                Assert.That(session.State, Is.EqualTo(ConnectionState.Open));
                var connection = (IDbConnection) session;
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }

            using (IDbConnection session = new TestSessionMemory(A.Fake<IDbFactory>()))
            {
                Assert.That(session.State, Is.EqualTo(ConnectionState.Open));
            }
        }
        [Test, Category("Integration")]
        public static void IUnitOfWork_Is_IDbTransactionAndIsConnected()
        {
            var factory = A.Fake<IDbFactory>();
            var session = new TestSessionMemory(factory);

            using (var uow = new IoC.UnitOfWork.UnitOfWork(factory, session, IsolationLevel.Serializable))
            {
                Assert.That(uow.Connection.State, Is.EqualTo(ConnectionState.Open));
                var transaction = (IDbTransaction)uow;
                Assert.That(transaction.Connection.State, Is.EqualTo(ConnectionState.Open));
            }

            using (IDbTransaction uow = new IoC.UnitOfWork.UnitOfWork(factory, session, IsolationLevel.Serializable))
            {
                Assert.That(uow.Connection.State, Is.EqualTo(ConnectionState.Open));
            }
        }

    }
}
