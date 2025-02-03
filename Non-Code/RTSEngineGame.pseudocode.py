

class LiveGame:

    List<Unit> units = new List<Unit>()

    void Tick(float deltaTimeMS):
        foreach(def unit in units) {
            unit.Tick(deltaTimeMS)
        }

'''
How to implement the algorithm:
1. Movement to target position
2. Add cache
3. Add recalculations if nodes are blocked
    - If Destination node is blocked, stop
4. Add distance calculations optimizations
5. Add enemy distractions (keeping original target position)
6. Add optimization for checking forward if the path is blocked (e.g. IsAnyBlockedLazy)
7. Error handling for GetNodesBetweenPositions (e.g. no path available)

5. Add closest base
'''




partial class Unit -- Base

    Unit(onPosition):
        SetPosition(onPosition)


    void Tick(fload deltaTimeMS):
        
        def actionToMake: MOVE | ATTACK | NONE

        def enemyNearby = GetEnemyNearby()
        if enemyNearby != None:
            if IsInAttackRange(enemyNearby):
                actionToMake = ATTACK
            else:
                OrderMove(enemyNearby.GetPosition())
                actionToMake = MOVE
            

        if actionToMake == MOVE:
            TickMovement(deltaTimeMS)
        if actionToMake == ATTACK
            TickAttack(deltaTimeMS)


partial class Unit -- Movement:

    def isMoving = False
    def speedPerSecond = 100
    def destinationPosition








    def upcomingPosition
    def currentNodesToDestination    
    void OrderMove(toPosition):
        destinationPosition = toPosition
        isMoving = True
        upcomingPosition = GetPosition()
        currentNodesToDestination = GetNodesBetweenPositions(GetPosition(), toPosition)

        def isNoPathAvailable = currentNodesToDestination == None
        if isNoPathAvailable:
            isMoving = False


    void TickMovement(float deltaTimeMS):
        if not isMoving:
            return

        SetPosition(upcomingPosition)
        
        if GetPosition() == destinationPosition:
            isMoving = False
            return

        # upcomingPosition is 100% never changing (to send to client)
        upcomingPosition = LerpWithNodesWithCachedPath(
            fromPos=GetPosition(),
            toPos=destinationPosition,
            distanceTraveled=speedPerSecond * deltaTimeMS / 1000,
            getNodesPath=() => currentNodesToDestination,
            setNodesPath=(nodes) => currentNodesToDestination = nodes
        )

        def canNotMove = upcomingPosition == GetPosition()

        if canNotMove:
            isMoving = False

        # Broadcast all new unit states after Tick








partial class Unit -- Static Algorithms

    # Does not include startNode and endNode
    static ?Node[] GetNodesBetweenPositions(fromPos, toPos):
        def myNode = GetNode(GetPosition())
        def endNode = GetNode(destinationPosition)
        
        if endNode.IsBlocked():
            return None

        def nodesToDestination = AStar(myNode, endNode)
        if nodesToDestination == None:                  # No path available
            return None

        nodesToDestination.Remove(myNode, endNode)      # Don't include start node and end node, for more smooth movement
        return nodesToDestination


    # getNodesPath should return the nodes path fromPos toPos
    static Point2D LerpWithNodesWithCachedPath(fromPos, toPos, distanceTraveled, getNodesPath: void -> ?Node[], setNodesPath):
        
        def nodesPath = getNodesPath()

        if nodesPath == None:
            return fromPos

        # Recalculate if path is blocked
        if nodesPath.Any(node => node.IsBlocked()):
            def newNodesPath = GetNodesBetweenPositions(fromPos, toPos)
            if newNodesPath == None:
                return fromPos
            setNodesPath(newNodesPath)
            nodesPath = getNodesPath()

        if nodesPath.Length == 0:
            if distanceTraveled.IsLowerThanDistanceBetweenPoints(fromPos, toPos)    # Optimization
                return Lerp(...)
            else:
                return toPos

        def nNodesTraveled = 0
        def distanceLeftToTravel = distanceTraveled
        def currentPos = fromPos
        for (node, i) in nodesPath:
            def distanceToNextNode = i == 0? currentPos.DistanceTo(node.GetPosition()): prevNode.DistanceTo(node) # Optimization
            if distanceLeftToTravel >= distanceToNextNode:
                nNodesTraveled++
                distanceLeftToTravel -= distanceToNextNode
                currentPos = node.GetPosition()
            else:
                break
        
        def newNodesPath = nodesPath.Slice(nNodesTraveled)
        setNodesPath(newNodesPath)

        if (newNodesPath.Length == 0):
            def lastNodePos = nodesPath.Last.GetPosition()
            if distanceLeftToTravel.IsEGreaterThanDistanceBetweenPoints(lastNodePos, toPos):    # Optimization
                return toPos
            return Lerp(nodesPath.Last, toPos, distanceLeftToTravel)

        return Lerp(nodesPath[nNodesTraveled-1], nodesPath[nNodesTraveled], distanceLeftToTravel)


    # Given a start pos and end pos
    static Point2D LerpWithNodes(fromPos, toPos, distanceTraveled):
        return LerpWithNodesWithCachedPath(
            fromPos=fromPos,
            toPos=toPos, 
            distanceTraveled=distanceTraveled,
            getNodesPath=() => GetNodesBetweenPositions(fromPos, toPos),
            setNodesPath=() => {}
        )