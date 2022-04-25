using System;
using System.ServiceModel;

namespace ChartixServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(ChartiX.ServiceChat)))
            {
                host.Open();
                Console.WriteLine("Сервер запущен");
                Console.ReadLine();
            }
        }
    }
}
