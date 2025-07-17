     private readonly object lockObject = new object();

     public bool TryGetAnswer(out T answer)
     {
             if (got_answer)
             {
                 lock (lockObject)
                     answer = result;
                 return true;
             }
             else
             {
                 answer = default(T);
                 return false;
             }
     }


