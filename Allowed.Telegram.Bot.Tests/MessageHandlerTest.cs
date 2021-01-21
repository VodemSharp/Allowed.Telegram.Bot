using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Tests.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Allowed.Telegram.Bot.Tests
{
    [TestClass]
    public class MessageHandlerTest
    {
        private static MessageHandler MessageHandler { get; set; }
        private static MessageFactory MessageFactory { get; set; }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            ControllersCollection controllersCollection =
                new ControllersCollection
                {
                    ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(typeof(CommandController))).ToList()
                };

            MessageHandler = new MessageHandler(controllersCollection, null, null, null);

            MessageFactory = new MessageFactory { };
        }

        [TestMethod]
        [DataRow("/start", "MC1")]
        [DataRow("/default", "DC1")]
        [DataRow("This is text!", "DC2")]
        [DataRow("🙊 Some text...", "DC3")]
        [DataRow("🙊 Some text...", "DC3")]
        public void CommandTest(string text, string result)
        {
            Assert.AreEqual(MessageHandler.OnMessage(MessageFactory.CreateTextMessage(text)), result);
        }
    }
}
