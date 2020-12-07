using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Show_song_text.Interfaces;
using Show_song_text.Models;
using Xamarin.Forms;
using Show_song_text.Utils;

/*
 * Thanks for Jesse C. Slicer and Jamal for this soluion from https://codereview.stackexchange.com/questions/24758/tcp-async-socket-server-client-communication
 * Not everything is needed for me, but this solution work perfect and exactly as I want. I add only messegingcanter to send information and recive text from this class. 
 */

namespace Show_song_text.PresentationServerUtilis
{
    public delegate void ConnectedHandler(IAsyncClient a);
    public delegate void ClientMessageReceivedHandler(IAsyncClient a, string msg);
    public delegate void ClientMessageSubmittedHandler(IAsyncClient a, bool close);

    public sealed class AsyncClient : IAsyncClient
    {

        private Socket listener;
        private bool close;

        private static readonly IAsyncClient instance = new AsyncClient();

        private readonly ManualResetEvent connected = new ManualResetEvent(false);
        private readonly ManualResetEvent sent = new ManualResetEvent(false);
        private readonly ManualResetEvent received = new ManualResetEvent(false);

        public event ConnectedHandler Connected;

        public event ClientMessageReceivedHandler MessageReceived;

        public event ClientMessageSubmittedHandler MessageSubmitted;


        private AsyncClient()
        {
        }

        public static IAsyncClient Instance
        {
            get
            {
                return instance;
            }
        }
        public void StartClient(int port, IPAddress ip)
        {
            var endpoint = new IPEndPoint(ip, port);

            try
            {
                this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listener.BeginConnect(endpoint, this.OnConnectCallback, this.listener);
                this.connected.WaitOne();

                var connectedHandler = this.Connected;

                if (connectedHandler != null)
                {
                    connectedHandler(this);
                }
            }
            catch (SocketException)
            {
                // TODO:
            }
        }

        public bool IsConnected()
        {
            return !(this.listener.Poll(1000, SelectMode.SelectRead) && this.listener.Available == 0);
        }

        private void OnConnectCallback(IAsyncResult result)
        {
            var server = (Socket)result.AsyncState;

            try
            {
                server.EndConnect(result);
                this.connected.Set();
                MessagingCenter.Send(this, Events.ConnectToServer, true);

            }
            catch (SocketException)
            {
            }
        }

        #region Receive data
        public void Receive()
        {
            var state = new StateObject(this.listener);
            try
            {
                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
            }
            catch (Exception e)
            {

                Console.WriteLine("Client Recive: " + e.Message + " " + e.InnerException.Message);
            }
            
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            var state = (IStateObject)result.AsyncState;
            var receive = state.Listener.EndReceive(result);

            if (receive > 0)
            {
                state.Append(Encoding.UTF8.GetString(state.Buffer, 0, receive));
            }

            if (receive == state.BufferSize)
            {
                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
            }
            else
            {
                var messageReceived = this.MessageReceived;

                if (messageReceived != null)
                {
                    messageReceived(this, state.Text);
                }
                MessagingCenter.Send(this, Events.SendedText, state.Text);

                state.Reset();
                this.received.Set();
                Receive();
            }
        }
        #endregion

        #region Send data
        public void Send(string msg, bool close)
        {
            if (!this.IsConnected())
            {
                throw new Exception("Destination socket is not connected.");
            }

            var response = Encoding.UTF8.GetBytes(msg);

            this.close = close;
            this.listener.BeginSend(response, 0, response.Length, SocketFlags.None, this.SendCallback, this.listener);
        }

        private void SendCallback(IAsyncResult result)
        {
            try
            {
                var resceiver = (Socket)result.AsyncState;

                resceiver.EndSend(result);
            }
            catch (SocketException)
            {
                // TODO:
            }
            catch (ObjectDisposedException)
            {
                // TODO;
            }

            var messageSubmitted = this.MessageSubmitted;

            if (messageSubmitted != null)
            {
                messageSubmitted(this, this.close);
            }

            this.sent.Set();
        }
        #endregion

        private void Close()
        {
            try
            {
                if (!this.IsConnected())
                {
                    return;
                }

                this.listener.Shutdown(SocketShutdown.Both);
                this.listener.Close();
            }
            catch (SocketException)
            {
                // TODO:
            }
        }

        public void Dispose()
        {
            this.connected.Dispose();
            this.sent.Dispose();
            this.received.Dispose();
            this.Close();
        }
    }

}