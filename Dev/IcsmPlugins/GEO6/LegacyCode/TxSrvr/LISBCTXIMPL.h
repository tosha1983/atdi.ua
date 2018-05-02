// LISBCTXIMPL.H : Declaration of the TLISBCTxImpl

#ifndef LISBCTxImplH
#define LISBCTxImplH

#define ATL_APARTMENT_THREADED

#include "LISBCTxServer_TLB.h"
#include "LISBC_TLB.h"
#include <IBIntf.hpp>
#include <vector>
#include <string>
#include <IBQuery.hpp>
using namespace std;
/////////////////////////////////////////////////////////////////////////////
// TLISBCTxImpl     Implements ILISBCTx, default interface of LISBCTx
// ThreadingModel : Single
// Dual Interface : FALSE
// Event Support  : FALSE
// Default ProgID : Project1.LISBCTx
// Description    : Вещательный передатчик
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TLISBCTxImpl :
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TLISBCTxImpl, &CLSID_LISBCTx>,
  public ILISBCTx,
  DUALINTERFACE_IMPL(LISBCTx, ILisBcAntPatt),
  DUALINTERFACE_IMPL(LISBCTx, ILisBcLfMf),
  DUALINTERFACE_IMPL(LISBCTx, ILisBcFxm),
  DUALINTERFACE_IMPL(LISBCTx, ILisAssocAllotId),
  DUALINTERFACE_IMPL(LISBCTx, ILisBcDvbt2)
{
public:
  TLISBCTxImpl(): db(NULL), effectpowerhor(36, -999.0), effectpowerhor_night(36, -999.0),
            effectpowervert(36, -999.0), effectheight(36, 0.0),
            ant_diag_h(36, 0.0), ant_diag_h_night(36, 0.0), ant_diag_v(36, 0.0),
            id(0), //database(0),
            systemcast((TBCTxType)0), data_changes(0), is_fetched(false), detLoaded(false),
            channel_id(0), stand_id(0), acout_id(0), acin_id(0), maxCoordDist(0),
            rpc((TBcRpc)-1), rxMode((TBcRxMode)-1),
            storage(NULL), d_op(false), n_op(false), is_day(-1), mod_type((TBcModType)0), prot_levl(-1),
            d_ant_type(0), n_ant_type(0), heightantenna_night(0), power_sound_night(0), n_bw(9),
            n_adj_ratio(0), lfmf_system(-1), noise_zone(0)
            , epr_sound_max(-999.), epr_sound_max_night(-999.)
            , bandwidth(0.)
            , is_dvb_t2(false), pilot_pattern(0), diversity(0), rotated_constellations(false), mode_of_extentions(false)
            , modulation(0), code_rate(0), fft_size(0), guard_interval(0)
  {
    CheckIBLoaded();
  }
  ~TLISBCTxImpl()
  {
    if (storage)
        try { storage->Release(); } catch (...) {}
  }

  // Data used when registering Object
  //
  DECLARE_THREADING_MODEL(otSingle);
  DECLARE_PROGID("LISBCTxServer.LISBCTx");
  DECLARE_DESCRIPTION("Вещательный передатчик");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TLISBCTxImpl>
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TLISBCTxImpl)
  COM_INTERFACE_ENTRY(ILISBCTx)
  DUALINTERFACE_ENTRY(ILisBcAntPatt)
  DUALINTERFACE_ENTRY(ILisBcLfMf)
  DUALINTERFACE_ENTRY(ILisBcFxm)
  DUALINTERFACE_ENTRY(ILisAssocAllotId)
  DUALINTERFACE_ENTRY(ILisBcDvbt2)
END_COM_MAP()

