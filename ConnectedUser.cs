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
    class ConnectedUser
    {
		User user;
		public IPAddress Ip { get; private set; }
		public string Name { get; private set; }

		public TcpClient tcpClient;
		NetworkStream stream = null;
		int tcpPort = 5556;
		public Thread userThread;

		public ConnectedUser(IPAddress ip, string name, User user)
		{
			Ip = ip;
			Name = name;
			this.user = user;
			var iPEndPoint = new IPEndPoint(Ip, tcpPort);
			tcpClient = new TcpClient();
			tcpClient.Connect(iPEndPoint);
			userThread = new Thread(new ThreadStart(Listen));
			userThread.Start();
		}

		public ConnectedUser(TcpClient tcpClient, User user)
		{
			this.tcpClient = tcpClient;
			this.user = user;
			Ip = IPAddress.Parse(tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]);
			Name = "undef";
			userThread = new Thread(new ThreadStart(Listen));
			userThread.Start();
		}




		void Listen()
		{
			try
			{
				stream = tcpClient.GetStream();
				var name = (new Message(0, user.Name)).GetBytes();
				stream.Write(name, 0, name.Length);
				while (true)
				{
					var packet = new Message(GetMessage());
					if (packet.Type == 0)
					{
						Name = packet.Content;
						user.form1.message.Enqueue(new OutPutMess(Name, " Приcоединился",Ip));
					}
					else
					{
						user.form1.message.Enqueue(new OutPutMess(this.Name,packet.Content,Ip));
					}
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.Message);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}

				tcpClient.Close();
				user.server.Disconnect(this);
			}
		}

		byte[] GetMessage()
		{
			byte[] data = new byte[64];
			var packet = new List<byte>();
			StringBuilder builder = new StringBuilder();
			int bytes = 0;
			do
			{
				bytes = stream.Read(data, 0, data.Length);
				packet.AddRange(data.Take(bytes));
			}
			while (stream.DataAvailable);

			return packet.ToArray();
		}

		public void WriteToStream(byte[] data)
		{
			if (stream != null)
			{
				stream.Write(data, 0, data.Length);
			}
		}
	}
}
