using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using static UdpTproxyTest.Libc;
using static UdpTproxyTest.CmsgUtils;


namespace UdpTproxyTest {
    public class UDPSocket {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);

        public unsafe void Server(string address, int port) {
            Span<byte> val = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(val, 1);

            _socket.SetRawSocketOption(0, 19, val);
            _socket.SetRawSocketOption(0, 20, val);

            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));

            while (true) {
                Receive();
            }
        }

        private unsafe void Receive() {
            SockAddrIn srcIpAddr;

            var cmbuf = stackalloc byte[100];
            var bytes = stackalloc byte[16*1024];

            iovec iov;
            iov.iov_base = bytes;
            iov.iov_len = (16 * 1024) - 1;
                
            user_msghdr mh;
            mh.msg_name = &srcIpAddr;
            mh.msg_namelen = sizeof(SockAddrIn);

            mh.msg_control = cmbuf;
            mh.msg_controllen = 100;

            mh.msg_iovlen = 1;
            mh.msg_iov = &iov;

            var res = recvmsg((int)_socket.Handle, &mh, 0);

            if (res < 0) {
                Console.WriteLine("smth BAD happened idk");
            } else {
                Console.WriteLine($"From: {new IPEndPoint(srcIpAddr.SinAddr, BinaryPrimitives.ReverseEndianness(srcIpAddr.SinPort))}");

                for (cmsghdr* cmsg = CMSG_FIRSTHDR(mh); cmsg != null; cmsg = CMSG_NEXTHDR(&mh, cmsg)) {
                    if (cmsg->cmsg_level != (int)SocketOptionLevel.IP || cmsg->cmsg_type != 20) continue;

                    var realDst = Marshal.PtrToStructure<SockAddrIn>((nint)CMSG_DATA(cmsg));
                    var dstEp = new IPEndPoint(realDst.SinAddr, BinaryPrimitives.ReverseEndianness(realDst.SinPort));
                    Console.WriteLine($"Real EP: {dstEp}");
                }

                Console.WriteLine("Actual data:");
                Console.WriteLine($"{Convert.ToHexString(new ReadOnlySpan<byte>(bytes, 16*1024).Slice(0, (int)res))}");
            }
        }
    }
}