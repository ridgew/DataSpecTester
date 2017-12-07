[Define]
#EASE成功标识(short)
EaseSuccessFlag=short=>{Error:-1, UnKnown:0, Success:1010, SuccessUserAgent:1020, SuccessExpress:1120}

#编码序号(byte)
EaseEncode=byte=>{UTF8:0, Unicode:1, GB2312:2}

#EASE数据压缩格式(byte)
EaseCompress=byte=>{NoCompress:0, Lz77:1}

#首次连网标志(1-开机 0-使用中 2-清掉缓存)
EaseConnectState=byte=>{Working:0, StartUp:1, ClearCache:2}

#网络运营商ID(byte)
NetworkID=byte=>{ALL:0, CMCC:1, CUC:2, CTG:3}

#客户端拨号方式(byte)
ClientDialType=byte=>{CTNET:0, CTWAP:1}

#客户端功能号(byte)
RequestType=byte=>{PageV21:0/*页面请求:兼容EASE 2.1*/, Mixed:1/*页面及资源请求*/, Page:2/*页面请求*/, Resource:3/*资源请求*/, Application:4/*应用请求*/, UpdateCenter:5/*更新服务器连接地址*/}

#服务器响应码(short 3.2-2.6.9)
StatusCode=short=>{Exception:-1/*服务器异常*/, Success:0/*服务器处理成功*/, Updatable:1/*主程序有更新,请按照客户端更新策略下载*/}

#操作类型(byte)
CommandType=byte=>{None:0/*无任何操作*/, SMS:1/*发送短信*/, WAP:2/*调用WAP浏览器*/, Dial:3/*拨打电话*/, Updatable:4/*主程序存在更新，下载主程序*/}
