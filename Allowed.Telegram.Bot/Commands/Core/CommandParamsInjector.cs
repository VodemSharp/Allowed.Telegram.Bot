namespace Allowed.Telegram.Bot.Commands.Core;

public static class CommandParamsInjector
{
    public static List<object?> GetParameters(IServiceProvider provider, Command command,
        Dictionary<Type, Func<Type, object>> typeInstanceDict)
    {
        List<object?> parameters = [];
        var parametersInfo = command.Handler.Method.GetParameters();

        foreach (var parameterInfo in parametersInfo)
        {
            var matchingKey = typeInstanceDict.Keys.SingleOrDefault(
                type => type.IsAssignableFrom(parameterInfo.ParameterType));
            
            if (matchingKey != null && typeInstanceDict.TryGetValue(matchingKey, out var value))
            {
                parameters.Add(value.Invoke(parameterInfo.ParameterType));
                continue;
            }

            var parameterObj = provider.GetService(parameterInfo.ParameterType);
            parameters.Add(parameterObj);
        }

        return parameters;
    }
}