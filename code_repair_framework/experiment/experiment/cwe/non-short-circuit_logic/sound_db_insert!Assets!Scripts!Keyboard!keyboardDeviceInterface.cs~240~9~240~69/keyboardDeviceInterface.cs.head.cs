You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.
[Same Type of Bug and Fix Example]
'''

public class ExampleScript : MonoBehaviour
{
    public static void Main(string[] args)
    {
        string a = null;
        //if (a != null & a.ToLower() == "hello world")
        //FIXED CODE:
        if (a != null && a.ToLower() == "hello world")

'''
//Buggy Code Line