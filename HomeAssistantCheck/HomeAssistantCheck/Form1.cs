using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using Microsoft.Win32;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace HomeAssistantCheck
{
    public partial class Form1 : Form
    {
        string RegistryApplicationKey = "";

        public Form1()
        {
            int Count = 0;
            InitializeComponent();
            RegistryApplicationKey = "SOFTWARE\\" + Application.ProductName;
            textBox1.Text = GetStringValue("LocalURL", "homeassistant.local");
            textBox2.Text = GetStringValue("ExternalURL", "somename.duckdns.org");
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            for (Count=0;Count<60000;Count++)
            {
                comboBox1.Items.Add(Count.ToString());
                comboBox2.Items.Add(Count.ToString());
            }
            comboBox1.SelectedIndex = GetIntValue("LocalPort", 8123);
            comboBox2.SelectedIndex = GetIntValue("ExternalPort", 8123);
        }

        private bool StoreStringValue(string ValueStr, string Str)
        {
            string StoredValue;
            RegistryKey rkey = Registry.CurrentUser.CreateSubKey(RegistryApplicationKey);
            if (rkey != null)
            {
                StoredValue = (string)rkey.GetValue(ValueStr, Str + "!");
                if (Str != StoredValue) rkey.SetValue(ValueStr, Str);
                return (true);
            }
            return false;
        }

        private string GetStringValue(string ValueStr, string Default)
        {
            string Result = Default;
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(RegistryApplicationKey);
            if (rkey != null)
            {
                Result = (string)rkey.GetValue(ValueStr, Default);
            }
            return (Result);
        }

        private bool StoreIntValue(string ValueStr, int Number)
        {
            string TmpStr = Number.ToString();
            string StoredValue;
            RegistryKey rkey = Registry.CurrentUser.CreateSubKey(RegistryApplicationKey);
            if (rkey != null)
            {
                StoredValue = (string)rkey.GetValue(ValueStr, "-1");
                if (TmpStr != StoredValue) rkey.SetValue(ValueStr, TmpStr);
                return (true);
            }
            return false;
        }

        private int GetIntValue(string ValueStr, int Default)
        {
            string TmpStr = Default.ToString();
            int res;
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(RegistryApplicationKey);
            if (rkey != null)
            {
                TmpStr = (string)rkey.GetValue(ValueStr, Default.ToString());
            }
            if (int.TryParse(TmpStr, out res))
            {
                return (res);
            }
            return (Default);
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        bool IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    client.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPHostEntry hostInfo = null;
            IPAddress[] IPNumber;
            button1.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            checkBox4.Enabled = false;
            checkBox5.Enabled = false;
            checkBox6.Enabled = false;
            checkBox7.Enabled = false;
            checkBox1.Checked = PingHost("8.8.8.8");
            checkBox1.Enabled = true;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            label1.Text = "";
            label2.Text = "";
            try
            {
                hostInfo = Dns.GetHostEntry(textBox1.Text);
            }
            catch
            {
                //Something went wrong!!
            }
            if (hostInfo != null)
            {
                checkBox2.Checked = true;
                IPNumber = hostInfo.AddressList;
                label1.Text = IPNumber[0].ToString();
                Application.DoEvents(); // Force a repaint of the form
            }
            checkBox2.Enabled = true;
            if (label1.Text != "")
            {
                checkBox3.Checked = PingHost(label1.Text);
                checkBox3.Enabled = true;
                Application.DoEvents(); // Force a repaint of the form
                checkBox6.Checked = IsPortOpen(label1.Text, comboBox1.SelectedIndex, TimeSpan.FromMilliseconds(1000));
                checkBox6.Enabled = true;
                Application.DoEvents(); // Force a repaint of the form
            }
            checkBox3.Enabled = true;
            checkBox6.Enabled = true;
            hostInfo = null;
            try
            {
                hostInfo = Dns.GetHostEntry(textBox2.Text);
            }
            catch
            {
                //Something went wrong!!
            }            
            if (hostInfo != null)
            {
                checkBox4.Checked = true;
                IPNumber = hostInfo.AddressList;
                label2.Text = IPNumber[0].ToString();
                Application.DoEvents(); // Force a repaint of the form
            }
            checkBox4.Enabled = true;
            if (label2.Text != "")
            {
                //Ping to external ip sometimes fails while "pinging" to the external ip and port is successful
                checkBox5.Checked = PingHost(label2.Text);
                checkBox5.Enabled = true;
                Application.DoEvents(); // Force a repaint of the form
                checkBox7.Checked = IsPortOpen(label2.Text, comboBox2.SelectedIndex, TimeSpan.FromMilliseconds(1000));
                checkBox7.Enabled = true;
            }
            checkBox5.Enabled = true;            
            checkBox7.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            StoreStringValue("LocalURL", textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            StoreStringValue("ExternalURL", textBox2.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreIntValue("LocalPort", comboBox1.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreIntValue("ExternalPort", comboBox2.SelectedIndex);
        }
    }
}