// ILISBCTx
public:
    char szVal[96]; //  большего размера НЕ БУДЕТ
    ILisBcStorage *storage;

  bool          data_changes;
  bool          detLoaded;

  long          id;
  TBCTxType     systemcast;
  long          typesystem_id;
  long          systemcast_id;
  double        longitude, latitude;
  TBCTvSystems  typesystem;
  double        video_carrier;
  long          video_offset_line;
  long          video_offset_herz;
  TBCTvStandards systemcolour;
  double        power_video;
  double        epr_video_max;
  double        epr_video_hor;
  double        epr_video_vert;
  vector<double> effectpowerhor;
  vector<double> effectpowerhor_night;
  vector<double> effectpowervert;
  long          identifiersfn;
  long          relativetiming;
  double        blockcentrefreq;
  double        sound_carrier_primary;
  long          sound_offset_primary;
  double        power_sound;
  double        power_sound_night;
  double        epr_sound_max;
  double        epr_sound_max_night;
  double        epr_sound_hor;
  double        epr_sound_vert;
  double        v_sound_ratio;
  long          monostereo;
  double        sound_carrier_second;
  long          sound_offset_second;
  double        epr_sound_max_second;
  double        epr_sound_hor_second;
  double        epr_sound_vert_second;
  double        v_sound_ratio_second;
  long          heightantenna;
  long          heightantenna_night;
  vector<double> effectheight;
  long          height_eft_max;
  TBCPolarization polarization;
  TBCDirection  directivity;
  double        fiderloss;
  long          fiderlenght;
  long          angleelevation_hor;
  long          angleelevation_vert;
  double        antennagain;
  double        antennagain_n;
  vector<double> ant_diag_h;
  vector<double> ant_diag_h_night;
  vector<double> ant_diag_v;
  long          testpointsis;
  long          analogtelesystem;
  TBCOffsetType typeoffset;
  long          channel_id;
  double        sort_key_in;
  double        sort_key_out;
  TIBDatabase   *db;
  bool          use_for_calc;
//  vector<TBCTestpoint> testpoints;
  TBCDVBSystem  dvb_system;
  TBCFMSystem   fm_system;
  long          lfmf_system;
  double    gaussianchannel;
  double    rayleighchannel;
  double    riceanchannel;
  long          stand_id;
  long          acout_id;
  long          acin_id;

  WideString    adm_response;
  WideString    adm_sited_in;
  long          status_code;
  WideString    station_name;
  long          site_height;
  WideString    channel;
  DATE          date_of_last_change;
  WideString    numregion;

  double        maxCoordDist;
  long          adminid;

  long          fourDummyBytes; //  заебала пизда - при пустом adminid сбрасывает флаг is_fetched в false.
                                //  добавим "санитарную зону" в 4 байта длиной.
  bool          is_fetched;

  TBcRpc        rpc;            // Reference Plan Configuration - цифра
  TBcRxMode     rxMode;         // тип приёма - цифра
  double        pol_isol;       // развязка по антенне в случае кроссполяризационного приёма (фикс. приём в цифре)

  double        d_bw;           // signal bandwidth
  double        n_bw;           // signal bandwidth
  VARIANT_BOOL  d_op;           // day operation
  VARIANT_BOOL  n_op;           // night operation
  long          is_day;         // is day mode (or night)
  double        d_adj_ratio;    // day adjanced ratio
  double        n_adj_ratio;    // night adjanced ratio
  char          d_ant_type;     // antenna type
  char          n_ant_type;     // antenna type
  double        gnd_cond;       // ground conductivity
  long          noise_zone;     // noise zone
  TBcModType    mod_type;       // for LFMF DRM - QAM factor (0-16, 1-64)
  long          prot_levl;      // for LFMF DRM - protection level [0-3]

  double        bandwidth;      // used for FXM

  WideString    assoc_allot_id; // associated_adm_allot_id

  // DVB-T2 specific
  VARIANT_BOOL  is_dvb_t2;
  unsigned long pilot_pattern;
  unsigned long diversity;
  VARIANT_BOOL  rotated_constellations;
  VARIANT_BOOL  mode_of_extentions;

  TModulation   modulation;
  TCodeRate     code_rate;
  TFftSize      fft_size;
  TGuardInterval2 guard_interval;

    template<class T>
    HRESULT GetFieldVal(T* val, T fld, GUID iid)
    {
      try
      {
        HRESULT hr = S_OK;
        if (!is_fetched)
            if (FAILED(hr = reload()))
                return hr;
        if (systemcast == ttAM && !detLoaded)
            if (FAILED(hr = reload()))
                return hr;
        *val = fld;
      }
      catch(Exception &e)
      {
        return Error(e.Message.c_str(), iid);
      }
      return S_OK;
    }
    template<class T>
    HRESULT SetFieldVal(T& fld, T val)
    {
        if (fld != val)
        {
            fld = val;
            data_changes = true;
        }
        return S_OK;
    }
    void RecalcErp();


protected:

