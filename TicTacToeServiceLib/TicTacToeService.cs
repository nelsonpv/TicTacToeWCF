using System.Collections.Generic;
using System.ServiceModel;
using Nelson.TicTacToe.Common;
using System;
using System.Linq;

namespace Nelson.TicTacToe.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TicTacToeService : ITicTacToe
    {
        private Dictionary<PlayerType, ITicTacToeCallback> _dictionary =
            new Dictionary<PlayerType, ITicTacToeCallback>();

        #region ITicTacToe Members

        public void Register(PlayerType requestedPlayer)
        {
            ITicTacToeCallback client = OperationContext.Current.GetCallbackChannel<ITicTacToeCallback>();

            // Reached Max Limit.
            if (_dictionary.Count == 2)
            {                
                client.RegistrationFailed();
                return;
            }

            // Check Availability.
            PlayerType allottedPlayer = CheckAvailability(requestedPlayer);
            _dictionary.Add(allottedPlayer, client);
            client.Registered((_dictionary.Count == 1), allottedPlayer);

            if (_dictionary.Count == 2)
            {
                foreach (KeyValuePair<PlayerType, ITicTacToeCallback> pair in _dictionary) 
                {
                    if (((ICommunicationObject)pair.Value).State == CommunicationState.Opened)
                    {
                        pair.Value.GameStarted();
                    }
                }
            }
        }

        public void Move(MoveMetadata moveMetadata)
        {
            ITicTacToeCallback sender = OperationContext.Current.GetCallbackChannel<ITicTacToeCallback>();
            
            foreach (KeyValuePair<PlayerType, ITicTacToeCallback> pair in _dictionary)
            {
                if (((ICommunicationObject)pair.Value).State == CommunicationState.Opened)
                {                    
                    pair.Value.Moved(moveMetadata, !pair.Value.Equals(sender));
                }
            }
        }

        public void Abort(PlayerType player)
        {
            ITicTacToeCallback sender = OperationContext.Current.GetCallbackChannel<ITicTacToeCallback>();

            foreach (KeyValuePair<PlayerType, ITicTacToeCallback> pair in _dictionary)
            {
                if (((ICommunicationObject)pair.Value).State == CommunicationState.Opened)
                {
                    if (!pair.Value.Equals(sender))
                    {
                        pair.Value.Aborted(player);
                        break;
                    }
                }
            }
            
            _dictionary.Clear();
            /*
            ITicTacToeCallback sender = OperationContext.Current.GetCallbackChannel<ITicTacToeCallback>();

            Func<KeyValuePair<PlayerType, ITicTacToeCallback>, bool> criteria = pair =>
            {
                bool returnValue = false;
                if (((ICommunicationObject)pair.Value).State == CommunicationState.Opened)
                {
                    returnValue = !pair.Value.Equals(sender);
                }
                return returnValue;
            };

            KeyValuePair<PlayerType, ITicTacToeCallback> result = _dictionary.FirstOrDefault(criteria);

            if (result.Value != null)
                result.Value.Aborted(player);

            _dictionary.Clear();            
            */
        }

        public void Unregister(PlayerType player)
        {
            _dictionary.Remove(player);
        }

        #endregion

        #region Members

        private PlayerType CheckAvailability(PlayerType requestedPlayer)
        {
            PlayerType allottedPlayer;

            if (_dictionary.ContainsKey(requestedPlayer))
            {
                // If the player type is already chosen then
                // fallback to the other one.
                if (requestedPlayer == PlayerType.Cross)
                {
                    allottedPlayer = PlayerType.Zero;
                }
                else
                {
                    allottedPlayer = PlayerType.Cross;
                }
            }
            else
            {
                allottedPlayer = requestedPlayer;
            }

            return allottedPlayer;
        }

        #endregion
    }
}
