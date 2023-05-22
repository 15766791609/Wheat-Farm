using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {

        PolygonCollider2D confinerShapr = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShapr;
        //获取新范围时需要清除缓存
        confiner.InvalidatePathCache();
    }
}
