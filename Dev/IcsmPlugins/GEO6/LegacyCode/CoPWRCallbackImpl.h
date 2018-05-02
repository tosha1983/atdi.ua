// COPWRCALLBACKIMPL.H : Declaration of the TCoPWRCallbackImpl

#ifndef CoPWRCallbackImplH
#define CoPWRCallbackImplH

#define ATL_APARTMENT_THREADED

#include "LISBC_TLB.H"
#include "PiFolio_OCX.h"
#include "PiFolio_TLB.h"

#include <ComCtrls.hpp>
#include <IBQuery.hpp>
#include <Forms.hpp>
#include <map>
#include <SysUtils.hpp>

/////////////////////////////////////////////////////////////////////////////
// TCoPWRCallbackImpl     Implements PWRCallback, default interface of CoPWRCallback
// ThreadingModel : Apartment
// Dual Interface : TRUE
// Event Support  : FALSE
// Default ProgID : LISBC.CoPWRCallback
// Description    :
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TCoPWRCallbackImpl :
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TCoPWRCallbackImpl, &CLSID_CoPWRCallback>,
  public IDispatchImpl<PWRCallback, &IID_PWRCallback, &LIBID_LISBC>
{
private:
    int status;
    std::map<AnsiString, double*> arrays;
    TIBQuery* query;

    void __fastcall FillArra(double** _array, int transmitterId, AnsiString fieldName);
    AnsiString __fastcall NumToStr(double num, int numCount, int afterPoint);
    AnsiString __fastcall GetDNParam(AnsiString field);

public:
  TCoPWRCallbackImpl();
  ~TCoPWRCallbackImpl();

  // Data used when registering Object
  //
  DECLARE_THREADING_MODEL(otApartment);
  DECLARE_PROGID("LISBC.CoPWRCallback");
  DECLARE_DESCRIPTION("");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TCoPWRCallbackImpl>
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TCoPWRCallbackImpl)
  COM_INTERFACE_ENTRY(PWRCallback)
  COM_INTERFACE_ENTRY2(IDispatch, PWRCallback)
END_COM_MAP()

// PWRCallback
public:
 
  STDMETHOD(ReplaceBookmark(BSTR Bookmark, OLE_CANCELBOOL* Result));
  STDMETHOD(SetCallback(Pifolio_tlb::PWRCallBackAction CurrentPoint, OLE_CANCELBOOL* Result));
  STDMETHOD(SetStatus(BSTR StatusMsg));
};

#endif //CoPWRCallbackImplH
