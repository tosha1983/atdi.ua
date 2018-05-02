unit Transmitter;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, DBTables, Math, p1546, G1, CocR370_TLB, ua50, profile;
const
  RECEIVER_HEIGHT = 10;
  STEP = 0.3;
type
  TCodeString = string[10];
  TTxType = (ttUNKNOWN, ttTV, ttFM, ttAM, ttDVB, ttDAB);
  TTvSystems = (tvB, tvG, tvH, tvI, tvD, tvD1, tvK, tvK1, tvL);
  TTvStandards = (csSECAM, csPAL, csNTSC, csNA); // NA - unknown colour system
  TBand = (VHF, UHF);
  TSoundCarrier = (sndAM, sndFM);         // модуляция звука
  TInterferenceType = (T, C);       // тип помехи - тропосферная или длительная
  TOffsetType = (otP, otNP);            // тип СНЧ - precision, non-precision
  TString2 = string[2];
  TString32 = string[32];
  TTxArray = array [0..36] of double;      // данные (ефф. высоты или ЕИМ) по направлениям от 10 до 360 град.
  TCalcModel = (MODEL_P370, MODEL_P1546, MODEL_FREESPACE);
  TPolar = (pVER, pHOR, pMIX, pCIR);       // antenna polarization - VERTICAL, HORIZONTAL, CIRCULAR

  TCalcParams = record
    pt: byte;
    pm: byte;
    use_hef: boolean; // use Hef    // использовать значения Нэфф из базы данных, а не из карты высот
    use_rel: boolean; // use relief
    use_mor: boolean; // use morphology data (учет моря)
    use_ua50: boolean;
    color: TColor;    // цвет зоны на карте
    az_start: double;
    az_finish: double;
    az_prec: double; // precision - точность по азимуту
    d_prec: double;  // precision - точность по дистанции
    grid_step: double;
    model: TCalcModel;
//    gtopo_path: string
  end;

  TTxAttributes = record
    _id: integer;        // id from database
    _f: double;          // frequency
    _sys: TTvSystems;    //
    _std: TTvStandards;  //
    _snd: TSoundCarrier;
    _offset_type: TOffsetType;
    _ch: integer;
    _code: TCodeString;

    _v_carrier: double;       // MHz
    _v_offset: double;        // MHz
    _s_carrier: double;       // MHz
    _s_offset: double;        // MHz
    _v_power: double;         // Watt
    _s_power: double;         // Watt

    _v_erp_max: double;       // dB kW
    _v_erp_ver: double;       // dB kW
    _v_erp_hor: double;       // dB kW
    _s_erp_max: double;       // dB kW
    _s_erp_ver: double;       // dB kW
    _s_erp_hor: double;       // dB kW

    _gain: double;            // усиление антенны
    _direction: char;         // направленность антенны (D / N - направленная (D_irection) / ненапр. (N_on direction));
    _lon: double;
    _lat: double;
    _polar: TPolar;
    _h: integer;              // Tx height (m)
    _mono_stereo: char;       // 'M' - mono, 'S' - stereo
    _tx_type_str: TString2;   // 'TV', 'FM', 'AM'
    _type: TTxType;
    _erp: double;             // Power (dB kW)
    _emin: double;            // Emin

    _tv_band: byte;           // I, II, III, IV, V tv-bands
    _band: TBand;             // VHF, UHF
    _name: TString32;         //
    _erp_az_h: TTxArray;         // powers by azimuth (horizontal)
    _erp_az_v: TTxArray;         // powers by azimuth (vertical)
    _hef_az: TTxArray;           // Hef by azimuth
    _gain_az: TTxArray;          // Antenna Gain by azimuth

    _erp_az_empty: boolean;
    _hef_az_empty: boolean;
    _gain_az_empty: boolean;
    _fider_loss: double;       // потери в фидере;
    _fider_length: double;     // длина фидера;

    _signal: byte;             // signal type
    _h_max: integer;
    _sort_key: double;
    _sort_key2: double;
    _sort_key3: double;
    _useua50: boolean;
    _use_for_calc: boolean;
    _use_for_duel: boolean;
    _status: integer;
    _distance: double; // дистанция до планируемого или мешающего передатчика (можно использовать в списке передатчиков типа TTxList)
  end;

  PTxAttributes = ^TTxAttributes;

  TTx = class (TObject)
  private
    _att: TTxAttributes;

    _profile: TProfile;
    _R370: R370;
    _pr_c: double;            // protection ratio (continuous)
    _pr_t: double;            // protection ratio (tropospheric)

    procedure AddErp (tx_id, az: integer; val_h, val_v: double; query: TQuery);
    procedure AddGain (tx_id, az: integer; val: double; query: TQuery);
    procedure AddHef (tx_id, az: integer; val: double; query: TQuery);
//    procedure UpdateErpTab (tx_id, az: integer; val: double; query: TQuery);
//    procedure UpdateGainTab (tx_id, az: integer; val: double; query: TQuery);
//    procedure UpdateHefTab (tx_id, az: integer; val: double; query: TQuery);
    procedure UpdateErpArray;
    function GetEmin: double;
  protected
    procedure SetUseUa50(val: boolean);
   procedure SetHMax(val: integer);
   procedure SetCode(val: TCodeString);
   procedure SetId(val: integer);
   procedure SetF(val: double);          // frequency
   procedure SetSys(val: TTvSystems);    //
   procedure SetStd(val: TTvStandards);   //
   procedure SetSnd(val: TSoundCarrier);
   procedure SetOffset_type(val: TOffsetType);
   procedure SetCh(val: integer);
   procedure SetV_carrier(val: double);        // MHz
   procedure SetV_offset(val: double);         // MHz
   procedure SetS_carrier(val: double);        // MHz
   procedure SetS_offset(val: double);         // MHz
   procedure SetV_power(val: double);               // Watt
   procedure SetS_power(val: double);               // Watt
//   procedure SetV_erp(val: double);               // dB kW
   procedure Set_S_erp_max(val: double);                // dB kW
   procedure Set_S_erp_ver(val: double);                // dB kW
   procedure Set_S_erp_hor(val: double);                // dB kW
   procedure Set_V_erp_max(val: double);                // dB kW
   procedure Set_V_erp_ver(val: double);                // dB kW
   procedure Set_V_erp_hor(val: double);                // dB kW
//   procedure SetS_erp(val: double);               // dB kW
   procedure SetAntennaGain(val: double);
   procedure SetLon(val: double);
   procedure SetLat(val: double);
   procedure SetPolar(val: TPolar);
   procedure SetH(val: integer);               // Tx height (m)
   procedure SetMono_stereo(val: char);        // 'M' - mono, 'S' - stereo
   procedure SetTxTypeStr(val: TString2);      // 'TV', 'FM', 'AM'
   procedure SetTxType(val: TTxType);
//   procedure SetErp(val: double);            // Power (dB kW)
   procedure Setname(val: TString32);          //
   procedure Setfider_loss(val: double);       // потери в фидере);
   procedure Setfider_length(val: double);     // длина фидера);
   procedure SetSortKey(val: double);
   procedure SetSortKey2(val: double);
   procedure SetSortKey3(val: double);
   procedure SetSignal(val: byte);
    procedure SetDir(val: char);
    procedure SetUseForCalc(val: boolean);
    procedure SetUseForDuel(val: boolean);
    procedure SetDistance(val: double);
