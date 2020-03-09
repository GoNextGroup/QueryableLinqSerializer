using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.CoreClasses;
using System;
using System.Linq.Expressions;

namespace QueryableLinqSerializer
{
    public class QueryableVisitor : ExpressionVisitor
    {
        public Node<Expression> ExpressionTreeRoot { get; private set; }
        
        private Node<Expression> currentLeaf;
        private Node<Expression> parentLeaf;

        private string baseErrorString = "Unsupported type";

        protected QueryableVisitor(Expression rawExpression)
        {
            ExpressionTreeRoot = new Node<Expression>();
            currentLeaf = ExpressionTreeRoot;
            Visit(rawExpression);
        }

        public static Node<Expression> ParseExpression(Expression expression) => (new QueryableVisitor(expression)).ExpressionTreeRoot;

        /*
        public static Expression GenerateExpression(Leaf<Expression> root)
        {
            if (root == null) { return null; }

            var currentLeaf = root;


            var newLeaf = this.currentLeaf.AddLeaf();
            var parentLeaf = this.currentLeaf;

            this.currentLeaf = newLeaf;
            newLeaf.Value = base.Visit(node);

            this.currentLeaf = parentLeaf;

            return newLeaf.Value;

        }
        */


        protected internal Expression ParseCurrentLeaf(Node<Expression> leaf)
        {
            switch (leaf.Value)
            {
                case BinaryExpression exprBinary:

                    if (exprBinary.Method == null)
                        return Expression.MakeBinary(exprBinary.NodeType, exprBinary.Left, exprBinary.Right, exprBinary.IsLiftedToNull, exprBinary.Method, exprBinary.Conversion);
                    if (exprBinary.Conversion == null)
                        return Expression.MakeBinary(exprBinary.NodeType, exprBinary.Left, exprBinary.Right, exprBinary.IsLiftedToNull, exprBinary.Method);

                    return Expression.MakeBinary(exprBinary.NodeType, exprBinary.Left, exprBinary.Right, exprBinary.IsLiftedToNull, exprBinary.Method, exprBinary.Conversion);
                    break; //<-- this code (and all other "breaks" added for developer convenience as a code division marker

                case ConditionalExpression exprConditional:
                    return Expression.Condition(exprConditional.Test, exprConditional.IfTrue, exprConditional.IfFalse, exprConditional.Type);

                case ConstantExpression exprConstant:
                    return Expression.Constant(exprConstant.Value, exprConstant.Type);

                case InvocationExpression exprInvocation:
                    return Expression.Invoke(exprInvocation.Expression, exprInvocation.Arguments);

                case LambdaExpression exprLambda:

                    return Expression.Lambda(exprLambda.Body, exprLambda.TailCall, exprLambda.Parameters);

                case ListInitExpression exprListInit:
                    return Expression.ListInit(exprListInit.NewExpression, exprListInit.Initializers);

                case MemberExpression exprMember:
                    return Expression.MakeMemberAccess(exprMember.Expression, exprMember.Member);

                case MemberInitExpression exprMemberInit:
                    return Expression.MemberInit(exprMemberInit.NewExpression, exprMemberInit.Bindings);

                case MethodCallExpression exprMethodCall:
                    return Expression.Call(exprMethodCall.Method, exprMethodCall.Arguments);

                case NewArrayExpression exprNewArray:
                    return exprNewArray.NodeType == ExpressionType.NewArrayInit ? Expression.NewArrayInit(exprNewArray.Type, exprNewArray.Expressions) : Expression.NewArrayBounds(exprNewArray.Type, exprNewArray.Expressions);

                case NewExpression exprNew:
                    return Expression.New(exprNew.Constructor, exprNew.Arguments, exprNew.Members);

                case ParameterExpression exprParameters:
                    return Expression.Parameter(exprParameters.Type, exprParameters.Name);

                case TypeBinaryExpression exprTypeBinary:
                    switch (exprTypeBinary.NodeType)
                    {
                        case ExpressionType.TypeIs:
                            return Expression.TypeIs(exprTypeBinary.Expression, exprTypeBinary.TypeOperand);
                            break;

                        case ExpressionType.TypeEqual:
                            return Expression.TypeEqual(exprTypeBinary.Expression, exprTypeBinary.TypeOperand);
                            break;

                        default:
                            throw new NotSupportedException(string.Format("{0} : {1}", baseErrorString, exprTypeBinary.NodeType.ToString()));
                            break;
                    }
                    break;

                case UnaryExpression exprUnary:
                    return exprUnary.NodeType == ExpressionType.UnaryPlus ? Expression.UnaryPlus(exprUnary.Operand, exprUnary.Method) : Expression.MakeUnary(exprUnary.NodeType, exprUnary.Operand, exprUnary.Type, exprUnary.Method);
            }

            throw new NotSupportedException(string.Format("{0}  {1}", baseErrorString, leaf.Value.NodeType.ToString()));
        }

        protected internal Node<Expression> AddNewLeaf(Expression expression)
        {
            var newLeaf = currentLeaf.AddLeaf();
            newLeaf.Value = expression;

            parentLeaf = currentLeaf;
            currentLeaf = newLeaf;

            return newLeaf;
        }
        protected internal void RestoreParent(Node<Expression> leaf) => parentLeaf = leaf.Parent;


        protected internal Expression Visiting(Func<Expression, Expression> visitFunc, Expression expression)
        {
            var newLeaf = AddNewLeaf(visitFunc(expression));
            RestoreParent(newLeaf);

            return newLeaf.Value;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            return Visiting((expr) => base.VisitBinary(expr as BinaryExpression), node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Visiting((expr) => base.VisitBlock(expr as BlockExpression), node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return Visiting((expr) => base.VisitConditional(expr as ConditionalExpression), node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return Visiting((expr) => base.VisitConstant(expr as ConstantExpression), node);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            return Visiting((expr) => base.VisitDefault(expr as DefaultExpression), node);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return Visiting((expr) => base.VisitDynamic(expr as DynamicExpression), node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return Visiting((expr) => base.VisitExtension(expr as Expression), node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            return Visiting((expr) => base.VisitIndex(expr as IndexExpression), node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return Visiting((expr) => base.VisitInvocation(expr as InvocationExpression), node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visiting((expr) => base.VisitLambda<T>(expr as Expression<T>), node);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            return Visiting((expr) => base.VisitListInit(expr as ListInitExpression), node);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            return Visiting((expr) => base.VisitLoop(expr as LoopExpression), node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return Visiting((expr) => base.VisitMember(expr as MemberExpression), node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return Visiting((expr) => base.VisitMemberInit(expr as MemberInitExpression), node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return Visiting((expr) => base.VisitMethodCall(expr as MethodCallExpression), node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            return Visiting((expr) => base.VisitNew(expr as NewExpression), node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return Visiting((expr) => base.VisitNewArray(expr as NewArrayExpression), node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Visiting((expr) => base.VisitParameter(expr as ParameterExpression), node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return Visiting((expr) => base.VisitRuntimeVariables(expr as RuntimeVariablesExpression), node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return Visiting((expr) => base.VisitTypeBinary(expr as TypeBinaryExpression), node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return Visiting((expr) => base.VisitUnary(expr as UnaryExpression), node);
        }
    }
}
