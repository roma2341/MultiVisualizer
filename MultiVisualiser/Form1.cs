using MultiVisualiser.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVisualiser
{
    public partial class Form1 : Form
    {
        SoundService soundService = new SoundService();
        public Form1()
        {
            InitializeComponent();
            string[] driversNames = this.soundService.getDriverNamesAvailable();
            foreach (string name in driversNames)
            {
                this.comboBoxDriversNames.Items.Add(name);
            }
            this.comboBoxDriversNames.SelectedValueChanged += new EventHandler(DriverHameChanged);

        }

        private void DriverHameChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.comboBoxDriversNames.SelectedIndex;
            if (selectedIndex != -1)
            {
                String driverName = this.comboBoxDriversNames.Items[selectedIndex].ToString();
                this.soundService.init(driverName,1);
                for (int i = 1; i <= this.soundService.getInputChannelsCount(); i++)
                {
                    this.сomboBoxChannelsCount.Items.Add(i);
                }
            }
        }


        private void btnRecordStart_Click(object sender, EventArgs e)
        {
            int channelsCount = int.Parse(this.сomboBoxChannelsCount.SelectedItem.ToString());
            string driverName = this.comboBoxDriversNames.SelectedItem.ToString();
            this.soundService.startRecording(driverName,channelsCount);
        }

        private void btnRecordStop_Click(object sender, EventArgs e)
        {
            this.soundService.stopRecording();
        }
    }
}
