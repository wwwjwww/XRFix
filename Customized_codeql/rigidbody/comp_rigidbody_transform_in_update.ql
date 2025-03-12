/** 
 * @id cs/rigidbody_transform
 * @kind problem
 * @name Transform object of Rigidbody in Update() methods
 * @description Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
*/

import csharp
import semmle.code.csharp.dataflow.DataFlow
import semmle.code.csharp.dataflow.TaintTracking

class InstantiateSource extends DataFlow::Node {
    InstantiateSource() {
        exists(MethodCall call | call.getEnclosingCallable().getName() = "Update"| this.asExpr() = call or this.asExpr() = call.getAnArgument() or this.asExpr() = call.getQualifier())
    }
}

class InstantiateSink extends DataFlow::ExprNode {
    InstantiateSink(){
        exists(MethodCall call| this.asExpr() = call.getQualifier() or this.asExpr() = call | call.getTarget().getName().matches("GetComponent<Rigidbody>%"))
        or
        exists(Access ta, MethodCall call| this.asExpr() = ta and call=ta.getParent().getParent() | call.getTarget().getName().matches("GetComponent<Rigidbody>%") )
        or
        exists(FieldAccess fa | fa.getTarget().getType().getName() = "Rigidbody"| this.asExpr() = fa)
        or
        exists(MethodCall call, Access ac | this.asExpr() = ac and call=ac.getParent().getParent().getParent()| call.getTarget().getName().matches("AddForce%") and call.getQualifier().getType().getName() = "Rigidbody")
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
select source.getNode(), "Here exists the Rigidbody transform in update $@.", sink.getNode(), "here"