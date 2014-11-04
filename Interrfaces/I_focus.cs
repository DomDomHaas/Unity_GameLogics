using UnityEngine;
using System.Collections;

public interface I_focus {

	void onFocusEnter();
	
	void onClick();
	
	void onFocusExit();
	
	void setVisible(bool visible);
	
}
