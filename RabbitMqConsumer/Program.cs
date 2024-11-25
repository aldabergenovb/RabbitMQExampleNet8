using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqConsumer;

abstract class Program
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, es) =>
        {
            var body = es.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Ok, received: {0}", message);
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: "hello",
            autoAck: true,
            consumer: consumer
        );
        
        Console.WriteLine("Press [enter] to exit");
        while (Console.ReadLine() != "exit")
        {
            Thread.Sleep(1000);
        }
        
    }
}