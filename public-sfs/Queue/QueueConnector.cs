using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace public_sfs
{
    public static class QueueConnector
    {
        // Thread-safe. Recommended that you cache rather than recreating it on every request.
        public static IMessageReceiver Receiver { get; private set; }

        public static IMessageReceiver DeadLetterReceiver { get; private set; }

        public static void Initialize(string connectionString, string queueName)
        {
            Receiver = new MessageReceiver(connectionString, queueName);

            string deadLetterQueue = EntityNameHelper.FormatDeadLetterPath(Receiver.Path);
            DeadLetterReceiver = new MessageReceiver(connectionString, deadLetterQueue);
        }
    }
}