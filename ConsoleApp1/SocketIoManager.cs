using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;

namespace EverballDotNet
{
    class SocketIoManager
    {
        private Socket _socket;
        object _matchData;

        public SocketIoManager(string url)
        {            
            _socket = IO.Socket(url);
        }

        public void Conecta()
        {
            _socket.Connect();
        }

        public void Play()
        {

            _socket.On("connect", () =>
            {
                Console.WriteLine("connect: Recibido evento connect desde el server");
                var login = new { name = "123456", password = "123456" };
                _socket.Emit("login", login);
            });

            _socket.On("server_message", (data) =>
            {
                var dataStr = data as string;
                Console.WriteLine($"server_message: {data}");

                if (dataStr.IndexOf("Logged in as") == 0)
                {
                    var join = new { name = "One__", password = "123" };
                    _socket.Emit("join_room", join);
                }
            });

            _socket.On("server_state", (msg) =>
            {
                var json = JsonConvert.SerializeObject(msg);

                var serverState = JsonConvert.DeserializeObject<ServerState>(json);
                Console.WriteLine($"serverState: {msg}");
                //if (_matchStart?.Role == 2)
                //{
                //    PlayAsPlayer2(serverState);
                //}
                //else
                //{
                //    PlayAsPlayer1(serverState);
                //}
            });

            _socket.On("match_start", (msg) =>
            {
                var json = JsonConvert.SerializeObject(msg);
                Console.WriteLine($"match_start: {msg}");
                //_matchStart = JsonConvert.DeserializeObject<_matchData>(json);
            });

            _socket.On("connect_error", (exception) =>
            {
                var ex = exception as Exception;
                Console.WriteLine($"Error: {ex?.InnerException?.Message}");
            });
        }

        //private PlayAsPlayer1()
        //{

        //}

        //private PlayAsPlayer2()
        //{

        //}

        public void Disconnect()
        {
            _socket?.Disconnect();
        }
    } // end class
} //end namespace
