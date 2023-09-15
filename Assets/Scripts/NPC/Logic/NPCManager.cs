using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoSingleton<NPCManager>
{
    public List<NPCPostion> npcPositionList;

    public SceneRouteDataList_SO SceneRouteDate;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitScenenRouteDict();
    }


    private void InitScenenRouteDict()
    {
        if(SceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in SceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                    continue;
                else
                    sceneRouteDict.Add(key, route);
            }
        }
    }
    

    /// <summary>
    /// 获得两个场景中间的路径
    /// </summary>
    /// <param name="fromScenenName"></param>
    /// <param name="gotoSceneName"></param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromScenenName, string gotoSceneName)
    {
        return sceneRouteDict[fromScenenName + gotoSceneName];
    }
}
