using System;
using System.Windows.Forms;
using Nelson.TicTacToe.Common;

namespace Nelson.TicTacToe.Client
{
    internal partial class FrmPlayerChoice : Form
    {
        public FrmPlayerChoice()
        {
            InitializeComponent();
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            string selection = cbPlayerChoice.SelectedItem as string;

            if (selection == null)
            {
                MessageBox.Show("Please select a player type.", "TicTacToe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FrmTicTacToe frm = new FrmTicTacToe();
            
            if (selection == "Cross")
            {
                frm.PlayerChoice = PlayerType.Cross;
            }
            else if (selection == "Zero")
            {
                frm.PlayerChoice = PlayerType.Zero;
            }

            Hide();
            frm.Show();
        }
    }
}
