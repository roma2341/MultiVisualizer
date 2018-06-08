using MultiVisualiser.Services;
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
        }

        private void btnRecordStart_Click(object sender, EventArgs e)
        {
            this.soundService.init(this.comboBoxDriversNames.SelectedItem.ToString());
        }
    }
}
