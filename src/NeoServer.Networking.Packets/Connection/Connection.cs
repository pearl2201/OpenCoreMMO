﻿using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace NeoServer.Networking
{
    public class Connection : IConnection
    {
        public event EventHandler<IConnectionEventArgs> OnProcessEvent;
        public event EventHandler<IConnectionEventArgs> OnCloseEvent;
        public event EventHandler<IConnectionEventArgs> OnPostProcessEvent;

        private Socket Socket;
        private Stream Stream;
        private object writeLock;



        public IReadOnlyNetworkMessage InMessage { get; private set; }

        public uint[] XteaKey { get; private set; }
        public uint PlayerId { get; set; }
        public bool IsAuthenticated { get; set; } = false;

        public bool Disconnected { get; private set; } = false;

        public int BeginStreamReadCalls { get; set; }

        public Connection(Socket socket)
        {
            Socket = socket;
            Stream = new NetworkStream(Socket);
            XteaKey = new uint[4];
            IsAuthenticated = false;
            InMessage = new ReadOnlyNetworkMessage(new byte[16394], 0);
            writeLock = new object();

        }
        public void BeginStreamRead()
        {
            Stream.BeginRead(InMessage.Buffer, 0, 2, OnRead, null);
        }

        public void SetXtea(uint[] xtea)
        {
            XteaKey = xtea;
        }


        private void OnRead(IAsyncResult ar)
        {



            var clientDisconnected = !this.CompleteRead(ar);
            if (clientDisconnected && !IsAuthenticated)
            {
                Close();
                return;
            }
            if (clientDisconnected && IsAuthenticated)
            {
                Disconnected = true;
            }



            // if (length == 0)
            // {
            //     Disconnected = true;
            // }

            try
            {
                //Buffer = new byte[16394];

                var eventArgs = new ConnectionEventArgs(this);
                OnProcessEvent?.Invoke(this, eventArgs);

            }
            catch (Exception e)
            {
                // Invalid data from the client
                // TODO: I must not swallow exceptions.
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Close();
            }
        }

        private bool CompleteRead(IAsyncResult ar)
        {
            try
            {
                int read = Stream.EndRead(ar);




                if (read == 0)
                {
                    Console.WriteLine($"{read} bytes read from {PlayerId}");
                    // client disconnected
                    //this.Close();

                    return false;
                }

                int size = BitConverter.ToUInt16(InMessage.Buffer, 0) + 2;

                while (read < size)
                {
                    if (Stream.CanRead)
                    {
                        read += Stream.Read(InMessage.Buffer, read, size - read);
                    }
                }

                InMessage.Resize(size);

                Console.WriteLine($"{size} bytes read on connection {PlayerId}");



                return true;
            }
            catch (Exception e)
            {


                // TODO: is closing the connection really necesary?
                this.Close();
            }

            return false;
        }



        public void Close()
        {
            Stream.Close();
            Socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent?.Invoke(this, new ConnectionEventArgs(this));

        }

        private void SendMessage(INetworkMessage message, bool notification = false)
        {
            try
            {
                lock (writeLock)
                {
                    var streamMessage = message.AddHeader();
                    Stream.BeginWrite(streamMessage, 0, streamMessage.Length, null, null);
                }

                if (!notification)
                {
                    var eventArgs = new ConnectionEventArgs(this);
                    OnPostProcessEvent?.Invoke(this, eventArgs);
                }


            }
            catch (ObjectDisposedException)
            {
                Close();
            }
        }

        public void SendFirstConnection()
        {
            var message = new NetworkMessage();

            new FirstConnectionPacket().WriteToMessage(message);

            SendMessage(message);
        }

        public void Send(IOutgoingPacket packet, bool notification = false)
        {
            var message = new NetworkMessage();

            packet.WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage, notification);

        }

        public void Send(Queue<IOutgoingPacket> outgoingPackets, bool notification = false)
        {
            var message = new NetworkMessage();

            while (outgoingPackets.Any())
            {
                outgoingPackets.Dequeue().WriteToMessage(message);
            }

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage, notification);
        }
        public void Disconnect(string text)
        {
            var message = new NetworkMessage();

            new TextMessagePacket(text).WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);
            Close();
        }
    }
}