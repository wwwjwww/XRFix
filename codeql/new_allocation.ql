/** 
 * @id cs/new_allocation
 * @kind problem
 * @name Using New() allocation in Update() method.
 * @description Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*/
import csharp

from ObjectCreation obj
where obj.getEnclosingCallable().getName() = "Update"
select obj, "Using New() to create object in Update here."