﻿using CornSDK;using System;using Telegram.Bot;namespace Clansty.tianlang{    static class C    {        /*         * TODO:         */        internal const string Version = "4.1.6.4"; //20201018#if DEBUG        internal const long self = 2981882373;#else        internal const long self = 1980853671;#endif        internal const string link = "tg://join?invite=FPePbVRJzNUQculZSGf7Vw";        internal const long tguid = 712657902;        internal static DefaultCronLogger logger = new DefaultCronLogger();        internal static void Write(object text, ConsoleColor color = ConsoleColor.White)        {            Console.ForegroundColor = color;            Console.Write(text);            Console.ForegroundColor = ConsoleColor.White;        }        internal static void WriteLn(object text, ConsoleColor color = ConsoleColor.White)        {            logger.Log(text.ToString());        }        internal static Corn QQ = null;        internal static TelegramBotClient TG = null;    }}