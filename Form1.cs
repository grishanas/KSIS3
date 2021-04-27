using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace KSIS3_2
{
    public partial class Form1 : Form
    {
        User user;
        Form1 form1;
        public ConcurrentQueue<OutPutMess> message;
        public Form1()
        {
            InitializeComponent();
            form1 = this;
            message = new ConcurrentQueue<OutPutMess>();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            var mes = this.textBox2.Text.ToString();
            this.textBox2.Clear();
            user.form1.message.Enqueue(new OutPutMess(user.Name, mes,user.IP));
            user.SendMessage(mes);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user = new User(this.textBox1.Text.ToString(), form1);
            user.form1.message.Enqueue(new OutPutMess(user.Name, "Присоединился", user.IP));
            this.button1.Enabled = false;
            this.textBox1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OutPutMess mes;
           
            while (message.TryPeek(out mes))
            {
                
                form1.richTextBox1.Text += mes.ToString()+"\r\n"; 
                message.TryDequeue(out mes);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            user.server.listener.Stop();
            user.server.nameReceiver.Close();
            foreach (var Tread in user.server.users)
            {
                Tread.tcpClient.Close();
            }
        }
    }
}
