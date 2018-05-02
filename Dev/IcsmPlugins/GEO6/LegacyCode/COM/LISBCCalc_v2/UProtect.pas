unit UProtect;

INTERFACE
uses Math, LISBCTxServer_TLB, UShare, UProtectOverlapping;


const

    BAND_I = 1;
    BAND_II = 2;
    BAND_III = 3;
    BAND_IV = 4;
    BAND_V = 5;

// Таблица защитных отношений ЧМ звуков. вещания от ТВ сигналов Д/СЕКАМ
// 1 строка (1, х) - разница частот между полезным сигналом и несущей изображения
// 2 строка (2, х) - защтные отношения для Моно звук. вещания
// 3 строка (3, х) - защтные отношения для Стерео звук. вещания

     PRFMFromTV_Table: array [1..3, 1..34] of double =
     (
     ( -2,  -1, -0.5, -0.15, -0.1, -0.05,  0, 0.05, 0.1, 0.15, 0.25, 0.5,  1,  2,  3,  4, 4.18, 4.25, 4.41, 4.48, 4.7,   5,   6, 6.25, 6.3, 6.4, 6.45, 6.475, 6.5, 6.525, 6.55, 6.6, 6.7,   7),
     (-30,  -2,    0,    19,   24,    30, 35,   30,  24,   19,   10,   0, -1, -3, -4, -5,    8,   10,   10,    8,  -5, -15, -25,  -13,  -5,   6,   15,    25,  28,    25,   15,   6,  -3, -30),
     (-12,  18,   20,    25,   35,    50, 45,   50,  35,   31,   25,  20, 20, 18, 17, 15,   25,   26,   26,   25,  15,   0,  -5,   -6,   5,  26,   40,    43,  35,     3,   40,  26,   0, -13)
     );

     PR_Adjacent_IPAL: array [1..3, 1..22] of double =
     (
     (-16,  -9.3,  -7.4,  -6.5,  -6.2,  -5.9,  -5.8,  -5.4,  -5.1,   -5,  -4.3,   -4,  -3.5,   -3,  -2.5,   -2, -1.25,  6.75,   8,   10,  14.75,   16),
     (-23,   -18,   -10,    11,    18,    18,    10,    10,    16,   16,    16,   12,     2,    2,     2,   14,    40,    35,   0,   -4,    -13,  -15),
     (-33,   -28,   -20,     1,     8,     8,     0,     0,     6,    6,     6,    2,    -8,   -8,    -8,    4,    32,    25,  -10, -14,    -23,  -25)
     );

type
{
    TTvSystems = (B, G, H, I, D, D1, K, K1, L);
    TTvStandards = (SECAM, PAL);
    TBand = (VHF, UHF);
    TSoundCarrier = (AM, FM);  // модуляция звука
    TInterferenceType = (T, C); // тип помехи - тропосферная или длительная
    TOffsetType = (P, NP); // тип СНЧ - precision, non-precision
}
{
  TTxType = (ttUNKNOWN, ttTV, ttFM, ttAM, ttDVB, ttDAB);
  TTvSystems = (tvB, tvG, tvH, tvI, tvD, tvD1, tvK, tvK1, tvL);
  TTvStandards = (csSECAM, csPAL, csNTSC, csNA); // NA - unknown colour system
  TSoundCarrier = (sndAM, sndFM);         // ????????? ?????
  TInterferenceType = (T, C);       // ??? ?????? - ???????????? ??? ??????????
  TOffsetType = (otP, otNP);            // ??? ??? - precision, non-precision
  TString2 = string[2];
  TString32 = string[32];
  TTxArray = array [0..36] of double;      // ?????? (???. ?????? ??? ???) ?? ???????????? ?? 10 ?? 360 ????.
  TCalcModel = (MODEL_P370, MODEL_P1546, MODEL_FREESPACE);
  TBCPolarization = (pVER, pHOR, pMIX, pCIR);       // antenna polarization - VERTICAL, HORIZONTAL, CIRCULAR
}

    TBand = (VHF, UHF);

    TTxParams = record
      id: integer;
      f: double;
      std: TBCTvStandards;
      snd: TBCSound;
      offset_type: TBCOffsetType;
      v_carrier: double;
      s_carrier: double;
      v_offset_line: integer;
      s_offset_line: integer;
      mono_stereo: char;
      tx_type: TBCTxType;
      case integer of
       0: (tvsys: TBCTvSystems;);
       1: (fmsys: TBCFMSystem;);
       2: (dvbsys: TBCDVBSystem);
    end;

    PTxParams = ^TTxParams;
// Функции для расчета защ. отношений для телевизионных передатчиков
// (т.е. ТВ против ТВ)

// для соседнего канала расчитывается защ. для тропосферной помехи
// для длительной помехи это значение необх. уменьшить на 10 дБ.

function ProtectUpperAdjacent(wanted_sys: TBCTvSystems): integer;

function ProtectLowerAdjacent(band: TBand; wanted_sys, unwanted_sys: TBCTvSystems; sound: TBCSound): integer;
function ProtectLowerAdjacentVHF(sound: TBCSound): integer;
function ProtectLowerAdjacentUHF(wanted_sys, unwanted_sys: TBCTvSystems): integer;
function ProtectLowerAdjacentUHF_G(unwanted_sys: TBCTvSystems): integer;
function ProtectLowerAdjacentUHF_H(unwanted_sys: TBCTvSystems): integer;
function ProtectLowerAdjacentUHF_I(unwanted_sys: TBCTvSystems): integer;
function ProtectLowerAdjacentUHF_D_D1_K(unwanted_sys: TBCTvSystems): integer;
function ProtectLowerAdjacentUHF_K1(unwanted_sys: TBCTvSystems): integer;
function ProtectLowerAdjacentUHF_L(unwanted_sys: TBCTvSystems): integer;

// protection ratio for co-channel interference
// необходимо делать поправку для разной модуляции видео:
// (-2 дБ если у защ.- отрицательная и у меш. - положительная)
// (+2 дБ если у защ.- положительнаяи у меш. - отрицательная )

function ProtectCo(it: TBCSInterferenceType; offset: integer; offsettype: TBCOffsetType): integer;
function ProtectCo_P(it: TBCSInterferenceType; offset: integer): integer;  // for Precision offset
function ProtectCo_NP(it: TBCSInterferenceType; offset: integer): integer; // for Non-Precision offset

// для зеркального канала также необходимы поправки (ВТ.655 таблица 10)

function ProtectImage_Nplus9_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nminus9_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nminus8_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nplus10_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nplus8_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;

function ProtectImage_Nplus9_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nminus9_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nminus8_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nplus10_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
function ProtectImage_Nplus8_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;

procedure ProtectTVOverlapping(tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);

// Функциии для расчета защ. отношений между ТВ и ОВЧ радиовещ.

// Расчет защитных отношений ЧМ звуков. вещания от ТВ сигналов Д/СЕКАМ
// для длительной помехи
// delta_f - разница частот между полезным сигналом и несущей изображения
// IsMono - тип вещания - (true  - Mono broadcast
//                         false - Stereo broadcast)
// Для тропосферной помехи полученый разультат уменьш. на 8 дБ

