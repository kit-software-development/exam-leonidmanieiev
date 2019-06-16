using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Net.Messaging;
using AC.Algos;
using Net.TCP;

namespace TCP.Client
{
    public partial class MainWindow : Form
    {
        // client identifier
        private static int cid;

        public MainWindow()
        {
            Random rand = new Random();
            // assign random id to client
            cid = rand.Next(0, 1000000);
            InitializeComponent();
            statusLabel.Text = "Enter messaga and click Send button";
        }

        // convert string with two component public key into tuple
        private Tuple<long, long> FetchPublicKey(string publicKey)
        {
            string[] n_e = publicKey.Split(' ');
            return Tuple.Create(Convert.ToInt64(n_e[0]), Convert.ToInt64(n_e[1]));
        }

        private void OnIncomingMessage(object sender, IncomingMessageEventArgs e)
        { 
            var publicKey = FetchPublicKey(e._message.ToString());
            string encryptedMessage = RSA.Encrypt(textBox.Text, publicKey);

            // send encrypted message and client identifier
            e._connection.Send(new TextMessage(encryptedMessage + ' ' + cid.ToString()));

            statusLabel.Text = "Message sent";
            textBox.Text = "";
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = TCPConnection.Connect(IPAddress.Loopback, 8080))
                {
                    conn._incomingMessage += OnIncomingMessage;
                    // workaround for response that has not come yet
                    System.Threading.Thread.Sleep(500);
                    conn.Receive();
                }
            } catch (SocketException se)
            {
                statusLabel.Text = "Error. Connection has been closed by server";
            }
        }
    }
}
