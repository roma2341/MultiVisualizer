using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiVisualiser.Services
{
    class SoundService
    {
        AsioOut asioOut;
        int SAMPLE_RATE = 44100;
        bool inited = false;
        int channels = 1;
        public void startRecording(string driverName,int channels, EventHandler<AsioAudioAvailableEventArgs> recordHandler)
        {
            this.init(driverName, channels, recordHandler);
            this.asioOut.Play();
        }
        public void stopRecording()
        {
            if (this.asioOut != null)
            {
                this.asioOut.Stop();
            }
        }
        public void init(string driverName,int channels,EventHandler<AsioAudioAvailableEventArgs> recordHandler)
        {
            if (this.asioOut != null)
            {
                this.asioOut.Stop();
                this.asioOut.Dispose();
            }
            this.channels = channels;
            this.asioOut = new AsioOut(driverName);
            this.asioOut.AudioAvailable += recordHandler;
            this.asioOut.InitRecordAndPlayback(null, channels, SAMPLE_RATE);

        }
        public string[] getDriverNamesAvailable()
        {
            return AsioOut.GetDriverNames();
        }
        public int getInputChannelsCount()
        {
            var inputChannels = this.asioOut.DriverInputChannelCount;
            return inputChannels;
        }


    }
}
