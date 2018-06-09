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
            SocketIoManager socket = new SocketIoManager(serverToConnect);

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

            socket.Conecta();
            socket.Play(user, pass);
            Console.ReadLine();
        } //end Main

    } //end class

} //end namespace






