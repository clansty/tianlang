from nonebot import message_preprocessor, NoneBot, aiocqhttp
from nonebot.plugin import PluginManager
import groups, send

global last
global times
dd = "打断"
dg = "出现了打断怪"
last=""
times=1

@message_preprocessor
async def _(bot: NoneBot, event: aiocqhttp.Event, plugin_manager: PluginManager):
    global last
    global times
    if(event.group_id != groups.test):
        return
    if(event.raw_message == last):
        times += 1
        if(times == 4):
            times = 1
            if(last == dd):
                send.test(dg)
                last = dg
            else:
                send.test(dd)
                last = dd
    else:
        times = 1
        last = event.raw_message
