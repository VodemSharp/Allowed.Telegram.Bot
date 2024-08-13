namespace Allowed.Telegram.Bot.Commands.Core;

public static class CommandParamsInjector
{
    public static List<object?> GetParameters(IServiceProvider provider, Command command,
        Dictionary<Type, object> typeInstanceDict)
    {
        List<object?> parameters = [];
        var parametersInfo = command.Handler.Method.GetParameters();

        foreach (var parameterInfo in parametersInfo)
        {
            if (typeInstanceDict.TryGetValue(parameterInfo.ParameterType, out var value))
            {
                parameters.Add(value);
                continue;
            }

            var parameterObj = provider.GetService(parameterInfo.ParameterType);
            parameters.Add(parameterObj);
        }

        return parameters;
    }
}