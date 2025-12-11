using System;
using System.Windows.Forms;

namespace MyWork
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void btn_help_Click(object sender, EventArgs e)
        {
            HelpForm form_rules = new HelpForm();
            form_rules.Owner = this;
            this.Hide();
            form_rules.ShowDialog();
            this.Show();
        }

        private void btn_new_game_Click(object sender, EventArgs e)
        {
            using (PlayerSetupForm setupForm = new PlayerSetupForm())
            {
                if (setupForm.ShowDialog() == DialogResult.OK)
                {
                    GameForm gameForm = new GameForm(setupForm.PlayerCount);
                    this.Hide();
                    gameForm.ShowDialog();
                    this.Show();
                }
            }
        }
    }
}