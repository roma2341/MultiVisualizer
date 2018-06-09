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
using System.Windows.Forms.DataVisualization.Charting;

namespace MultiVisualiser
{
    public partial class Form1 : Form
    {
        SoundService soundService = new SoundService();
        float[] samplesBuffer = new float[512];
        Chart[] charts;
        int CHART_MAX_VALUES = 3000;

        public Form1()
        {
            InitializeComponent();
            this.charts = new Chart[] { this.chart1, this.chart2, this.chart3, this.chart4 };
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
                this.soundService.init(driverName,1, OnAsioOutAudioAvailable);
                for (int i = 1; i <= this.soundService.getInputChannelsCount(); i++)
                {
                    this.сomboBoxChannelsCount.Items.Add(i);
                }
            }
        }

        private void OnAsioOutAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            e.GetAsInterleavedSamples(samplesBuffer);
            Console.WriteLine("SAMPLES_PER_BUFFER:" + e.SamplesPerBuffer.ToString());
            Console.WriteLine("INPUT_BUFFERS:" + e.InputBuffers.Length);
            Console.WriteLine("bytes:" + samplesBuffer.Length);
            int channelsCount = this.soundService.getInputChannelsCount();
            for (int i = 0; i < this.samplesBuffer.Length; i += channelsCount)
            {
                for (int j = 0; j < channelsCount; j++)
                {
                    Chart chart = this.charts[j];
                    float signalValue = this.samplesBuffer[i + j];
                    chart.Invoke((MethodInvoker)delegate {
                        if(chart.Series[0].Points.Count > CHART_MAX_VALUES )
                        {
                            chart.Series[0].Points.Clear();
                        }
                        chart.Series[0].Points.Add(signalValue);
                    });
                }
            }

        }

        private void btnRecordStart_Click(object sender, EventArgs e)
        {
            int channelsCount = int.Parse(this.сomboBoxChannelsCount.SelectedItem.ToString());
            string driverName = this.comboBoxDriversNames.SelectedItem.ToString();
            this.soundService.startRecording(driverName,channelsCount, OnAsioOutAudioAvailable);
        }

        private void btnRecordStop_Click(object sender, EventArgs e)
        {
            this.soundService.stopRecording();
        }
    }
}
