namespace Online_Battleship
{
    partial class OnlineBattleshipForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being us ed.
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arkivToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGame = new System.Windows.Forms.ToolStripMenuItem();
            this.quit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.lblSetShip = new System.Windows.Forms.Label();
            this.lblGameOver = new System.Windows.Forms.Label();
            this.btnJoinGame = new System.Windows.Forms.Button();
            this.textYourTurn = new System.Windows.Forms.Label();
            this.textIPAddress = new System.Windows.Forms.TextBox();
            this.numericPort = new System.Windows.Forms.NumericUpDown();
            this.textPort = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.arkivToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(150, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arkivToolStripMenuItem
            // 
            this.arkivToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.newGame,
                this.quit});
            this.arkivToolStripMenuItem.Name = "arkivToolStripMenuItem";
            this.arkivToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.arkivToolStripMenuItem.Text = "Game";
            // 
            // newGame
            // 
            this.newGame.Name = "newGame";
            this.newGame.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newGame.Size = new System.Drawing.Size(175, 22);
            this.newGame.Text = "New Game";
            this.newGame.Click += new System.EventHandler(this.NewGameItem_Click);
            // 
            // quit
            // 
            this.quit.Name = "quit";
            this.quit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quit.Size = new System.Drawing.Size(175, 22);
            this.quit.Text = "Quit";
            this.quit.Click += new System.EventHandler(this.QuitItem_Click);
            // 
            // btnStartGame
            // 
            this.btnStartGame.Enabled = false;
            this.btnStartGame.Location = new System.Drawing.Point(13, 28);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(74, 23);
            this.btnStartGame.TabIndex = 1;
            this.btnStartGame.Text = "Host";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.BtnStartGame_Click);
            // 
            // lblSetShip
            // 
            this.lblSetShip.AutoSize = true;
            this.lblSetShip.Location = new System.Drawing.Point(174, 33);
            this.lblSetShip.Name = "lblSetShip";
            this.lblSetShip.Size = new System.Drawing.Size(84, 13);
            this.lblSetShip.TabIndex = 2;
            this.lblSetShip.Text = "Place your ships";
            // 
            // lblGameOver
            // 
            this.lblGameOver.AutoSize = true;
            this.lblGameOver.BackColor = System.Drawing.Color.Silver;
            this.lblGameOver.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGameOver.Font = new System.Drawing.Font("Consolas", 44.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameOver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblGameOver.Location = new System.Drawing.Point(145, 147);
            this.lblGameOver.Name = "lblGameOver";
            this.lblGameOver.Size = new System.Drawing.Size(288, 71);
            this.lblGameOver.TabIndex = 3;
            this.lblGameOver.Text = "YOU WON!";
            this.lblGameOver.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblGameOver.Visible = false;
            // 
            // btnJoinGame
            // 
            this.btnJoinGame.Enabled = false;
            this.btnJoinGame.Location = new System.Drawing.Point(93, 28);
            this.btnJoinGame.Name = "btnJoinGame";
            this.btnJoinGame.Size = new System.Drawing.Size(75, 23);
            this.btnJoinGame.TabIndex = 4;
            this.btnJoinGame.Text = "Client";
            this.btnJoinGame.UseVisualStyleBackColor = true;
            this.btnJoinGame.Click += new System.EventHandler(this.btnJoinGame_Click);
            // 
            // textYourTurn
            // 
            this.textYourTurn.AutoSize = true;
            this.textYourTurn.Enabled = false;
            this.textYourTurn.Location = new System.Drawing.Point(264, 33);
            this.textYourTurn.Name = "textYourTurn";
            this.textYourTurn.Size = new System.Drawing.Size(54, 13);
            this.textYourTurn.TabIndex = 5;
            this.textYourTurn.Text = "Your Turn";
            // 
            // textIPAddress
            // 
            this.textIPAddress.Location = new System.Drawing.Point(454, 0);
            this.textIPAddress.Name = "textIPAddress";
            this.textIPAddress.Size = new System.Drawing.Size(100, 20);
            this.textIPAddress.TabIndex = 6;
            // 
            // numericPort
            // 
            this.numericPort.Location = new System.Drawing.Point(592, 0);
            this.numericPort.Name = "numericPort";
            this.numericPort.Size = new System.Drawing.Size(51, 20);
            this.numericPort.TabIndex = 7;
            // 
            // textPort
            // 
            this.textPort.AutoSize = true;
            this.textPort.Location = new System.Drawing.Point(560, 2);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(26, 13);
            this.textPort.TabIndex = 8;
            this.textPort.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(390, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "IP Address";
            // 
            // BattleshipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 361);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPort);
            this.Controls.Add(this.numericPort);
            this.Controls.Add(this.textIPAddress);
            this.Controls.Add(this.textYourTurn);
            this.Controls.Add(this.btnJoinGame);
            this.Controls.Add(this.lblGameOver);
            this.Controls.Add(this.lblSetShip);
            this.Controls.Add(this.btnStartGame);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "BattleshipForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Battle Ship";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arkivToolStripMenuItem;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Label lblSetShip;
        private System.Windows.Forms.ToolStripMenuItem newGame;
        private System.Windows.Forms.ToolStripMenuItem quit;
        private System.Windows.Forms.Label lblGameOver;
        private System.Windows.Forms.Button btnJoinGame;
        private System.Windows.Forms.Label textYourTurn;
        private System.Windows.Forms.TextBox textIPAddress;
        private System.Windows.Forms.NumericUpDown numericPort;
        private System.Windows.Forms.Label textPort;
        private System.Windows.Forms.Label label1;
    }
}

