using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace KSIS3_2
{
    public class OutPutMess
    {
        public string Name;
        public string Message;
        public string time;
        public string IP;

        public OutPutMess(string name, string mes, IPAddress ip)
        {
            Name = name;
            Message = mes;
            IP = ip.ToString();
            time = DateTime.Now.ToString();
        }

        public override string ToString()
        {
            return Name+"   "+IP+"    "+time+":"+Message;
        }
    }
}
