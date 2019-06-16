using Net.TCP;
using AC.Algos;
using Net.Messaging;
using System.Net;
using System;

namespace TCP.Server
{
    class Program
    {
        // first component of both public and private keys
        private static long _n;
        // second component of public key
        private static long _e;
        // second component of private key
        private static long _d;

        static void Main(string[] args)
        {
            // listen for incoming connection
            var listener = new TCPConnectionListener(IPAddress.Any, 8080);
            listener._incomingConnection += OnIncomingConnection;
            listener.Start();
            Console.WriteLine("Server started. Press Enter to exit.\n");

            // wait for 'Enter' to stop listener
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter)
                keyInfo = Console.ReadKey();

            listener.Stop();
            Console.WriteLine("Server stoped.");
        }

        private static void OnIncomingConnection(object sender, IncomigConnectionEventArgs e)
        {
            // generate both public and private keys
            _n = RSA.GetN(); _e = RSA.GetE(); _d = RSA.GetD(_e);

            // send public key
            IMessage message = new TextMessage(_n.ToString() + ' ' + _e.ToString());
            e._connection.Send(message);

            e._connection._incomingMessage += OnIncomingMessage;
            // workaround for response that has not come yet
            System.Threading.Thread.Sleep(1000);
            e._connection.Receive();
        }

        // split message and id
        private static Tuple<string, string> GetMessageAndId(string data)
        {
            int i;
            string message = "", id = "";

            for(i = data.Length-1; i >= 0; i--)
            {
                if (data[i] != ' ') id += data[i];
                else break;
            }

            for(int j = 0; j < i; j++) message += data[j];

            return Tuple.Create(message, id);
        }

        private static void OnIncomingMessage(object sender, IncomingMessageEventArgs e)
        {
            // encrypted message and client id
            var messCID = GetMessageAndId(e._message.ToString());
            // decrypt message using private key
            string decryptedMessage = RSA.Decrypt(messCID.Item1, _d, _n);
            Console.WriteLine("client_" + messCID.Item2 + "> " + decryptedMessage);
            e._connection.Dispose();
        }
    }
}
