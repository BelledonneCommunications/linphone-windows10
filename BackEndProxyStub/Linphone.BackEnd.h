

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


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


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __Linphone2EBackEnd_h__
#define __Linphone2EBackEnd_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef ____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_FWD_DEFINED__
#define ____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_FWD_DEFINED__
typedef interface __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals;

#ifdef __cplusplus
namespace ABI {
    namespace Linphone {
        namespace BackEnd {
            interface __ICallControllerPublicNonVirtuals;
        } /* end namespace */
    } /* end namespace */
} /* end namespace */

#endif /* __cplusplus */

#endif 	/* ____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_FWD_DEFINED__ */


#ifndef ____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_FWD_DEFINED__
#define ____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_FWD_DEFINED__
typedef interface __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals;

#ifdef __cplusplus
namespace ABI {
    namespace Linphone {
        namespace BackEnd {
            interface __IGlobalsPublicNonVirtuals;
        } /* end namespace */
    } /* end namespace */
} /* end namespace */

#endif /* __cplusplus */

#endif 	/* ____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_FWD_DEFINED__ */


#ifndef ____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_FWD_DEFINED__
#define ____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_FWD_DEFINED__
typedef interface __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics;

#ifdef __cplusplus
namespace ABI {
    namespace Linphone {
        namespace BackEnd {
            interface __IGlobalsStatics;
        } /* end namespace */
    } /* end namespace */
} /* end namespace */

#endif /* __cplusplus */

#endif 	/* ____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_FWD_DEFINED__ */


/* header files for imported files */
#include "inspectable.h"
#include "AsyncInfo.h"
#include "EventToken.h"
#include "Windows.Foundation.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0000 */
/* [local] */ 

