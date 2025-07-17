using System.Collections;

    public void setPress(bool on)
    {
            // BUG: Constant condition
            // MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
            //             if(!on || (on && masterControl.instance.tooltipsOn))

            //Avoid constant conditions where possible, and either eliminate the conditions or replace them.
            // FIXED CODE:
