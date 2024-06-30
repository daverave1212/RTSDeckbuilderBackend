
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

Command:
	name
	data: Dictionary<string, string>[]					this generic data type can hold pretty much anything


class LiveGame:
	Player[] players;

ServerPlayer:
	StringServerClient serverClient
	LiveGame liveGame
	cardsInHand

	PopCardFromHand(): ServerCard

	Send:
		SendCommand(CommandDto)

	Receive:
		OnReceiveCommand(CommandDto)
	
	Commands:
		JoinLiveGame(liveGameId)
		PlayCard(cardId, tile):
			card = this.PopCardFromHand(cardId)
			player.SubtractCardCosts(card)
			card.ServerOnPlayed(player, tile)		this might do certain player.Send... or player.liveGame.SendToAll...

	States:
		GetSetMapStateCommand()
		GetSetShopStateCommand()
		GetSetHandStateCommand()
		GetSetResourcesStateCommand()


GameServer:
	
	LiveGames{}

	OnPlayerConnectToServer:
		remember player
		all further messages are interpreted by the ServerPlayer object

		OnMessageReceived(str):
			Decode the str into a CommandDto and pass it









CardTemplate:
	int goldCost
	int woodCost
	int productionCost

	ServerOnPlayed(player, tile)








ClientPlayer:
	
	SetResourcesState(...)
	SetMapState(...)
	SetShopState(...)
	SetHandState(...)

	Send:
		ConnectToServer()
		SendJoinLiveGame(liveGameId)		
		SendPlayCard(cardId, tileId)

	Receive:
		OnCommandReceived(command)

	Inputs:
		PlayCard(cardId): if resources are ok, card disappears from hand; SendPlayCard(cardId)