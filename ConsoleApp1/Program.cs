using System;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

namespace EverballDotNet
{
    class Program
    {
        static void Main(string[] args)
        {

            var ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();


            var socket = IO.Socket("http://localhost:3000");
            object dataFromServer;

            //Menu();

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("EVENT_CONNECT");
                
                //socket.Disconnect();
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, () =>
            {
                Console.WriteLine("Error connecting");
            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () =>
            {
                Console.WriteLine("Timeout error");
            });

            socket.On(Socket.EVENT_RECONNECTING, () =>
            {
                Console.WriteLine("Reconnecting");
            });

            socket.On("connect", () =>
            {
                Console.WriteLine("connect: Recibido evento connect desde el server");
                var login = new Login
                {
                    userName = "1234568",
                    userPassword = "1234568"
                };

                var datoToSend = JsonConvert.SerializeObject(login);
                socket.Emit("login", datoToSend);
                ManualResetEvent.Set();
            });

            socket.On("server_message", (data) =>
            {
                Console.WriteLine("1st line - server_message: {data}");
                var roomData = new Room
                {
                    RoomName = "One__",
                    RoomPassword = "123"
                };
                var datoToSend = JsonConvert.SerializeObject(roomData);
                socket.Emit("join_room", datoToSend);
                events.Enqueue(data);
                ManualResetEvent.Set();
            });


            socket.On("match_start", (data) =>
            {
                Console.WriteLine("1st line - match_start - {data}");
                dataFromServer = data;
            });

            socket.On("server_state", (data) =>
            {
                Console.WriteLine("1st line - server_state: {data} ");
                var mov = new Movement
                {
                    Angle = 90.0,
                    Force = 0.1,
                    Toy = 1
                };
                socket.Emit("client_input", JsonConvert.SerializeObject(mov));
                events.Enqueue(data);
                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne();
            Console.ReadLine();
        }

        static void Menu()
        {
            Console.WriteLine("1.- Connect to server game");
            Console.WriteLine("5.- Jugar");
            Console.WriteLine("10.- Desconectar");
            var tecla = Console.ReadKey();


            switch (tecla.Key)
            {
                case ConsoleKey.D1:
                    break;
                case ConsoleKey.D5:
                    break;
                case ConsoleKey.D0:
                    break;
            }
        }
    }
}





