using System;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using System.Threading;

namespace EverballDotNet
{
    class Program
    {
        const int timeToWait = 150;
        static object dataFromServer;
        static Socket socket;

        static private void Connect()
        {
            socket = IO.Socket("http://localhost:3000");
            socket.Connect();
            Console.WriteLine("Connected to server");
        }

        static private void Play()
        {
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

            });

            socket.On("server_message", (data) =>
            {
                Console.WriteLine($"server_message: {data}");
                var roomData = new Room
                {
                    RoomName = "One__",
                    RoomPassword = "123"
                };
                var datoToSend = JsonConvert.SerializeObject(roomData);
                socket.Emit("join_room", datoToSend);
            });

            socket.On("match_start", (data) =>
            {
                Console.WriteLine($"match_start - {data}");
                dataFromServer = data;
            });

            socket.On("server_state", (data) =>
            {
                Console.WriteLine($"server_state: {data} ");
                var mov = new Movement
                {
                    Angle = 90.0,
                    Force = 0.1,
                    Toy = 1
                };
                socket.Emit("client_input", JsonConvert.SerializeObject(mov));
                //Thread.Sleep(timeToWait);
            });
        }

        static void Main(string[] args)
        {
            Connect();
            Play();
        } //end Main

    } //end class

} //end namespace






