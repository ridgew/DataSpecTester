[ResponseHeader]
!import=define,base
#�������ӳɹ���־(1010 - �������ӳɹ�)
ESP_SuccessFlag=EaseSuccessFlag
#����������(�������˲�������)
ESP_LeaveLength=int
#���ID
ESP_SoftwareID=int
#�Ự��ʶ��������50��
ESP_SessionID=EaseString
#�����ܺ�
ESP_Protocol=RequestType


[ResponseBase]
!import=define,base
#��Ӧͷ��Ϣ
ESP_Header=ResponseHeader
#����������Ӧ��(��Ӧ�벻Ϊ0ʱ������ʾ��)
ESP_Code=StatusCode
#��Ӧ�벻Ϊ0 ʱ������ʾ�����Ϣ
ESP_Message=EaseString
#��������
ESP_Method=CommandType
#����ָ�� ��������Ϊ0 ʱ�����ڣ�����Ϊ0;��������Ϊ1ʱΪ����ָ��; ��������Ϊ2ʱΪWAP����; ��������Ϊ3ʱΪ�绰����; ��������Ϊ4ʱΪ��������������
ESP_Command=EaseString