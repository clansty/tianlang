﻿using CornSDK.ApiTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clansty.tianlang
{
    static class MemberList
    {
        internal static HashSet<long> major = new();
        internal static void UpdateMajor(List<GroupMember> l)
        {
            major.Clear();
            foreach (var i in l)
            {
                major.Add(i.UIN);
            }
        }
        internal static async Task UpdateMajor()
        {
            UpdateMajor(await C.QQ.GetGroupMembers(G.major));
        }
    }
}
