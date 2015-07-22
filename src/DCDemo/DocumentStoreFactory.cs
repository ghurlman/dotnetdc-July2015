using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RavenDemo
{
    public class DocumentStoreFactory
    {
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

        public static IDocumentStore Store
        {
            get { return store.Value; }
        }

        private static IDocumentStore CreateStore()
        {
            //NOTE: These configurations are here as an example; the database they pointed to has been deleted.
            IDocumentStore store = new DocumentStore()
            {
                Url = "https://meedcraft-azwk.ravenhq.com/databases/meedcraft-dotnetdc-demo",
                ApiKey = "3ef1e21a-cf62-448d-ae87-f76965587568"
            }.Initialize();

            return store;
        }
    }
}
