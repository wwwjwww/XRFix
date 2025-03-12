/** 
 * @id cs/new_allocation
 * @kind problem
 * @name Using New() allocation in Update() method.
 * @description Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*/
import csharp
import semmle.code.csharp.dataflow.DataFlow
import semmle.code.csharp.dataflow.TaintTracking

class InstantiateSource extends DataFlow::ExprNode {
    InstantiateSource() {
        exists(MethodCall call | call.getEnclosingCallable().getName() = "Update" or call.getEnclosingCallable().getName() = "LateUpdate" or call.getEnclosingCallable().getName() = "FixedUpdate" | this.asExpr() = call or this.asExpr() = call.getAnArgument())
    }
}

class InstantiateSink extends DataFlow::ExprNode {
    InstantiateSink(){
        exists(ObjectCreation obj | this.asExpr() = obj)
       
    }
}
predicate isComponentTainted(Expr expSrc, Expr expDest) {
    exists(MethodCall call|call.getTarget().getName().toLowerCase().matches("addcomponent%") or call.getTarget().getName().toLowerCase().matches("getcomponent%") | expSrc = call.getQualifier() and call = expDest)
}

class InstantiateCheck extends TaintTracking::Configuration{
    InstantiateCheck() {
        this = "InstantiateCheck"
    }
    override predicate isSource(DataFlow::Node source){
        source instanceof InstantiateSource
    }
    override predicate isSink(DataFlow::Node sink){
        sink instanceof InstantiateSink
    }
    override predicate isAdditionalTaintStep(DataFlow::Node node1, DataFlow::Node node2){
        isComponentTainted(node1.asExpr(), node2.asExpr())
    }
}

from DataFlow::PathNode sink, DataFlow::PathNode source, InstantiateCheck ick
where ick.hasFlowPath(source, sink)
select source, "sink of this Instantiate() method is $@.", sink, "sink"