using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzleitem : MonoBehaviour
{
    public GameObject GetEffect;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            Destroy(Instantiate(GetEffect, transform.position, Quaternion.identity), 2f);
            // 플레이어가 퍼즐 아이템과 충돌했을 때의 처리
            Debug.Log("Puzzle item collected!");
            gameObject.SetActive(false);
        }
    }
}
