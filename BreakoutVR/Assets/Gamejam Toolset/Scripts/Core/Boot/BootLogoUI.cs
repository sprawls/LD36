using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class BootLogoUI : MonoBehaviour 
{
	[Header("Component"), SerializeField]
	public RawImage m_image;

	public delegate void BootLogoEndCallback();
	private MovieTexture m_movie;

	private void Awake()
	{
		if (m_image == null)
		{
			Debug.LogError("One of the component related to the boot logo ui has not been set properly");
			return;
		}
	}

	public void StartLogoUI(BootLogoEndCallback callback)
	{
		StartCoroutine(Coroutine_BootLogo(callback));
	}

	private IEnumerator Coroutine_BootLogo(BootLogoEndCallback callback)
	{
		m_movie = (MovieTexture)m_image.texture;
		m_movie.Play();

		while (m_movie.isPlaying)
		{
			yield return null;
		}

		callback.Invoke();
	}
}
