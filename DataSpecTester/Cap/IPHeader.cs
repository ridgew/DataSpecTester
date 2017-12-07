using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DataSpecTester.Cap
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/IPv4#Data
    /// http://en.wikipedia.org/wiki/List_of_IP_protocol_numbers
    /// </summary>
    public enum IpProtocol : byte
    {
        /// <summary>
        /// Reserved
        /// </summary>
        Unknown = 255,

        /// <summary>
        /// Internet Control Message Protocol
        /// </summary>
        ICMP = 1,

        /// <summary>
        /// Internet Group Management Protocol
        /// </summary>
        IGMP = 2,

        /// <summary>
        /// Transmission Control Protocol
        /// </summary>
        TCP = 6,

        /// <summary>
        /// User Datagram Protocol
        /// </summary>
        UDP = 17,

        /// <summary>
        /// Open Shortest Path First
        /// </summary>
        OSPF = 89,

        /// <summary>
        /// Stream Control Transmission Protocol
        /// </summary>
        SCTP = 132


    }

    //http://www.codeproject.com/KB/IP/CSNetworkSniffer.aspx
    /// <summary>
    /// 
    /// </summary>
    public class IPHeader
    {
        //IP Header fields
        private byte byVersionAndHeaderLength;   //Eight bits for version and header length
        private byte byDifferentiatedServices;    //Eight bits for differentiated services (TOS)
        private ushort usTotalLength;              //Sixteen bits for total length of the datagram (header + message)
        private ushort usIdentification;           //Sixteen bits for identification
        private ushort usFlagsAndOffset;           //Eight bits for flags and fragmentation offset
        private byte byTTL;                      //Eight bits for TTL (Time To Live)
        private byte byProtocol;                 //Eight bits for the underlying protocol
        private short sChecksum;                  //Sixteen bits containing the checksum of the header
        //(checksum can be negative so taken as short)
        private uint uiSourceIPAddress;          //Thirty two bit source IP Address
        private uint uiDestinationIPAddress;     //Thirty two bit destination IP Address
        //End IP Header fields

        private byte byHeaderLength;             //Header length

        public IPHeader(byte[] byBuffer, int nReceived)
        {

            try
            {
                //Create MemoryStream out of the received bytes
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                //Next we create a BinaryReader out of the MemoryStream
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                //The first eight bits of the IP header contain the version and
                //header length so we read them
                byVersionAndHeaderLength = binaryReader.ReadByte();

                //The next eight bits contain the Differentiated services
                byDifferentiatedServices = binaryReader.ReadByte();

                //Next eight bits hold the total length of the datagram
                usTotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //Next sixteen have the identification bytes
                usIdentification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //Next sixteen bits contain the flags and fragmentation offset
                usFlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //Next eight bits have the TTL value
                byTTL = binaryReader.ReadByte();

                //Next eight represnts the protocol encapsulated in the datagram
                byProtocol = binaryReader.ReadByte();

                //Next sixteen bits contain the checksum of the header
                sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //Next thirty two bits have the source IP address
                uiSourceIPAddress = (uint)(binaryReader.ReadInt32());

                //Next thirty two hold the destination IP address
                uiDestinationIPAddress = (uint)(binaryReader.ReadInt32());

                //Now we calculate the header length

                byHeaderLength = byVersionAndHeaderLength;
                //The last four bits of the version and header length field contain the
                //header length, we perform some simple binary airthmatic operations to
                //extract them
                byHeaderLength <<= 4;
                byHeaderLength >>= 4;
                //Multiply by four to get the exact header length
                byHeaderLength *= 4;

                Data = new MemoryStream(byBuffer, byHeaderLength,
                    usTotalLength - byHeaderLength).ToArray();

                if (MessageLength > 4)
                {
                    //The first sixteen bits contain the source port
                    SourcePort = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 0));

                    //The next sixteen bits contain the destination port
                    DestinationPort = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 2));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MJsniffer", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Դ�˿�
        /// </summary>
        public ushort SourcePort { get; set; }

        /// <summary>
        /// Ŀ��˿�
        /// </summary>
        public ushort DestinationPort { get; set; }

        public string Version
        {
            get
            {
                //Calculate the IP version

                //The four bits of the IP header contain the IP version
                if ((byVersionAndHeaderLength >> 4) == 4)
                {
                    return "IP v4";
                }
                else if ((byVersionAndHeaderLength >> 4) == 6)
                {
                    return "IP v6";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string HeaderLength
        {
            get
            {
                return byHeaderLength.ToString();
            }
        }

        public ushort MessageLength
        {
            get
            {
                //MessageLength = Total length of the datagram - Header length
                return (ushort)(usTotalLength - byHeaderLength);
            }
        }

        public string DifferentiatedServices
        {
            get
            {
                //Returns the differentiated services in hexadecimal format
                return string.Format("0x{0:x2} ({1})", byDifferentiatedServices,
                    byDifferentiatedServices);
            }
        }

        public string Flags
        {
            get
            {
                //The first three bits of the flags and fragmentation field 
                //represent the flags (which indicate whether the data is 
                //fragmented or not)
                int nFlags = usFlagsAndOffset >> 13;
                if (nFlags == 2)
                {
                    return "Don't fragment";
                }
                else if (nFlags == 1)
                {
                    return "More fragments to come";
                }
                else
                {
                    return nFlags.ToString();
                }
            }
        }

        public string FragmentationOffset
        {
            get
            {
                //The last thirteen bits of the flags and fragmentation field 
                //contain the fragmentation offset
                int nOffset = usFlagsAndOffset << 3;
                nOffset >>= 3;

                return nOffset.ToString();
            }
        }

        public string TTL
        {
            get
            {
                return byTTL.ToString();
            }
        }

        public IpProtocol ProtocolType
        {
            get
            {
                //The protocol field represents the protocol in the data portion
                //of the datagram
                if (byProtocol == 6)        //A value of six represents the TCP protocol
                {
                    return IpProtocol.TCP;
                }
                else if (byProtocol == 17)  //Seventeen for UDP
                {
                    return IpProtocol.UDP;
                }
                else
                {
                    return IpProtocol.Unknown;
                }
            }
        }

        public string Checksum
        {
            get
            {
                //Returns the checksum in hexadecimal format
                return string.Format("0x{0:x2}", sChecksum);
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(uiSourceIPAddress);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(uiDestinationIPAddress);
            }
        }

        public string TotalLength
        {
            get
            {
                return usTotalLength.ToString();
            }
        }

        public string Identification
        {
            get
            {
                return usIdentification.ToString();
            }
        }


        public byte[] Data
        {
            //get
            //{
            //    int realLen = MessageLength;
            //    if (byIPData.Length > realLen)
            //    {
            //        byte[] buffer = new byte[realLen];
            //        Buffer.BlockCopy(byIPData, 0, buffer, 0, buffer.Length);
            //        byIPData = buffer;
            //    }
            //    return byIPData;
            //}
            get;
            private set;
        }
    }
}
