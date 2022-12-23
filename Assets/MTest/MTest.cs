/*---------------------------------------
*|  Describe : 
*|  Author   :  OTL
*|  Version  :  1.0
*|   $time$
*---------------------------------------
*/

using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class MTest : NetworkBehaviour
{
    private NetworkAnimator m_Animator;
    private Animator m_Anim;
    private bool m_IsMoving;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_Animator = GetComponent<NetworkAnimator>();
        m_Anim = m_Animator.Animator;

    }
    private void LateUpdate()
    {

        if (!IsSpawned || !IsOwner)
        {
            return;
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (!m_IsMoving)
            {
                m_IsMoving = true;
                MoveAnimServerRpc();
            }
        }
        else
        {
            if (m_IsMoving)
            {
                m_IsMoving = false;
                IdleAnimServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveAnimServerRpc()
    {
        m_Anim.Play("Move");
    }

    [ServerRpc(RequireOwnership = false)]
    private void IdleAnimServerRpc()
    {
        m_Anim.Play("Idle");
    }

}
