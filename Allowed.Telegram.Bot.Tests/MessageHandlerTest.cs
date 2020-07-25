using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Models.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Tests
{
    [TestClass]
    public class MessageHandlerTest
    {
        private IMessageHandler MessageHandler { get; set; }
        private ControllersCollection ControllersCollection { get; set; }

        public MessageHandlerTest()
        {
            ControllersCollection = new ControllersCollection { 
                ControllerTypes = new List<Type> {
                    
                }
            };

            //MessageHandler = new MessageHandler<TelegramRole<int>, TelegramState<int>>();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
