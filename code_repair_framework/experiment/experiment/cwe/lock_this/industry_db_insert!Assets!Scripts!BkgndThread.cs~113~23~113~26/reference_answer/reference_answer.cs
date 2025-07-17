     private readonly object lockObject = new object();

     public Compute(RunDelegateWithCompute run_delegate)
     {
             Schedule(() =>
             {
                 T result1 = run_delegate(this);
                 lock (this)
                 {
                     result = result1;
                     got_answer = true;
                 }
             });
     }

