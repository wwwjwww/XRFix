import csharp

from ObjectCreation obj
where obj.getEnclosingCallable().getName() = "Update"
select obj, "Using New() to create object in Update here."