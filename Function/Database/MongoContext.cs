using daedalus.Shared.Model;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;

namespace daedalus.Function.Database
{
    public class MongoContext
    {
        private MongoClient _client { get; set; }
        private IMongoDatabase Database { get; set; }

        /// <summary>
        /// Options from Startup, used to setup db connection
        /// </summary>
        /// <param name="applicationSettings"></param>
        public MongoContext()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            _client = new MongoClient(connectionString);

            Setup("daedalus");
        }

        private void Setup(string database)
        {
            ConventionPack pack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("IgnoreExtraElements", pack, t => true);

            Database = _client.GetDatabase(database);

            //Link the accessible collections to actual DB collections
            Conditions = Database.GetCollection<Condition>("conditions");
        }

        //Define the collections which are accessible
        public IMongoCollection<Condition> Conditions { get; set; }
    }
}