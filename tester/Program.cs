using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uNet;
using uNet.Client;
using uNet.Structures.Schemes;

namespace tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            var cl = new uNetClient("127.0.0.1", 1337, new TestScheme());

            if (cl.Connect())
                Console.WriteLine("Connected peer");

            Console.ReadLine();
        }
    }
}
