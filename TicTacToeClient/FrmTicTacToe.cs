using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.ServiceModel;
using System.Windows.Forms;
using Nelson.TicTacToe.Common;

namespace Nelson.TicTacToe.Client
{
    internal partial class FrmTicTacToe : Form, ITicTacToeCallback
    {
        private List<WinVector> _winVectorsToPaint = new List<WinVector>();     // Holds instructions for painting on the screen.
        private MoveMetadata[,] _moveMatrix = new MoveMetadata[3, 3];           // 3x3 matrix to hold the move data of both the players.
        private DuplexChannelFactory<ITicTacToeChannel> _duplexChannelFactory;  // Duplex channel factory.
        private ITicTacToeChannel _ticTacToeChannel;                            // Client channel.
        private PlayerType? _playerChoice;                                      // Player choice.
        private PlayerType _playerType;                                         // Indicates the player type.
        private Graphics _graphics;                                             // Graphics instance for painting.
        private bool _closeSilently;                                            // Indicates whether to close the form silently.
        private bool _isYourTurn;                                               // Indicates whether it is your turn.
        private bool _gameStated;                                               // Indicates whether both the players has joined.

        #region Constructor

        public FrmTicTacToe()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void frmTicTacToe_Load(object sender, System.EventArgs e)
        {
            InitializeGraphics();

            this._ticTacToeChannel = CreateTicTacToeChannel();
            this._ticTacToeChannel.Open();

            this._ticTacToeChannel.Faulted += (object s, EventArgs a) =>
            {
                // Don't try to recover. Can't re-create as the server is maintaining sessions.
                this._ticTacToeChannel.Abort();
            };

            // Register Player.
            this._ticTacToeChannel.Register(this._playerChoice.Value);
        }

        private void frmTicTacToe_Paint(object sender, PaintEventArgs e)
        {
            PaintBorders();

            PaintSymbols();

            PaintWinVectors();
        }

        private void frmTicTacToe_Click(object sender, EventArgs e)
        {
            if (!this._gameStated)
            {
                MessageBox.Show("Other Player has not joined yet.", 
                                "TicTacToe", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                return;
            }

            if (this._isYourTurn)
            {
                MouseEventArgs eargs = e as MouseEventArgs;
                Point clickedPoint = new Point(eargs.X, eargs.Y);

                if (!HasValueInCell(clickedPoint))
                {
                    MoveMetadata moveMetadata = new MoveMetadata(this._playerType, GetCellNumber(clickedPoint));

                    Store(moveMetadata);
                    this._ticTacToeChannel.Move(moveMetadata);
                    this._isYourTurn = false;
                }
                else
                {
                    MessageBox.Show("Click on a blank region.",
                                    "TicTacToe",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Not your turn.",
                                "TicTacToe",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }
        
        private void frmTicTacToe_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._closeSilently)
            {
                DialogResult result = MessageBox.Show("Do you wish to quit the game?",
                                                        "TicTacToe", 
                                                        MessageBoxButtons.YesNo, 
                                                        MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    this._ticTacToeChannel.Abort(this._playerType);
                }
            }

            CloseProxy();
        }

        private void frmTicTacToe_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Since frmPlayerChoice is hidden.
            Application.Exit();
        }

        #endregion

        #region Members

        private ITicTacToeChannel CreateTicTacToeChannel()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            EndpointAddress endpointAddress = new EndpointAddress(ConfigurationManager.AppSettings["TicTacToeServer"]);
            WSDualHttpBinding wsDualHttpBinding = new WSDualHttpBinding();

            _duplexChannelFactory = new DuplexChannelFactory<ITicTacToeChannel>(instanceContext, wsDualHttpBinding, endpointAddress);

            return _duplexChannelFactory.CreateChannel();
        }

        private void CloseProxy()
        {
            if (this._ticTacToeChannel != null)
            {
                try
                {
                    // The best practice is NOT to check for CommunicationState.Closed.
                    this._ticTacToeChannel.Close();
                }
                catch
                {
                    try
                    {
                        this._ticTacToeChannel.Abort();
                    }
                    catch { }
                }
            }

            if (this._duplexChannelFactory != null)
            {
                try
                {
                    // The best practice is NOT to check for CommunicationState.Closed.
                    this._duplexChannelFactory.Close();
                }
                catch
                {
                    try
                    {
                        this._duplexChannelFactory.Abort();
                    }
                    catch { }
                }
            }
        }

        #region Graphics

