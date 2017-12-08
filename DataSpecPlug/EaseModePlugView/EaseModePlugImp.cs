using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Net;
using DataSpecTester;
using Gwsoft.Configuration;
using Gwsoft.DataSpec;
using Gwsoft.EaseMode;
using System.Configuration;
using System.Net.Sockets;
#if NUnit
using NUnit.Framework;
#endif

namespace EaseModePlugView
{
    /// <summary>
    /// 易致插件数据查看实现
    /// </summary>
#if NUnit
    [TestFixture]
#endif
    [AppSettingsOptional("EaseGatewayServer.TimeoutSeconds", 30)]
    public class EaseModePlugImp : ITesterPlug
    {

        #region ITesterPlug 成员
        private string GetImpDescription(Type espSubType)
        {
            StringBuilder sb = new StringBuilder();

            object[] attrs = espSubType.GetCustomAttributes(typeof(ImplementStateAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                ImplementStateAttribute imp = (ImplementStateAttribute)attrs[0];

                sb.AppendFormat("{3}完成状态:{0} 接口版本:{1} 【说明:{2} {4}】\r\n",
                imp.ImplementComplete,
                imp.Version,
                imp.Description ?? "无",
                imp.ImplementComplete == CompleteState.OK ? "√ " : "",
                imp.ImplementComplete == CompleteState.OK ? string.Format("发布日期：{0}", imp.ReleaseDateGTM) : ""
                );
            }
            return sb.ToString();
        }

        /// <summary>
        /// 显示选择树中特定节点的详细信息并返回原始字节索引区间
        /// </summary>
        /// <remarks>[OK]</remarks>
        public long[] ReportNodeDetail(TreeView targetTree, TreeNode selectNode, LogWriter rptWriter)
        {
            long[] emptyResult = new long[] { 0, 0 };
            if (rptWriter == null) return emptyResult;

            object nodeBindTag = selectNode.Tag;

            if (nodeBindTag == null || !nodeBindTag.GetType().Equals(typeof(TreeNodeInstanceBind)))
            {
                return emptyResult;
            }
            else
            {
                TreeNodeInstanceBind nodeBind = (TreeNodeInstanceBind)nodeBindTag;

                StringBuilder sb = new StringBuilder();

                Type nodeBindType = nodeBind.NodeItem.GetType();
                if (nodeBindType.IsEnum)
                {
                    sb.AppendFormat("属性名称：{0}, 当前值: {1}，实际值：{2}, 传输长度: {3}字节", selectNode.Name, nodeBind.NodeItem,
                        Convert.ChangeType(nodeBind.NodeItem, Enum.GetUnderlyingType(nodeBindType)),
                        nodeBind.StoreLength);
                }
                else if (nodeBindType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    sb.AppendFormat("{0}", GetImpDescription(nodeBindType));
                    sb.AppendFormat("对象名称：{0}, 当前值: {1}, 传输长度: {2}字节", selectNode.Name, nodeBind.NodeItem, nodeBind.StoreLength);
                }
                else if (nodeBindType.IsArray)
                {
                    int subItemLength = (int)nodeBindType.GetProperty("Length").GetValue(nodeBind.NodeItem, null);
                    if (nodeBind.IsESPData && nodeBind.SubType != null)
                    {
                        sb.AppendFormat("{0}", GetImpDescription(nodeBind.SubType));
                    }
                    sb.AppendFormat("属性名称：{0}, 当前值：{1}, 元素项：{2}, 传输长度: {3}字节", selectNode.Name, nodeBind.NodeItem, subItemLength, nodeBind.StoreLength);
                }
                else
                {
                    if (nodeBind.IsArrayItem && nodeBind.SubType != null)
                    {
                        sb.AppendFormat("{0}", GetImpDescription(nodeBind.SubType));
                    }
                    sb.AppendFormat("属性名称：{0}, 当前值: {1}, 传输长度: {2}字节", selectNode.Name, nodeBind.NodeItem, nodeBind.StoreLength);
                }
                rptWriter(sb.ToString());

                return new long[] { nodeBind.StoreIndex, nodeBind.StoreIndex + nodeBind.StoreLength - 1 };
            }
        }

        LogWriter hostWriter = null;

        void Log(string format, params object[] args)
        {
            if (hostWriter != null)
            {
                hostWriter(format, args);
            }
        }

        void SynHostWriter(LogWriter currentWriter)
        {
            if (hostWriter == null || hostWriter != currentWriter)
            {
                hostWriter = currentWriter;
            }
        }

        long GetTreeNodeStoreIndex(TreeNode currentNode)
        {
            if (currentNode.PrevNode == null)
            {
                if (currentNode.Parent != null)
                {
                    TreeNodeInstanceBind parentNodeBind = (TreeNodeInstanceBind)currentNode.Parent.Tag;
                    if (parentNodeBind != null)
                    {
                        return parentNodeBind.StoreIndex;
                    }
                }
                return 0;
            }
            else
            {
                TreeNodeInstanceBind nodeBind = (TreeNodeInstanceBind)currentNode.PrevNode.Tag;
                return nodeBind.StoreIndex + nodeBind.StoreLength;
            }
        }


        void ReportToTreeView(ESPDataBase instance, TreeNodeCollection targetNodes, LogWriter writer, bool appendRoot)
        {
            targetNodes.Clear();

            Type instanceType = instance.GetType();
            string targetObjectType = instanceType.FullName;

            TreeNode rootNode = null;
            TreeNodeInstanceBind nodeBind = null;
            if (appendRoot)
            {
                rootNode = new TreeNode(targetObjectType);
                rootNode.Name = targetObjectType;
                rootNode.ImageIndex = rootNode.SelectedImageIndex = (int)DataSpecTester.ImageListIcon.Class;

                nodeBind = new TreeNodeInstanceBind
                {
                    IsFirstNode = true,
                    IsArrayItem = false,
                    IsESPData = true,
                    NodeItem = instance,
                    StoreIndex = 0,
                    NodeType = instanceType,
                    StoreLength = instance.GetContentLength()
                };
                rootNode.Tag = nodeBind;
                targetNodes.Add(rootNode);
            }

            ObjectTransferOrderAttribute[] configs = new ObjectTransferOrderAttribute[0];
            PropertyInfo[] transPropertys = SpecUtil.GetTransferProperties(instance.GetType(), out configs);

            PropertyInfo lastProperty = null;
            for (int m = 0, n = transPropertys.Length; m < n; m++)
            {
                PropertyInfo pInfo = transPropertys[m];

                #region 依据属性依次绑定

                TreeNode propertyNode = new TreeNode(string.Format("{0} [{1}]", pInfo.Name, pInfo.PropertyType));
                propertyNode.Name = pInfo.Name;
                propertyNode.ImageIndex = propertyNode.SelectedImageIndex = (int)DataSpecTester.ImageListIcon.Property;

                if (configs[m].Conditional) {
                    propertyNode.Text = propertyNode.Text + " *";
                }

                object propertyVal = pInfo.GetValue(instance, null);

                if (appendRoot && rootNode != null)
                {
                    rootNode.Nodes.Add(propertyNode);
                }
                else
                {
                    targetNodes.Add(propertyNode);
                }

                //if (propertyVal == null) continue;

                nodeBind = new TreeNodeInstanceBind
                {
                    IsFirstNode = lastProperty == null,
                    IsArrayItem = false,
                    IsESPData = false,
                    NodeItem = propertyVal,
                    NodeType = pInfo.PropertyType,
                    StoreIndex = GetTreeNodeStoreIndex(propertyNode),
                    StoreLength = 0  //TODO
                };

                if (pInfo.PropertyType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    ESPDataBase subInstance = (ESPDataBase)propertyVal;
                    nodeBind.IsESPData = true;
                    propertyNode.ImageIndex = propertyNode.SelectedImageIndex = (int)DataSpecTester.ImageListIcon.Class;
                    propertyNode.Tag = nodeBind;
                    if (subInstance != null)
                    {
                        //递归子级属性
                        nodeBind.StoreLength = subInstance.GetContentLength();
                        ReportToTreeView(subInstance, propertyNode.Nodes, writer, false);
                    }
                }
                else
                {
                    if (pInfo.PropertyType.IsEnum)
                    {
                        nodeBind.StoreLength = SpecUtil.GetCLRTypeByteLength(pInfo.PropertyType);

                        propertyNode.ImageIndex = propertyNode.SelectedImageIndex = (int)DataSpecTester.ImageListIcon.Enum;
                        propertyNode.Text = string.Format("{0} [{1}({2})]", pInfo.Name, pInfo.PropertyType, Enum.GetUnderlyingType(pInfo.PropertyType));
                        if (configs[m].Conditional)
                        {
                            propertyNode.Text = propertyNode.Text + " *";
                        }
                        propertyNode.Tag = nodeBind;

                    }
                    else if (pInfo.PropertyType.IsArray)
                    {
                        Type elementType = pInfo.PropertyType.GetElementType();
                        nodeBind.SubType = elementType;
                        propertyNode.Tag = nodeBind;

                        if (elementType.IsSubclassOf(typeof(ESPDataBase)))
                        {
                            #region 封装对象数组
                            ESPDataBase[] subItems = (ESPDataBase[])propertyVal;
                            if (subItems != null)
                            {
                                long totalLength = 0, currentLength = 0;
                                for (int p = 0, q = subItems.Length; p < q; p++)
                                {
                                    currentLength = subItems[p].GetContentLength();
                                    totalLength += currentLength;

                                    TreeNode subPropertyNode = new TreeNode();
                                    subPropertyNode.Name = subItems[p].GetType().FullName + "[" + p + "]";
                                    subPropertyNode.Text = subItems[p].GetType().FullName + "[" + p + "]";

                                    propertyNode.Nodes.Add(subPropertyNode);

                                    subPropertyNode.ImageIndex = subPropertyNode.SelectedImageIndex = (int)DataSpecTester.ImageListIcon.Class;
                                    subPropertyNode.Tag = new TreeNodeInstanceBind
                                                {
                                                    IsFirstNode = p == 0,
                                                    IsArrayItem = true,
                                                    IsESPData = true,
                                                    NodeItem = subItems[p],
                                                    SubType = null,
                                                    NodeType = elementType,
                                                    StoreIndex = GetTreeNodeStoreIndex(subPropertyNode),
                                                    StoreLength = currentLength
                                                };



                                    ReportToTreeView(subItems[p], subPropertyNode.Nodes, writer, false);
                                }
                                nodeBind.StoreLength = totalLength;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 字节数组
                            if (propertyVal == null) propertyVal = new byte[0];
                            if (elementType.Equals(typeof(byte)))
                            {
                                nodeBind.StoreLength = ((byte[])propertyVal).LongLength;

                                if (nodeBind.StoreLength > 0 && (instanceType.Equals(typeof(NetworkSwitchRequest))
                                    || instanceType.Equals(typeof(NetworkSwitchResponse)))
                                 )
                                {
                                    propertyNode.ImageIndex = propertyNode.SelectedImageIndex = (int)DataSpecTester.ImageListIcon.Class;

                                    if (instanceType.Equals(typeof(NetworkSwitchRequest)))
                                    {
                                        #region NetworkSwitchRequest
                                        RequestBase subRequest = ((NetworkSwitchRequest)instance).GetSubRequest();
                                        if (subRequest != null)
                                        {
                                            propertyNode.Text = string.Format("{0} [{1}({2})]", pInfo.Name, pInfo.PropertyType, subRequest.GetType());

                                            nodeBind.IsESPData = true;
                                            nodeBind.SubType = subRequest.GetType();

                                            ReportToTreeView(subRequest, propertyNode.Nodes, writer, false);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        //nodeBind.NodeItem
                                        #region  NetworkSwitchResponse
                                        object currentReqObj = ExchangeGet("Plug_Current_Request");
                                        if (currentReqObj != null)
                                        {
                                            //Trace.WriteLine("NetworkSwitchResponse of " + currentReqObj.GetType());
                                            RequestBase currentRequest = currentReqObj as RequestBase;
                                            if (currentReqObj.GetType().Equals(typeof(NetworkSwitchRequest)))
                                            {
                                                currentRequest = ((NetworkSwitchRequest)currentReqObj).GetSubRequest();
                                            }

                                            if (currentRequest != null)
                                            {
                                                ESPDataBase subResponse = ((NetworkSwitchResponse)instance).GetSubResponse(currentRequest.ESP_Header.ESP_Method);

                                                nodeBind.IsESPData = true;
                                                nodeBind.SubType = subResponse.GetType();

                                                propertyNode.Text = string.Format("{0} [{1}({2})]", pInfo.Name, pInfo.PropertyType, nodeBind.SubType);

                                                ReportToTreeView(subResponse, propertyNode.Nodes, writer, false);

                                            }
                                        }
                                        #endregion
                                    }

                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        nodeBind.StoreLength = SpecUtil.GetCLRTypeByteLength(pInfo.PropertyType);
                        propertyNode.Tag = nodeBind;
                    }
                }

                #endregion

                lastProperty = pInfo;

            }

            if (appendRoot && rootNode != null) rootNode.Expand();
        }

        //HybridStream hs = null;

        private bool TryParseAsDefineType(byte[] espDataBin, Type typeTarget, TreeView tvDisplay, LogWriter writer)
        {
            bool result = true;

            //hs = new HybridStream(espDataBin.AsMemoryStream());

            //StringBuilder diffBuilder = new StringBuilder();
            //StringWriter sw = new StringWriter(diffBuilder);

            string typeName = typeTarget.AssemblyQualifiedName;
            ESPDataBase instance = Activator.CreateInstance(typeTarget) as ESPDataBase;
            try
            {
                if (!ESPDataBase.IsValidInstance(espDataBin, typeTarget, null/*sw*/, out instance))
                {
                    result = false;
                    Log(writer, "解析为{0}无效！", typeName);
                    //Log(writer, "{0}", SpecUtil.ByteArrayToHexString(instance.GetNetworkBytes()));
                    //Log(writer, "{0}", diffBuilder.ToString());

                }
                else
                {
                    Log(writer, "解析为{0}成功！", typeName);
                }
            }
            catch (Exception exp)
            {
                result = false;
                Log(writer, "解析为{0}无效！", typeName);
                instance = Activator.CreateInstance(typeTarget) as ESPDataBase;
                Log(writer, "{0}\r\n{1}", exp.Message, exp.StackTrace);
            }

            //sw.Dispose();
            ReportToTreeView(instance, tvDisplay.Nodes, writer, true);
            return result;
        }

        /// <summary>
        /// 尝试以制定类型解析二进制序列对象
        /// </summary>
        /// <param name="espDataBin">协议二进制数据</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="tvDisplay">对象解析树</param>
        /// <param name="writer">日志输出</param>
        /// <returns></returns>
        public bool TryParseAsType(byte[] espDataBin, string typeName, TreeView tvDisplay, LogWriter writer)
        {
            Log(writer, "开始解析字节序列：长度{0}", espDataBin.Length);
            Type subType = Type.GetType(typeName, false);
            if (subType == null)
            {
                Log(writer, "类型{0}在当前应用程序域未找到！", typeName);
                return false;
            }
            return TryParseAsDefineType(espDataBin, subType, tvDisplay, writer);
        }

        Type[] availableTypes = Type.EmptyTypes;

        Dictionary<string, object> exchangeDict = new Dictionary<string, object>();

        /// <summary>
        /// 为交互应用保存对象
        /// </summary>
        /// <param name="objKey">对象的键值</param>
        /// <param name="storeObj">保存对象的值</param>
        public void ExchangeStore(string objKey, object storeObj)
        {
            if (exchangeDict.ContainsKey(objKey))
            {
                exchangeDict[objKey] = storeObj;
            }
            else
            {
                exchangeDict.Add(objKey, storeObj);
            }
        }

        /// <summary>
        /// 为交互应用获取对象
        /// </summary>
        /// <param name="objKey">获取对象的键值</param>
        /// <returns></returns>
        public object ExchangeGet(string objKey)
        {
            if (exchangeDict.ContainsKey(objKey))
            {
                return exchangeDict[objKey];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 解析二进制数据为树形数据查看
        /// </summary>
        public bool Parse(byte[] espDataBin, TreeView tvDisplay, LogWriter writer, out string returnType)
        {
            Log(writer, "开始解析字节序列：长度{0}", espDataBin.Length);
            SynHostWriter(writer);

            string targetObjectType = string.Empty;
            //availableTypes = new Type[] { typeof(NetworkSwitchRequest), typeof(NetworkSwitchResponse),
            //     typeof(PageV21Request), typeof(PageV21Response),
            //     typeof(MixedRequest), typeof(MixedResponse),
            //     typeof(PageRequest), typeof(PageResponse),
            //     typeof(ResourceRequest), typeof(ResourceResponse),
            //     typeof(ApplicationRequest), typeof(ApplicationResponse),
            //     typeof(GatewayUpdateRequest), typeof(GatewayUpdateResponse)
            //};

            bool blnSuccess = false;
            foreach (Type subType in availableTypes)
            {
                targetObjectType = subType.AssemblyQualifiedName;
                if (TryParseAsDefineType(espDataBin, subType, tvDisplay, writer))
                {
                    blnSuccess = true;
                    break;
                }
                //System.Windows.Forms.Application.DoEvents();
            }

            returnType = targetObjectType;
            return blnSuccess;
        }


        /// <summary>
        /// 获取插件宿主内的子类型集合
        /// </summary>
        /// <param name="baseType">基类型</param>
        /// <returns></returns>
        public Type[] GetPlugHostTypes(Type baseType)
        {
            Type[] retTypes = Type.EmptyTypes;
            List<Type> subTypeList = new List<Type>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Log("从程序集【{0}】中加载从属类型！", asm.FullName);

                foreach (Type t in asm.GetTypes())
                {
                    if (t.IsSubclassOf(baseType) && !t.IsAbstract && t.IsPublic)
                    {
                        subTypeList.Add(t);
                    }
                }
            }

            retTypes = subTypeList.ToArray();

            //按类型名称排序
            Array.Sort(retTypes, (Type t1, Type t2) =>
            {
                return string.Compare(t1.FullName, t2.FullName);
            });

            availableTypes = retTypes;

            return retTypes;
        }

        /// <summary>
        /// 设置跟踪输出口
        /// </summary>
        /// <param name="writer"></param>
        public void SetTraceWriter(LogWriter writer)
        {
            hostWriter = writer;
        }

        #endregion

        #region 插件解析支持
        #endregion

        #region 插件测试支持
        /// <summary>
        /// 获取远程测试返回数据
        /// </summary>
        /// <remarks>[TODO]</remarks>
        public byte[] GetTestResponse(byte[] espRequestData, LogWriter writer)
        {
            string easeGateWayServer = ConfigurationManager.AppSettings["EaseGatewayServer"];
            if (string.IsNullOrEmpty(easeGateWayServer))
            {
                throw new ConfigurationErrorsException("AppSettings节点中没有名为EaseGatewayServer，值形如：192.168.8.119:9000的配置！");
            }
            else
            {
                string serverAddress = "localhost";
                int serverPort = 9000;
                int idx = easeGateWayServer.IndexOf(':');
                if (idx != -1)
                {
                    serverAddress = easeGateWayServer.Substring(0, idx);
                    serverPort = Convert.ToInt32(easeGateWayServer.Substring(idx + 1));
                }
                else
                {
                    serverAddress = easeGateWayServer;
                }

                //MemoryStream returnStm = new MemoryStream();

                #region 远程Socket连接测试
                TcpClient simpleTcp = null;
                NetworkStream tcpStream = null;
                byte[] retBytes = new byte[0];

                try
                {
                    simpleTcp = new TcpClient(serverAddress, serverPort);
                    tcpStream = simpleTcp.GetStream();
                    tcpStream.WriteTimeout = AppSettingsOptionalAttribute.SettingValueOrDefault<EaseModePlugImp, int>() * 1000; //30秒
                    tcpStream.ReadTimeout = AppSettingsOptionalAttribute.SettingValueOrDefault<EaseModePlugImp, int>() * 1000; //30秒

                    Log(writer, "*向{1}:{2}发送二进制序列，长度:{0}...", espRequestData.Length, serverAddress, serverPort);
                    tcpStream.Write(espRequestData, 0, espRequestData.Length);

                    if (ExchangeGet("Plug_Current_ReadObjectFromStream") != null && Convert.ToBoolean(ExchangeGet("Plug_Current_ReadObjectFromStream")))
                    {
                        Log(writer, "直接读取为网关回应对象打开");

                        NetworkSwitchResponse resp = new NetworkSwitchResponse();
                        resp.BindFromNetworkStream(tcpStream, 0, false);
                        retBytes = resp.GetNetworkBytes();
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        byte[] buffer = new byte[20480];
                        int rc = 0;

                        rc = tcpStream.Read(buffer, 0, buffer.Length);
                        int totalRead = 10, currentRead = 0;
                        if (rc > 10 && SpecUtil.BytesStartWith(buffer, new byte[] { 0x03, 0xF2 }))
                        {
                            byte[] leaveLengthBytes = new byte[4];
                            Buffer.BlockCopy(buffer, 6, leaveLengthBytes, 0, leaveLengthBytes.Length);

                            totalRead = BitConverter.ToInt32(leaveLengthBytes.ReverseBytes(), 0);
                            currentRead = rc - 10;

                            ms.Write(buffer, 0, rc);
                            Log(writer, "剩余字节长度{0},本次读取字节长度{1}.", totalRead, rc);
                        }

                        while (simpleTcp.Connected && tcpStream.CanRead &&
                            currentRead < totalRead &&
                            (rc = tcpStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, rc);
                            currentRead += rc;
                            Log(writer, "{1}:{2}返回二进制序列，长度:{0}...", rc, serverAddress, serverPort);
                        }

                        retBytes = ms.ToArray();
                        ms.Dispose();

                    }
                    Log(writer, "√ 处理{1}:{2}完成，返回二进制序列，总长度:{0}.", retBytes.Length, serverAddress, serverPort);
                }
                catch (SocketException err)
                {
                    Log(writer, "TCP client: Socket error occured: {0}", err.Message);
                }
                catch (System.IO.IOException err)
                {
                    Log(writer, "TCP client: I/O error: {0} \r\n{1}", err.Message, err.StackTrace);
                }
                finally
                {
                    Log(writer, "释放连接相关资源.");
                    if (tcpStream != null)
                        tcpStream.Close();
                    if (simpleTcp != null)
                        simpleTcp.Close();
                }
                #endregion

                return retBytes;
            }
        }
        #endregion

        void Log(LogWriter writer, string format, params object[] args)
        {
            if (writer != null)
            {
                //writer.BeginInvoke(format, args, null, null);
                writer(format, args);
            }
        }

        /// <summary>
        /// 修改绑定树的实例之后更新绑定
        /// </summary>
        public void RefreshTreeView(TreeView targetTree, LogWriter rptWriter)
        {
            
        }

        /// <summary>
        /// 修改节点绑定之后的更新
        /// </summary>
        public void RefreshSelectNode(TreeNode selectNode, LogWriter rptWriter)
        {
            TreeNodeInstanceBind bind = selectNode.Tag as TreeNodeInstanceBind;
            if (bind == null)
            {
                Log(rptWriter,  "节点绑定Tag对象类型不为{0}，操作不支持！", typeof(TreeNodeInstanceBind).FullName);
            }
            else
            {
                if (bind.NodeType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    ESPDataBase instance = bind.NodeItem as ESPDataBase;
                    if (instance != null)
                    {
                        ReportToTreeView(instance, selectNode.Nodes, rptWriter, false);
                    }
                }
            }
        }

#if NUnit
        [Test]
        public void AppDownTest()
        {
            byte[] testBytes = SpecUtil.HexPatternStringToByteArray(@"C8 00 
00 00 1C 00 00 27 14 00 64 00 0B 00 00 0E 52 00 
00 00 27 03 00 00 00 00 00 00 00 00 00 00 01");

            //HttpClient client = new HttpClient();
            //client.UploadData("http://118.123.205.165:8082/App/Post.ashx", testBytes);
            //foreach (string key in client.ResponseHeaders.AllKeys)
            //{
            //    Console.WriteLine("{0}={1}", key, client.ResponseHeaders[key]);
            //}
            //client.Dispose();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://118.123.205.165:8082/App/Post.ashx");
            request.Method = "POST";
            request.ContentLength = testBytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";// "application/ease-appdown";
            Stream swReq = request.GetRequestStream();

            swReq.Write(testBytes, 0, testBytes.Length);
            swReq.Close();
            swReq.Dispose();

            HttpWebResponse Resp = null;
            try
            {
                Resp = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Resp = (HttpWebResponse)ex.Response;
            }


            foreach (string key in Resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", key, Resp.Headers[key]);
            }

            Stream respSw = Resp.GetResponseStream();

            List<byte> bytList = new List<byte>();
            int rdByte = 0;
            while ((rdByte = respSw.ReadByte()) != -1)
            {
                bytList.Add((byte)rdByte);
            }

            respSw.Close();
            respSw.Dispose();

            Console.WriteLine("Length:{0}\r\nRAW:{1}", bytList.Count, SpecUtil.ByteArrayToHexString(bytList.ToArray()));

            Resp.Close();
        }

        [Test]
        public void SpecUtilTest()
        {
            byte[] testBytes1 = new byte[] { 0x03, 0xF2, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x6A, 0x00, 0x03 };
            byte[] testBytes2 = new byte[] { 0x03, 0xF2, 0x00, 0x00, 0x00, 0x02 };
            byte[] testBytes3 = new byte[] { 0x00, 0x00, 0x00, 0x6A, 0x00, 0x03 };

            Assert.That(SpecUtil.BytesStartWith(testBytes1, testBytes2));
            Assert.That(SpecUtil.BytesStartWith(testBytes1, testBytes3, testBytes2.Length));

            Assert.That(SpecUtil.AreEqual(SpecUtil.TrimStart(testBytes1, testBytes2), testBytes3));
            Assert.That(SpecUtil.AreEqual(SpecUtil.AddPrefix(testBytes3, testBytes2), testBytes1));


        }

#endif
    }



}
