using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityEditor.ProjectWindowCallback;
using System.Text.RegularExpressions;
//雨松MoMo新书中的代码
public class CreateScriptWithTemplate
{
	//脚本模板所在目录
	private const string ZFRAMEWORK_SCRIPT_DEFAULT = "Assets/ZFramework/Editor/Util/ScriptTemplates/LibaryTemplate.cs.txt";
    private const string ZFRAMEWORK_MBSCRIPT_DEFAULT = "Assets/ZFramework/Editor/Util/ScriptTemplates/MonoBehaviorTemplate.cs.txt";
    private const string ZFRAMEWORK_EDITORSCRIPT_DEFAULT = "Assets/ZFramework/Editor/Util/ScriptTemplates/EditorTemplate.cs.txt";

    [MenuItem("Assets/Create/ZFrameworkLibaryScript", false, 80)]
	public static void CreatMyScript()//创建ZFramework库脚本
    {
        string locationPath = GetSelectedPathOrFallback();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
		locationPath + "/MyNewBehaviourScript.cs",
        null, ZFRAMEWORK_SCRIPT_DEFAULT);
    }
    [MenuItem("Assets/Create/ZFrameworkMBScript", false, 80)]
    public static void CreatMyScript2()//创建应用ZFramework库的MB脚本
    {
        string locationPath = GetSelectedPathOrFallback();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        locationPath + "/MyNewBehaviourScript.cs",
        null, ZFRAMEWORK_MBSCRIPT_DEFAULT);
    }
    [MenuItem("Assets/Create/ZFrameworkEditorScript", false, 80)]
    public static void CreatMyScript3()//创建ZFramework编辑器扩展脚本
    {
        string locationPath = GetSelectedPathOrFallback();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        locationPath + "/MyNewBehaviourScript.cs",
        null, ZFRAMEWORK_EDITORSCRIPT_DEFAULT);
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}    
 
class MyDoCreateScriptAsset : EndNameEditAction
{
 
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }
 
    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        StreamReader streamReader = new StreamReader(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
		//替换文件名
        text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
        bool encoderShouldEmitUTF8Identifier = true;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }
} 