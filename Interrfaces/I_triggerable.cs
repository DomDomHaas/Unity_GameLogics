using UnityEngine;
using System.Collections;
using System;

public interface I_triggerable
{
	
		void OnStart (object smashTrigger, EventArgs e);
	
		void OnStop (object smashTrigger, EventArgs e);

		void OnSuccess (object smashTrigger, EventArgs e);
	
		void OnFail (object smashTrigger, EventArgs e);
	
}
