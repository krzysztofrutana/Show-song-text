﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

/*
 * Thanks for Jesse C. Slicer and Jamal for this soluion from https://codereview.stackexchange.com/questions/24758/tcp-async-socket-server-client-communication
 * Not everything is needed for me, but this solution work perfect and exactly as I want. I add only messegingcanter to send information and recive text from this class. 
 */

namespace Show_song_text.Interfaces
{
    public interface IStateObject
    {
        int BufferSize { get; }

        int Id { get; }

        bool Close { get; set; }

        byte[] Buffer { get; }

        Socket Listener { get; }

        string Text { get; }

        void Append(string text);

        void Reset();
    }
}
