using System.Net.Security;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Nelson.TicTacToe.Common
{
    /// <summary>
    /// Service Contract for TicTacToe. Client -> Server
    /// </summary>
    [ServiceContract(Namespace = Constants.ServiceContractNamespace, 
        CallbackContract = typeof(ITicTacToeCallback), 
        SessionMode = SessionMode.Required,
        ProtectionLevel = ProtectionLevel.None)]
    public interface ITicTacToe
    {
        /// <summary>
        /// Registers the player with the server players list.
        /// </summary>
        /// <param name="requestedPlayer">Requested player type. Allotment is based on availability.</param>
        [OperationContract(IsOneWay = true)]
        void Register(PlayerType requestedPlayer);
        
        /// <summary>
        /// Broadcasts the move metadata to both the clients.
        /// </summary>
        /// <param name="moveMetadata">Move metadata.</param>
        [OperationContract(IsOneWay = true)]
        void Move(MoveMetadata moveMetadata);
        
        /// <summary>
        /// Informs the other player that the caller has abort.
        /// </summary>
        /// <param name="player">Player type.</param>
        [OperationContract(IsOneWay = true)]
        void Abort(PlayerType player);

        /// <summary>
        /// Unregisters the player from the server players list.
        /// </summary>
        /// <param name="player">Player type.</param>
        [OperationContract(IsOneWay = true)]
        void Unregister(PlayerType player);
    }
    
    /// <summary>
    /// Callback interface for TicTacToe. Server -> Client
    /// </summary>
    public interface ITicTacToeCallback
    {
        /// <summary>
        /// Registered event is called upon successful registration of a player.
        /// </summary>
        /// <param name="earlyBird">True value for the first player and False for the second player.</param>
        /// <param name="allottedPlayer">Alloted player based on availability.</param>
        [OperationContract(IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void Registered(bool earlyBird, PlayerType allottedPlayer);

        /// <summary>
        /// GameStarted event is called once both the players are connected.
        /// </summary>
        [OperationContract(IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void GameStarted();

        /// <summary>
        /// Moved event is called for every move.
        /// </summary>
        /// <param name="moveMetadata">Move metadata.</param>
        /// <param name="isYourTurn">True value if it's your turn; otherwise False.</param>
        [OperationContract(IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void Moved(MoveMetadata moveMetadata, bool isYourTurn);

        /// <summary>
        /// Aborted event is called to inform the other player that your have aborted.
        /// </summary>
        /// <param name="player">Aborted player.</param>
        [OperationContract(IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void Aborted(PlayerType player);

        /// <summary>
        /// RegistrationFailed event is generated once the max player limit is reached.
        /// Max limit is two. Cross and Zero.
        /// </summary>
        [OperationContract(IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void RegistrationFailed();
    }

    /// <summary>
    /// Holds the move metadata. This class is immutable. http://en.wikipedia.org/wiki/Immutable_object
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class MoveMetadata
    {
        public MoveMetadata(PlayerType playerType, CellNumber cellNumber)
        {            
            this.Player = playerType;
            this.CellNumber = cellNumber;
        }

        [DataMember]
        public PlayerType Player { get; private set; }

        [DataMember]
        public CellNumber CellNumber { get; private set; }
    }

    /// <summary>
    /// Holds the player type.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public enum PlayerType
    {
        [EnumMember]
        Zero = 0,
        [EnumMember]
        Cross = 1
    }
    
    /// <summary>
    /// Holds the cell number.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public enum CellNumber
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        First = 1,
        [EnumMember]
        Second = 2,
        [EnumMember]
        Third = 3,
        [EnumMember]
        Forth = 4,
        [EnumMember]
        Fifth = 5,
        [EnumMember]
        Sixth = 6,
        [EnumMember]
        Seventh = 7,
        [EnumMember]
        Eighth = 8,
        [EnumMember]
        Ninth = 9
    }
}
