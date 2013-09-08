using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kingsland.PiFaceSharp.Remote;
using System.Net;

namespace Kingsland.PiFaceSharp.SampleClient
{

    public partial class Form1 : Form
    {

        #region Constructors

        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private PiFaceTcpClient PiFaceClient
        {
            get;
            set;
        }

        #endregion

        #region Form Event Handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            var localEndPoint = new IPEndPoint(PiFaceTcpHelper.GetLocalIPAddress(), 43594);
            var remoteEndPoint = new IPEndPoint(PiFaceTcpHelper.GetLocalIPAddress(), 43596);
            this.PiFaceClient = new PiFaceTcpClient(localEndPoint, remoteEndPoint);
            this.PiFaceClient.Connect();
        }

        #endregion

        #region Output Pin Event Handlers

        private void chkOutputPin0_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(0, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin1_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(1, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin2_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(2, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin3_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(3, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin4_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(4, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin5_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(5, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin6_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(6, ((CheckBox)sender).Checked);
        }

        private void chkOutputPin7_CheckedChanged(object sender, EventArgs e)
        {
            this.PiFaceClient.SetOutputPinState(7, ((CheckBox)sender).Checked);
        }

        #endregion

    }

}
