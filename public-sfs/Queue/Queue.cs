using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using smartflowsheet.queue.api.model.consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace public_sfs
{
    public static class Queue
    {
        private static OnMessageOptions options = new OnMessageOptions
        {
            MaxConcurrentCalls = 5,
            AutoComplete = false,
        };

        private static OnMessageOptions deadLetterOptions = new OnMessageOptions
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false,
        };

        public static void BeginProcessingMessages(string connectionString, string queueName)
        {
            Console.WriteLine("Initialize QueueConnector");
            QueueConnector.Initialize(connectionString, queueName);

            Console.WriteLine("Start processing messages");

            QueueConnector.Client.OnMessageAsync(async receivedMessage =>
            {
                try
                {
                    // asynchronouse processing of messages
                    await Task.Run(() => ProcessMessageAsync(receivedMessage));

                    // complete if successful processing
                    await receivedMessage.CompleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }, options);

            QueueConnector.DeadLetterClient.OnMessageAsync(async deadMessage =>
            {
                try
                {
                    await Task.Run(() =>
                    {
                        Console.WriteLine("Processing DeadLetter message: " + deadMessage.SequenceNumber.ToString());
                    });
                    await deadMessage.CompleteAsync();
                }
                catch
                {
                    Console.WriteLine("Processing DeadLetter message crashed!");
                }
            }, deadLetterOptions);
        }

        public static void EndProcessingMessages()
        {
            Console.WriteLine("End processing messages");

            QueueConnector.Client.Close();
            QueueConnector.DeadLetterClient.Close();
        }

        private static void ProcessMessageAsync(BrokeredMessage message)
        {
            IQueueConsumer consumer = new QueueConsumer();
            consumer.ProcessQueueMessage(message);
        }
    }
}
