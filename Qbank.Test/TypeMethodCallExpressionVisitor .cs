using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Qbank.Test
{
    public class TypeMethodCallExpressionVisitor : ExpressionVisitor
    {
        readonly Type toFindCall;
        public MethodInfo Method;

        public TypeMethodCallExpressionVisitor(Type toFindCall)
        {
            this.toFindCall = toFindCall;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var parameters = node.Method.GetParameters();
            if (parameters.Length > 0)
                if (parameters.Length > 0 && parameters[0].ParameterType == toFindCall)
                    Method = node.Method;
            return base.VisitMethodCall(node);
        }
    }
}