using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TestScriptableObject")]
public class GI_TestData : ScriptableObject
{
    public int testInt;
    public string testString;
    public float testFloat;
    //リストを作成
    public List<int> testIntList;
}
