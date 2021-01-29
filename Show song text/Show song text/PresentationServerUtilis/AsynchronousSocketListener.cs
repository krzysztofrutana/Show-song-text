using Show_song_text.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Show_song_text.Models;
using Xamarin.Forms;
using Show_song_text.Utils;
using Show_song_text.ViewModels;
using Show_song_text.Helpers;

/*
 * Thanks for Jesse C. Slicer and Jamal for this soluion from https://codereview.stackexchange.com/questions/24758/tcp-async-socket-server-client-communication
 * Not everything is needed for me, but this solution work perfect and exactly as I want. I add only messegingcanter to send information and recive text from this class. 
 */
namespace Show_song_text.PresentationServerUtilis
{

    public sealed class AsyncSocketListener : ViewModelBase, IAsyncSocketListener
    {
        private const ushort Limit = 250;

        private static readonly IAsyncSocketListener instance = new AsyncSocketListener();
        private Socket listener;
        private Boolean IsRunning = Settings.ServerIsRunning;

        private readonly ManualResetEvent mre = new ManualResetEvent(false);
        private readonly IDictionary<int, IStateObject> clients = new Dictionary<int, IStateObject>();
        private static string TAG = "AsyncSocketListener";
        private static readonly ILogInterface Log = DependencyService.Get<ILogInterface>();


        private AsyncSocketListener()
        {
            Settings.ServerClientConnectedQty = this.clients.Count;
        }

        public static IAsyncSocketListener Instance
        {
            get
            {
                return instance;
            }
        }

        /* Starts the AsyncSocketListener */
        public void StartListening(int port)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList[0];
            var endpoint = new IPEndPoint(ip, port);

            try
            {
                using (listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    listener.Bind(endpoint);
                    listener.Listen(Limit);
                    IsRunning = Settings.ServerIsRunning;
                    while (IsRunning)
                    {
                        this.mre.Reset();
                        listener.BeginAccept(this.OnClientConnect, listener);
                        MessagingCenter.Send(this, Events.SendPlaylist, true);
                        ShowConsoleMessage("StartListening", "Waiting for client", false);
                        this.mre.WaitOne();
                        
                    }
                }

            }
            catch (SocketException e)
            {
                ShowConsoleMessage("StartListening", e.Message, true);
                IsRunning = Settings.ServerIsRunning = false;
            }
            catch (Exception e)
            {
                ShowConsoleMessage("StartListening", e.Message, true);
                IsRunning = Settings.ServerIsRunning = false;
            }
        }

        /* Gets a socket from the clients dictionary by his Id. */
        private IStateObject GetClient(int id)
        {
            IStateObject state;
            ShowConsoleMessage("GetClient","Get client state", false);

            return this.clients.TryGetValue(id, out state) ? state : null;
        }

        /* Checks if the socket is connected. */
        public bool IsConnected(int id)
        {
            var state = this.GetClient(id);

            return !(state.Listener.Poll(1000, SelectMode.SelectRead) && state.Listener.Available == 0);
        }

        /* Add a socket to the clients dictionary. Lock clients temporary to handle multiple access.
         * ReceiveCallback raise a event, after the message receive complete. */
        #region Receive data
        public void OnClientConnect(IAsyncResult result)
        {
            this.mre.Set();

            if (IsRunning == false) return;

            try
            {
                IStateObject state;

                lock (this.clients)
                {
                    var id = !this.clients.Any() ? 1 : this.clients.Keys.Max() + 1;

                    state = (IStateObject)new StateObject(((Socket)result.AsyncState).EndAccept(result), id);
                    this.clients.Add(id, state);
                    ShowConsoleMessage("OnClientConnect", $"Client connected, id: {id}", false);
                    Settings.ServerClientConnectedQty = this.clients.Count;
                    MessagingCenter.Send(this, Events.ClientConnected, 1);

                }

                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
            }
            catch (SocketException se)
            {
                // TODO:
                ShowConsoleMessage("OnClientConnect", se.Message, true);
            }
            catch (Exception e)
            {
                ShowConsoleMessage("OnClientConnect", e.Message, true);
            }
        }

