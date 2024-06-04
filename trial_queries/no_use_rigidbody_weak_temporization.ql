import csharp
import semmle.code.csharp.dataflow.DataFlow
import semmle.code.csharp.dataflow.TaintTracking

class InstantiateSource extends DataFlow::Node {
    InstantiateSource() {
        exists(MethodCall call | call .getTarget().getName().toLowerCase().matches("rotat%") and call.getEnclosingCallable().getName() = "Update"| this.asExpr() = call)
    }
}

class InstantiateSink extends DataFlow::ExprNode {
    InstantiateSink(){
        exists(MethodCall call|this.asExpr() = call.getQualifier() or this.asExpr() = call.getAnArgument())
    }
}

predicate isComponentTainted(Expr expSrc, Expr expDest) {
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
        isComponentTainted(node1.asExpr(), node2.asExpr())
    }
}

from DataFlow::PathNode sink, DataFlow::PathNode source, InstantiateCheck ick
where ick.hasFlowPath(source, sink)
select source.getNode(), "Here exists the method instantiate $@.", sink.getNode(), "here"