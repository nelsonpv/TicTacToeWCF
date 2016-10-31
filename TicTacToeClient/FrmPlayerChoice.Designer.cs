namespace Nelson.TicTacToe.Client
{
    partial class FrmPlayerChoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPlayerChoice));
            this.cbPlayerChoice = new System.Windows.Forms.ComboBox();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.lblPlayerChoice = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbPlayerChoice
            // 
            this.cbPlayerChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPlayerChoice.FormattingEnabled = true;
            this.cbPlayerChoice.Items.AddRange(new object[] {
            "Cross",
            "Zero"});
            this.cbPlayerChoice.Location = new System.Drawing.Point(116, 42);
            this.cbPlayerChoice.MaxDropDownItems = 2;
            this.cbPlayerChoice.Name = "cbPlayerChoice";
            this.cbPlayerChoice.Size = new System.Drawing.Size(121, 21);
            this.cbPlayerChoice.TabIndex = 0;
            // 
            // btnStartGame
            // 
            this.btnStartGame.Location = new System.Drawing.Point(116, 84);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(75, 23);
            this.btnStartGame.TabIndex = 1;
            this.btnStartGame.Text = "Start game";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // lblPlayerChoice
            // 
            this.lblPlayerChoice.AutoSize = true;
            this.lblPlayerChoice.Location = new System.Drawing.Point(33, 45);
            this.lblPlayerChoice.Name = "lblPlayerChoice";
            this.lblPlayerChoice.Size = new System.Drawing.Size(77, 13);
            this.lblPlayerChoice.TabIndex = 2;
            this.lblPlayerChoice.Text = "Player choice :";
            // 
            // frmPlayerChoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 145);
            this.Controls.Add(this.lblPlayerChoice);
            this.Controls.Add(this.btnStartGame);
            this.Controls.Add(this.cbPlayerChoice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmPlayerChoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TicTacToe";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPlayerChoice;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Label lblPlayerChoice;
    }
}