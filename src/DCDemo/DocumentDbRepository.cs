using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RavenDemo
{
    public static class DocumentDBRepository<T>
    {
        //Use the Database if it exists, if not create a new Database
        private static Database ReadDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            return db;
        }

        //Use the DocumentCollection if it exists, if not create a new Collection
        private static DocumentCollection ReadCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            return col;
        }

        //Expose the "database" value from configuration as a property for internal use
        private static string databaseId;
        private static string DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = "demodb";
                }

                return databaseId;
            }
        }

        //Expose the "collection" value from configuration as a property for internal use
        private static string collectionId;
        private static string CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = "democollection";
                }

                return collectionId;
            }
        }

        //Use the ReadOrCreateDatabase function to get a reference to the database.
        private static Database database;
        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadDatabase();
                }

                return database;
            }
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private static DocumentCollection collection;
        private static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        //This property establishes a new connection to DocumentDB the first time it is used, 
        //and then reuses this instance for the duration of the application avoiding the
        //overhead of instantiating a new instance of DocumentClient with each request
        private static DocumentClient client;
        private static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    //NOTE: These configurations are here as an example; the database they pointed to has been deleted.
                    string endpoint = "https://dotnetdc-demo.documents.azure.com:443/";
                    string authKey = "rEBc9EIGLanxIEcdTl2WEkfA + qVz7ntdW2GutDqtmChBppl4loq4z7waOoaZyO9zMp98skvrmf0S4zpwKc22uw == ";
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }

        public static IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .Where(predicate)
                .AsEnumerable();
        }

        public static async Task<Document> CreateItemAsync(T item)
        {
            return await Client.CreateDocumentAsync(Collection.SelfLink, item);
        }
    }
}
