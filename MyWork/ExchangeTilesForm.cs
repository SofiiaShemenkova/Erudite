using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyWork
{
    public partial class ExchangeTilesForm : Form
    {
        private List<Tile> playerTiles;
        private List<CheckBox> checkBoxes;

        public List<Tile> SelectedTiles { get; private set; }

        public ExchangeTilesForm(List<Tile> tiles)
        {
            InitializeComponent();
            playerTiles = tiles ?? new List<Tile>();
            SelectedTiles = new List<Tile>();
            checkBoxes = new List<CheckBox>();
        }   

        private void ExchangeTilesForm_Load(object sender, EventArgs e)
        {
            CreateCheckBoxes();
        }

        private void CreateCheckBoxes()
        {
            panelTiles.Controls.Clear();
            checkBoxes.Clear();

            if (playerTiles == null || playerTiles.Count == 0)
            {
                Label noTilesLabel = new Label();
                noTilesLabel.Text = "Нет фишек для обмена";
                noTilesLabel.Location = new Point(10, 10);
                noTilesLabel.Size = new Size(200, 30);
                panelTiles.Controls.Add(noTilesLabel);
                btnOk.Enabled = false;
                return;
            }

            int y = 10;
            foreach (Tile tile in playerTiles)
            {
                CheckBox cb = new CheckBox();
                cb.Text = $"{tile.Letter} ({tile.Score} очков)";
                cb.Tag = tile;
                cb.Location = new Point(10, y);
                cb.Size = new Size(200, 30);
                cb.Font = new Font("Arial", 10, FontStyle.Regular);
                cb.Checked = false;

                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(cb, $"Нажмите, чтобы отметить фишку {tile.Letter} для обмена");

                checkBoxes.Add(cb);
                panelTiles.Controls.Add(cb);
                y += 35;
            }

            btnOk.Enabled = playerTiles.Count > 0;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            SelectedTiles.Clear();
            foreach (CheckBox cb in checkBoxes)
            {
                if (cb.Checked)
                {
                    SelectedTiles.Add((Tile)cb.Tag);
                }
            }

            if (SelectedTiles.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите хотя бы одну фишку для обмена.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedTiles.Clear();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void lblInstruction_Click(object sender, EventArgs e)
        {

        }
    }
}