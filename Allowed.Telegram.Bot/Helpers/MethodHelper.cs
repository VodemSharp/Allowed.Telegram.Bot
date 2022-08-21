using System.Reflection;
using System.Runtime.CompilerServices;

namespace Allowed.Telegram.Bot.Helpers;

public static class MethodHelper
{
    private static bool IsAsyncMethod(MethodInfo method)
    {
        return method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
    }

    public static async Task<object> InvokeMethod(MethodInfo method, List<object> parameters, object instance)
    {
        if (IsAsyncMethod(method))
        {
            if (method.ReturnType == typeof(Task))
            {
                await (Task)method.Invoke(instance, parameters.ToArray());
                return null;
            }

            return await (Task<object>)method.Invoke(instance, parameters.ToArray());
        }

        return method.Invoke(instance, parameters.ToArray());
    }
}