import csharp
import semmle.code.csharp.dataflow.DataFlow
import semmle.code.csharp.dataflow.TaintTracking

class InstantiateSource extends DataFlow::ExprNode {
    InstantiateSource() {
        this.getExpr().(MethodCall).getTarget().getName().toLowerCase().matches("instantiate%") and
        this.getExpr().(MethodCall).getEnclosingCallable().getName() = "Update"
    }
}

class InstantiateCheck extends TaintTracking::Configuration{
    InstantiateCheck() {
        this = "InstantiateCheck"
    }
    override predicate isSource(DataFlow::Node source){
        source instanceof InstantiateSource
    }
    override predicate isSink(DataFlow::Node sink) {
        exists(MethodCall call | sink.asExpr() = call.getQualifier() or sink.asExpr() = call.getAnArgument()| call.getTarget().getName().toLowerCase().matches("destroy%"))
    }
}

from DataFlow::PathNode sink, DataFlow::PathNode source, InstantiateCheck ick
where ick.hasFlowPath(source, sink)
select source, "Here exists the method instantiate $@.", sink, "here"