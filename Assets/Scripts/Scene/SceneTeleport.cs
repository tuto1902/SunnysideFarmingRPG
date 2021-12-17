using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] SceneName sceneToLoad;
    [SerializeField] Vector3 spawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            float positionX = Mathf.Approximately(spawnPoint.x, 0f) ? player.transform.position.x : spawnPoint.x;
            float positionY = Mathf.Approximately(spawnPoint.y, 0f) ? player.transform.position.y : spawnPoint.y;
            float positionZ = 0f;

            SceneControllerManager.Instance.FadeAndLoadScene(sceneToLoad.ToString(), new Vector3(positionX, positionY, positionZ));
        }
    }
}
