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
        void recordSound()
        {
        }
        public void init(string driverName)
        {
            this.asioOut = new AsioOut(driverName);
        }
        public string[] getDriverNamesAvailable()
        {
            return AsioOut.GetDriverNames();
        }
        int getInputChannelsCount()
        {
            var inputChannels = this.asioOut.DriverInputChannelCount;
            return inputChannels;
        }
    }
}
