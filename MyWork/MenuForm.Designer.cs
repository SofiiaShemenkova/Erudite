namespace MyWork
{
    partial class MenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            this.label1 = new System.Windows.Forms.Label();
            this.btn_new_game = new System.Windows.Forms.Button();
            this.btn_help = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Monotype Corsiva", 38F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(110, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(296, 79);
            this.label1.TabIndex = 0;
            this.label1.Text = "ЭРУДИТ";
            // 
            // btn_new_game
            // 
            this.btn_new_game.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btn_new_game.Location = new System.Drawing.Point(124, 190);
            this.btn_new_game.Name = "btn_new_game";
            this.btn_new_game.Size = new System.Drawing.Size(282, 46);
            this.btn_new_game.TabIndex = 1;
            this.btn_new_game.Text = "НОВАЯ ИГРА";
            this.btn_new_game.UseVisualStyleBackColor = false;
            this.btn_new_game.Click += new System.EventHandler(this.btn_new_game_Click);
            // 
            // btn_help
            // 
            this.btn_help.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_help.Location = new System.Drawing.Point(124, 268);
            this.btn_help.Name = "btn_help";
            this.btn_help.Size = new System.Drawing.Size(282, 42);
            this.btn_help.TabIndex = 2;
            this.btn_help.Text = "ПОМОЩЬ";
            this.btn_help.UseVisualStyleBackColor = false;
            this.btn_help.Click += new System.EventHandler(this.btn_help_Click);
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(515, 453);
            this.Controls.Add(this.btn_help);
            this.Controls.Add(this.btn_new_game);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ЭРУДИТ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_new_game;
        private System.Windows.Forms.Button btn_help;
    }
}