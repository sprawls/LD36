using UnityEngine;
using System.Collections;

public class WaitForLevelLoad : CustomYieldInstruction {

	private AsyncOperation m_async;

	public WaitForLevelLoad(AsyncOperation asyncOperation)
	{
		m_async = asyncOperation;
	}

	public override bool keepWaiting { get { return !m_async.isDone; } }
}