        public void ReceiveCallback(IAsyncResult result)
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
                        Close(state.Id);
                        ShowConsoleMessage("ReceiveCallback", $"Client {state.Id} disconnected", false);
                    }
                    else
                    {

                        state.Reset();
                        ShowConsoleMessage("ReceiveCallback", $"Client send {state.Text}", false);
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
            }catch (Exception e)
            {
                ShowConsoleMessage("ReceiveCallback", e.Message, true);
            }
        }
        #endregion

        /* Send(int id, String msg, bool close) use bool to close the connection after the message sent. */
        #region Send data
        public void Send(string msg, bool close)
        {
            foreach (var id in this.clients.Keys)
            {
                var state = this.GetClient(id);

                if (state == null)
                {
                    ShowConsoleMessage("Send", "Client does not exist.", true);
                    throw new Exception("Client does not exist.");
                }

                if (!this.IsConnected(state.Id))
                {
                    ShowConsoleMessage("Send", "Destination socket is not connected" , true);
                    throw new Exception("Destination socket is not connected.");
                }

                try
                {
                    if (msg.Equals("<EOC>"))
                    {
                        var send = Encoding.UTF8.GetBytes(msg);

                        state.Close = close;
                        state.Listener.BeginSend(send, 0, send.Length, SocketFlags.None, this.SendEOCCallback, state);
                        ShowConsoleMessage("Send", "Start sending EOC message", false);
                    }
                    else
                    {
                        var send = Encoding.UTF8.GetBytes(msg);

                        state.Close = close;
                        state.Listener.BeginSend(send, 0, send.Length, SocketFlags.None, this.SendCallback, state);
                        ShowConsoleMessage("Send", "Start sending message", false);
                    }
                    
                }
                catch (SocketException se)
                {
                    ShowConsoleMessage("Send", se.Message, true);
                }
                catch (ArgumentException ae)
                {
                    ShowConsoleMessage("Send", ae.Message, true);
                }
                catch (Exception e)
                {
                    ShowConsoleMessage("Send", e.Message, true);
                }
            }

        }

        private void SendCallback(IAsyncResult result)
        {
            if (result != null)
            {
                var state = (IStateObject)result.AsyncState;

                try
                {
                    state.Listener.EndSend(result);
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

        private void SendEOCCallback(IAsyncResult result)
        {
            if (result != null)
            {
                var state = (IStateObject)result.AsyncState;

                try
                {
                    state.Listener.EndSend(result);
                    ShowConsoleMessage("SendCallback", "Message sent", false);
                    foreach(int id in this.clients.Keys.ToArray())
                    {
                        Close(id);
                    }

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

        public void Close(int id)
        {
            var state = this.GetClient(id);

            if (state == null)
            {
                ShowConsoleMessage("Close", "Client does not exist.", true);
                throw new Exception("Client does not exist.");
            }

            try
            {
                state.Listener.Shutdown(SocketShutdown.Both);
                state.Listener.Close();
                ShowConsoleMessage("Close", $"Connection with client {id} closed", false);

            }
            catch (SocketException se)
            {
                ShowConsoleMessage("SendCallback", se.Message, true);
            }
            catch (Exception e)
            {
                ShowConsoleMessage("SendCallback", e.Message, true);
            }
            finally
            {
                this.clients.Remove(id);
                Settings.ServerClientConnectedQty = this.clients.Count;
                MessagingCenter.Send(this, Events.ClientDisconnected, 1);
            }
        }

        public void Dispose()
        {
            Send("<EOC>", true);

            MessagingCenter.Send(this, Events.SendPlaylist, false);
        }

        public void StopListening() // Stop Listening
        {
            IsRunning = Settings.ServerIsRunning = false;
            Dispose();
            Socket exListener = Interlocked.Exchange(ref listener, null);
            if (exListener != null)
            {
                exListener.Close();
            }
            ShowConsoleMessage("StopListening", "Listener succesfull closed", false);
        }


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