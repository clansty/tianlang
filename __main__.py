from mirai import Mirai, Plain, MessageChain, Friend, Group, Member, FriendMessage, GroupMessage, TempMessage
import asyncio
import db
import groups
import appmgr
from user import User


qq = 2981882373  # 字段 qq 的值
authKey = 'QwQQAQOwO'  # 字段 authKey 的值
# httpapi所在主机的地址端口,如果 setting.yml 文件里字段 "enableWebsocket" 的值为 "true" 则需要将 "/" 换成 "/ws", 否则将接收不到消息.
mirai_api_http_locate = 'localhost:8080/ws'

app = Mirai(f"mirai://{mirai_api_http_locate}?authKey={authKey}&qq={qq}")


if(__name__ == "__main__"):

    groups.init(True)
    db.init()
    appmgr.init(app)

    @app.receiver("GroupMessage")
    async def _(app: Mirai, group: Group, member: Member, message: GroupMessage):
        if(group.id == groups.major):
            import repeater
            await repeater.handle(message.toString())

    @app.receiver("TempMessage")
    async def _(app: Mirai, group: Group, usr: Member, message: TempMessage):
        await FMHandler(app,usr,message)

    @app.receiver("FriendMessage")
    async def _(app: Mirai, usr: Friend, message: FriendMessage):
        await FMHandler(app,usr,message)

    async def FMHandler(app:Mirai, usr, msg):
        u=User(usr)
        print(u.get('name'))
        print(u.get('name', 'nick'))

    app.run()
