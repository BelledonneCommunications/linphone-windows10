

/* this ALWAYS GENERATED file contains the proxy stub code */


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


#include "Linphone.BackEnd.h"

#define TYPE_FORMAT_STRING_SIZE   131                               
#define PROC_FORMAT_STRING_SIZE   305                               
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   1            

typedef struct _Linphone2EBackEnd_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } Linphone2EBackEnd_MIDL_TYPE_FORMAT_STRING;

typedef struct _Linphone2EBackEnd_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } Linphone2EBackEnd_MIDL_PROC_FORMAT_STRING;

typedef struct _Linphone2EBackEnd_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } Linphone2EBackEnd_MIDL_EXPR_FORMAT_STRING;


static const RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const Linphone2EBackEnd_MIDL_TYPE_FORMAT_STRING Linphone2EBackEnd__MIDL_TypeFormatString;
extern const Linphone2EBackEnd_MIDL_PROC_FORMAT_STRING Linphone2EBackEnd__MIDL_ProcFormatString;
extern const Linphone2EBackEnd_MIDL_EXPR_FORMAT_STRING Linphone2EBackEnd__MIDL_ExprFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_ProxyInfo;


extern const USER_MARSHAL_ROUTINE_QUADRUPLE UserMarshalRoutines[ WIRE_MARSHAL_TABLE_SIZE ];

#if !defined(__RPC_ARM32__)
#error  Invalid build platform for this stub.
#endif

#if !(TARGET_IS_NT50_OR_LATER)
#error You need Windows 2000 or later to run this stub because it uses these features:
#error   /robust command line switch.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will fail with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const Linphone2EBackEnd_MIDL_PROC_FORMAT_STRING Linphone2EBackEnd__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure StartServer */

			0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x6 ),	/* 6 */
/*  8 */	NdrFcShort( 0x10 ),	/* ARM Stack size/offset = 16 */
/* 10 */	NdrFcShort( 0x8 ),	/* 8 */
/* 12 */	NdrFcShort( 0x8 ),	/* 8 */
/* 14 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 16 */	0xe,		/* 14 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 18 */	NdrFcShort( 0x0 ),	/* 0 */
/* 20 */	NdrFcShort( 0x1 ),	/* 1 */
/* 22 */	NdrFcShort( 0x0 ),	/* 0 */
/* 24 */	NdrFcShort( 0x3 ),	/* 3 */
/* 26 */	0x3,		/* 3 */
			0x80,		/* 128 */
/* 28 */	0x81,		/* 129 */
			0x82,		/* 130 */

	/* Parameter __outOfProcServerClassNamesSize */

/* 30 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 32 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 34 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter outOfProcServerClassNames */

/* 36 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 38 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 40 */	NdrFcShort( 0x2a ),	/* Type Offset=42 */

	/* Return value */

/* 42 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 44 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 46 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure DoPeriodicKeepAlive */

/* 48 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 50 */	NdrFcLong( 0x0 ),	/* 0 */
/* 54 */	NdrFcShort( 0x7 ),	/* 7 */
/* 56 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 58 */	NdrFcShort( 0x0 ),	/* 0 */
/* 60 */	NdrFcShort( 0x8 ),	/* 8 */
/* 62 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x1,		/* 1 */
/* 64 */	0xc,		/* 12 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 66 */	NdrFcShort( 0x0 ),	/* 0 */
/* 68 */	NdrFcShort( 0x0 ),	/* 0 */
/* 70 */	NdrFcShort( 0x0 ),	/* 0 */
/* 72 */	NdrFcShort( 0x1 ),	/* 1 */
/* 74 */	0x1,		/* 1 */
			0x80,		/* 128 */

	/* Return value */

/* 76 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 78 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 80 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_CallController */

/* 82 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 84 */	NdrFcLong( 0x0 ),	/* 0 */
/* 88 */	NdrFcShort( 0x8 ),	/* 8 */
/* 90 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 92 */	NdrFcShort( 0x0 ),	/* 0 */
/* 94 */	NdrFcShort( 0x8 ),	/* 8 */
/* 96 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 98 */	0xe,		/* 14 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 100 */	NdrFcShort( 0x0 ),	/* 0 */
/* 102 */	NdrFcShort( 0x0 ),	/* 0 */
/* 104 */	NdrFcShort( 0x0 ),	/* 0 */
/* 106 */	NdrFcShort( 0x2 ),	/* 2 */
/* 108 */	0x2,		/* 2 */
			0x80,		/* 128 */
/* 110 */	0x81,		/* 129 */
			0x0,		/* 0 */

	/* Parameter __returnValue */

/* 112 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 114 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 116 */	NdrFcShort( 0x40 ),	/* Type Offset=64 */

	/* Return value */

