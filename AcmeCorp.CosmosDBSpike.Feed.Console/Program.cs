namespace AcmeCorp.CosmosDBSpike.Feed.Console
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AcmeCorp.CosmosDBSpike.Configuration;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.ChangeFeedProcessor;
    using Microsoft.Azure.Documents.Client;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start (allow other console to create database.");
            Console.ReadLine();
            Run().Wait();
            Console.WriteLine("Finished.");
            Console.ReadLine();
        }

        private static async Task Run()
        {
            ChangeFeedOptions feedOptions = new ChangeFeedOptions { StartFromBeginning = true };
            // Can customize LeaseRenewInterval, LeaseAcquireInterval, LeaseExpirationInterval, FeedPollDelay
            ChangeFeedHostOptions feedHostOptions = new ChangeFeedHostOptions { LeaseRenewInterval = TimeSpan.FromSeconds(15) };
            DocumentCollectionInfo documentCollectionLocation = new DocumentCollectionInfo
            {
                Uri = new Uri(Connection.EndpointUrl),
                CollectionName = "FamilyCollection",
                // ConnectionPolicy = new ConnectionPolicy{},
                DatabaseName = "FamilyDB",
                MasterKey = Connection.PrimaryKey
            };
            ChangeFeedEventHost changeFeedEventHost = new ChangeFeedEventHost("hostName", documentCollectionLocation, documentCollectionLocation, feedOptions, feedHostOptions);
            ChangeFeedObserverFactory changeFeedObserverFactory = new ChangeFeedObserverFactory();
            await changeFeedEventHost.RegisterObserverFactoryAsync(changeFeedObserverFactory);
            await changeFeedEventHost.UnregisterObserversAsync();
        }
    }

    public class ChangeFeedObserver : IChangeFeedObserver
    {
        public Task OpenAsync(ChangeFeedObserverContext context)
        {
            return Task.FromResult<object>(null);
        }

        public Task CloseAsync(ChangeFeedObserverContext context, ChangeFeedObserverCloseReason reason)
        {
            Console.WriteLine($"Connection closed for reason '{reason}'.");
            return Task.FromResult<object>(null);
        }

        public Task ProcessChangesAsync(ChangeFeedObserverContext context, IReadOnlyList<Document> docs)
        {
            foreach (Document document in docs)
            {
                Console.WriteLine($"Found document with ID '{document.Id}'.");
            }

            return Task.FromResult<object>(null);
        }
    }

    public class ChangeFeedObserverFactory : IChangeFeedObserverFactory
    {
        public IChangeFeedObserver CreateObserver()
        {
            return new ChangeFeedObserver();
        }
    }
}
