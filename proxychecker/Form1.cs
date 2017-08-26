using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proxychecker
{
    public partial class Form1 : Form
    {

        int Working = 0;
        int Broken = 0;
        int Timeout;
        int progress = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateStatus(bool goodProxy, string Proxy)
        {
            Invoke(new MethodInvoker(
             delegate () {
                 if (goodProxy)
                 {
                     richTextBox2.Text += Proxy + Environment.NewLine;
                 }
             }));
        }


        public void threadjob()
        {
            Working = 0;
            Broken = 0;
            Timeout = 10000;
            string[] Lines = richTextBox1.Lines;

            new Thread(
              delegate (object o)
              {
                  try
                  {
                      progress++;
                      string[] Proxy = ((string)o).Split(':');
                      WebRequest req = WebRequest.Create("http://www.futhead.com");
                      req.Proxy = new WebProxy(Proxy[0], Convert.ToInt32(Proxy[1]));
                      req.Timeout = Timeout;
                      req.GetResponse();
                      UpdateStatus(true, string.Join(":", Proxy));
                  }
                  catch
                  {
                      UpdateStatus(false, "");
                  }

              }
              ).Start(Lines[progress]);

        }




        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Lines = richTextBox1.Lines.Distinct().ToArray();
            richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline);
            label5.Text = "" + Convert.ToInt32(richTextBox1.Lines.Length);
            richTextBox2.Clear();

            backgroundWorker1.RunWorkerAsync();

            label1.Text = "Status: done";


        }

        private void button2_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|RTF Files (*.rtf)|*.rtf";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var extension = System.IO.Path.GetExtension(saveFileDialog.FileName);
                if (extension.ToLower() == ".txt") /*saveFileDialog.FilterIndex==1*/
                    richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                else
                    richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            label1.Text = "Status: Working...";
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            richTextBox1.Enabled = false;
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                threadjob();
            }

            if (progress >= richTextBox1.Lines.Length)
            {
                label1.Text = "Status: Done checking proxies!";
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                richTextBox1.Enabled = true;
                label6.Text = "" + richTextBox2.Lines.Length;
                progress = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Lines = richTextBox1.Lines.Distinct().ToArray();
            richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline);
        }
    }
}