// LISBCTXLISTIMPL.H : Declaration of the TLISBCTxListImpl

#ifndef LISBCTxListImplH
#define LISBCTxListImplH

#define ATL_APARTMENT_THREADED

#include "LISBCTxServer_TLB.H"
#include <vector>
/////////////////////////////////////////////////////////////////////////////
// TLISBCTxListImpl     Implements ILISBCTxList, default interface of LISBCTxList
// ThreadingModel : Apartment
// Dual Interface : FALSE
// Event Support  : FALSE
// Default ProgID : LISBCTxServer.LISBCTxList
// Description    :
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TLISBCTxListImpl :
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TLISBCTxListImpl, &CLSID_LISBCTxList>,
  public ILISBCTxList
{
public:
  TLISBCTxListImpl()
  {
  }
  ~TLISBCTxListImpl();

  // Data used when registering Object
  //
  DECLARE_THREADING_MODEL(otApartment);
  DECLARE_PROGID("LISBCTxServer.LISBCTxList");
  DECLARE_DESCRIPTION("");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TLISBCTxListImpl>
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TLISBCTxListImpl)
  COM_INTERFACE_ENTRY(ILISBCTxList)
END_COM_MAP()

// ILISBCTxList
public:

  STDMETHOD(AddTx(ILISBCTx* Tx, long* idx));
  STDMETHOD(RemoveTx(ILISBCTx* Tx));
  STDMETHOD(RemoveId(long id));
  STDMETHOD(get_Size(long* Value));
  STDMETHOD(get_TxId(long idx, long* Value));
  STDMETHOD(get_Tx(long idx, ILISBCTx** Value));
  STDMETHOD(get_TxUseInCalc(long idx, VARIANT_BOOL* Value));
  STDMETHOD(set_TxUseInCalc(long idx, VARIANT_BOOL Value));
  STDMETHOD(Clear());
  STDMETHOD(get_TxUnwantInterfere(long idx, double* Value));
  STDMETHOD(get_TxWantInterfere(long idx, double* Value));
  STDMETHOD(set_TxUnwantInterfere(long idx, double Value));
  STDMETHOD(set_TxWantInterfere(long idx, double Value));
  STDMETHOD(SortByUnwantInterfere());
  STDMETHOD(SortByWantInterfere());
  STDMETHOD(get_TxUnwantedKind(long idx, signed_char* Value));
  STDMETHOD(get_TxWantedKind(long idx, signed_char* Value));
  STDMETHOD(set_TxUnwantedKind(long idx, signed_char Value));
  STDMETHOD(set_TxWantedKind(long idx, signed_char Value));
  STDMETHOD(set_TxAzimuth(long idx, double Value));
  STDMETHOD(set_TxDistance(long idx, double Value));
  STDMETHOD(set_TxZoneOverlapping(long idx, double Value));
  STDMETHOD(get_TxAzimuth(long idx, double* Value));
  STDMETHOD(get_TxDistance(long idx, double* Value));
  STDMETHOD(get_TxZoneOverlapping(long idx, double* Value));
  STDMETHOD(get_TxShowOnMap(long idx, VARIANT_BOOL* Value));
  STDMETHOD(set_TxShowOnMap(long idx, VARIANT_BOOL Value));
  struct TxListRecord {
    long id;
    bool usedInCalc;
    double wantedInterfere;
    double unwantedInterfere;
    char wantedKind;
    char unwantedKind;
    double distance;
    double azimuth;
    double zoneOverlapping;
    bool showOnMap;
    ILISBCTx *get_tx() { return tx; }
  protected:
    ILISBCTx *tx;
    TxListRecord() {};
  public:
    TxListRecord(long src_id, ILISBCTx* src_tx,
                bool src_usedInCalc = true, bool src_showOnMap = true, double src_zoneOverlapping = false,
                double src_distance = 0., double src_azimuth = 0.,
                double src_wantedInterfere = -999., double src_unwantedInterfere = -999.,
                char src_wantedKind = 'C', char src_unwantedKind = 'C'):
        id(src_id),
        usedInCalc(src_usedInCalc),
        wantedInterfere(src_wantedInterfere),
        unwantedInterfere(src_unwantedInterfere),
        wantedKind(src_wantedKind),
        unwantedKind(src_unwantedKind),
        distance(src_distance),
        azimuth(src_azimuth),
        zoneOverlapping(src_zoneOverlapping),
        showOnMap(src_showOnMap)
    {
        if (src_tx)
            src_tx->AddRef();
        tx = src_tx;
    };
    ~TxListRecord()
    {
        if(tx)
            try {
                tx->Release();
            } catch (...) {
            //TODO: log exception
            }
    }
  };

protected:
  typedef std::vector<TxListRecord*> TxVector;
  TxVector txVector;
};

#endif //LISBCTxListImplH
