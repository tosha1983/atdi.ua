// COLISBCOBJECTSERVERVERSIONIMPL.H : Declaration of the TCoLisBcObjectServerVersionImpl

#ifndef LisBcObjectServerVersionImplH
#define LisBcObjectServerVersionImplH

#define ATL_APARTMENT_THREADED

#include "LISBCTxServer_TLB.H"

/////////////////////////////////////////////////////////////////////////////
// TCoLisBcObjectServerVersionImpl     Implements ILisBcObjectServerVersion, default interface of CoLisBcObjectServerVersion
// ThreadingModel : Single
// Dual Interface : FALSE
// Event Support  : FALSE
// Default ProgID : LISBCTxServer.CoLisBcObjectServerVersion
// Description    :
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TLisBcObjectServerVersionImpl :
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TLisBcObjectServerVersionImpl, &CLSID_LisBcObjectServerVersion>,
  public ILisBcObjectServerVersion
{
public:                                  
  TLisBcObjectServerVersionImpl()
  {
  }

  // Data used when registering Object 
  //
  DECLARE_THREADING_MODEL(otSingle);
  DECLARE_PROGID("LISBCTxServer.LisBcObjectServerVersion");
  DECLARE_DESCRIPTION("");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TLisBcObjectServerVersionImpl> 
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TLisBcObjectServerVersionImpl)
  COM_INTERFACE_ENTRY(ILisBcObjectServerVersion)
END_COM_MAP()

// ILisBcObjectServerVersion
public:
 
  STDMETHOD(GetVersion(double* Value));
};

#endif //CoLisBcObjectServerVersionImplH
