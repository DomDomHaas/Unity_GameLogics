using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

	[Serializable]
	public class EffectChain: MonoBehaviour
	{

	  public float TotalTime = 0;

    public List<ParticleSystem> EffectPartSystems;

		public List<float>
			EffectEmissionDelays;

		public float LongestEffectDelay = 0;
		public float LongestLifeTime = 0;

		public List<string> SoundNames;
		
		public List<float>
			SoundDelays;

	  public bool StartOnAwake = false;


		protected virtual void Awake ()
		{
		  PauseAll();
		}

		protected virtual void Start ()
		{
		  if (StartOnAwake){
		    StartEffectChain();
		  }
		}

	  public void StartEffectChain()
	  {
      StartDelayedParticleEmissions();
      StartDelayedSoundFx();
    }

    protected void StartDelayedParticleEmissions ()
		{
			//Debug.Log ("partsys " + this.EffectPartSystems.Count);

			int i = 0;
			foreach (ParticleSystem partSys in this.EffectPartSystems) {

					partSys.Pause ();
					StartCoroutine (DelayedParticleEmission (this.EffectEmissionDelays[i], partSys));

				i++;
			}
		}

	  protected void PauseAll()
	  {
      int i = 0;
      foreach (ParticleSystem partSys in this.EffectPartSystems)
      {
        float delay = this.EffectEmissionDelays[i];

        if (delay > LongestEffectDelay) {
          LongestEffectDelay = delay;
        }

        if (partSys.startLifetime > LongestLifeTime) {
          LongestLifeTime = partSys.startLifetime;
        }

        partSys.Pause();

        i++;
      }
    }

    protected virtual IEnumerator DelayedParticleEmission (float delay, ParticleSystem sys)
		{
			//Debug.Log ("DelayedParticleEmission " + delay + " " + sys.name);
			yield return new WaitForSeconds (this.TotalTime + delay);
			sys.Play ();
		}



		protected void StartDelayedSoundFx ()
		{
			int i = 0;
			foreach (string soundName in this.SoundNames) {
				StartCoroutine (DelayedSoundFx (this.SoundDelays[i], soundName));
				i++;
			}
		}
		

		protected IEnumerator DelayedSoundFx (float delay, string soundName)
		{
			yield return new WaitForSeconds (this.TotalTime + delay);

			string[] splits = soundName.Split (',');

			if (splits.Length > 1) {
				MasterAudio.PlaySound3DAtTransformAndForget (splits [0], this.transform, 1, default(float?), 0, splits [1]);
			} else {
				MasterAudio.PlaySound3DAtTransformAndForget (soundName, this.transform);
			}

			//Debug.Log ("DelayedParticleEmission " + delay + " " + sys.name);
		}
    

	}