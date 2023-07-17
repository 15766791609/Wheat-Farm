using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    //��ʼ��ţ�ָ��ճ���
    int sceneIndex = -1;

    GUIContent[] sceneNames;
    //�����и��ʶ���
    readonly string[] scenePathSplit = { "/", ".unity" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent laber)
    {
        //�����������޳�����ִ��
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
        //��ʼ������
        sceneNames = new GUIContent[scenes.Length];

        for (int i = 0; i < sceneNames.Length; i++)
        {
            string path = scenes[i].path;
            Debug.Log(path);
            //�и��ַ���
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
        //���б�Ŀǰѡ��Ϊ��ʱ���޷�����ѡ��
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
