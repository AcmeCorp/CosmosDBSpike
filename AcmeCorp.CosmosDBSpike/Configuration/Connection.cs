namespace AcmeCorp.CosmosDBSpike.Configuration
{
    public static class Connection
    {
        // Emulator (Windows)
        public const string EndpointUrl = "https://localhost:8081/";
        public const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // Emulator (Docker)
        // public const string EndpointUrl = "https://172.28.244.23:8081/";
        // public const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // Azure
        // public const string EndpointUrl = "https://acmecorp-cosmosdbspike.documents.azure.com:443/";
        // public const string PrimaryKey = "HrRccXPxAkxyNfNOa7X4NS7fknhlGhxmbn2MA4b9hPgOnPgHzPqAa9bxskXNP3p9kpSETxDpI4Hrjhnc4iXrKg==";
    }
}
