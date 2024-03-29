﻿using System;
using System.Collections.Generic;
using Mirai_CSharp.Extensions;
using Mirai_CSharp.Models;
using NeteaseCloudMusicApi;
using Newtonsoft.Json;

namespace Clansty.tianlang
{
    /// <summary>
    /// 网抑云
    /// </summary>
    static class NetEase
    {
        public static void Request(IGroupMessageEventArgs e)
        {
            try
            {
                var sn = e.Chain.GetPlain().GetRight("点歌").Trim();
                var api = new CloudMusicApi();
                api.Request(CloudMusicApiProviders.Search,
                   new Dictionary<string, string>
                   {
                       ["keywords"] = sn,
                       ["limit"] = "1"
                   },
                   out var jobj);
                sn = jobj["result"]["songs"][0].Value<int>("id").ToString();
                //现在 sn 是 ID
                api.Request(CloudMusicApiProviders.SongDetail,
                    new Dictionary<string, string>
                    {
                        ["ids"] = sn
                    },
                    out jobj);
                var name = jobj["songs"][0].Value<string>("name");
                var res = "";
                foreach (var i in jobj["songs"][0]["ar"])
                { //适配多个歌手
                    res += i.Value<string>("name") + "/";
                }
                res = res.TrimEnd('/');
                //res 是歌手
                var pic = jobj["songs"][0]["al"].Value<string>("picUrl");
                api.Request(CloudMusicApiProviders.SongUrl,
                                new Dictionary<string, string>
                                {
                                    ["id"] = sn
                                },
                                out jobj);
                var mp3 = jobj["data"][0].Value<string>("url");
                var url = $"https://y.music.163.com/m/song?id={sn}";
                var json = new
                {
                    app = "com.tencent.structmsg",
                    desc = "音乐",
                    view = "music",
                    ver = "0.0.0.1",
                    prompt = $"[点歌]{name}",
                    meta = new
                    {
                        music = new
                        {
                            action = "",
                            android_pkg_name = "",
                            app_type = 1,
                            appid = 100495085,
                            desc = res,
                            jumpUrl = url,
                            musicUrl = mp3,
                            preview = pic,
                            sourceMsgId = "0",
                            source_icon = "",
                            source_url = "",
                            tag = $"甜狼 {C.Version}",
                            title = name
                        }
                    }
                };
                e.Reply(new IMessageBase[]{new AppMessage(JsonConvert.SerializeObject(json).Replace("https://", "http://").Replace("/", "\\/"))});
            }
            catch (Exception ex)
            {
                e.Reply(ex.ToString());
            }
        }
    }
}
