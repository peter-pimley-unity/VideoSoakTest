using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Monkey : MonoBehaviour
{

	[SerializeField]
	Loader m_loader;


	[SerializeField]
	private float m_minSleep;

	[SerializeField]
	private float m_maxSleep;

	// Start is called before the first frame update
	IEnumerator Start()
	{
		string path = Application.streamingAssetsPath;
		List<string> files = new List<string> ();
		files.AddRange (Directory.GetFiles (path, "*.mp4", SearchOption.AllDirectories));
		files.AddRange (Directory.GetFiles (path, "*.png", SearchOption.AllDirectories));

		Debug.Log ($"Files are: {string.Join (", ", files.ToArray())}");

		while (true)
		{
			int idx = Random.Range (0, files.Count);
			string file = files[idx];
			m_loader.ShowResource (file);
			float sleep = Random.Range (m_minSleep, m_maxSleep);
			yield return new WaitForSeconds (sleep);
		}
	}


}
