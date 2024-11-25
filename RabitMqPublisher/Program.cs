using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMqPublisher;

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

        Console.WriteLine("Publisher запущен. Введите текст для отправки сообщений. Для выхода введите `exit`.");

        while (true)
        {
            Console.Write("Введите сообщение: ");
            var message = Console.ReadLine();

            if (message?.ToLower() == "exit")
            {
                Console.WriteLine("Завершение работы Publisher.");
                break;
            }

            if (message != null)
            {
                var body = Encoding.UTF8.GetBytes(message);
            
                var basicProperties = new BasicProperties();

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "hello",
                    mandatory: false,
                    basicProperties: basicProperties,
                    body: body,
                    cancellationToken: CancellationToken.None);
            }

            Console.WriteLine($"Отправлено: {message}");
        }
    }
}