using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyWork
{
    public partial class PlayerSetupForm : Form
    {
        public int PlayerCount { get; private set; }

        public PlayerSetupForm()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            PlayerCount = (int)numPlayers.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}