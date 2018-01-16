using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace UniqueIPList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<string> ips = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.ResetText();
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                var match = Regex.Match(textBox1.Lines[i], @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                if (match.Success)
                    if (!ips.Contains(match.Captures[0].Value))
                        ips.Add(match.Captures[0].Value);
            }
            for (int i = 0; i < ips.Count; i++)
                textBox2.Text += ips[i] + "\r\n";
            label3.Text = "Unique IPs found = " + ips.Count;
        }
        public static Hashtable fwips = new Hashtable();
        public bool thrRun = false;
        public static int total = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.ResetText();
            fwips.Clear();
            /*Parallel.For(0, textBox1.Lines.Length, i =>
            {
                button2.Text = "A: " + i + " / " + textBox1.Lines.Length;
                Refresh();
                if (textBox1.Lines[i].Contains("length 4"))
                {
                    var match = Regex.Match(textBox1.Lines[i], @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                    if (match.Success)
                        if (match.Value != "212.29.229.4")
                        {
                            if (fwips.ContainsKey(match.Value))
                                fwips[match.Value] = Convert.ToInt32(fwips[match.Value]) + 1;
                            else
                                fwips.Add(match.Value, 1);
                        }
                }
            });*/
            /*int jj = 0;
            Parallel.ForEach(textBox1.Lines, i =>
            {
                button2.Text = "A: " + (++jj) + " / " + textBox1.Lines.Length;
                Refresh();
                if (i.Contains("length 4"))
                {
                    var match = Regex.Match(i, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                    if (match.Success)
                        if (match.Value != "212.29.229.4")
                        {
                            if (fwips.ContainsKey(match.Value))
                                fwips[match.Value] = Convert.ToInt32(fwips[match.Value]) + 1;
                            else
                                fwips.Add(match.Value, 1);
                        }
                }
            });*/
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                //20:30:53.614466 IP 24.78.87.141.45425 > 212.29.229.4.cbt: UDP, length 4
                button2.Text = "A: " + i + " / " + textBox1.Lines.Length;
                Refresh();
                var match = Regex.Match(textBox1.Lines[i], @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                if (match.Success)
                {
                    if (match.Value == "212.29.229.4")
                        continue;
                    if (fwips.ContainsKey(match.Value))
                    {
                        fwips[match.Value] = Convert.ToInt32(fwips[match.Value]) + 1;
                        //MessageBox.Show("added to " + match.Value + ", " + fwips[match.Value]);
                    }
                    else
                    {
                        fwips.Add(match.Value, 1);
                        //MessageBox.Show("created " + match.Value);
                    }
                }
                //else
                    //MessageBox.Show("No IP");
            }
            //MessageBox.Show(fwips.Count.ToString());
            string fieldName = string.Empty;
            foreach (DictionaryEntry de in fwips)
            {
                fieldName = de.Key as string;
                if (Convert.ToInt32(de.Value) >= (int)numericUpDown2.Value)
                    textBox2.Text += fieldName + "=" + de.Value + "\r\n";
            }
            label3.Text = "Unique IPs found = " + fwips.Count.ToString();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (!thrRun)
            {
                thrRun = true;
                total = 0;
                textBox2.ResetText();
                fwips.Clear();
                int TIMES = (int)numericUpDown1.Value;
                ManualResetEvent[] doneEvents = new ManualResetEvent[TIMES];
                ReadIPs[] array = new ReadIPs[TIMES];
                textBox3.Text = "Starting " + TIMES + " threads\r\n";
                int div = textBox1.Lines.Length / TIMES;
                for (int i = 0; i < TIMES; i++)
                {
                    doneEvents[i] = new ManualResetEvent(false);
                    ReadIPs f = new ReadIPs(i * div, ((i + 1) * div) > textBox1.Lines.Length ? textBox1.Lines.Length : ((i + 1) * div), textBox1.Lines, doneEvents[i], textBox3);
                    array[i] = f;
                    ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                }
                //WaitHandle.WaitAll(doneEvents);
                foreach (var ee in doneEvents)
                    ee.WaitOne();
                textBox3.Text += "Finished all";
                string fieldName = string.Empty;
                //MessageBox.Show(fwips.Count.ToString());
                foreach (DictionaryEntry de in fwips)
                {
                    fieldName = de.Key as string;
                    if (Convert.ToInt32(de.Value) >= (int)numericUpDown2.Value)
                        textBox2.Text += fieldName + "=" + de.Value + "\r\n";
                }
                label3.Text = "Unique IPs found = " + fwips.Count.ToString();
                thrRun = false;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace("\n", "\r\n");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            // iptables -A INPUT -s 1337 -j DROP
            string lines = string.Empty;
            for (int i = 0; i < textBox2.Lines.Length; i++)
                lines += "iptables -A INPUT -s " + textBox2.Lines[i].Split('=')[0] + " -j DROP\r\n";
            textBox2.Text = lines;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            string lines = string.Empty;
            for (int i = 0; i < textBox2.Lines.Length; i++)
                lines += textBox2.Lines[i].Split('=')[0] + "\r\n";
            textBox2.Text = lines;
        }
        public class ReadIPs
        {
            private int start = 0, end = 0;
            private string[] lines;
            private ManualResetEvent ev;
            private TextBox logtb;
            public ReadIPs(int start, int end, string[] lines, ManualResetEvent ev, TextBox logtb)
            {
                this.start = start;
                this.end = end;
                this.ev = ev;
                this.lines = lines;
                this.logtb = logtb;
            }
            public void ThreadPoolCallback(Object threadContext)
            {
                int threadIndex = (int)threadContext;
                Log("Thread " + threadIndex + " started");
                Read();
                Log("Thread " + threadIndex + " finished");
                ev.Set();
            }
            public void Read()
            {
                //MessageBox.Show("Start = " + start + ", end = " + end);
                for (int i = start; i < end && i < lines.Length; i++)
                {
                    var match = Regex.Match(lines[i], @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                    if (match.Success)
                    {
                        if (match.Value == "212.29.229.4")
                            continue;
                        if (fwips.ContainsKey(match.Value))
                            fwips[match.Value] = Convert.ToInt32(fwips[match.Value]) + 1;
                        else
                            fwips.Add(match.Value, 1);
                        //MessageBox.Show(fwips.Count.ToString() + " new");
                        total++;
                    }
                }
            }
            public void Log(string text)
            {
                /*if (text.Length == 0)
                {
                    if (logtb.InvokeRequired == true)
                        logtb.Invoke((MethodInvoker)delegate
                        {
                            logtb.ResetText();
                        });
                    else
                        logtb.ResetText();
                }
                else
                {
                    if (logtb.InvokeRequired == true)
                        logtb.Invoke((MethodInvoker)delegate
                        {
                            logtb.Text += text + "\r\n";
                        });
                    else
                        logtb.Text += text + "\r\n";
                }*/
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (thrRun)
                button5.Text = total + " / " + textBox1.Lines.Length;
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            ActiveForm.Text += value;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}