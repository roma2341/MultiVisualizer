using MultiVisualiser.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MultiVisualiser
{
    public partial class Form1 : Form
    {
        private bool isStartButtonEnabled = false;
        private bool isStopButtonEnabled = false;
        private bool isFirstChunk = true;
        SoundService soundService = new SoundService();
        Chart[] charts;

        public Form1()
        {
            InitializeComponent();
            this.charts = new Chart[] { this.chart1, this.chart2, this.chart3, this.chart4 };
            foreach (var chart in charts)
            {
                chart.ChartAreas[0].AxisY.Maximum = Int16.MaxValue;
            }

            string[] driversNames = this.soundService.getDriverNamesAvailable();
            foreach (string name in driversNames)
            {
                this.comboBoxDriversNames.Items.Add(name);
            }
            this.comboBoxDriversNames.SelectedValueChanged += new EventHandler(DriverHameChanged);

        }

        private void setStartButtonEnabled(bool isEnabled)
        {
            /*check if state changed*/
            if(isStartButtonEnabled == isEnabled)
            {
                return;
            }

            this.btnRecordStart.Enabled = isEnabled;
            isStartButtonEnabled = isEnabled;
        }

        private void setStopButtonEnabled(bool isEnabled)
        {
            /*check if already enabled*/
            if (isStopButtonEnabled == isEnabled)
            {
                return;
            }

            this.btnRecordStop.Enabled = isEnabled;
            isStopButtonEnabled = isEnabled;
        }


        private void DriverHameChanged(object sender, EventArgs e)
        {
            setStartButtonEnabled(true);
            int selectedIndex = this.comboBoxDriversNames.SelectedIndex;
            if (selectedIndex != -1)
            {
                String driverName = this.comboBoxDriversNames.Items[selectedIndex].ToString();
                this.soundService.init(driverName,1, OnAsioOutAudioAvailable);
                for (int i = 1; i <= this.soundService.getMaxInputChannelsCount(); i++)
                {
                    this.сomboBoxChannelsCount.Items.Add(i);
                }
            }
        }

        private void writeInterleavedSamplesToCSV(float[] buffer,int channelsCount,bool append = true)
        {
            string fileName = this.textBoxFileName.Text;
            using (var w = new StreamWriter(fileName, append)) 
            {
                for (int i = 0; i < buffer.Length; i += channelsCount)
                {
                    if(i > 0)
                    {
                        w.WriteLine();
                    }
                    for (int j = 0; j < channelsCount; j++)
                    {
                        if (j > 0)
                        {
                            w.Write(',');
                        }
                        int intValue = (int) (buffer[i + j] * Int16.MaxValue);
                        w.Write(intValue);
                    }

                }
                w.Flush();

            }
        }

        private void OnAsioOutAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            float[] samplesBuffer = e.GetAsInterleavedSamples();     
            Console.WriteLine("STARTED");
            Console.WriteLine("SAMPLES_PER_BUFFER:" + e.SamplesPerBuffer.ToString());
            Console.WriteLine("INPUT_BUFFERS:" + e.InputBuffers.Length);
            Console.WriteLine("bytes:" + samplesBuffer.Length);
            int channelsCount = this.soundService.getCurrentChannelsCount();
            if (this.cbRecordFile.Checked)
            {
                bool appendToFile = this.isFirstChunk == false;
                writeInterleavedSamplesToCSV(samplesBuffer, channelsCount, appendToFile);
            }
            else {
                for (int i = 0; i < samplesBuffer.Length; i += channelsCount)
                {
                    for (int j = 0; j < channelsCount; j++)
                    {
                        Chart chart = this.charts[j];
                        float signalValue = samplesBuffer[i + j] * Int16.MaxValue;
                        chart.Invoke((MethodInvoker)delegate {
                            int maxIndex = (int)chart.ChartAreas[0].AxisX.Maximum;
                            chart.Series[0].Points.Add(signalValue);
                            int pointsCount = chart.Series[0].Points.Count;
                            if (pointsCount >= maxIndex)
                            {
                                chart.Series[0].Points.RemoveAt(0);
                                chart.ResetAutoValues();
                            }
                        });
                    }
                }
            }
            isFirstChunk = false;
            Console.WriteLine("HANDLED");

        }

        private void btnRecordStart_Click(object sender, EventArgs e)
        {
            int channelsCount = int.Parse(this.сomboBoxChannelsCount.SelectedItem.ToString());
            string driverName = this.comboBoxDriversNames.SelectedItem.ToString();
            soundService.startRecording(driverName,channelsCount, OnAsioOutAudioAvailable);
            setStopButtonEnabled(true);
            isFirstChunk = true;
        }

        private void btnRecordStop_Click(object sender, EventArgs e)
        {
            soundService.stopRecording();
            setStartButtonEnabled(true);
        }
    }
}
