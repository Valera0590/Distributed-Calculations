using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Load_Dist__Client_
{
    public partial class ClientForm : Form
    {
        private MessageQueue qSrv = null;      // очередь сообщений, в которую будет производиться запись сообщений
        private bool _continue = true;          // флаг, указывающий продолжается ли работа с мэйлслотом
        private MessageQueue qCl = null;      // очередь сообщений, в которую будет производиться запись сообщений
        private bool flagFirst = false;
        private string clPath = ".\\private$\\ClientQueue";    // путь к очереди сообщений, Dns.GetHostName() - метод, возвращающий имя текущей машины
        private string srvPath = ".\\private$\\ServerQueue";    // путь к очереди сообщений, Dns.GetHostName() - метод, возвращающий имя текущей машины
        private int idClient;
        private Random rndCl = new Random(DateTime.Now.Millisecond);
        public ClientForm()
        {
            InitializeComponent();
            btnSend.Enabled = false;
            idClient = rndCl.Next(0,200);
            clPath += idClient;
            // если очередь сообщений с указанным путем существует, то открываем ее, иначе создаем новую
            if (MessageQueue.Exists(clPath))
                qCl = new MessageQueue(clPath);
            else
                qCl = MessageQueue.Create(clPath);

            // задаем форматтер сообщений в очереди
            qCl.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (!flagFirst)
            {
                Connect();
                flagFirst = true;
            }
            else    Send();
            //await TimerWaitAsync();
            /*await new Task(() =>
            {
                btnSend.Enabled = false;
                Thread.Sleep(1000);
                btnSend.Enabled = true;
            });*/
            await Task.Factory.StartNew(() =>
            {
                btnSend.Invoke((MethodInvoker)delegate
                {
                    btnSend.Enabled = false;
                    Thread.Sleep(500);
                    btnSend.Enabled = true;
                });
            });


        }
        private async void Connect()
        {
            if (MessageQueue.Exists(srvPath))
            {
                    // если очередь, путь к которой указан в поле srvPath существует, то открываем ее
                qSrv = new MessageQueue(srvPath);
                Send();
                await ReceiveMessageAsync();   // вызов асинхронного метода
            }
            else
                MessageBox.Show("Указан неверный путь к очереди, либо очередь не существует");
        }
        private void Send()
        {
                // выполняем отправку сообщения в очередь
            qSrv.Send(tbDiff.Text, idClient.ToString());
            tbDiff.Text = "";
        }
        private async Task ReceiveMessageAsync()
        {
            await Task.Run(() => ReceiveMessage());
        }

        // получение сообщения
        private void ReceiveMessage()
        {
            if (qCl == null)
                return;
            System.Messaging.Message msg = null;

            // входим в бесконечный цикл работы с очередью сообщений
            while (_continue)
            {
                if (qCl.Peek() != null)   // если в очереди есть сообщение, выполняем его чтение, интервал до следующей попытки чтения равен 10 секундам
                    msg = qCl.Receive(TimeSpan.FromSeconds(10.0));
                MessageBox.Show(msg.Body.ToString());
                Thread.Sleep(300);          // приостанавливаем работу потока перед тем, как приcтупить к обслуживанию очередного клиента
            }
        }
        /*private async Task TimerWaitAsync()
        {
            await Task.Run(() => TimerWait());
        }
        private void TimerWait()
        {
            btnSend.Enabled = false;
            Thread.Sleep(1000);
            btnSend.Enabled = true;
        }*/
        
        private void tbDiff_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar)) return;
            else    e.Handled = true;
        }
        private void tbDiff_TextChanged(object sender, EventArgs e)
        {
            if (tbDiff.Text == "") btnSend.Enabled = false;
            else btnSend.Enabled = true;
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (flagFirst == true)
            //{
            //    string message = "вышел";
            //        // выполняем отправку сообщения в очередь
            //    qSrv.Send(message, idClient.ToString());
            //}

            _continue = false;      // сообщаем, что работа с очередью завершена
        }

    }
}
