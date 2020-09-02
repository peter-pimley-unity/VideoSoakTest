using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{


	private string m_servicingFilename = string.Empty;
	private string m_requestedFilename = string.Empty;


	[SerializeField]
	private RawImage m_image;


	private VideoPlayer m_video;

	[SerializeField]
	private Texture2D m_loadingTexture;


	public void ShowResource (string filename)
	{
		m_requestedFilename = filename;
	}

	// Start is called before the first frame update
	void Start()
	{
		m_video = GetComponent<VideoPlayer>();
		StartCoroutine (Run());
	}

	private bool HasNewRequest => m_servicingFilename != m_requestedFilename;


	IEnumerator Run ()
	{
		while (true)
		{
			m_servicingFilename = m_requestedFilename;
			bool isVideo = m_servicingFilename.EndsWith (".mp4");
			bool isPng = m_servicingFilename.EndsWith (".png");

			if (isVideo)
				yield return PlayVideo ();
			else if (isPng)
				yield return ShowImage();
			else
				while (! HasNewRequest)
					yield return null;

			yield return null;
		}
	}


	private  IEnumerator PlayVideo ()
	{
		Debug.Assert (m_video.isPlaying == false);

		string url = m_servicingFilename;
		Debug.Log ($"Loading video at \"{url}\"");
		m_video.url = url;

		m_video.Play();
		while (!m_video.isPlaying)
			yield return null;
		m_image.texture = m_video.targetTexture;

		while (! HasNewRequest)
			yield return null;
		m_image.texture = m_loadingTexture;

		m_video.Stop();
		while (m_video.isPlaying)
			yield return null;
	}

	private IEnumerator ShowImage ()
	{
		string url = $"file://{m_servicingFilename}";
		Debug.Log ($"Loading image at \"{url}\"");
		using (var uwr = UnityEngine.Networking.UnityWebRequest.Get (url))
		{
			var dlTex = new UnityEngine.Networking.DownloadHandlerTexture();
			uwr.downloadHandler = dlTex;
			var op = uwr.SendWebRequest();
			yield return op;
			Debug.Assert (! uwr.isNetworkError);
			m_image.texture = dlTex.texture;
		}

		while (! HasNewRequest)
			yield return null;

		m_image.texture = m_loadingTexture;
		yield return Resources.UnloadUnusedAssets();
	}
}
