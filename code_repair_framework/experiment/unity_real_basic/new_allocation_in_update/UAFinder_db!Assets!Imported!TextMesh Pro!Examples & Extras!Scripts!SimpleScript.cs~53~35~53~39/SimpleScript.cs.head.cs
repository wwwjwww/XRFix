You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents. Please only change the code from /Assets/Imported/TextMesh Pro/Examples & Extras/Scripts/SimpleScript.cs
[Same Type of Bug and Fix Example]
'''

public class ExampleScript : MonoBehaviour
{
    //void Update()
    //{
        //List<string> mList = new List<String>();
    //}
    //FIXED CODE:
    List<string> new_mList = new List<String>();
    
    void Update()
    {
        mList = new_mList
    }

'''
[Buggy Code Line]