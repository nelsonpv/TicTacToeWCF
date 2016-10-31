namespace Nelson.TicTacToe.Client
{
    using System.ServiceModel;
    using Nelson.TicTacToe.Common;

    internal interface ITicTacToeChannel : ITicTacToe, IClientChannel
    {
    }
}
