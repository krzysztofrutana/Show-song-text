using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Show_song_text.Interfaces;
using Show_song_text.Models;
using Xamarin.Forms;
using Show_song_text.Utils;
using Show_song_text.Helpers;
using System.Net.NetworkInformation;

/*
 * Thanks for Jesse C. Slicer and Jamal for this soluion from https://codereview.stackexchange.com/questions/24758/tcp-async-socket-server-client-communication
 * Not everything is needed for me, but this solution work perfect and exactly as I want. I add only messegingcanter to send information and recive text from this class. 
 */

namespace Show_song_text.PresentationServerUtilis
{

    public sealed class AsyncClient : IAsyncClient
    {

        private Socket listener;

        private static readonly IAsyncClient instance = new AsyncClient();

        private readonly ManualResetEvent connected = new ManualResetEvent(false);

        public Boolean ItsConenctedToServer = false;

        private static string TAG = "AsyncClient";

        private static readonly ILogInterface Log = DependencyService.Get<ILogInterface>();


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

        #region Connection
        public void StartClient(int port, IPAddress ip)
        {
            var endpoint = new IPEndPoint(ip, port);

            try
            {
                this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listener.BeginConnect(endpoint, this.OnConnectCallback, this.listener);
                ShowConsoleMessage("StartClient", "Waiting for server connection", false);
                this.connected.WaitOne();
                

            }
            catch (SocketException se)
            {
                ShowConsoleMessage("StartClient", se.Message, true);
                ItsConenctedToServer = Settings.ClientIsConnected = false;
            }
            catch (Exception e)
            {
                ShowConsoleMessage("StartClient", e.Message, true);
                ItsConenctedToServer = Settings.ClientIsConnected = false;
            }
        }

        private void OnConnectCallback(IAsyncResult result)
        {
            var server = (Socket)result.AsyncState;

            try
            {
                server.EndConnect(result);
                this.connected.Set();
                MessagingCenter.Send(this, Events.ConnectToServer, true);
                ItsConenctedToServer = Settings.ClientIsConnected = true;
                ShowConsoleMessage("OnConnectCallback", "Connected to server, client side", false);


            }
            catch (SocketException se)
            {
                ShowConsoleMessage("OnConnectCallback", se.Message, true);
                ItsConenctedToServer = Settings.ClientIsConnected = false;
            }
            catch (Exception e)
            {
                ShowConsoleMessage("OnConnectCallback", e.Message, true);
                ItsConenctedToServer = Settings.ClientIsConnected = false;
            }
        }
        #endregion

        #region Check connection
        public bool IsConnected()
        {
            try
            {
                if (this.listener != null)
                {
                    Ping pingTest = new Ping();
                    PingReply reply = pingTest.Send(IPAddress.Parse(Settings.ConnectedServerIP));
                    Boolean status = reply.Status == IPStatus.Success;
                    if (status) ItsConenctedToServer = Settings.ClientIsConnected = true;

                    return status;
                }
                else
                {
                    return false;
                }
            }
            catch (PingException pe) 
            {
                ShowConsoleMessage("IsConnected", pe.Message, true);
                ItsConenctedToServer = Settings.ClientIsConnected = false;
                return ItsConenctedToServer;
            }
            catch (Exception e)
            {
                ShowConsoleMessage("IsConnected", e.Message, true);
                ItsConenctedToServer = Settings.ClientIsConnected = false;
                return ItsConenctedToServer;
            }
        }

        #endregion

        #region Receive data
        public void Receive()
        {
            if(IsConnected())
            {
                var state = new StateObject(this.listener);
                try
                {
                    state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
                    ShowConsoleMessage("Receive", "Beginning recive", false);
                }
                catch (Exception e)
                {
                    ShowConsoleMessage("Receive", "Client Recive: " + e.Message + " " + e.InnerException.Message, false);
                }
            }

        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var state = (IStateObject)result.AsyncState;
                var receive = state.Listener.EndReceive(result);

                if (receive > 0)
                {
                    state.Append(Encoding.UTF8.GetString(state.Buffer, 0, receive));
                    ShowConsoleMessage("ReceiveCallback", "Append message", false);
                }

                if (receive == state.BufferSize)
                {
                    state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
                    ShowConsoleMessage("ReceiveCallback", "Big message, start new recive", false);
                }
                else
                {
                    if (state.Text.Equals("<EOC>"))
                    {
                        Close();
                        Settings.ClientIsConnected = false;
                        MessagingCenter.Send(this, Events.ConnectToServer, false);
                        ShowConsoleMessage("ReceiveCallback", "Disconnect connection with server", false);
                    }
                    else if(!String.IsNullOrEmpty(state.Text))
                    {
                        MessagingCenter.Send(this, Events.SendedText, state.Text);
                        state.Reset();
                        Receive();
                        ShowConsoleMessage("ReceiveCallback", "Recive message from server", false);
                    }
                }
            }
            catch (SocketException se)
            {
                ShowConsoleMessage("ReceiveCallback", se.Message, true);
            }
            catch (ObjectDisposedException ode)
            {
                ShowConsoleMessage("ReceiveCallback", ode.Message, true);
            }
            catch (Exception e)
            {
                ShowConsoleMessage("ReceiveCallback", e.Message, true);
            }
        }
        #endregion

        #region Send data
        public void Send(string msg, bool close)
        {
            if (!this.IsConnected())
            {
                ShowConsoleMessage("Send", "Destination socket is not connected.", true);
                throw new Exception("Destination socket is not connected.");
            }

            var response = Encoding.UTF8.GetBytes(msg);

            this.listener.BeginSend(response, 0, response.Length, SocketFlags.None, this.SendCallback, this.listener);
            ShowConsoleMessage("Send", "Start sending", false);
        }

        private void SendCallback(IAsyncResult result)
        {
            if (IsConnected())
            {
                try
                {
                    var resceiver = (Socket)result.AsyncState;

                    resceiver.EndSend(result);
                    ShowConsoleMessage("SendCallback", "Message sent", false);
                }
                catch (SocketException se)
                {
                    ShowConsoleMessage("SendCallback", se.Message, true);
                }
                catch (ObjectDisposedException ode)
                {
                    ShowConsoleMessage("SendCallback", ode.Message, true);
                }
                catch (Exception e)
                {
                    ShowConsoleMessage("SendCallback", e.Message, true);
                }
            }
        }
        #endregion

        #region Close connection
        private void Close()
        {
            try
            {

                this.listener.Shutdown(SocketShutdown.Both);
                this.listener.Close();
                ShowConsoleMessage("Close", "Close connection with server", false);
            }
            catch (SocketException se)
            {
                ShowConsoleMessage("Close", se.Message, true);
            }
            catch (Exception e)
            {
                ShowConsoleMessage("Close", e.Message, true);
            }
        }

        public void DisconnectWithServer()
        {
            Settings.ClientIsConnected = false;
            MessagingCenter.Send(this, Events.ConnectToServer, false);
            Close();
            ShowConsoleMessage("DisconnectWithServer", "Close connection complete", false);
        }

        public void Dispose()
        {
            this.connected.Dispose();
            ShowConsoleMessage("Dispose", "Dispose event", false);
        }
        #endregion

        private void ShowConsoleMessage(string method, string text, bool exeption)
        {
            if (exeption)
            {
                Log.Debug(TAG, $"{method} exeption: {text}");
            }
            else
            {
                Log.Debug(TAG, $"{method} log: {text}");
            }

        }
       
    }

}