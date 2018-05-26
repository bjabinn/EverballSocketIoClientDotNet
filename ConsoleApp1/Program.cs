using System;

namespace EverballDotNet
{
    class Program
    {
        //static SocketIoManager socket = new SocketIoManager("http://localhost:3000");
        static SocketIoManager socket = new SocketIoManager("http://code-game.com:3000");

        static void Main(string[] args)
        {
            socket.Conecta();
            socket.Play();
            Console.ReadLine();
        } //end Main

    } //end class

} //end namespace






