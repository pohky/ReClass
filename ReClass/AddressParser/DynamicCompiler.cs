using System.Linq.Expressions;
using System.Reflection;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Native;

namespace ReClass.AddressParser;

public class DynamicCompiler : IExecutor {
    public IntPtr Execute(IExpression expression, IProcessReader processReader) {
        return CompileExpression(expression)(processReader);
    }

    public static Func<IProcessReader, IntPtr> CompileExpression(IExpression expression) {
        var processParameter = Expression.Parameter(typeof(IProcessReader));

        return Expression.Lambda<Func<IProcessReader, IntPtr>>(
            GenerateMethodBody(expression, processParameter),
            processParameter
        ).Compile();
    }

    private static Expression GenerateMethodBody(IExpression expression, Expression processParameter) {
        static MethodInfo GetIntPtrExtension(string name) {
            return typeof(IntPtrExtension).GetRuntimeMethod(name, [typeof(IntPtr), typeof(IntPtr)]);
        }

        switch (expression) {
            case ConstantExpression constantExpression: {
                    var convertFn = typeof(IntPtrExtension).GetRuntimeMethod(nameof(IntPtrExtension.From), [typeof(long)]);

                    return Expression.Call(null, convertFn, Expression.Constant(constantExpression.Value));
                }
            case NegateExpression negateExpression: {
                    var argument = GenerateMethodBody(negateExpression.Expression, processParameter);

                    var negateFn = typeof(IntPtrExtension).GetRuntimeMethod(nameof(IntPtrExtension.Negate), [typeof(IntPtr)]);

                    return Expression.Call(null, negateFn, argument);
                }
            case AddExpression addExpression: {
                    var argument1 = GenerateMethodBody(addExpression.Lhs, processParameter);
                    var argument2 = GenerateMethodBody(addExpression.Rhs, processParameter);

                    return Expression.Call(null, GetIntPtrExtension(nameof(IntPtrExtension.Add)), argument1, argument2);
                }
            case SubtractExpression subtractExpression: {
                    var argument1 = GenerateMethodBody(subtractExpression.Lhs, processParameter);
                    var argument2 = GenerateMethodBody(subtractExpression.Rhs, processParameter);

                    return Expression.Call(null, GetIntPtrExtension(nameof(IntPtrExtension.Sub)), argument1, argument2);
                }
            case MultiplyExpression multiplyExpression: {
                    var argument1 = GenerateMethodBody(multiplyExpression.Lhs, processParameter);
                    var argument2 = GenerateMethodBody(multiplyExpression.Rhs, processParameter);

                    return Expression.Call(null, GetIntPtrExtension(nameof(IntPtrExtension.Mul)), argument1, argument2);
                }
            case DivideExpression divideExpression: {
                    var argument1 = GenerateMethodBody(divideExpression.Lhs, processParameter);
                    var argument2 = GenerateMethodBody(divideExpression.Rhs, processParameter);

                    return Expression.Call(null, GetIntPtrExtension(nameof(IntPtrExtension.Div)), argument1, argument2);
                }
            case ModuleExpression moduleExpression: {
                    var getModuleByNameFunc = typeof(IProcessReader).GetRuntimeMethod(nameof(IProcessReader.GetModuleByName), [typeof(string)]);
                    var moduleNameConstant = Expression.Constant(moduleExpression.Name);

                    var moduleVariable = Expression.Variable(typeof(ModuleInfo));
                    var assignExpression = Expression.Assign(moduleVariable, Expression.Call(processParameter, getModuleByNameFunc, moduleNameConstant));

                    return Expression.Block(
                        [moduleVariable],
                        assignExpression,
                        Expression.Condition(
                            Expression.Equal(moduleVariable, Expression.Constant(null)),
                            Expression.Constant(IntPtr.Zero),
                            Expression.MakeMemberAccess(moduleVariable, typeof(ModuleInfo).GetProperty(nameof(ModuleInfo.BaseAddress))!)
                        )
                    );
                }
            case ReadMemoryExpression readMemoryExpression: {
                    var addressParameter = GenerateMethodBody(readMemoryExpression.Expression, processParameter);

                    var functionName = readMemoryExpression.ByteCount == 4 ? nameof(IRemoteMemoryReaderExtension.ReadRemoteInt32) : nameof(IRemoteMemoryReaderExtension.ReadRemoteInt64);
                    var readRemoteIntPtrFn = typeof(IRemoteMemoryReaderExtension).GetRuntimeMethod(functionName, [typeof(IRemoteMemoryReader), typeof(IntPtr)]);

                    var callExpression = Expression.Call(null, readRemoteIntPtrFn, processParameter, addressParameter);

                    var paramType = readMemoryExpression.ByteCount == 4 ? typeof(int) : typeof(long);
                    var convertFn = typeof(IntPtrExtension).GetRuntimeMethod(nameof(IntPtrExtension.From), [paramType]);

                    return Expression.Call(null, convertFn, callExpression);
                }
        }

        throw new ArgumentException($"Unsupported operation '{expression.GetType().FullName}'.");
    }
}
