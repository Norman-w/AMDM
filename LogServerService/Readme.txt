2021年12月10日10:21:25
本项目直接debug带参数运行,即可启动服务部署工具

安装本服务时,不需要带参数直接启动服务

本服务用于检测付药机程序是否在正常运行,并且记录日志.如果付药机程序没有在运行,说明可能已经卡死了.需要重新启动应用程序.防止因为未知原因导致程序崩溃没有及时重启造成的业务不可用或夺得计算机控制权

详细的服务说明见LogServerService.cs上的说明.

由于昨夜与项目组的商讨,决定了日志服务器可能到时候会放在一个单独的医院服务器上,所以付药机的部分日志应当推送到服务器.如果考虑付药机的运行空间问题和瘦客户端逻辑,可以考虑把大部分日志推送到服务器而非本机保存.

推荐推送到服务器的日志比如 客户端重启 客户端宕机 客户端启动 客户端其他信息.这些都可以保存到服务器的数据库中