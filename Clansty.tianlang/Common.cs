﻿using System;
using System.Runtime.InteropServices;
using Native.Sdk.Cqp;

namespace Clansty.tianlang
{
    public static class C
    {
        /*
         * TODO:
         * 號碼全用 long
         * 新生群名片标准化
         * 检测是否成员来决定是否推荐大群/新生群
         * 新生群后台实名实现
         * 进群申请的姓名校验
         * 進群申請分析更智能，用 parse nick 來 parse 姓名
         *
         * 7/31 管理群會話 TODO 總結：
         * 草了现在 branch 是 get only 的也得改（費點功夫）
         * 申請加群裏面的名字也校驗
         * 2020届初中是实名信息有的，改不了新的 enrollment
         * 到时候会有一个人两个实名身份的情况（難度大）
         */
        
        public const string Version = "3.1.18.3";//20200801
        public static CQApi CQApi = null;

        public static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteLn(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
    }
}
