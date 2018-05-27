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
                    //var join = new { name = "Test room", password = "abc" };
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

        //jugando a la derecha
        private void PlayAsPlayer1(ServerState serverState)
        {
            //DosPorLosOponentes_1PorElBalon(serverState);
        }

        //a la izqda
        private void PlayAsPlayer2(ServerState serverState)
        {
            DosPorLosOponentes_1PorElBalon(serverState);
        }

        private void DosPorLosOponentes_1PorElBalon(ServerState serverState)
        {
            if (serverState.Ball.x < 4.3)
            {                
                if (serverState.Team_2[0].cooldown <= 0)
                {
                    var mov1 = new CapMovement()
                    {
                        angle = (float)(Math.Atan2(1.5 - serverState.Team_2[0].y, 1.15 - serverState.Team_2[0].x) * 180 / Math.PI),
                        cap_num = 1,
                        force = 1.2f
                    };
                    _socket.Emit("client_input", mov1);
                }

                if (serverState.Team_2[1].cooldown <= 0)
                {
                    var mov2 = new CapMovement()
                    {
                        angle = (float)(Math.Atan2(2.5 - serverState.Team_2[1].y, 1.15 - serverState.Team_2[1].x) * 180 / Math.PI),
                        cap_num = 2,
                        force = 1.2f
                    };

                    _socket.Emit("client_input", mov2);
                }

                //chapa 3 va por la bola
                if (serverState.Team_2[2].cooldown <= 0)
                {
                    var mov3 = new CapMovement()
                    {
                        angle = (float)(Math.Atan2(3.5 - serverState.Team_2[2].y, 1.15 - serverState.Team_2[2].x) * 180 / Math.PI),
                        cap_num = 3,
                        force = 1.2f
                    };

                    _socket.Emit("client_input", mov3);
                }
            }
            else
            {
                //Chapa 1 y 2 van por sus chapas
                if (serverState.Team_2[0].cooldown <= 0)
                {
                    var mov1 = new CapMovement()
                    {
                        angle = (float)(Math.Atan2(serverState.Team_1[0].y - serverState.Team_2[0].y, serverState.Team_1[0].x - serverState.Team_2[0].x) * 180 / Math.PI),
                        cap_num = 1,
                        force = 1.2f
                    };
                    _socket.Emit("client_input", mov1);
                }

                if (serverState.Team_2[1].cooldown <= 0)
                {
                    var mov2 = new CapMovement()
                    {
                        angle = (float)(Math.Atan2(serverState.Team_1[1].y - serverState.Team_2[1].y, serverState.Team_1[1].x - serverState.Team_2[1].x) * 180 / Math.PI),
                        cap_num = 2,
                        force = 1.2f
                    };

                    _socket.Emit("client_input", mov2);
                }

                //chapa 3 va por la bola
                if (serverState.Team_2[2].cooldown <= 0)
                {
                    var mov3 = new CapMovement()
                    {
                        angle = (float)(Math.Atan2(serverState.Ball.y - serverState.Team_2[2].y, serverState.Ball.x - serverState.Team_2[2].x) * 180 / Math.PI),
                        cap_num = 3,
                        force = 1.2f
                    };

                    _socket.Emit("client_input", mov3);
                }
            }
            
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