/* 118 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 120 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 122 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure GetCurrentProcessId */

/* 124 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 126 */	NdrFcLong( 0x0 ),	/* 0 */
/* 130 */	NdrFcShort( 0x6 ),	/* 6 */
/* 132 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 134 */	NdrFcShort( 0x0 ),	/* 0 */
/* 136 */	NdrFcShort( 0x24 ),	/* 36 */
/* 138 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 140 */	0xe,		/* 14 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 142 */	NdrFcShort( 0x0 ),	/* 0 */
/* 144 */	NdrFcShort( 0x0 ),	/* 0 */
/* 146 */	NdrFcShort( 0x0 ),	/* 0 */
/* 148 */	NdrFcShort( 0x2 ),	/* 2 */
/* 150 */	0x2,		/* 2 */
			0x80,		/* 128 */
/* 152 */	0x81,		/* 129 */
			0x0,		/* 0 */

	/* Parameter __returnValue */

/* 154 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 156 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 158 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 160 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 162 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 164 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure GetUiDisconnectedEventName */

/* 166 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 168 */	NdrFcLong( 0x0 ),	/* 0 */
/* 172 */	NdrFcShort( 0x7 ),	/* 7 */
/* 174 */	NdrFcShort( 0x10 ),	/* ARM Stack size/offset = 16 */
/* 176 */	NdrFcShort( 0x8 ),	/* 8 */
/* 178 */	NdrFcShort( 0x8 ),	/* 8 */
/* 180 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 182 */	0xe,		/* 14 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 184 */	NdrFcShort( 0x1 ),	/* 1 */
/* 186 */	NdrFcShort( 0x0 ),	/* 0 */
/* 188 */	NdrFcShort( 0x0 ),	/* 0 */
/* 190 */	NdrFcShort( 0x3 ),	/* 3 */
/* 192 */	0x3,		/* 3 */
			0x80,		/* 128 */
/* 194 */	0x81,		/* 129 */
			0x82,		/* 130 */

	/* Parameter backgroundProcessId */

/* 196 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 198 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 200 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter __returnValue */

/* 202 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 204 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 206 */	NdrFcShort( 0x62 ),	/* Type Offset=98 */

	/* Return value */

/* 208 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 210 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 212 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure GetBackgroundProcessReadyEventName */

/* 214 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 216 */	NdrFcLong( 0x0 ),	/* 0 */
/* 220 */	NdrFcShort( 0x8 ),	/* 8 */
/* 222 */	NdrFcShort( 0x10 ),	/* ARM Stack size/offset = 16 */
/* 224 */	NdrFcShort( 0x8 ),	/* 8 */
/* 226 */	NdrFcShort( 0x8 ),	/* 8 */
/* 228 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 230 */	0xe,		/* 14 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 232 */	NdrFcShort( 0x1 ),	/* 1 */
/* 234 */	NdrFcShort( 0x0 ),	/* 0 */
/* 236 */	NdrFcShort( 0x0 ),	/* 0 */
/* 238 */	NdrFcShort( 0x3 ),	/* 3 */
/* 240 */	0x3,		/* 3 */
			0x80,		/* 128 */
/* 242 */	0x81,		/* 129 */
			0x82,		/* 130 */

	/* Parameter backgroundProcessId */

/* 244 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 246 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 248 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter __returnValue */

/* 250 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 252 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 254 */	NdrFcShort( 0x62 ),	/* Type Offset=98 */

	/* Return value */

/* 256 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 258 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 260 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_Instance */

/* 262 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 264 */	NdrFcLong( 0x0 ),	/* 0 */
/* 268 */	NdrFcShort( 0x9 ),	/* 9 */
/* 270 */	NdrFcShort( 0xc ),	/* ARM Stack size/offset = 12 */
/* 272 */	NdrFcShort( 0x0 ),	/* 0 */
/* 274 */	NdrFcShort( 0x8 ),	/* 8 */
/* 276 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 278 */	0xe,		/* 14 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 280 */	NdrFcShort( 0x0 ),	/* 0 */
/* 282 */	NdrFcShort( 0x0 ),	/* 0 */
/* 284 */	NdrFcShort( 0x0 ),	/* 0 */
/* 286 */	NdrFcShort( 0x2 ),	/* 2 */
/* 288 */	0x2,		/* 2 */
			0x80,		/* 128 */
/* 290 */	0x81,		/* 129 */
			0x0,		/* 0 */

	/* Parameter __returnValue */

/* 292 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 294 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 296 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Return value */

