using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NativeWifi;

namespace WindowsFormsAppMM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void GetWifiInfo()
        {
            using (var client = new WlanClient())
            {

                try
                {
                                   
                    StringBuilder sb = new StringBuilder();

                    WlanClient.WlanInterface wlanIface = client.Interfaces.FirstOrDefault();
                    wlanIface.Scan();

                    Wlan.WlanBssEntry[] wlanBssEntries = wlanIface.GetNetworkBssList();

                    int baseCount = wlanBssEntries.Length - 1;
                  
                    Random rnd = new Random();

                    int rndSelectedBss = 0;

                    if (baseCount >= 0)
                    {
                        rndSelectedBss = rnd.Next(0, baseCount);

                        Wlan.WlanBssEntry network = wlanBssEntries[rndSelectedBss];

                        byte[] macAddr = network.dot11Bssid;

                        string tMac = "";

                        for (int i = 0; i < macAddr.Length; i++)
                        {

                            tMac += macAddr[i].ToString("x2").PadLeft(2, '0').ToUpper();

                        }


                        sb.Append("");

                        sb.AppendLine("Found network with SSID: " +
                           System.Text.ASCIIEncoding.ASCII.GetString(network.dot11Ssid.SSID).ToString().Replace("\0", "")
                            );

                        sb.AppendLine("Signal: " + network.linkQuality.ToString() + " percent.");

                        sb.AppendLine("BSS Type: " + network.dot11BssType.ToString());

                        sb.AppendLine("MAC: " + tMac.ToString());

                        sb.AppendLine("RSS: " + network.rssi.ToString());

                        sb.AppendLine("Random: " + SeedRandom(network.linkQuality, network.rssi));

                        sb.AppendLine();

                        textBox1.Text += sb.ToString();

                    }
                    else
                    {
                        GetWifiInfo(); // try again
                    }
    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
            GetWifiInfo();
        }

        private string SeedRandom(uint lqual, int rssi)
        {

            int lqualconvert = Convert.ToInt32(lqual.ToString());
            int rssiconvert = Convert.ToInt32(rssi.ToString().Replace("-", ""));

            int guidhash = (Guid.NewGuid().GetHashCode());

            DateTime dt = DateTime.Now;

            string dtticks = dt.Ticks.ToString();

            int smallticksLength = dtticks.Length;

            char[] ticksArray = new char[smallticksLength];

            ticksArray = dtticks.ToCharArray();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < smallticksLength; i++)
            {

                if (i >= (smallticksLength / 2))
                    sb.Append(ticksArray[i].ToString());

            }

            int smallticks = Convert.ToInt32(sb.ToString());

            //int seed = 2; // returns the same random. So below will create new random
            // based on time, wifi signal of a random access point, and hash of GUID
            int seed = (lqualconvert * rssiconvert) + guidhash + smallticks;


            Random randObj = new Random(seed);

            int randout = randObj.Next();
            return randout.ToString();

        }
    }
}
