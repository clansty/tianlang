﻿using CornSDK;using System;using System.Runtime.InteropServices;using Telegram.Bot;namespace Clansty.tianlang{    static class C    {        /*         * TODO:         */        internal const string Version = "4.1.4.10"; //20201004#if DEBUG        internal const long self = 2981882373;#else        internal const long self = 1980853671;#endif        internal const string link = "tg://join?invite=FPePbVRJzNUQculZSGf7Vw";        internal const long tguid = 712657902;                internal static void Write(object text, ConsoleColor color = ConsoleColor.White)        {            Console.ForegroundColor = color;            Console.Write(text);            Console.ForegroundColor = ConsoleColor.White;        }        internal static void WriteLn(object text, ConsoleColor color = ConsoleColor.White)        {            Console.ForegroundColor = color;            Console.WriteLine(text);            Console.ForegroundColor = ConsoleColor.White;        }        internal static Corn QQ = null;        internal static TelegramBotClient TG = null;    }}