//    procedure SetStatus(val: integer);

  public
    constructor Create(tx_id: integer; query: TQuery);
    function GetHefFromMap(lon, lat, az: double): integer;
    procedure SetGain(az: integer; gain: double);
    function GetErp(azimuth: double): double;
    function GetGain(azimuth: double): double;
    function GetHef(azimuth: integer): double;
    function GetE(plon, plat: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
    function GetEByDistance(azimuth, d: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
    function GetE_1546(plon, plat: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
    function GetE_370(plon, plat: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
    procedure LoadTx(tx_id: integer; query: TQuery);
    procedure SaveTx(tx_id: integer; query: TQuery);
    procedure SetDefaults;
//    procedure CalcHef;
//    function GetE_FreeSpace(plon, plat: double): double;
    function GetE_FreeSpace(plon, plat: double; param: TCalcParams): double;
    procedure SetProtect(pr_c, pr_t: double);
    procedure GetAtt(p: PTxAttributes);
    procedure SetAtt(p: PTxAttributes);
    function GetAttStr: string;
    procedure LoadTxFromTVA(tva: string);
    function GetDataFromTVA(tva: string; start, width: integer): string;

  published
    property Code: TCodeString read _att._code write SetCode;
    property ID: integer read _att._id write SetID;
    property F: double read _att._f; // write SetF;                               // frequency
    property sys: TTvSystems read _att._sys write SetSys;                       //
    property std: TTvStandards read _att._std write SetStd;                     //
    property snd: TSoundCarrier read _att._snd write SetSnd;
    property offset_type: TOffsetType read _att._offset_type write SetOffset_type;
    property VCarrier: double read _att._v_carrier write SetV_carrier;               // MHz
    property VOffset: double read _att._v_offset write SetV_offset;                 // MHz
    property SCarrier: double read _att._s_carrier write SetS_carrier;               // MHz
    property SOffset: double read _att._s_offset write SetS_offset;                 // MHz
    property VPower: double read _att._v_power write SetV_power;               // Watt
    property SPower: double read _att._s_power write SetS_power;               // Watt
    property Lon: double read _att._lon write SetLon;
    property Lat: double read _att._lat write SetLat;
    property H: integer read _att._h write SetH;                              // Tx height (m)
    property Ch: integer read _att._ch write SetCh;                              // Tx Channel
    property MonoStereo: char read _att._mono_stereo write SetMono_stereo;             // 'M' - mono, 'S' - stereo
    property TxTypeStr: TString2 read _att._tx_type_str write SetTxTypeStr;                 // 'TV', 'FM', 'AM'
    property TxType: TTxType read _att._type write SetTxType;
    property Erp: double read _att._erp ; //write SetErp;                           // Power (dB kW)

    property SErpMax: double read _att._s_erp_max write Set_S_Erp_max;
    property SErpHor: double read _att._s_erp_hor write Set_S_Erp_hor;
    property SErpVer: double read _att._s_erp_ver write Set_S_Erp_ver;
    property VErpMax: double read _att._v_erp_max write Set_V_Erp_max;
    property VErpHor: double read _att._v_erp_hor write Set_V_Erp_hor;
    property VErpVer: double read _att._v_erp_ver write Set_V_Erp_ver;

    property Emin: double read _att._emin;                         // Emin
    property Tv_band: byte read _att._tv_band;                     // I, II, III, IV, V tv-bands
    property Band: TBand read _att._band;                          // VHF, UHF
    property Name: TString32 read _att._name write SetName;                      //
    property Pr_c: double read _pr_c;                         // protection ratio
    property Pr_t: double read _pr_t;                         // protection ratio
    property FiderLoss: double read _att._fider_loss write SetFider_loss;
    property FiderLength: double read _att._fider_length write SetFider_length;
    property Polar: TPolar read _att._polar write SetPolar;                       // polarization
    property Signal: byte read _att._signal write SetSignal;
    property Direction: char read _att._direction write SetDir;
    property Gain: double read _att._gain write SetAntennaGain;
    property HMax: integer read _att._h_max write SetHMax;
    property SortKey: double read _att._sort_key write SetSortKey;
    property SortKey2: double read _att._sort_key2 write SetSortKey2;
    property SortKey3: double read _att._sort_key3 write SetSortKey3;
    property UseUA50: boolean read _att._useua50 write SetUseUa50;
    property PRC: double read _pr_c;
    property PRT: double read _pr_t;
    property UseForCalc: boolean read _att._use_for_calc write SetUseForCalc;
    property UseForDuel: boolean read _att._use_for_duel write SetUseForDuel;
    property Distance: double read _att._distance write SetDistance;
    property Status: integer read _att._status;
  end;

//function GetEmin(f: double): double;
function FreqToBand(f: double): byte;
function IntToStd(std: TTvStandards): string;
function IntToSys(sys: TTvSystems): string;
function CorRecAngle(teta, f: double): double;
function GetVCarrier(ch: integer): double;
function GetSCarrier(ch: integer): double;
function TxTypeToString(t: TTxType): string;
function StrToTxType(s: string): TTxType;
procedure TxShowMessage(s: string);
procedure CodeToNum(code: TCodeString; var region, num: integer);
function FreqToOffset(f: double): integer;
function OffsetToFreq(off: integer): double;
function TVAStrToInt(sval: string): integer;
function TVAStrToFloat(sval: string): double;


implementation

function FreqToBand(f: double): byte;
begin
  result := 1;
//  if ( f >= 41 ) and ( f <= 68) then result := 1;
  if ( f >= 76 ) and ( f <= 100) then result := 2;
  if ( f >= 162 ) and ( f <= 230) then result := 3;
  if ( f >= 470 ) and ( f <= 582) then result := 4;
  if ( f > 582 ) and ( f <= 960) then result := 5;
end;

function IntToStd(std: TTvStandards): string;
begin
  case std of
    csPAL: result := 'PAL';
    csSECAM: result := 'SECAM';
    csNTSC: result := 'NTSC'
  else
    result := 'Unknown TV standard';
  end;
end;


function IntToSys(sys: TTvSystems): string;
begin
  case sys of
    tvB: result := 'B';
    tvG: result := 'G';
    tvD: result := 'D';
    tvK: result := 'K';
    tvK1: result := 'K1';
    tvD1: result := 'D1';
    tvH: result := 'H';
    tvL: result := 'L';
    tvI: result := 'I';
  else
    result := 'Unknown TV system';
  end;
end;

function TTx.GetEByDistance(azimuth, d: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
var plon, plat: double;
begin
  plon := _att._lon;
  plat := _att._lat;
  GetNextCoord(azimuth, d, plon, plat);
  result := GetE (plon, plat, param)
end;

procedure TTx.SetCode(val: TCodeString);
begin
  if _att._code <> val then _att._code := val;
end;

procedure TTx.SetSortKey(val: double);
begin
  if _att._sort_key <> val then _att._sort_key := val;
end;



procedure TTx.SetSortKey2(val: double);
begin
  if _att._sort_key2 <> val then _att._sort_key2 := val;
end;



procedure TTx.SetSortKey3(val: double);
begin
  if _att._sort_key3 <> val then _att._sort_key3 := val;
end;



function TTx.GetE(plon, plat: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
begin
  _profile.UseUA50 := param.use_ua50;
  case param.model of
    MODEL_P370: result := GetE_370 (plon, plat, param);
    MODEL_P1546: result := GetE_1546 (plon, plat, param);
    MODEL_FREESPACE: result := GetE_FreeSpace (plon, plat, param);
  else
    TxShowMessage('TTx.GetE: Unknown propagation model !');
    result := 0;
  end;
//  _att._sort_key := result;
end;


constructor TTx.Create(tx_id: integer; query: TQuery);
begin
  inherited Create;
  if (query <> nil) and (tx_id > 0) then LoadTx(tx_id, query) else SetDefaults;
  _R370 := CoR370.Create;
//  if InitCrvs <> 0 then ShowMessage('Ошибка при открытии файла 1546.crv');
//  _ua50 := TUA50.Create;
  _profile := TProfile.Create;
end;



procedure TTx.SetDistance(val: double);
begin
  if _att._distance <> val then _att._distance := val;
end;



procedure TTx.SaveTx(tx_id: integer; query: TQuery);
var s: string;
    new_id: integer;
    i: integer;
begin
  if tx_id <= 0 then  // новый передатчик - в базу
  begin
    query.Close;
    query.SQL.Clear;

    s := 'INSERT INTO tx (tx_id, tx_type, tx_sys, tx_snd,';
    s := s + ' tx_std, tx_f, v_carrier, v_offset, s_carrier,';
    s := s + ' s_offset, mono_stereo, offset_type, signal_type,';
    s := s + ' tx_ch, tx_name, v_power, s_power, v_erp_max,';
    s := s + ' s_erp_max, polar, tx_lon, tx_lat, tx_height,';
    s := s + ' fider_length, fider_loss, status) VALUES (';
    s := s + '0, ';
    s := s +'"'+ TxTypeToString(TxType)+ '", ';
    s := s + IntToStr(Integer(_att._sys))+', ';
    s := s + IntToStr(Integer(_att._snd))+', ';
    s := s + IntToStr(Integer(_att._std))+', ';
    s := s + FloatToStr(_att._f)+', ';
    s := s + FloatToStr(VCarrier)+', ';
    s := s + FloatToStr(VOffset)+', ';
    s := s + FloatToStr(SCarrier)+', ';
    s := s + FloatToStr(SOffset)+', ';
    s := s + '"' + _att._mono_stereo + '", ';
    s := s + IntToStr(Integer(_att._offset_type))+', ';
    s := s + IntToStr(_att._signal)+', ';
    s := s + IntToStr(_att._ch)+', ';
    s := s + '"' + _att._name + '", ';
    s := s + FloatToStr(_att._v_power)+', ';
    s := s + FloatToStr(_att._s_power)+', ';
    s := s + FloatToStr(_att._v_erp_max)+', ';
    s := s + FloatToStr(_att._s_erp_max)+', ';
    s := s + IntToStr(Integer(_att._polar))+', ';
    s := s + FloatToStr(_att._lon)+', ';
    s := s + FloatToStr(_att._lat)+', ';
    s := s + IntToStr(_att._h)+', ';
    s := s + FloatToStr(_att._fider_length)+', ';
    s := s + FloatToStr(_att._fider_loss)+', ';
    s := s + '-1';  // новый передатчик (для выбора ID)
    s := s + ')';
    query.SQL.Add(s);
    query.ExecSQL;

    query.Close;
    query.SQL.Clear;
    s := 'UPDATE tx';
    s := s + ' SET v_erp_max=' + FloatToStr(_att._v_erp_max) + ', ';
    s := s + 'v_erp_hor=' + FloatToStr(_att._v_erp_hor) + ', ';
    s := s + 'v_erp_ver=' + FloatToStr(_att._v_erp_ver) + ', ';
    s := s + 's_erp_max=' + FloatToStr(_att._s_erp_max) + ', ';
    s := s + 's_erp_ver=' + FloatToStr(_att._s_erp_ver) + ', ';
    s := s + 's_erp_hor=' + FloatToStr(_att._s_erp_hor) + ', ';
    s := s + 'h_eff_max=' + IntToStr(_att._h_max) + ', ';
    s := s + 'gain=' + FloatToStr(_att._gain) + ', ';
    s := s + 'code="' + _att._code + '"';
    s := s + ' WHERE tx_id=' + IntToStr(tx_id);
    query.SQL.Add(s);
    query.ExecSQL;
{
    _s_erp_max := query.FieldByname('s_erp_max').AsFloat;
    _s_erp_ver := query.FieldByname('s_erp_ver').AsFloat;
    _s_erp_hor := query.FieldByname('s_erp_hor').AsFloat;
    _v_erp_max := query.FieldByname('v_erp_max').AsFloat;
    _v_erp_ver := query.FieldByname('v_erp_ver').AsFloat;
    _v_erp_hor := query.FieldByname('v_erp_hor').AsFloat;

}

    s := 'SELECT tx_id FROM tx WHERE status = -1';
    query.Close;
    query.SQL.Clear;
    query.SQL.Add(s);
    query.Open;

    if query.RecordCount = 1 then
    begin
      new_id := query.FieldByName('tx_id').AsInteger;
      _att._id := new_id;
      for i := 0 to 35 do
      begin
        AddERP(new_id, i*10, _att._erp_az_h[i], _att._erp_az_v[i], query);
        AddGain(new_id, i*10, _att._gain_az[i], query);
        AddHef(new_id, i*10, _att._hef_az[i], query);
      end;
    end else
    begin
      TxShowMessage('Error while inserting new Tx');
    end;

    s := 'UPDATE tx SET status = 1 WHERE status = -1';
    query.Close;
    query.SQL.Clear;
    query.SQL.Add(s);
    query.ExecSQL;

  end else
  begin               // изменяем существующий передатчик
    s := 'UPDATE tx SET';
    s := s +' tx_type='+'"'+ TxTypeToString(TxType)+ '", ';
    s := s +' tx_sys='+ IntToStr(Integer(_att._sys))+', ';
    s := s +' tx_snd='+ IntToStr(Integer(_att._snd))+', ';
    s := s +' tx_std='+ IntToStr(Integer(_att._std))+', ';
    s := s +' tx_f='+ FloatToStr(_att._f)+', ';
    s := s +' v_carrier='+ FloatToStr(VCarrier)+', ';
    s := s +' v_offset='+ FloatToStr(VOffset)+', ';
    s := s +' s_carrier='+  FloatToStr(SCarrier)+', ';
    s := s +' s_offset='+  FloatToStr(SOffset)+', ';
    s := s +' mono_stereo='+  '"' + _att._mono_stereo + '", ';
    s := s +' offset_type='+  IntToStr(Integer(_att._offset_type))+', ';
    s := s +' signal_type='+  IntToStr(_att._signal)+', ';
    s := s +' tx_ch='+ IntToStr(_att._ch)+', ';
    s := s +' tx_name='+  '"' + _att._name + '", ';
    s := s +' v_power='+ FloatToStr(_att._v_power)+', ';
    s := s +' s_power='+ FloatToStr(_att._s_power)+', ';
    s := s +' v_erp_max='+ FloatToStr(_att._v_erp_max)+', ';
    s := s +' s_erp_max='+ FloatToStr(_att._s_erp_max)+', ';
    s := s +' tx_lon='+ FloatToStr(_att._lon)+', ';
    s := s +' tx_lat='+ FloatToStr(_att._lat)+', ';
    s := s +' fider_length='+ FloatToStr(_att._fider_length)+', ';
    s := s +' fider_loss='+ FloatToStr(_att._fider_loss)+', ';
    s := s +' polar='+ IntToStr(Integer(_att._polar))+', ';
    s := s +' tx_height='+ IntToStr(_att._h);
    s := s + ' WHERE tx_id=' + IntToStr(tx_id);
    query.Close;
    query.SQL.Clear;
    query.SQL.Add(s);
    query.ExecSQL;

    new_id := tx_id;
    _att._id := new_id;
    for i := 0 to 35 do
    begin
      AddErp(new_id, i*10, _att._erp_az_h[i], _att._erp_az_v[i], query);
      AddGain(new_id, i*10, _att._gain_az[i], query);
      AddHef(new_id, i*10, _att._hef_az[i], query);
//      UpdateERPTab(new_id, i*10, _erp_az_h[i], query);
//      UpdateGainTab(new_id, i*10, _gain_az[i], query);
//      UpdateHefTab(new_id, i*10, _hef_az[i], query);
    end;
  end;
end;
                             // az - azimuth in degrees
procedure TTx.AddErp (tx_id, az: integer; val_h, val_v: double; query: TQuery);
var s: string;
begin
  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT tx_id FROM erp360 WHERE tx_id=' + IntToStr(tx_id));
  query.Open;

  if query.RecordCount = 0 then
  begin
    query.Close;
    query.SQL.Clear;
    query.SQL.Add('SELECT tx_id FROM tx WHERE tx_id=' + IntToStr(tx_id));
    query.Open;

    if query.RecordCount = 0 then
    begin
      TxShowMessage('Tx with ID = '+IntToStr(tx_id)+' not found in database');
      Exit;
    end;

    query.Close;
    query.SQL.Clear;
    query.SQL.Add('INSERT INTO erp360 (tx_id) VALUES (' + IntToStr(tx_id)+')');
    query.ExecSQL;
  end;

  az := Round(az/10) * 10;
  s := IntToStr(az);

  while Length(s) < 3 do s := '0' + s;

  query.Close;
  query.SQL.Clear;
  s := 'UPDATE erp360 SET erp_h' + s + '=' + FloatToStr(val_h) + ', erp_v' + s + '=' + FloatToStr(val_v) + 'WHERE tx_id=' + IntToStr(tx_id);
  query.SQL.Add(s);
  query.ExecSQL;
end;

procedure TTx.AddHef (tx_id, az: integer; val: double; query: TQuery);
var s: string;
begin
  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT tx_id FROM heights360 WHERE tx_id=' + IntToStr(tx_id));
  query.Open;

  if query.RecordCount = 0 then
  begin
    query.Close;
    query.SQL.Clear;
    query.SQL.Add('SELECT tx_id FROM tx WHERE tx_id=' + IntToStr(tx_id));
    query.Open;

    if query.RecordCount = 0 then
    begin
      TxShowMessage('Tx with ID = '+IntToStr(tx_id)+' not found in database');
      Exit;
    end;

    query.Close;
    query.SQL.Clear;
    query.SQL.Add('INSERT INTO heights360 (tx_id) VALUES (' + IntToStr(tx_id)+')');
    query.ExecSQL;
  end;

  az := Round(az/10) * 10;
  s := IntToStr(az);

  while Length(s) < 3 do s := '0' + s;

  query.Close;
  query.SQL.Clear;
  s := 'UPDATE heights360 SET h' + s + '=' + FloatToStr(val) + 'WHERE tx_id=' + IntToStr(tx_id);
  query.SQL.Add(s);
  query.ExecSQL;
end;

procedure TTx.AddGain (tx_id, az: integer; val: double; query: TQuery);
var s: string;
begin
  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT tx_id FROM gain360 WHERE tx_id=' + IntToStr(tx_id));
  query.Open;

  if query.RecordCount = 0 then
  begin
    query.Close;
    query.SQL.Clear;
    query.SQL.Add('SELECT tx_id FROM tx WHERE tx_id=' + IntToStr(tx_id));
    query.Open;

    if query.RecordCount = 0 then
    begin
      TxShowMessage('Tx with ID = '+IntToStr(tx_id)+' not found in database');
      Exit;
    end;

    query.Close;
    query.SQL.Clear;
    query.SQL.Add('INSERT INTO gain360 (tx_id) VALUES (' + IntToStr(tx_id)+')');
    query.ExecSQL;
  end;

  az := Round(az/10) * 10;
  s := IntToStr(az);

  while Length(s) < 3 do s := '0' + s;

  query.Close;
  query.SQL.Clear;
  s := 'UPDATE gain360 SET gain' + s + '=' + FloatToStr(val) + 'WHERE tx_id=' + IntToStr(tx_id);
  query.SQL.Add(s);
  query.ExecSQL;
end;

procedure TTx.LoadTx (tx_id: integer; query: TQuery);
var s: string;
//    i: integer;
begin
  SetDefaults;

  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT * FROM tx WHERE tx_id=' + IntToStr(tx_id));
  query.Open;

  if query.RecordCount>0 then
  begin
    _att._polar := pHOR;
    _att._signal := 0; // for analog transmitters
    _att._id := query.FieldByName('tx_id').AsInteger;
//    _f := query.FieldByName('tx_f').AsFloat;
    _att._sys := TTvSystems(query.FieldByName('tx_sys').AsInteger);
    _att._std := TTvStandards(query.FieldByName('tx_std').AsInteger);
    _att._snd := TSoundCarrier(query.FieldByName('tx_snd').AsInteger);
    _att._offset_type := TOffsetType(query.FieldByname('offset_type').AsInteger);
    _att._ch := query.FieldByname('tx_ch').AsInteger;
    VCarrier := query.FieldByname('v_carrier').AsFloat;
    _att._v_offset := query.FieldByname('v_offset').AsFloat;
    SCarrier := query.FieldByname('s_carrier').AsFloat;
    _att._s_offset := query.FieldByname('s_offset').AsFloat;
    _att._polar := TPolar(query.FieldByname('polar').AsInteger);
    _att._lon := query.FieldByname('tx_lon').AsFloat;
    _att._lat := query.FieldByname('tx_lat').AsFloat;
    _att._h := query.FieldByname('tx_height').AsInteger;
    _att._fider_length := query.FieldByname('fider_length').AsFloat;
    _att._fider_loss := query.FieldByname('fider_loss').AsFloat;
    s := query.FieldByname('mono_stereo').AsString;
    if s<>'' then _att._mono_stereo := s[1] else _att._mono_stereo := '?';
    TxTypeStr := query.FieldByname('tx_type').AsString;

    if _att._type=ttTV then _att._f := query.FieldByname('v_carrier').AsFloat;
    if _att._type=ttFM then _att._f := query.FieldByname('s_carrier').AsFloat;

    _att._v_power := query.FieldByname('v_power').AsFloat;
    _att._s_power := query.FieldByname('s_power').AsFloat;

    _att._s_erp_max := query.FieldByname('s_erp_max').AsFloat;
    _att._s_erp_ver := query.FieldByname('s_erp_ver').AsFloat;
    _att._s_erp_hor := query.FieldByname('s_erp_hor').AsFloat;
    _att._v_erp_max := query.FieldByname('v_erp_max').AsFloat;
    _att._v_erp_ver := query.FieldByname('v_erp_ver').AsFloat;
    _att._v_erp_hor := query.FieldByname('v_erp_hor').AsFloat;
    _att._gain := query.FieldByname('gain').AsFloat;
    _att._direction := query.FieldByname('direction').AsString[1];


//    _code := query.FieldByname('code').AsString;

    _att._code := IntToStr(query.FieldByname('region_id').AsInteger);

    s := IntToStr(query.FieldByname('num').AsInteger);
    while Length(s) < 4 do s := '0' + s;

    _att._code := _att._code + s;

    _att._h_max := query.FieldByname('h_eff_max').AsInteger;

    if _att._type=ttTV then _att._erp := query.FieldByname('v_erp_max').AsFloat;
    if _att._type=ttFM then _att._erp := query.FieldByname('s_erp_max').AsFloat;
    if ((_att._type = ttFM) or (_att._type = ttAM)) and (_att._mono_stereo = 'M') then _att._emin := 37
    else _att._emin := GetEmin;
    _att._name := query.FieldByname('tx_name').AsString;
    _att._status := query.FieldByname('status').AsInteger;
    _pr_c := 0;
    _pr_t := 0;
  end;

  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT * FROM gain WHERE tx_id=' + IntToStr(tx_id));
  query.Open;
  query.First;
  if query.RecordCount>0 then
  begin
    _att._gain_az_empty := false;
    _att._gain_az[0] := query.FieldByname('gain000').AsFloat;
    _att._gain_az[1] := query.FieldByname('gain010').AsFloat;
    _att._gain_az[2] := query.FieldByname('gain020').AsFloat;
    _att._gain_az[3] := query.FieldByname('gain030').AsFloat;
    _att._gain_az[4] := query.FieldByname('gain040').AsFloat;
    _att._gain_az[5] := query.FieldByname('gain050').AsFloat;
    _att._gain_az[6] := query.FieldByname('gain060').AsFloat;
    _att._gain_az[7] := query.FieldByname('gain070').AsFloat;
    _att._gain_az[8] := query.FieldByname('gain080').AsFloat;
    _att._gain_az[9] := query.FieldByname('gain090').AsFloat;
    _att._gain_az[10] := query.FieldByname('gain100').AsFloat;
    _att._gain_az[11] := query.FieldByname('gain110').AsFloat;
    _att._gain_az[12] := query.FieldByname('gain120').AsFloat;
    _att._gain_az[13] := query.FieldByname('gain130').AsFloat;
    _att._gain_az[14] := query.FieldByname('gain140').AsFloat;
    _att._gain_az[15] := query.FieldByname('gain150').AsFloat;
    _att._gain_az[16] := query.FieldByname('gain160').AsFloat;
    _att._gain_az[17] := query.FieldByname('gain170').AsFloat;
    _att._gain_az[18] := query.FieldByname('gain180').AsFloat;
    _att._gain_az[19] := query.FieldByname('gain190').AsFloat;
    _att._gain_az[20] := query.FieldByname('gain200').AsFloat;
    _att._gain_az[21] := query.FieldByname('gain210').AsFloat;
    _att._gain_az[22] := query.FieldByname('gain220').AsFloat;
    _att._gain_az[23] := query.FieldByname('gain230').AsFloat;
    _att._gain_az[24] := query.FieldByname('gain240').AsFloat;
    _att._gain_az[25] := query.FieldByname('gain250').AsFloat;
    _att._gain_az[26] := query.FieldByname('gain260').AsFloat;
    _att._gain_az[27] := query.FieldByname('gain270').AsFloat;
    _att._gain_az[28] := query.FieldByname('gain280').AsFloat;
    _att._gain_az[29] := query.FieldByname('gain290').AsFloat;
    _att._gain_az[30] := query.FieldByname('gain300').AsFloat;
    _att._gain_az[31] := query.FieldByname('gain310').AsFloat;
    _att._gain_az[32] := query.FieldByname('gain320').AsFloat;
    _att._gain_az[33] := query.FieldByname('gain330').AsFloat;
    _att._gain_az[34] := query.FieldByname('gain340').AsFloat;
    _att._gain_az[35] := query.FieldByname('gain350').AsFloat;
    _att._gain_az[36] := query.FieldByname('gain360').AsFloat;
  end  else _att._gain_az_empty := true;

  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT * FROM erp360 WHERE tx_id=' + IntToStr(tx_id));
  query.Open;
  query.First;
  if query.RecordCount>0 then
  begin
    _att._erp_az_empty := false;
    _att._erp_az_h[0] := query.FieldByname('erp_h000').AsFloat;
    _att._erp_az_h[1] := query.FieldByname('erp_h010').AsFloat;
    _att._erp_az_h[2] := query.FieldByname('erp_h020').AsFloat;
    _att._erp_az_h[3] := query.FieldByname('erp_h030').AsFloat;
    _att._erp_az_h[4] := query.FieldByname('erp_h040').AsFloat;
    _att._erp_az_h[5] := query.FieldByname('erp_h050').AsFloat;
    _att._erp_az_h[6] := query.FieldByname('erp_h060').AsFloat;
    _att._erp_az_h[7] := query.FieldByname('erp_h070').AsFloat;
    _att._erp_az_h[8] := query.FieldByname('erp_h080').AsFloat;
    _att._erp_az_h[9] := query.FieldByname('erp_h090').AsFloat;
    _att._erp_az_h[10] := query.FieldByname('erp_h100').AsFloat;
    _att._erp_az_h[11] := query.FieldByname('erp_h110').AsFloat;
    _att._erp_az_h[12] := query.FieldByname('erp_h120').AsFloat;
    _att._erp_az_h[13] := query.FieldByname('erp_h130').AsFloat;
    _att._erp_az_h[14] := query.FieldByname('erp_h140').AsFloat;
    _att._erp_az_h[15] := query.FieldByname('erp_h150').AsFloat;
    _att._erp_az_h[16] := query.FieldByname('erp_h160').AsFloat;
    _att._erp_az_h[17] := query.FieldByname('erp_h170').AsFloat;
    _att._erp_az_h[18] := query.FieldByname('erp_h180').AsFloat;
    _att._erp_az_h[19] := query.FieldByname('erp_h190').AsFloat;
    _att._erp_az_h[20] := query.FieldByname('erp_h200').AsFloat;
    _att._erp_az_h[21] := query.FieldByname('erp_h210').AsFloat;
    _att._erp_az_h[22] := query.FieldByname('erp_h220').AsFloat;
    _att._erp_az_h[23] := query.FieldByname('erp_h230').AsFloat;
    _att._erp_az_h[24] := query.FieldByname('erp_h240').AsFloat;
    _att._erp_az_h[25] := query.FieldByname('erp_h250').AsFloat;
    _att._erp_az_h[26] := query.FieldByname('erp_h260').AsFloat;
    _att._erp_az_h[27] := query.FieldByname('erp_h270').AsFloat;
    _att._erp_az_h[28] := query.FieldByname('erp_h280').AsFloat;
    _att._erp_az_h[29] := query.FieldByname('erp_h290').AsFloat;
    _att._erp_az_h[30] := query.FieldByname('erp_h300').AsFloat;
    _att._erp_az_h[31] := query.FieldByname('erp_h310').AsFloat;
    _att._erp_az_h[32] := query.FieldByname('erp_h320').AsFloat;
    _att._erp_az_h[33] := query.FieldByname('erp_h330').AsFloat;
    _att._erp_az_h[34] := query.FieldByname('erp_h340').AsFloat;
    _att._erp_az_h[35] := query.FieldByname('erp_h350').AsFloat;
    _att._erp_az_h[36] := query.FieldByname('erp_h360').AsFloat;

    _att._erp_az_v[0] := query.FieldByname('erp_v000').AsFloat;
    _att._erp_az_v[1] := query.FieldByname('erp_v010').AsFloat;
    _att._erp_az_v[2] := query.FieldByname('erp_v020').AsFloat;
    _att._erp_az_v[3] := query.FieldByname('erp_v030').AsFloat;
    _att._erp_az_v[4] := query.FieldByname('erp_v040').AsFloat;
    _att._erp_az_v[5] := query.FieldByname('erp_v050').AsFloat;
    _att._erp_az_v[6] := query.FieldByname('erp_v060').AsFloat;
    _att._erp_az_v[7] := query.FieldByname('erp_v070').AsFloat;
    _att._erp_az_v[8] := query.FieldByname('erp_v080').AsFloat;
    _att._erp_az_v[9] := query.FieldByname('erp_v090').AsFloat;
    _att._erp_az_v[10] := query.FieldByname('erp_v100').AsFloat;
    _att._erp_az_v[11] := query.FieldByname('erp_v110').AsFloat;
    _att._erp_az_v[12] := query.FieldByname('erp_v120').AsFloat;
    _att._erp_az_v[13] := query.FieldByname('erp_v130').AsFloat;
    _att._erp_az_v[14] := query.FieldByname('erp_v140').AsFloat;
    _att._erp_az_v[15] := query.FieldByname('erp_v150').AsFloat;
    _att._erp_az_v[16] := query.FieldByname('erp_v160').AsFloat;
    _att._erp_az_v[17] := query.FieldByname('erp_v170').AsFloat;
    _att._erp_az_v[18] := query.FieldByname('erp_v180').AsFloat;
    _att._erp_az_v[19] := query.FieldByname('erp_v190').AsFloat;
    _att._erp_az_v[20] := query.FieldByname('erp_v200').AsFloat;
    _att._erp_az_v[21] := query.FieldByname('erp_v210').AsFloat;
    _att._erp_az_v[22] := query.FieldByname('erp_v220').AsFloat;
    _att._erp_az_v[23] := query.FieldByname('erp_v230').AsFloat;
    _att._erp_az_v[24] := query.FieldByname('erp_v240').AsFloat;
    _att._erp_az_v[25] := query.FieldByname('erp_v250').AsFloat;
    _att._erp_az_v[26] := query.FieldByname('erp_v260').AsFloat;
    _att._erp_az_v[27] := query.FieldByname('erp_v270').AsFloat;
    _att._erp_az_v[28] := query.FieldByname('erp_v280').AsFloat;
    _att._erp_az_v[29] := query.FieldByname('erp_v290').AsFloat;
    _att._erp_az_v[30] := query.FieldByname('erp_v300').AsFloat;
    _att._erp_az_v[31] := query.FieldByname('erp_v310').AsFloat;
    _att._erp_az_v[32] := query.FieldByname('erp_v320').AsFloat;
    _att._erp_az_v[33] := query.FieldByname('erp_v330').AsFloat;
    _att._erp_az_v[34] := query.FieldByname('erp_v340').AsFloat;
    _att._erp_az_v[35] := query.FieldByname('erp_v350').AsFloat;
    _att._erp_az_v[36] := query.FieldByname('erp_v360').AsFloat;

  end  else _att._erp_az_empty := true;

  query.Close;
  query.SQL.Clear;
  query.SQL.Add('SELECT * FROM heights360 WHERE tx_id=' + IntToStr(tx_id));
  query.Open;
  query.First;
  if query.RecordCount>0 then
  begin
    _att._hef_az_empty := false;
    _att._hef_az[0] := query.FieldByname('h000').AsFloat;
    _att._hef_az[1] := query.FieldByname('h010').AsFloat;
    _att._hef_az[2] := query.FieldByname('h020').AsFloat;
    _att._hef_az[3] := query.FieldByname('h030').AsFloat;
    _att._hef_az[4] := query.FieldByname('h040').AsFloat;
    _att._hef_az[5] := query.FieldByname('h050').AsFloat;
    _att._hef_az[6] := query.FieldByname('h060').AsFloat;
    _att._hef_az[7] := query.FieldByname('h070').AsFloat;
    _att._hef_az[8] := query.FieldByname('h080').AsFloat;
    _att._hef_az[9] := query.FieldByname('h090').AsFloat;
    _att._hef_az[10] := query.FieldByname('h100').AsFloat;
    _att._hef_az[11] := query.FieldByname('h110').AsFloat;
    _att._hef_az[12] := query.FieldByname('h120').AsFloat;
    _att._hef_az[13] := query.FieldByname('h130').AsFloat;
    _att._hef_az[14] := query.FieldByname('h140').AsFloat;
    _att._hef_az[15] := query.FieldByname('h150').AsFloat;
    _att._hef_az[16] := query.FieldByname('h160').AsFloat;
    _att._hef_az[17] := query.FieldByname('h170').AsFloat;
    _att._hef_az[18] := query.FieldByname('h180').AsFloat;
    _att._hef_az[19] := query.FieldByname('h190').AsFloat;
    _att._hef_az[20] := query.FieldByname('h200').AsFloat;
    _att._hef_az[21] := query.FieldByname('h210').AsFloat;
    _att._hef_az[22] := query.FieldByname('h220').AsFloat;
    _att._hef_az[23] := query.FieldByname('h230').AsFloat;
    _att._hef_az[24] := query.FieldByname('h240').AsFloat;
    _att._hef_az[25] := query.FieldByname('h250').AsFloat;
    _att._hef_az[26] := query.FieldByname('h260').AsFloat;
    _att._hef_az[27] := query.FieldByname('h270').AsFloat;
    _att._hef_az[28] := query.FieldByname('h280').AsFloat;
    _att._hef_az[29] := query.FieldByname('h290').AsFloat;
    _att._hef_az[30] := query.FieldByname('h300').AsFloat;
    _att._hef_az[31] := query.FieldByname('h310').AsFloat;
    _att._hef_az[32] := query.FieldByname('h320').AsFloat;
    _att._hef_az[33] := query.FieldByname('h330').AsFloat;
    _att._hef_az[34] := query.FieldByname('h340').AsFloat;
    _att._hef_az[35] := query.FieldByname('h350').AsFloat;
    _att._hef_az[36] := query.FieldByname('h360').AsFloat;
end  else _att._hef_az_empty := true;

end;

function GetSCarrier(ch: integer): double;
begin
  result := -1;
  if ch = 1 then result := 56.25;
  if ch = 2 then result := 65.75;
  if (ch >= 3) and (ch <= 5) then result := 83.75 + (ch - 3) * 8;
  if (ch >= 6) and (ch <= 12) then result := 181.75 + (ch - 6) * 8;
  if (ch >= 21) and (ch <= 69) then result := 477.75 + (ch - 21) * 8;

end;

function GetVCarrier(ch: integer): double;
begin
  case ch of
    1: result := 49.75;
    2: result := 59.25;
    3: result := 77.25;
    4: result := 85.25;
    5: result := 93.25;
    6: result := 175.25;
    7: result := 183.25;
    8: result := 191.25;
    9: result := 199.25;
    10: result := 207.25;
    11: result := 215.25;
    12: result := 223.25;
    21: result := 471.25;
    22: result := 479.25;
    23: result := 487.25;
    24: result := 495.25;
    25: result := 503.25;
    26: result := 511.25;
    27: result := 519.25;
    28: result := 527.25;
    29: result := 535.25;
    30: result := 543.25;
    31: result := 551.25;
    32: result := 559.25;
    33: result := 567.25;
    34: result := 575.25;
    35: result := 583.25;
    36: result := 591.25;
    37: result := 599.25;
    38: result := 607.25;
    39: result := 615.25;
    40: result := 623.25;
    41: result := 631.25;
    42: result := 639.25;
    43: result := 647.25;
    44: result := 655.25;
    45: result := 663.25;
    46: result := 671.25;
    47: result := 679.25;
    48: result := 687.25;
    49: result := 695.25;
    50: result := 703.25;
    51: result := 711.25;
    52: result := 719.25;
    53: result := 727.25;
    54: result := 735.25;
    55: result := 743.25;
    56: result := 751.25;
    57: result := 759.25;
    58: result := 767.25;
    59: result := 775.25;
    60: result := 783.25;
    61: result := 791.25;
    62: result := 799.25;
    63: result := 807.25;
    64: result := 815.25;
    65: result := 823.25;
    66: result := 831.25;
    67: result := 839.25;
    68: result := 847.25;
    69: result := 855.25;
  else result := -1;
  end;
end;



function TTx.GetEmin: double;
begin
{
  Эту функцию надо модифицировать с учетом типа передатчика.
  Эти значения Емин. верны для АТВ, но не для ОВЧ ЧМ или ЦТВ...
}
  result := 48;
  if ( _att._f >= 41 ) and ( _att._f <= 68) then result := 48;
  if ( _att._f >= 76 ) and ( _att._f <= 100) then result := 52;
  if ( _att._f >= 162 ) and ( _att._f <= 230) then result := 55;

  if ( _att._f >= 470 ) and ( _att._f <= 582) then
  begin
    result := 65;
    if _att._sys = tvK then result := result + 2;
  end;
  if ( _att._f >= 582 ) and ( _att._f <= 790) then
  begin
    result := 70;
    if _att._sys = tvK then result := result + 2;
  end;
//  if ( f >= 470 ) and ( f <= 960) then result := 62 + 20 * Log10( f / 474 );
////////////////////////////////////////////
//  result := result - 5;
end;



function TTx.GetErp(azimuth: double): double;
var az: integer;
begin
//  result := _erp_az[azimuth];
  if _att._direction = 'N' then
  begin
    case _att._type of
      ttTV: result := _att._v_erp_max; // + _gain;
      ttFM: result := _att._s_erp_max; // + _gain;
      ttAM: result := _att._s_erp_max; // + _gain;
    else
      result := _att._s_erp_max;
    end;
    Exit;
  end;
  if _att._erp_az_empty then result := _att._erp else
  begin
    azimuth := RadToDeg(azimuth);
    az := round(azimuth / 10);
    if az >= 0 then  while az > 36 do az := az - 36;
    if az < 0 then  while az <= 0  do az := az + 36;
    case _att._polar of
      pVER: result := _att._erp_az_v[az];
      pHOR: result := _att._erp_az_h[az];
    else
      result := _att._erp_az_h[az];
    end;
//    if pVER then
  end;
end;



function TTx.GetGain(azimuth: double): double;
var az: integer;
begin
//  result := _erp_az[azimuth];
  azimuth := RadToDeg(azimuth);
  az := round(azimuth / 10);

  if az >= 0 then  while az > 36 do az := az - 36;
  if az < 0 then  while az <= 0  do az := az + 36;

  if _att._gain_az_empty then result := 0 else result := _att._gain_az[az];
end;



{
  azimuth - это десятки градусов, начиная с севера
  0 = 0 град.
  1 = 10 град.
  2 = 20 град. и т.д.

  0 - на севере.
}
function TTx.GetHef(azimuth: integer): double;
begin
  if _att._hef_az_empty then result := _att._h else
  begin
    while azimuth < 0 do azimuth := azimuth + 36;
    while azimuth >= 36 do azimuth := azimuth - 36;
{
  в массиве нулевой индекс это Hэф. макс., поэтому увеличи-
  ваем azimuth на 1 
}
    azimuth := azimuth + 1;
    result := _att._hef_az[azimuth];
  end;
end;



function TTx.GetE_370(plon, plat: double; param: TCalcParams): double; //  расчет напряж. поля создаваемого передатчиков в точке
var isUHF: integer;
    effective_height, e: double;
    i: integer;
    morpho: integer;
    val50: double;
    ReliefData: TProcessReliefResult;
    p: pointer;
    points_count: longword;
    relief_step: double;
    power, ca: double;
begin
  relief_step := STEP;
  points_count := 0;
  p := nil;
  morpho := 0;
  power := GetERP(GetAzimuth(_att._lon, _att._lat, plon, plat));
  if _att._f >= 450 then isUHF := 2 else isUHF := 1;
  i := Round( (RadToDeg(GetAzimuth(_att._lon, _att._lat, plon, plat) + PI)+180) / 10);
{
  если в базе была Нэф - берем ее ...
}
  effective_height := GetHef(i);
  if (param.use_rel or param.use_mor)then
  begin
    if not param.use_mor then morpho := 1;
    _profile.GetProfile(_att._lon, _att._lat, plon, plat, relief_step, p, points_count);
    try
      ReliefData := ProcessRelief (p, points_count, _att._h, RECEIVER_HEIGHT);
      if ((not param.use_hef) or _att._hef_az_empty)then effective_height := ReliefData.Hef;
      ca := _R370.ClearanceAngleCorrection(isUHF, ReliefData.Teta2);
    except
      TxShowMessage('Calc370: '+IntToStr(isUHF)+' '+IntToStr(param.pt)+' '+FloatToStr(_att._lon)+' '+FloatToStr(_att._lat)+' '+FloatToStr(plon)+' '+FloatToStr(plat)+' '+FloatToStr(effective_height)+' '+FloatToStr(ReliefData.Teta1));
      e := 0;
    end;
  end else ca := 0;
  e :=_R370.Propagation(isUHF, param.pt, _att._lon, _att._lat, plon, plat, effective_height, morpho, val50);
  result := e - ca + power;
  if p <> nil then FreeMem(p);
end;



function TTx.GetE_1546(plon, plat: double; param: TCalcParams): double;
var p: pointer;
    d: double;
    i: integer;
    points_count: longword;
    relief_step: double;
    calc_1546_result: TCalc1546Result;
    effective_height: double;
    power: double;
begin
  relief_step := STEP;
  points_count := 0;
  p := nil;
  if param.use_hef then
  begin
    i := Round( (RadToDeg(GetAzimuth(_att._lon, _att._lat, plon, plat) + PI)+180) / 10);
    d := GetDistance(_att._lon, _att._lat, plon, plat);
    effective_height := GetHef(i);
  end else
  begin
    effective_height := -10000;
    if param.use_mor
    then d := _profile.GetProfile(_att._lon, _att._lat, plon, plat, relief_step, p, points_count)
    else d := _profile.GetProfile(_att._lon, _att._lat, plon, plat, relief_step, p, points_count);
//    then d := GetProfile(param.gtopo_path, _lon, _lat, plon, plat, relief_step, p, points_count)
//    else d := GetProfileWithOutMorpho(param.gtopo_path, _lon, _lat, plon, plat, relief_step, p, points_count);
  end;
  try
    if d < 1 then d := 1;
    calc_1546_result := Calc1546(_att._f, d, _att._h, RECEIVER_HEIGHT, param.pt, param.pm, _att._signal, points_count, p, effective_height);
  except
    TxShowMessage('Calc1546: '+FloatToStr(_att._f)+' '+FloatToStr(d)+' '+FloatToStr(h)+' '+IntToStr(param.pt)+' '+IntToStr(param.pm)+' '+IntToStr(_att._signal)+' '+IntToStr(points_count)+' '+FloatToStr(effective_height));
  end;
//  result := calc_1546_result.E + _erp;
  power := GetERP(GetAzimuth(_att._lon, _att._lat, plon, plat));
  result := calc_1546_result.E + power;
  if p <> nil then FreeMem(p);
end;



///////////////////////////////////
function CorRecAngle(teta, f: double): double;
{
  Возвращает величину коррекции поля за счет угла закрытия
  приемной антенны.
    Входные данные:
     - угол закрытия (градусы)
     - частота (МГц.)
}
var v,v0,j1,j2: double;
begin
  if teta < -0.8  then teta := -0.8;
  if teta > 40    then teta := 40;
  v := 0.065 * teta * Sqrt(f);
  v0 := 0.036 * Sqrt(f);
  j1 := 6.9 + 20 * log10( Sqrt(Sqr(v0 - 0.1) + 1) + v0 - 0.1);
  j2 := 6.9 + 20 * log10( Sqrt(Sqr(v  - 0.1) + 1) + v  - 0.1);
  result := j1 - j2;
end;



procedure TTx.SetId(val: integer);
begin
  if _att._id <> val then _att._id := val;
end;



procedure TTx.SetF(val: double);
begin
   if _att._f <> val then
   begin
     _att._f := val;
     if _att._type = ttTV then _att._v_carrier := val;
     if _att._type = ttFM then _att._s_carrier := val;
     _att._emin := GetEMin;
   end
 end;



procedure TTx.SetSys(val: TTvSystems);    //
begin
  if _att._sys <> val then _att._sys := val;
end;



procedure TTx.SetStd(val: TTvStandards);   //
begin
  if _att._std <> val then _att._std := val;
end;



procedure TTx.SetSnd(val: TSoundCarrier);
begin
  if _att._snd <> val then _att._snd := val;
end;



procedure TTx.SetOffset_type(val: TOffsetType);
begin
  if _att._offset_type <> val then _att._offset_type := val;
end;



procedure TTx.SetCh(val: integer);
begin
  if _att._ch <> val then _att._ch := val;
end;



procedure TTx.SetV_carrier(val: double);        // MHz
begin
  if _att._v_carrier <> val then
  begin
    _att._v_carrier := val;
    if _att._type = ttTV then _att._f := val;
    _att._emin := GetEMin;
  end;
end;



 procedure TTx.SetV_offset(val: double);         // MHz
 begin
   if _att._v_offset <> val then _att._v_offset := val;
 end;



 procedure TTx.SetS_carrier(val: double);        // MHz
 begin
   if _att._s_carrier <> val then
   begin
     _att._s_carrier := val;
     if (_att._type = ttFM) or (_att._type = ttAM) then _att._f := val;
     _att._emin := GetEMin;
   end;
 end;



 procedure TTx.SetS_offset(val: double);         // MHz
 begin
   if _att._s_offset <> val then _att._s_offset := val;
 end;



 procedure TTx.SetV_power(val: double);                // Watt
 begin
   if _att._v_power <> val then
   begin
     _att._v_power := val;
//     _v_erp := 10 * Log10(val / 1000);
   end;
 end;



 procedure TTx.SetS_power(val: double);               // Watt
 begin
   if _att._s_power <> val then
   begin
     _att._s_power := val;
//     _s_erp := 10 * Log10(val / 1000);
   end;
 end;



 {
 procedure TTx.SetV_erp(val: double);                // dB kW
 begin
   if _v_erp <> val then
   begin
      _v_erp := val;
      if _tx_type = 'TV' then _erp := val;
   end;
 end;

 procedure TTx.SetS_erp(val: double);               // dB kW
 begin
   if _s_erp <> val then
   begin
     _s_erp := val;
     if _tx_type = 'FM' then _erp := val;
   end;
  end;
}



 procedure TTx.Set_S_erp_max(val: double);               // dB kW
 begin
   if _att._s_erp_max <> val then
   begin
     _att._s_erp_max := val;
   end;
  end;



 procedure TTx.Set_S_erp_hor(val: double);               // dB kW
 begin
   if _att._s_erp_hor <> val then
   begin
     _att._s_erp_hor := val;
   end;
  end;



 procedure TTx.Set_S_erp_ver(val: double);               // dB kW
 begin
   if _att._s_erp_ver <> val then
   begin
     _att._s_erp_ver := val;
   end;
  end;



 procedure TTx.Set_V_erp_max(val: double);               // dB kW
 begin
   if _att._v_erp_max <> val then
   begin
     _att._v_erp_max := val;
   end;
  end;



 procedure TTx.Set_V_erp_hor(val: double);               // dB kW
 begin
   if _att._v_erp_hor <> val then
   begin
     _att._v_erp_hor := val;
   end;
  end;



 procedure TTx.Set_V_erp_ver(val: double);               // dB kW
 begin
   if _att._v_erp_ver <> val then
   begin
     _att._v_erp_ver := val;
   end;
  end;



 procedure TTx.SetLon(val: double);
 begin
   if _att._lon <> val then _att._lon := val;
 end;



 procedure TTx.SetLat(val: double);
 begin
   if _att._lat <> val then _att._lat := val;
 end;



 procedure TTx.SetPolar(val: TPolar);
 begin
   if _att._polar <> val then _att._polar := val;
 end;



 procedure TTx.SetH(val: integer);               // Tx height (m)
 begin
   if _att._h <> val then _att._h := val;
 end;



 procedure TTx.SetMono_stereo(val: char);        // 'M' - mono, 'S' - stereo
 begin
   if _att._mono_stereo <> val then _att._mono_stereo := val;
 end;



 procedure TTx.SetTxTypeStr(val: TString2);        // 'TV', 'FM', 'AM'
 begin
   if _att._tx_type_str <> val then
   begin
     _att._tx_type_str := val;
     _att._type := StrToTxType(_att._tx_type_str);
   end;
 end;



 procedure TTx.SetTxType(val: TTxType);        // 'TV', 'FM', 'AM'
 begin
   if _att._type <> val then
   begin
     _att._type := val;
     _att._tx_type_str := TxTypeToString(val);
   end;
 end;



// procedure TTx.SetErp(val: double);              // Power (dB kW)
// begin
//   if _erp <> val then _erp := val;
// end;



 procedure TTx.Setname(val: TString32);          //
 begin
   if _att._name <> val then _att._name := val;
 end;



 procedure TTx.Setfider_loss(val: double);  // потери в фидере);
 begin
  if _att._fider_loss <> val then
  begin
    _att._fider_loss := val;
    UpdateErpArray;
  end;
 end;



 procedure TTx.Setfider_length(val: double);  // длина фидера);
 begin
  if _att._fider_length <> val then
  begin
    _att._fider_length := val;
    UpdateErpArray;
  end;
 end;



 procedure TTx.SetSignal(val: byte);
 begin
  if _att._signal <> val then _att._signal := val;
 end;



 {
procedure TTx.UpdateGainTab (tx_id, az: integer; val: double; query: TQuery);
var s: string;
begin
  query.Close;
  query.SQL.Clear;
  s := 'UPDATE gain SET gain='+ FloatToStr(val)+ ' WHERE tx_id=' + IntToStr(tx_id) + ' AND azimuth=' + IntToStr(az);
  query.SQL.Add(s);
  query.ExecSQL;
end;

procedure TTx.UpdateHefTab (tx_id, az: integer; val: double; query: TQuery);
var s: string;
begin
  query.Close;
  query.SQL.Clear;
  s := 'UPDATE heights SET h_eff='+ FloatToStr(val)+ ' WHERE tx_id=' + IntToStr(tx_id) + ' AND azimuth=' + IntToStr(az);
  query.SQL.Add(s);
  query.ExecSQL;
end;

procedure TTx.UpdateErpTab (tx_id, az: integer; val: double; query: TQuery);
var s: string;
begin
  query.Close;
  query.SQL.Clear;
  s := 'UPDATE erp SET erp='+ FloatToStr(val)+ ' WHERE tx_id=' + IntToStr(tx_id) + ' AND azimuth=' + IntToStr(az);
  query.SQL.Add(s);
  query.ExecSQL;
end;
}



procedure TTx.SetGain(az: integer; gain: double);      // az = (0..360) deg.
begin
  az := Trunc(az / 10);
  _att._gain_az[az] := gain;
  UpdateErpArray;
end;



procedure TTx.UpdateErpArray;
var i: integer;
begin
  for i := 0 to 35 do
  begin
    case _att._type of
      ttTV: begin
              _att._erp_az_h[i] := _att._v_erp_hor - _att._fider_length * _att._fider_loss + _att._gain_az[i];
              _att._erp_az_v[i] := _att._v_erp_ver - _att._fider_length * _att._fider_loss + _att._gain_az[i];
            end;
      ttAM: begin
              _att._erp_az_h[i] := _att._s_erp_hor - _att._fider_length * _att._fider_loss + _att._gain_az[i];
              _att._erp_az_v[i] := _att._s_erp_ver - _att._fider_length * _att._fider_loss + _att._gain_az[i];
            end;
      ttFM: begin
              _att._erp_az_h[i] := _att._s_erp_hor - _att._fider_length * _att._fider_loss + _att._gain_az[i];
              _att._erp_az_v[i] := _att._s_erp_ver - _att._fider_length * _att._fider_loss + _att._gain_az[i];
            end;
      ttDVB: begin
              _att._erp_az_h[i] := _att._v_erp_hor - _att._fider_length * _att._fider_loss + _att._gain_az[i];
              _att._erp_az_v[i] := _att._v_erp_ver - _att._fider_length * _att._fider_loss + _att._gain_az[i];
            end;
      ttDAB: begin
              _att._erp_az_h[i] := _att._s_erp_hor - _att._fider_length * _att._fider_loss + _att._gain_az[i];
              _att._erp_az_v[i] := _att._s_erp_ver - _att._fider_length * _att._fider_loss + _att._gain_az[i];
            end;
    end;
  end;
end;



procedure TTx.SetAntennaGain(val: double);
begin
  if _att._gain <> val then
  begin
    _att._gain := val;
  end;
end;



procedure TTx.SetDefaults;
var i: integer;
begin
      ID:=-1;
      HMax := 0;
      TxType:= ttTV;
//      f:
      sys:=tvK;
      std:=csSECAM;
      snd:=sndFM;
      offset_type:=otNP;
      ch:=1;
      VCarrier:=GetVCarrier(ch);
      VOffset:=0;
      SCarrier:=GetSCarrier(ch);
      SOffset:=0;
      VPower:=1000;
      SPower:=100;
      VErpMax := -99;
      SErpMax := -99;
      VErpVer := -99;
      SErpVer := -99;
      VErpHor := -99;
      SErpHor := -99;
      lon:=34;
      lat:=50;
      h:=100;
      MonoStereo:='M';
//      emin:=GetEmin(f);
//      tv_band:
//      band:
      name:='DEFAULT';
//      pr_c:
//      pr_t:
      FiderLoss:=0;
      FiderLength:=0;
      polar:=pHOR;
      signal:=0;

      for i := 0 to 35 do
      begin
        _att._gain_az[i] := 0;
        _att._gain_az[i] := 0;
        _att._erp_az_h[i] := 0;
        _att._erp_az_v[i] := 0;
        _att._hef_az[i] := 0;
      end;
end;



procedure TTx.SetDir(val: char);
begin
  if _att._direction <> val then _att._direction := val;
end;



function TxTypeToString(t: TTxType): string;
begin
  case t of
    ttTV: result := 'TV';
    ttAM: result := 'AM';
    ttFM: result := 'FM';
    ttDVB: result := 'DV';
    ttDAB: result := 'DA';
  else
    result := '??';
  end;
end;



function StrToTxType(s: string): TTxType;
begin
  result := ttUNKNOWN;
  if s = 'TV' then result := ttTV;
  if s = 'FM' then result := ttFM;
  if s = 'AM' then result := ttAM;
  if s = 'DA' then result := ttDAB;
  if s = 'DV' then result := ttDVB;
  if s = 'DAB' then result := ttDAB;
  if s = 'DVB' then result := ttDVB;
end;



procedure TTx.SetHMax(val: integer);
begin
  if _att._h_max <> val then _att._h_max := val;
end;



{
procedure TTx.CalcHef;
begin
end;
}



function TTx.GetHefFromMap(lon, lat, az: double): integer;
var p: pointer;
    ReliefData: TProcessReliefResult;
    points_count: longword;
    relief_step: double;
//    effective_height: double;
//    power: double;
    plon, plat: double;
begin
  relief_step := STEP;
  points_count := 0;
  p := nil;
  plon := lon;
  plat := lat;
  GetNextCoord(az, 15, plon, plat);
  _profile.GetProfile(lon, lat, plon, plat, relief_step, p, points_count);
  ReliefData := ProcessRelief (p, points_count, _att._h, RECEIVER_HEIGHT);
  result := Round(ReliefData.Hef);
end;



procedure TTx.SetUseUa50(val: boolean);
begin
  if val <> _att._useua50 then
  begin
    _att._useua50 := val;
    _profile.UseUA50 := val;
  end;
end;



function TTx.GetE_FreeSpace(plon, plat: double; param: TCalcParams): double;
var //az: double;
//    p: pointer;
    d: double;
    i: integer;
    points_count: longword;
//    relief_step: double;
    calc_1546_result: TCalc1546Result;
    effective_height: double;
    power: double;
begin
//  d := GetDistance(_lon, _lat, plon, plat);
//  az := GetAzimuth(_lon, _lat, plon, plat);
//  if d <= 0 then d := 0.1;
//  erp := GetErp(az);

//  result := erp - 20 * Log10(d) + 74.8;
//  result := erp + 106.9 - 20 * log10(d);
//  result := erp + 90 - 20 * log10(d);

//  relief_step := STEP;
  points_count := 0;
//  p := nil;
  i := Round( (RadToDeg(GetAzimuth(_att._lon, _att._lat, plon, plat) + PI)+180) / 10);
  d := GetDistance(_att._lon, _att._lat, plon, plat);
//  if d = 0 then d := 0.01;
  effective_height := GetHef(i);
  try
    calc_1546_result := Calc1546(_att._f, d, _att._h, RECEIVER_HEIGHT, param.pt, param.pm, _att._signal, 0, nil, effective_height);
  except
    TxShowMessage('Calc1546: '+FloatToStr(_att._f)+' '+FloatToStr(d)+' '+FloatToStr(h)+' '+IntToStr(param.pt)+' '+IntToStr(param.pm)+' '+IntToStr(_att._signal)+' '+IntToStr(points_count)+' '+FloatToStr(effective_height));
  end;
  power := GetERP(GetAzimuth(_att._lon, _att._lat, plon, plat));
  result := calc_1546_result.E + power;
end;



{
  устанавливает значения защ. отношений
  Эта проц. вызывается при расчете защ. от., когда
  наш передатчик - мешающий. Это позволяет расч. однажды
  защ. отн. и затем использовать эти значения многократно
  при итеративных расчетах
}
procedure TTx.SetProtect(pr_c, pr_t: double);
begin
  if _pr_c <> pr_c then _pr_c := pr_c;
  if _pr_t <> pr_t then _pr_t := pr_t;
end;



procedure TTx.SetUseForCalc(val: boolean);
begin
  if _att._use_for_calc <> val then _att._use_for_calc := val;
end;



procedure TTx.SetUseForDuel(val: boolean);
begin
  if _att._use_for_duel <> val then _att._use_for_duel := val;
end;



procedure TxShowMessage(s: string);
var title, mes: PChar;
    w: HWND;
begin
  title := ' Error: TTx object';
  mes := PChar(s);
  w := WindowFromDC(CreateDC(PChar('DISPLAY'), nil, nil, nil));
  MessageBox(w, mes, title, MB_OK or MB_ICONERROR or MB_SYSTEMMODAL);
end;



procedure TTx.GetAtt(p: PTxAttributes);
begin
  p^ := _att;
end;



procedure TTx.SetAtt(p: PTxAttributes);
begin
  _att := p^;
end;



function TTx.GetAttStr: string;
begin
  result := Code + ' - ' + Name + ' ' + FloatToStrF(F, ffGeneral, 5, 2) + ' MHz' + ' P = ' + FloatToStrF(Max(VErpMax, SErpMax), ffGeneral, 5, 2) + ' dBkW';
end;

procedure CodeToNum(code: TCodeString; var region, num: integer);
var region_str, num_str: string;
begin
  region_str := Copy(code, 1, 4);
  num_str := code;
  Delete(num_str, 1, 4);

  region := StrToInt(region_str);
  num := StrToInt(num_str);
end;



{
  Загружает данные о передатчике из строки ТВА файла
}
procedure TTx.LoadTxFromTVA(tva: string);
var s: string;
    slon, slat: TCoordString;
    lon, lat : double;
    n, i: integer;
begin
  SetDefaults;
  _att._code := GetDataFromTVA(tva, 8, 9);
  _att._name := GetDataFromTVA(tva, 39, 20);
  s := GetDataFromTVA(tva, 82, 3);
  if s[1]='R' then s := GetDataFromTVA(tva, 83, 2);
  _att._ch := TVAStrToInt(s);
  s := GetDataFromTVA(tva, 106, 1);
  if s = 'P' then
      _att._offset_type := otP else
      _att._offset_type := otNP;
  _att._tx_type_str := 'TV';
  _att._type := ttTV;
  s := GetDataFromTVA(tva, 79, 2);
  if s = 'D' then _att._sys := tvD;
  if s = 'K' then _att._sys := tvK;
  if s = 'B' then _att._sys := tvB;
  if s = 'G' then _att._sys := tvG;
  if s = 'D1' then _att._sys := tvD1;
  if s = 'H' then _att._sys := tvH;
  if s = 'L' then _att._sys := tvL;
  if s = 'I' then _att._sys := tvI;
  if s = 'K1' then _att._sys := tvK1;
  if s = 'BG' then _att._sys := tvB;
  if s = 'DK' then _att._sys := tvD;

  s := GetDataFromTVA(tva, 81, 1);

  if s = 'P' then _att._std := csPAL;
  if s = 'S' then _att._std := csSECAM;
  if s = 'N' then _att._std := csNTSC;

  _att._snd := sndAM;
  _att._mono_stereo := 'M';

  slat := GetDatafromTVA(tva, 59, 7);
  slon := GetDatafromTVA(tva, 67, 7);
  CoordStrToFloat(slon, slat, lon, lat);
  _att._lat := lat;
  _att._lon := lon;
  _att._h := TVAStrToInt(GetDataFromTVA(tva, 146, 3));
  s := GetDataFromTVA(tva, 149, 1);
  _att._direction := s[1];

  s := GetDataFromTVA(tva, 107, 5);
  VErpHor := TVAStrToFloat(s) - 30;
  s := GetDataFromTVA(tva, 112, 5);
  VErpVer := TVAStrToFloat(s) - 30;
  VErpMax := Max(_att._v_erp_ver, _att._v_erp_hor);


{
  Читаем горизонтальную составляющую эфф. изл. мощности
}
  n := 150;
  i := 0;
  while i <= 35 do
  begin
    s := GetDataFromTVA(tva, n, 2);
    _att._erp_az_h[i] := _att._v_erp_hor + TVAStrToFloat(s);
    i := i + 1;
    n := n + 2;
  end;
  _att._erp_az_h[36] := _att._erp_az_h[0];
{
  Читаем вертикальную составляющую эфф. изл. мощности
}
  n := 222;
  i := 0;
  while i <= 35 do
  begin
    s := GetDataFromTVA(tva, n, 2);
    _att._erp_az_v[i] := _att._v_erp_ver + TVAStrToFloat(s);
    i := i + 1;
    n := n + 2;
  end;
  _att._erp_az_v[36] := _att._erp_az_h[0];
{
  Читаем эффективные высоты по направлениям
  Позиция 307 (i=0) это Hэф.макс., а дальше идут
  эффективные высоты по направлениям. Т.е. i = 1 это
  направление на север (azimuth = 0)
}
  n := 307;
  i := 0;
  while i <= 36 do
  begin
    s := GetDataFromTVA(tva, n, 5);
    _att._hef_az[i] := TVAStrToFloat(s);
    i := i + 1;
    n := n + 5;
  end;
  HMax := Round(_att._hef_az[0]);

  s := GetDataFromTVA(tva, 145, 1);
  if s = 'H' then _att._polar := pHOR;
  if s = 'V' then _att._polar := pVER;
  if s = 'M' then _att._polar := pMIX;

  s := GetDataFromTVA(tva, 89, 9);
  VCarrier := TVAStrToFloat(s);
  s := GetDataFromTVA(tva, 98, 8);
  VOffset := TVAStrToInt(s) / 1e+6;
  s := GetDataFromTVA(tva, 117, 4);
  SCarrier := VCarrier + TVAStrToFloat(s);
end;



function TTx.GetDataFromTVA(tva: string; start, width: integer): string;
var s: string;
    i, n: integer;
begin
  s := Copy(tva, start, width);
  i := 1;
  n := Length(s);
  while s[n]=' ' do n := n - 1;
  while s[i]=' ' do i := i + 1;
  s := Copy(s, i, n-i+1);
  result := s;
end;



function TVAStrToFloat(sval: string): double;
var s: string;
    i: integer;
    sep: char;
begin
  if sval = '' then
  begin
    result := 0;
    Exit;
  end;
  s := '';
  sep := DecimalSeparator;
  DecimalSeparator := '.';
  for i := 1 to Length(sval) do if sval[i] <> ' ' then s := s + sval[i];
  try
    result := StrToFloat(s);
  finally
     DecimalSeparator := sep;
  end;
end;



function TVAStrToInt(sval: string): integer;
var s: string;
    i: integer;
begin
  if sval = '' then
  begin
    result := 0;
    Exit;
  end;
  s := '';
  for i := 1 to Length(sval) do if sval[i] <> ' ' then s := s + sval[i];
  result := StrToInt(s);
end;



{
  преобразует смещение частоты (в МГц) в единицы смещения
  равные 1/12 частоты строк
}
function FreqToOffset(f: double): integer;
const e = 50;
begin
  f := f * 1e+6;
  result := -1;
  if Abs(f - 25) < e then result := 0;
  if Abs(f - 1325) < e then result := 1;
  if Abs(f - 2625) < e then result := 2;
  if Abs(f - 3925) < e then result := 3;
  if Abs(f - 5225) < e then result := 4;
  if Abs(f - 6525) < e then result := 5;
  if Abs(f - 7825) < e then result := 6;
  if Abs(f - 9100) < e then result := 7;
  if Abs(f - 10400) < e then result := 8;
  if Abs(f - 11700) < e then result := 9;
  if Abs(f - 13000) < e then result := 10;
  if Abs(f - 14300) < e then result := 11;
  if Abs(f - 15600) < e then result := 12;
end;



function OffsetToFreq(off: integer): double;
begin
  case off of
    1:     result := 1325;
    2:     result := 2625;
    3:     result := 3925;
    4:     result := 5225;
    5:     result := 6525;
    6:     result := 7825;
    7:     result := 9100;
    8:     result := 10400;
    9:     result := 11700;
    10:     result := 13000;
    11:     result := 14300;
    12:     result := 15600;
   -1:     result := -1325;
   -2:     result := -2625;
   -3:     result := -3925;
   -4:     result := -5225;
   -5:     result := -6525;
   -6:     result := -7825;
   -7:     result := -9100;
   -8:     result := -10400;
   -9:     result := -11700;
   -10:     result := -13000;
   -11:     result := -14300;
   -12:     result := -15600;
  else      result := 0;
  end;
end;




end.
