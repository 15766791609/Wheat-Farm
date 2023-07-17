using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    //初始序号，指向空场景
    int sceneIndex = -1;

    GUIContent[] sceneNames;
    //用于切割的识别符
    readonly string[] scenePathSplit = { "/", ".unity" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent laber)
    {
        //生成设置中无场景则不执行
        if (EditorBuildSettings.scenes.Length == 0) return;

        if(sceneIndex ==-1)
        {
            GetScenenNameArray(property);
        }
        int oldIndex = sceneIndex;

        sceneIndex =  EditorGUI.Popup(position, laber, sceneIndex, sceneNames);

        if (oldIndex != sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }

    private void GetScenenNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;
        //初始化数组
        sceneNames = new GUIContent[scenes.Length];

        for (int i = 0; i < sceneNames.Length; i++)
        {
            string path = scenes[i].path;
            Debug.Log(path);
            //切割字符串
            string[] splitPath = path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";
            if (splitPath.Length > 0)
            {
                sceneName = splitPath[splitPath.Length - 1];
            }
            else
            {
                sceneName = ("(Delete Scene)");
            }
            sceneNames[i] = new GUIContent(sceneName);
        }

        if (sceneNames.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Check Your Buili Setting") };

        }
        //当列表目前选项为空时会无法更改选项
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;
            for (int i = 0; i < sceneNames.Length - 1; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }
            if (!nameFound)
                sceneIndex = 0; 
        }
        else
        {
            sceneIndex = 0;
        }

        property.stringValue = sceneNames[sceneIndex].text;

    }

}
