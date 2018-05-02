// COLISBCSTORAGEIMPL.H : Declaration of the TCoLisBcStorageImpl

#ifndef CoStorageImplH
#define CoStorageImplH

#define ATL_APARTMENT_THREADED

#include "LISBC_TLB.H"


/////////////////////////////////////////////////////////////////////////////
// TCoLisBcStorageImpl     Implements ILisBcStorage, default interface of CoLisBcStorage
// ThreadingModel : Apartment
// Dual Interface : FALSE
// Event Support  : FALSE
// Default ProgID : LISBC.CoLisBcStorage
// Description    : Storage service for LisBc transmitters
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TCoLisBcStorageImpl : 
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TCoLisBcStorageImpl, &CLSID_CoLisBcStorage>,
  public ILisBcStorage
{
public:
  TCoLisBcStorageImpl()
  {
  }

  // Data used when registering Object 
  //
  DECLARE_THREADING_MODEL(otApartment);
  DECLARE_PROGID("LISBC.CoLisBcStorage");
  DECLARE_DESCRIPTION("Storage service for LisBc transmitters");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TCoLisBcStorageImpl> 
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TCoLisBcStorageImpl)
  COM_INTERFACE_ENTRY(ILisBcStorage)
END_COM_MAP()

// ILisBcStorage
public:
 
  STDMETHOD(LoadObject(LPUNKNOWN obj));
  STDMETHOD(SaveObject(LPUNKNOWN obj));
  STDMETHOD(LoadDetails(LPUNKNOWN obj));
};

#endif //CoLisBcStorageImplH
