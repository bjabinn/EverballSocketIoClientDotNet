using System;

namespace EverballDotNet
{
    class Program
    {      
        static void Main(string[] args)
        {
            Console.Write("Servidor: ");
            string serverToConnect = Console.ReadLine();
            if (string.IsNullOrEmpty(serverToConnect)) {            
                serverToConnect = "http://code-game.com:3000";
            }
            

            Console.Write("Usuario: ");
            var user = Console.ReadLine();
            if (string.IsNullOrEmpty(user))
            {
                user = "bjabinn2";
            }

            Console.Write("Contraseña: ");
            var pass = Console.ReadLine();
            if (string.IsNullOrEmpty(pass))
            {
                pass = "123456";
            }

            Console.Write("Sala: ");
            var sala = Console.ReadLine();
            if (string.IsNullOrEmpty(sala))
            {
                sala = "One__";
            }

            Console.Write("Password de la sala: ");
            var passSala = Console.ReadLine();
            if (string.IsNullOrEmpty(passSala))
            {
                passSala = "123";
            }

            SocketIoManager socket = new SocketIoManager(serverToConnect, user, pass, sala, passSala);

            socket.Conecta();
            socket.Play();
            Console.ReadLine();
        } //end Main

    } //end class

} //end namespace






