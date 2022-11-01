using System;
using System.Net;

/*
 * Thanks for Jesse C. Slicer and Jamal for this soluion from https://codereview.stackexchange.com/questions/24758/tcp-async-socket-server-client-communication
 * Not everything is needed for me, but this solution work perfect and exactly as I want. I add only messegingcanter to send information and recive text from this class. 
 */

namespace ShowSongText.Abstraction
{
    public interface IAsyncClient : IDisposable
    {
        void StartClient(int port, IPAddress ip);

        bool IsConnected();

        void Receive();

        void Send(string msg, bool close);
        void DisconnectWithServer();
    }
}