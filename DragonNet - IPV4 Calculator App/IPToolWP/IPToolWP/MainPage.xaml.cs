using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace IPToolWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IPV4Tool IP;
        private bool IPSet = false;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            //Set 192.168.0.1 as default case
            SetIP();
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            
            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void SetIP()
        {
            String IPAddress = IPField1.Text.ToString() + "." + IPField2.Text.ToString() + "." + IPField3.Text.ToString() + "." + IPField4.Text.ToString();
            IP = new IPV4Tool(IPAddress);

            //Check to make sure the IP address is valid before getting valid netmasks
            if (IP.GetIP() != null)
            {
                //Set Possible masks
                int Index = IP.GetNetworkClassPrefix();
                SCIDR.Minimum = Index;
                SCIDR.Maximum = 30;
                SCIDR.Value = Index;
                IPSet = true;
            }
            UpdateGUI();
        }
        private void UpdateGUI()
        {
            if (IP.GetIP() != null)
            {
                //IP Class
                Class.Text = "Class: " + IP.GetNetworkClassName();
                //Text if Class D or E
                if (IP.GetNetworkClassName() == "D" || IP.GetNetworkClassName() == "E")
                {
                    //Mask
                    Mask.Text = "Mask: " + "N/A";
                    //Prefix
                    CIDR.Text = "/ " + "N/A";
                    //Max Hosts
                    MHosts.Text = "Max Hosts: " + "N/A";
                    //Max Subnets
                    MSubnets.Text = "Max Subnets: " + "N/A";
                    //Range of Ip Addresses
                    IPRange.Text = "IP Range: " + "N/A" + " - " + "N/A";
                    //Network
                    Net.Text = "Network: " + "N/A";
                    //Broadcast
                    Broadcast.Text = "Broadcast: " + "N/A";
                }
                else
                {
                    //Mask
                    Mask.Text = "Mask: " + IP.GetNetworkMaskByPrefix((int)SCIDR.Value);
                    //Prefix
                    CIDR.Text = "/ " + SCIDR.Value;
                    IP.setNetworkPrefix((int)SCIDR.Value);
                    //Max Hosts
                    MHosts.Text = "Max Hosts: " + IP.GetMaxNetworkHosts();
                    //Max Subnets
                    MSubnets.Text = "Max Subnets: " + IP.GetMaxSubNets();
                    //Range of Ip Addresses
                    string[] IPArray = IP.GetNetworkIPRange();
                    IPRange.Text = "IP Range: " + IPArray[0] + " - " + IPArray[1];
                    //Network
                    Net.Text = "Network: " + IP.GetNetwork();
                    //Broadcast
                    Broadcast.Text = "Broadcast: " + IP.GetBroadCast();
                }
            }
           // Invalid IP Address     
            else
            {
                //IP Class
                Class.Text = "Class: " + "Invalid";
                //Mask
                Mask.Text = "Invalid IP Address";
                //Prefix
                CIDR.Text = "";
                //Max Hosts
                MHosts.Text = "Max Hosts: " + "Invalid";
                //Max Subnets
                MSubnets.Text = "Max Subnets: " + "Invalid";
                //Range of Ip Addresses
                IPRange.Text = "IP Range: " + "Invalid" + " - " + "Invalid";
                //Network
                Net.Text = "Network: " + "Invalid";
                //Broadcast
                Broadcast.Text = "Broadcast: " + "Invalid";
            }
        }
        //Slider Value changes
        private void SCIDR_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //only call if the IP address is set
            if (IPSet) UpdateGUI();
        }
        //Textbox values changed
        private void IPField1_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetIP();
        }

        private void IPField2_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetIP();
        }

        private void IPField3_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetIP();
        }

        private void IPField4_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetIP();
        }

        //Clear Textbox when selected
        private void IPField1_GotFocus(object sender, RoutedEventArgs e)
        {
            IPField1.Text = "";
        }

        private void IPField2_GotFocus(object sender, RoutedEventArgs e)
        {
            IPField2.Text = "";
        }

        private void IPField3_GotFocus(object sender, RoutedEventArgs e)
        {
            IPField3.Text = "";
        }

        private void IPField4_GotFocus(object sender, RoutedEventArgs e)
        {
            IPField4.Text = "";
        }

        private void Usable_Click(object sender, RoutedEventArgs e)
        {
            if (Usable.IsChecked == true)
            {
                IP.SetAllow1STBit(true);
                UpdateGUI();
            }
            else
            {
                IP.SetAllow1STBit(false);
                UpdateGUI();
            }
        }

    }
}
