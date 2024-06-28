
# Architecture
The server has instances of:
	Server
	LiveGame
	ServerPlayer (interpretor)

The client has instances of:
	ClientPlayer (interpretor)



Program:
	new StringServer
	new LiveGame

class LiveGame:
	Player[] players;



ClientPlayer:
	
	SetResourcesState(json)
	SetMapState(json)
	SetShopState(json)
	SetHandState(json)

	Send:
		ConnectToServer()
		SendJoinLiveGame(liveGameId)		
		SendPlayCard(cardId, tileId)

	Receive:
		OnCommandsReceived(commands)

	Inputs:
		PlayCard(cardId): if resources are ok, card disappears from hand; SendPlayCard(cardId)



ServerPlayer:
	LiveGame liveGame
	cardsInHand

	PopCardFromHand(): ServerCard

	Send:
		SendCommands(...command: JSON)
		SendSetMapStateCommand()
		SendSetShopStateCommand()
		SendSetHandStateCommand()
		SendSetResourcesStateCommand()

	Receive:
		OnReceiveCommands(commands)
		JoinLiveGame(liveGameId)
		PlayCard(cardId, tile):
			card = this.PopCardFromHand(cardId)
			player.SubtractCardCosts(card)
			card.ServerOnPlayed(player, tile)		this might do certain player.Send... or player.liveGame.SendToAll...



Server:

	SendCommandsToAll(...command: JSON)

	OnPlayerConnectToServer:
		remember player
		all further messages are interpreted by the ServerPlayer object
