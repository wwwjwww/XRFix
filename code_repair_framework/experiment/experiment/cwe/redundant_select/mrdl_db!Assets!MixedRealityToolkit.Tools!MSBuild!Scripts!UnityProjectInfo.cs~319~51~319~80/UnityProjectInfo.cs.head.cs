You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.
[Same Type of Bug and Fix Example]
'''

public class ExampleScript : MonoBehaviour
{
    static void Main(string[] args)
    {
        List<int> lst = Enumerable.Range(1, 10).ToList();

        //foreach (int i in lst.Select(e => e).Where(e => e % 2 == 0))
        //FIXED CODE:
        foreach (int i in lst.Where(e => e % 2 == 0))

'''
//Buggy Code Line