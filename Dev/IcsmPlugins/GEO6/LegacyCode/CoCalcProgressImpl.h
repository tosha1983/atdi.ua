// COCALCPROGRESSIMPL.H : Declaration of the TCoCalcProgressImpl

#ifndef CoCalcProgressImplH
#define CoCalcProgressImplH

#define ATL_APARTMENT_THREADED

#include "LISBC_TLB.H"
#include "LISProgress_OCX.h"


/////////////////////////////////////////////////////////////////////////////
// TCoCalcProgressImpl     Implements ILISProgress, default interface of CoCalcProgress
// ThreadingModel : Apartment
// Dual Interface : TRUE
// Event Support  : FALSE
// Default ProgID : LISBC.CoCalcProgress
// Description    : Контроль выполнения длительных процедур расчёта
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TCoCalcProgressImpl : 
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TCoCalcProgressImpl, &CLSID_CoCalcProgress>,
  public IDispatchImpl<ILISProgress, &IID_ILISProgress, &LIBID_LISBC>
{
public:
  TCoCalcProgressImpl()
  {
  }

  // Data used when registering Object 
  //
  DECLARE_THREADING_MODEL(otApartment);
  DECLARE_PROGID("LISBC.CoCalcProgress");
  DECLARE_DESCRIPTION("Контроль выполнения длительных процедур расчёта");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TCoCalcProgressImpl> 
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TCoCalcProgressImpl)
  COM_INTERFACE_ENTRY(ILISProgress)
  COM_INTERFACE_ENTRY2(IDispatch, ILISProgress)
END_COM_MAP()

// ILISProgress
public:
 
  STDMETHOD(Progress(long* perc));
};

#endif //CoCalcProgressImplH