        private void InitializeGraphics()
        {
            _graphics = this.CreateGraphics();
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        private void PaintWinVectors()
        {
            // Paint Win Vectors.
            Pen greenPen = new Pen(Color.Green, 4F);
            _winVectorsToPaint.ForEach((WinVector winVector) =>
            {
                switch (winVector)
                {
                    case WinVector.TOP:
                        _graphics.DrawLine(greenPen, new Point(0, 50), new Point(300, 50));
                        break;
                    case WinVector.CENTER:
                        _graphics.DrawLine(greenPen, new Point(0, 150), new Point(300, 150));
                        break;
                    case WinVector.BOTTOM:
                        _graphics.DrawLine(greenPen, new Point(0, 250), new Point(300, 250));
                        break;
                    case WinVector.LEFT:
                        _graphics.DrawLine(greenPen, new Point(50, 0), new Point(50, 300));
                        break;
                    case WinVector.MIDDLE:
                        _graphics.DrawLine(greenPen, new Point(150, 0), new Point(150, 300));
                        break;
                    case WinVector.RIGHT:
                        _graphics.DrawLine(greenPen, new Point(250, 0), new Point(250, 300));
                        break;
                    case WinVector.BACK_DIAGONAL:
                        _graphics.DrawLine(greenPen, new Point(0, 0), new Point(300, 300));
                        break;
                    case WinVector.FORWARD_DIAGONAL:
                        _graphics.DrawLine(greenPen, new Point(0, 300), new Point(300, 0));
                        break;
                    default:
                        break;
                }
            });
        }

        private void PaintSymbols()
        {
            // Horizontal
            for (byte c = 0; c <= 2; c++)
            {
                // Vertical
                for (byte r = 0; r <= 2; r++)
                {
                    MoveMetadata moveMetadata = this._moveMatrix[c, r];

                    if (moveMetadata != null)
                    {
                        switch (moveMetadata.CellNumber)
                        {
                            case CellNumber.First:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Second:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Third:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Forth:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Fifth:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Sixth:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Seventh:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Eighth:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                            case CellNumber.Ninth:
                                PaintSymbol(moveMetadata, GetPointToPaint(moveMetadata.CellNumber));
                                break;
                        }
                    }
                }
            }
        }

        private void PaintSymbol(MoveMetadata moveMetadata, Point coordinate)
        {
            _graphics.DrawString((moveMetadata.Player == PlayerType.Zero ? "0" : "X"),
                                    new Font("Arial", 30F),
                                    new SolidBrush((moveMetadata.Player == PlayerType.Zero ? Color.Blue : Color.Red)),
                                    coordinate.X,
                                    coordinate.Y);
        }

        private void PaintBorders()
        {
            Pen grayPen = new Pen(Color.Gray, 1F);
            _graphics.DrawLine(grayPen, new Point(100, 0), new Point(100, 300));
            _graphics.DrawLine(grayPen, new Point(200, 0), new Point(200, 300));
            _graphics.DrawLine(grayPen, new Point(0, 100), new Point(300, 100));
            _graphics.DrawLine(grayPen, new Point(0, 200), new Point(300, 200));
        }

        /// <summary>
        /// 1, 2, 3
        /// 4, 5, 6
        /// 7, 8, 9
        /// </summary>
        /// <param name="clickedPoint"></param>
        /// <returns></returns>
        private static Point GetPointToPaint(CellNumber cellNumber)
        {
            Point pointToPaint = Point.Empty;
            int number = 20;

            // Find the center of the cell.
            switch (cellNumber)
            {
                case CellNumber.First:
                    pointToPaint = new Point(50, 50);
                    break;
                case CellNumber.Second:
                    pointToPaint = new Point(150, 50);
                    break;
                case CellNumber.Third:
                    pointToPaint = new Point(250, 50);
                    break;
                case CellNumber.Forth:
                    pointToPaint = new Point(50, 150);
                    break;
                case CellNumber.Fifth:
                    pointToPaint = new Point(150, 150);
                    break;
                case CellNumber.Sixth:
                    pointToPaint = new Point(250, 150);
                    break;
                case CellNumber.Seventh:
                    pointToPaint = new Point(50, 250);
                    break;
                case CellNumber.Eighth:
                    pointToPaint = new Point(150, 250);
                    break;
                case CellNumber.Ninth:
                    pointToPaint = new Point(250, 250);
                    break;
            }

            // Subtract to position correctly.
            pointToPaint = Point.Subtract(pointToPaint, new Size(number, number));

            return pointToPaint;
        }

        #endregion
       
        private void CheckGameStatus(MoveMetadata moveMetadata)
        {
            WinVector winVector = WinVector.NONE;

            // Horizontal
            for (byte i = 0; i <= 2; i++)
            {
                if ((this._moveMatrix[i, 0] != null && this._moveMatrix[i, 0].Player == moveMetadata.Player) &&
                    (this._moveMatrix[i, 1] != null && this._moveMatrix[i, 1].Player == moveMetadata.Player) &&
                    (this._moveMatrix[i, 2] != null && this._moveMatrix[i, 2].Player == moveMetadata.Player))
                {
                    if (i == 0)
                        winVector = WinVector.TOP;
                    else if (i == 1)
                        winVector = WinVector.CENTER;
                    else if (i == 2)
                        winVector = WinVector.BOTTOM;

                    this._winVectorsToPaint.Add(winVector);

                    // There can't be move than one filled horizontal rows.
                    break;
                }
            }

            // Vertical
            for (byte i = 0; i <= 2; i++)
            {
                if ((this._moveMatrix[0, i] != null && this._moveMatrix[0, i].Player == moveMetadata.Player) &&
                    (this._moveMatrix[1, i] != null && this._moveMatrix[1, i].Player == moveMetadata.Player) &&
                    (this._moveMatrix[2, i] != null && this._moveMatrix[2, i].Player == moveMetadata.Player))
                {
                    if (i == 0)
                        winVector = WinVector.LEFT;
                    else if (i == 1)
                        winVector = WinVector.MIDDLE;
                    else if (i == 2)
                        winVector = WinVector.RIGHT;

                    this._winVectorsToPaint.Add(winVector);

                    // There can't be move than one filled vertical rows.
                    break;
                }
            }

            // Diagonal
            if ((this._moveMatrix[0, 0] != null && this._moveMatrix[0, 0].Player == moveMetadata.Player) &&
                (this._moveMatrix[1, 1] != null && this._moveMatrix[1, 1].Player == moveMetadata.Player) &&
                (this._moveMatrix[2, 2] != null && this._moveMatrix[2, 2].Player == moveMetadata.Player))
            {
                winVector = WinVector.BACK_DIAGONAL;
                this._winVectorsToPaint.Add(winVector);
            }

            if ((this._moveMatrix[0, 2] != null && this._moveMatrix[0, 2].Player == moveMetadata.Player) &&
                (this._moveMatrix[1, 1] != null && this._moveMatrix[1, 1].Player == moveMetadata.Player) &&
                (this._moveMatrix[2, 0] != null && this._moveMatrix[2, 0].Player == moveMetadata.Player))
            {
                winVector = WinVector.FORWARD_DIAGONAL;
                this._winVectorsToPaint.Add(winVector);
            }

            if (this._winVectorsToPaint.Count > 0)
            {
                Won(moveMetadata.Player);
                return;
            }

            // Draw
            if (this._moveMatrix[0, 0] != null &
                this._moveMatrix[0, 1] != null &
                this._moveMatrix[0, 2] != null &
                this._moveMatrix[1, 0] != null &
                this._moveMatrix[1, 1] != null &
                this._moveMatrix[1, 2] != null &
                this._moveMatrix[2, 0] != null &
                this._moveMatrix[2, 1] != null &
                this._moveMatrix[2, 2] != null &
                this._winVectorsToPaint.Count == 0)
            {
                Draw();
            }
        }

        private void Draw()
        {
            toolStripStatus.Text = "Game over.";

            this._ticTacToeChannel.Unregister(this._playerType);
            
            MessageBox.Show("Game is a draw.",
                            "TicTacToe",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            this._closeSilently = true;
            Close();
        }

        private void Won(PlayerType winner)
        {
            toolStripStatus.Text = "Game over.";

            this._ticTacToeChannel.Unregister(this._playerType);

            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "{0} has won the game.", winner),
                            "TicTacToe",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            
            this._closeSilently = true;            
            Close();
        }
        
        private bool HasValueInCell(Point clickedPoint)
        {
            MoveMetadata hasValueInCell = null;
            CellNumber cellNumber = GetCellNumber(clickedPoint);

            switch (cellNumber)
            {
                case CellNumber.First:
                    hasValueInCell = this._moveMatrix[0, 0];
                    break;
                case CellNumber.Second:
                    hasValueInCell = this._moveMatrix[0, 1];
                    break;
                case CellNumber.Third:
                    hasValueInCell = this._moveMatrix[0, 2];
                    break;
                case CellNumber.Forth:
                    hasValueInCell = this._moveMatrix[1, 0];
                    break;
                case CellNumber.Fifth:
                    hasValueInCell = this._moveMatrix[1, 1];
                    break;
                case CellNumber.Sixth:
                    hasValueInCell = this._moveMatrix[1, 2];
                    break;
                case CellNumber.Seventh:
                    hasValueInCell = this._moveMatrix[2, 0];
                    break;
                case CellNumber.Eighth:
                    hasValueInCell = this._moveMatrix[2, 1];
                    break;
                case CellNumber.Ninth:
                    hasValueInCell = this._moveMatrix[2, 2];
                    break;
                case CellNumber.None:
                    break;
            }

            return (hasValueInCell != null);
        }

        private static CellNumber GetCellNumber(Point clickedPoint)
        {
            CellNumber cellNumber = CellNumber.None;

            if (clickedPoint.Between(new Point(0, 0), new Point(100, 100)))
            {
                cellNumber = CellNumber.First;
            }

            if (clickedPoint.Between(new Point(100, 0), new Point(200, 100)))
            {
                cellNumber = CellNumber.Second;
            }

            if (clickedPoint.Between(new Point(200, 0), new Point(300, 100)))
            {
                cellNumber = CellNumber.Third;
            }

            if (clickedPoint.Between(new Point(0, 100), new Point(100, 200)))
            {
                cellNumber = CellNumber.Forth;
            }

            if (clickedPoint.Between(new Point(100, 100), new Point(200, 200)))
            {
                cellNumber = CellNumber.Fifth;
            }

            if (clickedPoint.Between(new Point(200, 100), new Point(300, 200)))
            {
                cellNumber = CellNumber.Sixth;
            }

            if (clickedPoint.Between(new Point(0, 200), new Point(100, 300)))
            {
                cellNumber = CellNumber.Seventh;
            }

            if (clickedPoint.Between(new Point(100, 200), new Point(200, 300)))
            {
                cellNumber = CellNumber.Eighth;
            }

            if (clickedPoint.Between(new Point(200, 200), new Point(300, 300)))
            {
                cellNumber = CellNumber.Ninth;
            }
             
            return cellNumber;
        }

        private void Store(MoveMetadata moveMetadata)
        {
            switch (moveMetadata.CellNumber)
            {
                case CellNumber.First:
                    this._moveMatrix[0, 0] = moveMetadata;
                    break;
                case CellNumber.Second:
                    this._moveMatrix[0, 1] = moveMetadata;
                    break;
                case CellNumber.Third:
                    this._moveMatrix[0, 2] = moveMetadata;
                    break;
                case CellNumber.Forth:
                    this._moveMatrix[1, 0] = moveMetadata;
                    break;
                case CellNumber.Fifth:
                    this._moveMatrix[1, 1] = moveMetadata;
                    break;
                case CellNumber.Sixth:
                    this._moveMatrix[1, 2] = moveMetadata;
                    break;
                case CellNumber.Seventh:
                    this._moveMatrix[2, 0] = moveMetadata;
                    break;
                case CellNumber.Eighth:
                    this._moveMatrix[2, 1] = moveMetadata;
                    break;
                case CellNumber.Ninth:
                    this._moveMatrix[2, 2] = moveMetadata;
                    break;
            }
        }

        #endregion

        #region ITicTacToeCallback Members

        public void Registered(bool earlyBird, PlayerType allottedPlayer)
        {
            if (earlyBird)
            {
                this._isYourTurn = true;
                toolStripStatus.Text = "Other Player has not joined yet.";
            }

            if (allottedPlayer != this._playerChoice.Value)
            {
                MessageBox.Show("Your choice was not available.", "TicTacToe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this._playerType = allottedPlayer;

            this.Text = string.Format(CultureInfo.InvariantCulture, "TicTacToe - {0}", this._playerType.ToString());
        }

        public void Moved(MoveMetadata moveMetadata, bool isYourTurn)
        {
            this.Store(moveMetadata);
            Invalidate();

            this.CheckGameStatus(moveMetadata);

            this._isYourTurn = isYourTurn;
            toolStripStatus.Text = isYourTurn ? "Your turn" : "Not your turn";
        }

        public void Aborted(PlayerType player)
        {
            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "{0} has end the game.", player.ToString()), 
                            "TicTacToe", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);

            this._closeSilently = true;
            Close();
        }

        public void GameStarted()
        {
            this._gameStated = true;

            if (this._isYourTurn)
            {
                toolStripStatus.Text = "Your turn";
            }
            else
            {
                toolStripStatus.Text = "Not your turn";
            }
        }

        public void RegistrationFailed()
        {
            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Sorry, reached the maximum player limit."),
                           "TicTacToe",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Exclamation);

            _closeSilently = true;
            Close();
        }

        #endregion

        #region Property

        public PlayerType? PlayerChoice
        {
            set { this._playerChoice = value; }
        }

        #endregion
    }

    internal static class Extensions
    {
        public static bool Between(this Point clickedPoint, Point first, Point second)
        {
            return ((clickedPoint.X >= first.X && clickedPoint.X <= second.X) &&
                (clickedPoint.Y >= first.Y && clickedPoint.Y <= second.Y));
        }
    }
}
