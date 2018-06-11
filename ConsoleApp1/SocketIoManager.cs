using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

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
        private string _user, _pass, _sala, _passSala;

        public SocketIoManager(string url, string user, string pass, string sala, string passSala)
        {
            _socket = IO.Socket(url);
            _user = user;
            _pass = pass;
            _sala = sala;
            _passSala = passSala;
        }

        public void Conecta()
        {
            _socket.Connect();
        }

        public void Play()
        {
            _socket.On("connect", () =>
            {
                //WriteLog("connect: Recibido evento connect desde el server");               
                var login = new { name = _user, password = _pass };
                _socket.Emit("login", login);
            });

            _socket.On("server_message", (data) =>
            {
                var dataStr = data as string;
                //WriteLog($"server_message: {data}");

                if (dataStr.IndexOf("Logged in as") == 0)
                {                                        
                    var join = new { name = _sala, password = _passSala };
                    _socket.Emit("join_room", join);
                }
            });

            _socket.On("server_state", (msg) =>
            {
                var json = JsonConvert.SerializeObject(msg);

                _serverState = JsonConvert.DeserializeObject<ServerState>(json);
                //WriteLog($"server_state: {msg}");
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
                //WriteLog($"match_start: {msg}");
                _matchData = JsonConvert.DeserializeObject<MatchData>(json);
                jugandoComo = (Lado)(_matchData?.role);
            });

            _socket.On("connect_error", (exception) =>
            {
                var ex = exception as Exception;
                //WriteLog($"Error: {ex?.InnerException?.Message}");
            });
        }

        //jugando a la derecha
        private void PlayAsPlayer1(ServerState serverState)
        {
            DosPorLosOponentes_1PorElBalon(serverState, Lado.derecho);
        }

        //a la izqda
        private void PlayAsPlayer2(ServerState serverState)
        {
            DosPorLosOponentes_1PorElBalon(serverState,Lado.izquierdo);
        }

        private void DosPorLosOponentes_1PorElBalon(ServerState serverState, Lado lado)
        {
            if (_matchData != null)
            {
                var mitalDelCampo = (_matchData.playground_info.field_corners.top_right_x -
                                 _matchData.playground_info.field_corners.top_left_x) / 2;

                Team[] miEquipo;
                Team[] otroEquipo;
                if (lado == Lado.izquierdo)
                {
                    miEquipo = serverState.Team_2;
                    otroEquipo = serverState.Team_1;
                }
                else
                {
                    miEquipo = serverState.Team_1;
                    otroEquipo = serverState.Team_2;
                }

                if (serverState.Match_event == "Kickoff")
                {
                    Thread.Sleep(1000);
                }
                if (lado == Lado.izquierdo)
                {
                    MueveCap(0, serverState, miEquipo, mitalDelCampo, 1.75, 1.15, otroEquipo);
                    MueveCap(1, serverState, miEquipo, mitalDelCampo, 2.75, 1.15, otroEquipo);
                    MueveCap(2, serverState, miEquipo, mitalDelCampo, 3.75, 1.15, otroEquipo);
                }
                else
                {
                    MueveCap(0, serverState, miEquipo, mitalDelCampo, 1.75, 1.15, otroEquipo);
                    MueveCap(1, serverState, miEquipo, mitalDelCampo, 2.75, 1.15, otroEquipo);
                    MueveCap(2, serverState, miEquipo, mitalDelCampo, 3.75, 1.15, otroEquipo);
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

        private void MueveCap(int capNum, ServerState serverState, Team[] miEquipo, 
                              float mitadDelCampo, double coordenadaY_defensa, double coordenadaX_defensa,
                              Team[] otroEquipo)
        {
            if (miEquipo[capNum].cooldown <= 0)
            {
                //cap is ahead to the ball
                if (miEquipo[capNum].x >= serverState.Ball.x)
                {
                    //the cap go back to the goal to defend
                    var angleBetweenCap_Ball = Math.Atan2(serverState.Ball.y - miEquipo[capNum].y, serverState.Ball.x - miEquipo[capNum].x) * 180 / Math.PI;
                    var angleToDefensePosition = (float)(Math.Atan2(coordenadaY_defensa - miEquipo[capNum].y, coordenadaX_defensa - miEquipo[capNum].x) * 180 / Math.PI);
                    float selectedAngle = angleToDefensePosition;
                    if (angleToDefensePosition > angleBetweenCap_Ball - 10 || angleToDefensePosition < angleBetweenCap_Ball + 10)
                    {
                        selectedAngle = (float)angleBetweenCap_Ball - 15;
                    }

                    var mov = new CapMovement()
                    {
                        angle = selectedAngle,
                        cap_num = capNum+1,
                        force = 1.2f
                    };
                    _socket.Emit("client_input", mov);

                }
                else //cap behind the ball
                {
                    if (serverState.Ball.x < mitadDelCampo)
                    {
                        //por su chapa
                        var mov1 = new CapMovement()
                        {
                            angle = (float)(Math.Atan2(otroEquipo[capNum].y - miEquipo[capNum].y, otroEquipo[capNum].x - miEquipo[capNum].x) * 180 / Math.PI),
                            cap_num = capNum + 1,
                            force = 1.2f
                        };
                        _socket.Emit("client_input", mov1);
                    }
                    else
                    {

                        //kick the ball                    
                        var mov = new CapMovement()
                        {
                            angle = (float)(Math.Atan2(serverState.Ball.y - miEquipo[capNum].y, serverState.Ball.x - miEquipo[capNum].x) * 180 / Math.PI),
                            cap_num = capNum + 1,
                            force = 1.2f
                        };
                        _socket.Emit("client_input", mov);

                    }

                }
            }

        }
    } // end class
} //end namespace
