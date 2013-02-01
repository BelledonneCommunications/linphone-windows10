

/* this ALWAYS GENERATED file contains the proxy stub code */


 /* File created by MIDL compiler version 8.00.0595 */
/* at Fri Feb 01 16:16:20 2013
 */
/* Compiler settings for C:\Users\ELVII_~1\AppData\Local\Temp\Linphone.BackEnd.OutOfProcess.idl-119c995b:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=ARM 8.00.0595 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#if defined(_ARM_)


#pragma warning( disable: 4049 )  /* more than 64k source lines */
#if _MSC_VER >= 1200
#pragma warning(push)
#endif

#pragma warning( disable: 4211 )  /* redefine extern to static */
#pragma warning( disable: 4232 )  /* dllimport identity*/
#pragma warning( disable: 4024 )  /* array to pointer mapping*/
#pragma warning( disable: 4152 )  /* function/data pointer conversion in expression */

#define USE_STUBLESS_PROXY


/* verify that the <rpcproxy.h> version is high enough to compile this file*/
#ifndef __REDQ_RPCPROXY_H_VERSION__
#define __REQUIRED_RPCPROXY_H_VERSION__ 475
#endif


#include "rpcproxy.h"
#ifndef __RPCPROXY_H_VERSION__
#error this stub requires an updated version of <rpcproxy.h>
#endif /* __RPCPROXY_H_VERSION__ */


#include "Linphone.BackEnd.OutOfProcess.h"

#define TYPE_FORMAT_STRING_SIZE   25                                
#define PROC_FORMAT_STRING_SIZE   43                                
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   0            

typedef struct _Linphone2EBackEnd2EOutOfProcess_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } Linphone2EBackEnd2EOutOfProcess_MIDL_TYPE_FORMAT_STRING;

typedef struct _Linphone2EBackEnd2EOutOfProcess_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } Linphone2EBackEnd2EOutOfProcess_MIDL_PROC_FORMAT_STRING;

typedef struct _Linphone2EBackEnd2EOutOfProcess_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } Linphone2EBackEnd2EOutOfProcess_MIDL_EXPR_FORMAT_STRING;


static const RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const Linphone2EBackEnd2EOutOfProcess_MIDL_TYPE_FORMAT_STRING Linphone2EBackEnd2EOutOfProcess__MIDL_TypeFormatString;
extern const Linphone2EBackEnd2EOutOfProcess_MIDL_PROC_FORMAT_STRING Linphone2EBackEnd2EOutOfProcess__MIDL_ProcFormatString;
extern const Linphone2EBackEnd2EOutOfProcess_MIDL_EXPR_FORMAT_STRING Linphone2EBackEnd2EOutOfProcess__MIDL_ExprFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_ProxyInfo;



#if !defined(__RPC_ARM32__)
#error  Invalid build platform for this stub.
#endif

#if !(TARGET_IS_NT50_OR_LATER)
#error You need Windows 2000 or later to run this stub because it uses these features:
#error   /robust command line switch.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will fail with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const Linphone2EBackEnd2EOutOfProcess_MIDL_PROC_FORMAT_STRING Linphone2EBackEnd2EOutOfProcess__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure get_CallController */

			0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x6 ),	/* 6 */
/*  8 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 10 */	NdrFcShort( 0x0 ),	/* 0 */
/* 12 */	NdrFcShort( 0x8 ),	/* 8 */
/* 14 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 16 */	0xe,		/* 14 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 18 */	NdrFcShort( 0x0 ),	/* 0 */
/* 20 */	NdrFcShort( 0x0 ),	/* 0 */
/* 22 */	NdrFcShort( 0x0 ),	/* 0 */
/* 24 */	NdrFcShort( 0x2 ),	/* 2 */
/* 26 */	0x2,		/* 2 */
			0x80,		/* 128 */
/* 28 */	0x81,		/* 129 */
			0x0,		/* 0 */

	/* Parameter __returnValue */

/* 30 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 32 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 34 */	NdrFcShort( 0x2 ),	/* Type Offset=2 */

	/* Return value */

