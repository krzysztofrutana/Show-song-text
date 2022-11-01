using System;

/*
 * Thanks for Jesse C. Slicer and Jamal for this soluion from https://codereview.stackexchange.com/questions/24758/tcp-async-socket-server-client-communication
 * Not everything is needed for me, but this solution work perfect and exactly as I want. I add only messegingcanter to send information and recive text from this class. 
 */

namespace ShowSongText.Abstraction
{
    public interface IAsyncSocketListener : IDisposable
    {

        void StartListening(int port);

        bool IsConnected(int id);

        void OnClientConnect(IAsyncResult result);

        void ReceiveCallback(IAsyncResult result);

        void Send(string msg, bool close);

        void Close(int id);

        void StopListening();

    }
}
