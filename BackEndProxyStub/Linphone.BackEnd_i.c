

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.00.0595 */
/* at Fri Feb 01 16:16:19 2013
 */
/* Compiler settings for C:\Users\ELVII_~1\AppData\Local\Temp\Linphone.BackEnd.idl-47c5eda2:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=ARM 8.00.0595 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


#ifdef __cplusplus
extern "C"{
#endif 


#include <rpc.h>
#include <rpcndr.h>

#ifdef _MIDL_USE_GUIDDEF_

#ifndef INITGUID
#define INITGUID
#include <guiddef.h>
#undef INITGUID
#else
#include <guiddef.h>
#endif

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        DEFINE_GUID(name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)

#else // !_MIDL_USE_GUIDDEF_

#ifndef __IID_DEFINED__
#define __IID_DEFINED__

typedef struct _IID
{
    unsigned long x;
    unsigned short s1;
    unsigned short s2;
    unsigned char  c[8];
} IID;

#endif // __IID_DEFINED__

#ifndef CLSID_DEFINED
#define CLSID_DEFINED
typedef IID CLSID;
#endif // CLSID_DEFINED

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

#endif !_MIDL_USE_GUIDDEF_

MIDL_DEFINE_GUID(IID, IID___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals,0xC140594A,0x6BD4,0x3151,0x94,0x4E,0x28,0xC9,0x55,0x63,0x2D,0x15);


MIDL_DEFINE_GUID(IID, IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals,0x74C65E76,0x06C7,0x38A6,0x9E,0x0A,0x61,0xE3,0x73,0x9B,0x7E,0x1E);


MIDL_DEFINE_GUID(IID, IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics,0xCE34112C,0xC406,0x3477,0xBB,0xB4,0x7E,0x1F,0x99,0xEE,0x77,0x6C);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif


