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
        exists(FieldAccess fa | fa.getTarget().getType().getName() = "Rigidbody"| this.asExpr() = fa)
    }
}

class InstantiateSink extends DataFlow::ExprNode {
    InstantiateSink(){
        exists(MethodCall call | call.getEnclosingCallable().getName() = "Update" and call.getTarget().getName().toLowerCase().matches("translate") or call.getTarget().getName().toLowerCase().matches("rotate") | this.asExpr() = call.getQualifier() or this.asExpr() = call.getAnArgument() or this.asExpr() = call.getQualifier().getAChild())
    }
}

predicate isUnityAdditionalTainted(Expr expSrc, Expr expDest) {
    exists(MethodCall call, MethodCall call1, Method m|expSrc = call.getQualifier() 
    and expDest = call1 and call1.getTarget() = m and m.getName().toLowerCase().matches("getcomponent%"))
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
        isUnityAdditionalTainted(node1.asExpr(), node2.asExpr())
    }
}

from DataFlow::PathNode sink, DataFlow::PathNode source, InstantiateCheck ick
where ick.hasFlowPath(source, sink)
select source.getNode(), "Here exists the Rigidbody transform in update $@.", sink.getNode(), "here"