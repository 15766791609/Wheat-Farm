using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnEnable()
    {
        EventHandler.AfterScenenUnloadEvent += SwitchConfinerShape;
    }
    private void OnDisable()
    {
        EventHandler.AfterScenenUnloadEvent -= SwitchConfinerShape;

    }
    private void SwitchConfinerShape()
    {

        PolygonCollider2D confinerShapr = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShapr;
        //��ȡ�·�Χʱ��Ҫ�������
        confiner.InvalidatePathCache();
    }
}