function ProtectFMfromSECAM_C(delta_f: double; IsMono: boolean): double;
function ProtectVisionFromFM_C(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
function ProtectVisionFromFM_T(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;

function ProtectSoundFromFM_Co_C(delta_f: double; wanted_sound: TBCSound): double;
function ProtectSoundFromFM_Co_T(delta_f: double; wanted_sound: TBCSound): double;
function ProtectDigitalSoundFromFM(UnwantedIsAM: boolean): integer;

function ProtectSoundFromFM_Adjacent_C(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
function ProtectSoundFromFM_Adjacent_T(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
function ProtectSoundFromFM_Adjacent_IPAL(delta_f: double; inter: TBCSInterferenceType): double;

function ProtectFMFromMonoFM_75_C(delta_f: double; amfm: TBCSound): double;
function ProtectFMFromMonoFM_75_T(delta_f: double; amfm: TBCSound): double;
function ProtectFMFromStereoFM_75_C(delta_f: double; amfm: TBCSound): double;
function ProtectFMFromStereoFM_75_T(delta_f: double; amfm: TBCSound): double;
procedure ProtectTVfromDVB8_Co(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
procedure ProtectTVfromDVB8_LowerAdjacent(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
procedure ProtectTVfromDVB8_UpperAdjacent(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
procedure ProtectTVfromDVB8_ImageNplus8(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
procedure ProtectTVfromDVB8_ImageNplus9(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
procedure ProtectTVfromDVB8_ImageNminus9(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);

procedure Protect_TV_TV2 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
procedure Protect_TV_FM2 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
procedure Protect_FM_TV2 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
procedure Protect_FM_FM_75(tx1, tx2: PTxParams; var pr_c, pr_t: double);
procedure Protect_FM_FM_50(tx1, tx2: PTxParams; var pr_c, pr_t: double);
procedure GetProtectRatio2(tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
procedure GetProtectRatio(const tx0, tx1: ILISBCTx; var pr_c, pr_t: Double);

function GetAntennaDiscrimination_I(azimuth: double): double;
function GetAntennaDiscrimination_III(azimuth: double): double;
function GetAntennaDiscrimination_IV(azimuth: double): double;

// Учет апроксимации антенны телевизионного приемника.
// на входе - номер полосы и азимут главного усиления антенны (радианы: -PI .. +PI )
function GetAntennaDiscrimination(band: byte; azimuth: double):double;



//  Учет поляризации антенны (только для вертик. и гориз. поляризации)
function GetPolarCorrect (wanted_polar, unwanted_polar: TBCPolarization): double;



IMPLEMENTATION



function GetPolarCorrect (wanted_polar, unwanted_polar: TBCPolarization): double;
begin
  if wanted_polar <> unwanted_polar then result := -16 else result := 0;
end;



function GetAntennaDiscrimination_I(azimuth: double): double;
const
  A1 = 50;
  A2 = 70;
  R1 = 0;
  R2 = -7;
begin
  result := R1;
  if azimuth >= DegToRad(A2) then result := R2;

  if (azimuth > DegToRad(A1)) and (azimuth < DegToRad(A2)) then
  begin
    result := (R2 / DegToRad(A2 - A1)) * azimuth - DegToRad(A1) * (R2 / DegToRad(A2 - A1));
  end;
end;



function GetAntennaDiscrimination_III(azimuth: double): double;
const
  A1 = 27;
  A2 = 60;
  R1 = 0;
  R2 = -14;
begin
  result := R1;
  if azimuth >= DegToRad(A2) then result := R2;

  if (azimuth > DegToRad(A1)) and (azimuth < DegToRad(A2)) then
  begin
    result := (R2 / DegToRad(A2 - A1)) * azimuth - DegToRad(A1) * (R2 / DegToRad(A2 - A1));
  end;
end;



function GetAntennaDiscrimination_IV(azimuth: double): double;
const
  A1 = 20;
  A2 = 60;
  R1 = 0;
  R2 = -16;
begin
  result := R1;
  if azimuth >= DegToRad(A2) then result := R2;

  if (azimuth > DegToRad(A1)) and (azimuth < DegToRad(A2)) then
  begin
    result := (R2 / DegToRad(A2 - A1)) * azimuth - DegToRad(A1) * (R2 / DegToRad(A2 - A1));
  end;
end;



{
Избирательность приемной атенны в зависимости от направления
}
function GetAntennaDiscrimination(band: byte; azimuth: double): double;
begin
  azimuth := abs (azimuth);
  case band of
    BAND_I: result := GetAntennaDiscrimination_I(azimuth);
    BAND_II: result := GetAntennaDiscrimination_I(azimuth);
    BAND_III: result := GetAntennaDiscrimination_III(azimuth);
    BAND_IV: result := GetAntennaDiscrimination_IV(azimuth);
    BAND_V: result := GetAntennaDiscrimination_IV(azimuth);
  else
    result := 0;
  end;
end;



procedure Protect_DAB_DAB (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
begin
  pr_c := 28;
  pr_t := 28;
end;



procedure Protect_TV_TV2 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
const PRECISION = 1e-4;
var offset: integer;
    band: TBand;
    is7Mhz: boolean;
begin
{
  Проверяем, является ли система с сеткой 7 МГц
}

  if (tx1.tvsys = tvB) and (tx2.tvsys = tvB) then is7Mhz := true else is7Mhz := false;
{
  соканальные передатчики и перекрывающиеся каналы
}

  if ((tx2.v_carrier - tx1.v_carrier) >= -2) and ((tx2.v_carrier - tx1.v_carrier) <= 6) then
  begin
    if Abs(tx1.v_carrier - tx2.v_carrier) < PRECISION then
    begin
      if tx2.tx_type = ttDVB then ProtectTVFromDVB8_Co(tx1.std, tx1.tvsys, pr_c, pr_t)
      else
      begin
        offset := tx2.v_offset_line;
        offset := Abs(offset - tx1.v_offset_line);
        begin
          pr_c := ProtectCo(itCONT, offset, tx2.offset_type);
          pr_t := ProtectCo(itTROPO, offset, tx2.offset_type);
        end;
      end;
    end else ProtectTVOverlapping(tx1, tx2, pr_c, pr_t);
  end;
{
  зеркальный канал n+8
}
  if (Abs((tx2.v_carrier - tx1.v_carrier) - 8*8) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - 7*8) < PRECISION)) then
  begin
    if tx2.tx_type = ttDVB then ProtectTVfromDVB8_ImageNplus8(tx1.std, tx1.tvsys, pr_c, pr_t)
    else
    begin
      pr_c := ProtectImage_Nplus8_C(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
      pr_t := ProtectImage_Nplus8_T(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
    end;
  end;

{
  зеркальный канал n+9
}
  if (Abs((tx2.v_carrier - tx1.v_carrier) - 8*9) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - 7*9) < PRECISION)) then
  begin
    if tx2.tx_type = ttDVB then ProtectTVfromDVB8_ImageNplus9(tx1.std, tx1.tvsys, pr_c, pr_t)
    else
    begin
      pr_c := ProtectImage_Nplus9_C(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
      pr_t := ProtectImage_Nplus9_T(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
    end;
  end;

{
  зеркальный канал n+10
}
  if (Abs((tx2.v_carrier - tx1.v_carrier) - 8*10) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - 7*10) < PRECISION)) then
  begin
    pr_c := ProtectImage_Nplus10_C(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
    pr_t := ProtectImage_Nplus10_T(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
  end;

{
  зеркальный канал n-8
}
 if (Abs((tx2.v_carrier - tx1.v_carrier) - (-8*8)) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - (-7*8)) < PRECISION)) then
 begin
    pr_c := ProtectImage_Nminus8_C(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
    pr_t := ProtectImage_Nminus8_T(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
  end;

{
  зеркальный канал n-9
}
 if (Abs((tx2.v_carrier - tx1.v_carrier) - (-8*9)) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - (-7*9)) < PRECISION)) then
  begin
    if tx2.tx_type = ttDVB then ProtectTVfromDVB8_ImageNminus9(tx1.std, tx1.tvsys, pr_c, pr_t)
    else
    begin
      pr_c := ProtectImage_Nminus9_C(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
      pr_t := ProtectImage_Nminus9_T(tx1.tvsys, tx1.std, tx2.tvsys, tx2.std);
    end;
  end;

{
  верхний соседний канал
}
 if (Abs((tx2.v_carrier - tx1.v_carrier) - (8*1)) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - (7*1)) < PRECISION)) then
  begin
    if tx2.tx_type = ttDVB then ProtectTVfromDVB8_UpperAdjacent(tx1.std, tx1.tvsys, pr_c, pr_t)
    else
    begin
      pr_t := ProtectUpperAdjacent(tx1.tvsys);
      pr_c := pr_t + 10;
    end;
  end;

{
  нижний соседний канал
}
 if (Abs((tx2.v_carrier - tx1.v_carrier) - (-8*1)) < PRECISION) or (is7Mhz and (Abs((tx2.v_carrier - tx1.v_carrier) - (-7*1)) < PRECISION)) then
  begin
    if tx2.tx_type = ttDVB then ProtectTVfromDVB8_LowerAdjacent(tx1.std, tx1.tvsys, pr_c, pr_t)
    else
    begin
      if tx1.v_carrier < 450 then band := VHF else band := UHF;
      pr_t := ProtectLowerAdjacent(band, tx1.tvsys, tx2.tvsys, tx1.snd);
      pr_c := pr_t + 10;
    end;
  end;

end;



procedure Protect_FM_TV2 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
var delta_f: double;
    is_mono: boolean;
begin
 delta_f := tx1.S_Carrier - tx2.V_Carrier;
 if tx1.Mono_Stereo = 'M' then is_mono := true else is_mono := false;
 pr_c := ProtectFMfromSECAM_C(delta_f, is_mono);
 if pr_c = NO_INTERFERENCE then pr_t := NO_INTERFERENCE else
   pr_t := pr_c - 8;
end;



procedure Protect_TV_FM2 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
var delta_f: double;
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  delta_f := tx1.v_carrier - tx2.S_carrier;
  if (delta_f >= 1.25) and (delta_f <= 6.3) then
  begin
    pr_c := ProtectVisionFromFM_C(delta_f, tx1.tvsys, tx1.std);
    pr_t := ProtectVisionFromFM_T(delta_f, tx1.tvsys, tx1.std);
  end;

  // защита ТВ звука от ОВЧ ЧМ вещателей
  delta_f := Abs(tx1.S_carrier - tx2.S_carrier);
  if (delta_f >= 0) and (delta_f <= 0.25) then
  begin
    pr_c := ProtectSoundFromFM_Co_C(delta_f, tx1.snd);
    pr_t := ProtectSoundFromFM_Co_T(delta_f, tx1.snd);
  end;

  delta_f := tx1.V_carrier - tx2.S_carrier;
  if ((delta_f >= -15) and (delta_f <= -1.25)) or ((delta_f >= 5.75) and (delta_f <= 15)) then
  begin
    pr_c := ProtectSoundFromFM_Adjacent_C(delta_f, tx1.tvsys, tx1.std);
    pr_t := ProtectSoundFromFM_Adjacent_T(delta_f, tx1.tvsys, tx1.std);
  end;

  if (tx1.tvsys = tvI) and (tx1.std = csPAL) then
  if ((delta_f >= -16) and (delta_f <= -1.25)) or ((delta_f >= 6.75) and (delta_f <= 16)) then
  begin
    pr_c := ProtectSoundFromFM_Adjacent_C(delta_f, tx1.tvsys, tx1.std);
    pr_t := ProtectSoundFromFM_Adjacent_T(delta_f, tx1.tvsys, tx1.std);
  end;
end;



procedure GetProtectRatio2(tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  if (tx1.tx_type = ttTV) and ((tx2.tx_type = ttTV) or (tx2.tx_type = ttDVB))
    then Protect_TV_TV2(tx1,tx2,pr_c,pr_t);

  if (tx1.tx_type = ttTV) and (tx2.tx_type = ttFM)
    then Protect_TV_FM2(tx1,tx2,pr_c,pr_t);

  if (tx1.tx_type = ttFM) and (tx2.tx_type = ttTV)
    then Protect_FM_TV2(tx1,tx2,pr_c,pr_t);

  if (tx1.tx_type = ttFM) and (tx2.tx_type = ttFM) then
  begin
    if (tx1.fmsys = fm1) or (tx1.fmsys = fm4)
        then Protect_FM_FM_75(tx1,tx2,pr_c,pr_t)
        else Protect_FM_FM_50(tx1,tx2,pr_c,pr_t);
  end;

//  tx2.SetProtect(pr_c, pr_t);
end;



function ProtectSoundFromFM_Adjacent_IPAL(delta_f: double; inter: TBCSInterferenceType): double;
var fmin, fmax: double;
    prmin, prmax: double;
    i: integer;
begin
  i := 1;
  while (delta_f >= PR_Adjacent_IPAL[1, i]) and (i <= 22) do
  begin
    i := i+1;
  end;
  if (delta_f < PR_Adjacent_IPAL[1, 1]) or (delta_f > PR_Adjacent_IPAL[1, 22]) then result := NO_INTERFERENCE else
  begin
    fmin := PR_Adjacent_IPAL[1, i-1];
    fmax := PR_Adjacent_IPAL[1, i];
    prmax := PR_Adjacent_IPAL[2, i];
    prmin := PR_Adjacent_IPAL[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;

function ProtectSoundFromFM_Adjacent_C(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
var pr_table: array [1..2, 1..10] of double;
    i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  pr_table[1,1] := -14;
  pr_table[1,2] := -6;
  pr_table[1,3] := -2.5;
  pr_table[1,4] := -1.5;
  pr_table[1,5] := -1.25;
  pr_table[1,6] := 5.75;
  pr_table[1,7] := 6.2;
  pr_table[1,8] := 6.75;
  pr_table[1,9] := 8.5;
  pr_table[1,10] := 15;

  pr_table[2,1] := -10;
  pr_table[2,2] := -10;
  pr_table[2,3] := 11;
  pr_table[2,4] := 11;
  pr_table[2,5] := 40;
  pr_table[2,6] := 30;
  pr_table[2,7] := -2;
  pr_table[2,8] := 30;
  pr_table[2,9] := -2;
  pr_table[2,10] := -2;
  if (wanted_std = csPAL) then
  begin
    pr_table[2,6] := 35;
  end;
  if (wanted_sys = tvB)
  or (wanted_sys = tvD)
  or (wanted_sys = tvD1)
  or (wanted_sys = tvG)
  or (wanted_sys = tvK) then pr_table[2,5] := 32;
  i := 1;
  while (delta_f >= pr_table[1, i]) and (i <= 10) do
  begin
    i := i+1;
  end;
  if (delta_f < pr_table[1, 1]) or (delta_f > pr_table[1, 10]) then result := NO_INTERFERENCE else
  begin
    fmin := pr_table[1, i-1];
    fmax := pr_table[1, i];
    prmax := pr_table[2, i];
    prmin := pr_table[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectSoundFromFM_Adjacent_T(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
var pr_table: array [1..2, 1..10] of double;
    i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  pr_table[1,1] := -14;
  pr_table[1,2] := -6;
  pr_table[1,3] := -2.5;
  pr_table[1,4] := -1.5;
  pr_table[1,5] := -1.25;
  pr_table[1,6] := 5.75;
  pr_table[1,7] := 6.2;
  pr_table[1,8] := 6.75;
  pr_table[1,9] := 8.5;
  pr_table[1,10] := 15;


  pr_table[2,1] := -15;
  pr_table[2,2] := -15;
  pr_table[2,3] := 1;
  pr_table[2,4] := 1;
  pr_table[2,5] := 32;
  pr_table[2,6] := 25;
  pr_table[2,7] := -12;
  pr_table[2,8] := 25;
  pr_table[2,9] := -12;
  pr_table[2,10] := -12;

  if (wanted_sys = tvB)
  or (wanted_sys = tvD)
  or (wanted_sys = tvD1)
  or (wanted_sys = tvG)
  or (wanted_sys = tvK) then pr_table[2,5] := 23;

  i := 1;
  while (delta_f >= pr_table[1, i]) and (i <= 10) do
  begin
    i := i+1;
  end;
  if (delta_f < pr_table[1, 1]) or (delta_f > pr_table[1, 10]) then result := NO_INTERFERENCE else
  begin
    fmin := pr_table[1, i-1];
    fmax := pr_table[1, i];
    prmax := pr_table[2, i];
    prmin := pr_table[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectDigitalSoundFromFM(UnwantedIsAM: boolean): integer;
begin
  if UnwantedIsAM then result := 11 else result := 12;
end;



function ProtectSoundFromFM_Co_T(delta_f: double; wanted_sound: TBCSound): double;
var pr_table: array [1..2, 1..4] of double;
    i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  pr_table[1,1] := 0 / 1e+6;
  pr_table[1,2] := 15 / 1e+6;
  pr_table[1,3] := 50 / 1e+6;
  pr_table[1,4] := 250 / 1e+6;
  if wanted_sound = sndFM then
  begin
    pr_table[2,1] := 32;
    pr_table[2,2] := 30;
    pr_table[2,3] := 22;
    pr_table[2,4] := -6;
  end;
  if wanted_sound = sndAM then
  begin
    pr_table[2,1] := 49;
    pr_table[2,2] := 40;
    pr_table[2,3] := 10;
    pr_table[2,4] := 7;
  end;

  i := 1;
  while (delta_f >= pr_table[1, i]) and (i <= 4) do
  begin
    i := i+1;
  end;
  if (delta_f < pr_table[1, 1]) or (delta_f > pr_table[1, 4]) then result := NO_INTERFERENCE else
  begin
    fmin := pr_table[1, i-1];
    fmax := pr_table[1, i];
    prmax := pr_table[2, i];
    prmin := pr_table[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectSoundFromFM_Co_C(delta_f: double; wanted_sound: TBCSound): double;
var pr_table: array [1..2, 1..4] of double;
    i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  pr_table[1,1] := 0 / 1e+6;
  pr_table[1,2] := 15 / 1e+6;
  pr_table[1,3] := 50 / 1e+6;
  pr_table[1,4] := 250 / 1e+6;
  if wanted_sound = sndFM then
  begin
    pr_table[2,1] := 39;
    pr_table[2,2] := 35;
    pr_table[2,3] := 24;
    pr_table[2,4] := -6;
  end;
  if wanted_sound = sndAM then
  begin
    pr_table[2,1] := 56;
    pr_table[2,2] := 50;
    pr_table[2,3] := 15;
    pr_table[2,4] := 12;
  end;

  i := 1;
  while (delta_f >= pr_table[1, i]) and (i <= 4) do
  begin
    i := i+1;
  end;
  if (delta_f < pr_table[1, 1]) or (delta_f > pr_table[1, 4]) then result := NO_INTERFERENCE else
  begin
    fmin := pr_table[1, i-1];
    fmax := pr_table[1, i];
    prmax := pr_table[2, i];
    prmin := pr_table[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectVisionFromFM_C(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
var pr_table: array [1..2, 1..11] of double;
    i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  pr_table[1,1] := -1.25;
  pr_table[1,2] := -0.5;
  pr_table[1,3] := 0;
  pr_table[1,4] := 0.5;
  pr_table[1,5] := 1;
  pr_table[1,6] := 2;
  pr_table[1,7] := 3;
  pr_table[1,8] := 3.6;
  pr_table[1,9] := 4.8;
  pr_table[1,10] := 5.7;
  pr_table[1,11] := 6.0;

  if (wanted_std = csSECAM) then pr_table[1,9] := 4.3;
  if (wanted_std = csSECAM) then pr_table[1,11] := 6.3;
  if (wanted_sys = tvB) or (wanted_sys = tvG) then pr_table[1,10] := 5.3;
  if (wanted_sys = tvB) or (wanted_sys = tvG) then pr_table[1,11] := 6.0;

  pr_table[2,1] := 40;
  pr_table[2,2] := 50;
  pr_table[2,3] := 54;
  pr_table[2,4] := 58;
  pr_table[2,5] := 58;
  pr_table[2,6] := 54;
  pr_table[2,7] := 44;
  pr_table[2,8] := 53;
  pr_table[2,9] := 53;
  pr_table[2,10] := 35;
  pr_table[2,11] := 35;

  if (wanted_std = csSECAM) then
  begin
    pr_table[2,10] := 30;
    pr_table[2,11] := 30;
    if not ((wanted_sys = tvD) or (wanted_sys = tvK)) then
    begin
      pr_table[2,8] := 45;
      pr_table[2,9] := 45;
    end;
  end;
  if (wanted_sys = tvB)
  or (wanted_sys = tvD)
  or (wanted_sys = tvD1)
  or (wanted_sys = tvG)
  or (wanted_sys = tvK) then pr_table[2,1] := 32;

  i := 1;
  while (delta_f >= pr_table[1, i]) and (i <= 11) do
  begin
    i := i+1;
  end;
  if (delta_f < pr_table[1, 1]) or (delta_f > pr_table[1, 11]) then result := NO_INTERFERENCE else
  begin
    fmin := pr_table[1, i-1];
    fmax := pr_table[1, i];
    prmax := pr_table[2, i];
    prmin := pr_table[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectVisionFromFM_T(delta_f: double; wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards): double;
var pr_table: array [1..2, 1..11] of double;
    i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  pr_table[1,1] := -1.25;
  pr_table[1,2] := -0.5;
  pr_table[1,3] := 0;
  pr_table[1,4] := 0.5;
  pr_table[1,5] := 1;
  pr_table[1,6] := 2;
  pr_table[1,7] := 3;
  pr_table[1,8] := 3.6;
  pr_table[1,9] := 4.8;
  pr_table[1,10] := 5.7;
  pr_table[1,11] := 6.0;

  if (wanted_std = csSECAM) then pr_table[1,9] := 4.3;
  if (wanted_std = csSECAM) then pr_table[1,11] := 6.3;
  if (wanted_sys = tvB) or (wanted_sys = tvG) then pr_table[1,10] := 5.3;
  if (wanted_sys = tvB) or (wanted_sys = tvG) then pr_table[1,11] := 6.0;

  pr_table[2,1] := 32;
  pr_table[2,2] := 44;
  pr_table[2,3] := 47;
  pr_table[2,4] := 50;
  pr_table[2,5] := 50;
  pr_table[2,6] := 44;
  pr_table[2,7] := 36;
  pr_table[2,8] := 45;
  pr_table[2,9] := 45;
  pr_table[2,10] := 25;
  pr_table[2,11] := 25;

  if (wanted_std = csSECAM) then
  begin
    if not ((wanted_sys = tvD) or (wanted_sys = tvK)) then
    begin
      pr_table[2,8] := 40;
      pr_table[2,9] := 40;
    end;
  end;
  if (wanted_sys = tvB)
  or (wanted_sys = tvD)
  or (wanted_sys = tvD1)
  or (wanted_sys = tvG)
  or (wanted_sys = tvK) then pr_table[2,1] := 32;

  i := 1;
  while (delta_f >= pr_table[1, i]) and (i <= 11) do
  begin
    i := i+1;
  end;
  if (delta_f < pr_table[1, 1]) or (delta_f > pr_table[1, 11]) then result := NO_INTERFERENCE else
  begin
    fmin := pr_table[1, i-1];
    fmax := pr_table[1, i];
    prmax := pr_table[2, i];
    prmin := pr_table[2, i-1];
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectFMfromSECAM_C(delta_f: double; IsMono: boolean): double;
var i: integer;
    fmin, fmax: double;
    prmin, prmax: double;
begin
  i := 1;
  while (delta_f >= PRFMFromTV_Table[1, i]) and (i <= 34) do
  begin
    i := i+1;
  end;
  if (delta_f < PRFMFromTV_Table[1, 1]) or (delta_f > PRFMFromTV_Table[1, 34])
  then result := NO_INTERFERENCE else
  begin
    fmin := PRFMFromTV_Table[1, i-1];
    fmax := PRFMFromTV_Table[1, i];
    if IsMono then
    begin
      prmax := PRFMFromTV_Table[2, i];
      prmin := PRFMFromTV_Table[2, i-1];
    end else
    begin
      prmax := PRFMFromTV_Table[3, i];
      prmin := PRFMFromTV_Table[3, i-1];
    end;
    result := (delta_f - fmin) * (prmax - prmin) / (fmax - fmin) + prmin;
  end;
end;



function ProtectImage_Nminus9_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 6;
    if (unwanted_sys = tvI) then result := 6;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 3;
    if (unwanted_sys = tvK1) then result := 3;
    if (unwanted_sys = tvL) then result := 7;
  end;
end;

function ProtectImage_Nminus8_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -20;
    if (unwanted_sys = tvI) then result := -20;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -20;
    if (unwanted_sys = tvK1) then result := -20;
    if (unwanted_sys = tvL) then result := -20;
  end;
end;



function ProtectImage_Nplus9_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvD1) or (wanted_sys = tvG) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 6;
    if (unwanted_sys = tvI) then result := 2;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -5;
    if (unwanted_sys = tvK1) then result := -5;
    if (unwanted_sys = tvL) then result := -5;
  end;

  if (wanted_sys = tvH) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 6;
    if (unwanted_sys = tvI) then result := -2;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -1;
    if (unwanted_sys = tvK1) then result := -1;
    if (unwanted_sys = tvL) then result := 3;
  end;

  if (wanted_sys = tvI) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -4;
    if (unwanted_sys = tvI) then result := -2;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -2;
    if (unwanted_sys = tvK1) then result := -2;
    if (unwanted_sys = tvL) then result := 2;
  end;

  if (wanted_sys = tvD) and (unwanted_std = csPAL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 11;
    if (unwanted_sys = tvI) then result := 11;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := 11;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 11;
    if (unwanted_sys = tvK1) then result := 11;
    if (unwanted_sys = tvL) then result := 13;
  end;

  if ((wanted_sys = tvD) or (wanted_sys = tvK))and (unwanted_std = csSECAM) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 21;
    if (unwanted_sys = tvI) then result := 21;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := 21;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 21;
    if (unwanted_sys = tvK1) then result := 21;
    if (unwanted_sys = tvL) then result := 23;
  end;

  if (wanted_sys = tvK1) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -5;
    if (unwanted_sys = tvI) then result := 0;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 4;
    if (unwanted_sys = tvK1) then result := 4;
    if (unwanted_sys = tvL) then result := 8;
  end;
end;



function ProtectImage_Nplus10_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvK1) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 7;
    if (unwanted_sys = tvI) then result := 7;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 7;
    if (unwanted_sys = tvK1) then result := 7;
    if (unwanted_sys = tvL) then result := 9;
  end;
end;



function ProtectImage_Nplus8_C(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvD) and (unwanted_std = csPAL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -1;
    if (unwanted_sys = tvI) then result := -15;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := -10;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -10;
    if (unwanted_sys = tvK1) then result := -10;
    if (unwanted_sys = tvL) then result := -6;
  end;

  if ((wanted_sys = tvD) or (wanted_sys = tvK))and (unwanted_std = csSECAM) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 9;
    if (unwanted_sys = tvI) then result := -10;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := -7;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -7;
    if (unwanted_sys = tvK1) then result := -7;
    if (unwanted_sys = tvL) then result := -3;
  end;
end;



function ProtectImage_Nminus9_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -3;
    if (unwanted_sys = tvI) then result := -2;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -4;
    if (unwanted_sys = tvK1) then result := -4;
    if (unwanted_sys = tvL) then result := 0;
  end;
end;



function ProtectImage_Nminus8_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -20;
    if (unwanted_sys = tvI) then result := -20;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -20;
    if (unwanted_sys = tvK1) then result := -20;
    if (unwanted_sys = tvL) then result := -20;
  end;
end;



function ProtectImage_Nplus9_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvD1) or (wanted_sys = tvG) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -1;
    if (unwanted_sys = tvI) then result := -4;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -12;
    if (unwanted_sys = tvK1) then result := -12;
    if (unwanted_sys = tvL) then result := -8;
  end;

  if (wanted_sys = tvH) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -1;
    if (unwanted_sys = tvI) then result := -4;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -9;
    if (unwanted_sys = tvK1) then result := -9;
    if (unwanted_sys = tvL) then result := -5;
  end;

  if (wanted_sys = tvI) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -13;
    if (unwanted_sys = tvI) then result := -10;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := -10;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -10;
    if (unwanted_sys = tvK1) then result := -10;
    if (unwanted_sys = tvL) then result := -6;
  end;

  if (wanted_sys = tvD) and (unwanted_std = csPAL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 3;
    if (unwanted_sys = tvI) then result := 3;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := 3;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 3;
    if (unwanted_sys = tvK1) then result := 3;
    if (unwanted_sys = tvL) then result := 5;
  end;

  if ((wanted_sys = tvD) or (wanted_sys = tvK))and (unwanted_std = csSECAM) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 13;
    if (unwanted_sys = tvI) then result := 13;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := 13;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 13;
    if (unwanted_sys = tvK1) then result := 13;
    if (unwanted_sys = tvL) then result := 15;
  end;

  if (wanted_sys = tvK1) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -13;
    if (unwanted_sys = tvI) then result := -9;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -5;
    if (unwanted_sys = tvK1) then result := -5;
    if (unwanted_sys = tvL) then result := -1;
  end;

end;

function ProtectImage_Nplus10_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvK1) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 7;
    if (unwanted_sys = tvI) then result := 7;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := 7;
    if (unwanted_sys = tvK1) then result := 7;
    if (unwanted_sys = tvL) then result := 9;
  end;
end;



function ProtectImage_Nplus8_T(wanted_sys: TBCTvSystems; wanted_std: TBCTvStandards; unwanted_sys: TBCTvSystems; unwanted_std: TBCTvStandards): integer;
begin
  result := NO_INTERFERENCE;
  if (wanted_sys = tvD) and (unwanted_std = csPAL) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := -8;
    if (unwanted_sys = tvI) then result := -25;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := -20;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -20;
    if (unwanted_sys = tvK1) then result := -20;
    if (unwanted_sys = tvL) then result := -16;
  end;

  if ((wanted_sys = tvD) or (wanted_sys = tvK))and (unwanted_std = csSECAM) then
  begin
    if (unwanted_sys = tvG) or (unwanted_sys = tvH) then result := 2;
    if (unwanted_sys = tvI) then result := -15;
    if (unwanted_sys = tvD) and (unwanted_std = csPAL) then result := -12;
    if ((unwanted_sys = tvD) or (unwanted_sys = tvD1) or (unwanted_sys = tvK)) and (unwanted_std = csSECAM) then result := -12;
    if (unwanted_sys = tvK1) then result := -12;
    if (unwanted_sys = tvL) then result := -8;
  end;
end;



function ProtectCo(it: TBCSInterferenceType; offset: integer; offsettype: TBCOffsetType): integer;
var n: integer;
begin
  if offset < 0 then n := -12 else n := 12;
  if offset <> 0 then while Abs(offset) > 12 do offset := offset - n;
  case offsettype of
    otPRECISION: result := ProtectCo_P(it, offset);
    otNONPRECISION: result := ProtectCo_NP(it, offset);
  else
    result := NO_INTERFERENCE;
  end;
end;



function ProtectCo_P(it: TBCSInterferenceType; offset: integer): integer;
begin
  result := NO_INTERFERENCE;
  if it = itTROPO then
  case offset of
    0: result := 32;
    1: result := 34;
    2: result := 30;
    3: result := 26;
    4: result := 22;
    5: result := 22;
    6: result := 24;
    7: result := 22;
    8: result := 22;
    9: result := 26;
    10: result := 30;
    11: result := 34;
    12: result := 38;
  end else
  case offset of
    0: result := 36;
    1: result := 38;
    2: result := 34;
    3: result := 30;
    4: result := 27;
    5: result := 27;
    6: result := 30;
    7: result := 27;
    8: result := 27;
    9: result := 30;
    10: result := 34;
    11: result := 38;
    12: result := 42;
  end;
end;



function ProtectCo_NP(it: TBCSInterferenceType; offset: integer): integer;
begin
  result := NO_INTERFERENCE;
  if it = itTROPO then
  case offset of
    0: result := 45;
    1: result := 44;
    2: result := 40;
    3: result := 34;
    4: result := 30;
    5: result := 28;
    6: result := 27;
    7: result := 28;
    8: result := 30;
    9: result := 34;
    10: result := 40;
    11: result := 44;
    12: result := 45;
  end else
  case offset of
    0: result := 52;
    1: result := 51;
    2: result := 48;
    3: result := 44;
    4: result := 40;
    5: result := 36;
    6: result := 33;
    7: result := 36;
    8: result := 40;
    9: result := 44;
    10: result := 48;
    11: result := 51;
    12: result := 52;
  end;
end;



function ProtectLowerAdjacentVHF(sound: TBCSound): integer;
begin
  case sound of
    sndFM: result := -9;
    sndAM: result := -8;
  else
    result := NO_INTERFERENCE
  end;
end;



function ProtectLowerAdjacentUHF_G(unwanted_sys: TBCTvSystems): integer;
begin
  case unwanted_sys of
    tvG: result := -9;
    tvH: result := -9;
    tvI: result := -9;
    tvD: result := -9;
    tvD1: result := -9;
    tvK: result := -9;
    tvK1: result := -9;
    tvL: result := -5;
  else
    result := NO_INTERFERENCE;
  end;
end;



function ProtectLowerAdjacentUHF_D_D1_K(unwanted_sys: TBCTvSystems): integer;
begin
  case unwanted_sys of
    tvG: result := -9;
    tvH: result := -9;
    tvI: result := -9;
    tvD: result := -9;
    tvD1: result := -9;
    tvK: result := -9;
    tvK1: result := -9;
    tvL: result := -5;
  else
    result := NO_INTERFERENCE;
  end;
end;



function ProtectLowerAdjacentUHF_H(unwanted_sys: TBCTvSystems): integer;
begin
  case unwanted_sys of
    tvG: result := -9;
    tvH: result := -9;
    tvI: result := -9;
    tvD: result := 13;
    tvD1: result := 13;
    tvK: result := 13;
    tvK1: result := 13;
    tvL: result := 17;
  else
    result := NO_INTERFERENCE;
  end;
end;



function ProtectLowerAdjacentUHF_K1(unwanted_sys: TBCTvSystems): integer;
begin
  case unwanted_sys of
    tvG: result := -9;
    tvH: result := -9;
    tvI: result := -9;
    tvD: result := -9;
    tvD1: result := -9;
    tvK: result := -9;
    tvK1: result := -9;
    tvL: result := 17;
  else
    result := NO_INTERFERENCE;
  end;
end;



procedure ProtectTVfromDVB8_Co(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  case wanted_std of
    csPAL:
        begin
          case wanted_sys of
            tvB: begin
                   pr_t := 34;
                   pr_c := 40;
                 end;
             tvB1: begin
                   pr_t := 34;
                   pr_c := 40;
                  end;
            tvG: begin
                   pr_t := 34;
                   pr_c := 40;
                 end;
            tvD: begin
                   pr_t := 34;
                   pr_c := 40;
                 end;
            tvK: begin
                   pr_t := 34;
                   pr_c := 40;
                 end;
            tvI: begin
                   pr_t := 37;
                   pr_c := 41;
                 end;
          end;
        end;
    csSECAM:
        begin
          case wanted_sys of
            tvL: begin
                   pr_t := 37;
                   pr_c := 42;
                 end;
            tvD: begin
                   pr_t := 35;
                   pr_c := 41;
                 end;
            tvK: begin
                   pr_t := 35;
                   pr_c := 41;
                 end;
          end;
        end;
  end;
end;



procedure ProtectTVfromDVB8_LowerAdjacent(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  case wanted_std of
    csPAL:
        begin
          case wanted_sys of
            tvB1: begin
                   pr_t := -7;
                   pr_c := -4;
                 end;
            tvG: begin
                   pr_t := -7;
                   pr_c := -4;
                 end;
            tvD: begin
                   pr_t := -7;
                   pr_c := -4;
                 end;
            tvK: begin
                   pr_t := -7;
                   pr_c := -4;
                 end;
            tvI: begin
                   pr_t := -8;
                   pr_c := -4;
                 end;
          end;
        end;
    csSECAM:
        begin
          case wanted_sys of
            tvL: begin
                   pr_t := -9;
                   pr_c := -7;
                 end;
            tvD: begin
                   pr_t := -5;
                   pr_c := -1;
                 end;
            tvK: begin
                   pr_t := -5;
                   pr_c := -1;
                 end;
          end;
        end;
  end;
end;



procedure ProtectTVfromDVB8_UpperAdjacent(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  case wanted_std of
    csPAL:
        begin
          case wanted_sys of
            tvB1: begin
                   pr_t := -9;
                   pr_c := -7;
                 end;
            tvG: begin
                   pr_t := -9;
                   pr_c := -7;
                 end;
            tvI: begin
                   pr_t := -10;
                   pr_c := -6;
                 end;
          end;
        end;
    csSECAM:
        begin
          case wanted_sys of
            tvL: begin
                   pr_t := -1;
                   pr_c := -1;
                 end;
            tvD: begin
                   pr_t := -8;
                   pr_c := -5;
                 end;
            tvK: begin
                   pr_t := -8;
                   pr_c := -5;
                 end;
          end;
        end;
  end;
end;



procedure ProtectTVfromDVB8_ImageNplus8(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  case wanted_std of
    csSECAM:
        begin
          case wanted_sys of
            tvD: begin
                   pr_t := -16;
                   pr_c := -11;
                 end;
            tvK: begin
                   pr_t := -16;
                   pr_c := -11;
                 end;
          end;
        end;
  end;
end;



procedure ProtectTVfromDVB8_ImageNplus9(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  case wanted_std of
    csPAL:
        begin
          case wanted_sys of
            tvB1: begin
                   pr_t := -19;
                   pr_c := -15;
                 end;
            tvG: begin
                   pr_t := -19;
                   pr_c := -15;
                 end;
          end;
        end;
    csSECAM:
        begin
          case wanted_sys of
            tvD: begin
                   pr_t := -16;
                   pr_c := -11;
                 end;
            tvK: begin
                   pr_t := -16;
                   pr_c := -11;
                 end;
          end;
        end;
  end;
end;



procedure ProtectTVfromDVB8_ImageNminus9(wanted_std: TBCTvStandards; wanted_sys: TBCTvSystems; var pr_c, pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
  case wanted_std of
    csSECAM:
        begin
          case wanted_sys of
            tvL: begin
                   pr_t := -25;
                   pr_c := -22;
                 end;
          end;
        end;
  end;
end;



function ProtectLowerAdjacentUHF_L(unwanted_sys: TBCTvSystems): integer;
begin
  case unwanted_sys of
    tvG: result := -9;
    tvH: result := -9;
    tvI: result := 0;
    tvD: result := -12;
    tvD1: result := -12;
    tvK: result := -12;
    tvK1: result := -12;
    tvL: result := -8;
  else
    result := NO_INTERFERENCE;
  end;
end;



function ProtectLowerAdjacentUHF_I(unwanted_sys: TBCTvSystems): integer;
begin
  case unwanted_sys of
    tvG: result := -9;
    tvH: result := -9;
    tvI: result := -9;
    tvD: result := 13;
    tvD1: result := 13;
    tvK: result := 13;
    tvK1: result := 13;
    tvL: result := 17;
  else
    result := NO_INTERFERENCE;
  end;
end;



function ProtectLowerAdjacentUHF(wanted_sys, unwanted_sys: TBCTvSystems): integer;
begin
  case wanted_sys of
    tvG: result := ProtectLowerAdjacentUHF_G(unwanted_sys);
    tvH: result := ProtectLowerAdjacentUHF_H(unwanted_sys);
    tvI: result := ProtectLowerAdjacentUHF_I(unwanted_sys);
    tvD: result := ProtectLowerAdjacentUHF_D_D1_K(unwanted_sys);
    tvD1: result := ProtectLowerAdjacentUHF_D_D1_K(unwanted_sys);
    tvK: result := ProtectLowerAdjacentUHF_D_D1_K(unwanted_sys);
    tvK1: result := ProtectLowerAdjacentUHF_K1(unwanted_sys);
    tvL: result := ProtectLowerAdjacentUHF_L(unwanted_sys);
  else
    result := NO_INTERFERENCE;
  end;

end;



function ProtectLowerAdjacent(band: TBand; wanted_sys, unwanted_sys: TBCTvSystems; sound: TBCSound): integer;
begin
  case band of
    VHF: result := ProtectLowerAdjacentVHF(sound);
    UHF: result := ProtectLowerAdjacentUHF(wanted_sys, unwanted_sys);
  else
    result := NO_INTERFERENCE;
  end;

end;



function ProtectUpperAdjacent(wanted_sys: TBCTvSystems): integer;
begin
  if (wanted_sys = tvD) or (wanted_sys = tvK) then result := -6 else result := -12;
end;



procedure Protect_FM_FM_75(tx1, tx2: PTxParams; var pr_c, pr_t: double);
var delta_f: double;
begin
{
  delta_f в герцах
}
  delta_f := 1e+6 * Abs(tx1.S_carrier - tx2.S_carrier);

  if tx1.Mono_Stereo = 'M' then
  begin
    pr_c := ProtectFMFromMonoFM_75_C(delta_f, tx1.Snd);
    pr_t := ProtectFMFromMonoFM_75_T(delta_f, tx1.Snd);
  end else
  begin
    pr_c := ProtectFMFromStereoFM_75_C(delta_f, tx1.Snd);
    pr_t := ProtectFMFromStereoFM_75_T(delta_f, tx1.Snd);
  end;
end;



function ProtectFMFromMonoFM_75_C(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_MONO_75_C: array[0..16] of double =
   (36,  31,  24,  16,  12, 9.5,  8,  7,  6, 4.5,  2,  -2,  -7, -11.5,  -15, -17.5,  -20);
   _FM_AM_MONO_75_C: array[0..16] of double =
   (36,  31,  24,  16,  12, 9.5,  8,  7,  6, 4.5,  2,  -2,  -7,    -7,   -7,    -7,   -7);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_MONO_75_C[i])
       else _c.AddXY(i*25000, _FM_AM_MONO_75_C[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else result := r;
end;



function ProtectFMFromMonoFM_75_T(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_MONO_75_T: array[0..16] of double =
   (28,  27,  22,  16,  12, 9.5,  8,  7,  6, 4.5,  2,  -2,  -7, -11.5,  -15, -17.5,  -20);
   _FM_AM_MONO_75_T: array[0..16] of double =
   (28,  27,  22,  16,  12, 9.5,  8,  7,  6, 4.5,  2,  -2,  -7,    -7,   -7,    -7,   -7);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_MONO_75_T[i])
       else _c.AddXY(i*25000, _FM_AM_MONO_75_T[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else result := r;
end;


function ProtectFMFromStereoFM_75_C(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_STEREO_75_C: array[0..16] of double =
   (45,  51,  51,  45,  33, 24,  18,  11,  7, 4.5,  2,  -2,  -7, -11.5,  -15, -17.5,  -20);
   _FM_AM_STEREO_75_C: array[0..16] of double =
   (45,  51,  51,  45,  33, 24,  18,  11,  7, 4.5,  2,  -2,  -7,    -7,   -7,    -7,   -7);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_STEREO_75_C[i])
       else _c.AddXY(i*25000, _FM_AM_STEREO_75_C[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else
  result := r;
end;



function ProtectFMFromStereoFM_75_T(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_STEREO_75_T: array[0..16] of double =
   (37,  43,  43,  37,  25, 18,  14,  10,  7, 4.5,  2,  -2,  -7, -11.5,  -15, -17.5,  -20);
   _FM_AM_STEREO_75_T: array[0..16] of double =
   (37,  43,  43,  37,  25, 18,  14,  10,  7, 4.5,  2,  -2,  -7,    -7,   -7,    -7,   -7);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_STEREO_75_T[i])
       else _c.AddXY(i*25000, _FM_AM_STEREO_75_T[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else
  result := r;
end;



{
  Защ. отнош. ЦТВ от АТВ по основному каналу. Значения действительны для
  DVB7 DVB8, как для тропосферной, так и для длительной помехи.
}
function ProtectDVBfromTV_Co(wanted_dvbsys: TBCDVBSystem): integer;
begin
  case wanted_dvbsys of
    dsA1: result := -12;
    dsA2: result := -8;
    dsA3: result := -4;
    dsA5: result := 3;
    dsA7: result := 9;

    dsB1: result := -8;
    dsB2: result := -3;
    dsB3: result := 3;
    dsB5: result := 9;
    dsB7: result := 16;

    dsC1: result := -3;
    dsC2: result := 3;
    dsC3: result := 9;
    dsC5: result := 15;
    dsC7: result := 20;
  else result := NO_INTERFERENCE
  end;
end;



{
  Расчет защитных отношений.
  tx0 - защищаемый
  tx1 - мешающий
  pr_c - защ. отношения для длительной помехи (дБ)
  pr_t - защ. отношения для тропосферной помехи (дБ)
}
procedure GetProtectRatio(const tx0, tx1: ILISBCTx; var pr_c, pr_t: Double);
var wanted, unwanted: TTxParams;
    ms: integer;
    off: integer;
    c,t: double;
begin
  tx0.Get_video_carrier(wanted.f);
  tx0.Get_systemcolor(wanted.std);
  tx0.Get_systemcast(wanted.tx_type);
  tx0.Get_typeoffset(wanted.offset_type);
  tx0.Get_video_carrier(wanted.v_carrier);
  tx0.Get_video_offset_herz(off);
  tx0.Get_video_offset_line(wanted.v_offset_line);
  tx0.Get_sound_carrier_primary(wanted.s_carrier);
  tx0.Get_typesystem(wanted.tvsys);
  tx0.Get_monostereo_primary(ms);
  if ms = 0 then wanted.mono_stereo := 'M' else wanted.mono_stereo := 'S';
  case wanted.tx_type of
    ttTV: wanted.snd := sndAM;
    ttFM: wanted.snd := sndFM;
  else
    wanted.snd := sndFM;
  end;

//  tx0.Get_channel_id(wanted.ch);

  tx1.Get_video_carrier(unwanted.f);
  tx1.Get_systemcolor(unwanted.std);
  tx1.Get_systemcast(unwanted.tx_type);
  tx1.Get_typeoffset(unwanted.offset_type);
  tx1.Get_video_carrier(unwanted.v_carrier);
  tx1.Get_video_offset_herz(off);
  tx1.Get_video_offset_line(unwanted.v_offset_line);
  tx1.Get_sound_carrier_primary(unwanted.s_carrier);
  tx1.Get_typesystem(unwanted.tvsys);
  tx1.Get_monostereo_primary(ms);
  if ms = 0 then unwanted.mono_stereo := 'M' else unwanted.mono_stereo := 'S';
  case unwanted.tx_type of
    ttTV: unwanted.snd := sndAM;
    ttFM: unwanted.snd := sndFM;
  else
    unwanted.snd := sndFM;
  end;
//  tx1.Get_channel_id(unwanted.ch);

  GetProtectRatio2(@wanted, @unwanted, c, t);
  pr_c := c;
  pr_t := t;

end;



{
  Расчет защитных отношений для перекрывающихся каналов
  см. ITU-R BT.655 par. 4.
}
procedure ProtectTVOverlapping(tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
var offset: integer;
    isBGDK: boolean;
    isHIK1L: boolean;
    isPAL: boolean;
    isSECAM: boolean;
    isNO: boolean;
    isPO: boolean;
    isOffset0: boolean;
    isOffset1: boolean;
    isOffset2: boolean;
    isOffset3: boolean;
    isOffset4: boolean;
    isOffset5: boolean;
    isOffset6: boolean;
    isOffset7: boolean;
    isOffset8: boolean;
    isOffset9: boolean;
    isOffset10: boolean;
    isOffset11: boolean;
    isOffset12: boolean;
    pr_curve_c: TPROverlappingCurve;
    pr_curve_t: TPROverlappingCurve;
    df_curve: TPROverlappingCurve;
    df, delta_pr: double;
begin
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;

  df := tx2.f - tx1.f;

  if not ((df >= -2) and (df <= 6)) then Exit;

  offset := tx2.v_offset_line - tx1.v_offset_line;

  isOffset0 := (offset = 0);
  isOffset1 := (offset = 1);
  isOffset2 := (offset = 2);
  isOffset3 := (offset = 3);
  isOffset4 := (offset = 4);
  isOffset5 := (offset = 5);
  isOffset6 := (offset = 6);
  isOffset7 := (offset = 7);
  isOffset8 := (offset = 8);
  isOffset9 := (offset = 9);
  isOffset10 := (offset = 10);
  isOffset11 := (offset = 11);
  isOffset12 := (offset = 12);

  if (tx1.offset_type = otPrecision) and (tx2.offset_type = otPrecision) then isPO := true else isPO := false;
  isNO := not isPO;
{
  Проверяем тип системы, если система не ПАЛ и не СЕКАМ - выходим без помех:
  pr_c := NO_INTERFERENCE;
  pr_t := NO_INTERFERENCE;
}
  isPAL := false;
  isSECAM := false;
  case tx1.std of
    csPAL: isPAL := true;
    csSECAM: isSECAM := true;
  else
    Exit;
  end;

  isBGDK := false;
  isHIK1L := false;

  case tx1.tvsys of
    tvB: isBGDK := true;
    tvG: isBGDK := true;
    tvD: isBGDK := true;
    tvK: isBGDK := true;
    tvH: isHIK1L := true;
    tvI: isHIK1L := true;
    tvK1: isHIK1L := true;
    tvL: isHIK1L := true;
  else
    Exit;
  end;

{
  Выбираем кривые по умолчанию
}
   pr_curve_c := PR_Curve_C_NO_0_SECAM_BGDK;
   pr_curve_t := PR_Curve_C_NO_0_SECAM_BGDK;

{
  Теперь выбираем нужную кривую, соответствующую заданной комбинации параметров.
  Если кривая не найдется, будут использоваться кривые по умолчанию
}
////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset0 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_0_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_0_PAL_HIK1L;
           end;
     if isPO and isOffset0 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_0_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_0_PAL_HIK1L;
           end;
     if isNO and isOffset0 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_0_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_0_PAL_BGDK;
           end;
     if isPO and isOffset0 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_0_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_0_PAL_BGDK;
           end;
     if isNO and isOffset0 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_0_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_0_SECAM_HIK1L;
           end;
     if isPO and isOffset0 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_0_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_0_SECAM_HIK1L;
           end;
     if isNO and isOffset0 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_0_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_0_SECAM_BGDK;
           end;
     if isPO and isOffset0 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_0_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_0_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset1 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_1_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_1_PAL_HIK1L;
           end;
     if isPO and isOffset1 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_1_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_1_PAL_HIK1L;
           end;
     if isNO and isOffset1 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_1_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_1_PAL_BGDK;
           end;
     if isPO and isOffset1 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_1_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_1_PAL_BGDK;
           end;
     if isNO and isOffset1 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_1_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_1_SECAM_HIK1L;
           end;
     if isPO and isOffset1 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_1_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_1_SECAM_HIK1L;
           end;
     if isNO and isOffset1 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_1_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_1_SECAM_BGDK;
           end;
     if isPO and isOffset1 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_1_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_1_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset2 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_2_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_2_PAL_HIK1L;
           end;
     if isPO and isOffset2 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_2_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_2_PAL_HIK1L;
           end;
     if isNO and isOffset2 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_2_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_2_PAL_BGDK;
           end;
     if isPO and isOffset2 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_2_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_2_PAL_BGDK;
           end;
     if isNO and isOffset2 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_2_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_2_SECAM_HIK1L;
           end;
     if isPO and isOffset2 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_2_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_2_SECAM_HIK1L;
           end;
     if isNO and isOffset2 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_2_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_2_SECAM_BGDK;
           end;
     if isPO and isOffset2 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_2_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_2_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset3 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_3_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_3_PAL_HIK1L;
           end;
     if isPO and isOffset3 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_3_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_3_PAL_HIK1L;
           end;
     if isNO and isOffset3 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_3_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_3_PAL_BGDK;
           end;
     if isPO and isOffset3 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_3_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_3_PAL_BGDK;
           end;
     if isNO and isOffset3 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_3_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_3_SECAM_HIK1L;
           end;
     if isPO and isOffset3 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_3_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_3_SECAM_HIK1L;
           end;
     if isNO and isOffset3 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_3_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_3_SECAM_BGDK;
           end;
     if isPO and isOffset3 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_3_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_3_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset4 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_4_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_4_PAL_HIK1L;
           end;
     if isPO and isOffset4 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_4_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_4_PAL_HIK1L;
           end;
     if isNO and isOffset4 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_4_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_4_PAL_BGDK;
           end;
     if isPO and isOffset4 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_4_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_4_PAL_BGDK;
           end;
     if isNO and isOffset4 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_4_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_4_SECAM_HIK1L;
           end;
     if isPO and isOffset4 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_4_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_4_SECAM_HIK1L;
           end;
     if isNO and isOffset4 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_4_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_4_SECAM_BGDK;
           end;
     if isPO and isOffset4 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_4_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_4_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset5 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_5_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_5_PAL_HIK1L;
           end;
     if isPO and isOffset5 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_5_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_5_PAL_HIK1L;
           end;
     if isNO and isOffset5 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_5_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_5_PAL_BGDK;
           end;
     if isPO and isOffset5 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_5_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_5_PAL_BGDK;
           end;
     if isNO and isOffset5 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_5_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_5_SECAM_HIK1L;
           end;
     if isPO and isOffset5 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_5_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_5_SECAM_HIK1L;
           end;
     if isNO and isOffset5 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_5_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_5_SECAM_BGDK;
           end;
     if isPO and isOffset5 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_5_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_5_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset6 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_6_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_6_PAL_HIK1L;
           end;
     if isPO and isOffset6 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_6_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_6_PAL_HIK1L;
           end;
     if isNO and isOffset6 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_6_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_6_PAL_BGDK;
           end;
     if isPO and isOffset6 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_6_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_6_PAL_BGDK;
           end;
     if isNO and isOffset6 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_6_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_6_SECAM_HIK1L;
           end;
     if isPO and isOffset6 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_6_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_6_SECAM_HIK1L;
           end;
     if isNO and isOffset6 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_6_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_6_SECAM_BGDK;
           end;
     if isPO and isOffset6 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_6_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_6_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset7 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_7_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_7_PAL_HIK1L;
           end;
     if isPO and isOffset7 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_7_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_7_PAL_HIK1L;
           end;
     if isNO and isOffset7 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_7_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_7_PAL_BGDK;
           end;
     if isPO and isOffset7 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_7_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_7_PAL_BGDK;
           end;
     if isNO and isOffset7 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_7_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_7_SECAM_HIK1L;
           end;
     if isPO and isOffset7 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_7_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_7_SECAM_HIK1L;
           end;
     if isNO and isOffset7 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_7_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_7_SECAM_BGDK;
           end;
     if isPO and isOffset7 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_7_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_7_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset8 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_8_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_8_PAL_HIK1L;
           end;
     if isPO and isOffset8 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_8_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_8_PAL_HIK1L;
           end;
     if isNO and isOffset8 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_8_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_8_PAL_BGDK;
           end;
     if isPO and isOffset8 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_8_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_8_PAL_BGDK;
           end;
     if isNO and isOffset8 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_8_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_8_SECAM_HIK1L;
           end;
     if isPO and isOffset8 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_8_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_8_SECAM_HIK1L;
           end;
     if isNO and isOffset8 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_8_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_8_SECAM_BGDK;
           end;
     if isPO and isOffset8 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_8_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_8_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset9 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_9_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_9_PAL_HIK1L;
           end;
     if isPO and isOffset9 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_9_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_9_PAL_HIK1L;
           end;
     if isNO and isOffset9 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_9_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_9_PAL_BGDK;
           end;
     if isPO and isOffset9 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_9_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_9_PAL_BGDK;
           end;
     if isNO and isOffset9 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_9_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_9_SECAM_HIK1L;
           end;
     if isPO and isOffset9 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_9_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_9_SECAM_HIK1L;
           end;
     if isNO and isOffset9 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_9_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_9_SECAM_BGDK;
           end;
     if isPO and isOffset9 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_9_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_9_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset10 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_10_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_10_PAL_HIK1L;
           end;
     if isPO and isOffset10 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_10_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_10_PAL_HIK1L;
           end;
     if isNO and isOffset10 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_10_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_10_PAL_BGDK;
           end;
     if isPO and isOffset10 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_10_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_10_PAL_BGDK;
           end;
     if isNO and isOffset10 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_10_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_10_SECAM_HIK1L;
           end;
     if isPO and isOffset10 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_10_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_10_SECAM_HIK1L;
           end;
     if isNO and isOffset10 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_10_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_10_SECAM_BGDK;
           end;
     if isPO and isOffset10 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_10_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_10_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset11 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_11_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_11_PAL_HIK1L;
           end;
     if isPO and isOffset11 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_11_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_11_PAL_HIK1L;
           end;
     if isNO and isOffset11 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_11_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_11_PAL_BGDK;
           end;
     if isPO and isOffset11 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_11_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_11_PAL_BGDK;
           end;
     if isNO and isOffset11 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_11_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_11_SECAM_HIK1L;
           end;
     if isPO and isOffset11 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_11_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_11_SECAM_HIK1L;
           end;
     if isNO and isOffset11 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_11_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_11_SECAM_BGDK;
           end;
     if isPO and isOffset11 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_11_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_11_SECAM_BGDK;
           end;

////////////////////////////////////////////////////////////////////////////
     if isNO and isOffset12 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_12_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_NO_12_PAL_HIK1L;
           end;
     if isPO and isOffset12 and isPAL and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_12_PAL_HIK1L;
             pr_curve_t := PR_Curve_T_PO_12_PAL_HIK1L;
           end;
     if isNO and isOffset12 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_12_PAL_BGDK;
             pr_curve_t := PR_Curve_T_NO_12_PAL_BGDK;
           end;
     if isPO and isOffset12 and isPAL and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_12_PAL_BGDK;
             pr_curve_t := PR_Curve_T_PO_12_PAL_BGDK;
           end;
     if isNO and isOffset12 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_NO_12_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_NO_12_SECAM_HIK1L;
           end;
     if isPO and isOffset12 and isSECAM and isHIK1L then
           begin
             pr_curve_c := PR_Curve_C_PO_12_SECAM_HIK1L;
             pr_curve_t := PR_Curve_T_PO_12_SECAM_HIK1L;
           end;
     if isNO and isOffset12 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_NO_12_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_NO_12_SECAM_BGDK;
           end;
     if isPO and isOffset12 and isSECAM and isBGDK then
           begin
             pr_curve_c := PR_Curve_C_PO_12_SECAM_BGDK;
             pr_curve_t := PR_Curve_T_PO_12_SECAM_BGDK;
           end;

  if ((tx1.tvsys = tvB) or (tx1.tvsys = tvG))
    then df_curve := PR_Curve_Delta_F_BG
    else df_curve := PR_Curve_Delta_F;

  if (tx1.std = csSECAM) and  ((tx1.tvsys = tvD) or (tx1.tvsys = tvK)) then
  begin
    pr_curve_c[8] := pr_curve_c[8] + 8;
    pr_curve_c[9] := pr_curve_c[9] + 8;
    pr_curve_t[8] := pr_curve_t[8] + 5;
    pr_curve_t[9] := pr_curve_t[9] + 5;
  end;

  pr_c := GetCurveValue(df_curve, pr_curve_c, df);
  pr_t := GetCurveValue(df_curve, pr_curve_t, df);

{
  Делаем соотв. поправки к кривым с учетом разных типов модуляции -
  положительная модуляция у системы L, у остальных - отрицательная.
  см. ITU-R BT.655 par.4 Table 10
}
  if tx1.tvsys = tvL
    then if tx2.tvsys = tvL then delta_pr := -2 else delta_pr := -4
    else if tx2.tvsys = tvL then delta_pr := -0 else delta_pr := -2;

  if pr_c <> NO_INTERFERENCE then pr_c := pr_c + delta_pr;
  if pr_t <> NO_INTERFERENCE then pr_t := pr_t + delta_pr;
end;



function ProtectFMFromMonoFM_50_C(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_MONO_50_C: array[0..16] of double =
   (39,  32,  24,  15,  12, 7.5,  6,  2,  -2.5, -3.5,  -6,  -7.5,  -10,  -12,  -15, -17.5,  -20);
   _FM_AM_MONO_50_C: array[0..16] of double =
   (39,  32,  24,  15,  12, 7.5,  6,  2,  -2.5, -3.5,  -6,  -7.5,  -10,  -10,  -10,   -10,  -10);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_MONO_50_C[i])
       else _c.AddXY(i*25000, _FM_AM_MONO_50_C[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else result := r;
end;



function ProtectFMFromMonoFM_50_T(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_MONO_50_T: array[0..16] of double =
   (32,  28,  22,  15,  12, 7.5,  6,  2,  -2.5, -3.5,  -6,  -7.5,  -10, -12,  -15, -17.5,  -20);
   _FM_AM_MONO_50_T: array[0..16] of double =
   (32,  28,  22,  15,  12, 7.5,  6,  2,  -2.5, -3.5,  -6,  -7.5,  -10,  -10, -10, -10,     10);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_MONO_50_T[i])
       else _c.AddXY(i*25000, _FM_AM_MONO_50_T[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else result := r;
end;



function ProtectFMFromStereoFM_50_C(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_STEREO_50_C: array[0..16] of double =
   (49,  53,  51,  45,  33, 25,  18,  12,  7,  5,  2,  0,  -7, -10,  -15, -17.5,  -20);
   _FM_AM_STEREO_50_C: array[0..16] of double =
   (49,  53,  51,  45,  33, 25,  18,  12,  7,  5,  2,  0,  -7, -7,  -7,   -7,  -7);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_STEREO_50_C[i])
       else _c.AddXY(i*25000, _FM_AM_STEREO_50_C[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else result := r;
end;



function ProtectFMFromStereoFM_50_T(delta_f: double; amfm: TBCSound): double;
const
   _FM_FM_STEREO_50_T: array[0..16] of double =
   (41,  45,  43,  37,  25, 18,  14,  11,  7,  5,  2,  0,  -7, -10,  -15, -17.5,  -20);
   _FM_AM_STEREO_50_T: array[0..16] of double =
   (41,  45,  43,  37,  25, 18,  14,  11,  7,  5,  2,  0,  -7,    -7,   -7,    -7,   -7);
var i: integer;
    r: double;
    _c: TCurve;
begin
  _c := TCurve.Create;
  for i := 0 to 16 do
  begin
    if amfm = sndFM
       then _c.AddXY(i*25000, _FM_FM_STEREO_50_T[i])
       else _c.AddXY(i*25000, _FM_AM_STEREO_50_T[i]);
  end;
  r := _c.GetY(delta_f);
  _c.Free;

  if IsNaN(r) then result := NO_INTERFERENCE else result := r;
end;



procedure Protect_FM_FM_50(tx1, tx2: PTxParams; var pr_c, pr_t: double);
var delta_f: double;
begin
{
  delta_f в герцах
}
  delta_f := 1e+6 * Abs(tx1.S_carrier - tx2.S_carrier);

  if tx1.Mono_Stereo = 'M' then
  begin
    pr_c := ProtectFMFromMonoFM_50_C(delta_f, tx1.Snd);
    pr_t := ProtectFMFromMonoFM_50_T(delta_f, tx1.Snd);
  end else
  begin
    pr_c := ProtectFMFromStereoFM_50_C(delta_f, tx1.Snd);
    pr_t := ProtectFMFromStereoFM_50_T(delta_f, tx1.Snd);
  end;
end;


{
  BT-1368-3 Annex 2 par 1.2.2
}
procedure ProtectDVBFromTV_Adjacent_Nminus1 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  if (tx1.tvsys = tvB) and (tx1.std = csPAL) and (tx2.dvbsys = dsA2) then pr_c := -44;
  if (tx1.tvsys = tvI) and (tx1.std = csPAL) and (tx2.dvbsys = dsB1) then pr_c := -43;
  if (tx1.tvsys = tvB) and (tx1.std = csPAL) and (tx2.dvbsys = dsB2) then pr_c := -42;

  if (tx1.tvsys = tvI) and (tx1.std = csPAL) and (tx2.dvbsys = dsC1) then pr_c := -38;
  if (tx1.tvsys = tvB) and (tx1.std = csPAL) and (tx2.dvbsys = dsC2) then pr_c := -35;
  if (tx1.tvsys = tvI) and (tx1.std = csPAL) and (tx2.dvbsys = dsC2) then pr_c := -34;
  if (tx1.tvsys = tvL) and (tx1.std = csSECAM) and (tx2.dvbsys = dsC2) then pr_c := -35;
  pr_t := pr_c;
end;



{
  BT-1368-3 Annex 2 par 1.2.3
}
procedure ProtectDVBFromTV_Adjacent_Nplus1 (tx1, tx2: PTxParams; var pr_c: double; var pr_t: double);
begin
  pr_c := NO_INTERFERENCE;
  case tx2.dvbsys of
    dsA2: pr_c := -47;
    dsB2: pr_c := -43;
    dsC2: pr_c := -38;
  end;
  pr_t := pr_c;
end;


end.
