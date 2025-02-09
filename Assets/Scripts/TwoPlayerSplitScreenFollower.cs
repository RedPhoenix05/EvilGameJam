using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPlayerSplitScreenFollower : MonoBehaviour
{
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;

    private void Update()
    {
        if (player1 && player2)
        {
            transform.position = player1.position + (player2.position - player1.position) / 2;
            transform.right = player2.position - transform.position;
        }
    }
}
