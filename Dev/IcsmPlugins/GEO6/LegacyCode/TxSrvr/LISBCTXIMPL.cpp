// LISBCTXIMPL : Implementation of TLISBCTxImpl (CoClass: LISBCTx, Interface: ILISBCTx)

#include <vcl.h>
#include <ibsql.hpp>
#include <ibquery.hpp>
#include <DBTables.hpp>
#include <cstring>
#include <math>

#pragma hdrstop

#include "LISBCTXIMPL.H"
#include <strstream>
#include <memory>
#include <wstring.h>
#include <values.h>

#ifdef StrToInt
#undef StrToInt
#endif
/////////////////////////////////////////////////////////////////////////////
// TLISBCTxImpl

STDMETHODIMP TLISBCTxImpl::get_id(long* Value)
{
  try  {
    *Value = id;
    //ShowMessage(AnsiString("ILISBCTx: m_dwRef = ")+m_dwRef);
  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxImpl::get_latitude(double* Value)
{
  try {
    if (!is_fetched) reload();
   *Value = latitude;
  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_longitude(double* Value)
{
  try {
    if (!is_fetched) reload();
   *Value = longitude;
  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_systemcolor(TBCTvStandards* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = systemcolour;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_typesystem(TBCTvSystems* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = typesystem;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_video_carrier(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = video_carrier;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_video_offset_herz(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = video_offset_herz;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_video_offset_line(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = video_offset_line;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_latitude(double Value)
{
  if (id < 0) try
  {
    if (floor(latitude*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    latitude = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_longitude(double Value)
{
  if (id < 0) try
  {
    if (floor(longitude*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    longitude = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_systemcolor(TBCTvStandards Value)
{
  try
  {
    if (systemcolour != Value) data_changes = true;
    systemcolour = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::set_typesystem(TBCTvSystems Value)
{
  try
  {
   if (typesystem != Value) data_changes = true;
   typesystem = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_video_carrier(double Value)
{
  try
  {
    if (floor(video_carrier*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    video_carrier = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_video_offset_herz(long Value)
{
  try
  {
   if (video_offset_herz != Value) data_changes = true;
   video_offset_herz = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_video_offset_line(long Value)
{
  try
  {
    if (video_offset_line != Value) data_changes = true;
    video_offset_line = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_effectpowerhor(long idx, double* Value)
{
  try
  {
    if (!is_fetched)
        reload();
    if (systemcast != ttAM)
        *Value = effectpowerhor.at(idx);
    else
    {
        if (!detLoaded)
            reload();
        if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        if (is_day && d_op && !effectpowerhor.empty())
            *Value = effectpowerhor[idx];
        else if (!is_day && n_op && !effectpowerhor_night.empty())
            *Value = effectpowerhor_night[idx];
        else
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_effectpowervert(long idx, double* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = effectpowervert.at(idx);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_video_hor(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = epr_video_hor;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_video_max(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = epr_video_max;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_video_vert(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = epr_video_vert;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_power_video(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = power_video;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_effectpowerhor(long idx, double Value)
{
  try
  {
   if (floor(effectpowerhor.at(idx)*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   effectpowerhor.at(idx) = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_effectpowervert(long idx, double Value)
{
  try
  {
   if (floor(effectpowervert.at(idx)*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   effectpowervert.at(idx) = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_video_hor(double Value)
{
  try
  {
   if (floor(epr_video_hor*1000+0.5) != floor(Value*1000+0.5))
           data_changes = true;
   epr_video_hor = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_video_max(double Value)
{
  try
  {
   if (floor(epr_video_max*1000+0.5) != floor(Value*1000+0.5))
      data_changes = true;
   epr_video_max = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_video_vert(double Value)
{
  try
  {
   if (floor(epr_video_vert*1000+0.5) != floor(Value*1000+0.5))
        data_changes = true;
   epr_video_vert = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxImpl::set_power_video(double Value)
{
  try
  {
   if (floor(power_video*1000+0.5) != floor(Value*1000+0.5))
         data_changes = true;
   power_video = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_blockcentrefreq(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = blockcentrefreq;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_identifiersfn(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = identifiersfn;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_relativetimingsfn(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = relativetiming;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxImpl::set_blockcentrefreq(double Value)
{
  try
  {
   if (floor(blockcentrefreq*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   blockcentrefreq = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_identifiersfn(long Value)
{
  try
  {
    if (identifiersfn != Value) data_changes = true;
    identifiersfn = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_relativetimingsfn(long Value)
{
  try
  {
   if (relativetiming != Value) data_changes = true;
   relativetiming = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_effectheight(long idx, double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = effectheight.at(idx);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_sound_hor_primary(double* Value)
{
  try
  {
    if (!is_fetched)
        reload();
    if (systemcast != ttAM)
        *Value = epr_sound_hor;
    else
    {
        if (!detLoaded)
            reload();
        if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        *Value = is_day ? epr_sound_max : epr_sound_max_night;
    }

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_sound_hor_second(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = epr_sound_hor_second;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_sound_max_primary(double* Value)
{
  try
  {
    if (!is_fetched)
        reload();
    if (systemcast != ttAM)
        *Value = epr_sound_max;
    else
    {
        if (!detLoaded)
            reload();
        if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        *Value = is_day ? epr_sound_max : epr_sound_max_night;
    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_sound_max_second(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = epr_sound_max_second;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_sound_vert_primary(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    if (systemcast != ttAM)
        *Value = epr_sound_vert;
    else
        *Value = -999.;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_epr_sound_vert_second(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = epr_sound_vert_second;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_monostereo_primary(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = monostereo;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_power_sound_primary(double* Value)
{
  try
  {
    if (!is_fetched)
        reload();
    if (systemcast != ttAM)
        *Value = power_sound;
    else
    {
        if (!detLoaded)
            reload();
        if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        *Value = is_day ? power_sound : power_sound_night;
    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_power_sound_second(double* Value)
{
  return Error(__FUNC__": method deprecated", IID_ILISBCTx);
};


STDMETHODIMP TLISBCTxImpl::get_sound_carrier_primary(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = sound_carrier_primary;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_sound_carrier_second(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = sound_carrier_second;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_sound_offset_primary(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = sound_offset_primary;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_sound_offset_second(long* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = sound_offset_second;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_v_sound_ratio_primary(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = v_sound_ratio;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_v_sound_ratio_second(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = v_sound_ratio_second;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_effectheight(long idx, double Value)
{
  try
  {
   if (floor(effectheight.at(idx)*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   effectheight.at(idx) = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_sound_hor_primary(double Value)
{
  try
  {
    if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    double &fld = (systemcast != ttAM) ? epr_sound_hor :
        (is_day || !n_op) ? epr_sound_max : epr_sound_max_night;
    if (floor(fld*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    fld = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_sound_hor_second(double Value)
{
  try
  {
   if (floor(epr_sound_hor_second*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   epr_sound_hor_second = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_sound_max_primary(double Value)
{
  try
  {
    if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    double& fld = (systemcast != ttAM || is_day || !n_op) ? epr_sound_max : epr_sound_max_night;
    if (floor(fld*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    fld = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_sound_max_second(double Value)
{
  try
  {
   if (floor(epr_sound_max_second*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   epr_sound_max_second = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_sound_vert_primary(double Value)
{
  try
  {
    double& fld = epr_sound_vert;
    if (floor(fld*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    fld = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_epr_sound_vert_second(double Value)
{
  try
  {
   if (floor(epr_sound_vert_second*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   epr_sound_vert_second = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_monostereo_primary(long Value)
{
  try
  {
   if (monostereo != Value) data_changes = true;
   monostereo = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_power_sound_primary(double Value)
{
  try
  {
    if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    double& fld = (systemcast != ttAM || is_day || !n_op) ? power_sound : power_sound_night;
    if (floor(fld*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    fld = Value;                      
    char ant_t = (n_op && !is_day) ? n_ant_type : d_ant_type;
    if(ant_t == 'A')
    {
        if(n_op && !is_day)
            epr_sound_max_night = 10. * log10(power_sound_night);
        else
            epr_sound_max = 10. * log10(power_sound);
    }
    RecalcErp();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_power_sound_second(double Value)
{
  return Error(__FUNC__": method deprecated", IID_ILISBCTx);
};


STDMETHODIMP TLISBCTxImpl::set_sound_carrier_primary(double Value)
{
  try
  {
   if (floor(sound_carrier_primary*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   sound_carrier_primary = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_sound_carrier_second(double Value)
{
  try
  {
   if (floor(sound_carrier_second*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   sound_carrier_second = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_sound_offset_primary(long Value)
{
  try
  {
   if (sound_offset_primary != Value) data_changes = true;
   sound_offset_primary = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_sound_offset_second(long Value)
{
  try
  {
  if (sound_offset_second != Value) data_changes = true;
  sound_offset_second = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_v_sound_ratio_primary(double Value)
{
  try
  {
   if (floor(v_sound_ratio*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   v_sound_ratio = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_v_sound_ratio_second(double Value)
{
  try
  {
   if (floor(v_sound_ratio_second*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   v_sound_ratio_second = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_direction(TBCDirection* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = directivity;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_height_eft_max(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = height_eft_max;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_polarization(TBCPolarization* Value)
{
    if (systemcast == ttAM)
    {
        *Value = polarization = plHOR;
        return S_OK;
    }
    else
        return GetFieldVal<TBCPolarization>(Value, polarization, IID_ILISBCTx);
};

STDMETHODIMP TLISBCTxImpl::set_direction(TBCDirection Value)
{
  try
  {
    if (floor(directivity*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    directivity = Value;
    if (directivity == drND)
        reset_diags();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_height_eft_max(long Value)
{
  try
  {
   if (height_eft_max != Value) data_changes = true;
   height_eft_max = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_polarization(TBCPolarization Value)
{
  try
  {
    if (polarization != Value) data_changes = true;
    if (systemcast != ttAM && (polarization == plHOR || polarization == plVER))
    {
        std::vector<double>::iterator vi;
        if(polarization == plHOR)
        {
          std::vector<double>& fld  =  ant_diag_h;
          for (vi = fld.begin(); vi < fld.end(); vi++)
            *vi = 0.0;
        }
        else if(polarization == plVER)
        {
            std::vector<double>& fld  =  ant_diag_v;
            for (vi = fld.begin(); vi < fld.end(); vi++)
                *vi = 0.0;
        }         

    }
    else
        if (directivity == drND)
            reset_diags();
    polarization = Value;
    
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_angleelevation_hor(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = angleelevation_hor;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_angleelevation_vert(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = angleelevation_vert;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_antennagain(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = antennagain;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_effectantennagains(long idx, double* Value)
{
  try
  {
    throw *(new Exception ("get_effectantennagains(): Метод не используется"));
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_fiderlenght(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = fiderlenght;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_fiderloss(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = fiderloss;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_testpointsis(long* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = testpointsis;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_angleelevation_hor(long Value)
{
  try
  {
   if (angleelevation_hor != Value) data_changes = true;
   angleelevation_hor = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_angleelevation_vert(long Value)
{
  try
  {
   if (angleelevation_vert != Value) data_changes = true;
   angleelevation_vert = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_antennagain(double Value)
{
    if (floor(antennagain*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    antennagain = Value;
    if (directivity == drND)
        reset_diags();
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_effectantennagains(long idx, double Value)
{
  try
  {
    throw *(new Exception ("set_effectantennagains(): Метод не используется"));
    if (floor(ant_diag_h.at(idx)*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
    ant_diag_h.at(idx) = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_fiderlenght(long Value)
{
  try
  {
   if (fiderlenght != Value) data_changes = true;
   fiderlenght = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_fiderloss(double Value)
{
  try
  {
   if (floor(fiderloss*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   fiderloss = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_testpointsis(long Value)
{
  try
  {
   if (testpointsis != Value) data_changes = true;
   testpointsis = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_heightantenna(long* Value)
{
  try
  {
    if (!is_fetched)
        reload();
    if (systemcast != ttAM)
        *Value = heightantenna;
    else
    {
        if (!detLoaded)
            reload();
        if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        *Value = is_day ? heightantenna : heightantenna_night;
    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_heightantenna(long Value)
{
  try
  {
    if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    long* fld = (systemcast != ttAM || is_day || !n_op) ? &heightantenna : &heightantenna_night;
    if (*fld != Value)
        data_changes = true;
    *fld = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::init(long pdatabase, long load_id)
{
  try {

    if (id)
        return Error("Об'єкт проініціалізований", IID_ILISBCTx);

    if (pdatabase != NULL)
    {
        OleCheck(((IUnknown*)pdatabase)->QueryInterface(IID_ILisBcStorage, (void**)&storage));
        if (storage == NULL)
            return Error("Can't get Storage interface", IID_ILISBCTx);
    }

    id = load_id;

    if (load_id < 0)
    {
        is_fetched = true;
        detLoaded = true;
    }

    data_changes = false;

  } catch (Exception &e) {
        id = 0;
        return Error((AnsiString("Помилка завантаження передавача ID = ") + AnsiString(load_id) + ": "+e.Message).c_str(), IID_ILISBCTx);
  }

  return S_OK;
}

STDMETHODIMP TLISBCTxImpl::save()
{
    if (data_changes && id > 0) try {

        //if (storage == NULL)
        //    return Error("Storage interface is NULL", IID_ILISBCTx);

        //storage->SaveObject((IUnknown*)(ILISBCTx*)this);

        if (db == NULL)
            return Error("save(): database is not assigned", IID_ILISBCTx);

        std::auto_ptr<TIBQuery> sql (new TIBQuery(NULL));
        std::auto_ptr<TIBQuery> sqlAux (new TIBQuery(NULL));

        sqlAux->Database = db;
        sql->Database = db;

        //std::auto_ptr<TIBTransaction> tr (new TIBTransaction(NULL));
        //tr->DefaultDatabase = (TIBDatabase *)database;
        //sql->Transaction = tr.get();
        //sqlAux->Transaction = tr.get();
        //tr->StartTransaction();

        randomize();

        AnsiString TypeOffset;
        if (typeoffset == otPrecision)
            TypeOffset = "P";
        else
            TypeOffset = "NP";

        AnsiString SystemColour;
        if (systemcolour == csNTSC)
            SystemColour = "NTSC";
        else if (systemcolour == csSECAM)
            SystemColour = "SECAM";
        else
            SystemColour = "PAL";

        AnsiString Polarization, Direction;
        if (systemcast == ttAM || polarization == plHOR)
            Polarization = "H";
        else if (polarization == plVER)
            Polarization = "V";
        else if (polarization == plMIX)
            Polarization = "M";

        if (directivity == drD)
            Direction = "D";
        else
            Direction = "ND";

        sqlAux->Close();
        sqlAux->SQL->Text = "";
        if (systemcast == ttFM)
            sqlAux->SQL->Text = "select id from ANALOGRADIOSYSTEM where ENUMVAL = " + AnsiString(fm_system);
        else if (systemcast == ttDVB)
            sqlAux->SQL->Text = "select id from DIGITALTELESYSTEM where ENUMVAL = " + AnsiString(dvb_system);
        else if (systemcast == ttAM)
            typesystem_id = lfmf_system;
        else if (systemcast == ttTV)
            sqlAux->SQL->Text = "select id from ANALOGTELESYSTEM where ENUMVAL = " + AnsiString(typesystem);
        if (sqlAux->SQL->Text.Length() > 0)
        {
            sqlAux->Open();
            typesystem_id = sqlAux->Fields->Fields[0]->AsInteger;
        } 

        sqlAux->Close();
        sqlAux->SQL->Text = "select id from SYSTEMCAST where ENUMVAL = " + AnsiString(systemcast);
        sqlAux->Open();
        systemcast_id = sqlAux->Fields->Fields[0]->AsInteger;
        sqlAux->Close();

        AnsiString sqlText = "update TRANSMITTERS"
        "  set ID = :ID"
        ", LATITUDE = :LATITUDE"
        ", LONGITUDE = :LONGITUDE"
        ", TYPESYSTEM = :TYPESYSTEM"
        ", SYSTEMCAST_ID = :SYSTEMCAST_ID"
        ", VIDEO_CARRIER = :VIDEO_CARRIER"
        ", VIDEO_OFFSET_LINE = :VIDEO_OFFSET_LINE"
        ", VIDEO_OFFSET_HERZ = :VIDEO_OFFSET_HERZ"
        ", SYSTEMCOLOUR = :SYSTEMCOLOUR"
        ", POWER_VIDEO = :POWER_VIDEO"
        ", EPR_VIDEO_MAX = :EPR_VIDEO_MAX"
        ", EPR_VIDEO_HOR = :EPR_VIDEO_HOR"
        ", EPR_VIDEO_VERT = :EPR_VIDEO_VERT "
        ", CHANNEL_ID = :CHANNEL_ID"
        ", IDENTIFIERSFN = :IDENTIFIERSFN"
        ", RELATIVETIMINGSFN = :RELATIVETIMINGSFN"
        ", BLOCKCENTREFREQ = :BLOCKCENTREFREQ"
        ", SOUND_CARRIER_PRIMARY = :SOUND_CARRIER_PRIMARY"
        ", SOUND_OFFSET_PRIMARY = :SOUND_OFFSET_PRIMARY"
        ", POWER_SOUND_PRIMARY = :POWER_SOUND_PRIMARY"
        ", EPR_SOUND_MAX_PRIMARY = :EPR_SOUND_MAX_PRIMARY"
        ", EPR_SOUND_HOR_PRIMARY = :EPR_SOUND_HOR_PRIMARY"
        ", EPR_SOUND_VERT_PRIMARY = :EPR_SOUND_VERT_PRIMARY "
        ", V_SOUND_RATIO_PRIMARY = :V_SOUND_RATIO_PRIMARY "
        ", MONOSTEREO_PRIMARY = :MONOSTEREO_PRIMARY"
        ", SOUND_CARRIER_SECOND = :SOUND_CARRIER_SECOND "
        ", SOUND_OFFSET_SECOND = :SOUND_OFFSET_SECOND"
        ", POWER_SOUND_SECOND = :POWER_SOUND_SECOND "
        ", EPR_SOUND_MAX_SECOND = :EPR_SOUND_MAX_SECOND "
        ", EPR_SOUND_HOR_SECOND = :EPR_SOUND_HOR_SECOND "
        ", EPR_SOUND_VER_SECOND = :EPR_SOUND_VER_SECOND "
        ", V_SOUND_RATIO_SECOND = :V_SOUND_RATIO_SECOND "
        ", HEIGHTANTENNA =  :HEIGHTANTENNA"
        ", HEIGHT_EFF_MAX = :HEIGHT_EFF_MAX "
        ", POLARIZATION = :POLARIZATION"     //   Polari
        ", DIRECTION = :DIRECTION"                  //  Direction
        ", FIDERLOSS = :FIDERLOSS "
        ", FIDERLENGTH = :FIDERLENGTH "
        ", ANGLEELEVATION_HOR = :ANGLEELEVATION_HOR "
        ", ANGLEELEVATION_VER = :ANGLEELEVATION_VER "
        ", ANTENNAGAIN = :ANTENNAGAIN"
        ", TESTPOINTSIS = :TESTPOINTSIS"
        ", EFFECTPOWERHOR=:EFFECTPOWERHOR"
        ", EFFECTPOWERVER=:EFFECTPOWERVER"
        ", EFFECTHEIGHT=:EFFECTHEIGHT"
        ", ANT_DIAG_H=:ANT_DIAG_H "
        ", ANT_DIAG_V=:ANT_DIAG_V "
        ", RPC=:RPC"
        ", RX_MODE=:RX_MODE"
        ", POL_ISOL=:POL_ISOL"
        ", BANDWIDTH=:BANDWIDTH"
        ", MOD_TYPE=:MOD_TYPE"
        ", PROT_LEVL=:PROT_LEVL"
        ", GND_COND=:GND_COND"
        ", NOISE_ZONE=:NOISE_ZONE"
        ", CARRIER=:CARRIER"
        ", IS_DVB_T2 = :IS_DVB_T2"
        ", PILOT_PATTERN = :PILOT_PATTERN"
        ", DIVERSITY = :DIVERSITY"
        ", ROTATED_CNSTLS = :ROTATED_CNSTLS"
        ", MODE_OF_EXTNTS = :MODE_OF_EXTNTS"
        ", MODULATION = :MODULATION"
        ", CODE_RATE = :CODE_RATE"
        ", FFT_SIZE = :FFT_SIZE"
        ", GUARD_INTERVAL = :GUARD_INTERVAL"
        ;

        if (maxCoordDist > 0)
            sqlText += ", MAX_COORD_DIST = :MAX_COORD_DIST";

        sqlText += (" where id = " + AnsiString(id));

        sql->SQL->Text = sqlText;


        sql->ParamByName("ID")->AsInteger         =  id;
        sql->ParamByName("LATITUDE")->AsFloat     =  latitude;
        sql->ParamByName("LONGITUDE")->AsFloat    =  longitude;
        sql->ParamByName("VIDEO_CARRIER")->AsFloat =  video_carrier;
        sql->ParamByName("SYSTEMCAST_ID")->AsInteger =  systemcast_id;
        sql->ParamByName("TYPESYSTEM")->AsInteger =  typesystem_id;
        sql->ParamByName("VIDEO_OFFSET_LINE")->AsInteger =  video_offset_line;
        sql->ParamByName("VIDEO_OFFSET_HERZ")->AsInteger =  video_offset_herz;
        sql->ParamByName("SYSTEMCOLOUR")->AsString = SystemColour;
        sql->ParamByName("POWER_VIDEO")->AsFloat   =  power_video;
        sql->ParamByName("EPR_VIDEO_MAX")->AsFloat =  epr_video_max;
        sql->ParamByName("EPR_VIDEO_HOR")->AsFloat =  epr_video_hor;
        sql->ParamByName("EPR_VIDEO_VERT")->AsFloat = epr_video_vert;
        sql->ParamByName("CHANNEL_ID")->AsFloat    =  channel_id;
        sql->ParamByName("IDENTIFIERSFN")->AsInteger = identifiersfn;
        sql->ParamByName("RELATIVETIMINGSFN")->AsInteger = relativetiming;
        sql->ParamByName("BLOCKCENTREFREQ")->AsFloat = blockcentrefreq;
        sql->ParamByName("SOUND_CARRIER_PRIMARY")->AsFloat = sound_carrier_primary;
        sql->ParamByName("SOUND_OFFSET_PRIMARY")->AsFloat  = sound_offset_primary;
        sql->ParamByName("POWER_SOUND_PRIMARY")->AsFloat  = power_sound;
        sql->ParamByName("EPR_SOUND_MAX_PRIMARY")->AsFloat = epr_sound_max;
        sql->ParamByName("EPR_SOUND_HOR_PRIMARY")->AsFloat  = epr_sound_hor;
        sql->ParamByName("EPR_SOUND_VERT_PRIMARY")->AsFloat = epr_sound_vert;
        sql->ParamByName("V_SOUND_RATIO_PRIMARY")->AsFloat  = v_sound_ratio;
        sql->ParamByName("MONOSTEREO_PRIMARY")->AsInteger = monostereo;
        sql->ParamByName("SOUND_CARRIER_SECOND")->AsFloat = sound_carrier_second;
        sql->ParamByName("SOUND_OFFSET_SECOND")->AsInteger  = sound_offset_second;
        sql->ParamByName("EPR_SOUND_MAX_SECOND")->AsFloat = epr_sound_max_second;
        sql->ParamByName("EPR_SOUND_HOR_SECOND")->AsFloat  = epr_sound_hor_second;
        sql->ParamByName("EPR_SOUND_VER_SECOND")->AsFloat = epr_sound_vert_second;
        sql->ParamByName("V_SOUND_RATIO_SECOND")->AsFloat = v_sound_ratio_second;
        sql->ParamByName("HEIGHTANTENNA")->AsInteger = heightantenna;
        sql->ParamByName("HEIGHT_EFF_MAX")->AsInteger  = height_eft_max;
        sql->ParamByName("POLARIZATION")->AsString  = Polarization;
        sql->ParamByName("DIRECTION")->AsString = Direction;
        sql->ParamByName("FIDERLOSS")->AsFloat = fiderloss;
        sql->ParamByName("FIDERLENGTH")->AsInteger = fiderlenght;
        sql->ParamByName("ANGLEELEVATION_HOR")->AsInteger = angleelevation_hor;
        sql->ParamByName("ANGLEELEVATION_VER")->AsInteger = angleelevation_vert;
        sql->ParamByName("ANTENNAGAIN")->AsFloat = antennagain;
        sql->ParamByName("TESTPOINTSIS")->AsInteger = testpointsis;

        sql->ParamByName("RPC")->AsInteger = rpc;
        sql->ParamByName("RX_MODE")->AsInteger = rxMode;
        sql->ParamByName("POL_ISOL")->AsFloat = pol_isol;
        sql->ParamByName("BANDWIDTH")->AsFloat = bandwidth;
        sql->ParamByName("MOD_TYPE")->AsInteger = mod_type;
        sql->ParamByName("PROT_LEVL")->AsInteger = prot_levl;
        sql->ParamByName("GND_COND")->AsFloat = gnd_cond;
        sql->ParamByName("NOISE_ZONE")->AsInteger = noise_zone;

        double freq = 0.;
        get_freq_carrier(&freq);
        if (freq > 0)
            sql->ParamByName("CARRIER")->AsFloat = freq;

        if (maxCoordDist > 0)
            sql->ParamByName("MAX_COORD_DIST")->AsFloat = maxCoordDist;

        std::auto_ptr<TMemoryStream> memstream (new TMemoryStream());

        memstream->Clear();
        vector<double>::iterator vi;
        //for(int i = 0; i<36 ; i++)
        //          memstream->Write(&effectpowerhor.at(i), sizeof(double));
        for (vi = effectpowerhor.begin(); vi < effectpowerhor.end(); vi++)
            memstream->Write(vi, sizeof(double));
        sql->ParamByName("EFFECTPOWERHOR")->LoadFromStream(memstream.get(), ftBlob);
        memstream->Clear();

        //for(int i = 0; i<36 ; i++)  memstream->Write(&effectpowervert.at(i), sizeof(double));
        for (vi = effectpowervert.begin(); vi < effectpowervert.end(); vi++)
            memstream->Write(vi, sizeof(double));
        sql->ParamByName("EFFECTPOWERVER")->LoadFromStream(memstream.get(), ftBlob);
        memstream->Clear();

        //for(int i = 0; i< 36 ; i++)  memstream->Write(&effectheight.at(i), sizeof(double));
        for (vi = effectheight.begin(); vi < effectheight.end(); vi++)
            memstream->Write(vi, sizeof(double));
        sql->ParamByName("EFFECTHEIGHT")->LoadFromStream(memstream.get(), ftBlob);
        memstream->Clear();

        //for(int i = 0; i < 36 ; i++)  memstream->Write(&effectantennagains.at(i), sizeof(double));
        if (systemcast != ttAM && polarization == plVER)
        {
            sql->ParamByName("ANT_DIAG_H")->Value = Variant();
        }
        else
        {
            for (vi = ant_diag_h.begin(); vi < ant_diag_h.end(); vi++)
                memstream->Write(vi, sizeof(double));
            sql->ParamByName("ANT_DIAG_H")->LoadFromStream(memstream.get(), ftBlob);
            memstream->Clear();
        }

        if (systemcast != ttAM && polarization == plHOR)
        {
            sql->ParamByName("ANT_DIAG_V")->Value = Variant();
        }
        else
        {
            for (vi = ant_diag_v.begin(); vi < ant_diag_v.end(); vi++)
                memstream->Write(vi, sizeof(double));
            sql->ParamByName("ANT_DIAG_V")->LoadFromStream(memstream.get(), ftBlob);
            memstream->Clear();
        }

        sql->ParamByName("IS_DVB_T2")->AsSmallInt = (is_dvb_t2 ? 1 : 0);
        sql->ParamByName("PILOT_PATTERN")->AsSmallInt = pilot_pattern;
        sql->ParamByName("DIVERSITY")->AsSmallInt = diversity;
        sql->ParamByName("ROTATED_CNSTLS")->AsSmallInt = (rotated_constellations ? 1 : 0);
        sql->ParamByName("MODE_OF_EXTNTS")->AsSmallInt = (mode_of_extentions ? 1 : 0);

        sql->ParamByName("MODULATION")->AsSmallInt     = modulation       ;
        sql->ParamByName("CODE_RATE")->AsSmallInt      = code_rate        ;
        sql->ParamByName("FFT_SIZE")->AsSmallInt       = fft_size         ;
        sql->ParamByName("GUARD_INTERVAL")->AsSmallInt = guard_interval   ;


        sql->ExecSQL();
        sql->Close();

        if (systemcast == ttAM)
        {
            static std::auto_ptr<TIBSQL> checkQry(new TIBSQL(NULL));
            if (!checkQry->Prepared)
            {
                checkQry->Database = db;
                checkQry->SQL->Text = "select STA_ID from LFMF_OPER where STA_ID = :STA_ID and DAYNIGHT = :DAYNIGHT";
                checkQry->Prepare();
            }
            static std::auto_ptr<TIBSQL> delQry(new TIBSQL(NULL));
            if (!delQry->Prepared)
            {
                delQry->Database = db;
                delQry->SQL->Text = "delete from LFMF_OPER where STA_ID=:STA_ID and DAYNIGHT=:DAYNIGHT";
                delQry->Prepare();
            }
            static std::auto_ptr<TIBSQL> insQry(new TIBSQL(NULL));
            if (!insQry->Prepared)
            {
                insQry->Database = db;
                insQry->SQL->Text = "insert into LFMF_OPER (STA_ID,DAYNIGHT,BDWDTH,ADJ_RATIO,ANT_TYPE,"
                        "ERP,E_MAX,PWR_KW,AGL,GAIN_AZM) values (:STA_ID,:DAYNIGHT,:BDWDTH,:ADJ_RATIO,:ANT_TYPE,"
                        ":ERP,:E_MAX,:PWR_KW,:AGL,:GAIN_AZM)";
                insQry->Prepare();
            }
            static std::auto_ptr<TIBSQL> updQry(new TIBSQL(NULL));
            if (!updQry->Prepared)
            {
                updQry->Database = db;
                updQry->SQL->Text = "update LFMF_OPER set BDWDTH=:BDWDTH,ADJ_RATIO=:ADJ_RATIO,ANT_TYPE=:ANT_TYPE,"
                        "ERP=:ERP,E_MAX=:E_MAX,PWR_KW=:PWR_KW,AGL=:AGL,GAIN_AZM=:GAIN_AZM"
                        " where STA_ID=:STA_ID and DAYNIGHT=:DAYNIGHT";
                updQry->Prepare();
            }

            if (d_op)
            {
                checkQry->Close();
                checkQry->ParamByName("STA_ID")->AsInteger = id;
                checkQry->ParamByName("DAYNIGHT")->AsString = "HJ";
                checkQry->ExecQuery();

                TIBSQL* sql = checkQry->Eof ? insQry.get() : updQry.get();
                sql->Close();
                sql->ParamByName("STA_ID")->AsInteger   = id;
                sql->ParamByName("DAYNIGHT")->AsString = "HJ";
                sql->ParamByName("BDWDTH")->AsDouble    = d_bw;
                sql->ParamByName("ADJ_RATIO")->AsDouble = d_adj_ratio;
                sql->ParamByName("E_MAX")->AsDouble     = epr_sound_max;
                if (d_ant_type)
                    sql->ParamByName("ANT_TYPE")->AsString  = d_ant_type;
                else
                    sql->ParamByName("ANT_TYPE")->Clear();
                sql->ParamByName("PWR_KW")->AsDouble    = power_sound;
                sql->ParamByName("AGL")->AsInteger      = heightantenna;

                for (vi = ant_diag_h.begin(); vi < ant_diag_h.end(); vi++)
                    memstream->Write(vi, sizeof(double));
                sql->ParamByName("GAIN_AZM")->LoadFromStream(memstream.get());
                memstream->Clear();

                sql->ExecQuery();
            } else {
                delQry->Close();
                delQry->ParamByName("STA_ID")->AsInteger = id;
                delQry->ParamByName("DAYNIGHT")->AsString = "HJ";
                delQry->ExecQuery();
            }

            if (n_op)
            {
                checkQry->Close();
                checkQry->ParamByName("STA_ID")->AsInteger = id;
                checkQry->ParamByName("DAYNIGHT")->AsString = "HN";
                checkQry->ExecQuery();

                TIBSQL* sql = checkQry->Eof ? insQry.get() : updQry.get();
                sql->Close();
                sql->ParamByName("STA_ID")->AsInteger   = id;
                sql->ParamByName("DAYNIGHT")->AsString = "HN";
                sql->ParamByName("BDWDTH")->AsDouble    = n_bw;
                sql->ParamByName("ADJ_RATIO")->AsDouble = n_adj_ratio;
                sql->ParamByName("E_MAX")->AsDouble     = epr_sound_max_night;
                if (n_ant_type)
                    sql->ParamByName("ANT_TYPE")->AsString  = n_ant_type;
                else
                    sql->ParamByName("ANT_TYPE")->Clear();
                sql->ParamByName("PWR_KW")->AsDouble    = power_sound_night;
                sql->ParamByName("AGL")->AsInteger      = heightantenna_night;

                for (vi = ant_diag_h_night.begin(); vi < ant_diag_h_night.end(); vi++)
                    memstream->Write(vi, sizeof(double));
                sql->ParamByName("GAIN_AZM")->LoadFromStream(memstream.get());
                memstream->Clear();

                sql->ExecQuery();
            } else {
                delQry->Close();
                delQry->ParamByName("STA_ID")->AsInteger = id;
                delQry->ParamByName("DAYNIGHT")->AsString = "HN";
                delQry->ExecQuery();
            }
        }

        sql->Transaction->CommitRetaining();

    } catch (std::exception &e) {
        return Error((AnsiString("Помилка запису передавача ID = ") + AnsiString(id) + ": "+e.what()).c_str(), IID_ILISBCTx);
    } catch (Exception &e) {
        return Error((AnsiString("Помилка запису передавача ID = ") + AnsiString(id) + ": "+e.Message).c_str(), IID_ILISBCTx);
    }

    data_changes = false;
    return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_data_changes(long* Value)
{
  try
  {
   *Value = data_changes;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_systemcast(TBCTxType* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = systemcast;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_systemcast(TBCTxType Value)
{
  try
  {
    if (systemcast != Value) data_changes = true;
    systemcast = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_analogtelesystem(long* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = analogtelesystem;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_analogtelesystem(long Value)
{
  try
  {
   if (analogtelesystem != Value) data_changes = true;
   analogtelesystem = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_typeoffset(TBCOffsetType* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = typeoffset;
  }
 catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_typeoffset(TBCOffsetType Value)
{
  try
  {
   if (typeoffset != Value) data_changes = true;
   typeoffset = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_channel_id(long* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = channel_id;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_channel_id(long Value)
{
  try
  {
    if (channel_id != Value) {
        data_changes = true;
        channel_id = Value;
    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_erp(long azimuth, double* power)   //111
{
  try {

    if (!is_fetched)
        reload();
    if (systemcast == ttAM && !detLoaded)
        reload();

    while (azimuth >= 360) azimuth -= 360;
    while (azimuth < 0) azimuth += 360;

    int lower = azimuth / 10;
    int higher = lower + 1;
    if (higher == 36)
        higher = 0;

    if(id < 0)
    {
        *power =  epr_video_max - 30;
    }
    else if (systemcast == ttAM)
    {
        double pwrFrom, pwrTo;
        HRESULT hr = get_effectpowerhor(lower, &pwrFrom);
        if (FAILED(hr))
            return hr;
        hr = get_effectpowerhor(higher, &pwrTo);
        if (FAILED(hr))
            return hr;
        *power = pwrFrom + ((pwrTo - pwrFrom) * (azimuth / 10.0 - lower));
    } else if (polarization == plHOR) {
               *power = effectpowerhor[lower] + (effectpowerhor[higher] - effectpowerhor[lower]) * (azimuth / 10.0 - lower);
    } else if (polarization == plVER) {
        *power = effectpowervert[lower] + (effectpowervert[higher] - effectpowervert[lower]) * (azimuth / 10.0 - lower);
    } else {
        *power = effectpowerhor[lower] + (effectpowerhor[higher] - effectpowerhor[lower]) * (azimuth / 10.0 - lower);

    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
}

STDMETHODIMP TLISBCTxImpl::get_freq_carrier(double* freq)
{
  try
  {

    if (!is_fetched) reload();

    if (systemcast == ttTV) *freq = video_carrier;
    else if (systemcast == ttDAB) *freq = blockcentrefreq;
    else if (systemcast == ttDVB) *freq = video_carrier;
    else if (systemcast == ttFM) *freq = sound_carrier_primary;
    else if (systemcast == ttAM) *freq = sound_carrier_primary;
    else *freq = sound_carrier_primary;
   }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_h_eff(long azimuth, long* height)
{

  try {

    if (!is_fetched) reload();

    while (azimuth >= 360) azimuth -= 360;
    while (azimuth < 0) azimuth += 360;

    int lower = floor(azimuth/10.0);
    int higher = ceil(azimuth/10.0);
    if (higher == 36)
        higher = 0;

    *height = effectheight[lower] + (effectheight[higher] - effectheight[lower]) * (azimuth / 10.0 - lower);

  } catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;

}


STDMETHODIMP TLISBCTxImpl::get_sort_key_in(double* Value)
{
  try
  {
  *Value = sort_key_in;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_sort_key_out(double* Value)
{
  try
  {
  *Value = sort_key_out;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_sort_key_in(double Value)
{
  try
  {
   if (floor(sort_key_in*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   sort_key_in = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_sort_key_out(double Value)
{
  try
  {
   if (floor(sort_key_out*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   sort_key_out = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


//void readBlobIntoVector(TField* blobField, std::vector<double>& array, double defValue = 0.0)
void readBlobIntoVector(TIBSQL *sql, TIBXSQLVAR* blobField, std::vector<double>& array, double defValue = 0.0, TBCTxType systemcast = 0, TBCPolarization polarization = 0)
{
    //TStream *blobStream = blobField->DataSet->CreateBlobStream(blobField, bmRead);
    //std::auto_ptr<TMemoryStream> blobStream (new TMemoryStream);
    //blobField->SaveToStream(blobStream.get());

    if (isc_open_blob2 == NULL)
        MessageBox(NULL, "isc_open_blob2 == NULL", "бля", MB_ICONHAND);

    std::auto_ptr<TIBBlobStream> blobStream (new TIBBlobStream());
    blobStream->Mode = bmRead;
    blobStream->Database = sql->Database;
    blobStream->Transaction = sql->Transaction;
    blobStream->BlobID = blobField->AsQuad;

    int stream_len = blobStream->Size / sizeof(double);
    double *dTemp = new double[stream_len];
    if (dTemp == NULL)
        return;
    try {
        blobStream->ReadBuffer(dTemp, blobStream->Size);
        double* pos = dTemp;
        array.clear();
        bool allZero = true;
        for (int i = 0 ; i < 36; i++) {
            if ((stream_len--) > 0) {
                array.push_back(*(pos++));
                if (array.back() != 0.0)
                    allZero = false;
            } else {
                array.push_back(defValue);
            }
        }

        if (systemcast != 0 && systemcast != ttAM && (polarization == plHOR || polarization == plVER))
        {   if (allZero) {
               for (pos = array.begin(); pos < array.end(); pos++)
                  *pos = defValue;
            }
        }
    } __finally {
        if (dTemp)
            delete[] dTemp;                                               
    }
}

STDMETHODIMP TLISBCTxImpl::reload()
{
    if (id <= 0)
        return S_FALSE;

    if (storage == NULL)
        return Error("Storage interface is NULL", IID_ILISBCTx);

    try {
        HRESULT hr = 0;
        if (!is_fetched)
        {
            hr = storage->LoadObject((IUnknown*)(ILISBCTx*)this);
            if (!SUCCEEDED(hr))
                return hr;
            is_fetched = true;
        } else if (systemcast == ttAM && !detLoaded) {
            hr = storage->LoadDetails((IUnknown*)(ILISBCTx*)this);
            if (!SUCCEEDED(hr))
                return hr;
            detLoaded = true;
        }
    }  catch(Exception &e) {
        return Error((AnsiString("Помилка завантаження передавача: ")+e.Message).c_str(), IID_ILISBCTx);
    }  catch(exception &e) {
        return Error((AnsiString("Помилка завантаження передавача: ")+e.what()).c_str(), IID_ILISBCTx);
    }
    data_changes = false;

    #ifdef _DEBUG
    MessageBeep(0xFFFFFFFF);
    #endif

  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_use_for_calc(VARIANT_BOOL* Value)
{
  try
  {
   *Value = use_for_calc;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_use_for_calc(VARIANT_BOOL Value)
{
  try
  {
  use_for_calc = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_dvb_system(TBCDVBSystem* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = dvb_system;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_dvb_system(TBCDVBSystem Value)
{
  try
  {
   if (dvb_system != Value) data_changes = true;
   dvb_system = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_fm_system(TBCFMSystem* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = fm_system;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_fm_system(TBCFMSystem Value)
{
  try
  {
  if (fm_system != Value) data_changes = true;
  fm_system = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_gaussianchannel(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = gaussianchannel;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_gaussianchannel(double Value)
{
  try
  {
  if (floor(gaussianchannel*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
  gaussianchannel = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_rayleighchannel(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = rayleighchannel;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_riceanchannel(double* Value)
{
  try
  {
    if (!is_fetched) reload();
   *Value = rayleighchannel;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_rayleighchannel(double Value)
{
  try
  {
   if (floor(rayleighchannel*1000+0.5) != floor(Value*1000+0.5)) data_changes = true;
   rayleighchannel = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_riceanchannel(double Value)
{
  try {
    if (floor(riceanchannel*1000+0.5) != floor(Value*1000+0.5)) {
        data_changes = true;
        riceanchannel = Value;
    }
  } catch(Exception &e) {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};

void save_vector(vector<double>& v, ostrstream& ws)
{
    for (vector<double>::iterator vi = v.begin(); vi < v.end(); vi++)
        ws << *vi << ' ';
}

STDMETHODIMP TLISBCTxImpl::saveToString(BSTR* Dest)
{
    try {
    ostrstream out;
    out << "id="                    << (long)id                              << endl;
    out << "systemcast="            << (long)systemcast                << endl;
    out << "longitude="             << longitude                       << endl;
    out << "latitude="              << latitude                        << endl;
    out << "typesystem="            << (long)typesystem                << endl;
    out << "video_carrier="         << video_carrier                  << endl;
    out << "video_offset_line="     << video_offset_line              << endl;
    out << "video_offset_herz="     << video_offset_herz              << endl;
    out << "systemcolour="          << (long)systemcolour             << endl;
    out << "power_video="           << power_video                    << endl;
    out << "epr_video_max="         << epr_video_max                  << endl;
    out << "epr_video_hor="         << epr_video_hor                  << endl;
    out << "epr_video_vert="        << epr_video_vert                 << endl;
    out << "effectpowerhor=";       save_vector(effectpowerhor, out);  out << endl;
    out << "effectpowervert=";      save_vector(effectpowervert, out); out << endl;
    out << "identifiersfn="          << identifiersfn                   << endl;
    out << "relativetiming="        << relativetiming                 << endl;
    out << "blockcentrefreq="       << blockcentrefreq                << endl;
    out << "sound_carrier_primary=" << sound_carrier_primary          << endl;
    out << "sound_offset_primary="  << sound_offset_primary           << endl;
    out << "power_sound="   << power_sound            << endl;
    out << "epr_sound_max=" << epr_sound_max          << endl;
    out << "epr_sound_hor=" << epr_sound_hor          << endl;
    out << "epr_sound_vert="<< epr_sound_vert         << endl;
    out << "v_sound_ratio=" << v_sound_ratio          << endl;
    out << "monostereo="    << monostereo             << endl;
    out << "sound_carrier_second="  << sound_carrier_second           << endl;
    out << "sound_offset_second="   << sound_offset_second            << endl;
    out << "epr_sound_max_second="  << epr_sound_max_second           << endl;
    out << "epr_sound_hor_second="  << epr_sound_hor_second           << endl;
    out << "epr_sound_vert_second=" << epr_sound_vert_second          << endl;
    out << "v_sound_ratio_second="  << v_sound_ratio_second           << endl;
    out << "heightantenna="         << heightantenna                  << endl;
    out << "effectheight=";         save_vector(effectheight, out);     out << endl;
    out << "height_eft_max="        << height_eft_max                 << endl;
    out << "polarization="          << (long)polarization             << endl;
    out << "directivity="             << (long)directivity                << endl;
    out << "fiderloss="             << fiderloss                      << endl;
    out << "fiderlenght="           << fiderlenght                    << endl;
    out << "angleelevation_hor="    << angleelevation_hor             << endl;
    out << "angleelevation_vert="   << angleelevation_vert            << endl;
    out << "antennagain="           << antennagain                    << endl;
    out << "ant_diag_h=";              save_vector(ant_diag_h, out);    out << endl;
    out << "ant_diag_v=";              save_vector(ant_diag_v, out);    out << endl;
    out << "testpointsis="          << testpointsis                   << endl;
    out << "analogtelesystem="      << analogtelesystem               << endl;
    out << "typeoffset="            << (long)typeoffset               << endl;
    out << "channel_id="            << channel_id                     << endl;
    out << "sort_key_in="           << sort_key_in                    << endl;
    out << "sort_key_out="          << sort_key_out                   << endl;
    out << "use_for_calc="          << use_for_calc                   << endl;
//  vector<TBCTestpoint> testpoints;     point> testpoints;            << endl;
    out << "dvb_system="            << (long)dvb_system               << endl;
    out << "fm_system="            << (long)fm_system               << endl;

  WideString wsout(out.str());
  *Dest = wsout.Detach();

    } catch (exception &e) {
        return Error(e.what(), IID_ILISBCTx);
    } catch (Exception &e) {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    } catch (Exception *e) {
        return Error(e->Message.c_str(), IID_ILISBCTx);
    }

  return S_OK;
}

void load_vector(vector<double>& v, AnsiString as)
{
    v.clear();
    v.reserve(36);
    for(int i = 1; i < as.Length(); i++)
        if(as[i] == ' ') as[i] = '\n';
    TStringList* sl = new TStringList();
    try {
        sl->Text = as;
        for(int i = 0; i < 36; i++)
            if (i < sl->Count)   {
                v.push_back(sl->Strings[i].ToDouble());     
                }
            else
                v.push_back(0);
    } __finally {
        delete sl;
    }
}



#define setValue(var, val, type, def)                       \
{                                                           \
    if (!(val.IsEmpty()))                                   \
    {                                                       \
        switch(type) {                                      \
            case varDouble:  var = val.ToDouble(); break;   \
            case varInteger: var = val.ToInt(); break;      \
            default: break;                                 \
        }                                                   \
    }                                                       \
    else var = def;                                         \
}                                                       

STDMETHODIMP TLISBCTxImpl::loadFromString(BSTR Source)
{
//    istrstream in(AnsiString(Source).c_str());
    char oldDecimalSeparator = DecimalSeparator;
    DecimalSeparator = '.';
    TStringList* sl = new TStringList();
    try {
        sl->Text = AnsiString(Source);

        setValue(id, sl->Values["id"], varInteger, 0);
        //id=                       sl->Values["id"].ToInt();
        systemcast=    (TBCTxType)sl->Values["systemcast"].ToInt();
        longitude=                sl->Values["longitude"].ToDouble();
        latitude=                 sl->Values["latitude"].ToDouble();
        typesystem= (TBCTvSystems)sl->Values["typesystem"].ToInt();
        video_carrier=            sl->Values["video_carrier"].ToDouble();
        video_offset_line=        sl->Values["video_offset_line"].ToInt();
        video_offset_herz=        sl->Values["video_offset_herz"].ToInt();
        systemcolour=(TBCTvStandards)sl->Values["systemcolour"].ToInt();
        power_video=              sl->Values["power_video"].ToDouble();
        epr_video_max=            sl->Values["epr_video_max"].ToDouble();
        epr_video_hor=            sl->Values["epr_video_hor"].ToDouble();
        epr_video_vert=           sl->Values["epr_video_vert"].ToDouble();
        load_vector(effectpowerhor, sl->Values["effectpowerhor"]);
        load_vector(effectpowervert, sl->Values["effectpowervert"]);
        identifiersfn=            sl->Values["identifiersfn"].ToDouble();
        relativetiming=           sl->Values["relativetiming"].ToDouble();
        blockcentrefreq=          sl->Values["blockcentrefreq"].ToDouble();
        sound_carrier_primary=    sl->Values["sound_carrier_primary"].ToDouble();
        sound_offset_primary=     sl->Values["sound_offset_primary"].ToDouble();
        power_sound=      sl->Values["power_sound"].ToDouble();
        epr_sound_max=    sl->Values["epr_sound_max"].ToDouble();
        epr_sound_hor=    sl->Values["epr_sound_hor"].ToDouble();
        epr_sound_vert=   sl->Values["epr_sound_vert"].ToDouble();
        v_sound_ratio=    sl->Values["v_sound_ratio"].ToDouble();
        monostereo=       sl->Values["monostereo"].ToDouble();
        sound_carrier_second=     sl->Values["sound_carrier_second"].ToDouble();
        sound_offset_second=      sl->Values["sound_offset_second"].ToDouble();
        epr_sound_max_second=     sl->Values["epr_sound_max_second"].ToDouble();
        epr_sound_hor_second=     sl->Values["epr_sound_hor_second"].ToDouble();
        epr_sound_vert_second=    sl->Values["epr_sound_vert_second"].ToDouble();
        v_sound_ratio_second=     sl->Values["v_sound_ratio_second"].ToDouble();
        heightantenna=            sl->Values["heightantenna"].ToInt();
        load_vector(effectheight, sl->Values["effectheight"]);
        height_eft_max=           sl->Values["height_eft_max"].ToDouble();
        polarization=(TBCPolarization)sl->Values["polarization"].ToInt();
        directivity=  (TBCDirection)sl->Values["directivity"].ToInt();
        fiderloss=                sl->Values["fiderloss"].ToDouble();
        fiderlenght=              sl->Values["fiderlenght"].ToDouble();
        angleelevation_hor=       sl->Values["angleelevation_hor"].ToDouble();
        angleelevation_vert=      sl->Values["angleelevation_vert"].ToDouble();
        antennagain=              sl->Values["antennagain"].ToDouble();
        load_vector(ant_diag_h,      sl->Values["ant_diag_h"]);
        testpointsis=             sl->Values["testpointsis"].ToInt();
        analogtelesystem=         sl->Values["analogtelesystem"].ToInt();
        typeoffset=(TBCOffsetType)sl->Values["typeoffset"].ToInt();
        channel_id=               sl->Values["channel_id"].ToInt();
        sort_key_in=              sl->Values["sort_key_in"].ToDouble();
        sort_key_out=             sl->Values["sort_key_out"].ToDouble();
        use_for_calc=             sl->Values["use_for_calc"].ToInt();
//  vector<TBCTestpoint> testpoints;     point> testpoints;              endl;
        dvb_system= (TBCDVBSystem)sl->Values["dvb_system"].ToInt();
        fm_system=   (TBCFMSystem)sl->Values["fm_system"].ToInt();


    } catch (exception &e) {
        DecimalSeparator = oldDecimalSeparator;
        delete sl;
        return Error(e.what(), IID_ILISBCTx);
    } catch (Exception &e) {
        DecimalSeparator = oldDecimalSeparator;
        delete sl;
        return Error(e.Message.c_str(), IID_ILISBCTx);
    } catch (Exception *e) {
        DecimalSeparator = oldDecimalSeparator;
        delete sl;
        return Error(e->Message.c_str(), IID_ILISBCTx);
    }

    DecimalSeparator = oldDecimalSeparator;
    delete sl;
    return S_OK;
}

String XSQLVARToString (TIBXSQLVAR *fld)
{
    String res;
    if (fld && fld->Data && fld->Data->sqllen > 0 && fld->Data->sqldata)
    {
        short sType = fld->Data->sqltype | 0x0001;
        if (sType != 0x01C1 && sType != 0x01C5)
            MessageBox(NULL, "sqltype != 449 && sqltype != 453", "Оп-па!", MB_ICONWARNING);
        short& sSize = sType == 0x01C1 ? *(short*)(fld->Data->sqldata) : fld->Data->sqllen;
        char *a = new char[sSize + 1];
        a[sSize] = '\0';
        memcpy(a, fld->Data->sqldata + (sType == 0x01C1 ? 2 : 0), sSize);
        res = a;
        delete[] a;
    }
    return res;
}

STDMETHODIMP TLISBCTxImpl::loadFromQuery(long query)
{
    try {
        static const String className("TIBSQL");
        if (className != ((TObject*)query)->ClassName())
        {
            /* TODO: log error*/
            throw *(new Exception("Сбой при вызове loadFromQuery(): это не TIBSQL"));
        }

        TIBSQL *sql = (TIBSQL*)query;

        db = sql->Database;

        TIBXSQLDA *rec = sql->Current();
        bool isDetails = false;
        for (int i = 0; i < rec->Count; i++ )
            if (strcmp("DAYNIGHT", rec->Vars[i]->AsXSQLVAR->sqlname) == 0)
                isDetails = true;

        if (!isDetails)
        {
            longitude =       sql->FieldByName("LONGITUDE")->AsDouble;
            latitude =        sql->FieldByName("LATITUDE")->AsDouble;

            systemcast =      (TBCTxType)sql->FieldByName("SC_ENUMVAL")->AsInteger;

            typesystem = (TBCTvSystems)sql->FieldByName("AT_ENUMVAL")->AsInteger;
            dvb_system = (TBCDVBSystem)sql->FieldByName("DTS_ENUMVAL")->AsInteger;
            lfmf_system = (TBCFMSystem)sql->FieldByName("TYPESYSTEM")->AsInteger;
            fm_system = (TBCFMSystem)sql->FieldByName("ARS_ENUMVAL")->AsInteger;

            channel_id =         sql->FieldByName("CHANNEL_ID")->AsInteger;
            identifiersfn =           sql->FieldByName("IDENTIFIERSFN")->AsInteger;
            relativetiming =          sql->FieldByName("RELATIVETIMINGSFN")->AsInteger;
            blockcentrefreq =         sql->FieldByName("BLOCKCENTREFREQ")->AsDouble;
            sound_carrier_primary =   sql->FieldByName("SOUND_CARRIER_PRIMARY")->AsDouble;
            power_sound =     sql->FieldByName("POWER_SOUND_PRIMARY")->AsDouble;
            epr_sound_max =   sql->FieldByName("EPR_SOUND_MAX_PRIMARY")->AsDouble;
            epr_sound_hor =   sql->FieldByName("EPR_SOUND_HOR_PRIMARY")->AsDouble;
            epr_sound_vert =  sql->FieldByName("EPR_SOUND_VERT_PRIMARY")->AsDouble;
            v_sound_ratio =   sql->FieldByName("V_SOUND_RATIO_PRIMARY")->AsDouble;
            monostereo =      sql->FieldByName("MONOSTEREO_PRIMARY")->AsInteger;
            sound_carrier_second =    sql->FieldByName("SOUND_CARRIER_SECOND")->AsDouble;
            epr_sound_max_second =    sql->FieldByName("EPR_SOUND_MAX_SECOND")->AsDouble;
            epr_sound_hor_second =    sql->FieldByName("EPR_SOUND_HOR_SECOND")->AsDouble;
            epr_sound_vert_second =   sql->FieldByName("EPR_SOUND_VER_SECOND")->AsDouble;
            v_sound_ratio_second =    sql->FieldByName("V_SOUND_RATIO_SECOND")->AsDouble;
            heightantenna =           sql->FieldByName("HEIGHTANTENNA")->AsInteger;
            height_eft_max =          sql->FieldByName("HEIGHT_EFF_MAX")->AsInteger;

            fiderloss =               sql->FieldByName("FIDERLOSS")->AsDouble;
            fiderlenght =             sql->FieldByName("FIDERLENGTH")->AsInteger;
            angleelevation_hor =      sql->FieldByName("ANGLEELEVATION_HOR")->AsInteger;
            angleelevation_vert =     sql->FieldByName("ANGLEELEVATION_VER")->AsInteger;
            antennagain =             sql->FieldByName("ANTENNAGAIN")->AsDouble;

            video_carrier =           sql->FieldByName("VIDEO_CARRIER")->AsDouble;
            video_offset_line = sql->FieldByName("VIDEO_OFFSET_LINE")->AsInteger;
            video_offset_herz = sql->FieldByName("VIDEO_OFFSET_HERZ")->AsInteger;

            power_video =     sql->FieldByName("POWER_VIDEO")->AsDouble;
            epr_video_max = sql->FieldByName("EPR_VIDEO_MAX")->AsDouble;
            epr_video_hor = sql->FieldByName("EPR_VIDEO_HOR")->AsDouble;
            epr_video_vert = sql->FieldByName("EPR_VIDEO_VERT")->AsDouble;

            acin_id = sql->FieldByName("ACCOUNTCONDITION_IN")->AsInteger;
            acout_id = sql->FieldByName("ACCOUNTCONDITION_OUT")->AsInteger;
            stand_id = sql->FieldByName("STAND_ID")->AsInteger;

            testpointsis = sql->FieldByName("TESTPOINTSIS")->AsInteger;

            rpc = (TBcRpc)sql->FieldByName("RPC")->AsInteger;
            rxMode = (TBcRxMode)sql->FieldByName("RX_MODE")->AsInteger;
            pol_isol = sql->FieldByName("POL_ISOL")->AsDouble;

            mod_type = (TBcModType)sql->FieldByName("MOD_TYPE")->AsInteger;
            prot_levl = sql->FieldByName("PROT_LEVL")->AsInteger;
            gnd_cond = sql->FieldByName("GND_COND")->AsDouble;
            noise_zone = sql->FieldByName("NOISE_ZONE")->AsInteger;

            maxCoordDist = sql->FieldByName("MAX_COORD_DIST")->AsFloat;

            TIBXSQLVAR *fld;

            adminid = 0;
            fld = sql->FieldByName("ADMINISTRATIONID");
            PXSQLVAR varVal = fld->Data;
            if (!fld->IsNull)
            {
                char a[] = "\0\0\0\0\0";
                short len = *(short*)(varVal->sqldata);
                memcpy(a, varVal->sqldata + 2, len < 4 ? len : 4);
                adminid = atoi(a);
                //String a = XSQLVARToString(fld);
                //try { adminid = StrToInt(a); } catch(...) {}
            }

            String asNtsc("NTSC");
            String asPal("PAL");
            String str = XSQLVARToString(fld = sql->FieldByName("SYSTEMCOLOUR")).Trim();
            systemcolour = csSECAM;
            if (str == asNtsc)
                systemcolour = csNTSC;
            else if (str == asPal)
                systemcolour = csPAL;

            if (XSQLVARToString(sql->FieldByName("TYPEOFFSET")).Trim() == 'P')
                typeoffset = otPrecision;
            else
                typeoffset = otNonPrecision;

            if (systemcast == ttAM)
                polarization = plHOR;
            else
            {
                str = XSQLVARToString(sql->FieldByName("POLARIZATION")).Trim();
                if (str == 'V') polarization = plVER;
                    else if (str == 'H') polarization = plHOR;
                    else if (str == 'M') polarization = plMIX;
                    else  polarization = plVER;
            }

            if (XSQLVARToString(sql->FieldByName("DIRECTION")).Trim() == 'D') directivity = drD;
                else directivity = drND;

            double maxErp = MINDOUBLE;
            if (systemcast == ttTV || systemcast == ttDVB)
                maxErp = epr_video_max;
            else //if (systemcast == ttFM || systemcast == ttDAB)
                maxErp = epr_sound_max;

            readBlobIntoVector(sql, sql->FieldByName("EFFECTPOWERHOR"), effectpowerhor, maxErp, systemcast, polarization);
            readBlobIntoVector(sql, sql->FieldByName("EFFECTPOWERVER"), effectpowervert, maxErp, systemcast, polarization);
            readBlobIntoVector(sql, sql->FieldByName("EFFECTHEIGHT"), effectheight, height_eft_max != 0.0 ? height_eft_max : heightantenna);
            readBlobIntoVector(sql, sql->FieldByName("ANT_DIAG_H"), ant_diag_h);
            readBlobIntoVector(sql, sql->FieldByName("ANT_DIAG_V"), ant_diag_v);

            typesystem_id = sql->FieldByName("typesystem")->AsInteger;
            adm_response = WideString(XSQLVARToString(sql->FieldByName("adm_response")));
            adm_sited_in = WideString(XSQLVARToString(sql->FieldByName("adm_sited_in")));
            channel = WideString(XSQLVARToString(sql->FieldByName("channel")));
            date_of_last_change = sql->FieldByName("date_of_last_change")->AsDateTime;
            site_height = sql->FieldByName("site_height")->AsInteger;
            station_name = WideString(XSQLVARToString(sql->FieldByName("station_name")));
            status_code = sql->FieldByName("status_code")->AsInteger;
            numregion = WideString(XSQLVARToString(sql->FieldByName("numregion")));
            bandwidth = sql->FieldByName("BANDWIDTH")->AsDouble;

            assoc_allot_id = WideString(XSQLVARToString(sql->FieldByName("associated_adm_allot_id")));

            is_dvb_t2 = sql->FieldByName("IS_DVB_T2")->AsShort;
            pilot_pattern = sql->FieldByName("PILOT_PATTERN")->AsShort;
            diversity = sql->FieldByName("DIVERSITY")->AsShort;
            rotated_constellations = sql->FieldByName("ROTATED_CNSTLS")->AsShort;
            mode_of_extentions = sql->FieldByName("MODE_OF_EXTNTS")->AsShort;

            modulation = sql->FieldByName("MODULATION")->AsShort;
            code_rate = sql->FieldByName("CODE_RATE")->AsShort;
            fft_size = sql->FieldByName("FFT_SIZE")->AsShort;
            guard_interval = sql->FieldByName("GUARD_INTERVAL")->AsShort;       

            is_fetched = true;
            data_changes = false;
            detLoaded = false;

        } else if (systemcast == ttAM) {
            d_op = false;
            n_op = false;
            epr_sound_hor = -999.;
            while (!sql->Eof && sql->FieldByName("OBJ_ID")->AsInteger == id)
            {
                bool isDay = XSQLVARToString(sql->FieldByName("DAYNIGHT")) == "HJ";
                bool isNight = XSQLVARToString(sql->FieldByName("DAYNIGHT")) == "HN";
                if (isDay)
                {
                    d_bw        = sql->FieldByName("BDWDTH")->AsDouble;
                    d_op        = true;
                    d_adj_ratio = sql->FieldByName("ADJ_RATIO")->AsDouble;
                    String s = sql->FieldByName("ANT_TYPE")->AsString;
                    d_ant_type = s.Length() > 0 ? s[1] : '\0';
                    epr_sound_max = sql->FieldByName("E_MAX")->AsDouble;
                    power_sound = sql->FieldByName("PWR_KW")->AsDouble;
                    heightantenna = sql->FieldByName("AGL")->AsInteger;
                    readBlobIntoVector(sql, sql->FieldByName("GAIN_AZM"), ant_diag_h);
                } else if (isNight) {
                    n_bw        = sql->FieldByName("BDWDTH")->AsDouble;
                    n_op        = true;
                    n_adj_ratio = sql->FieldByName("ADJ_RATIO")->AsDouble;
                    String s = sql->FieldByName("ANT_TYPE")->AsString;
                    n_ant_type  = s.Length() > 0 ? s[1] : '\0';
                    epr_sound_max_night = sql->FieldByName("E_MAX")->AsDouble;
                    power_sound_night = sql->FieldByName("PWR_KW")->AsDouble;
                    heightantenna_night = sql->FieldByName("AGL")->AsInteger;
                    readBlobIntoVector(sql, sql->FieldByName("GAIN_AZM"), ant_diag_h_night);
                }
                sql->Next();
            }
            if(d_op)
                is_day = 1;
            else
                if(n_op)
                    is_day = 0;
            detLoaded = true;
            RecalcErp();

        }
        else is_day = 1; //если systemcast != ttAM

    } catch(Exception &e) {
        return Error((__FUNC__"():\n"+e.Message).c_str(), IID_ILISBCTx);
    }
    return S_OK;
}

STDMETHODIMP TLISBCTxImpl::get_acin_id(long* Value)
{
    try {
        if (!is_fetched) reload();
        *Value = acin_id;
    } catch(Exception &e) {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_acout_id(long* Value)
{
    try {
        if (!is_fetched) reload();
        *Value = acout_id;
    } catch(Exception &e) {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_is_fetched(VARIANT_BOOL* Value)
{
    *Value = is_fetched;
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_adminid(long* Value)
{
    try {
        if (!is_fetched) reload();
        *Value = adminid;
    } catch(Exception &e) {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
};

 
STDMETHODIMP TLISBCTxImpl::get_stand_id(long* Value)
{
    try {
        if (!is_fetched) reload();
        *Value = stand_id;
    } catch(Exception &e) {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_acin_id(long Value)
{
    acin_id = Value;
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_acout_id(long Value)
{
    acout_id = Value;
    return S_OK;
};

STDMETHODIMP TLISBCTxImpl::set_adminid(long Value)
{
    try {
        adminid = Value;
    } catch(Exception &e) {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_stand_id(long Value)
{
    stand_id = Value;
    return S_OK;
};



STDMETHODIMP TLISBCTxImpl::invalidate()
{
    is_fetched = false;
    is_day = -1;
    return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_database(long* Value)
{
  try
  {
    //*Value = database;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_database(long Value)
{
  try
  {
    //database = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxImpl::get_typesystem_id(long* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = typesystem_id;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_typesystem_id(long Value)
{
  try
  {
    if (typesystem_id != Value)
    {
        typesystem_id = Value;
        data_changes = true;
    }
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_adm_response(BSTR* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = WideString(adm_response).Detach();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_adm_sited_in(BSTR* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = WideString(adm_sited_in).Detach();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_channel(BSTR* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = WideString(channel).Detach();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_date_of_last_change(DATE* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = date_of_last_change;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_site_height(long* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = site_height;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_station_name(BSTR* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = WideString(station_name).Detach();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_status_code(long* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = status_code;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_numregion(BSTR* Value)
{
  try
  {
    if (!is_fetched) reload();
    *Value = WideString(numregion).Detach();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_maxCoordDist(double* Value)
{
  try
  {
    if (!is_fetched) reload();
        *Value = maxCoordDist;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_maxCoordDist(double Value)
{
  try
  {
    maxCoordDist = Value;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxImpl::set_station_name(BSTR Value)
{
  try
  {
    station_name = WideString(Value);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
    return S_OK;
}


STDMETHODIMP TLISBCTxImpl::set_channel_name(BSTR Value)
{
  try
  {
    channel = WideString(Value);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
    return S_OK;
}


STDMETHODIMP TLISBCTxImpl::get_rpc(TBcRpc* Value)
{
    *Value = rpc;
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_rxMode(TBcRxMode* Value)
{
    *Value = rxMode;
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_rpc(TBcRpc Value)
{
  if (Value < rpc1 || Value > rpc0)
  {
    return Error(AnsiString().sprintf("Значение RPC (%d) вне допустимых пределов", Value).c_str(), IID_ILISBCTx);
  }
  if (Value != rpc0 && systemcast == ttDVB && Value > rpc3 || systemcast == ttDAB && Value < rpc4 && Value != rpc0)
  {
    return Error(AnsiString().sprintf("Значение RPC (%d) не подходит для данного типа присвоения", Value).c_str(), IID_ILISBCTx);
  }
  if (rpc != Value)
  {
    rpc = Value;
    data_changes = true;
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_rxMode(TBcRxMode Value)
{
  if (Value < rmFx || Value > rmPo)
  {
    return Error(AnsiString().sprintf("Значение Типа Приёма (%d) вне допустимых пределов", Value).c_str(), IID_ILISBCTx);
  }
  if (rxMode != Value)
  {
    rxMode = Value;
    data_changes = true;
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::attribs()
{
  try
  {

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_attribsDs(long* Value)
{
  try
  {

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_coord(BSTR* Value)
{
  try
  {

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_coord(BSTR Value)
{
  try
  {

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



STDMETHODIMP TLISBCTxImpl::get_gain_h(long idx, double* Value)
{
    *Value = 0;
    if (directivity == drND && systemcast != ttAM)
    {
        *Value = antennagain;
        return S_OK;
    } else if (idx >= 0 && idx < 36)
    {
        if (!is_fetched)
            reload();
        if (systemcast != ttAM)
            *Value = ant_diag_h[idx];
        else
        {
            if (!detLoaded)
                reload();
            if(is_day == -1)
                return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
            if ((is_day || !n_op) && !ant_diag_h.empty())
                *Value = ant_diag_h[idx];
            else if (!ant_diag_h_night.empty())
                *Value = ant_diag_h_night[idx];
        }
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::set_gain_h(long idx, double Value)
{
    if (directivity == drND && systemcast != ttAM)
        return S_FALSE;
    else if (idx >= 0 && idx < 36)
    {
        if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        vector<double>& fld = (systemcast != ttAM || is_day || !n_op) ? ant_diag_h : ant_diag_h_night;
        if (fld.empty())
        {
            data_changes = true;
            double val = 0.;
            fld.insert(fld.begin(), 36, val);
        }
        if (Value != fld[idx])
            data_changes = true;
        fld[idx] = Value;
        RecalcErp();
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::get_gain_v(long idx, double* Value)
{
    if (systemcast == ttAM)
        return S_FALSE;
    if (directivity == drND)
    {
        *Value = antennagain;
        return S_OK;
    } else if (idx >= 0 && idx < 36)
    {
        if (!is_fetched) reload();
        *Value = ant_diag_v[idx];
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::set_gain_v(long idx, double Value)
{
    if (directivity == drND || systemcast == ttAM)
        return S_FALSE;
    else if (idx >= 0 && idx < 36)
    {
        double& fld = ant_diag_v[idx];
        if (Value != fld)
            data_changes = true;
        fld = Value;
        if (antennagain < Value)
            antennagain = Value;
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::get_discr_h(long idx, double* Value)
{
    *Value = 0;
    if (directivity == drND || systemcast == ttAM)
    {
        return S_OK;
    } else if (idx >= 0 && idx < 36)
    {
        if (!is_fetched) reload();
        if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        double& fld = (systemcast != ttAM || is_day || !n_op) ? ant_diag_h[idx] : ant_diag_h_night[idx];
        *Value = antennagain - fld;
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::set_discr_h(long idx, double Value)
{
    if (directivity == drND || systemcast == ttAM)
        return S_FALSE;
    else if (idx >= 0 && idx < 36)
    {
        if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
        double& fld = (systemcast != ttAM || is_day || !n_op) ? ant_diag_h[idx] : ant_diag_h_night[idx];
        if (antennagain - Value != fld)
            data_changes = true;
        fld = antennagain - Value;
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::get_discr_v(long idx, double* Value)
{
    *Value = 0;
    if (directivity == drND || systemcast == ttAM)
    {
        return S_OK;
    } else if (idx >= 0 && idx < 36)
    {
        if (!is_fetched) reload();
        double& fld = ant_diag_v[idx];
        *Value = antennagain - fld;
        return S_OK;
    } else
        return S_FALSE;
};

STDMETHODIMP TLISBCTxImpl::set_discr_v(long idx, double Value)
{
    if (directivity == drND || systemcast == ttAM)
        return S_FALSE;
    else if (idx >= 0 && idx < 36)
    {
        double& fld = ant_diag_v[idx];
        if (antennagain - Value != fld)
            data_changes = true;
        fld = antennagain - Value;
        return S_OK;
    } else
        return S_FALSE;
};

void TLISBCTxImpl::reset_diags()
{
    std::vector<double>::iterator vi;
    std::vector<double>& fldh = (systemcast != ttAM || is_day || !n_op) ? ant_diag_h : ant_diag_h_night;
    //if (polarization != plVER)
        for (vi = fldh.begin(); vi < fldh.end(); vi++)
            *vi = antennagain;
    std::vector<double>& fldv = ant_diag_v;
    //if (polarization != plHOR)
        for (vi = fldv.begin(); vi < fldv.end(); vi++)
            *vi = antennagain;
}

STDMETHODIMP TLISBCTxImpl::get_pol_isol(double* Value)
{
    return GetFieldVal<double>(Value, pol_isol, IID_ILISBCTx);
};

STDMETHODIMP TLISBCTxImpl::set_pol_isol(double Value)
{
    return SetFieldVal<double>(pol_isol, Value);
};

STDMETHODIMP TLISBCTxImpl::get_lfmf_system(long* Value)
{
    return GetFieldVal<long>(Value, lfmf_system, IID_ILisBcLfMf);
};

STDMETHODIMP TLISBCTxImpl::set_lfmf_system(long Value)
{
    return SetFieldVal<long>(lfmf_system, Value);
};

STDMETHODIMP TLISBCTxImpl::get_lfmf_bw(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    if (n_op && !is_day)
        *Value = n_bw;
    else
        *Value = d_bw;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisBcLfMf);
  }
  return S_OK;
};

STDMETHODIMP TLISBCTxImpl::set_lfmf_bw(double Value)
{
    if(systemcast == ttAM && is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    double &fld = (n_op && !is_day) ? n_bw : d_bw;
    if (fld != Value)
    {
        fld = Value;
        data_changes = true;
    }
    return S_OK;
};

STDMETHODIMP TLISBCTxImpl::get_day_op(VARIANT_BOOL* Value)
{
    return GetFieldVal<VARIANT_BOOL>(Value, d_op, IID_ILisBcLfMf);
};

STDMETHODIMP TLISBCTxImpl::get_is_day(VARIANT_BOOL* Value)
{
    *Value = is_day;
    return S_OK;
};

STDMETHODIMP TLISBCTxImpl::get_night_op(VARIANT_BOOL* Value)
{
    return GetFieldVal<VARIANT_BOOL>(Value, n_op, IID_ILisBcLfMf);
};

STDMETHODIMP TLISBCTxImpl::set_day_op(VARIANT_BOOL Value)
{
    return SetFieldVal<VARIANT_BOOL>(d_op, Value);
};

STDMETHODIMP TLISBCTxImpl::set_is_day(VARIANT_BOOL Value)
{
    if(Value && d_op)
        is_day = Value;
    else
    {
        if(!Value && n_op)
            is_day = Value;
        else
            is_day = -1;
    }
    if (!is_day && id < 0)
        n_op = true;
    return S_OK;
};

STDMETHODIMP TLISBCTxImpl::set_night_op(VARIANT_BOOL Value)
{
    return SetFieldVal<VARIANT_BOOL>(n_op, Value);
};

STDMETHODIMP TLISBCTxImpl::get_adj_ratio(double* Value)
{
  try
  {
    if (!is_fetched) reload();
    if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    if (n_op && !is_day)
        *Value = n_adj_ratio;
    else
        *Value = d_adj_ratio;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisBcLfMf);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_ant_type(unsigned_char* Value)
{
  try
  {
    if (!is_fetched) reload();
    if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    if (n_op && !is_day)
        *Value = n_ant_type;
    else
        *Value = d_ant_type;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisBcLfMf);
  }
  return S_OK;
};


STDMETHODIMP TLISBCTxImpl::get_gnd_cond(double* Value)
{
    return GetFieldVal<double>(Value, gnd_cond, IID_ILisBcLfMf);
};


STDMETHODIMP TLISBCTxImpl::get_noise_zone(long* Value)
{
    return GetFieldVal<long>(Value, noise_zone, IID_ILisBcLfMf);
};


STDMETHODIMP TLISBCTxImpl::set_adj_ratio(double Value)
{
    if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    double &fld = (n_op && !is_day) ? n_adj_ratio : d_adj_ratio;
    if (fld != Value)
    {
        fld = Value;
        data_changes = true;
    }
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_ant_type(unsigned_char Value)
{
    if(is_day == -1)
            return Error(__FUNC__"(): wrong data or incorrect operation mode", IID_ILISBCTx);
    char &fld = (n_op && !is_day) ? n_ant_type : d_ant_type;
    if (fld != Value)
    {
        fld = Value;
        data_changes = true;
        RecalcErp();
    }
    return S_OK;
};


STDMETHODIMP TLISBCTxImpl::set_gnd_cond(double Value)
{
    return SetFieldVal<double>(gnd_cond, Value);
};

STDMETHODIMP TLISBCTxImpl::set_noise_zone(long Value)
{
    return SetFieldVal<long>(noise_zone, Value);
};

STDMETHODIMP TLISBCTxImpl::get_mod_type(long* Value)
{
    return GetFieldVal<long>(Value, mod_type, IID_ILisBcLfMf);
};

STDMETHODIMP TLISBCTxImpl::set_mod_type(long Value)
{
    return SetFieldVal<long>((long&)mod_type, Value);
};

STDMETHODIMP TLISBCTxImpl::get_prot_levl(long* Value)
{
    return GetFieldVal<long>(Value, prot_levl, IID_ILisBcLfMf);
};

STDMETHODIMP TLISBCTxImpl::set_prot_levl(long Value)
{
    return SetFieldVal<long>(prot_levl, Value);
};

void TLISBCTxImpl::RecalcErp()
{
    if (systemcast == ttAM)
    {
        double pwr_dbkw = (d_ant_type == 'B') ? (power_sound > 0. ? 10. * log10(power_sound) : -999.) : epr_sound_max;
        double pwr_dbkw_night = (n_ant_type == 'B') ? (power_sound_night > 0. ? 10. * log10(power_sound_night) : -999.) : epr_sound_max_night;

        if (effectpowerhor.empty())
            effectpowerhor.insert(effectpowerhor.begin(), 36, pwr_dbkw);
        if (effectpowerhor_night.empty())
            effectpowerhor_night.insert(effectpowerhor_night.begin(), 36, pwr_dbkw_night);
        for (int i = 0; i < ant_diag_h.size(); i++)
            effectpowerhor[i] = pwr_dbkw + ((d_ant_type == 'B') ? ant_diag_h[i] : 0.);
        for (int i = 0; i < ant_diag_h_night.size(); i++)
            effectpowerhor_night[i] = pwr_dbkw_night + ((n_ant_type == 'B') ? ant_diag_h_night[i] : 0.);
    }
}

STDMETHODIMP TLISBCTxImpl::get_fxm_bandwidth(double* Value)
{
    return GetFieldVal<double>(Value, bandwidth, IID_ILisBcFxm);
};


STDMETHODIMP TLISBCTxImpl::get_fxm_system(unsigned_long* Value)
{
    return GetFieldVal<unsigned_long>(Value, typesystem_id, IID_ILisBcFxm);
};


STDMETHODIMP TLISBCTxImpl::set_fxm_bandwidth(double Value)
{
    return SetFieldVal<double>(bandwidth, Value);
};


STDMETHODIMP TLISBCTxImpl::set_fxm_system(unsigned_long Value)
{
    return SetFieldVal<long>(typesystem_id, Value);
};



STDMETHODIMP TLISBCTxImpl::GetAssAllotId(BSTR* assAllId)
{
    *assAllId = WideString(assoc_allot_id).Detach();
    return S_OK;
}


STDMETHODIMP TLISBCTxImpl::get_Diversity(unsigned_long* Value)
{
    return GetFieldVal<unsigned_long>(Value, diversity, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_IsDvbt2(VARIANT_BOOL* Value)
{
    return GetFieldVal<VARIANT_BOOL>(Value, is_dvb_t2, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_ModeOfExtentions(VARIANT_BOOL* Value)
{
    return GetFieldVal<VARIANT_BOOL>(Value, mode_of_extentions, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_PilotPattern(unsigned_long* Value)
{
    return GetFieldVal<unsigned_long>(Value, pilot_pattern, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_RotatedConstellations(VARIANT_BOOL* Value)
{
    return GetFieldVal<VARIANT_BOOL>(Value, rotated_constellations, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::set_Diversity(unsigned_long Value)
{
    return SetFieldVal<unsigned_long>(diversity, Value);
};


STDMETHODIMP TLISBCTxImpl::set_IsDvbt2(VARIANT_BOOL Value)
{
    return SetFieldVal<VARIANT_BOOL>(is_dvb_t2, Value);
};


STDMETHODIMP TLISBCTxImpl::set_ModeOfExtentions(VARIANT_BOOL Value)
{
    return SetFieldVal<VARIANT_BOOL>(mode_of_extentions, Value);
};


STDMETHODIMP TLISBCTxImpl::set_PilotPattern(unsigned_long Value)
{
    return SetFieldVal<unsigned_long>(pilot_pattern, Value);
};


STDMETHODIMP TLISBCTxImpl::set_RotatedConstellations(VARIANT_BOOL Value)
{
    return SetFieldVal<VARIANT_BOOL>(rotated_constellations, Value);
};



STDMETHODIMP TLISBCTxImpl::get_CodeRate(TCodeRate* Value)
{
    return GetFieldVal<TCodeRate>(Value, code_rate, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_FftSize(TFftSize* Value)
{
    return GetFieldVal<TFftSize>(Value, fft_size, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_GuardInterval(TGuardInterval2* Value)
{
    return GetFieldVal<TGuardInterval2>(Value, guard_interval, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::get_Modulation(TModulation* Value)
{
    return GetFieldVal<TModulation>(Value, modulation, IID_ILisBcDvbt2);
};


STDMETHODIMP TLISBCTxImpl::set_CodeRate(TCodeRate Value)
{
    return SetFieldVal<TCodeRate>(code_rate, Value);
};


STDMETHODIMP TLISBCTxImpl::set_FftSize(TFftSize Value)
{
    return SetFieldVal<TFftSize>(fft_size, Value);
};


STDMETHODIMP TLISBCTxImpl::set_GuardInterval(TGuardInterval2 Value)
{
    return SetFieldVal<TGuardInterval2>(guard_interval, Value);
};


STDMETHODIMP TLISBCTxImpl::set_Modulation(TModulation Value)
{
    return SetFieldVal<TModulation>(modulation, Value);
};



