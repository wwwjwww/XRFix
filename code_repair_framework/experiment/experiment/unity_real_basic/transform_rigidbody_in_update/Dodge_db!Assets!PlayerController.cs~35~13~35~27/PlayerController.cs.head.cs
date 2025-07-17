You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.
[Same Type of Bug and Fix Example]
'''

public class ExampleScript : MonoBehaviour
{
    public Rigidbody rb;

    //void Update()
    //{
        //rb.velocity = direction * speed;
        //rb.angularVelocity = 0;
        //rb.gravityScale = 0;
    //}
    //FIXED CODE:
    void FixedUpdate()
    {
        rb.velocity = direction * speed;
        rb.angularVelocity = 0;
        rb.gravityScale = 0;
    }

'''
[Buggy Code Line]