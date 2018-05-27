using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;
using System.IO;

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
                //Console.WriteLine("connect: Recibido evento connect desde el server");
                var login = new { name = "bjabinn2", password = "123456" };
                _socket.Emit("login", login);
            });

            _socket.On("server_message", (data) =>
            {
                var dataStr = data as string;
                WriteLog($"server_message: {data}");

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
                WriteLog($"server_state: {msg}");
                if (jugandoComo == Lado.derecho)
                {
                    PlayAsPlayer1(_serverState);
                }
                else
                {
                    PlayAsPlayer2(_serverState);
                }
            });

            _socket.On("match_start", (msg) =>
            {
                var json = JsonConvert.SerializeObject(msg);
                WriteLog($"match_start: {msg}");
                _matchData = JsonConvert.DeserializeObject<MatchData>(json);
                jugandoComo = (Lado)(_matchData?.role);               
            });

            _socket.On("connect_error", (exception) =>
            {
                var ex = exception as Exception;
                WriteLog($"Error: {ex?.InnerException?.Message}");
            });
        }

        private void PlayAsPlayer1(ServerState serverState)
        {
            WriteLog("PlayAsPlayer1" + JsonConvert.SerializeObject(serverState));
        }

        private void PlayAsPlayer2(ServerState serverState)
        {
            //Chapa 1 y 2 van por sus chapas
            var mov1 = new CapMovement()
            {
                angle = 45,
                cap_num = 1,
                force = 1.2f
            };
            if (serverState.Team_2[0].cooldown == 0)
            {
                _socket.Emit("join_room", mov1);
            }
            

            //chapa 3 va por la bola

        }

        public void Disconnect()
        {
            _socket?.Disconnect();
        }

        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = "C:\\Logs\\";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }
    } // end class
} //end namespace
