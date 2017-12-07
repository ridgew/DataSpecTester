[ResponseHeader]
!import=define,base
#网络连接成功标志(1010 - 网络连接成功)
ESP_SuccessFlag=EaseSuccessFlag
#包后续长度(不包含此参数长度)
ESP_LeaveLength=int
#软件ID
ESP_SoftwareID=int
#会话标识（不大于50）
ESP_SessionID=EaseString
#请求功能号
ESP_Protocol=RequestType


[ResponseBase]
!import=define,base
#响应头信息
ESP_Header=ResponseHeader
#服务器端响应码(响应码不为0时弹出提示框)
ESP_Code=StatusCode
#响应码不为0 时弹出提示框的消息
ESP_Message=EaseString
#操作类型
ESP_Method=CommandType
#操作指令 操作类型为0 时不存在，长度为0;操作类型为1时为短信指令; 操作类型为2时为WAP链接; 操作类型为3时为电话号码; 操作类型为4时为主程序下载链接
ESP_Command=EaseString