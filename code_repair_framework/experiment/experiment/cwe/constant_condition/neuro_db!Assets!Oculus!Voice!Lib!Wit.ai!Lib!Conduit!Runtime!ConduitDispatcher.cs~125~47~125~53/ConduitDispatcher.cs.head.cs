You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.
[Same Type of Bug and Fix Example]
'''

class Bad
{
    public int Max(int a, int b)
    {
        //return a > a ? a : b;
        //FIXED CODE:
        return a > b ? a : b;

'''
//Buggy Code Line