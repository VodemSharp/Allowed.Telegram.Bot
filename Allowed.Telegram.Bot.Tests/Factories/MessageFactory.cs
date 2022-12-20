using System.Runtime.Serialization;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Tests.Factories;

public class MessageFactory
{
    private Message CreateMessage()
    {
        return (Message)FormatterServices
            .GetUninitializedObject(typeof(Message));
    }

    //private MessageEventArgs CreateMessageEventArgs(bool withMessage = true)
    //{
    //    MessageEventArgs args = (MessageEventArgs)FormatterServices
    //        .GetUninitializedObject(typeof(MessageEventArgs));

    //    if (withMessage)
    //    {
    //        MethodInfo method = typeof(MessageEventArgs).GetProperty("Message").SetMethod;
    //        method.Invoke(args, new object[] { CreateMessage() });
    //    }

    //    return args;
    //}

    //public MessageEventArgs CreateTextMessage(string text)
    //{
    //    MessageEventArgs args = CreateMessageEventArgs();
    //    args.Message.Text = text;

    //    return args;
    //}
}