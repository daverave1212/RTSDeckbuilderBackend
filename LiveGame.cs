using System;

public class LiveGame {

	public static int TICK_RATE = 100;

	string id;
	List<UnitRep> units = new List<UnitRep>();
	GameMapRep map;

	public LiveGame(string _id, GameMapRep gmr=null) {
		id = _id;
		map = gmr;

		Utils.DoEveryAsync(100, Update);
	}

	void Update() {
		foreach (var unit in units) {
			unit.Update();
		}
	}


	public void ReceiveMessage(string message) {
	
	}

	public void SendMessage(string message) { }

	public void AddUnit(UnitRep unit) {
		units.Add(unit);
	}

}
