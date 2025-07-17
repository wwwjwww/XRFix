     private readonly object lockObject = new object();

     public Compute(RunDelegate run_delegate)
     {
             Schedule(() =>
             {
                 T result1 = run_delegate();
                 lock (lockObject)
                 {
                     result = result1;
                     got_answer = true;
                 }
             });
     }