/* 298 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 300 */	NdrFcShort( 0x8 ),	/* ARM Stack size/offset = 8 */
/* 302 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const Linphone2EBackEnd_MIDL_TYPE_FORMAT_STRING Linphone2EBackEnd__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x11, 0x0,	/* FC_RP */
/*  4 */	NdrFcShort( 0x26 ),	/* Offset= 38 (42) */
/*  6 */	
			0x12, 0x0,	/* FC_UP */
/*  8 */	NdrFcShort( 0xe ),	/* Offset= 14 (22) */
/* 10 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 12 */	NdrFcShort( 0x2 ),	/* 2 */
/* 14 */	0x9,		/* Corr desc: FC_ULONG */
			0x0,		/*  */
/* 16 */	NdrFcShort( 0xfffc ),	/* -4 */
/* 18 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 20 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 22 */	
			0x17,		/* FC_CSTRUCT */
			0x3,		/* 3 */
/* 24 */	NdrFcShort( 0x8 ),	/* 8 */
/* 26 */	NdrFcShort( 0xfff0 ),	/* Offset= -16 (10) */
/* 28 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 30 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 32 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 34 */	NdrFcShort( 0x0 ),	/* 0 */
/* 36 */	NdrFcShort( 0x4 ),	/* 4 */
/* 38 */	NdrFcShort( 0x0 ),	/* 0 */
/* 40 */	NdrFcShort( 0xffde ),	/* Offset= -34 (6) */
/* 42 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 44 */	NdrFcShort( 0x0 ),	/* 0 */
/* 46 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 48 */	NdrFcShort( 0x4 ),	/* ARM Stack size/offset = 4 */
/* 50 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 52 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 56 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 58 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 60 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (32) */
/* 62 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 64 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 66 */	NdrFcShort( 0x2 ),	/* Offset= 2 (68) */
/* 68 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 70 */	NdrFcLong( 0xc140594a ),	/* -1052747446 */
/* 74 */	NdrFcShort( 0x6bd4 ),	/* 27604 */
/* 76 */	NdrFcShort( 0x3151 ),	/* 12625 */
/* 78 */	0x94,		/* 148 */
			0x4e,		/* 78 */
/* 80 */	0x28,		/* 40 */
			0xc9,		/* 201 */
/* 82 */	0x55,		/* 85 */
			0x63,		/* 99 */
/* 84 */	0x2d,		/* 45 */
			0x15,		/* 21 */
/* 86 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 88 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 90 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 92 */	NdrFcShort( 0x6 ),	/* Offset= 6 (98) */
/* 94 */	
			0x13, 0x0,	/* FC_OP */
/* 96 */	NdrFcShort( 0xffb6 ),	/* Offset= -74 (22) */
/* 98 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 100 */	NdrFcShort( 0x0 ),	/* 0 */
/* 102 */	NdrFcShort( 0x4 ),	/* 4 */
/* 104 */	NdrFcShort( 0x0 ),	/* 0 */
/* 106 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (94) */
/* 108 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 110 */	NdrFcShort( 0x2 ),	/* Offset= 2 (112) */
/* 112 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 114 */	NdrFcLong( 0x74c65e76 ),	/* 1959157366 */
/* 118 */	NdrFcShort( 0x6c7 ),	/* 1735 */
/* 120 */	NdrFcShort( 0x38a6 ),	/* 14502 */
/* 122 */	0x9e,		/* 158 */
			0xa,		/* 10 */
/* 124 */	0x61,		/* 97 */
			0xe3,		/* 227 */
/* 126 */	0x73,		/* 115 */
			0x9b,		/* 155 */
/* 128 */	0x7e,		/* 126 */
			0x1e,		/* 30 */

			0x0
        }
    };

static const USER_MARSHAL_ROUTINE_QUADRUPLE UserMarshalRoutines[ WIRE_MARSHAL_TABLE_SIZE ] = 
        {
            
            {
            HSTRING_UserSize
            ,HSTRING_UserMarshal
            ,HSTRING_UserUnmarshal
            ,HSTRING_UserFree
            }

        };



/* Standard interface: __MIDL_itf_Linphone2EBackEnd_0000_0000, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IInspectable, ver. 0.0,
   GUID={0xAF86E2E0,0xB12D,0x4c6a,{0x9C,0x5A,0xD7,0xAA,0x65,0x10,0x1E,0x90}} */


/* Object interface: __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals, ver. 0.0,
   GUID={0xC140594A,0x6BD4,0x3151,{0x94,0x4E,0x28,0xC9,0x55,0x63,0x2D,0x15}} */

#pragma code_seg(".orpc")
static const unsigned short __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0
    };

static const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_ProxyInfo =
    {
    &Object_StubDesc,
    Linphone2EBackEnd__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    Linphone2EBackEnd__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(6) ___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsProxyVtbl = 
{
    0,
    &IID___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IInspectable::GetIids */ ,
    0 /* IInspectable::GetRuntimeClassName */ ,
    0 /* IInspectable::GetTrustLevel */
};


static const PRPC_STUB_FUNCTION __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION
};

