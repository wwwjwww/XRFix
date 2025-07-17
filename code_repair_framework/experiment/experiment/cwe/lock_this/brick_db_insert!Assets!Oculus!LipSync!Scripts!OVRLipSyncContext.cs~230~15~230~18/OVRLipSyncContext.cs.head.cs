You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.
[Same Type of Bug and Fix Example]
'''

    //public void Inc()
    //{
        //lock (this)   // Correct
        //{
        //    ++value;
        //}
    //}
    //FIXED CODE:
    private readonly Object mutex = new Object();

    int value = 0;

    public void Inc()
    {
        lock (mutex)
        {
            ++value;
        }
    }

'''
[Buggy Code Line]