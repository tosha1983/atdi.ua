// COLISBCDIGALLOTIMPL.H : Declaration of the TCoLisBcDigAllotImpl

#ifndef LisBcDigAllotImplH
#define LisBcDigAllotImplH

#define ATL_APARTMENT_THREADED

#include "LISBCTxServer_TLB.H"
#include "LISBC_TLB.h"
#include <vector>
#include <map>
#include <IBSQL.hpp>
#include <IBIntf.hpp>

/////////////////////////////////////////////////////////////////////////////
// TCoLisBcDigAllotImpl     Implements ILisBcDigAllot, default interface of CoLisBcDigAllot
// ThreadingModel : Apartment
// Dual Interface : FALSE
// Event Support  : FALSE
// Default ProgID : LISBCTxServer.CoLisBcDigAllot
// Description    : 
/////////////////////////////////////////////////////////////////////////////
class ATL_NO_VTABLE TLisBcDigAllotImpl :
  public CComObjectRootEx<CComSingleThreadModel>,
  public CComCoClass<TLisBcDigAllotImpl, &CLSID_LisBcDigAllot>,
  public ILisBcDigAllot,
  DUALINTERFACE_IMPL(LisBcDigAllot, ILISBCTx)
{
public:
  TLisBcDigAllotImpl(): id(0), isLoaded(false), isChanged(false), db(NULL),
                        isSubAreasLoaded(false), storage(NULL)//, lon(0.0), lat(0.0)
  {
    CheckIBLoaded();
  }
  ~TLisBcDigAllotImpl()
  {
    if (storage)
        try { storage->Release(); } catch (...) {}
  }

  // Data used when registering Object
  //
  DECLARE_THREADING_MODEL(otApartment);
  DECLARE_PROGID("LISBCTxServer.LisBcDigAllot");
  DECLARE_DESCRIPTION("");

  // Function invoked to (un)register object
  //
  static HRESULT WINAPI UpdateRegistry(BOOL bRegister)
  {
    TTypedComServerRegistrarT<TLisBcDigAllotImpl>
    regObj(GetObjectCLSID(), GetProgID(), GetDescription());
    return regObj.UpdateRegistry(bRegister);
  }


BEGIN_COM_MAP(TLisBcDigAllotImpl)
  COM_INTERFACE_ENTRY(ILisBcDigAllot)
  DUALINTERFACE_ENTRY(ILISBCTx)
END_COM_MAP()

// ILisBcDigAllot
public:

    ILisBcStorage *storage;

  STDMETHOD(get_pointsDs(long* Value));
  STDMETHOD(get_subareasDs(long* Value));
  STDMETHOD(get_acin_id(long* Value));
  STDMETHOD(get_acout_id(long* Value));
  STDMETHOD(get_adm_response(BSTR* Value));
  STDMETHOD(get_adm_sited_in(BSTR* Value));
  STDMETHOD(get_adminid(long* Value));
  STDMETHOD(get_analogtelesystem(long* Value));
  STDMETHOD(get_angleelevation_hor(long* Value));
  STDMETHOD(get_angleelevation_vert(long* Value));
  STDMETHOD(get_antennagain(double* Value));
  STDMETHOD(get_blockcentrefreq(double* Value));
  STDMETHOD(get_channel(BSTR* Value));
  STDMETHOD(get_channel_id(long* Value));
  STDMETHOD(get_data_changes(long* Value));
  STDMETHOD(get_database(long* Value));
  STDMETHOD(get_date_of_last_change(DATE* Value));
  STDMETHOD(get_direction(TBCDirection* Value));
  STDMETHOD(get_dvb_system(TBCDVBSystem* Value));
  STDMETHOD(get_effectantennagains(long idx, double* Value));
  STDMETHOD(get_effectheight(long idx, double* Value));
  STDMETHOD(get_effectpowerhor(long idx, double* Value));
  STDMETHOD(get_effectpowervert(long idx, double* Value));
  STDMETHOD(get_epr_sound_hor_primary(double* Value));
  STDMETHOD(get_epr_sound_hor_second(double* Value));
  STDMETHOD(get_epr_sound_max_primary(double* Value));
  STDMETHOD(get_epr_sound_max_second(double* Value));
  STDMETHOD(get_epr_sound_vert_primary(double* Value));
  STDMETHOD(get_epr_sound_vert_second(double* Value));
  STDMETHOD(get_epr_video_hor(double* Value));
  STDMETHOD(get_epr_video_max(double* Value));
  STDMETHOD(get_epr_video_vert(double* Value));
  STDMETHOD(get_erp(long azimuth, double* power));
  STDMETHOD(get_fiderlenght(long* Value));
  STDMETHOD(get_fiderloss(double* Value));
  STDMETHOD(get_fm_system(TBCFMSystem* Value));
  STDMETHOD(get_freq_carrier(double* freq));
  STDMETHOD(get_gaussianchannel(double* Value));
  STDMETHOD(get_h_eff(long azimuth, long* height));
  STDMETHOD(get_height_eft_max(long* Value));
  STDMETHOD(get_heightantenna(long* Value));
  STDMETHOD(get_id(long* Value));
  STDMETHOD(get_identifiersfn(long* Value));
  STDMETHOD(get_is_fetched(VARIANT_BOOL* Value));
  STDMETHOD(get_latitude(double* Value));
  STDMETHOD(get_longitude(double* Value));
  STDMETHOD(get_maxCoordDist(double* Value));
  STDMETHOD(get_monostereo_primary(long* Value));
  STDMETHOD(get_numregion(BSTR* Value));
  STDMETHOD(get_polarization(TBCPolarization* Value));
  STDMETHOD(get_power_sound_primary(double* Value));
  STDMETHOD(get_power_sound_second(double* Value));
  STDMETHOD(get_power_video(double* Value));
  STDMETHOD(get_rayleighchannel(double* Value));
  STDMETHOD(get_relativetimingsfn(long* Value));
  STDMETHOD(get_riceanchannel(double* Value));
  STDMETHOD(get_rpc(TBcRpc* Value));
  STDMETHOD(get_ref_plan_cfg(BSTR* Value));
  STDMETHOD(get_rxMode(TBcRxMode* Value));
  STDMETHOD(get_site_height(long* Value));
  STDMETHOD(get_sort_key_in(double* Value));
  STDMETHOD(get_sort_key_out(double* Value));
  STDMETHOD(get_sound_carrier_primary(double* Value));
  STDMETHOD(get_sound_carrier_second(double* Value));
  STDMETHOD(get_sound_offset_primary(long* Value));
  STDMETHOD(get_sound_offset_second(long* Value));
  STDMETHOD(get_stand_id(long* Value));
  STDMETHOD(get_station_name(BSTR* Value));
  STDMETHOD(get_status_code(long* Value));
  STDMETHOD(get_systemcast(TBCTxType* Value));
  STDMETHOD(get_systemcolor(TBCTvStandards* Value));
  STDMETHOD(get_testpointsis(long* Value));
  STDMETHOD(get_typeoffset(TBCOffsetType* Value));
  STDMETHOD(get_typesystem(TBCTvSystems* Value));
  STDMETHOD(get_v_sound_ratio_primary(double* Value));
  STDMETHOD(get_v_sound_ratio_second(double* Value));
  STDMETHOD(get_video_carrier(double* Value));
  STDMETHOD(get_video_offset_herz(long* Value));
  STDMETHOD(get_video_offset_line(long* Value));
  STDMETHOD(init(long pdatabase, long load_id));
  STDMETHOD(invalidate());
  STDMETHOD(loadFromQuery(long query));
  STDMETHOD(loadFromString(BSTR Source));
  STDMETHOD(reload());
  STDMETHOD(save());
  STDMETHOD(saveToString(BSTR* Dest));
  STDMETHOD(set_acin_id(long Value));
  STDMETHOD(set_acout_id(long Value));
  STDMETHOD(set_adminid(long Value));
  STDMETHOD(set_analogtelesystem(long Value));
  STDMETHOD(set_angleelevation_hor(long Value));
  STDMETHOD(set_angleelevation_vert(long Value));
  STDMETHOD(set_antennagain(double Value));
  STDMETHOD(set_blockcentrefreq(double Value));
  STDMETHOD(set_channel_id(long Value));
  STDMETHOD(set_channel_name(BSTR Value));
  STDMETHOD(set_database(long Value));
  STDMETHOD(set_direction(TBCDirection Value));
  STDMETHOD(set_dvb_system(TBCDVBSystem Value));
  STDMETHOD(set_effectantennagains(long idx, double Value));
  STDMETHOD(set_effectheight(long idx, double Value));
  STDMETHOD(set_effectpowerhor(long idx, double Value));
  STDMETHOD(set_effectpowervert(long idx, double Value));
  STDMETHOD(set_epr_sound_hor_primary(double Value));
  STDMETHOD(set_epr_sound_hor_second(double Value));
  STDMETHOD(set_epr_sound_max_primary(double Value));
  STDMETHOD(set_epr_sound_max_second(double Value));
  STDMETHOD(set_epr_sound_vert_primary(double Value));
  STDMETHOD(set_epr_sound_vert_second(double Value));
  STDMETHOD(set_epr_video_hor(double Value));
  STDMETHOD(set_epr_video_max(double Value));
  STDMETHOD(set_epr_video_vert(double Value));
  STDMETHOD(set_fiderlenght(long Value));
  STDMETHOD(set_fiderloss(double Value));
  STDMETHOD(set_fm_system(TBCFMSystem Value));
  STDMETHOD(set_gaussianchannel(double Value));
  STDMETHOD(set_height_eft_max(long Value));
  STDMETHOD(set_heightantenna(long Value));
  STDMETHOD(set_identifiersfn(long Value));
  STDMETHOD(set_latitude(double Value));
  STDMETHOD(set_longitude(double Value));
  STDMETHOD(set_maxCoordDist(double Value));
  STDMETHOD(set_monostereo_primary(long Value));
  STDMETHOD(set_polarization(TBCPolarization Value));
  STDMETHOD(set_power_sound_primary(double Value));
  STDMETHOD(set_power_sound_second(double Value));
  STDMETHOD(set_power_video(double Value));
  STDMETHOD(set_rayleighchannel(double Value));
  STDMETHOD(set_relativetimingsfn(long Value));
  STDMETHOD(set_riceanchannel(double Value));
  STDMETHOD(set_rpc(TBcRpc Value));
  STDMETHOD(set_ref_plan_cfg(BSTR Value));
  STDMETHOD(set_rxMode(TBcRxMode Value));
  STDMETHOD(set_sort_key_in(double Value));
  STDMETHOD(set_sort_key_out(double Value));
  STDMETHOD(set_sound_carrier_primary(double Value));
  STDMETHOD(set_sound_carrier_second(double Value));
  STDMETHOD(set_sound_offset_primary(long Value));
  STDMETHOD(set_sound_offset_second(long Value));
  STDMETHOD(set_stand_id(long Value));
  STDMETHOD(set_station_name(BSTR Value));
  STDMETHOD(set_systemcast(TBCTxType Value));
  STDMETHOD(set_systemcolor(TBCTvStandards Value));
  STDMETHOD(set_testpointsis(long Value));
  STDMETHOD(set_typeoffset(TBCOffsetType Value));
  STDMETHOD(set_typesystem(TBCTvSystems Value));
  STDMETHOD(set_v_sound_ratio_primary(double Value));
  STDMETHOD(set_v_sound_ratio_second(double Value));
  STDMETHOD(set_video_carrier(double Value));
  STDMETHOD(set_video_offset_herz(long Value));
  STDMETHOD(set_video_offset_line(long Value));
  STDMETHOD(AddPoint(long subzone, long num, BcCoord point));
  STDMETHOD(AddSubarea(long id));
  STDMETHOD(ClearPoints(long subzone));
  STDMETHOD(ClearSubarea());
  STDMETHOD(DelPoint(long subzone, long num));
  STDMETHOD(DelSubarea(long id));
  STDMETHOD(get_adm_id(long* Value));
  STDMETHOD(get_ctry(BSTR* Value));
  STDMETHOD(get_notice_type(BSTR* Value));
  STDMETHOD(get_freq(double* Value));
  STDMETHOD(get_offset(long* Value));
  STDMETHOD(get_allot_name(BSTR* Value));
  STDMETHOD(get_point(long subareaId, long num, BcCoord* Value));
  STDMETHOD(get_points_num(long subareaId, long* Value));
  STDMETHOD(get_polar(unsigned_char* Value));
  STDMETHOD(get_typ_ref_netwk(BSTR* Value));
  STDMETHOD(get_plan_entry(long* Value));
  STDMETHOD(get_sfn_id(long* Value));
  STDMETHOD(get_spect_mask(unsigned_char* Value));
  STDMETHOD(get_nb_sub_areas(long* Value));
  STDMETHOD(get_geo_area(BSTR* Value));
  STDMETHOD(set_ctry(BSTR Value));
  STDMETHOD(set_freq(double Value));
  STDMETHOD(set_offset(long Value));
  STDMETHOD(set_allot_name(BSTR Value));
  STDMETHOD(set_polar(unsigned_char Value));
  STDMETHOD(set_typ_ref_netwk(BSTR Value));
  STDMETHOD(set_plan_entry(long Value));
  STDMETHOD(set_sfn_id(long Value));
  STDMETHOD(set_spect_mask(unsigned_char Value));
  STDMETHOD(set_geo_area(BSTR Value));
  STDMETHOD(get_attribsDs(long* Value));
  STDMETHOD(get_remarks1(BSTR* Value));
  STDMETHOD(get_remarks2(BSTR* Value));
  STDMETHOD(get_remarks3(BSTR* Value));
  STDMETHOD(set_remarks1(BSTR Value));
  STDMETHOD(set_remarks2(BSTR Value));
  STDMETHOD(set_remarks3(BSTR Value));

  STDMETHOD(get_adm_ref_id(BSTR* Value));
  STDMETHOD(set_adm_ref_id(BSTR Value));
  STDMETHOD(get_db_sect(long* Value));
  STDMETHOD(set_db_sect(long Value));
  STDMETHOD(get_SubareaCount(long* Value));
  STDMETHOD(get_subareaId(long idx, long* Value));
  STDMETHOD(get_subareaTag(long idx, long* Value));
  STDMETHOD(get_coord(BSTR* Value));
  STDMETHOD(set_coord(BSTR Value));
  STDMETHOD(get_pol_isol(double* Value));
  STDMETHOD(set_pol_isol(double Value));
private:
    long id;
    bool isLoaded;
    bool isChanged;

    TIBDatabase *db;

  long subareasDs;
  long pointsDs;
  WideString dab_dvb;

  int adm_id;
  WideString adm_ref_id;
  long plan_entry;
  WideString country;
  WideString name;
  WideString uniq_country;

  WideString coord;

  typedef std::vector<BcCoord> SubArea;
  typedef std::map<int, SubArea> SubAreas;

  std::vector<int> subAreaIds;
  std::vector<int> subAreaTags;

  SubAreas subAreas;
  bool isSubAreasLoaded;
  //bool isSubzonesChanged;

  WideString rpc;
  WideString rn;
  double freq;
  long channel_id;
  int freqOffset;
  unsigned char pol;
  unsigned char sm;
  long sfn_id;
  WideString remarks1;
  WideString remarks2;
  WideString remarks3;

    double cenGravLat;
    double cenGravLon;

    HRESULT loadSubzones();
    void recalcCoord();

};

#endif //CoLisBcDigAllotImplH
