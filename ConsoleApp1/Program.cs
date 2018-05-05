using System;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;

namespace EverballDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            object dataFromServer;

            var socket = IO.Socket("http://localhost:3000");
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("EVENT_CONNECT");
                //socket.Disconnect();
            });

            socket.On("connect", () =>
            {
                Console.WriteLine("connect: Recibido evento connect desde el server");
                var login = new Login
                {
                    userName = "1234568",
                    userPassword = "1234568"
                };

                Console.WriteLine("connect - Trying to connect using (" + login.userName + ")...");
                socket.Emit("login", JsonConvert.SerializeObject(login));
            });

            socket.On("server_message", (data) =>
            {
                Console.WriteLine("1st line - server_message: {data}");
                var roomData = new Room
                {
                    RoomName = "One__",
                    RoomPassword = "123"
                };
                socket.Emit("join_room", roomData);
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

            });
        }
    }
}





