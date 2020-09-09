using CornSDK;
using System;
using Telegram.Bot;

namespace Clansty.tianlang
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            Test.Do();
#else
            Console.CancelKeyPress += Exit;
            Console.Title = $@"甜狼 {C.Version}";
            var handler = new Events();
            C.QQ = new Corn(new CornConfig()
            {
                selfQQ = C.self,
                friendMsgHandler = handler,
                groupAddMemberHandler = handler,
                groupInviteRequestHandler = handler,
                groupJoinRequestHandler = handler,
                groupMsgHandler = handler,
                friendRequestHandler = handler,
                tempMsgHandler = handler
            });
            C.TG = new TelegramBotClient(Tg.Token);
            C.TG.OnMessage += Tg.OnMsg;
            Db.Init();
            Timers.Init();
            MemberList.UpdateMajor();
            C.TG.StartReceiving();

            while (true)
            {
                var em = Console.ReadLine();
                if (em is null)
                    continue;
                try
                {
                    var key = (em.GetLeft(" ") == "" ? em : em.GetLeft(" ")).ToLower();
                    var act = em.GetRight(" ");
                    if (Cmds.gcmds.ContainsKey(key))
                    {
                        var m = Cmds.gcmds[key];
                        C.WriteLn(Cmds.gcmds[key].Func(act));
                    }
                }
                catch (Exception ex)
                {
                    C.WriteLn(ex.Message, ConsoleColor.Red);
                }
            }
#endif
        }

        internal static void Exit(object a = null, object b = null)
        {
            Db.Commit();
            System.Diagnostics.Process tt =
                System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
            tt.Kill();
        }
    }
}