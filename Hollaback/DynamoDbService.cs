using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace Hollaback
{
    public class DynamoDbService
    {
        private const string postedItemsTable = "posted-items";

        private AmazonDynamoDBClient _dynamoDbClient;

        public DynamoDbService(AmazonDynamoDBClient dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<bool> IsPosted(string itemId)
        {
            try
            { 
                var table = Table.LoadTable(_dynamoDbClient, postedItemsTable);

                var retrievedId = await table.GetItemAsync(new Primitive(itemId));

                return retrievedId != null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to put item: {e.Message} - {e.StackTrace}");

                return false;
            }
        }

        public async Task<bool> SetPosted(string itemId)
        {
            try
            {
                var table = Table.LoadTable(_dynamoDbClient, postedItemsTable);

                var document = Document.FromJson(JsonConvert.SerializeObject(new Item { Id = itemId }));

                await table.PutItemAsync(document);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to put item: {e.Message} - {e.StackTrace}");

                return false;
            }
        }
    }
}
