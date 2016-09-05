using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class BallsController : Singleton<BallsController>
{
    [SerializeField]
    private Ball m_ballPrefab;

    //================================================================================================

    public static Action OnNoBallsLeft;
    public static Action<Ball> OnBallSpawn;

    //================================================================================================

    private List<Ball> m_ballsInPlay = new List<Ball>();

    //================================================================================================
    //-------------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Awake()
    {
        if (m_ballPrefab == null)
        {
            Debug.LogErrorFormat("The ball prefab reference of the object \"{0}\" isn't set in the component BallsController");
        }
    }

    //-------------------------------------------------------------------------------------
    public void SpawnBall()
    {
        SpawnBall(transform.position, transform.rotation);
    }

    //-------------------------------------------------------------------------------------
    public void SpawnBall(Vector3 position)
    {
        SpawnBall(position, Quaternion.identity);
    }

    //-------------------------------------------------------------------------------------
    public void SpawnBall(Vector3 position, Quaternion rotation)
    {
        GameObject ball = Instantiate(m_ballPrefab.gameObject, position, rotation) as GameObject;
        Ball ballComponent = ball.GetComponent<Ball>();
        m_ballsInPlay.Add(ballComponent);

        if (OnBallSpawn != null)
        {
            OnBallSpawn(ballComponent);
        }
    }

    #region Callbacks
    //------------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();
        
        Ball.OnGlobalBallDestroy += Callback_OnGlobalBallDestroyed;
    }

    //------------------------------------------------------------------------------
    private void Callback_OnGlobalBallDestroyed(Ball ballRef)
    {
        if (!m_ballsInPlay.Contains(ballRef))
        {
            Debug.LogErrorFormat("The ball with {0} was created outside of it's controller and isn't being handle by the gameloop." +
                                 " This can cause serious issue. Use BallsController.Instance.SpawnBall(position, rotation) instead of spawning manually. " +
                                 "Harmless error if testing in gym.", ballRef.name);
            return;
        }

        m_ballsInPlay.Remove(ballRef);
        if (m_ballsInPlay.Count == 0 && OnNoBallsLeft != null)
        {
            OnNoBallsLeft();
        }
    }

    //------------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();
        
        Ball.OnGlobalBallDestroy -= Callback_OnGlobalBallDestroyed;
    }
    #endregion
}