#if defined(__cplusplus)
}
#endif // defined(__cplusplus)
#include <Windows.Foundation.h>
#if defined(__cplusplus)
extern "C" {
#endif // defined(__cplusplus)



#ifdef __cplusplus
namespace ABI {
namespace Linphone {
namespace BackEnd {
class CallController;
} /*BackEnd*/
} /*Linphone*/
}
#endif
#ifdef __cplusplus
namespace ABI {
namespace Linphone {
namespace BackEnd {
class Globals;
} /*BackEnd*/
} /*Linphone*/
}
#endif
#if !defined(____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_INTERFACE_DEFINED__)
extern const __declspec(selectany) WCHAR InterfaceName_Linphone_BackEnd___ICallControllerPublicNonVirtuals[] = L"Linphone.BackEnd.__ICallControllerPublicNonVirtuals";
#endif /* !defined(____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_INTERFACE_DEFINED__) */


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0000 */
/* [local] */ 






extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0000_v0_0_s_ifspec;

#ifndef ____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_INTERFACE_DEFINED__
#define ____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_INTERFACE_DEFINED__

/* interface __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals */
/* [uuid][object] */ 



/* interface ABI::Linphone::BackEnd::__ICallControllerPublicNonVirtuals */
/* [uuid][object] */ 


EXTERN_C const IID IID___x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals;

#if defined(__cplusplus) && !defined(CINTERFACE)
    } /* end extern "C" */
    namespace ABI {
        namespace Linphone {
            namespace BackEnd {
                
                MIDL_INTERFACE("C140594A-6BD4-3151-944E-28C955632D15")
                __ICallControllerPublicNonVirtuals : public IInspectable
                {
                public:
                };

                extern const __declspec(selectany) IID & IID___ICallControllerPublicNonVirtuals = __uuidof(__ICallControllerPublicNonVirtuals);

                
            }  /* end namespace */
        }  /* end namespace */
    }  /* end namespace */
    extern "C" { 
    
#else 	/* C style interface */

    typedef struct __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetIids )( 
            __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals * This,
            /* [out] */ ULONG *iidCount,
            /* [size_is][size_is][out] */ IID **iids);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeClassName )( 
            __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals * This,
            /* [out] */ HSTRING *className);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrustLevel )( 
            __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals * This,
            /* [out] */ TrustLevel *trustLevel);
        
        END_INTERFACE
    } __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsVtbl;

    interface __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals
    {
        CONST_VTBL struct __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtualsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_GetIids(This,iidCount,iids)	\
    ( (This)->lpVtbl -> GetIids(This,iidCount,iids) ) 

#define __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_GetRuntimeClassName(This,className)	\
    ( (This)->lpVtbl -> GetRuntimeClassName(This,className) ) 

#define __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_GetTrustLevel(This,trustLevel)	\
    ( (This)->lpVtbl -> GetTrustLevel(This,trustLevel) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ____x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0001 */
/* [local] */ 

#if !defined(____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_INTERFACE_DEFINED__)
extern const __declspec(selectany) WCHAR InterfaceName_Linphone_BackEnd___IGlobalsPublicNonVirtuals[] = L"Linphone.BackEnd.__IGlobalsPublicNonVirtuals";
#endif /* !defined(____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_INTERFACE_DEFINED__) */


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0001 */
/* [local] */ 



extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0001_v0_0_s_ifspec;

#ifndef ____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_INTERFACE_DEFINED__
#define ____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_INTERFACE_DEFINED__

/* interface __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals */
/* [uuid][object] */ 



/* interface ABI::Linphone::BackEnd::__IGlobalsPublicNonVirtuals */
/* [uuid][object] */ 


EXTERN_C const IID IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals;

#if defined(__cplusplus) && !defined(CINTERFACE)
    } /* end extern "C" */
    namespace ABI {
        namespace Linphone {
            namespace BackEnd {
                
                MIDL_INTERFACE("74C65E76-06C7-38A6-9E0A-61E3739B7E1E")
                __IGlobalsPublicNonVirtuals : public IInspectable
                {
                public:
                    virtual HRESULT STDMETHODCALLTYPE StartServer( 
                        /* [in] */ UINT32 __outOfProcServerClassNamesSize,
                        /* [in][size_is] */ HSTRING *outOfProcServerClassNames) = 0;
                    
                    virtual HRESULT STDMETHODCALLTYPE DoPeriodicKeepAlive( void) = 0;
                    
                    virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CallController( 
                        /* [out][retval] */ ABI::Linphone::BackEnd::__ICallControllerPublicNonVirtuals **__returnValue) = 0;
                    
                };

                extern const __declspec(selectany) IID & IID___IGlobalsPublicNonVirtuals = __uuidof(__IGlobalsPublicNonVirtuals);

                
            }  /* end namespace */
        }  /* end namespace */
    }  /* end namespace */
    extern "C" { 
    
#else 	/* C style interface */

    typedef struct __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetIids )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This,
            /* [out] */ ULONG *iidCount,
            /* [size_is][size_is][out] */ IID **iids);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeClassName )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This,
            /* [out] */ HSTRING *className);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrustLevel )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This,
            /* [out] */ TrustLevel *trustLevel);
        
        HRESULT ( STDMETHODCALLTYPE *StartServer )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This,
            /* [in] */ UINT32 __outOfProcServerClassNamesSize,
            /* [in][size_is] */ HSTRING *outOfProcServerClassNames);
        
        HRESULT ( STDMETHODCALLTYPE *DoPeriodicKeepAlive )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CallController )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals * This,
            /* [out][retval] */ __x_ABI_CLinphone_CBackEnd_C____ICallControllerPublicNonVirtuals **__returnValue);
        
        END_INTERFACE
    } __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsVtbl;

    interface __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals
    {
        CONST_VTBL struct __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtualsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_GetIids(This,iidCount,iids)	\
    ( (This)->lpVtbl -> GetIids(This,iidCount,iids) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_GetRuntimeClassName(This,className)	\
    ( (This)->lpVtbl -> GetRuntimeClassName(This,className) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_GetTrustLevel(This,trustLevel)	\
    ( (This)->lpVtbl -> GetTrustLevel(This,trustLevel) ) 


#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_StartServer(This,__outOfProcServerClassNamesSize,outOfProcServerClassNames)	\
    ( (This)->lpVtbl -> StartServer(This,__outOfProcServerClassNamesSize,outOfProcServerClassNames) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_DoPeriodicKeepAlive(This)	\
    ( (This)->lpVtbl -> DoPeriodicKeepAlive(This) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_get_CallController(This,__returnValue)	\
    ( (This)->lpVtbl -> get_CallController(This,__returnValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ____x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0002 */
/* [local] */ 

#if !defined(____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_INTERFACE_DEFINED__)
extern const __declspec(selectany) WCHAR InterfaceName_Linphone_BackEnd___IGlobalsStatics[] = L"Linphone.BackEnd.__IGlobalsStatics";
#endif /* !defined(____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_INTERFACE_DEFINED__) */


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0002 */
/* [local] */ 



extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0002_v0_0_s_ifspec;

#ifndef ____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_INTERFACE_DEFINED__
#define ____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_INTERFACE_DEFINED__

/* interface __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics */
/* [uuid][object] */ 



/* interface ABI::Linphone::BackEnd::__IGlobalsStatics */
/* [uuid][object] */ 


EXTERN_C const IID IID___x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics;

#if defined(__cplusplus) && !defined(CINTERFACE)
    } /* end extern "C" */
    namespace ABI {
        namespace Linphone {
            namespace BackEnd {
                
                MIDL_INTERFACE("CE34112C-C406-3477-BBB4-7E1F99EE776C")
                __IGlobalsStatics : public IInspectable
                {
                public:
                    virtual HRESULT STDMETHODCALLTYPE GetCurrentProcessId( 
                        /* [out][retval] */ UINT32 *__returnValue) = 0;
                    
                    virtual HRESULT STDMETHODCALLTYPE GetUiDisconnectedEventName( 
                        /* [in] */ UINT32 backgroundProcessId,
                        /* [out][retval] */ HSTRING *__returnValue) = 0;
                    
                    virtual HRESULT STDMETHODCALLTYPE GetBackgroundProcessReadyEventName( 
                        /* [in] */ UINT32 backgroundProcessId,
                        /* [out][retval] */ HSTRING *__returnValue) = 0;
                    
                    virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Instance( 
                        /* [out][retval] */ ABI::Linphone::BackEnd::__IGlobalsPublicNonVirtuals **__returnValue) = 0;
                    
                };

                extern const __declspec(selectany) IID & IID___IGlobalsStatics = __uuidof(__IGlobalsStatics);

                
            }  /* end namespace */
        }  /* end namespace */
    }  /* end namespace */
    extern "C" { 
    
#else 	/* C style interface */

    typedef struct __x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetIids )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [out] */ ULONG *iidCount,
            /* [size_is][size_is][out] */ IID **iids);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeClassName )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [out] */ HSTRING *className);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrustLevel )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [out] */ TrustLevel *trustLevel);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentProcessId )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [out][retval] */ UINT32 *__returnValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetUiDisconnectedEventName )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [in] */ UINT32 backgroundProcessId,
            /* [out][retval] */ HSTRING *__returnValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetBackgroundProcessReadyEventName )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [in] */ UINT32 backgroundProcessId,
            /* [out][retval] */ HSTRING *__returnValue);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Instance )( 
            __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics * This,
            /* [out][retval] */ __x_ABI_CLinphone_CBackEnd_C____IGlobalsPublicNonVirtuals **__returnValue);
        
        END_INTERFACE
    } __x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsVtbl;

    interface __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics
    {
        CONST_VTBL struct __x_ABI_CLinphone_CBackEnd_C____IGlobalsStaticsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_GetIids(This,iidCount,iids)	\
    ( (This)->lpVtbl -> GetIids(This,iidCount,iids) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_GetRuntimeClassName(This,className)	\
    ( (This)->lpVtbl -> GetRuntimeClassName(This,className) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_GetTrustLevel(This,trustLevel)	\
    ( (This)->lpVtbl -> GetTrustLevel(This,trustLevel) ) 


#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_GetCurrentProcessId(This,__returnValue)	\
    ( (This)->lpVtbl -> GetCurrentProcessId(This,__returnValue) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_GetUiDisconnectedEventName(This,backgroundProcessId,__returnValue)	\
    ( (This)->lpVtbl -> GetUiDisconnectedEventName(This,backgroundProcessId,__returnValue) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_GetBackgroundProcessReadyEventName(This,backgroundProcessId,__returnValue)	\
    ( (This)->lpVtbl -> GetBackgroundProcessReadyEventName(This,backgroundProcessId,__returnValue) ) 

#define __x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_get_Instance(This,__returnValue)	\
    ( (This)->lpVtbl -> get_Instance(This,__returnValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ____x_ABI_CLinphone_CBackEnd_C____IGlobalsStatics_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0003 */
/* [local] */ 

#ifndef RUNTIMECLASS_Linphone_BackEnd_CallController_DEFINED
#define RUNTIMECLASS_Linphone_BackEnd_CallController_DEFINED
extern const __declspec(selectany) WCHAR RuntimeClass_Linphone_BackEnd_CallController[] = L"Linphone.BackEnd.CallController";
#endif
#ifndef RUNTIMECLASS_Linphone_BackEnd_Globals_DEFINED
#define RUNTIMECLASS_Linphone_BackEnd_Globals_DEFINED
extern const __declspec(selectany) WCHAR RuntimeClass_Linphone_BackEnd_Globals[] = L"Linphone.BackEnd.Globals";
#endif


/* interface __MIDL_itf_Linphone2EBackEnd_0000_0003 */
/* [local] */ 



extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_Linphone2EBackEnd_0000_0003_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  HSTRING_UserSize(     unsigned long *, unsigned long            , HSTRING * ); 
unsigned char * __RPC_USER  HSTRING_UserMarshal(  unsigned long *, unsigned char *, HSTRING * ); 
unsigned char * __RPC_USER  HSTRING_UserUnmarshal(unsigned long *, unsigned char *, HSTRING * ); 
void                      __RPC_USER  HSTRING_UserFree(     unsigned long *, HSTRING * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


