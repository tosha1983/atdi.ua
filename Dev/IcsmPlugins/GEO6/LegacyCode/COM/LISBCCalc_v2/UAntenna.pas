unit UAntenna;

INTERFACE
uses Math, LISBCTxServer_TLB, UShare;


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

TBand = (VHF, UHF);

var
  _back_lobe_fm_mono: double;
  _back_lobe_fm_stereo: double;
  _back_lobe_tv_band2: double;
  _polar_correct_fm: double;

function GetAntennaDiscrimination_I(azimuth: double): double;
function GetAntennaDiscrimination_III(azimuth: double): double;
function GetAntennaDiscrimination_IV(azimuth: double): double;
{
  Учет апроксимации антенны телевизионного приемника.
  на входе - номер полосы и азимуты на полезн и помех передатчик в градусах
}
function GetAntennaDiscriminationDeg(band: byte; azimuth1, azimuth2: double):double;
function GetAntennaDiscriminationFMDeg(FM_Sys: TBCFMSystem; azimuth1, azimuth2: double):double;



{
  Учет поляризации антенны (только для вертик. и гориз. поляризации)
}
function GetPolarCorrect (wanted_polar, unwanted_polar: TBCPolarization): double;

{
  Учет поляризации антенны для ОВЧ-ЧМ (только для вертик. и гориз. поляризации)
}
function GetPolarCorrectFM (wanted_polar, unwanted_polar: TBCPolarization): double;
function AzimuthSub(azimuth1, azimuth2: double): double;



IMPLEMENTATION


function GetPolarCorrectFM (wanted_polar, unwanted_polar: TBCPolarization): double;
begin
    if (wanted_polar <> unwanted_polar) and (wanted_polar <> plMIX) and (unwanted_polar <> plMIX) then result := _polar_correct_fm else result := 0;
end;



function GetPolarCorrect (wanted_polar, unwanted_polar: TBCPolarization): double;
begin
  if (wanted_polar <> unwanted_polar) and (wanted_polar <> plMIX) and (unwanted_polar <> plMIX) then result := -16 else result := 0;
end;


function AzimuthSub(azimuth1, azimuth2: double): double;
begin
  while azimuth1 < -0 do azimuth1 := azimuth1 + 360;
  while azimuth1 > 360 do azimuth1 := azimuth1 - 360;
  if azimuth1 > 180 then azimuth1 := azimuth1 - 360;

  while azimuth2 < -0 do azimuth2 := azimuth2 + 360;
  while azimuth2 > 360 do azimuth2 := azimuth2 - 360;
  if azimuth2 > 180 then azimuth2 := azimuth2 - 360;

  result := (abs (azimuth1 - azimuth2));
  if result > 180 then result := 360 - result;
end;


function GetAntennaDiscrimination_I(azimuth: double): double;
const
  A1 = 50;
  A2 = 70;
  R1 = 0;
  R2 = -6;
begin
  result := R1;
  if azimuth >= DegToRad(A2) then result := R2;

  if (azimuth > DegToRad(A1)) and (azimuth < DegToRad(A2)) then
  begin
    result := (R2 / DegToRad(A2 - A1)) * azimuth - DegToRad(A1) * (R2 / DegToRad(A2 - A1));
  end;
end;


function GetAntennaDiscrimination_II(azimuth: double): double;
const
  A1 = 50;
  A2 = 70;
  R1 = 0;
var  R2: double;
begin
  R2 := _back_lobe_tv_band2;
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
  R2 = -12;
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
см. также ULISBCCalcCOM.GetEControlPoint.
}
function GetAntennaDiscriminationDeg(band: byte; azimuth1, azimuth2: double):double;
var azimuth: double;
begin
  azimuth := DegToRad(AzimuthSub(azimuth1, azimuth2));

  case band of
    BAND_I: result := GetAntennaDiscrimination_I(azimuth);
    BAND_II: result := GetAntennaDiscrimination_II(azimuth);
    BAND_III: result := GetAntennaDiscrimination_III(azimuth);
    BAND_IV: result := GetAntennaDiscrimination_IV(azimuth);
    BAND_V: result := GetAntennaDiscrimination_IV(azimuth);
  else
    result := 0;
  end;
end;



function GetAntennaDiscrimination_FM_Mono(azimuth: double): double;
const
  A1 = 50;
  A2 = 70;
  R1 = 0;
var
  R2: double;
begin
  R2 := _back_lobe_fm_mono;
  result := R1;
  if azimuth >= DegToRad(A2) then result := R2;

  if (azimuth > DegToRad(A1)) and (azimuth < DegToRad(A2)) then
  begin
    result := (R2 / DegToRad(A2 - A1)) * azimuth - DegToRad(A1) * (R2 / DegToRad(A2 - A1));
  end;
end;



function GetAntennaDiscrimination_FM_Stereo(azimuth: double): double;
const
  A1 = 27;
  A2 = 60;
  R1 = 0;
var   R2: double;
begin
  R2 := _back_lobe_fm_stereo;
  result := R1;
  if azimuth >= DegToRad(A2) then result := R2;

  if (azimuth > DegToRad(A1)) and (azimuth < DegToRad(A2)) then
  begin
    result := (R2 / DegToRad(A2 - A1)) * azimuth - DegToRad(A1) * (R2 / DegToRad(A2 - A1));
  end;
end;



{
Избирательность приемной атенны в зависимости от направления для ОВЧ-ЧМ
см. также ULISBCCalcCOM.GetEControlPoint.
}
function GetAntennaDiscriminationFMDeg(FM_Sys: TBCFMSystem; azimuth1, azimuth2: double): double;
var azimuth: double;
begin
  azimuth := DegToRad(AzimuthSub(azimuth1, azimuth2));
  case FM_Sys of
    fm1: result := GetAntennaDiscrimination_FM_Mono(azimuth);
    fm2: result := GetAntennaDiscrimination_FM_Mono(azimuth);
    fm3: result := GetAntennaDiscrimination_FM_Stereo(azimuth);
    fm4: result := GetAntennaDiscrimination_FM_Stereo(azimuth);
    fm5: result := GetAntennaDiscrimination_FM_Stereo(azimuth);
  else
    result := 0;
  end;
end;



INITIALIZATION
  _back_lobe_fm_mono := -6;
  _back_lobe_fm_stereo := -12;
  _back_lobe_tv_band2 := -12;
  _polar_correct_fm := -10;

end.
