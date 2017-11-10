namespace AcmeCorp.CosmosDBSpike.Console
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using AcmeCorp.CosmosDBSpike.Configuration;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    class Program
    {
        private DocumentClient client;

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            try
            {
                Program p = new Program();
                p.GetStartedDemo().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private async Task GetStartedDemo()
        {
            // Create a new instance of the DocumentClient
            this.client = new DocumentClient(new Uri(Connection.EndpointUrl), Connection.PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "FamilyDB" });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("FamilyDB"), new DocumentCollection { Id = "FamilyCollection" });

            // Insert a document, here we create a Family object
            Family andersenFamily = new Family
            {
                Id = "Andersen.2",
                LastName = "Andersen",
                Parents = new Parent[]
                {
                    new Parent { FirstName = "Thomas" },
                    new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FirstName = "Henriette Thaulow",
                        Gender = "female",
                        Grade = 5,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Fluffy" }
                        }
                    }
                },
                District = "WA5",
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = true
            };

            await this.CreateFamilyDocumentIfNotExists("FamilyDB", "FamilyCollection", andersenFamily);

            Family andersonFamilyDocument = await this.client.ReadDocumentAsync<Family>(UriFactory.CreateDocumentUri("FamilyDB", "FamilyCollection", andersenFamily.Id));

            Console.WriteLine("\nRead family {0}", andersonFamilyDocument.Id);

            this.WriteToConsoleAndPromptToContinue("{0}", andersonFamilyDocument);

            Family wakefieldFamily = new Family
            {
                Id = "Wakefield.2",
                LastName = "Wakefield",
                Parents = new Parent[]
                {
                    new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
                    new Parent { FamilyName = "Miller", FirstName = "Ben" }
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Merriam",
                        FirstName = "Jesse",
                        Gender = "female",
                        Grade = 8,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Goofy" },
                            new Pet { GivenName = "Shadow" }
                        }
                    },
                    new Child
                    {
                        FamilyName = "Miller",
                        FirstName = "Lisa",
                        Gender = "female",
                        Grade = 1
                    }
                },
                District = "NY23",
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = false
            };

            await this.CreateFamilyDocumentIfNotExists("FamilyDB", "FamilyCollection", wakefieldFamily);

            Family wakefieldFamilyDocument = await this.client.ReadDocumentAsync<Family>(UriFactory.CreateDocumentUri("FamilyDB", "FamilyCollection", wakefieldFamily.Id));

            Console.WriteLine("\nRead family {0}", wakefieldFamilyDocument.Id);

            this.WriteToConsoleAndPromptToContinue("{0}", wakefieldFamilyDocument);

            this.ExecuteSimpleQuery("FamilyDB", "FamilyCollection");

            // Clean up/delete the database and client
            // await this.client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri("FamilyDB"));
        }

        private async Task CreateFamilyDocumentIfNotExists(string databaseName, string collectionName, Family family)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, family.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), family);
                    this.WriteToConsoleAndPromptToContinue("\nCreated Family {0}", family.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        private void ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            // Run a simple query via LINQ. DocumentDB indexes all properties, so queries can be completed efficiently and with low latency.
            // Here we find the Andersen family via its LastName
            IQueryable<Family> familyQuery = this.client.CreateDocumentQuery<Family>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                .Where(f => f.LastName == "Andersen");

            // The query is executed synchronously here, but can also be executed asynchronously via the IDocumentQuery<T> interface
            Console.WriteLine("\nRunning LINQ query...");
            foreach (Family family in familyQuery)
            {
                Console.WriteLine("\nRead {0}", family);
            }

            // Now execute the same query via direct SQL
            IQueryable<Family> familyQueryInSql = this.client.CreateDocumentQuery<Family>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                "SELECT * FROM Family WHERE Family.LastName = 'Andersen'",
                queryOptions);

            Console.WriteLine("\nRunning direct SQL query...");
            foreach (Family family in familyQueryInSql)
            {
                Console.WriteLine("\nRead {0}", family);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}
