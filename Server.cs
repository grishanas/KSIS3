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
    class Server
    {
		User myUser;
		public List<ConnectedUser> users = new List<ConnectedUser>();
		public UdpClient nameReceiver = new UdpClient();
		public TcpListener listener;

		int udpPort = 5555;
		int tcpPort = 5556;

		Thread listenThread;
		Thread nameReciveThread;

		public Server(User user)
		{
			this.myUser = user;
			listenThread = new Thread(new ThreadStart(Listen));
			listenThread.Start();
			nameReciveThread = new Thread(new ThreadStart(ReceiveNewUser));
			nameReciveThread.Start();
		}

		protected bool IsMyIp(IPAddress ip)
		{
			string host = Dns.GetHostName();
			IPAddress[] addresses = Dns.GetHostEntry(host).AddressList;
			foreach (var address in addresses)
			{
				if (address.Equals(ip))
				{
					return true;
				}
			}
			return false;

		}

		void ReceiveNewUser()
        {
			nameReceiver = new UdpClient(udpPort);
			nameReceiver.EnableBroadcast = true;
			IPEndPoint remoteIp = null;
			try
			{
				while (true)
				{
					byte[] data = nameReceiver.Receive(ref remoteIp);
					string name = Encoding.UTF8.GetString(data);
					if (!IsMyIp(remoteIp.Address))
					{
						users.Add(new ConnectedUser(remoteIp.Address, name, myUser));
						//users.Add(new ConnectedUser(myUser.IP, name, myUser));
						//myUser.form1.message.Enqueue(new OutPutMess(name, "присоединился"));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				nameReceiver.Close();
			}
		}

		void Listen()
		{
			try
			{
				listener = new TcpListener(IPAddress.Any, tcpPort);
				listener.Start();
				while (true)
				{
					var tcpClient = listener.AcceptTcpClient();
					users.Add(new ConnectedUser(tcpClient, myUser));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				if (listener != null)
				{
					listener.Stop();
				}
			}
		}

		public void Disconnect(ConnectedUser connectedUser)
		{
			if (users.Contains(connectedUser))
			{
				//this.myUser.form1.richTextBox1.Text += connectedUser.Name + "покинул чат";
				myUser.form1.message.Enqueue(new OutPutMess(connectedUser.Name, "Покинул чат",connectedUser.Ip));
				users.Remove(connectedUser);

			}
		}

		public void SendAllUsers(byte[] data)
		{
			foreach (var user in users)
			{
				user.WriteToStream(data);
			}
		}
	}
}