CInterfaceStubVtbl ___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsStubVtbl =
{
    &IID___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals,
    &__x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_ServerInfo,
    6,
    &__x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Standard interface: __MIDL_itf_Linphone2EBackEnd_0000_0001, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals, ver. 0.0,
   GUID={0x74C65E76,0x06C7,0x38A6,{0x9E,0x0A,0x61,0xE3,0x73,0x9B,0x7E,0x1E}} */

#pragma code_seg(".orpc")
static const unsigned short __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0,
    48,
    82
    };

static const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_ProxyInfo =
    {
    &Object_StubDesc,
    Linphone2EBackEnd__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    Linphone2EBackEnd__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(9) ___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsProxyVtbl = 
{
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_ProxyInfo,
    &IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IInspectable::GetIids */ ,
    0 /* IInspectable::GetRuntimeClassName */ ,
    0 /* IInspectable::GetTrustLevel */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals::StartServer */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals::DoPeriodicKeepAlive */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals::get_CallController */
};


static const PRPC_STUB_FUNCTION __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl ___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsStubVtbl =
{
    &IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_ServerInfo,
    9,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Standard interface: __MIDL_itf_Linphone2EBackEnd_0000_0002, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics, ver. 0.0,
   GUID={0xCE34112C,0xC406,0x3477,{0xBB,0xB4,0x7E,0x1F,0x99,0xEE,0x77,0x6C}} */

#pragma code_seg(".orpc")
static const unsigned short __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    124,
    166,
    214,
    262
    };

static const MIDL_STUBLESS_PROXY_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_ProxyInfo =
    {
    &Object_StubDesc,
    Linphone2EBackEnd__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    Linphone2EBackEnd__MIDL_ProcFormatString.Format,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(10) ___x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsProxyVtbl = 
{
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_ProxyInfo,
    &IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IInspectable::GetIids */ ,
    0 /* IInspectable::GetRuntimeClassName */ ,
    0 /* IInspectable::GetTrustLevel */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics::GetCurrentProcessId */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics::GetUiDisconnectedEventName */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics::GetBackgroundProcessReadyEventName */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics::get_Instance */
};


static const PRPC_STUB_FUNCTION __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl ___x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsStubVtbl =
{
    &IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_ServerInfo,
    10,
    &__x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Standard interface: __MIDL_itf_Linphone2EBackEnd_0000_0003, ver. 0.0,
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
    Linphone2EBackEnd__MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x50002, /* Ndr library version */
    0,
    0x8000253, /* MIDL Version 8.0.595 */
    0,
    UserMarshalRoutines,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0
    };

const CInterfaceProxyVtbl * const _Linphone2EBackEnd_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &___x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsProxyVtbl,
    ( CInterfaceProxyVtbl *) &___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsProxyVtbl,
    ( CInterfaceProxyVtbl *) &___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsProxyVtbl,
    0
};

const CInterfaceStubVtbl * const _Linphone2EBackEnd_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &___x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsStubVtbl,
    ( CInterfaceStubVtbl *) &___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsStubVtbl,
    ( CInterfaceStubVtbl *) &___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsStubVtbl,
    0
};

PCInterfaceName const _Linphone2EBackEnd_InterfaceNamesList[] = 
{
    "__x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics",
    "__x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals",
    "__x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals",
    0
};

const IID *  const _Linphone2EBackEnd_BaseIIDList[] = 
{
    &IID_IInspectable,
    &IID_IInspectable,
    &IID_IInspectable,
    0
};


#define _Linphone2EBackEnd_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _Linphone2EBackEnd, pIID, n)

int __stdcall _Linphone2EBackEnd_IID_Lookup( const IID * pIID, int * pIndex )
{
    IID_BS_LOOKUP_SETUP

    IID_BS_LOOKUP_INITIAL_TEST( _Linphone2EBackEnd, 3, 2 )
    IID_BS_LOOKUP_NEXT_TEST( _Linphone2EBackEnd, 1 )
    IID_BS_LOOKUP_RETURN_RESULT( _Linphone2EBackEnd, 3, *pIndex )
    
}

const ExtendedProxyFileInfo Linphone2EBackEnd_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _Linphone2EBackEnd_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _Linphone2EBackEnd_StubVtblList,
    (const PCInterfaceName * ) & _Linphone2EBackEnd_InterfaceNamesList,
    (const IID ** ) & _Linphone2EBackEnd_BaseIIDList,
    & _Linphone2EBackEnd_IID_Lookup, 
    3,
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

