﻿using CornSDK;
using System;
using System.Text.RegularExpressions;

namespace Clansty.tianlang
{
    class NthsBotEvents : ICornEventHandler
    {
        public void OnTempMsg(TempMsgArgs e)
        {
            OnPrivateMsg(new PrivateMsgArgs()
            {
                FromQQ = e.FromQQ,
                FromNick = e.FromNick,
                Msg = e.Msg,
                Robot = e.Robot
            });
        }

        public void OnFriendMsg(FriendMsgArgs e)
        {
            OnPrivateMsg(new PrivateMsgArgs()
            {
                FromQQ = e.FromQQ,
                FromNick = e.FromNick,
                Msg = e.Msg,
                Robot = e.Robot
            });
        }

        private static void OnPrivateMsg(PrivateMsgArgs e)
        {
            var u = new User(e.FromQQ);
            if (u.Status == Status.setup)
                Setup.Enter(e);
            else if (e.Msg.StartsWith("我叫"))
            {
                //here, to line 93 handles manual name-filling event
                var name = e.Msg.GetRight("我叫").Trim();
                if (u.Verified)
                {
                    e.Reply(Strs.RnVerified); //你已经实名认证了，无需再次补填姓名
                    return;
                }

                if (u.VerifyMsg == VerifingResult.unsupported)
                {
                    //尝试进行验证
                    u.Name = name;
                    e.Reply(Strs.RnOK);
                    S.Si(u.ToString(Strs.NameFillInSucceeded));
                    UserInfo.CheckQmpAsync(u);
                    return;
                }

                try
                {
                    var p = Person.Get(name);
                    if (p.User is null)
                    {
                        //尝试进行验证
                        u.Name = name;
                        if (u.Verified)
                        {
                            e.Reply(Strs.RnOK);
                            S.Si(u.ToString(Strs.NameFillInSucceeded));
                            return;
                        }

                        e.Reply(Strs.UnexceptedErr + u.VerifyMsg);
                        UserInfo.CheckQmpAsync(u);
                        return;
                    }

                    e.Reply(Strs.RnOccupied);
                    return;
                }
                catch (PersonNotFoundException)
                {
                    e.Reply(Strs.RnNotFound);
                    return;
                }
                catch (Exception ex)
                {
                    C.WriteLn(ex);
                    e.Reply(Strs.UnexceptedErr);
                    return;
                }
            } // end manual name-filling handling
            else if (e.Msg == "tg")
            {
                TgBinding.Init(u);
            }
            else if (e.Msg.StartsWith("你已经绑定了一个 Telegram 账号"))
            {
                TgBinding.Init(u, true);
            }
            else if (e.FromQQ == 839827911 && e.Msg == "update")
            {
                var ret = Cmds.gcmds["update"].Func(null);
                e.Reply(ret);
            }
        }

        public void OnGroupMsg(GroupMsgArgs e)
        {
            Q2Tg.NewGroupMsg(e);

            if (e.Msg.StartsWith("点歌"))
                NetEase.Request(e);
            
            if (e.FromGroup==G.check)
            {
                Watchdog.Msg(e);
            }
            
            if (e.FromGroup == G.si)
                Cmds.Enter(e.Msg, e.FromQQ, false);

            if (e.FromGroup == G.major)
            {
                UserInfo.CheckQmpAsync(new User(e.FromQQ), e.FromCard);
                if (e.Msg.StartsWith("sudo "))
                    Cmds.Enter(e.Msg.GetRight("sudo "), e.FromQQ, true);
                Repeater.Enter(e.Msg);
            }

        }

        public void OnFriendRequest(FriendRequestArgs e)
        {
            e.Accept();
        }

        public void OnGroupAddMember(GroupMemberChangedArgs e)
        {
            if (e.FromGroup == G.major)
            {
                MemberList.major.Add(e.FromQQ);
                var u = new User(e.FromQQ);
                UserInfo.CheckQmpAsync(u);
                if (u.IsFresh)
                {
                    Setup.New(e.FromQQ, true);
                }
                else
                {
                    u.Namecard = u.ProperNamecard;
                    u.Status = Status.no;
                    S.Si(u.ToString(Strs.WizardSkip));
                }
            }

            if (e.FromGroup == G.g2020)
            {
                if (!MemberList.major.Contains(e.FromQQ))
                {
                    //这里由于不是大群成员所以需要以手动群临时方式发送
                    e.Robot.SendTempMsg(G.g2020, e.FromQQ, Strs.InviteMajor);
                }
            }
        }

