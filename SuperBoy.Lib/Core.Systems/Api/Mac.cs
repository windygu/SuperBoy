using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNet.Utilities
{
 /// <summary>
 /// Mac 的摘要说明。
 /// </summary>
 public class Mac
 {
  public enum Ncbconst 
  { 
   Ncbnamsz   =16,      /* absolute length of a net name         */ 
   MaxLana   =254,      /* lana's in range 0 to MAX_LANA inclusive   */ 
   Ncbenum      =0x37,      /* NCB ENUMERATE LANA NUMBERS            */ 
   NrcGoodret   =0x00,      /* good return                              */ 
   Ncbreset    =0x32,      /* NCB RESET                        */ 
   Ncbastat    =0x33,      /* NCB ADAPTER STATUS                  */ 
   NumNamebuf =30,      /* Number of NAME's BUFFER               */ 
  }
  [StructLayout(LayoutKind.Sequential)] 
   public struct AdapterStatus 
  { 
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=6)] 
   public byte[] adapter_address; 
   public byte   rev_major;  
   public byte   reserved0;  
   public byte   adapter_type;  
   public byte   rev_minor;  
   public ushort    duration;  
   public ushort    frmr_recv;  
   public ushort    frmr_xmit;  
   public ushort    iframe_recv_err;  
   public ushort    xmit_aborts;  
   public uint   xmit_success;  
   public uint   recv_success;  
   public ushort    iframe_xmit_err;  
   public ushort    recv_buff_unavail;  
   public ushort    t1_timeouts;  
   public ushort    ti_timeouts;  
   public uint   reserved1;  
   public ushort    free_ncbs;  
   public ushort    max_cfg_ncbs;  
   public ushort    max_ncbs;  
   public ushort    xmit_buf_unavail;  
   public ushort    max_dgram_size;  
   public ushort    pending_sess;  
   public ushort    max_cfg_sess;  
   public ushort    max_sess;  
   public ushort    max_sess_pkt_size;  
   public ushort    name_count; 
  } 

  [StructLayout(LayoutKind.Sequential)] 
   public struct NameBuffer 
  {  
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=(int)Ncbconst.Ncbnamsz)] 
   public byte[] name; 
   public byte name_num;  
   public byte name_flags;  
  } 

  [StructLayout(LayoutKind.Sequential)] 
   public struct Ncb 
  {  
   public byte  ncb_command;  
   public byte  ncb_retcode;  
   public byte  ncb_lsn;  
   public byte  ncb_num;  
   public IntPtr ncb_buffer;  
   public ushort ncb_length;  
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=(int)Ncbconst.Ncbnamsz)] 
   public byte[]  ncb_callname;  
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=(int)Ncbconst.Ncbnamsz)] 
   public byte[]  ncb_name;  
   public byte  ncb_rto;  
   public byte  ncb_sto;  
   public IntPtr ncb_post;  
   public byte  ncb_lana_num;  
   public byte  ncb_cmd_cplt;  
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=10)] 
   public byte[] ncb_reserve; 
   public IntPtr ncb_event; 
  } 

  [StructLayout(LayoutKind.Sequential)] 
   public struct LanaEnum 
  {  
   public byte length;  
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=(int)Ncbconst.MaxLana)] 
   public byte[] lana; 
  } 

  [StructLayout(LayoutKind.Auto)] 
   public struct Astat 
  {  
   public AdapterStatus adapt; 
   [MarshalAs(UnmanagedType.ByValArray, SizeConst=(int)Ncbconst.NumNamebuf)] 
   public NameBuffer[] NameBuff; 
  } 
  public class Win32Api 
  { 
   [DllImport("NETAPI32.DLL")] 
   public static extern char Netbios(ref Ncb ncb); 
  } 

  
  public static string GetMacAddress() 
  { 
   var addr=""; 
   int cb; 
   Astat adapter; 
   var ncb=new Ncb(); 
   char uRetCode; 
   LanaEnum lenum; 

   ncb.ncb_command = (byte)Ncbconst.Ncbenum; 
   cb = Marshal.SizeOf(typeof(LanaEnum)); 
   ncb.ncb_buffer = Marshal.AllocHGlobal(cb); 
   ncb.ncb_length = (ushort)cb; 
   uRetCode = Win32Api.Netbios(ref ncb); 
   lenum = (LanaEnum)Marshal.PtrToStructure(ncb.ncb_buffer, typeof(LanaEnum)); 
   Marshal.FreeHGlobal(ncb.ncb_buffer); 
   if(uRetCode != (short)Ncbconst.NrcGoodret) 
    return ""; 

   for(var i=0; i < lenum.length ;i++) 
   { 
    ncb.ncb_command = (byte)Ncbconst.Ncbreset; 
    ncb.ncb_lana_num = lenum.lana[i]; 
    uRetCode = Win32Api.Netbios(ref ncb); 
    if(uRetCode != (short)Ncbconst.NrcGoodret) 
     return ""; 

    ncb.ncb_command = (byte)Ncbconst.Ncbastat; 
    ncb.ncb_lana_num = lenum.lana[i]; 
    ncb.ncb_callname[0]=(byte)'*'; 
    cb = Marshal.SizeOf(typeof(AdapterStatus)) + Marshal.SizeOf(typeof(NameBuffer))*(int)Ncbconst.NumNamebuf; 
    ncb.ncb_buffer = Marshal.AllocHGlobal(cb); 
    ncb.ncb_length = (ushort)cb; 
    uRetCode = Win32Api.Netbios(ref ncb); 
    adapter.adapt = (AdapterStatus)Marshal.PtrToStructure(ncb.ncb_buffer, typeof(AdapterStatus)); 
    Marshal.FreeHGlobal(ncb.ncb_buffer); 

    if (uRetCode == (short)Ncbconst.NrcGoodret) 
    { 
     if(i>0) 
      addr += "-"; 
      addr = string.Format("{0,2:X}-{1,2:X}-{2,2:X}-{3,2:X}-{4,2:X}-{5,2:X}", 
      adapter.adapt.adapter_address[0], 
      adapter.adapt.adapter_address[1], 
      adapter.adapt.adapter_address[2], 
      adapter.adapt.adapter_address[3], 
      adapter.adapt.adapter_address[4], 
      adapter.adapt.adapter_address[5]); 
    } 
   } 
   return addr; 
  }
 }
}