private:
    void reset_diags();

  STDMETHOD(get_id(long* Value));
  STDMETHOD(get_latitude(double* Value));
  STDMETHOD(get_longitude(double* Value));
  STDMETHOD(get_systemcolor(TBCTvStandards* Value));
  STDMETHOD(get_typesystem(TBCTvSystems* Value));
  STDMETHOD(get_video_carrier(double* Value));
  STDMETHOD(get_video_offset_herz(long* Value));
  STDMETHOD(get_video_offset_line(long* Value));
  STDMETHOD(set_latitude(double Value));
  STDMETHOD(set_longitude(double Value));
  STDMETHOD(set_systemcolor(TBCTvStandards Value));
  STDMETHOD(set_typesystem(TBCTvSystems Value));
  STDMETHOD(set_video_carrier(double Value));
  STDMETHOD(set_video_offset_herz(long Value));
  STDMETHOD(set_video_offset_line(long Value));
  STDMETHOD(get_effectpowerhor(long idx, double* Value));
  STDMETHOD(get_effectpowervert(long idx, double* Value));
  STDMETHOD(get_epr_video_hor(double* Value));
  STDMETHOD(get_epr_video_max(double* Value));
  STDMETHOD(get_epr_video_vert(double* Value));
  STDMETHOD(get_power_video(double* Value));
  STDMETHOD(set_effectpowerhor(long idx, double Value));
  STDMETHOD(set_effectpowervert(long idx, double Value));
  STDMETHOD(set_epr_video_hor(double Value));
  STDMETHOD(set_epr_video_max(double Value));
  STDMETHOD(set_epr_video_vert(double Value));
  STDMETHOD(set_power_video(double Value));
  STDMETHOD(get_blockcentrefreq(double* Value));
  STDMETHOD(get_identifiersfn(long* Value));
  STDMETHOD(get_relativetimingsfn(long* Value));
  STDMETHOD(set_blockcentrefreq(double Value));
  STDMETHOD(set_identifiersfn(long Value));
  STDMETHOD(set_relativetimingsfn(long Value));
  STDMETHOD(get_effectheight(long idx, double* Value));
  STDMETHOD(get_epr_sound_hor_primary(double* Value));
  STDMETHOD(get_epr_sound_hor_second(double* Value));
  STDMETHOD(get_epr_sound_max_primary(double* Value));
  STDMETHOD(get_epr_sound_max_second(double* Value));
  STDMETHOD(get_epr_sound_vert_primary(double* Value));
  STDMETHOD(get_epr_sound_vert_second(double* Value));
  STDMETHOD(get_monostereo_primary(long* Value));
  STDMETHOD(get_power_sound_primary(double* Value));
  STDMETHOD(get_power_sound_second(double* Value));
  STDMETHOD(get_sound_carrier_primary(double* Value));
  STDMETHOD(get_sound_carrier_second(double* Value));
  STDMETHOD(get_sound_offset_primary(long* Value));
  STDMETHOD(get_sound_offset_second(long* Value));
  STDMETHOD(get_v_sound_ratio_primary(double* Value));
  STDMETHOD(get_v_sound_ratio_second(double* Value));
  STDMETHOD(set_effectheight(long idx, double Value));
  STDMETHOD(set_epr_sound_hor_primary(double Value));
  STDMETHOD(set_epr_sound_hor_second(double Value));
  STDMETHOD(set_epr_sound_max_primary(double Value));
  STDMETHOD(set_epr_sound_max_second(double Value));
  STDMETHOD(set_epr_sound_vert_primary(double Value));
  STDMETHOD(set_epr_sound_vert_second(double Value));
  STDMETHOD(set_monostereo_primary(long Value));
  STDMETHOD(set_power_sound_primary(double Value));
  STDMETHOD(set_power_sound_second(double Value));
  STDMETHOD(set_sound_carrier_primary(double Value));
  STDMETHOD(set_sound_carrier_second(double Value));
  STDMETHOD(set_sound_offset_primary(long Value));
  STDMETHOD(set_sound_offset_second(long Value));
  STDMETHOD(set_v_sound_ratio_primary(double Value));
  STDMETHOD(set_v_sound_ratio_second(double Value));
  STDMETHOD(get_direction(TBCDirection* Value));
  STDMETHOD(get_height_eft_max(long* Value));
  STDMETHOD(get_polarization(TBCPolarization* Value));
  STDMETHOD(set_direction(TBCDirection Value));
  STDMETHOD(set_height_eft_max(long Value));
  STDMETHOD(set_polarization(TBCPolarization Value));
  STDMETHOD(get_angleelevation_hor(long* Value));
  STDMETHOD(get_angleelevation_vert(long* Value));
  STDMETHOD(get_antennagain(double* Value));
  STDMETHOD(get_effectantennagains(long idx, double* Value));
  STDMETHOD(get_fiderlenght(long* Value));
  STDMETHOD(get_fiderloss(double* Value));
  STDMETHOD(get_testpointsis(long* Value));
  STDMETHOD(set_angleelevation_hor(long Value));
  STDMETHOD(set_angleelevation_vert(long Value));
  STDMETHOD(set_antennagain(double Value));
  STDMETHOD(set_effectantennagains(long idx, double Value));
  STDMETHOD(set_fiderlenght(long Value));
  STDMETHOD(set_fiderloss(double Value));
  STDMETHOD(set_testpointsis(long Value));
  STDMETHOD(init(long pdatabase, long load_id));
  STDMETHOD(save());
  STDMETHOD(get_heightantenna(long* Value));
  STDMETHOD(set_heightantenna(long Value));
  STDMETHOD(get_data_changes(long* Value));
  STDMETHOD(get_systemcast(TBCTxType* Value));
  STDMETHOD(set_systemcast(TBCTxType Value));
  STDMETHOD(get_analogtelesystem(long* Value));
  STDMETHOD(set_analogtelesystem(long Value));
  STDMETHOD(get_typeoffset(TBCOffsetType* Value));
  STDMETHOD(set_typeoffset(TBCOffsetType Value));
  STDMETHOD(get_channel_id(long* Value));
  STDMETHOD(set_channel_id(long Value));
  STDMETHOD(get_erp(long azimuth, double* power));
  STDMETHOD(get_freq_carrier(double* freq));
  STDMETHOD(get_h_eff(long azimuth, long* height));
  STDMETHOD(get_sort_key_in(double* Value));
  STDMETHOD(get_sort_key_out(double* Value));
  STDMETHOD(set_sort_key_in(double Value));
  STDMETHOD(set_sort_key_out(double Value));
  STDMETHOD(reload());
  STDMETHOD(get_use_for_calc(VARIANT_BOOL* Value));
  STDMETHOD(set_use_for_calc(VARIANT_BOOL Value));
  STDMETHOD(get_dvb_system(TBCDVBSystem* Value));
  STDMETHOD(set_dvb_system(TBCDVBSystem Value));
  STDMETHOD(get_fm_system(TBCFMSystem* Value));
  STDMETHOD(set_fm_system(TBCFMSystem Value));
  STDMETHOD(get_gaussianchannel(double* Value));
  STDMETHOD(set_gaussianchannel(double Value));
  STDMETHOD(get_rayleighchannel(double* Value));
  STDMETHOD(get_riceanchannel(double* Value));
  STDMETHOD(set_rayleighchannel(double Value));
  STDMETHOD(set_riceanchannel(double Value));
  STDMETHOD(loadFromString(BSTR Source));
  STDMETHOD(saveToString(BSTR* Dest));
  STDMETHOD(loadFromQuery(long query));
  STDMETHOD(get_acin_id(long* Value));
  STDMETHOD(get_acout_id(long* Value));
  STDMETHOD(get_is_fetched(VARIANT_BOOL* Value));
  STDMETHOD(get_adminid(long* Value));
  STDMETHOD(get_stand_id(long* Value));
  STDMETHOD(set_acin_id(long Value));
  STDMETHOD(set_acout_id(long Value));
  STDMETHOD(set_adminid(long Value));
  STDMETHOD(set_stand_id(long Value));
  STDMETHOD(invalidate());
  STDMETHOD(get_database(long* Value));
  STDMETHOD(set_database(long Value));

  STDMETHOD(get_typesystem_id(long* Value));
  STDMETHOD(set_typesystem_id(long Value));
  STDMETHOD(get_adm_response(BSTR* Value));
  STDMETHOD(get_adm_sited_in(BSTR* Value));
  STDMETHOD(get_channel(BSTR* Value));
  STDMETHOD(get_date_of_last_change(DATE* Value));
  STDMETHOD(get_site_height(long* Value));
  STDMETHOD(get_station_name(BSTR* Value));
  STDMETHOD(get_status_code(long* Value));


  STDMETHOD(get_numregion(BSTR* Value));
  STDMETHOD(get_maxCoordDist(double* Value));
  STDMETHOD(set_maxCoordDist(double Value));
  STDMETHOD(set_station_name(BSTR Value));
  STDMETHOD(set_channel_name(BSTR Value));
  STDMETHOD(get_rpc(TBcRpc* Value));
  STDMETHOD(get_rxMode(TBcRxMode* Value));
  STDMETHOD(set_rpc(TBcRpc Value));
  STDMETHOD(set_rxMode(TBcRxMode Value));
  STDMETHOD(attribs());
  STDMETHOD(get_attribsDs(long* Value));
  STDMETHOD(get_coord(BSTR* Value));
  STDMETHOD(set_coord(BSTR Value));


  STDMETHOD(get_gain_h(long idx, double* Value));
  STDMETHOD(set_gain_h(long idx, double Value));
  STDMETHOD(get_gain_v(long idx, double* Value));
  STDMETHOD(set_gain_v(long idx, double Value));
  STDMETHOD(get_discr_h(long idx, double* Value));
  STDMETHOD(set_discr_h(long idx, double Value));
  STDMETHOD(get_discr_v(long idx, double* Value));
  STDMETHOD(set_discr_v(long idx, double Value));
  
  STDMETHOD(get_pol_isol(double* Value));
  STDMETHOD(set_pol_isol(double Value));
  STDMETHOD(get_lfmf_system(long* Value));
  STDMETHOD(set_lfmf_system(long Value));
  STDMETHOD(get_lfmf_bw(double* Value));
  STDMETHOD(set_lfmf_bw(double Value));
  STDMETHOD(get_day_op(VARIANT_BOOL* Value));
  STDMETHOD(get_is_day(VARIANT_BOOL* Value));
  STDMETHOD(get_night_op(VARIANT_BOOL* Value));
  STDMETHOD(set_day_op(VARIANT_BOOL Value));
  STDMETHOD(set_is_day(VARIANT_BOOL Value));
  STDMETHOD(set_night_op(VARIANT_BOOL Value));
  STDMETHOD(get_adj_ratio(double* Value));
  STDMETHOD(get_ant_type(unsigned_char* Value));
  STDMETHOD(get_gnd_cond(double* Value));
  STDMETHOD(get_noise_zone(long* Value));
  STDMETHOD(set_adj_ratio(double Value));
  STDMETHOD(set_ant_type(unsigned_char Value));
  STDMETHOD(set_gnd_cond(double Value));
  STDMETHOD(set_noise_zone(long Value));
  STDMETHOD(get_mod_type(long* Value));
  STDMETHOD(set_mod_type(long Value));
  STDMETHOD(get_prot_levl(long* Value));
  STDMETHOD(set_prot_levl(long Value));
  STDMETHOD(get_fxm_bandwidth(double* Value));
  STDMETHOD(get_fxm_system(unsigned_long* Value));
  STDMETHOD(set_fxm_bandwidth(double Value));
  STDMETHOD(set_fxm_system(unsigned_long Value));
  STDMETHOD(GetAssAllotId(BSTR* assAllId));
  STDMETHOD(get_Diversity(unsigned_long* Value));
  STDMETHOD(get_IsDvbt2(VARIANT_BOOL* Value));
  STDMETHOD(get_ModeOfExtentions(VARIANT_BOOL* Value));
  STDMETHOD(get_PilotPattern(unsigned_long* Value));
  STDMETHOD(get_RotatedConstellations(VARIANT_BOOL* Value));
  STDMETHOD(set_Diversity(unsigned_long Value));
  STDMETHOD(set_IsDvbt2(VARIANT_BOOL Value));
  STDMETHOD(set_ModeOfExtentions(VARIANT_BOOL Value));
  STDMETHOD(set_PilotPattern(unsigned_long Value));
  STDMETHOD(set_RotatedConstellations(VARIANT_BOOL Value));
  STDMETHOD(get_CodeRate(TCodeRate* Value));
  STDMETHOD(get_FftSize(TFftSize* Value));
  STDMETHOD(get_GuardInterval(TGuardInterval2* Value));
  STDMETHOD(get_Modulation(TModulation* Value));
  STDMETHOD(set_CodeRate(TCodeRate Value));
  STDMETHOD(set_FftSize(TFftSize Value));
  STDMETHOD(set_GuardInterval(TGuardInterval2 Value));
  STDMETHOD(set_Modulation(TModulation Value));
};

#endif //LISBCTxImplH
