/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service
 
License Usage
 
Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2025 Audiokinetic Inc.
*******************************************************************************/

#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if !UNITY_2019_1_OR_NEWER
#define AK_ENABLE_TIMELINE
#endif
#if AK_ENABLE_TIMELINE
//--------------------------------------------------------------------------------------------
// The representation of the Timeline Clip
//--------------------------------------------------------------------------------------------

[System.Serializable]
[System.Obsolete(AkUnitySoundEngine.Deprecation_2019_2_0)]
public class AkRTPCPlayable : UnityEngine.Playables.PlayableAsset, UnityEngine.Timeline.ITimelineClipAsset
{
	public bool overrideTrackObject = false;
	public UnityEngine.ExposedReference<UnityEngine.GameObject> RTPCObject;

	public bool setRTPCGlobally = false;
	public AkRTPCPlayableBehaviour template = new AkRTPCPlayableBehaviour();

	public AK.Wwise.RTPC Parameter { get; set; }

	public UnityEngine.Timeline.TimelineClip OwningClip { get; set; }

	UnityEngine.Timeline.ClipCaps UnityEngine.Timeline.ITimelineClipAsset.clipCaps
	{
		get { return UnityEngine.Timeline.ClipCaps.Looping & UnityEngine.Timeline.ClipCaps.Extrapolation & UnityEngine.Timeline.ClipCaps.Blending; }
	}

	public override UnityEngine.Playables.Playable CreatePlayable(UnityEngine.Playables.PlayableGraph graph, UnityEngine.GameObject go)
	{
		var playable = UnityEngine.Playables.ScriptPlayable<AkRTPCPlayableBehaviour>.Create(graph, template);
		var b = playable.GetBehaviour();
		b.overrideTrackObject = overrideTrackObject;
		b.setRTPCGlobally = setRTPCGlobally;
		b.rtpcObject = overrideTrackObject ? RTPCObject.Resolve(graph.GetResolver()) : go;
		b.parameter = Parameter;
		return playable;
	}
}


//--------------------------------------------------------------------------------------------
// The behaviour template.
//--------------------------------------------------------------------------------------------

[System.Serializable]
[System.Obsolete(AkUnitySoundEngine.Deprecation_2019_2_0)]
public class AkRTPCPlayableBehaviour : UnityEngine.Playables.PlayableBehaviour
{
	[UnityEngine.SerializeField]
	private float RTPCValue = 0.0f;

	public bool setRTPCGlobally { set; private get; }

	public bool overrideTrackObject { set; private get; }

	public UnityEngine.GameObject rtpcObject { set; private get; }

	public AK.Wwise.RTPC parameter { set; private get; }

	public override void ProcessFrame(UnityEngine.Playables.Playable playable, UnityEngine.Playables.FrameData info,
		object playerData)
	{
		if (parameter != null)
		{
			// If we are overriding the track object, the rtpcObject will have been resolved in AkRTPCPlayable::CreatePlayable().
			if (!overrideTrackObject)
			{
				// At this point, rtpcObject will have been set to the timeline owner object in AkRTPCPlayable::CreatePlayable().
				// If the track object is null, we keep using the timeline owner object. Otherwise, we swap it for the track object.
				var obj = playerData as UnityEngine.GameObject;
				if (obj != null)
					rtpcObject = obj;
			}

			if (setRTPCGlobally || rtpcObject == null)
				parameter.GetGlobalValue(RTPCValue);
			else
				parameter.SetValue(rtpcObject, RTPCValue);
		}

		base.ProcessFrame(playable, info, playerData);
	}
}
#endif // AK_ENABLE_TIMELINE
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.