        public void OnGroupJoinRequest(GroupRequestArgs e)
        {
            if (e.FromGroup == G.major)
            {
                var msg = e.Msg.Contains("答案：")
                    ? e.Msg.GetRight("答案：").Trim()
                    : e.Msg;
                if (msg.Contains('['))
                    msg = msg.GetLeft("[");
                var u = new User(e.FromQQ);
                C.Write("有人 ", ConsoleColor.DarkCyan);
                C.Write(e.FromQQ, ConsoleColor.Cyan);
                C.Write(" 加群 ", ConsoleColor.DarkCyan);
                C.Write(msg, ConsoleColor.Cyan);
                C.WriteLn("");
                if (u.Role == UserType.blackListed)
                {
                    e.Reject("blacklisted");
                    S.Si(u.ToString(Strs.AddRejected + Strs.Blacklisted) + $"\n申请信息: {msg}");
                    return;
                }

                if (u.VerifyMsg == VerifingResult.succeed) //本身就实名好了，填什么都给进
                {
                    e.Accept();
                    S.Si(u.ToString(Strs.AddAccepted + Strs.VerifiedUser) + $"\n申请信息: {msg}");
                    return;
                }

                string name;
                int enr;
                //现在这样，直接把姓名赋值给 u.Name，要是 succeed 就给进，occupied notFound unsupported 都不给进
                if (msg.IndexOf(" ", StringComparison.Ordinal) < 0) //没有空格的情况，20200813 直接用 parseNick 解析吧
                {
                    enr = UserInfo.ParseEnrollment(msg);
                    name = UserInfo.ParseNick(msg);
                }
                else
                {
                    enr = UserInfo.ParseEnrollment(msg.GetLeft(" "));
                    name = msg.GetRight(" ").Trim();
                }

                //其实现在有没有空格都是一样的了，就判断那些字段有没有
                if (string.IsNullOrWhiteSpace(name))
                {
                    e.Reject(Strs.FormatIncorrect);
                    S.Si(u.ToString(Strs.AddRejected + Strs.FormatErr) + $"\n申请信息: {msg}");
                    return;
                }

                //emptyName 这里已经处理过了接下来不会有
                if (enr > 2000)
                    u.Enrollment = enr;
                u.Name = name;

                #region 这三种情况都是人在受支持的年级的

                if (u.VerifyMsg == VerifingResult.succeed)
                {
                    e.Accept();
                    S.Si(u.ToString(Strs.AddAccepted + Strs.RnOK) + $"\n申请信息: {msg}");
                    return;
                }

                if (u.VerifyMsg == VerifingResult.occupied)
                {
                    e.Reject(Strs.JoinChkOccupied);
                    S.Si(u.ToString(Strs.AddRejected + Strs.AddReqOccupied) + $"\n申请信息: {msg}");
                    return;
                }

                if (u.VerifyMsg == VerifingResult.notFound)
                {
                    e.Reject(Strs.PersonNotFound);
                    S.Si(u.ToString(Strs.AddRejected + Strs.PersonNotFound) + $"\n申请信息: {msg}");
                    return;
                }

                #endregion

                //接下来是 unsupported 再做细分
                if (u.Enrollment < 2000)
                {
                    e.Reject(Strs.EnrFormatErr);
                    S.Si(u.ToString(Strs.AddRejected + Strs.EnrFormatErr) + $"\n申请信息: {msg}");
                    return;
                }

                //由于年级受支持的一定会返回上面三种情况，所以这里不用判断一定是年级不受支持的
                u.Junior = UserInfo.ParseJunior(msg.GetLeft(" "));
                S.Si(u.ToString(Strs.EnrUnsupported) + $"\n申请信息: {msg}");
                return;
            }
        }

        public void OnGroupInviteRequest(GroupRequestArgs e)
        {
            e.Accept();
        }

        public void OnGroupLeftMember(GroupMemberChangedArgs e)
        {
            
        }
    }
}