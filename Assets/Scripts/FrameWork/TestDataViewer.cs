using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDataViewer : MonoBehaviour
{
    [SerializeField] GI_TestData testData;

    int testInt;
    string testString;
    float testFloat;
    //intのリストを作成
    List<int> testIntList;

    void Start()
    {
        testInt = testData.testInt;
        testString = testData.testString;
        testFloat = testData.testFloat;
        testIntList = testData.testIntList;

        Debug.Log("testInt : " + testInt + "testString : " + testString + "testFloat : " + testFloat);
        for (int i = 0; i < testIntList.Count; i++)
        {
            Debug.Log("testIntList[" + i + "] : " + testIntList[i]);
        }
    }
}
