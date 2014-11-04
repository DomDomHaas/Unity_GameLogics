using UnityEngine;
using System.Collections;

public interface I_hitable  {

	void hitByBall(BallControl ball);
	
	void hitByPlayer(PlayerControl otherPlayer);
	
}
