using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace public_sfs
{
    public static class QueueConnector
    {
        // Thread-safe. Recommended that you cache rather than recreating it
        // on every request.
        public static QueueClient Client;
        public static QueueClient DeadLetterClient;

        public static void Initialize(string connectionString, string queueName)
        {
            NamespaceManager nsManager = NamespaceManager.CreateFromConnectionString(connectionString);
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(connectionString);

            QueueDescription queue_description = new QueueDescription(queueName);
            queue_description.DefaultMessageTimeToLive = new TimeSpan(0, 0, 10);
            queue_description.EnableDeadLetteringOnMessageExpiration = true;
            queue_description.MaxDeliveryCount = 4;

            Client = factory.CreateQueueClient(queueName);
            Client.RetryPolicy = RetryExponential.Default;

            var dfQueue = QueueClient.FormatDeadLetterPath(Client.Path);
            DeadLetterClient = factory.CreateQueueClient(dfQueue);
        }
    }
}
