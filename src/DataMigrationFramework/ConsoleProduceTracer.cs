using System;
using System.Threading;

namespace DataMigrationFramework
{
    public class ConsoleProduceTracer : IProducerTracer
    {
        public void Log(string name, string message)
        {
            Console.WriteLine($"[{System.DateTime.Now}][{Thread.CurrentThread.ManagedThreadId}][{name}] {message}");
        }
    }
}
