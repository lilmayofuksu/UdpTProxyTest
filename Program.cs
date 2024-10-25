// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using UdpTproxyTest;

/*while (!Debugger.IsAttached) {
    Console.WriteLine("Debugger not attached, eeping...");
    Thread.Sleep(1000);
}*/

Console.WriteLine("Hello, World!");
var a = new UDPSocket();
a.Server("192.168.1.3", 2323);

