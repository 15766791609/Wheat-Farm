using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

//�༭��ģʽ������
[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        //�����Ƿ���������
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            if (mapData != null)
                mapData.tileProperties.Clear();
        }
    }
    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            UpdateTilePeoperties();
#if UNITY_EDITOR
            if (mapData != null)
                EditorUtility.SetDirty(mapData);
#endif
        }
    }

    private void UpdateTilePeoperties()
    {
        //��ȡ��ʵ��������Ƭ�ķ�Χ
        currentTilemap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //�ѻ��Ʒ�Χ�����½�����
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //�ѻ��Ʒ�Χ�����Ͻ�����
                Vector3Int endPos = currentTilemap.cellBounds.max;

                for (int x = startPos.x; x < endPos.x; x++)
                {
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        //��Ƭ��ͼ����ģ��
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = this.gridType,
                                boolTypeValue = true
                            };

                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
