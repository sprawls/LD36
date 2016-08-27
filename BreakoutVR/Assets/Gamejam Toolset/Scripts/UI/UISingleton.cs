using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIFadeable))]
[RequireComponent(typeof(Canvas))]
public class UISingleton<T> : Singleton<T> where T : UISingleton<T>
{
	protected UIFadeable m_fadeable = null;
	protected Canvas m_canvas = null;

	//=====================================================================================================
	protected override void OnEnable()
	{
		base.OnEnable();
		
		m_canvas = GetComponent<Canvas>();
		m_fadeable = GetComponent<UIFadeable>();
	}

	//--------------------------------------------------------------------------
	public void Show(bool skipFade = false)
	{
		m_fadeable.Show(skipFade);
	}

	//--------------------------------------------------------------------------
	public void Hide(bool skipFade = false)
	{
		m_fadeable.Hide(skipFade);
	}
}