/* 36 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 38 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 40 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const Linphone2EBackEnd2EOutOfProcess_MIDL_TYPE_FORMAT_STRING Linphone2EBackEnd2EOutOfProcess__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/*  4 */	NdrFcShort( 0x2 ),	/* Offset= 2 (6) */
/*  6 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/*  8 */	NdrFcLong( 0xc140594a ),	/* -1052747446 */
/* 12 */	NdrFcShort( 0x6bd4 ),	/* 27604 */
/* 14 */	NdrFcShort( 0x3151 ),	/* 12625 */
/* 16 */	0x94,		/* 148 */
			0x4e,		/* 78 */
/* 18 */	0x28,		/* 40 */
			0xc9,		/* 201 */
/* 20 */	0x55,		/* 85 */
			0x63,		/* 99 */
/* 22 */	0x2d,		/* 45 */
			0x15,		/* 21 */

			0x0
        }
    };


/* Standard interface: __MIDL_itf_Linphone2EBackEnd2EOutOfProcess_0000_0000, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IInspectable, ver. 0.0,
   GUID={0xAF86E2E0,0xB12D,0x4c6a,{0x9C,0x5A,0xD7,0xAA,0x65,0x10,0x1E,0x90}} */


/* Object interface: __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals, ver. 0.0,
   GUID={0xEBDEF036,0x447F,0x3D47,{0xB3,0x3F,0x71,0x50,0x68,0xAC,0xD4,0xCC}} */

#pragma code_seg(".orpc")
static const unsigned short __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0
    };

static const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_ProxyInfo =
    {
    &Object_StubDesc,
    Linphone2EBackEnd2EOutOfProcess__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    Linphone2EBackEnd2EOutOfProcess__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(7) ___x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtualsProxyVtbl = 
{
    &__x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_ProxyInfo,
    &IID___x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IInspectable::GetIids */ ,
    0 /* IInspectable::GetRuntimeClassName */ ,
    0 /* IInspectable::GetTrustLevel */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals::get_CallController */
};


static const PRPC_STUB_FUNCTION __x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2
};

CInterfaceStubVtbl ___x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtualsStubVtbl =
{
    &IID___x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals,
    &__x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_ServerInfo,
    7,
    &__x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Standard interface: __MIDL_itf_Linphone2EBackEnd2EOutOfProcess_0000_0001, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */

static const MIDL_STUB_DESC Object_StubDesc = 
    {
    0,
    NdrOleAllocate,
    NdrOleFree,
    0,
    0,
    0,
    0,
    0,
    Linphone2EBackEnd2EOutOfProcess__MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x50002, /* Ndr library version */
    0,
    0x8000253, /* MIDL Version 8.0.595 */
    0,
    0,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0
    };

const CInterfaceProxyVtbl * const _Linphone2EBackEnd2EOutOfProcess_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &___x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtualsProxyVtbl,
    0
};

const CInterfaceStubVtbl * const _Linphone2EBackEnd2EOutOfProcess_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &___x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtualsStubVtbl,
    0
};

PCInterfaceName const _Linphone2EBackEnd2EOutOfProcess_InterfaceNamesList[] = 
{
    "__x_ABI_CLinphone_CBackEnd_COutOfProcess_C____IServerPublicNonVirtuals",
    0
};

const IID *  const _Linphone2EBackEnd2EOutOfProcess_BaseIIDList[] = 
{
    &IID_IInspectable,
    0
};


#define _Linphone2EBackEnd2EOutOfProcess_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _Linphone2EBackEnd2EOutOfProcess, pIID, n)

int __stdcall _Linphone2EBackEnd2EOutOfProcess_IID_Lookup( const IID * pIID, int * pIndex )
{
    
    if(!_Linphone2EBackEnd2EOutOfProcess_CHECK_IID(0))
        {
        *pIndex = 0;
        return 1;
        }

    return 0;
}

const ExtendedProxyFileInfo Linphone2EBackEnd2EOutOfProcess_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _Linphone2EBackEnd2EOutOfProcess_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _Linphone2EBackEnd2EOutOfProcess_StubVtblList,
    (const PCInterfaceName * ) & _Linphone2EBackEnd2EOutOfProcess_InterfaceNamesList,
    (const IID ** ) & _Linphone2EBackEnd2EOutOfProcess_BaseIIDList,
    & _Linphone2EBackEnd2EOutOfProcess_IID_Lookup, 
    1,
    2,
    0, /* table of [async_uuid] interfaces */
    0, /* Filler1 */
    0, /* Filler2 */
    0  /* Filler3 */
};
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif


#endif /* if defined(_ARM_) */

