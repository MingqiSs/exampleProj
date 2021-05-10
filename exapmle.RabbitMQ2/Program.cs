using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace exapmle.RabbitMQ2
{
    class Program
    {
		/// <summary>
		/// 定义消费者
		/// </summary>
		/// <param name="args"></param>
        static void Main(string[] args)
        {

			//创建连接工厂
			ConnectionFactory factory = new ConnectionFactory
			{
				UserName = "guest",//用户名
				Password = "guest",//密码
				HostName = "192.168.1.77"//rabbitmq ip
			};

			//创建连接
			var connection = factory.CreateConnection();
			//创建通道
			var channel = connection.CreateModel();

			//事件基本消费者
			EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

			//接收到消息事件
			consumer.Received += (ch, ea) =>
			{
				var message = Encoding.UTF8.GetString(ea.Body.ToArray());
				Console.WriteLine($"收到消息： {message}");
				channel.BasicAck(ea.DeliveryTag, false);
				Console.WriteLine($"已发送回执[{ea.DeliveryTag}]");
			};
			//启动消费者 设置为手动应答消息
			channel.BasicConsume("hello1", false, consumer);
			Console.WriteLine("消费者已启动");
			Console.ReadKey();
			channel.Dispose();
			connection.Close();
		}
    }
}
