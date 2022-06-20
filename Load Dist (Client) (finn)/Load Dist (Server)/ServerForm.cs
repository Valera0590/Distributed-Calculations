using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Load_Dist__Server_
{
    public partial class ServerForm : Form
    {
        private MessageQueue qSrv = null;          // очередь сообщений
        private bool _continue = true;          // флаг, указывающий продолжается ли работа с мэйлслотом
        //private List<string> LoginClients = new List<string>();
        private MessageQueue qCl = null;          // очередь сообщений
        private string clPath = ".\\private$\\ClientQueue";    // путь к очереди сообщений, Dns.GetHostName() - метод, возвращающий имя текущей машины
        private string srvPath = ".\\private$\\ServerQueue";    // путь к очереди сообщений, Dns.GetHostName() - метод, возвращающий имя текущей машины
        WebUnites root;
        public ServerForm()
        {
            InitializeComponent();
            // если очередь сообщений с указанным путем существует, то открываем ее, иначе создаем новую
            if (MessageQueue.Exists(srvPath))
                qSrv = new MessageQueue(srvPath);
            else
                qSrv = MessageQueue.Create(srvPath);

            // задаем форматтер сообщений в очереди
            qSrv.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });
            RecMes();
        }
        private async void RecMes()
        {
            await ReceiveMessageAsync();   // вызов асинхронного метода
        }

        private async Task ReceiveMessageAsync()
        {
            await Task.Run(() => ReceiveMessage());
        }

        // получение сообщения
        private void ReceiveMessage()
        {
            System.Messaging.Message msg = null;
            root = new WebUnites();
            // входим в бесконечный цикл работы с очередью сообщений
            while (_continue)
            {
                if (qSrv.Peek() != null)   // если в очереди есть сообщение, выполняем его чтение, интервал до следующей попытки чтения равен 10 секундам
                    msg = qSrv.Receive(TimeSpan.FromSeconds(10.0));
                FindUnit(Convert.ToInt32(msg.Body), msg.Label);
                Thread.Sleep(500);          // приостанавливаем работу потока перед тем, как приcтупить к обслуживанию очередного клиента
            }
        }

        private async void FindUnit (int diff, string idClient)
        {
            //root.FindInd(ref root, diff, -1);
            int indexUnit = -1;
            List<(int, int, int, int)> list = new List<(int, int, int, int)>();
            list.AddRange(root.TreeToListFinn(root));
            root = root.RestoreIncs(root);              // очищение списков Inc и NInc
            list.Capacity = list.Count;
            foreach (var item in list)      // определение ID узла, который способен выполнить задачу сложности diff
                if (item.Item4 - item.Item3 >= diff)
                {
                    indexUnit = item.Item1;
                    break;
                }
            if (indexUnit != -1)
            {
                root = root.FindIndexFinn(root, indexUnit, diff);
                list.Clear();
                //list.Add((root.ID, root.Level, root.ActuallyPower, root.MaxPower));
                list.AddRange(root.TreeToListFinn(root));
                root = root.RestoreIncs(root);              // очищение списков Inc и NInc
                list.Capacity = list.Count;
                //list.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                
                new Task(async () =>
                {
                    await WorkingTime(idClient, diff, indexUnit);
                    root = root.DeleteLoadFinn(root, indexUnit, diff);
                    list.Clear();
                    //list.Add((root.ID, root.Level, root.ActuallyPower, root.MaxPower));
                    list.AddRange(root.TreeToListFinn(root));
                    list.Capacity = list.Count;
                    root = root.RestoreIncs(root);              // очищение списков Inc и NInc
                    //list.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                    PrintAsTree(list, indexUnit);
                }).Start();
            }
            else
            {
                string temp = clPath;
                clPath += idClient;
                if (MessageQueue.Exists(clPath))
                {
                    // если очередь, путь к которой указан в поле tbPath существует, то открываем ее
                    qCl = new MessageQueue(clPath);
                    // выполняем отправку сообщения в очередь
                    qCl.Send("Процесс сложности " + diff + " требует слишком большой\nвычислительной мощности. Попробуйте позже!", idClient);
                }
                else MessageBox.Show("Указан неверный путь к очереди, либо очередь не существует");
                clPath = temp;
            }
            rtbProcessing.Invoke((MethodInvoker)delegate
            {
                rtbProcessing.Text += "\n   -->   Процесс сложности " + diff + " начал выполнение на узле "+indexUnit+"!\n";
            });
            PrintAsTree(list, indexUnit);
            
        }
        private void PrintAsTree(List<(int, int, int, int)> list, int indexU)
        {
            for (int i = 0; i < list.Capacity; i++)
            {
                if (i == 0)
                    rtbProcessing.Invoke((MethodInvoker)delegate
                    {
                        rtbProcessing.Text += "  Инициирующий узел " + list[i].Item1 + " : Загружен (" + list[i].Item3 + "/" + list[i].Item4 + ")\n";     // выводим полученное сообщение на форму
                    });
                else
                {
                    string lvl = "  ";
                    for (int j = 0; j < list[i].Item2; j++)
                    {
                        lvl += "· ";
                    }
                    rtbProcessing.Invoke((MethodInvoker)delegate
                    {
                        rtbProcessing.Text += lvl + "Узел " + list[i].Item1 + " : Загружен (" + list[i].Item3 + "/" + list[i].Item4 + ")\n";     // выводим полученное сообщение на форму
                    });
                }
            }
        }
        
        private async Task WorkingTime(string idClient, int diff, int indexUnit)
        {
            Thread.Sleep(100 * diff);
            rtbProcessing.Invoke((MethodInvoker)delegate
            {
                rtbProcessing.Text += "   -->   Процесс на узле "+indexUnit+" сложности " + diff + " выполнен!\n";
            });
            
            string temp = clPath;
            clPath += idClient;
            if (MessageQueue.Exists(clPath))
            {
                // если очередь, путь к которой указан в поле tbPath существует, то открываем ее
                qCl = new MessageQueue(clPath);
                // выполняем отправку сообщения в очередь
                qCl.Send("Процесс сложности " + diff + " закончил выполнение!", idClient);
            }
            else MessageBox.Show("Указан неверный путь к очереди, либо очередь не существует");
            clPath = temp;
            
        }
        

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _continue = false;      // сообщаем, что работа с очередью сообщений 
        }

        private void btnClearForm_Click(object sender, EventArgs e)
        {
            rtbProcessing.Text = "";
        }
    }
}
