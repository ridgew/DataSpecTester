[Define]
#EASE�ɹ���ʶ(short)
EaseSuccessFlag=short=>{Error:-1, UnKnown:0, Success:1010, SuccessUserAgent:1020, SuccessExpress:1120}

#�������(byte)
EaseEncode=byte=>{UTF8:0, Unicode:1, GB2312:2}

#EASE����ѹ����ʽ(byte)
EaseCompress=byte=>{NoCompress:0, Lz77:1}

#�״�������־(1-���� 0-ʹ���� 2-�������)
EaseConnectState=byte=>{Working:0, StartUp:1, ClearCache:2}

#������Ӫ��ID(byte)
NetworkID=byte=>{ALL:0, CMCC:1, CUC:2, CTG:3}

#�ͻ��˲��ŷ�ʽ(byte)
ClientDialType=byte=>{CTNET:0, CTWAP:1}

#�ͻ��˹��ܺ�(byte)
RequestType=byte=>{PageV21:0/*ҳ������:����EASE 2.1*/, Mixed:1/*ҳ�漰��Դ����*/, Page:2/*ҳ������*/, Resource:3/*��Դ����*/, Application:4/*Ӧ������*/, UpdateCenter:5/*���·��������ӵ�ַ*/}

#��������Ӧ��(short 3.2-2.6.9)
StatusCode=short=>{Exception:-1/*�������쳣*/, Success:0/*����������ɹ�*/, Updatable:1/*�������и���,�밴�տͻ��˸��²�������*/}

#��������(byte)
CommandType=byte=>{None:0/*���κβ���*/, SMS:1/*���Ͷ���*/, WAP:2/*����WAP�����*/, Dial:3/*����绰*/, Updatable:4/*��������ڸ��£�����������*/}
