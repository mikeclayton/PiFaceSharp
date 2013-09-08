using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Kingsland.PiFaceSharp;
using Kingsland.PiFaceSharp.Emulators;
using Kingsland.PiFaceSharp.Remote;
using System.Diagnostics;

namespace Kingsland.PiFaceSharp.Emulator
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        #region Properties

        private PiFaceEmulator PiFaceEmulator
        {
            get;
            set;
        }

        private PiFaceTcpServer PiFaceTcpServer
        {
            get;
            set;
        }

        #endregion

        #region Form Event Handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            // initialise the piface emulator
            this.PiFaceEmulator = new PiFaceEmulator();
            this.PiFaceEmulator.OutputPinStateChanged += this.PiFaceEmulator_OnOutputPinStateChanged;
            this.PiFaceEmulator.InputPinStateChanged += this.PiFaceEmulator_OnInputPinStateChanged;
            // intitialise form controls
            var settings = Properties.Settings.Default;
            txtLocalAddress.Text = settings.LocalAddress;
            if (string.IsNullOrEmpty(txtLocalAddress.Text))
            {
                txtLocalAddress.Text = PiFaceTcpHelper.GetLocalIPAddress().ToString();
            }
            txtLocalAddress.Enabled = true;
            txtLocalPort.Text = settings.LocalPort;
            if(string.IsNullOrEmpty(txtLocalPort.Text))
            {
                txtLocalPort.Text = "43596";
            }
            txtLocalPort.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblStatus.Text = "Stopped.";
            btnStart_Click(null, null);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // initialise the piface server
            var address = PiFaceTcpHelper.GetLocalIPAddress();
            var endpoint = new IPEndPoint(address, 43596);
            this.PiFaceTcpServer = new PiFaceTcpServer(this.PiFaceEmulator, endpoint);
            this.PiFaceTcpServer.Start();
            // update form controls
            txtLocalAddress.Enabled = false;
            txtLocalPort.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "Started.";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Stopping...";
            this.PiFaceTcpServer.Stop();
            // update form controls
            txtLocalAddress.Enabled = true;
            txtLocalPort.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblStatus.Text = "Stopped.";
        }

        private void lnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(lnkGitHub.Text);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Emulator Event Handlers

        private void PiFaceEmulator_OnOutputPinStateChanged(object sender, EventArgs e)
        {
            this.RefreshPiFacePreview();
        }

        private void PiFaceEmulator_OnInputPinStateChanged(object sender, EventArgs e)
        {
            this.RefreshPiFacePreview();
        }

        #endregion
        
        private void RefreshPiFacePreview()
        {

            var bitmap = (Bitmap)Properties.Resources.PiFaceBackground.Clone();
            using(var graphics = Graphics.FromImage(bitmap))
            {
                var outputState = this.PiFaceEmulator.GetOutputPinStates();
                var outputPinOn = Properties.Resources.PiFaceOutputPinOn;
                var outputLedOn = Properties.Resources.PiFaceOutputLedOn;
                for (var pin = 0; pin < 8; pin++)
                {
                    if ((outputState & (0x01 << pin)) > 0)
                    {
                        graphics.DrawImage(outputPinOn, 242 - pin * 12, 7);
                        graphics.DrawImage(outputLedOn, 241 - pin * 12, 25);
                    }
                }
                var inputState = this.PiFaceEmulator.GetInputPinStates();
                var inputPinOn = Properties.Resources.PiFaceInputPinOn;
                var inputButtonOn = Properties.Resources.PiFaceInputButtonOn;
                for (var pin = 0; pin < 8; pin++)
                {
                    if ((inputState & (0x01 << pin)) > 0)
                    {
                        graphics.DrawImage(inputPinOn, 6 + pin * 12, 184);
                        if (pin < 4)
                        {
                            graphics.DrawImage(inputButtonOn, 14 + pin * 25, 154);
                        }
                    }
                }
            }
            this.Invoke((MethodInvoker)delegate {
                this.PiFacePreview.Image = bitmap;
            });
        }

    }

}
