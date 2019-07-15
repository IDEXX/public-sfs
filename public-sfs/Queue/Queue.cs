using Microsoft.Azure.ServiceBus;
using smartflowsheet.queue.api.model.consumers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace public_sfs
{
    public static class Queue
    {
        private static readonly MessageHandlerOptions Options = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 5,
            AutoComplete = false,
        };

        private static readonly MessageHandlerOptions DeadLetterOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false,
        };

        public static void StartProcessingMessages(string connectionString, string queueName)
        {
            Console.WriteLine("Initialize QueueConnector");
            QueueConnector.Initialize(connectionString, queueName);

            Console.WriteLine("Start processing messages");

            QueueConnector.Client.RegisterMessageHandler(ProcessMessageAsync, Options);

            QueueConnector.Client.RegisterMessageHandler(ProcessDeadLetterMessageAsync, DeadLetterOptions);
        }

        public static async Task EndProcessingMessages()
        {
            Console.WriteLine("End processing of messages");

            await QueueConnector.Client.CloseAsync();
            await QueueConnector.DeadLetterClient.CloseAsync();
        }

        private static async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Processing a message: {message.SystemProperties.SequenceNumber}");
            try
            {
                IQueueConsumer consumer = new QueueConsumer();
                await consumer.ProcessQueueMessage(message);

                await QueueConnector.Receiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch
            {
                Console.WriteLine("Processing a message crashed!");
            }
        }

        private static async Task ProcessDeadLetterMessageAsync(Message message, CancellationToken token)
        {
            try
            {
                Console.WriteLine($"Processing a Dead Letter message: {message.SystemProperties.SequenceNumber}");
                
                await QueueConnector.DeadLetterReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch
            {
                Console.WriteLine("Processing a Dead Letter message crashed!");
            }
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine("Message processing exception");

            return Task.CompletedTask;
        }
    }
}