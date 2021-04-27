using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KSIS3_2
{
    class User
    {
        public string Name;
        public IPAddress IP;

        protected int UDPSender = 5555;
        protected UdpClient UDPClient;
        public Form1 form1;
        public Server server;

        public User(string name,Form1 form)
        {
            form1 = form;
            Name = name;
            server = new Server(this);
            IP = GetCurrrentHostIp();
            SendName();
        }

        public IPAddress GetCurrrentHostIp()
        {
            string host = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostEntry(host).AddressList;
            foreach (var address in addresses)
            {
                if (address.GetAddressBytes().Length == 4)
                {
                    return address;
                }
            }
            return null;
        }

        void SendName()
        {
            UDPClient = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, UDPSender);
            UDPClient.EnableBroadcast = true;
            var udpMassege = Encoding.UTF8.GetBytes(Name);
            UDPClient.Send(udpMassege, udpMassege.Length, endPoint);
            UDPClient.Close();
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = (new Message(1, message)).GetBytes();
                server.SendAllUsers(data);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
