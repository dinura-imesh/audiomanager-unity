using System;
using System.Collections;
using System.Collections.Generic;
using DefynModules.EventCore.Managers;
using DefynModules.EventCore.Monobehaviors;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : EventHandledMonoBehavior
{

	#region VARIABLES

	public static AudioManager instance;

	public AudioFile[] audioFiles;

	private float timeToReset;

	private bool timerIsSet = false;

	private EnumAudioId tmpName;

	private float tmpVol;

	private bool isLowered = false;

	private bool fadeOut = false;

	private bool fadeIn = false;

	private string fadeInUsedString;

	private string fadeOutUsedString;
	#endregion
	
	
	// Use this for initialization
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		foreach (var s in audioFiles)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.audioClip;
			s.source.volume = s.volume;
			s.source.loop = s.isLooping;
			if (s.playOnAwake)
			{
				s.source.Play();
			}
		}
	}


	#region METHODS

	public static void PlayMusic(EnumAudioId audioId)
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
		if (s == null)
		{
			Debug.LogError("Sound name" + audioId + "not found!");
			return;
		}
		else
		{
			if (s.source.isPlaying)
			{
				Debug.LogWarning($"Requested Audio File {audioId} is already Playing");
			}
			else
			{
				s.source.Play();
			}
		}
	}

	
	
	public static void StopMusic(EnumAudioId audioId)
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
		if (s == null)
		{
			Debug.LogError("Sound name" + audioId + "not found!");
			return;
		}
		else
		{
			s.source.Stop();
		}
	}

	public static void PauseMusic(EnumAudioId audioId)
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
		if (s == null)
		{
			Debug.LogError("Sound name" + audioId + "not found!");
			return;
		}
		else
		{
			s.source.Pause();
		}
	}

	public static void UnPauseMusic(EnumAudioId audioId)
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
		if (s == null)
		{
			Debug.LogError("Sound name" + audioId + "not found!");
			return;
		}
		else
		{
			s.source.UnPause();
		}
	}

	public static void LowerVolume(EnumAudioId audioId, float _duration)
	{
		if (instance.isLowered == false)
		{
			AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioId + "not found!");
				return;
			}
			else
			{
				instance.tmpName = audioId;
				instance.tmpVol = s.volume;
				instance.timeToReset = Time.time + _duration;
				instance.timerIsSet = true;
				s.source.volume = s.source.volume / 3;
			}

			instance.isLowered = true;
		}
	}

	public static void FadeOut(EnumAudioId audioId, float duration)
	{
		instance.StartCoroutine(instance.IFadeOut(audioId, duration));
	}

	public static void FadeIn(EnumAudioId audioId, float targetVolume, float duration)
	{
		instance.StartCoroutine(instance.IFadeIn(audioId, targetVolume, duration));
	}



        //not for use
    private IEnumerator IFadeOut(EnumAudioId audioId, float duration)
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
		if (s == null)
		{
			Debug.LogError("Sound name" + name + "not found!");
			yield return null;
		}
		else
		{
			if (fadeOut == false)
			{
				fadeOut = true;
				float startVol = s.source.volume;
				fadeOutUsedString = name;
				while (s.source.volume > 0)
				{
					s.source.volume -= startVol * Time.deltaTime / duration;
					yield return null;
				}

				s.source.Stop();
				yield return new WaitForSeconds(duration);
				fadeOut = false;
			}

			else
			{
				Debug.Log("Could not handle two fade outs at once : " + name + " , " + fadeOutUsedString +"! Stopped the music " + name);
				StopMusic(audioId);
			}
		}
	}

	public IEnumerator IFadeIn(EnumAudioId audioId,float targetVolume, float duration)
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == audioId);
		if (s == null)
		{
			Debug.LogError("Sound name" + name + "not found!");
			yield return null;
		}
		else
		{
			if (fadeIn == false)
			{
				fadeIn = true;
				instance.fadeInUsedString = name;
				s.source.volume = 0f;
				s.source.Play();
				while (s.source.volume < targetVolume)
				{
					s.source.volume += Time.deltaTime / duration;
					yield return null;
				}
				
				yield return new WaitForSeconds(duration);
				fadeIn = false;
			}
			else
			{
				Debug.Log("Could not handle two fade ins at once: " + name + " , " + fadeInUsedString+ "! Played the music " + name);
				StopMusic(audioId);
				PlayMusic(audioId);
			}
		}
	}

	void ResetVol()
	{
		AudioFile s = Array.Find(instance.audioFiles, AudioFile => AudioFile.audioID == tmpName);
		s.source.volume = tmpVol;
		isLowered = false;
	}

	private void Update()
	{
		if (Time.time >= timeToReset && timerIsSet)
		{
			ResetVol();
			timerIsSet = false;
		}
	}

	#endregion

	public override void SubscribeEvents()
	{
		EventManager.AddListener<GameStartEvent>(HandleOnGameStartEvent);
		EventManager.AddListener<PlaneDestroyEvent>(HandleOnPlaneDestroyEvent);
		EventManager.AddListener<MainMenuLoadEvent>(HandleOnMainMenuLoadEvent);
	}



	public override void UnsubscribeEvents()
	{
		
	}

	private void HandleOnGameStartEvent(GameStartEvent e)
	{
		if (e.launch)
		{
		}
	}

	private void HandleOnPlaneDestroyEvent(PlaneDestroyEvent e)
	{
		if (e.launch)
		{
			
		}
			
	}
	private void HandleOnMainMenuLoadEvent(MainMenuLoadEvent e)
	{
		if (e.launch)
		{
			
		}
	}
}
