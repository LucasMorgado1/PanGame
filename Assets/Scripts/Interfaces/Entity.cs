using System.Collections;
using UnityEngine;

public interface IWalkable
{
    void Walk();
}

public interface IJump
{
    void Jump();
}

public interface IDash
{
    IEnumerator Dash();
}

