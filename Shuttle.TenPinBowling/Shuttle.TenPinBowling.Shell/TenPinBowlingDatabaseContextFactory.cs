using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class TenPinBowlingDatabaseContextFactory : ITenPinBowlingDatabaseContextFactory
    {
        private readonly DatabaseContextFactory _factory;

        public TenPinBowlingDatabaseContextFactory(IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory, IDatabaseContextCache databaseContextCache)
        {
            _factory = new DatabaseContextFactory(dbConnectionFactory,dbCommandFactory,databaseContextCache);
        }

        public IDatabaseContext Create(string connectionStringName)
        {
            return _factory.Create(connectionStringName);
        }

        public IDatabaseContext Create(string providerName, string connectionString)
        {
            return _factory.Create(providerName, connectionString);
        }

        public IDatabaseContext Create(IDbConnection dbConnection)
        {
            return _factory.Create(dbConnection);
        }

        public IDatabaseContext Create()
        {
            return _factory.Create("Shuttle");
        }
    }
}