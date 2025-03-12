/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     private void Update()
*     {
*         if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
*         {
*             Throw();
*         }
*     }
