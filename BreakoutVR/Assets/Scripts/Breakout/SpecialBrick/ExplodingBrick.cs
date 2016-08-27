using UnityEngine;
using System.Collections;

public class ExplodingBrick : Brick
{
    [SerializeField]
    private float m_explosionRadius = 0.5f;

    protected override void Internal_OnDestroy()
    {


        base.Internal_OnDestroy();
    }
}
