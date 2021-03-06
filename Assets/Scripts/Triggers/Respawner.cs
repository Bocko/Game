using UnityEngine;

public class Respawner : MonoBehaviour
{
    public Transform respawnPos;
    public Color gizmoColor;
    public ParticleSystem respawnEffect;

    ParticleSystem spawnedEffect;

    //FOR THIS TO WORK ON THE PLAYER I HAD TO TURN ON "AUTO SYNC TRANSFORMS" IN THE PROJECT SETTINGS'S PHYSICS TAB
    //if this causes any problems just turn it off and check if the collider has a character controller
    //and if it has one disable it set the pos and then reenable it
    void OnTriggerEnter(Collider other)
    {
        if (spawnedEffect == null)
        {
            spawnedEffect = Instantiate(respawnEffect, respawnPos);
        }
        spawnedEffect.Play();
        other.transform.position = respawnPos.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, UnityIcons.RespawnerTriggerIcon, false, gizmoColor);
        Gizmos.DrawIcon(respawnPos.position, UnityIcons.RespawnerPointIcon, false, gizmoColor);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
