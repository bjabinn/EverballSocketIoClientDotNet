using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;

namespace EverballDotNet
{
    enum Lado
    {
        derecho = 1,
        izquierdo = 2
    }

    class SocketIoManager
    {
        private Socket _socket;
        private ServerState _serverState;
        private MatchData _matchData;
        private Lado jugandoComo;

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
                var login = new { name = "bjabinn2", password = "123456" };
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

                _serverState = JsonConvert.DeserializeObject<ServerState>(json);
                Console.WriteLine($"serverState: {msg}");
                if (jugandoComo == Lado.derecho)
                {
                    PlayAsPlayer1(_serverState, _matchData);
                }
                else
                {
                    PlayAsPlayer2(_serverState, _matchData);
                }
            });

            _socket.On("match_start", (msg) =>
            {
                var json = JsonConvert.SerializeObject(msg);
                Console.WriteLine($"match_start: {msg}");
                _matchData = JsonConvert.DeserializeObject<MatchData>(json);
                jugandoComo = (Lado)(_matchData?.role);               
            });

            _socket.On("connect_error", (exception) =>
            {
                var ex = exception as Exception;
                Console.WriteLine($"Error: {ex?.InnerException?.Message}");
            });
        }

        private void PlayAsPlayer1(ServerState serverState, MatchData matchData)
        {
            Console.WriteLine(JsonConvert.SerializeObject(serverState));
        }

        private void PlayAsPlayer2(ServerState serverState, MatchData matchData)
        {
            Console.WriteLine(JsonConvert.SerializeObject(serverState));
        }

        public void Disconnect()
        {
            _socket?.Disconnect();
        }
    } // end class
} //end namespace
