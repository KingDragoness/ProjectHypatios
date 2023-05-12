using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


namespace TestingPurposes
{

	public class OdinProcessorTest : MonoBehaviour
	{
		public MyProcessedClass Processed;

	}

	[System.Serializable]
	public class MyProcessedClass
	{
		public ScaleMode Mode;
		public float Size;
	}
}