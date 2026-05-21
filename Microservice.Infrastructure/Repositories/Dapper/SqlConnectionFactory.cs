using Microservice.Application.Contracts.Persistence.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Microservice.Infrastructure.Repositories.Dapper
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default");
                //?? throw new InvalidOperationException("Connection string not configured");
        }

        public async Task<IDbConnection> CreateAsync(CancellationToken ct = default)
        {
            var connection = new SqlConnection(_connectionString);

            //await connection.OpenAsync(ct);

            return connection;
        }
    }

}