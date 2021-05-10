using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace exapmle.RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var exchangeName = "textExchangeName";
            var routeKey = "";
            var queueName = "hello";
            var queueName1 = "hello1";
            string queueName2 = "hello2";
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
            //RabbitMQ提供了四种Exchange模式：direct,fanout,topic,header 
            //定义一个Direct类型交换机  Direct模式,可以使用rabbitMQ自带的Exchange：default Exchange 。所以不需要将Exchange进行任何绑定(binding)操作 。消息传递时，RouteKey必须完全匹配，才会被队列接收，否则该消息会被抛弃。
            #region default Direct
            //channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false, null);
            ////声明一个队列
            //channel.QueueDeclare(queueName, false, false, false, null);
            ////将队列绑定到交换机
            //channel.QueueBind(queueName, exchangeName, routeKey, null);
            #endregion

            #region fanout  广播
            //定义一个Direct类型交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false, false, null);

            //定义队列1
            channel.QueueDeclare(queueName1, false, false, false, null);
            //定义队列2
            channel.QueueDeclare(queueName2, false, false, false, null);

            //将队列绑定到交换机
            channel.QueueBind(queueName1, exchangeName, routeKey, null);
            channel.QueueBind(queueName2, exchangeName, routeKey, null);

            //生成两个队列的消费者
            ConsumerGenerator(queueName1);
            ConsumerGenerator(queueName2);

            #endregion

            #region Topic  模糊路由匹配 
            routeKey = "TestRouteKey.*";
            //定义一个Direct类型交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);

            //定义队列1
            channel.QueueDeclare(queueName, false, false, false, null);

            //将队列绑定到交换机
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            #endregion

            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入exit退出！");
            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish(exchangeName, routeKey, null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close();
        }
        /// <summary>
        /// 根据队列名称生成消费者
        /// </summary>
        /// <param name="queueName"></param>
        static void ConsumerGenerator(string queueName)
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

                Console.WriteLine($"Queue:{queueName}收到消息： {message}");
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine($"Queue:{queueName}，消费者已启动");
        }
    }
}
