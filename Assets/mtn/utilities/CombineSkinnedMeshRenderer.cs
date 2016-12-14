using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using HierarchyDict = System.Collections.Generic.Dictionary<string, UnityEngine.Transform>;
// This script takes an input GameObject (Main Character) and tries to match the main character bones found iwith
// the bones in the Game Object to which this script is attached

// There are therefore two inputs and the output is the current game object modified with the bone array in the main character
// Input:   m_attached, (this) current.game.object
// Output:  current.game.object modified bone array from m_attached

public class CombineSkinnedMeshRenderer : MonoBehaviour
{
	// This is the main character object
	public GameObject m_attached;

	// Use this for initialization
	void Start()
	{
		SkinnedMeshRenderer targetRenderer = m_attached.GetComponent<SkinnedMeshRenderer>();
		Dictionary<string, Transform> mainCharacterBoneMap = new Dictionary<string, Transform>();
		// get bonemap of main character object
		foreach (Transform bone in targetRenderer.bones)
		{
			mainCharacterBoneMap[bone.name] = bone;
		}

		// get current Game object's skinned mesh renderer i.e. the object to which this script
		// is currently attached 
		SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
		Transform[] boneArray = thisRenderer.bones;
		// check that all the bone names of the current game object exist in the main character 
		// by retrieving from the main character's bonemap.
		// retrieve each bone by index order in the current object and create an output boneArray for each bone by copying from the mainCharacterBoneMap.
		for (int idx = 0; idx < boneArray.Length; ++idx)
		{
			string boneName = boneArray[idx].name;
			if (false == mainCharacterBoneMap.TryGetValue(boneName, out boneArray[idx]))
			{
				// Error if we cannot find the bonename in the main characterBoneMap
				Debug.LogError("failed to get bone: " + boneName);
				Debug.Break();
			}
		}
		// make the  bones in the current game object
		// equal to the output bone array taken from the main character
		thisRenderer.bones = boneArray; //take effect
	}
}
