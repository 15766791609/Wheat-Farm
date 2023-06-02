using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("M Studio//Test")]
    public static void ShowExample()
    {
        Test wnd = GetWindow<Test>();
        wnd.titleContent = new GUIContent("Test");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        ObjectField a = root.Q<ObjectField>();
        Debug.Log(a);
        Label b = root.Q<Label>();
        Debug.Log(b.text);
    }
}
