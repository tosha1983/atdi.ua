unit ULISBCCalcCOM;

{$WARN SYMBOL_PLATFORM OFF}

{
  16.10.2003 (0.0.0.22)
     1. Исправлен учет дискриминации приемной антенны в функции GetEControlPoint.
     2. Теперь направление и поляризация не учитываются одновременно (см. ITU-R 419, NOTE 3)
  27.10.2003 (0.0.0.23)
     1. Добавлены защитные отношения для перекрывающихся каналов ТВ (необходимо тестирование)
  xx.xx.2003 (0.0.2.0)
     1. Расчет защитных отношений вынесен в отдельную СОМ-библиотеку, которая
     подключается при инициализации. Реализованы защитные отношения для:
        UPR_TV_TV     ok
        UPR_TV_FM     ok
        UPR_TV_DAB    ok
        UPR_TV_DVB    ok

        UPR_FM_TV     ok
        UPR_FM_FM     ok
        UPR_FM_DAB    ok

        UPR_DVB_TV    ok
        UPR_DVB_DAB   ok (only DVB-C2 8Mhz)
        UPR_DVB_DVB   ok ?

        UPR_DAB_TV    ok
        UPR_DAB_DAB   ok
        UPR_DAB_FM    ok
        UPR_DAB_DVB   ok (only DVB-C2 8Mhz)
  22.12.2003 (0.0.2.1)
     1. Уточнены значения дискриминации антенны для ТВ (модуль UAntenna)
  24.12.2003 (0.0.2.2)
     1. В интерфейс вынесены новые методы:
        function GetMaxRadius(d_initial, azimuth): double;
        function GetMaxRadiusEmin(tx: ILISBCTx, d_initial, azimuth, emin): double;
  08.01.2004 (0.0.2.3)
     1. Уменьшена точность расчета зоны покрытия с 1 м.(!!!!????) до 100 м. (const _STEP_FINAL = 0.1)
  09.01.2004 (0.0.2.4)
     1. Некоторая оптимизация расчета зон
     2. Вместо методов CalcInterf_Wanted и CalcInterf_Unwanted добавлен
        один метод < CalcDuelInterf > который для всех
        передатчиков в выборке рассчитывает:
          1. дуэльные помехи НАМ;
          2. дуэльные помехи ОТ НАС;
          3. тип помехи НАМ;
          4. тип помехи ОТ НАС;
          5. расстояние до основного передатчика;
          6. азимут с основного передатчика на мешающий;
          7. величину перекрытия теоретических зон основного и
             мешающего передатчика, км. (если <= 0, то зоны не перекрываются,
             если > 0, зоны перекрываются).
     3. Минимальный радиус зоны принят равным 1 км. ( const _MIN_DISTANCE = 1).
        Это ограничение рекоммендации P.1546.
  20.01.2004 (0.0.2.9)
     1. В методе CalcDuelInterf азимут на мешающий передатчик не записывался
        в список передатчиков. Исправлено.
  26.01.2004 (0.0.2.10-0.0.2.12)
     1. Исправлена ошибка: в функции TCoLISBCCalc.GetE() при записи
        в лог-файл неправильно вычислялся азимут, из-за чего в
        файл записывалось неверное значение эфф. высоты. На расчеты это не влияет.
     2. Расширено отладочное логирование.
     3. Отладочная информация теперь выводится в форматированном виде (HTML).
        Переделаны методы SetLogFileName, AddMessage и AddMessage2. Добавлен
        новый метод SetMessageColor.
     4. Тропосферная помеха считается для 10% времени если:
          Wanted передатчик - АТВ в нижнем диапазоне (1-12 канал)
          Wanted передатчик - ФМ - моно.
     5. В отладку координаты теперь выводятся в нормальном виде- ГГ.ММ.СС
  29.01.2004 (0.0.2.13)
     1. Добавлен метод CalcDuel3, возвращающий всякую-разную информацию по дуэли,
        как-то напряженности поля для четырех дуэльных точек, типы помех и
        соответствующие защитные отношения т.п.
  30.01.2004 (0.0.2.14)
     1. Наконец-то заработала поддержка событий. Теперь есть событие OnProgress,
        сообщающее текущее состояние расчетов в %.
     2. Дискриминация приемной антенны учитывается и для цифрового ТВ.
  06.02.2004 (0.0.2.15-0.0.2.18)
     1. Добавлен новый метод для расчета дуэльных помех. Теперь их два:
         - быстрый (считает помехи в только точках установки дуэльных передатчиков)
         - медленный (считает помехи на границе теоретических зон дуэльных перед.)
     2. В структуре cp_result: PControlPointResult в поле a_discr передается или
        развязка по направленности или развязка по поляризации - одно из двух,
        соответственно поле a_polar вообще не используется.
  11.02.2004 (0.0.2.19)
     1. Некоторые параметры для расчетов (пока что только Емин для ЦТВ) грузятся
        из реестра в методе Init.

  24.02.2004 (0.0.2.20)
     1. Исправлена ошибка - в методе CalcDuelInterfSlow не выводился азимут.

  14.09.2004 (0.0.2.27)
     1. Параметры направленности (и поляр.) антенны для ОВЧ-ЧМ и ТВ (диапазон 2) вынесены в реестр.
     2. Есть исправления в ЗО (библиотека LISBCProtect.dll) для АТВ - АРМ

  10.01.2005 (0.0.3.1)
     1. Основное изменение - теперь зоны возвращаются в виде SafeArrays

  27.01.2005
     1. Если в параметрах рельефа используется только процент моря, тогда
        в процедурах расчета напряженности типа GetE.... можно задавать
        более грубый шаг рельефа, равный ( ДИСТАНЦИЯ / 100 ). 

  28.09.2006
    1. Исправлен неправильный расчет дискриминации антенны FM в GetEControlPoint
    2. Исправлен GetMaxDistancePrec_Quick (в некоторых случаях возвращался результат
    1 км, хотя зона нормальная)


}

interface

uses
  Windows, ActiveX, AxCtrls, Classes, ComObj, LISBCCalc_TLB, Math, SysUtils,
  LISBCTxServer_TLB, UAntenna, RSAGeography_TLB, LISPropagation_TLB, LISBCProtect_TLB,
  UShare, IniFiles, StdVcl, Dialogs, Controls, Graphics, G1, LISBCCoordDist_TLB,
  LISProgress_TLB, Variants, UReferenceNetwork, UZRoutine, UEminDialog, TCALib, uOtherParams;

const
{
  константа для функций GetMaxDistance...
  задает шаг итераций по расстоянию.
}
  _FILE_VERSION = 'TXLIST001';
  _EARTH_RADIUS = 6371.032;    // средний радиус Земли (км)
  _PREC = 0.01;       // точность расчетов по напряженности поля для метода упрощенного перемножения
  _STEP_INIT = 10;  // начальный шаг при расчете зон
  _LOCATION_PROBABILITY = 70;
  _MIN_DISTANCE = 1;  // минимальная допустимая дистанция (км.) для радиуса зоны
  _RX_ANTENNA_HEIGHT = 10;
  _DIST_PREC = 0.01; // в методе GetEControlPoint допуски для сравения координат контр.точки и коорд. Тх
  _DEBUG_FILE_NAME = 'CP_DEBUG.HTML';
  _STEP_CALC_ALLOT = 10;
  _USE_NEW_RELIEF_CALC = TRUE;


type
  T36ValuesArray = array[0..35] of double;
  P36ValuesArray = ^T36ValuesArray;
  TTxArray = T36ValuesArray;
  TMyPointDuelResultArray = array[1..4] of TPointDuelResult;
  PMyPointDuelResultArray = ^TPointDuelResultArray;
  TControlPointResult = packed record
    e_int: double; // напряженность поля от меш. перед. (без ЗО и дискриминации ант.)
    int_type: TBCSInterferenceType;
    a_pr: double;
    a_discr: double;
    a_polar: double;
  end;

  PControlPointResult = ^TControlPointResult;
  TCoordDistResult = packed record
    d: integer;
    az: integer;
    seaperc: byte;
    heff: integer;
    erp: double;
  end;

  PCoordDistResult = ^TCoordDistResult;


  TFreeSpacePropag = class(TTypedComObject, IPropagation)
  protected
    function GetFieldStrength(const Tx: IUnknown; var Path: TRSAGeoPathResults; Perc: TPercentage; out E: Double): HResult; stdcall;
    function Init: HResult; stdcall;
  end;


  TCoLISBCCalc = class(TAutoObject, IConnectionPointContainer, ILISBCCalc, IParams, IEminCalc, ISFNCalc, ICoordZone)
  private
/////////////////////////////////////////////////////////////
    FConnectionPoints: TConnectionPoints;
    FConnectionPoint: TConnectionPoint;
    FEvents: ILISBCCalcEvents;
    { note: FEvents maintains a *single* event sink. For access to more
      than one event sink, use FConnectionPoint.SinkList, and iterate
      through the list of sinks. }
/////////////////////////////////////////////////////////////
     _message_color_str: string;
     _message_color: TColor;
     _cd: ICoordDist;
     _relief: IRSAGeoPath;
     _relief_param: TRSAPathParams;
     _lis_progress: ILISProgress;

     _propag: IPropagation;
     _protect: IProtectRatio;
     _spherics: IRSASpherics;
     _txlist: ILISBCTxList;
     _debug: boolean;
     _logfile: TFileStream;
     _calc_method: TCalcMethod;
     _cover_probability: double;
     _messages: TStringList;
     _quick_calc_max_dist : boolean;

     _emin_dvb_200: double;
     _emin_dvb_500: double;
     _emin_dvb_700: double;


     _STEP_FINAL: double;  // точность расчета зоны покрытия (км.)
     _DISTANCE_STEP: double; // // точность расчета идеальной зоны покрытия (км.)


     _coord_dist_ini_file: string;
     _dvb_antenna_discrimination: boolean;
     _rxMode: TBcRxMode;
     _quick_calc_duel_interf: boolean;
     _use_morphology_only: boolean;
     _request_for_coord_dist: boolean;
     _emin_for_allotment: double;

     _prop: IPropagation;

     _coordFieldStrength: double;

    procedure SetMessageColor(color: TColor);
    function GetAzimuthDeg(lon1, lat1, lon2, lat2: double): double;
    function GetEControlPoint(wanted, unwanted: ILISBCTx; lon, lat: double; cp_result: PControlPointResult): double;
    function GetE_Azimuth(tx: ILISBCTx; a, d: double; perc: TPercentage): double;
    procedure GetE_t50_t1x(tx: ILISBCTx; lon, lat: double; perc: TPercentage; var e50, e1: double);
    function GetSumEControlPoint(lon, lat: double): double;
    function GetEusableChester(lon, lat: double): double;
    function GetMaxDistanceWithOutInterferences(tx: ILISBCTx; d, azimuth: double): double;
    function GetMaxDistanceEmin_GE06(tx: ILISBCTx; azimuth, emin: double): double;
    function GetMaxDistanceEmin(tx: ILISBCTx; d, azimuth, emin: double): double;
    function GetMaxDistancePrec(d, azimuth, step_init, step_final: double; tx_noise_limited: ILISBCTx): double;
    function GetMaxDistancePrec_Quick(d, azimuth, step_init, step_final: double; tx_noise_limited: ILISBCTx): double;
    function GetMaxDistance(d, azimuth: Double): Double;
    function GetArea(zone: PSafeArray): double;
    procedure GetMaxVariation(zone1, zone2: PSafeArray; var val: double;
      var dir: integer);
    function GetSS(tx0, tx1: ILISBCTx): double;
    function GetMinSS: double;
    procedure AddMessage(s: string);
    function GetEusableSimplif(lon, lat, probab: double): double;
    function GetEusablePowerSum(lon, lat: double): double;
    function GetDistanceKm(lon1, lat1, lon2, lat2: double): double;
    procedure CalcDuelInterfQuick;
    procedure GetNextCoordDeg(azimuth, distance: double; var lon, lat: double);
//    function CheckESimplif(azimuth, d: double): boolean;
//    function CheckE(azimuth, d: double): boolean;

    function CheckESimplif(azimuth, d: double): double;
    function CheckEPowerSum(azimuth, d: double): double;
    function CheckEChester(azimuth, d: double): double;
    function CheckEmin(tx: ILISBCTx; azimuth, d: double): double;
    function CheckE(azimuth, d: double): double;
//    procedure ShowDebugMessages;
    procedure AddMessage2(s: string);
//    procedure GetEuDeviation(nl_zone, il_zone: PSafeArray; var eu_values: PSafeArray);
    procedure CalcDuelInterfSlow;
    function ZonesOverlapping(tx0, tx1: ILISBCTx; var distance, azimuth: double): boolean;
    procedure LoadParamsFromRegistry;
///    function GetTxCoordDistance(tx: ILISBCTx; azimuth: double): double;
//    function GetTxCoordDistance2(tx: ILISBCTx; azimuth: double; res: PCoordDistResult): double;
    function GetTxCoordDistance3(tx: ILISBCTx; azimuth: double; res: PCoordDistResult; use_max_parameters: boolean): double;
    function GetErpDegradationControlPoint(idx_unwanted: Integer; eu_threshold: Double; lon, lat: double): double;
    function GetSumE_DVB(lon, lat: double): double;
    function EminDVB(tx: ILISBCTx): double;
    function GetE_RN_BorderPoint(rn: TReferenceNetwork; bp_lon, bp_lat, lon, lat: double): double;
    function GetE(tx: ILISBCTx; lon, lat: double; perc: TPercentage): double;
    function GetE_Allot_Subarea(allot: ILISBCDigAllot; rn: TReferenceNetwork; SubAreaID: integer;
                                lon, lat: double): double;
    function GetE_Allot(allot: ILISBCDigAllot; rn: TReferenceNetwork; lon, lat: double): double;
    function GetE_Contour_Allot_Subarea(allot: ILISBCDigAllot; rn: TReferenceNetwork;
                                            SubAreaID: integer; emin: double): PSafeArray;
    function GetEmin_trigger(const tx: ILISBCTx): Double;
    function EminDAB(tx: ILISBCTx): double;
    procedure GetCoordinationZone_OLD(const tx: ILISBCTx; out zone_km: PSafeArray);
    procedure GetCoordinationZone_GE06(const tx: ILISBCTx; out zone_km: PSafeArray);
    function SumPowerdB(pwdb1, pwdb2: double): double;
    procedure GetE_t50_t1x_all_polar(tx: ILISBCTx; lon, lat: double;
      perc: TPercentage; var e50_H, e1_H, e50_V, e1_V: double);
    function GetMaxDistanceEmin_SFN(txlist: ILISBCTxList; center_lon,
      center_lat, azimuth, emin: double): double;
    procedure NewRunOnAzimuth(A: TRSAGeoPoint; Az: TRSAAzimuth; Dist: TRSADistance; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
    procedure NewRunPointToPoint(A: TRSAGeoPoint; B: TRSAGeoPoint; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
    function EminFXM(tx: ILISBCTx): double;

  protected
    function GetMaxRadius(d_initial, azimuth: Double): Double; safecall;
    procedure GetProtectRatio(const tx0, tx1: ILISBCTx; out pr_c, pr_t: Double); safecall;
    procedure SetPropagServer(const Propag: IPropagation); safecall;
    procedure SetReliefServer(const Relief: IRSAGeoPath); safecall;
    procedure Init; safecall;
    procedure GetZone_NoiseLimited(const tx: ILISBCTx; step_deg: double;
      out zone_km: PSafeArray); safecall;
    function GetUsableFieldStrength(lon, lat: Double): Double; safecall;
    procedure GetZone_InterferenceLimited(step_deg: Double;
      out zone_km: PSafeArray); safecall;
    procedure CalcDuel(const tx1, tx2: ILISBCTx; var duel_result: TDuelResult); safecall;
    procedure GetFieldDistribution(const tx: ILISBCTx; zone_number: Shortint; var distribution: TFieldDistribution); safecall;
    procedure CalcDuel2(const tx1, tx2: ILISBCTx; var duel_result: TDuelResult2); safecall;
    function GetFieldDistribution2(const tx: ILISBCTx; lon1, lat1, lon2, lat2, spacing: Double): PSafeArray; safecall;
    function GetFieldDistribution3(const tx: ILISBCTx; a1, a2, da, r1, r2, dr: Double): PSafeArray; safecall;
    procedure SetTxListServer(const txlist: ILISBCTxList); safecall;
    { TODO: Change all instances of type [IProgress] to [ILISBCCalcEvents].}
 { Delphi was not able to update this file to reflect
   The change of the name of your event interface
   because of the presence of instance variables.
   The type library was updated but you must update
   this implementation file by hand. }

    property ConnectionPoints: TConnectionPoints read FConnectionPoints
      implements IConnectionPointContainer;
    procedure EventSinkChanged(const EventSink: IUnknown); override;
    function OffsetSelection(offset_start, offset_finish: Integer): PSafeArray; safecall;
    function WantedTxAnalysis(const wanted_txlist: ILISBCTxList;
      out resultarray: PSafeArray): HResult; stdcall;
    function ERPSelection(erp_start, erp_finish: Integer): PSafeArray; safecall;
    procedure CalcInterf_Unwanted; safecall;
    procedure CalcInterf_Wanted; safecall;
    function GetFieldStrength(const tx: ILISBCTx; lon, lat: Double;
      perc: Integer): Double; safecall;



    procedure SetLogFileName(const filename: WideString); safecall;
    function Get_CalcMethod: TCalcMethod; safecall;
    procedure Set_CalcMethod(Value: TCalcMethod); safecall;
    function Get_CoverageProbability: Double; safecall;
    procedure Set_CoverageProbability(Value: Double); safecall;
    function GetEmin(const tx: ILISBCTx): Double; safecall;
    function GetAntennaDiscrimination(f, angle: Double): Double; safecall;
    function GetMaxRadiusEmin(const tx: ILISBCTx; d_initial, azimuth, emin: Double): Double; safecall;
    procedure CalcDuelInterf; safecall;
    procedure CalcDuel3(const tx0, tx1: ILISBCTx; var pointDuelResult: TPointDuelResultArray); safecall;
    procedure GetCoordinationZone(const tx: ILISBCTx; out zone_km: PSafeArray);
      safecall;
    procedure SetProgressServer(const progress: ILISProgress); safecall;
    procedure GetFieldStrengthControlPoint(const tx_wanted,
      tx_unwanted: ILISBCTx; lon, lat: Double;
      var cp_result: TControlPointCalcResult); safecall;
    procedure GetErpDegradation(idx_unwanted: Integer; eu_threshold: Double;
      out degradation: PSafeArray); safecall;
    procedure GetErpDegradation1(idx_unwanted: Integer; eu_threshold: Double;
      out degradation: PSafeArray); safecall;
    procedure GetErpDegradation2(idx_unwanted: Integer; eu_threshold: Double;
      out degradation: PSafeArray); safecall;
    procedure GetErpDegradation3(idx_unwanted: Integer; eu_threshold: Double;
      out degradation: PSafeArray); safecall;
    function GetSumFieldStrength(lon, lat: Double): Double; safecall;
    function GetEContourAllotSubarea(const allot: ILisBcDigAllot;
      subareaId: Integer; emin: Double): PSafeArray; safecall;
    function GetEAllotSubarea(const allot: ILisBcDigAllot; subareaId: Integer;
      lon, lat: Double): Double; safecall;
    function GetEAllot(const allot: ILisBcDigAllot; lon, lat: Double): Double;
      safecall;
    function GetRNPosition(const allot: ILisBcDigAllot; lon, lat,
      azimuth_deg: Double): ILISBCTxList; safecall;
    function GetRNPositionAtBorderPoint(const allot: ILisBcDigAllot; lon, lat,
      azimuth_deg: Double): ILISBCTxList; safecall;
    procedure GetZone_Emin_GE06(const tx: ILISBCTx; step_deg, emin: Double;
      out zone_km: PSafeArray);
    function GetParam(paramName: Integer): OleVariant; safecall;
    procedure ConfigDialog; safecall;
    procedure LoadConfig(const regPath: WideString); safecall;
    procedure SaveConfig(const regPath: WideString); safecall;
    procedure SetParam(const paramName: WideString; Value: OleVariant);
      safecall;  function IEminCalc.GetEmin = IEminCalc_GetEmin;
    function IEminCalc_GetEmin(freq: Double): Double; safecall;
    procedure GetSfnZone(const txlist: LISBCTxList; var center_lon, center_lat,
      emin: Double; out zone_km: PSafeArray); safecall;
    function CreateRn(allot: ILisBcDigAllot): TReferenceNetwork;
    function getCoordFieldStrength: Double; safecall;
    procedure setCoordFieldStrength(fs: Double); safecall;
   public
     procedure Initialize; override;
     procedure Fire_Progress(var perc: integer);
     procedure Fire_LISProgress(var perc: integer);
  { TODO: Change all instances of type [ILISBCCalcEvents] to [ILISBCCalcCOMEvents].}
 { Delphi was not able to update this file to reflect
   The change of the name of your event interface
   because of the presence of instance variables.
   The type library was updated but you must update
   this implementation file by hand. }
  { TODO: Change all instances of type [ILISBCCalcCOMEvents] to [ILISBCCalcEvents].}
 { Delphi was not able to update this file to reflect
   The change of the name of your event interface
   because of the presence of instance variables.
   The type library was updated but you must update
   this implementation file by hand. }
end;

procedure GetNextCoord(azimuth: double; distance: double; var lon, lat: double);
function GetDistance(lon1, lat1, lon2, lat2: double): double;
function GetAzimuth(lon1, lat1, lon2, lat2: double): double;

function CompareTx(Item1, Item2: Pointer): Integer;
function CompareTx2(Item1, Item2: Pointer): Integer;
function GetDataFromTVA(tva: string; start, width: integer): string;
procedure CreateNewDirectory(dir: string);
function ProbabilityIntegral(x: double): double;
function RnStrToRn(rnstr: string): TBcRn;
function RpcStrToRpc(rpcstr: string): TBcRpc;
//function GetAzimuthDeg(lon1, lat1, lon2, lat2: double): double;
function CorrectFieldStrength(const tx: ILISBCTx): Double;

implementation


uses ComServ, Registry, UPR_DVBT2_Share, UPR_Share;





{
   Расчет максимальной дистанции (радиуса уверенного приема) в заданном
   направлении при отсутствии помех. Параметр d указывает с какой дистанции следует начинать
   итерации при поиске MaxDistance
   Azimuth = (0 .. 360);
}
function TCoLISBCCalc.GetMaxDistanceWithOutInterferences(tx: ILISBCTx; d, azimuth: double): double;
var step_init: double;
    tx_type: TBCTxType;
begin

 tx.Get_systemcast(tx_type);
 if tx_type = ttAllot then
 begin
   result := 90;
   Exit;
 end;

{
  Если последний параметр не NIL, то функция расчитает
  максимальный радиус без учета помех - теоретический радиус
}
  if d <= _DISTANCE_STEP then step_init := _STEP_INIT else step_init := d / 5;

  if _quick_calc_max_dist then
    result := GetMaxDistancePrec_Quick(d, azimuth, step_init, _STEP_FINAL, tx) else
    result := GetMaxDistancePrec(d, azimuth, step_init, _STEP_FINAL, tx);
end;



{
   Расчет максимальной дистанции (радиуса уверенного приема) в заданном
   направлении для заданной минимальной напряж. поля
}
function TCoLISBCCalc.GetMaxDistanceEmin(tx: ILISBCTx; d, azimuth, emin: double): double;
var dd, e1: double;
    k: shortint;
begin
  if _debug then
  begin
    SetMessageColor(clOlive);
    AddMessage('+-+-+-+ Расчет Rмакс по аз. ' + FloatToStr(azimuth) + ' град. для Емин = ' +  FloatToStr(emin) + '+-+-+-+' );
  end;

{
  Выбираем шаг = 0.1 км.
}
  dd := _DISTANCE_STEP;

//  tx.Get_latitude(p_lat);
//  tx.Get_longitude(p_lon);
//  GetNextCoord(azimuth, d, p_lon, p_lat);

//  e1 := GetE(tx, p_lon, p_lat, 50);

  e1 := GetE_azimuth(tx, azimuth, d, 50);

  if e1 >= Emin then k := 1 else k := -1;
  while (d>_DISTANCE_STEP) and ((e1 >= Emin) xor (k = -1)) do
  begin
//    tx.Get_latitude(p_lat);
//    tx.Get_longitude(p_lon);
//    GetNextCoord(azimuth, d, p_lon, p_lat);
//    e1 := GetE(tx, p_lon, p_lat, 50);
    e1 := GetE_azimuth(tx, azimuth, d, 50);
    d := d + k * dd;
  end;
  result := d;

  if _debug then
  begin
    SetMessageColor(clOlive);
    AddMessage('+-+-+-+ ********************** +-+-+-+' );
  end;
end;





function TCoLISBCCalc.GetSumEControlPoint(lon, lat: double): double;
var i,n: integer;
//    j: integer;
    e, ecp_value: double;
    tx0, unwanted: ILISBCTx;
    b: wordbool;
//    it: TBCSInterferenceType;
begin
{
  n - Количество мешающих передатчиков
}
  _txlist.Get_Size(n);
  n := n - 1;
  if (n > 0) then
  begin
    _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope

//    SortTxList(lon, lat);
    e := 0;
    i := 1;   // начинаем отсчет с первого индекса (индекс 0 - планируемый перед. 1,2,3... - мешающие)
//    j := 0;
//    while (j < 20)  and (i < n+1) do
    while (i < n+1) do
    begin
      _txlist.Get_Tx(i, unwanted); unwanted._AddRef; // delphi interface decrements reference when goes off the scope
      _txlist.Get_TxUseInCalc(i, b);
      if b then
      begin
        ecp_value := GetEControlPoint(tx0, unwanted, lon, lat, nil);
        e := e + Power(10,  ecp_value / 10);
//        j := j + 1;
      end;
      unwanted := nil;
      i := i + 1;
    end;

/////    tx0._Release;
    if e > 0 then result := 10 * Log10(e) else result := NO_INTERFERENCE;
  end else result := NO_INTERFERENCE;

end;



function GetDVBCN(dvbsys: TBCDVBSystem): double;
begin
  case dvbsys of
    dsA1: result := 3.1;
    dsA2: result := 4.9;
    dsA3: result := 5.9;
    dsA5: result := 6.9;
    dsA7: result := 7.7;

    dsB1: result := 8.8;
    dsB2: result := 11.1;
    dsB3: result := 12.5;
    dsB5: result := 13.5;
    dsB7: result := 13.9;

    dsC1: result := 14.4;
    dsC2: result := 16.5;
    dsC3: result := 18.0;
    dsC5: result := 19.3;
    dsC7: result := 20.1;
  else
    ErrorMessage('TCoLISBCCalc.GetDVBCN', 'Unknown DVB system');
    result := 20.1;
  end;
end;



function GetAntennaGainDVB(freq: double): double;
const
  _F4 = 500;
  _F5 = 800;

var band: byte;
begin

  band := FreqToBand(freq);
{
  Здесь различия между Chester97 и ITU-R BT.1368 (в диапазоне 3)
  см. ITU-R BT.1368 Annex 2 par 5 Table 39
  Chester97 Annex 1 par. 3.3.2.
}
  case band of
    3: result := 7;
    4: begin
         result := 10;
         result := result + 10 * Log10(freq / _F4);
       end;
    5: begin
         result := 12;
         result := result + 10 * Log10(freq / _F5);
       end;
  else
    ErrorMessage('GetAntennaGainDVB', 'Wrong band for DVB');
    result := 0;
  end;

end;



{
 Расчет мин. напряженности для ЦТВ ITU-R BT.1368
}
{
function GetEmin_DVB2(tx: ILISBCTx): double;
var sys: TBCDVBSystem;
     f: double;
     band: byte;
begin
  tx.Get_dvb_system(sys);
  tx.get_freq_carrier(f);
  band := FreqToBand(f);
  result := 26.7;
  case band of
    3: begin
         case sys of
           dsA1: result := 26.7;
           dsA2: result := 26.7;
           dsA3: result := 26.7;
           dsB1: result := 32.9;
           dsB2: result := 32.9;
           dsB3: result := 32.9;
           dsC1: result := 38.5;
           dsC2: result := 38.5;
           dsC3: result := 38.5;
         end;
       end;
    4: begin
         case sys of
           dsA1: result := 31.8;
           dsA2: result := 31.8;
           dsA3: result := 31.8;
           dsB1: result := 37.6;
           dsB2: result := 37.6;
           dsB3: result := 37.6;
           dsC1: result := 42.6;
           dsC2: result := 42.6;
           dsC3: result := 42.6;
         end;
       end;
    5: begin
         case sys of
           dsA1: result := 35.8;
           dsA2: result := 35.8;
           dsA3: result := 35.8;
           dsB1: result := 41.6;
           dsB2: result := 41.6;
           dsB3: result := 41.6;
           dsC1: result := 46.6;
           dsC2: result := 46.6;
           dsC3: result := 46.6;
         end;
       end;
  else ErrorMessage('GetEminDVB2', 'Wrong Band');
  end;
end;
}



{
  Задает цвет, которым будут выводится отладочные сообщения
}
procedure TCoLISBCCalc.SetMessageColor(color: TColor);
begin
  if color <> _message_color then
  begin
    if _message_color_str <> '' then AddMessage2('</FONT>');
    _message_color := color;
    _message_color_str := '#' + IntToHex(Integer(color), 6);
    AddMessage2('<FONT COLOR=' + _message_color_str + '>');
  end;
end;



{
  Расчет используемой напряженности поля в заданной точке (Честер97 Annex1 p.58)
}
function TCoLISBCCalc.GetEusableChester(lon, lat: double): double;
var i,j,n: integer;
    e, ecp_value: double;
    tx0, unwanted: ILISBCTx;
    emin: double;
    b: wordbool; 
//    it: TBCSInterferenceType;
begin
{
  n - Количество мешающих передатчиков
}
  _txlist.Get_Size(n);
  n := n - 1;
  if (n > 0) then
  begin
    _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope

//    SortTxList(lon, lat);
    e := 0;
    i := 1;   // начинаем отсчет с первого индекса (индекс 0 - планируемый перед. 1,2,3... - мешающие)
    j := 0;
    while (j < 20)  and (i < n+1) do
    begin
      _txlist.Get_Tx(i, unwanted); unwanted._AddRef; // delphi interface decrements reference when goes off the scope
      _txlist.Get_TxUseInCalc(i, b);
//      unwanted.Get_use_for_calc(b);
      if b then
      begin
        ecp_value := GetEControlPoint(tx0, unwanted, lon, lat, nil);
        e := e + Power(10,  ecp_value / 10);
        j := j + 1;
      end;
      i := i + 1;
      unwanted := nil;
    end;

    emin := GetEmin(tx0);
///    tx0._Release;
    result := 10 * Log10(Power(10,  emin / 10) + e);
  end else result := NO_INTERFERENCE;
end;



{
  Расчет защитных отношений.
  tx0 - защищаемый
  tx1 - мешающий
  pr_c - защ. отношения для длительной помехи (дБ)
  pr_t - защ. отношения для тропосферной помехи (дБ)
}
procedure TCoLISBCCalc.GetProtectRatio(const tx0, tx1: ILISBCTx; out pr_c, pr_t: Double);
var
    systemcast1: TBCTxType;
    systemcast2: TBCTxType;
    DVBValue : TBCDVBSystem;
begin
  _protect.GetProtectRatio(tx0, tx1, pr_c, pr_t);

  tx0.Get_systemcast(systemcast1);
  tx1.Get_systemcast(systemcast2);

  //make some corrections - moved from GetECheckPoint()
  
  if (systemcast1 = ttDVB) and (systemcast2 = ttFxm) then begin
    tx0.Get_rxMode(_rxMode);
    tx0.Get_dvb_system(DVBValue);
    case DVBValue of
      dsA1: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 12.5;
                pr_t := pr_t - 12.5;
                end;
            rmMo: begin
                pr_c := pr_c - 7.3;
                pr_t := pr_t - 7.3;
                end;
            rmPi: begin
                pr_c := pr_c - 10.3;
                pr_t := pr_t - 10.3;
                end;
            rmPo: begin
                pr_c := pr_c - 10.3;
                pr_t := pr_t - 10.3;
                end;
        end;
      end;
      dsA2: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 10.5;
                pr_t := pr_t - 10.5;
                end;
            rmMo: begin
                pr_c := pr_c - 5.2;
                pr_t := pr_t - 5.2;
                end;
            rmPi: begin
                pr_c := pr_c - 8.2;
                pr_t := pr_t - 8.2;
                end;
            rmPo: begin
                pr_c := pr_c - 8.2;
                pr_t := pr_t - 8.2;
                end;
            end;
      end;
      dsA3: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 9.3;
                pr_t := pr_t - 9.3;
                end;
            rmMo: begin
                pr_c := pr_c - 3.9;
                pr_t := pr_t - 3.9;
                end;
            rmPi: begin
                pr_c := pr_c - 6.9;
                pr_t := pr_t - 6.9;
                end;
            rmPo: begin
                pr_c := pr_c - 6.9;
                pr_t := pr_t - 6.9;
                end;
        end;
      end;
      dsA5: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 8.1;
                pr_t := pr_t - 8.1;
                end;
            rmMo: begin
                pr_c := pr_c - 2.6;
                pr_t := pr_t - 2.6;
                end;
            rmPi: begin
                pr_c := pr_c - 5.6;
                pr_t := pr_t - 5.6;
                end;
            rmPo: begin
                pr_c := pr_c - 5.6;
                pr_t := pr_t - 5.6;
                end;
        end;
      end;
      dsA7: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 7.1;
                pr_t := pr_t - 7.1;
                end;
            rmMo: begin
                pr_c := pr_c - 1.5;
                pr_t := pr_t - 1.5;
                end;
            rmPi: begin
                pr_c := pr_c - 4.5;
                pr_t := pr_t - 4.5;
                end;
            rmPo: begin
                pr_c := pr_c - 4.5;
                pr_t := pr_t - 4.5;
                end;
        end;
      end;
      dsB1: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 6.8;
                pr_t := pr_t - 6.8;
                end;
            rmMo: begin
                pr_c := pr_c - 1.6;
                pr_t := pr_t - 1.6;
                end;
            rmPi: begin
                pr_c := pr_c - 3.6;
                pr_t := pr_t - 3.6;
                end;
            rmPo: begin
                pr_c := pr_c - 3.6;
                pr_t := pr_t - 3.6;
                end;
        end;
      end;
      dsB2: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 4.3;
                pr_t := pr_t - 4.3;
                end;
            rmMo: begin
                pr_c := pr_c + 1.0;
                pr_t := pr_t + 1.0;
                end;
            rmPi: begin
                pr_c := pr_c - 2.0;
                pr_t := pr_t - 2.0;
                end;
            rmPo: begin
                pr_c := pr_c - 2.0;
                pr_t := pr_t - 2.0;
                end;
        end;
      end;
      dsB3: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 2.7;
                pr_t := pr_t - 2.7;
                end;
            rmMo: begin
                pr_c := pr_c + 2.7;
                pr_t := pr_t + 2.7;
                end;
            rmPi: begin
                pr_c := pr_c - 0.3;
                pr_t := pr_t - 0.3;
                end;
            rmPo: begin
                pr_c := pr_c - 0.3;
                pr_t := pr_t - 0.3;
                end;
        end;
      end;
      dsB5: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 1.5;
                pr_t := pr_t - 1.5;
                end;
            rmMo: begin
                pr_c := pr_c + 4.0;
                pr_t := pr_t + 4.0;
                end;
            rmPi: begin
                pr_c := pr_c + 1.0;
                pr_t := pr_t + 1.0;
                end;
            rmPo: begin
                pr_c := pr_c + 1.0;
                pr_t := pr_t + 1.0;
                end;
        end;
      end;
      dsB7: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 0.9;
                pr_t := pr_t - 0.9;
                end;
            rmMo: begin
                pr_c := pr_c + 4.7;
                pr_t := pr_t + 4.7;
                end;
            rmPi: begin
                pr_c := pr_c + 1.7;
                pr_t := pr_t + 1.7;
                end;
            rmPo: begin
                pr_c := pr_c + 1.7;
                pr_t := pr_t + 1.7;
                end;
        end;
      end;
      dsC1: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c - 1.2;
                pr_t := pr_t - 1.2;
                end;
            rmMo: begin
                pr_c := pr_c + 4.0;
                pr_t := pr_t + 4.0;
                end;
            rmPi: begin
                pr_c := pr_c + 1.0;
                pr_t := pr_t + 1.0;
                end;
            rmPo: begin
                pr_c := pr_c + 1.0;
                pr_t := pr_t + 1.0;
                end;
        end;
      end;
      dsC2: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c + 1.1;
                pr_t := pr_t + 1.1;
                end;
            rmMo: begin
                pr_c := pr_c + 6.4;
                pr_t := pr_t + 6.4;
                end;
            rmPi: begin
                pr_c := pr_c + 3.4;
                pr_t := pr_t + 3.4;
                end;
            rmPo: begin
                pr_c := pr_c + 3.4;
                pr_t := pr_t + 3.4;
                end;
        end;
      end;
      dsC3: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c + 2.8;
                pr_t := pr_t + 2.8;
                end;
            rmMo: begin
                pr_c := pr_c + 8.2;
                pr_t := pr_t + 8.2;
                end;
            rmPi: begin
                pr_c := pr_c + 5.2;
                pr_t := pr_t + 5.2;
                end;
            rmPo: begin
                pr_c := pr_c + 5.2;
                pr_t := pr_t + 5.2;
                end;
        end;
      end;
      dsC5: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c + 4.3;
                pr_t := pr_t + 4.3;
                end;
            rmMo: begin
                pr_c := pr_c + 9.8;
                pr_t := pr_t + 9.8;
                end;
            rmPi: begin
                pr_c := pr_c + 6.8;
                pr_t := pr_t + 6.8;
                end;
            rmPo: begin
                pr_c := pr_c + 6.8;
                pr_t := pr_t + 6.8;
                end;
        end;
      end;
      dsC7: begin
        case _rxMode of
            rmFx: begin
                pr_c := pr_c + 5.3;
                pr_t := pr_t + 5.3;
                end;
            rmMo: begin
                pr_c := pr_c + 10.9;
                pr_t := pr_t + 10.9;
                end;
            rmPi: begin
                pr_c := pr_c + 7.9;
                pr_t := pr_t + 7.9;
                end;
            rmPo: begin
                pr_c := pr_c + 7.9;
                pr_t := pr_t + 7.9;
                end;
            end;
        end;
      end;
  end;

//  result := S_OK;
end;



procedure TCoLISBCCalc.SetPropagServer(const Propag: IPropagation);
begin
  if Propag = nil then
  begin
    _propag := TFreeSpacePropag.Create;
    _propag.Init;
  end else begin
    _propag := Propag;
    _prop := Propag;
  end;
//  result := S_OK;
end;



procedure TCoLISBCCalc.SetReliefServer(const Relief: IRSAGeoPath);
var terrinfo: IRSATerrainInfo;
      p1, p2: TRSAGeoPoint;
      geodata: TRSAGeoPathData;
      path: TRSAGeoPathResults;
begin
  _relief := Relief;
  if _relief <> nil then
  begin
    _relief.Get_TerrainInfo(terrinfo);
    _spherics := terrinfo as IRSASpherics;
{
  Выбираем две точки и снимаем профиль рельефа.
  Проверяем параметры рельефа. Если ни один параметр не используется,
  то рельеф отключаем вообще.
}
    p1.H := 46;
    p1.L := 33;
    p1.Alt := 0;

    p2.H := 45.5;
    p2.L := 33.3;
    p2.Alt := 0;

    geodata.TxHeight := 10;
    geodata.RxHeight := 10;

    OleCheck(_relief.RunPointToPoint(p1, p2, geodata, path));

    if IsNAN(path.HEff)
        and IsNAN(path.TxClearance)
        and IsNAN(path.RxClearance)
        and IsNAN(path.SeaPercent) then _relief := nil;

    if IsNAN(path.HEff)
        and IsNAN(path.TxClearance)
        and IsNAN(path.RxClearance)
        and (not IsNAN(path.SeaPercent)) then _use_morphology_only := true else _use_morphology_only := false;
    _use_morphology_only := false;

    if IsNan(path.HEff) then _relief_param.CalcHEff := false else _relief_param.CalcHEff := true;
    if IsNan(path.TxClearance) then _relief_param.CalcTxClearance := false else _relief_param.CalcTxClearance := true;
    if IsNan(path.RxClearance) then _relief_param.CalcRxClearance := false else _relief_param.CalcRxClearance := true;
    if IsNan(path.SeaPercent) then _relief_param.CalcSeaPercent := false else _relief_param.CalcSeaPercent := true;

  end
//  result := S_OK;
end;



{
  Расчет мешающей напряженности поля (с учетом защитных отношений) создаваемой одним мешающим передатчиком
  в заданной точке
}

function TCoLISBCCalc.GetEControlPoint(wanted, unwanted: ILISBCTx; lon, lat: double; cp_result: PControlPointResult): double;
var pr_c, pr_t, plon, plat: double;
    az_to_wanted, antenna_az: double;
    az_to_unwanted, f: double;
    a_discr, a_polar, e_c, e_t, e_c_, e_t_: double;
    e_c_H, e_t_H, e_c_V, e_t_V: double;
    wpolar, upolar: TBCPolarization;
    _type: TBCTxType;
    id1, id2: integer;
    perc: TPercentage;
    fm_sys: TBCFMSystem;
    sid1, sid2: widestring;

    systemcast1: TBCTxType;
    systemcast2: TBCTxType;
    //mix_correct: double;

    //DVBValue : TBCDVBSystem;

    iAssocIdW, iAssocIdUw: ILisAssocAllotId;
    assocIdW, assocIdUw: widestring;
begin
  wanted.Get_systemcast(systemcast1);
  unwanted.Get_systemcast(systemcast2);
  if (systemcast1 = ttAllot) or (systemcast2 = ttAllot) then
  begin
    result := NO_INTERFERENCE;
    if cp_result <> nil then
    begin
        cp_result^.e_int := NO_INTERFERENCE;
        cp_result^.int_type := itCONT;
        cp_result^.a_pr := NO_INTERFERENCE;
        cp_result^.a_discr := NO_INTERFERENCE;
        cp_result^.a_polar := NO_INTERFERENCE;
    end;
    Exit;
  end;

  GetProtectRatio(wanted, unwanted, pr_c, pr_t);
  if (pr_c = NO_INTERFERENCE) and (pr_t = NO_INTERFERENCE) and (cp_result = nil) then
  begin
    result := NO_INTERFERENCE;
    if cp_result <> nil then
    begin
      cp_result^.e_int := NO_INTERFERENCE;
      cp_result^.int_type := itCONT;
      cp_result^.a_pr := NO_INTERFERENCE;
      cp_result^.a_discr := NO_INTERFERENCE;
      cp_result^.a_polar := NO_INTERFERENCE;
    end;
    if _debug then
    begin
      wanted.Get_id(id1);
      unwanted.Get_id(id2);
      wanted.Get_station_name(sid1);
      unwanted.Get_station_name(sid2);
      SetMessageColor(clBlue);
      AddMessage('============= Расчет напряж. поля помехи =============');
      AddMessage('No Interference: SID1=' + sid1 + ' SID2=' + sid2);
      AddMessage('============= ************************** =============');
    end;
    Exit;
  end;

  perc := prOne;

  unwanted.Get_longitude(plon);
  unwanted.Get_latitude(plat);
  az_to_unwanted := GetAzimuthDeg(lon, lat, plon, plat);

  wanted.Get_longitude(plon);
  wanted.Get_latitude(plat);

{
  Проверяем, если наша контрольная точка находится в месте
  установки защищаемого передатчика, тогда считаем что антенна
  направлена прямо на мешающий, иначе вычисляем честный азимут.
  Допуск по широте и долготе ~0.001 градуса, что примерно соот-
  ветствует 100 метрам.
}
  if (Abs(plon - lon)< 0.00051) and ((Abs(plat - lat)< 0.00051))
    then az_to_wanted := az_to_unwanted
    else az_to_wanted := GetAzimuthDeg(lon, lat, plon, plat);

{
  учитываем дискриминацию приемной антенны по направлению и по поляризации,
  только для аналогового ТВ (и ЦТВ)
}
  wanted.Get_polarization(wpolar);
  unwanted.Get_polarization(upolar);
  wanted.Get_systemcast(_type);
  if (_type = ttTV) or (_type = ttDVB) or (_type = ttDAB) then
  begin
    wanted.Get_video_carrier(f);
    if (_type = ttTV) and (f < 450) then perc := prTen;
    wanted.Get_rxMode(_rxMode);
    if _rxMode = rmFx then
        _dvb_antenna_discrimination := true
    else
        _dvb_antenna_discrimination := false;
    if (_type = ttDVB) and (not _dvb_antenna_discrimination) then
    begin
      a_polar := 0;
      a_discr := 0;
    end else
    begin
      a_polar := GetPolarCorrect(wpolar, upolar);
      if a_polar <> 0 then a_discr := 0 else
      begin
        antenna_az := az_to_wanted - az_to_unwanted;
        if Abs(antenna_az) <= 20
          then a_discr := 0
          else a_discr := UAntenna.GetAntennaDiscriminationDeg(FreqToBand(f), az_to_wanted, az_to_unwanted);
      end;
    end
  end else
  begin
    if _type = ttFM then
    begin
      wanted.Get_fm_system(fm_sys);
      a_polar := GetPolarCorrectFM(wpolar, upolar);
      if a_polar <> 0 then a_discr := 0 else
      begin
        antenna_az := az_to_wanted - az_to_unwanted;
        if Abs(antenna_az) <= 20
          then a_discr := 0
            {
              UAntenna.GetAntennaDiscriminationFM -- Азимут в радианах!
              В отличие от TCoLISBCCalc.GetAntennaDiscrimination, где азим. в градусах.
            }
          else a_discr := UAntenna.GetAntennaDiscriminationFMDeg(fm_sys, az_to_wanted, az_to_unwanted);
      end;
      case fm_sys of
        fm1: perc := prTen;
        fm2: perc := prTen;
        fm3: perc := prOne;
        fm4: perc := prOne;
        fm5: perc := prOne;
      end;
    end else
    begin
      a_discr := 0;
      a_polar := 0;
    end;
  end;

    {
      Для тропосферной помехи берем процент времени = 1 (или 10)
      см. Chester97 Annex 1 - 6.2
          Geneva84 Annex 2 - 2.1.3.1
    }
    {
      c -  коррекция распространения
      для аналогового ТВ равна 0, для ЦТВ - см. Chester97 Annex 1 - 6.2
    }

  GetE_t50_t1x (unwanted, lon, lat, perc, e_c, e_t);

  e_c_ := e_c + pr_c + Min(a_discr, a_polar);
  e_t_ := e_t + pr_t + Min(a_discr, a_polar);

  if upolar = plMIX then
  begin
    GetE_t50_t1x_all_polar (unwanted, lon, lat, perc, e_c_H, e_t_H, e_c_V, e_t_V);
    if _type = ttFM then a_polar := GetPolarCorrectFM(plHOR, plVer) else a_polar := GetPolarCorrect(plHOR, plVer);

    if wpolar = plHOR then
    begin
      e_c_H := e_c_H + pr_c + a_discr;
      e_t_H := e_t_H + pr_t + a_discr;
      e_c_V := e_c_V + pr_c + a_polar;
      e_t_V := e_t_V + pr_t + a_polar;

      e_c_ := SumPowerdB(e_c_H, e_c_V);
      e_t_ := SumPowerdB(e_t_H, e_t_V);
    end;

    if wpolar = plVER then
    begin
      e_c_V := e_c_V + pr_c + a_discr;
      e_t_V := e_t_V + pr_t + a_discr;
      e_c_H := e_c_H + pr_c + a_polar;
      e_t_H := e_t_H + pr_t + a_polar;

      e_c_ := SumPowerdB(e_c_H, e_c_V);
      e_t_ := SumPowerdB(e_t_H, e_t_V);
    end;

    if wpolar = plMIX then
    begin
      e_c_V := e_c_V + pr_c + a_discr;
      e_t_V := e_t_V + pr_t + a_discr;
      e_c_H := e_c_H + pr_c + a_discr;
      e_t_H := e_t_H + pr_t + a_discr;

      e_c_ := SumPowerdB(e_c_H, e_c_V);
      e_t_ := SumPowerdB(e_t_H, e_t_V);
    end;
  end;

  if _debug then
  begin
      wanted.Get_id(id1);
      unwanted.Get_id(id2);
      wanted.Get_station_name(sid1);
      unwanted.Get_station_name(sid2);
      SetMessageColor(clBlue);
      AddMessage('============= Расчет напряж. поля помехи =============');

      AddMessage('SID1=' + sid1 + ' SID2=' + sid2);
      AddMessage('AZwant = ' + IntToStr(Round(az_to_wanted)) + ' deg');
      AddMessage('AZunwa = ' + IntToStr(Round(az_to_unwanted)) + ' deg');

      AddMessage('   Ant = ' + FloatToStr(a_discr) + ' dB');
      AddMessage(' Polar = ' + FloatToStr(a_polar) + ' dB');
      AddMessage(' Econt = ' + FloatToStr(e_c) + ' dBmkV/m');
      AddMessage('PRcont = ' + FloatToStr(pr_c) + ' dB');
      AddMessage(' Etrop = ' + FloatToStr(e_t) + ' dBmkV/m');
      AddMessage('PRtrop = ' + FloatToStr(pr_t) + ' dB');
      AddMessage('============= ************************** =============');
  end;

    OleCheck(wanted.QueryInterface(ILisAssocAllotId, iAssocIdW));
    OleCheck(unwanted.QueryInterface(ILisAssocAllotId, iAssocIdUw));

    iAssocIdW.GetAssAllotId(assocIdW);
    iAssocIdUw.GetAssAllotId(assocIdUw);

  if e_c_ > e_t_ then
  begin
    if ((assocIdW <> '') and (assocIdUw <> '') and (assocIdW = assocIdUw)) then
        result := NO_INTERFERENCE
    else
        result := e_c_;
    if cp_result <> nil then
    begin
      cp_result^.e_int := e_c;
      cp_result^.int_type := itCONT;
      cp_result^.a_pr := pr_c;
      cp_result^.a_polar := NaN;
      if a_polar <> 0 then
        cp_result^.a_discr := a_polar
      else
        cp_result^.a_discr := a_discr;
    end;
  end else
  begin
    if ((assocIdW <> '') and (assocIdUw <> '') and (assocIdW = assocIdUw)) then
        result := NO_INTERFERENCE
    else
        result := e_t_;
    if cp_result <> nil then
    begin
      cp_result^.e_int := e_t;
      cp_result^.int_type := itTROPO;
      cp_result^.a_pr := pr_t;
      cp_result^.a_polar := NaN;
      if a_polar <> 0 then
        cp_result^.a_discr := a_polar
      else
        cp_result^.a_discr := a_discr;
    end;
  end;
end;



{
  расчитывает азимут по направлению от точки (lon1, lat1)
  до точки (lon2, lat2) относительно севера в радианах
                    N
                  -0 0
                    |
         W  -pi/2 --+-- pi/2  E
                    |
                -pi   pi
                    S
}
function GetAzimuth(lon1, lat1, lon2, lat2: double): double;
var lat, lon: double;
    quater: byte;
begin
  lat := lat2-lat1;
  lon := lon2-lon1;
  quater := 1;
  if (lat >= 0) and (lon >= 0) then quater := 1;
  if (lat >= 0) and (lon < 0) then quater := 2;
  if (lat < 0) and (lon < 0) then quater := 3;
  if (lat < 0) and (lon >= 0) then quater := 4;

  lat1:=lat1 * PI / 180;
  lon1:=lon1 * PI / 180;
  lat2:=lat2 * PI / 180;
  lon2:=lon2 * PI / 180;
  lat:=(lat2 - lat1);
  lon:=(lon2 - lon1) * Cos((lat1 + lat2) / 2);

  if Abs(lon) < 1e-4 then
  begin
//    if lat1<=lat2 then result:=PI / 2 else result:=-(PI / 2);
    if lat >= 0 then result := 0 else result := PI;
    Exit;
  end;

  if Abs(lat) < 1e-4 then
  begin
//    if lat1<=lat2 then result:=PI / 2 else result:=-(PI / 2);
    if lon >= 0 then result := PI / 2 else result := -(PI / 2);
    Exit;
  end;
  result:=ArcTan(Tan(lat) / Sin(lon));
  case quater of
    1: result:=PI/2-result;
    2: result:=-(PI/2+result);
    3: result:=-(PI/2+result);
    4: result:=PI/2-result;
  end;
end;





{
  Расчет напряженности поля сразу для разных процентов времени 50% и 1x% (1% или 10%)
}
procedure TCoLISBCCalc.GetE_t50_t1x(tx: ILISBCTx; lon, lat: double; perc: TPercentage; var e50, e1: double);
var plon, plat: double;
    p1, p2: TRSAGeoPoint;
    h: integer;
    geodata: TRSAGeoPathData;
    path: TRSAGeoPathResults;
    id, az: integer;
    f, erp: double;
    lons, lats: TCoordString;
    sid: widestring;
    heff: integer;
    prms: TRSAPathParams;

    fxm_system : integer;
    fxm: ILisBcFxm;
    systemcast: TBCTxType;
begin
  tx.Get_longitude(plon);
  tx.Get_latitude(plat);
  p1.H := plat;
  p1.L := plon;
  p1.Alt := 0;

  p2.H := lat;
  p2.L := lon;
  p2.Alt := 0;

  tx.Get_heightantenna(h);
//  tx.Get_height_eft_max(h);
  geodata.TxHeight := h;
  geodata.RxHeight := _RX_ANTENNA_HEIGHT;

  if (_relief <> nil) and (_use_morphology_only) then
  begin
    _relief.Get_Step(prms.Step);
    prms.CalcHEff := false;
    prms.CalcTxClearance := false;
    prms.CalcRxClearance := false;
    prms.CalcSeaPercent := true;
    prms.Step := Max(_STEP_FINAL, GetDistanceKm(plon, plat, lon, lat) / 100);
    _relief.Set_Params(prms)
  end;


  if _relief = nil then
  begin
    path.Distance := GetDistanceKm(plon, plat, lon, lat);
    path.Azimuth := GetAzimuthDeg(plon, plat, lon, lat);
{
    az := Round(Path.Azimuth/10);
    if az > 35 then az := 0;
    tx.Get_effectheight(az, heff);
}
    tx.get_h_eff(Round(path.Azimuth), heff);
    path.HEff := heff;

    path.TxClearance := NaN;
    path.RxClearance := NaN;
    path.SeaPercent := 0;
  end else
  begin
    if (p1.H = p2.H) and (p1.L = p2.L) then
    begin
      path.Distance := 0;
      path.Azimuth := 0;
      path.HEff := geodata.TxHeight - geodata.RxHeight;
      path.TxClearance := NaN;
      path.RxClearance := NaN;
      path.SeaPercent := NaN;
    end else NewRunPointToPoint(p1, p2, geodata, path);
  end;

  if Path.Distance < _DISTANCE_STEP then Path.Distance := _DISTANCE_STEP;


  tx.Get_systemcast(systemcast);
  if (systemcast = ttFxm) then begin
    tx.QueryInterface(IID_ILisBcFxm, fxm);
    fxm_system := fxm.fxm_system;

    if (fxm_system = osBC) or (fxm_system = osBD) then begin
        try begin
            SetPropagServer(nil);
            _propag.GetFieldStrength(tx, path, 50, e50);
            _propag.GetFieldStrength(tx, path, perc, e1);
        end
        finally
            SetPropagServer(_prop);
        end;
      end else begin
         _propag.GetFieldStrength(tx, path, 50, e50);
         _propag.GetFieldStrength(tx, path, perc, e1);
      end;
  end else begin
    _propag.GetFieldStrength(tx, path, 50, e50);
    _propag.GetFieldStrength(tx, path, perc, e1);
  end;

  e50 := e50 - CorrectFieldStrength(tx);
  e1 := e1 - CorrectFieldStrength(tx);
  if _debug then
  begin
    tx.Get_id(id);
    tx.Get_station_name(sid);
    SetMessageColor(clRed);

    AddMessage('............. Расчет напряженности 50% + 1x% .............');
    AddMessage('  SID: ' + sid);

    tx.get_freq_carrier(f);
    az := Round(Path.Azimuth);
    if az > 354 then az := 354;
    tx.get_erp(az, erp);

    AddMessage('FREQ: ' + FloatToStr(f) + ' Mhz');
    AddMessage(' ERP: ' + FloatToStr(erp) + ' dBkW');

    FloatToCoordStr(p1.L, p1.H, lons, lats);
    AddMessage('LON1: ' + lons);
    AddMessage('LAT1: ' + lats);

    FloatToCoordStr(p2.L, p2.H, lons, lats);
    AddMessage('LON2: ' + lons);
    AddMessage('LAT2: ' + lats);

//    AddMessage('LON1: ' + FloatToStr(p1.L));
//    AddMessage('LAT1: ' + FloatToStr(p1.H));
//    AddMessage('LON2: ' + FloatToStr(p2.L));
//    AddMessage('LAT2: ' + FloatToStr(p2.H));
    AddMessage('   Distance: ' + FloatToStr(Path.Distance) + ' km');
    AddMessage('    Azimuth: ' + FloatToStr(Path.Azimuth) + ' deg');

    if isNaN(Path.HEff) then tx.get_h_eff(az, h);
//    if isNaN(Path.HEff) then tx.get_h_eff(Trunc(Path.Azimuth/10), h);

    AddMessage('       Heff: ' + IntToStr(h) + ' m');
    AddMessage('RxClearance: ' + FloatToStr(Path.RxClearance) + ' deg');
    AddMessage('TxClearance: ' + FloatToStr(Path.TxClearance) + ' deg');
    AddMessage('Sea Percent: ' + FloatToStr(Path.SeaPercent) + ' %');
    AddMessage('Field Stren (50%): ' + FloatToStr(e50) + 'dBmkV/m');
    AddMessage('Field Stren (1x%): ' + FloatToStr(e1) + 'dBmkV/m');
    AddMessage('............. ***************************** .............');
  end;
end;



function TCoLISBCCalc.GetE(tx: ILISBCTx; lon, lat: double; perc: TPercentage): double;
var plon, plat, e: double;
    p1, p2: TRSAGeoPoint;
    h: integer;
    heff: integer;
    geodata: TRSAGeoPathData;
    path: TRSAGeoPathResults;
    id, az, azl: integer;
    f, erp: double;
    lons, lats: TCoordString;
    prms: TRSAPathParams;
//    old_color: TColor;
    systemcast: TBCTxType;
    allot: ILisBcDigAllot;
    polar: TBCPolarization;
    erp_h, erp_v, erp_vh, erp_vl, e_h, e_v: double;
    rn: TReferenceNetwork;

    fxm_system : integer;
    fxm: ILisBcFxm;
begin
    az := 0;
  tx.Get_systemcast(systemcast);
  if systemcast = ttAllot then
  begin
    OleCheck(tx.QueryInterface(ILisBcDigAllot, allot));

    rn := CreateRn(allot);
    try
        result := GetE_Allot(allot, rn, lon, lat);
    finally
        rn.Free;
    end;
    Exit;
  end;

  tx.Get_longitude(plon);
  tx.Get_latitude(plat);
  p1.H := plat;
  p1.L := plon;
  p1.Alt := 0;

  p2.H := lat;
  p2.L := lon;
  p2.Alt := 0;

  tx.Get_heightantenna(h);
//  tx.Get_height_eft_max(h);
  geodata.TxHeight := h;
  geodata.RxHeight := _RX_ANTENNA_HEIGHT;

  if (_relief <> nil) and (_use_morphology_only) then
  begin
    _relief.Get_Step(prms.Step);
    prms.CalcHEff := false;
    prms.CalcTxClearance := false;
    prms.CalcRxClearance := false;
    prms.CalcSeaPercent := true;
    prms.Step := Max(_STEP_FINAL, GetDistanceKm(plon, plat, lon, lat) / 100);
    _relief.Set_Params(prms)
  end;

  try
  if _relief = nil then
  begin
    path.Distance := GetDistanceKm(plon, plat, lon, lat);
    path.Azimuth := GetAzimuthDeg(plon, plat, lon, lat);
{
    az := Round(Path.Azimuth/10);
    if az > 35 then az := 0;
    tx.Get_effectheight(az, heff);
    path.HEff := heff;
}
    tx.get_h_eff(Round(path.Azimuth), heff);
    path.HEff := heff;

    path.TxClearance := NaN;
    path.RxClearance := NaN;
    path.SeaPercent := 0;
  end else NewRunPointToPoint(p1, p2, geodata, path);
  except
   path.Distance  := GetDistanceKm(p1.L, p1.H, p2.L, p2.H);
   path.Azimuth := GetAzimuthDeg (p1.L, p1.H, p2.L, p2.H);
   az := Round(path.Azimuth);
   path.HEff := heff;
   path.TxClearance := NaN;
   path.RxClearance := NaN;
   path.SeaPercent := 0;
  end;

  if Path.Distance < _DISTANCE_STEP then Path.Distance := _DISTANCE_STEP;

  if (systemcast = ttFxm) then begin
    tx.QueryInterface(IID_ILisBcFxm, fxm);
    fxm_system := fxm.fxm_system;
    if (fxm_system = osBC) or (fxm_system = osBD) then begin
        try begin
            SetPropagServer(nil);
            _propag.GetFieldStrength(tx, path, perc, e);
        end;
        finally
            SetPropagServer(_prop);
        end;
    end else
        _propag.GetFieldStrength(tx, path, perc, e);
  end else
    _propag.GetFieldStrength(tx, path, perc, e);

  e := e - CorrectFieldStrength(tx);
  if _debug then
  begin
    tx.Get_id(id);
//    old_color := _message_color;
    SetMessageColor(clRed);
    AddMessage('--------------Напряженность в точке (ДОЛ, ШИР)--------');
    AddMessage('  ID: ' + IntToStr(id));

    tx.get_freq_carrier(f);
    az := Round(Path.Azimuth);
    if az > 354 then az := 354;
    tx.get_erp(az, erp);

    AddMessage('FREQ: ' + FloatToStr(f) + ' Mhz');
    AddMessage(' ERP: ' + FloatToStr(erp) + ' dBkW');

    FloatToCoordStr(p1.L, p1.H, lons, lats);
    AddMessage('LON1: ' + lons);
    AddMessage('LAT1: ' + lats);

    FloatToCoordStr(p2.L, p2.H, lons, lats);
    AddMessage('LON2: ' + lons);
    AddMessage('LAT2: ' + lats);

    //    AddMessage('LON1: ' + FloatToCoordStr(p1.L));
//    AddMessage('LAT1: ' + FloatToStr(p1.H));
//    AddMessage('LON2: ' + FloatToStr(p2.L));
//    AddMessage('LAT2: ' + FloatToStr(p2.H));
    AddMessage('   Distance: ' + FloatToStr(Path.Distance) + ' km');
    AddMessage('    Azimuth: ' + FloatToStr(Path.Azimuth) + ' deg');

//    if isNaN(Path.HEff) then tx.get_h_eff(Trunc(Path.Azimuth/10), h);
    if isNaN(Path.HEff) then tx.get_h_eff(az, h);

    AddMessage('       Heff: ' + IntToStr(h) + ' m');
    AddMessage('RxClearance: ' + FloatToStr(Path.RxClearance) + ' deg');
    AddMessage('TxClearance: ' + FloatToStr(Path.TxClearance) + ' deg');
    AddMessage('Sea Percent: ' + FloatToStr(Path.SeaPercent) + ' %');
    AddMessage('Field Stren: ' + FloatToStr(e) + 'dBmkV/m');
    AddMessage('--------------*********************----------------------');
/// SetMessageColor(old_color);
  end;
  result := e;

  tx.Get_polarization(polar);
  if polar = plMIX then
  begin
    tx.get_erp(az, erp);
    if az >= 355 then az := 0;
    azl := Floor(az / 10.0);
    tx.Get_effectpowervert (azl, erp_vl);
    if azl = 35 then
        azl := -1;
    tx.Get_effectpowervert(azl + 1, erp_vh);
    e_h := e;
    erp_v := erp_vl + (erp_vh - erp_vl)*(az / 10.0 - azl);
    e_v := e - erp + erp_v;
    result := Max(e_h, e_v);
  end;

end;



{
  Азимут в градусах, 0..360
}
function TCoLISBCCalc.GetE_Azimuth(tx: ILISBCTx; a, d: double; perc: TPercentage): double;
var plon, plat, e, f, erp, erp_vl, erp_vh, e_h, e_v: double;
    p1, p2: TRSAGeoPoint;
    azl, h, id, az: integer;
    geodata: TRSAGeoPathData;
    path: TRSAGeoPathResults;
    lons, lats: TCoordString;
    heff: integer;
    prms: TRSAPathParams;
    systemcast: TBCTxType;
    allot: ILisBcDigAllot;
    polar: TBCPolarization;
    erp_h, erp_v: double;
    rn: TReferenceNetwork;

    fxm_system : integer;
    fxm: ILisBcFxm;
begin
  az := 0;
  tx.Get_systemcast(systemcast);
  if systemcast = ttAllot then
  begin
    OleCheck(tx.QueryInterface(ILisBcDigAllot, allot));
    tx.Get_longitude(plon);
    tx.Get_latitude(plat);
    GetNextCoordDeg(a,d,plon,plat);

    rn := CreateRn(allot);
    try
        result := GetE_Allot(allot, rn, plon, plat);
    finally
        rn.Free;
    end;
    Exit;
  end;
//  if d < _DISTANCE_STEP / 10 then d := _DISTANCE_STEP / 10;
  tx.Get_longitude(plon);
  tx.Get_latitude(plat);
  p1.H := plat;
  p1.L := plon;
  p1.Alt := 0;

  tx.Get_heightantenna(h);
//  tx.Get_height_eft_max(h);
  geodata.TxHeight := h;
  geodata.RxHeight := _RX_ANTENNA_HEIGHT;

  if (_relief <> nil) and (_use_morphology_only) then
  begin
    _relief.Get_Step(prms.Step);
    prms.CalcHEff := false;
    prms.CalcTxClearance := false;
    prms.CalcRxClearance := false;
    prms.CalcSeaPercent := true;
    prms.Step := Max(_STEP_FINAL, d / 100);
    _relief.Set_Params(prms)
  end;
    try
  if _relief = nil then
  begin
    path.Distance := d;
    path.Azimuth := a;
{
    az := Round(Path.Azimuth/10);
    if az > 35 then az := 0;
    tx.Get_effectheight(az, heff);
    path.HEff := heff;
}
    tx.get_h_eff(Round(path.Azimuth), heff);
    path.HEff := heff;
    path.TxClearance := NaN;
    path.RxClearance := NaN;
    path.SeaPercent := 0;
  end else NewRunOnAzimuth(p1, a, d, geodata, path);
    except
      p2.H := p1.H;
      p2.L := p1.L;
      p2.Alt := p1.Alt;
      GetNextCoordDeg(a, d, p2.L, p2.H);
      NewRunPointToPoint(p1, p2, geodata, path)

//     showMessage('BUG!!! ' + FloatToStr(d) + ' '  + FloatToStr(a));
//     beep;
    end;
    
  if systemcast = ttFxm then begin
    tx.QueryInterface(IID_ILisBcFxm, fxm);
    fxm_system := fxm.fxm_system;
        if (fxm_system = osBC) or (fxm_system = osBD) then begin
            try begin
                SetPropagServer(nil);
                _propag.GetFieldStrength(tx, path, perc, e);
            end;
            finally
                SetPropagServer(_prop);
            end;
        end else
            _propag.GetFieldStrength(tx, path, perc, e);
  end else begin
    _propag.GetFieldStrength(tx, path, perc, e);
  end;


  e := e - CorrectFieldStrength(tx);
  if _debug then
  begin
    tx.Get_id(id);
    SetMessageColor(clRed);
    AddMessage('--------------Напряженность в точке (D, AZ)--------');
    AddMessage('  ID: ' + IntToStr(id));

    tx.get_freq_carrier(f);
    az := Round(Path.Azimuth);
    if az > 354 then az := 354;
    tx.get_erp(az, erp);

    AddMessage('FREQ: ' + FloatToStr(f) + ' Mhz');
    AddMessage(' ERP: ' + FloatToStr(erp) + ' dBkW');

    FloatToCoordStr(p1.L, p1.H, lons, lats);
    AddMessage('LON1: ' + lons);
    AddMessage('LAT1: ' + lats);

//    AddMessage('LON1: ' + FloatToStr(p1.L));
//    AddMessage('LAT1: ' + FloatToStr(p1.H));
//    AddMessage('LON2: ' + FloatToStr(p2.L));
//    AddMessage('LAT2: ' + FloatToStr(p2.H));
    AddMessage('   Distance: ' + FloatToStr(Path.Distance) + ' km');
    AddMessage('    Azimuth: ' + FloatToStr(Path.Azimuth) + ' deg');

    if isNaN(Path.HEff) then tx.get_h_eff(az, h);

    AddMessage('       Heff: ' + IntToStr(h) + ' m');
    AddMessage('RxClearance: ' + FloatToStr(Path.RxClearance) + ' deg');
    AddMessage('TxClearance: ' + FloatToStr(Path.TxClearance) + ' deg');
    AddMessage('Sea Percent: ' + FloatToStr(Path.SeaPercent) + ' %');
    AddMessage('Field Stren: ' + FloatToStr(e) + 'dBmkV/m');
    AddMessage('--------------*********************----------------------');
  end;

  result := e;
  tx.Get_polarization(polar);
  if polar = plMIX then
  begin
    az := Round(a);
    tx.get_erp(az, erp);

    if az >= 355 then az := 0;
    azl := Floor(az / 10.0);
    tx.Get_effectpowervert (azl, erp_vl);
    if azl = 35 then
        azl := -1;
    tx.Get_effectpowervert(azl + 1, erp_vh);
    e_h := e;
    erp_v := erp_vl + (erp_vh - erp_vl)*(az / 10.0 - azl);
    e_v := e - erp + erp_v;
    result := Max(e_h, e_v);
  end;
end;



procedure TCoLISBCCalc.Init;
begin
  SetReliefServer(nil);
  SetPropagServer(nil);
  _txlist := nil;
  _spherics := CoRSASpherics.Create;
   SetLogFileName('');
  _calc_method := cmPowerSum;
  _cover_probability := 0.5;
  _STEP_FINAL := 0.5;
  _messages := TStringList.Create;
  _protect := CoProtectRatio.Create;
  _message_color_str := '';
  _message_color := -1;
  _lis_progress := nil;
  _use_morphology_only := false;
  _prop := nil;

{
  Теперь загружаем все параметры из реестра
}
  LoadParamsFromRegistry;

  _cd := CoCoordDist.Create;
  _cd.Init(_coord_dist_ini_file);

  _DISTANCE_STEP :=_STEP_FINAL;
  _emin_for_allotment := -999;

//  ErrorMessage('ULISBCCalcCOM', 'Ошибка инициализации');
end;



{
  Функция для сортировки списка передатчиков по
  полю SortKey
}
function CompareTx(Item1, Item2: Pointer): Integer;
var tx1, tx2: ILISBCTx;
    e1, e2: double;
begin
  tx1 := ILISBCTx(Item1);
  tx1.Get_Sort_Key_In(e1);
  tx2 := ILISBCTx(Item2);
  tx2.Get_Sort_Key_In(e2);
  result := 0;
  if e1>e2 then result := -1;
  if e1<e2 then result := 1;
end;



{
  Функция для сортировки списка передатчиков по
  полю SortKey2
}
function CompareTx2(Item1, Item2: Pointer): Integer;
var tx1, tx2: ILISBCTx;
    e1, e2: double;
begin
  tx1 := ILISBCTx(Item1);
  tx1.Get_Sort_Key_Out(e1);
  tx2 := ILISBCTx(Item2);
  tx2.Get_Sort_Key_Out(e2);
  result := 0;
  if e1>e2 then result := -1;
  if e1<e2 then result := 1;
end;



procedure GetNextCoord(azimuth: double; distance: double; var lon, lat: double);
const
  EARTH_RADIUS = 6371.032;    // средний радиус Земли (км)
  EARTH_RADIUS_POLAR = 6356.777;    // полярный радиус Земли (км)
  EARTH_RADIUS_EQUATOR = 6378.160;    // радиус Земли по экватору(км)

var delta_lat, delta_lon, lon_lat: double;
begin
////////////////////////////////////////////////
// процедура возвращает координаты точки
// находящейся на расстояниии distance
// по направлению azimuth от точки,
// заданной координатами (lon, lat)
// результирующие координаты возвращаются
// в тех же (lon, lat)...
////////////////////////////////////////////////

  if distance=0 then Exit;

  lat := DegToRad(lat);
  lon := DegToRad(lon);

  lon_lat:=distance / EARTH_RADIUS;   // дистанция в градусах

 // azimuth := azimuth - PI/2;

//  delta_lat:=ArcSin(Sin(lon_lat) * Sin(azimuth));          // широта
//  delta_lon:=ArcTan(Tan(lon_lat) * Cos(azimuth));          // долгота

//  delta_lat:=ArcSin(Sin(lon_lat) * Sin(PI/2 - azimuth));   // широта
//  delta_lon:=ArcTan(Tan(lon_lat) * Cos(PI/2 - azimuth));   // долгота

  delta_lat:=ArcSin(Sin(lon_lat) * Sin(azimuth + PI/2));     // широта
  delta_lon:=ArcTan(Tan(lon_lat) * Cos(azimuth - PI/2));     // долгота
{
  PI/2 добавляем и отнимаем потому что у нас отсчет азимута - от севера (см GetAzimuth)
}

//  delta_lon:=delta_lon * Cos (lat + delta_lat / 2);   // поправка для долготы
  delta_lon:=delta_lon / Cos (lat + delta_lat / 2);   // поправка для долготы

  lat:=lat + delta_lat;
  lon:=lon + delta_lon;
  lat := RadToDeg(lat);
  lon := RadToDeg(lon);
end;









{
  Расчет зоны ограниченной шумами
}
procedure TCoLISBCCalc.GetZone_NoiseLimited(const tx: ILISBCTx;
  step_deg: Double; out zone_km: PSafeArray);
var d: double;
    i: integer;
    a_deg: integer;
    naz: integer;
    idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
begin
  naz := Trunc(360 / step_deg);
  bounds.cElements := naz;
  bounds.lLbound := 0;
  zone_km := SafeArrayCreate(varDouble, 1, bounds);

  d := GetMaxDistanceWithOutInterferences(tx, _DISTANCE_STEP, 0);
  idx[0] := 0;
  SafeArrayPutElement(zone_km, idx, d);

  for i := 1 to naz-1 do
  begin
    a_deg := Round(i * step_deg);
    d := GetMaxDistanceWithOutInterferences(tx, d, a_deg);
    idx[0] := i;
    SafeArrayPutElement(zone_km, idx, d);
  end;
end;



function TCoLISBCCalc.GetUsableFieldStrength(lon, lat: Double): Double;
var
//    cmd: PChar;
    eu: double;
begin
  result := -1;
{
  try
    SetLogFileName(_DEBUG_FILE_NAME);
  except
    ShowMessage('Не могу создать файл ' + _DEBUG_FILE_NAME);
  end;
}
  if _debug then
  begin
    SetMessageColor(clGreen);
    AddMessage('<><><><><><><>Расчет Еисп.<><><><><><><><><><><>');
  end;
  case _calc_method of
    cmChester: eu := GetEusableChester(lon, lat);
    cmPowerSum: eu := GetEusablePowerSum(lon, lat);
    cmSimplified: eu := GetEusableSimplif(lon, lat, _COVER_PROBABILITY);
  else
    eu := GetEusablePowerSum(lon, lat);
  end;

  if _debug then
  begin
    SetMessageColor(clGreen);
    AddMessage('Eu = ' + FloatToStr(eu) + ' dBmkV/m');
    AddMessage('<><><><><><><>************<><><><><><><><><><><>');
  end;

{
  if _debug and (MessageDlg('Показать отчет?', mtConfirmation, [mbYes, mbNo], 0) = mrYes) then
  begin
    SetLogFileName('');
    s := 'iexplore '+ _DEBUG_FILE_NAME;
    cmd := PChar(s);
    WinExec(cmd , SW_MAXIMIZE);
  end;
}

  result := eu;
end;




{
  Проверяет условие превышения напряженности поля передатчика E величины
  используемой напряженности поля Eu (Честер 97 Annex1 page 58).
  Может использоваться при определении макс. радиуса зоны приема в заданном
  направлении ( function GetMaxDistance() ) вместо функции CheckE();
  см. также CheckE

  Азимут  - (0 .. 360)
}
{
function TCoLISBCCalc.CheckEChester(azimuth, d: double): boolean;
var lon0, lat0: double;
    e1, e2: double;
    tx0: ILISBCTx;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);
  GetNextCoordDeg(azimuth, d, lon0, lat0);
  e2 := GetEusableChester(lon0, lat0);
  e1 := GetE(tx0, lon0, lat0, 50);
  result := (e1 >= e2);
end;
}


function TCoLISBCCalc.CheckEChester(azimuth, d: double): double;
var lon0, lat0: double;
    e1, e2: double;
    tx0: ILISBCTx;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);
  GetNextCoordDeg(azimuth, d, lon0, lat0);
  e2 := GetEusableChester(lon0, lat0);
  e1 := GetE(tx0, lon0, lat0, 50);
///  tx0._Release;
  result := e1 - e2;
end;



{
  Проверка условия нормального приема - сигнал от передатчика
  должен быть больше суммарной помехи и больше Емин.
  Azimuth = (0..360);
  Используется при определении макс. радиуса зоны приема в заданном
  направлении ( function GetMaxDistance() )
  см. также CheckE2
}
{function TCoLISBCCalc.CheckEPowerSum(azimuth, d: double): boolean;
var e1, e2, emin: double;
    tx0: ILISBCTx;
    lon0, lat0: double;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);

  GetNextCoordDeg(azimuth, d, lon0, lat0);
  e2 := GetSumEControlPoint(lon0, lat0);

  e1 := GetE_Azimuth(tx0, azimuth, d, 50);

  emin := _GetEMin(tx0);
  result := (e1 >= e2) and (e1 >= emin);
end;


}
function TCoLISBCCalc.CheckEPowerSum(azimuth, d: double): double;
var e1, e2, emin: double;
    tx0: ILISBCTx;
    lon0, lat0: double;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);

  GetNextCoordDeg(azimuth, d, lon0, lat0);
  e2 := GetSumEControlPoint(lon0, lat0);

  e1 := GetE_Azimuth(tx0, azimuth, d, 50);

  emin := GetEMin(tx0);

  e2 := Max(e2, emin);

  result := e1 - e2;

end;



function TCoLISBCCalc.CheckEmin(tx: ILISBCTx; azimuth, d: double): double;
var e1, emin: double;
begin
  e1 := GetE_Azimuth(tx, azimuth, d, 50);
  emin := GetEMin(tx);
  result := e1 - emin;
end;



{
function TCoLISBCCalc.CheckESimplif(azimuth, d: double): boolean;
var e1, e2, emin: double;
    tx0: ILISBCTx;
    lon0, lat0: double;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);

  GetNextCoordDeg(azimuth, d, lon0, lat0);
  e2 := GetEusableSimplif(lon0, lat0, _COVER_PROBABILITY);

  e1 := GetE_Azimuth(tx0, azimuth, d, 50);

  emin := _GetEMin(tx0);
  result := (e1 >= e2) and (e1 >= emin);
end;
}

{
  Проверяет, соответствует ли напряженность в точке (azimuth, d) требуемой.
  Возвращает разность между напряженностью, создаваемой основным передатчиком
  и требуемой напряженностью
}
function TCoLISBCCalc.CheckESimplif(azimuth, d: double): double;
var e1, e2, emin: double;
    tx0: ILISBCTx;
    lon0, lat0: double;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);

  GetNextCoordDeg(azimuth, d, lon0, lat0);
  e2 := GetEusableSimplif(lon0, lat0, _COVER_PROBABILITY);

  e1 := GetE_Azimuth(tx0, azimuth, d, 50);

  emin := GetEMin(tx0);

///  tx0._Release;

  e2 := Max(e2, emin);

  result := e1 - e2;
end;



{
   Расчет максимальной дистанции (радиуса уверенного приема) в заданном
   направлении. Параметр d указывает с какой дистанции следует начинать
   итерации при поиске MaxDistance
   Azimuth = (0 .. 360);
}
{
function TCoLISBCCalc.GetMaxDistance(d, azimuth: double): double;
var dd: double;
    k: shortint;
begin
  dd := DISTANCE_STEP;
  if d < DISTANCE_STEP then d := DISTANCE_STEP;
{
  Два варианта расчетов -
  1. по напряженности поля помехи
  2. по используемой напряженности поля
  Закомментировать ненужный!!!
}
{
  Вариант 1.
}
//  if CheckE2(azimuth, d) then k := 1 else k := -1;
//  while (d>DISTANCE_STEP) and (CheckE2(azimuth, d) xor (k = -1)) do d := d + k * dd;
{
  Вариант 2.
}
//  if CheckE(azimuth, d) then k := 1 else k := -1;
//  while (CheckE(azimuth, d) xor (k = -1)) and (d>=DISTANCE_STEP) do d := d + k * dd;

//  if d < 0 then result := 0 else result := d;
//end;



{
   Расчет максимальной дистанции (радиуса уверенного приема) в заданном
   направлении. Параметр d указывает с какой дистанции следует начинать
   итерации при поиске MaxDistance
   Azimuth = (0 .. 360);
}

function TCoLISBCCalc.GetMaxDistance(d, azimuth: Double): Double;
var step_init: double;
    tx0: ILISBCTx;
    txtype: TBCTxType;
    f: double;
    allot: ILISBCDigAllot;
begin

  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_systemcast(txtype);
  if txtype = ttAllot then
  begin
//    if false then
    if _emin_for_allotment < -990 then
    begin
      OleCheck(tx0.QueryInterface(ILisBcDigAllot, allot));
      allot.Get_freq(f);
      EminDialog := TEminDialog.Create(nil);
      EminDialog.Frequency := f;
      if EminDialog.ShowModal = mrOk then _emin_for_allotment := EminDialog.Emin;
      EminDialog.Free;
    end;
    result := GetMaxDistanceEmin_GE06(tx0, azimuth, _emin_for_allotment);
    Exit;
  end;
{
  Если последний параметр не NIL, то функция расчитает
  максимальный радиус без учета помех - теоретический радиус
}

  if _debug then
  begin
    SetMessageColor(clOlive);
    AddMessage('++++ Расчет максим. дистанции по аз. ' + FloatToStr(azimuth) + ' град. ++++');
  end;


  if d <= _DISTANCE_STEP then step_init := _STEP_INIT else step_init := d / 5;

  if _quick_calc_max_dist then
    result := GetMaxDistancePrec_Quick(d, azimuth, step_init, _STEP_FINAL, nil) else
    result := GetMaxDistancePrec(d, azimuth, step_init, _STEP_FINAL, nil);

  if _debug then
  begin
    SetMessageColor(clOlive);
    AddMessage('+++++++++ *********************** +++++++++');
  end;
end;



function TCoLISBCCalc.GetMaxRadius(d_initial, azimuth: Double): Double; safecall;
begin
    result := GetMaxDistance(d_initial, azimuth);
end;



{
  Расчет для передатчика зоны покрытия, ограниченной мешающим воздействием
  других передатчиков из списка
}
procedure TCoLISBCCalc.GetZone_InterferenceLimited(step_deg: Double;
  out zone_km: PSafeArray);
var
    d: double;
    i, k: integer;
    a_deg: integer;
    //b: wordbool;
    perc: integer;
    naz: integer;
    idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    tx: ILisBcTx;
    txtype: TBcTxType;
begin

  naz := Trunc(360 / step_deg);
  bounds.cElements := naz;
  bounds.lLbound := 0;
  zone_km := SafeArrayCreate(varDouble, 1, bounds);

  perc := 0;
  Fire_Progress(perc);

  d := GetMaxDistance(_DISTANCE_STEP, 0);
  idx[0] := 0;
  SafeArrayPutElement(zone_km, idx, d);

  for i := 1 to naz - 1 do
  begin
    perc := Round(i*100 / naz);
    Fire_Progress(perc);

    if perc <= 100 then
    begin
        a_deg := Round(i * step_deg);
        d := GetMaxDistance(d, a_deg);
        idx[0] := i;
        SafeArrayPutElement(zone_km, idx, d);
    end else begin
        for k := i to naz - 1 do
        begin
            idx[0] := k;
            d := 0;
            SafeArrayPutElement(zone_km, idx, d);
        end;
        Break;
    end;
  end;

  _txlist.Get_Tx(0, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
  tx.Get_systemcast(txtype);

  _emin_for_allotment := -999;

end;


{
function TLISBCCalcCOM.GetTxCount: Integer;
begin
  result := _tx_list.Count;
end;
}

{
procedure TCoLISBCCalc.TxToData(tx: ILISBCTx; var data: TTxData);
var i, n: integer;
    d: double;
    sys: TBCTvSystems;
    std: TBCTvStandards;
    off: TBCOffsetType;
    DIR: tbcdIRECTION;
    pol: TBCPolarization;
    typ: TBCTxType;
begin
  tx.Get_id(i);
  data._id := i;

//  tx.get_freq_carrier(d);
//  data._f := d;

  tx.Get_typesystem(sys);
  data._sys := sys;

  tx.Get_systemcolor(std);
  data._std := std;

  tx.Get_typeoffset(off);
  data._offset_type := off;

  tx.Get_channel_id(i);
  data._ch := i;

  tx.Get_video_carrier(d);
  data._v_carrier := d;

  tx.Get_video_offset_herz(i);
  data._v_offset := i;

  tx.Get_sound_carrier_primary(d);
  data._s_carrier := d;

  tx.Get_sound_offset_primary(i);
  data._s_offset := i;

  tx.Get_power_video(d);
  data._v_power := d;

  tx.Get_power_sound_primary(d);
  data._s_power := d;

  tx.Get_epr_video_max(d);
  data._v_erp_max := d;

  tx.Get_epr_video_hor(d);
  data._v_erp_hor := d;

  tx.Get_epr_video_vert(d);
  data._v_erp_ver := d;

  tx.Get_epr_sound_max_primary(d);
  data._s_erp_max := d;

  tx.Get_epr_sound_hor_primary(d);
  data._s_erp_hor := d;

  tx.Get_epr_sound_vert_primary(d);
  data._s_erp_ver := d;

  tx.Get_direction(dir);
  data._direction := dir;

  tx.Get_longitude(d);
  data._lon := d;

  tx.Get_latitude(d);
  data._lat := d;

  tx.Get_polarization(pol);
  data._polar := pol;

  tx.Get_heightantenna(i);
  data._h := i;

  tx.Get_height_eft_max(i);
  data._h_max := i;

  tx.Get_monostereo_primary(i);
  data._mono_stereo := i;

  tx.Get_systemcast(typ);
  data._type := typ;

  for n := 0 to 35 do
  begin
    tx.Get_effectpowerhor(n, d);
    data._erp_az_h[n] := d;
  end;

  for n := 0 to 35 do
  begin
    tx.Get_effectpowervert(n, d);
    data._erp_az_v[n] := d;
  end;

  for n := 0 to 35 do
  begin
    tx.Get_effectheight(n, d);
    data._hef_az[n] := d;
  end;
end;

}

{
  Читает нужное поле из файла TVA
  Поле задается значениями start (позиция начала поля) и width (длина поля)
}
function GetDataFromTVA(tva: string; start, width: integer): string;
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


{
procedure TCoLISBCCalc.DataToTx(data: TTxData; var tx: ILISBCTx);
var n: integer;
    d: double;
begin

//  tx.Set_id(data._id);
//  tx.Set_freq_carrier(data._f);
  tx.Set_typesystem(data._sys);
  tx.Set_systemcolor(data._std);
  tx.Set_typeoffset(data._offset_type);
  tx.Set_channel_id(data._ch);
  tx.Set_video_carrier(data._v_carrier);
  tx.Set_video_offset_herz(data._v_offset);
  tx.Set_sound_carrier_primary(data._s_carrier);
  tx.Set_sound_offset_primary(data._s_offset);
  tx.Set_power_video(data._v_power);
  tx.Set_power_sound_primary(Round(data._s_power));
  tx.Set_epr_video_max(data._v_erp_max);
  tx.Set_epr_video_hor(data._v_erp_hor);
  tx.Set_epr_video_vert(data._v_erp_ver);
  tx.Set_epr_sound_max_primary(data._s_erp_max);
  tx.Set_epr_sound_hor_primary(data._s_erp_hor);
  tx.Set_epr_sound_vert_primary(data._s_erp_ver);
  tx.Set_direction(data._direction);
  tx.Set_longitude(data._lon);
  tx.Set_latitude(data._lat);
  tx.Set_polarization(data._polar);
  tx.Set_heightantenna(data._h);
  tx.Set_height_eft_max(data._h_max);
  tx.Set_monostereo_primary(data._mono_stereo);
  tx.Set_systemcast(data._type);
//  tx.Set_Sort_Key_in(data._sort_key);
//  tx.Set_Sort_Key_out(data._sort_key);

  for n := 0 to 35 do
  begin
    d := data._erp_az_h[n];
    tx.Set_effectpowerhor(n, d);
  end;

  for n := 0 to 35 do
  begin
    d := data._erp_az_v[n];
    tx.Set_effectpowervert(n, d);
  end;

  for n := 0 to 35 do
  begin
    d := data._hef_az[n];
    tx.Set_effectheight(n, d);
  end;

end;

}

{
  Расчет дуэли для АТВ
}
procedure TCoLISBCCalc.CalcDuel(const tx1, tx2: ILISBCTx;
  var duel_result: TDuelResult);
var az_1_2: double;
    az_2_1: double;
    lon1, lat1, lon2, lat2, d: double;
    lon, lat: double;
    e: double;
//    it: TBCSInterferenceType;
begin
//  result := S_OK;
  d := 30;
  tx1.Get_longitude(lon1);
  tx1.Get_latitude(lat1);
  tx2.Get_longitude(lon2);
  tx2.Get_latitude(lat2);

  az_1_2 := GetAzimuth(lon1, lat1, lon2, lat2);
  az_2_1 := az_1_2 - PI;
{
  Расчитываем помеху на границе идеальной зоны Tx1 в точке ближней к
  передатчику Tx2
}
  d := GetMaxDistanceWithOutInterferences(tx1, d, az_1_2);
  lon := lon1;
  lat := lat1;
  GetNextCoord(az_1_2, d, lon, lat);
  e := GetEControlPoint(tx1, tx2, lon, lat, nil);

  duel_result.lon1 := lon;
  duel_result.lat1 := lat;
  duel_result.eu1 := e;
{
  Расчитываем помеху на границе идеальной зоны Tx1 в точке дальней от
  передатчика Tx2 (т.е. на противоположной стороне зоны)
}
  d := GetMaxDistanceWithOutInterferences(tx1, d, az_2_1);
  lon := lon1;
  lat := lat1;
  GetNextCoord(az_2_1, d, lon, lat);
  e := GetEControlPoint(tx1, tx2, lon, lat, nil);

  duel_result.lon2 := lon;
  duel_result.lat2 := lat;
  duel_result.eu2 := e;

{
  Расчитываем помеху на границе идеальной зоны Tx2 в точке ближней к
  передатчику Tx1
}
  d := GetMaxDistanceWithOutInterferences(tx2, d, az_2_1);
  lon := lon2;
  lat := lat2;
  GetNextCoord(az_2_1, d, lon, lat);
  e := GetEControlPoint(tx2, tx1, lon, lat, nil);

  duel_result.lon3 := lon;
  duel_result.lat3 := lat;
  duel_result.eu3 := e;
{
  Расчитываем помеху на границе идеальной зоны Tx2 в точке дальней от
  передатчика Tx1 (т.е. на противоположной стороне зоны)
}
  d := GetMaxDistanceWithOutInterferences(tx2, d, az_1_2);
  lon := lon2;
  lat := lat2;
  GetNextCoord(az_1_2, d, lon, lat);
  e := GetEControlPoint(tx2, tx1, lon, lat, nil);

  duel_result.lon4 := lon;
  duel_result.lat4 := lat;
  duel_result.eu4 := e;
end;



{
  StrToFloat с учетом типа разделителя - '.' или ','
  и удалением лишних пробелов
}
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
procedure TCoLISBCCalc.SaveDataToFile(data: TTxData; filename: string);
var f: TIniFile;
    i: integer;
    sep: char;
begin
  f := TIniFile.Create(filename);

  sep := DecimalSeparator;
  DecimalSeparator := '.';
 try
  f.WriteString('TRANSMITTER', 'ID', IntToStr(data._id));
  f.WriteString('TRANSMITTER', 'TXTYPE', IntToStr(data._type));

  f.WriteString('TRANSMITTER', 'LON', FloatToStr(data._lon));
  f.WriteString('TRANSMITTER', 'LAT', FloatToStr(data._lat));

  f.WriteString('TRANSMITTER', 'TVSYSTEM', IntToStr(data._sys));
  f.WriteString('TRANSMITTER', 'TVCOLORSTD', IntToStr(data._std));
  f.WriteString('TRANSMITTER', 'TYPEOFFSET', IntToStr(data._offset_type));

  f.WriteString('TRANSMITTER', 'CHANNEL', IntToStr(data._ch));
  f.WriteString('TRANSMITTER', 'DIRECTION', IntToStr(data._direction));

  f.WriteString('TRANSMITTER', 'POLAR', IntToStr(data._polar));
  f.WriteString('TRANSMITTER', 'MONOSTEREO', IntToStr(data._mono_stereo));

  f.WriteString('TRANSMITTER', 'ANTHEIGHT', IntToStr(data._h));
  f.WriteString('TRANSMITTER', 'HMAX', IntToStr(data._h_max));

  f.WriteString('TRANSMITTER', 'VCARRIER', FloatToStr(data._v_carrier));
  f.WriteString('TRANSMITTER', 'VOFFSET', IntToStr(data._v_offset));
  f.WriteString('TRANSMITTER', 'VPOWER', FloatToStr(data._v_power));
  f.WriteString('TRANSMITTER', 'VERPMAX', FloatToStr(data._v_erp_max));
  f.WriteString('TRANSMITTER', 'VERPVER', FloatToStr(data._v_erp_ver));
  f.WriteString('TRANSMITTER', 'VERPHOR', FloatToStr(data._v_erp_hor));

  f.WriteString('TRANSMITTER', 'SCARRIER', FloatToStr(data._s_carrier));
  f.WriteString('TRANSMITTER', 'SOFFSET', IntToStr(data._s_offset));
  f.WriteString('TRANSMITTER', 'SPOWER', FloatToStr(data._s_power));
  f.WriteString('TRANSMITTER', 'SERPMAX', FloatToStr(data._s_erp_max));
  f.WriteString('TRANSMITTER', 'SERPVER', FloatToStr(data._s_erp_ver));
  f.WriteString('TRANSMITTER', 'SERPHOR', FloatToStr(data._s_erp_hor));

  for i := 0 to 35 do f.WriteString('TRANSMITTER', 'ERPHOR_' + IntToStr(i), FloatToStr(data._erp_az_h[i]));
  for i := 0 to 35 do f.WriteString('TRANSMITTER', 'ERPVER_' + IntToStr(i), FloatToStr(data._erp_az_v[i]));
  for i := 0 to 35 do f.WriteString('TRANSMITTER', 'HEFF_' + IntToStr(i), FloatToStr(data._hef_az[i]));
 finally
  DecimalSeparator := sep;
 end;
end;



procedure TCoLISBCCalc.LoadDataFromFile(filename: string; var data: TTxData);
var f: TIniFile;
    i: integer;
    sep: char;
begin
  f := TIniFile.Create(filename);

  sep := DecimalSeparator;
  DecimalSeparator := '.';
 try
  data._type := StrToInt(f.ReadString('TRANSMITTER', 'TXTYPE', '0'));

  data._lon := StrToFloat(f.ReadString('TRANSMITTER', 'LON', '0'));
  data._lat := StrToFloat(f.ReadString('TRANSMITTER', 'LAT', '0'));

  data._sys := StrToInt(f.ReadString('TRANSMITTER', 'TVSYSTEM', '0'));
  data._std := StrToInt(f.ReadString('TRANSMITTER', 'TVCOLORSTD', '0'));
  data._offset_type := StrToInt(f.ReadString('TRANSMITTER', 'TYPEOFFSET', '0'));

  data._ch := StrToInt(f.ReadString('TRANSMITTER', 'CHANNEL', '0'));
  data._direction := StrToInt(f.ReadString('TRANSMITTER', 'DIRECTION', '0'));

  data._polar := StrToInt(f.ReadString('TRANSMITTER', 'POLAR', '0'));
  data._mono_stereo := StrToInt(f.ReadString('TRANSMITTER', 'MONOSTEREO', '0'));

  data._h := StrToInt(f.ReadString('TRANSMITTER', 'ANTHEIGHT', '0'));
  data._h_max := StrToInt(f.ReadString('TRANSMITTER', 'HMAX', '0'));

  data._v_carrier := StrToFloat(f.ReadString('TRANSMITTER', 'VCARRIER', '0'));
  data._v_offset := StrToInt(f.ReadString('TRANSMITTER', 'VOFFSET', '0'));
  data._v_power := StrToFloat(f.ReadString('TRANSMITTER', 'VPOWER', '0'));
  data._v_erp_max := StrToFloat(f.ReadString('TRANSMITTER', 'VERPMAX', '0'));
  data._v_erp_ver := StrToFloat(f.ReadString('TRANSMITTER', 'VERPVER', '0'));
  data._v_erp_hor := StrToFloat(f.ReadString('TRANSMITTER', 'VERPHOR', '0'));

  data._s_carrier := StrToFloat(f.ReadString('TRANSMITTER', 'SCARRIER', '0'));
  data._s_offset := StrToInt(f.ReadString('TRANSMITTER', 'SOFFSET', '0'));
  data._s_power := StrToFloat(f.ReadString('TRANSMITTER', 'SPOWER', '0'));
  data._s_erp_max := StrToFloat(f.ReadString('TRANSMITTER', 'SERPMAX', '0'));
  data._s_erp_ver := StrToFloat(f.ReadString('TRANSMITTER', 'SERPVER', '0'));
  data._s_erp_hor := StrToFloat(f.ReadString('TRANSMITTER', 'SERPHOR', '0'));

  for i := 0 to 35 do data._erp_az_h[i] := StrToFloat(f.ReadString('TRANSMITTER', 'ERPHOR_' + IntToStr(i), '0'));
  for i := 0 to 35 do data._erp_az_v[i] := StrToFloat(f.ReadString('TRANSMITTER', 'ERPVER_' + IntToStr(i), '0'));
  for i := 0 to 35 do data._hef_az[i] := StrToFloat(f.ReadString('TRANSMITTER', 'HEFF_' + IntToStr(i), '0'));
 finally
  DecimalSeparator := sep;
 end;
end;

}
{
function TCoLISBCCalc.SaveTxToFile(const tx: ILISBCTx;
  const filename: WideString): HResult;
var data: TTxData;
begin
  TxToData(tx, data);
  SaveDataToFile(data, filename);
  result := S_OK;
end;



function TCoLISBCCalc.LoadTxFromFile(const filename: WideString;
  var tx: ILISBCTx): HResult;
var data: TTxData;
begin
  LoadDataFromFile(filename, data);
  DataToTx(data, tx);
  result := S_OK;
end;



procedure TCoLISBCCalc.LoadFromFile(const filename: WideString);
var tx: ILISBCTx;
    data: TTXData;
    f: TFileStream;
    s: string[10];
    i: integer;
begin
  f := TFileStream.Create(filename, fmOpenRead);
  f.Seek(0, soFromBeginning);
  f.Read(s, SizeOf(s));
  if s <> FILE_VERSION then
  begin
    ErrorMessage('LoadFromFile', 'Unknown file format');
//    result := S_FALSE;
    Exit;
  end;
  _txlist.Clear;
  while f.Position < f.Size do
  begin
    f.Read(data, SizeOf(data));
    tx := CoLISBCTx.Create;
    DataToTx(data, tx);
    _txlist.AddTx(tx, i);
  end;
  f.Free;
//  result := S_OK;
end;



procedure TCoLISBCCalc.SaveToFile(const filename: WideString);
var tx: ILISBCTx;
    data: TTXData;
    f: TFileStream;
    n, i: integer;
    s: string[10];
begin
  f := TFileStream.Create(filename, fmCreate);
  _txlist.Get_Size(n);
  f.Seek(0, soFromBeginning);
  s := FILE_VERSION;
  f.Write(s, SizeOf(s));
  for i := 0 to n-1 do
  begin
    _txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
    TxToData(tx, data);
    f.Write(data, SizeOf(data));
    tx := nil;
  end;
  f.Free;
//  result := S_OK;
end;
}
{
function TLISBCCalcCOM.Clear: HResult;
begin
  while _tx_list.Count > 0 do RemoveTx(0);
  result := S_OK;
end;
}


{
  расчет простр. распределения напряженности поля в виде изолиний,
  представляющих зоны с различным знач.Emin
}
procedure TCoLISBCCalc.GetFieldDistribution(const tx: ILISBCTx;
  zone_number: Shortint; var distribution: TFieldDistribution);
type TFieldZoneArray = array[0..2] of TFieldZone;
const max_emin = 120; // dBmV/m
var zones: ^TFieldZoneArray;
    pz: P36ValuesArray;
    i, n: integer;
    d, a_deg, a: double;
    delta_emin, emin, current_emin: double;
begin
{
  расчитываем шаг мин. напряженности поля для отрисовки зон
}
  if zone_number > 5 then zone_number := 5;
  emin := GetEMin(tx);
  delta_emin := (max_emin - emin) / zone_number;
  delta_emin := delta_emin/1;
{
  просчитываем нулевое направление
}
  zones := @distribution;
  for n := 0 to zone_number-1 do
  begin
    current_emin := emin + delta_emin * n;
    d := GetMaxDistanceEMin(tx, 0.1, 0, current_emin);
    zones^[n].e := current_emin;
    pz := @zones^[n].zone;
    pz^[0] := d;
  end;
{
  просчитываем все зоны по каждому направлению
}
  for i := 1 to 35 do
  begin
    a_deg := i * 10;
    if a_deg > 180 then a_deg := a_deg - 360;
    a := DegToRad(a_deg);
    for n := 0 to zone_number-1 do
    begin
      current_emin := emin + delta_emin * n;
      d := GetMaxDistanceEMin(tx, 0.1, a, current_emin);
      zones^[n].e := current_emin;
      pz := @zones^[n].zone;
      pz^[i] := d;
    end;
  end;
//  result := S_OK;
end;




procedure TCoLISBCCalc.CalcDuel2(const tx1, tx2: ILISBCTx;
  var duel_result: TDuelResult2);
var _txs: ILISBCTxList;
    _old_tx_list: ILISBCTxList;
    val: double;
    dir, i: integer;
begin
{
  Расчитываем Noise-limited зоны для дуэлянтов
}
  GetZone_NoiseLimited(tx1, 10, duel_result.Tx1_NoiseLimited);
  GetZone_NoiseLimited(tx2, 10, duel_result.Tx2_NoiseLimited);

  _txs := CoLISBCTxList.Create;
{
  Создаем расчетный набор передатчиков:
    tx1 - анализируемый
    tx2 - мешающий
  и расчитываем Interference-Limited зону для первого передатчика
}
  _txs.AddTx(tx1, i);
  _txs.AddTx(tx2, i);
  _txs.Set_TxUseInCalc(0, TRUE);
  _txs.Set_TxUseInCalc(1, TRUE);
{
  На время расчета дуэли заменяем наш список передатчиков
  на _txs, который содержит лишь двух дуэлянтов
}
  _old_tx_list := _txlist;
  _txlist := _txs;

  try
    GetZone_InterferenceLimited(10, duel_result.Tx1_InterferenceLimited);

//    GetEuDeviation(duel_result.Tx1_NoiseLimited, duel_result.Tx1_InterferenceLimited, eu_result);

{
  Создаем другой расчетный набор передатчиков:
    tx2 - анализируемый
    tx1 - мешающий
  и расчитываем Interference-Limited зону для второго передатчика
}
    _txs.Clear;
    _txs.AddTx(tx2, i);
    _txs.AddTx(tx1, i);
    _txs.Set_TxUseInCalc(0, TRUE);
    _txs.Set_TxUseInCalc(1, TRUE);

    _txlist := _txs;

    GetZone_InterferenceLimited(10, duel_result.Tx2_InterferenceLimited);

///    GetEuDeviation(duel_result.Tx2_NoiseLimited, duel_result.Tx2_InterferenceLimited, eu_result);

  finally
//    _txs._Release;
    _txlist := _old_tx_list;
  end;
   duel_result.Area1_NoiseLimited := GetArea(duel_result.Tx1_NoiseLimited);
   duel_result.Area2_NoiseLimited := GetArea(duel_result.Tx2_NoiseLimited);
   duel_result.Area1_InterferenceLimited := GetArea(duel_result.Tx1_InterferenceLimited);
   duel_result.Area2_InterferenceLimited := GetArea(duel_result.Tx2_InterferenceLimited);

   GetMaxVariation(duel_result.Tx1_NoiseLimited, duel_result.Tx1_InterferenceLimited, val, dir);
   duel_result.Tx1_Variation_Val := val;
   duel_result.Tx1_Variation_Dir := dir;

   GetMaxVariation(duel_result.Tx2_NoiseLimited, duel_result.Tx2_InterferenceLimited, val, dir);
   duel_result.Tx2_Variation_Val := val;
   duel_result.Tx2_Variation_Dir := dir;
end;



{
  расчет площади зоны
}
function TCoLISBCCalc.GetArea(zone: PSafeArray): double;
var 
    i: integer;
    s: double;
    r1, r2: double;
    naz: integer;
    lbound, ubound: integer;
    idx: array[0..0] of integer;
    step_deg: double;
begin
  if SafeArrayGetDim(zone) <> 1 then
  begin
    ErrorMessage('TCoLISBCCalc.GetArea', 'Wrong SafeArray dimensions');
    result := 0;
    Exit;
  end;

  SafeArrayGetLBound(zone, 0, lbound);
  SafeArrayGetLBound(zone, 0, ubound);

  naz := ubound - lbound + 1;
  step_deg := 360 / naz;

  s := 0;
  for i := 0 to naz - 2 do
  begin
//   r1 := p^[i];
//   r2 := p^[i+1];
    idx[0] := i;
    SafeArrayGetElement(zone, idx, r1);
    idx[0] := i+1;
    SafeArrayGetElement(zone, idx, r2);

    s := s + 0.5 * r1 * r2 * Sin(DegToRad(step_deg));
  end;

//  r1 := p^[35];
//  r2 := p^[0];

  idx[0] := naz-1;
  SafeArrayGetElement(zone, idx, r1);
  idx[0] := 0;
  SafeArrayGetElement(zone, idx, r2);

  s := s + 0.5 * r1 * r2 * Sin(DegToRad(step_deg));
  result := s;
end;



procedure TCoLISBCCalc.GetMaxVariation(zone1, zone2: PSafeArray; var val: double; var dir: integer);
var p1, p2: P36ValuesArray;
    i, maxdir: integer;
    r1, r2, r, maxval: double;
begin


//  ErrorMessage('TCoLISBCCalc.GetMaxVariation', 'This procedure isn''t working yet');
  Exit;

  p1 := @zone1;
  p2 := @zone2;

  maxval := 0;
  maxdir := 0;

  for i := 0 to 35 do
  begin
    r1 := p1^[i];
    r2 := p2^[i];
    r := Abs(r2 - r1);
    if r > maxval then
    begin
      maxval := r;
      maxdir := i;
    end;
  end;
  val := maxval;
  dir := maxdir;
end;



function TCoLISBCCalc.GetFieldDistribution2(const tx: ILISBCTx; lon1, lat1,
  lon2, lat2, spacing: Double): PSafeArray;
type bds = array[0..1] of SAFEARRAYBOUND;
var
// da, dr: double; // шаг по углу и расстоянию
    a1, a2, r1, r2: double; // начальный и конечный угол и радиусы
    lon, lat: double;
    res: PSafeArray;
    bounds: ^bds;
    d, e: double;
    point1, point2: TRSAGeoPoint;
    txlon, txlat: double;
//    adim, rdim: integer;
    i, j: integer;
    londim, latdim: integer;
    idx: array[0..1] of integer;
begin
  bounds^[0].lLbound := 0;
  bounds^[0].cElements := 3;
  bounds^[1].lLbound := 0;
  bounds^[1].cElements := 3;

  point1.H := lat1;
  point1.L := lon1;
  point1.Alt := 0;
  point2.H := lat2;
  point2.L := lon2;
  point2.Alt := 0;

  _spherics.Distance(point1, point2, d);
{
 рассчитываем шаг по азимуту, чтобы он примерно
 соответствовал заданному SPACING-у
}
//  da := ArcSin(spacing / d);

{
  шаг по расстоянию равен спейсингу
}
//  dr := spacing;

{
  определяем нач. и конечн. значения азимута и дистанции
}
  tx.Get_longitude(txlon);
  tx.Get_latitude(txlat);
  point1.H := txlat;
  point1.L := txlon;
  point2.H := lat1;
  point2.L := lon1;
  _spherics.Azimuth(point1, point2, a1);
  _spherics.Distance(point1, point2, r1);
  point2.H := lat2;
  point2.L := lon2;
  _spherics.Azimuth(point1, point2, a2);
  _spherics.Distance(point1, point2, r2);
{
  определяем размерность массива по азимуту и расстоянию
}
//  adim := Round((a2 - a1) / da);
//  rdim := Round((r2 - r1) / dr);
//////////////////////////////////////////////////////////////////
{
  сделаем проще ...
  спейсинг  - это шаг по широте и долготе
}
  londim := Round((lon2-lon1) / spacing);
  latdim := Round((lat2-lat1) / spacing);

  bounds^[0].lLbound := 0;
  bounds^[0].cElements := londim;
  bounds^[1].lLbound := 0;
  bounds^[1].cElements := latdim;

  res := SafeArrayCreate(VT_R8, 2, bounds);

  for i := 0 to londim do
    for j := 0 to latdim do
  begin
    lon := lon1 + i * spacing;
    lat := lat1 + i * spacing;
    e := GetE(tx, lon, lat, 50);
    idx[0] := i;
    idx[1] := j;
    SafeArrayPutElement(res, idx, e);
  end;
//  resultarray := res;
  result := res;
//  result := S_OK;
end;



{
  Распределение напряженности поля
  a1, a2  - начальный и конечный азимут сектора (градусы 0 - 360)
  da      - шаг по азимуту (градусы)
  r1, r2  - начальный и конечный радиус (км.)
  dr      - шаг по радиусу
  resultarray - указатель на массив, в который будут записаны результаты.
  Массив создается в вызываемой функции, т.о. уничтожаться должен вызывающей функцией...

  Данные пакуются таким образом, что большему индексу соответсвует больший азимут,
  и большему индексу соответствует меньшее расстояние
}
function TCoLISBCCalc.GetFieldDistribution3(const tx: ILISBCTx; a1, a2, da,
  r1, r2, dr: Double): PSafeArray;
type bds = array[0..1] of SAFEARRAYBOUND;
var
    res: PSafeArray;
    bounds: bds;
//    pbounds: ^bds;
    d, e: double;
    adim, rdim: integer;
    i, j: integer;
    idx: array[0..1] of integer;
    r, a: double;
begin
{
  a2 всегда больше a1
  r2 всегда больше r1
}
  if a2 < a1 then
  begin
    d := a1;
    a1 := a2;
    a2 := d;
  end;
  if r2 < r1 then
  begin
    d := r1;
    r1 := r2;
    r2 := d;
  end;
{
  определяем размерность и создаем массив
}
  adim := Round((a2 - a1) / da);
  rdim := Round((r2 - r1) / dr);

  bounds[0].lLbound := 0;
  bounds[0].cElements := adim;
  bounds[1].lLbound := 0;
  bounds[1].cElements := rdim;
  res := SafeArrayCreate(VT_R8, 2, bounds);

  a := a1;
  for i := 0 to adim - 1 do
  begin
    r := r2;
    for j := 0 to rdim - 1 do
    begin
      e := GetE_Azimuth(tx, a, r, 50);
{
  Эта строчка теперь не нужна
      if e < emin then e := 0;
}
      idx[0] := i;
      idx[1] := j;
      SafeArrayPutElement(res, idx, e);
      r := r - dr;
    end;
    a := a + da;
  end;
//  resultarray := res;
  result := res;
//  result := S_OK;
end;



procedure TCoLISBCCalc.SetTxListServer(const txlist: ILISBCTxList);
begin
  if _txlist <> txlist then _txlist := txlist;
//  result := S_OK;
end;




procedure TCoLISBCCalc.Fire_Progress(var perc: integer);
var
  I: Integer;
  EventSinkList: TList;
  EventSink: ILISBCCalcEvents;
begin
  if perc < 0 then perc := 0;
  if perc > 100 then perc := 100;
  if FConnectionPoint <> nil then
  begin
    EventSinkList :=FConnectionPoint.SinkList; {get the list of client sinks }
    for I := 0 to EventSinkList.Count - 1 do
    begin
      EventSink := IUnknown(EventSinkList.Items[i]) as ILISBCCalcEvents;
      EventSink.Progress(perc);
    end;
  end;
  Fire_LISProgress(perc);
end;



procedure TCoLISBCCalc.EventSinkChanged(const EventSink: IUnknown);
begin
  FEvents := EventSink as ILISBCCalcEvents;
end;



procedure TCoLISBCCalc.Initialize;
begin
  inherited Initialize;
  FConnectionPoints := TConnectionPoints.Create(Self);
  if AutoFactory.EventTypeInfo <> nil then
    FConnectionPoint := FConnectionPoints.CreateConnectionPoint(
      AutoFactory.EventIID, ckSingle, EventConnect)
  else FConnectionPoint := nil;
end;



{
  Возвращает для передатч. tx0 отношение (Sinterf / Snoise) - т.е.
  отношение  площади зоны, огранич. помехами (со стороны мешающего
  передатчика tx1) к площади зоны, огранич. шумами для передатчика.
  Можно использовать как критерий мешающего воздействия передатчика
  tx1 на tx0.
}
function TCoLISBCCalc.GetSS(tx0, tx1: ILISBCTx): double;
var sn, si: double;
    _txs: ILISBCTxList;
    _old_tx_list: ILISBCTxList;
    i: integer;
    zn, zi: PSafeArray; // зоны шумов и помех
    xxx: double;
begin
{
  Расчитываем Noise-limited зону
}
  GetZone_NoiseLimited(tx0, 10,zn);
  _txs := CoLISBCTxList.Create;
{
  Создаем расчетный набор передатчиков:
    tx0 - анализируемый
    tx1 - мешающий
  и расчитываем Interference-Limited зону для первого передатчика
}
  _txs.AddTx(tx0, i);
  _txs.AddTx(tx1, i);
  _txs.Set_TxUseInCalc(0, TRUE);
  _txs.Set_TxUseInCalc(1, TRUE);
{
  На время расчета дуэли заменяем наш список передатчиков
  на _txs, который содержит лишь двух дуэлянтов
}
  _old_tx_list := _txlist;
  _txlist := _txs;

  try
    GetZone_InterferenceLimited(10, zi);
  finally
    _txlist := _old_tx_list;
  end;

  si := GetArea(zi);
  sn := GetArea(zn);

  if sn <= 0 then
  begin
    ErrorMessage('TLISBCCalcCOM.GetSS', 'Error: Snoise can''t be zero');
    result := 0;
    Exit;
  end;
  xxx := si / sn;
  result := xxx;
end;



{
  Подбор СНЧ для АТВ. Параметры: диапазон СНЧ (в линиях (1/12Fline)).
  На выходе массив [0..1, offset_start..offset_finish] of double.
}
function TCoLISBCCalc.OffsetSelection(offset_start,
  offset_finish: Integer): PSafeArray;
type bds = array[0..1] of SAFEARRAYBOUND;
var tx0: ILISBCTx;
    res: PSafeArray;
    bounds: bds;
    i: integer;
    idx: array[0..1] of integer;
    zi, zn: PSafeArray;
    ss0, ss1: double;
begin
{
  Наш массив имеет размерность [0..1, offset_start..offset_finish]. Все
  элементы массива типа double. В строке [0, ....]  записаны отношения
  (Sinterf / Snoise) для планируемого передатчика (нулевой индекс в выборке)
  с соответствующим СНЧ. В строке [1, ....] записаны максимальное мешающее
  воздействие планируемого передатчика на остальные передатчики в выборке
  (Критерием меш. воздействия также принимаются отношения Sinterf / Snoise
  для передатчков выборки, из которых выбирается минимальное)
}

  if offset_start > offset_finish then
  begin
    ErrorMessage ('TCoLISBCCalc.OffsetSelection', 'Offset_start should be less then Offset_finish');
//    result := S_FALSE;
    Exit;
  end;
  if (offset_start < -20) or  (offset_start > 20) then
  begin
    ErrorMessage ('TCoLISBCCalc.OffsetSelection', 'Offset_start should be in range [-20 .. 20]');
//    result := S_FALSE;
    exit;
  end;
  if (offset_finish < -20) or  (offset_finish > 20) then
  begin
    ErrorMessage ('TCoLISBCCalc.OffsetSelection', 'Offset_finish should be in range [-20 .. 20]');
//    result := S_FALSE;
    Exit;
  end;

  bounds[0].lLbound := 0;
  bounds[0].cElements := 2;
  bounds[1].lLbound := offset_start;
  bounds[1].cElements := offset_finish - offset_start + 1;

  res := SafeArrayCreate(VT_R8, 2, bounds);

  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  GetZone_NoiseLimited(tx0, 10, zn);
{
  Перебираем все значения СНЧ из диапазона и проводим
  расчеты для каждого значения. Величина noise_limited
  зоны не зависит от СНЧ.
}
  for i := offset_start to offset_finish do
  begin
    tx0.Set_video_offset_line(i);
    GetZone_InterferenceLimited(10, zi);
    ss0 := GetArea(zi) / GetArea(zn);
    ss1 := GetMinSS;
    idx[0] := 0;
    idx[1] := i;
    SafeArrayPutElement(res, idx, ss0);
    idx[0] := 1;
    SafeArrayPutElement(res, idx, ss1);
  end;
//  resultarray := res;
  result := res;
//  result := S_OK;
end;



function TCoLISBCCalc.GetMinSS: double;
var tx0, tx1: ILISBCTx;
    i, n: integer;
    ss: double;
    ssmin: double;
    b: wordbool;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  _txlist.Get_Size(n);
  ssmin := 1;
  for i := 1 to n - 1 do
  begin
    _txlist.Get_TxUseInCalc(i, b);
    if b then
    begin
      _txlist.Get_Tx(i, tx1); tx1._AddRef; // delphi interface decrements reference when goes off the scope
        {
        Расчитываем ss для передатчика tx1
        }
      ss := GetSS(tx1, tx0);
      tx1 := nil;
      if ss < ssmin then ssmin := ss;
    end;
  end;
  result := ssmin;
end;



{
  wanted_txlist - список планируемых передатчиков (точнее - множество
  вариантов планируемого передатчика). Каждый передатчик из списка по очереди
  анализируется как tx0 рабочего списка.
  На выходе массив [0..1, 0..wanted_txlist.Size-1] of double.
  В нулевой строке массива - критерий качества для соответсвующего
  планируемого передатчика (полщадь зоны обслуживания - interference-limited),
  в первой строке - критерий мешающего воздействия соответствующего планируемого
  передатчика на передатчики рабочего списка (максимальное изменение зоны
  обслуживания работающего передатчика)

  ... не реализовано ...
}
function TCoLISBCCalc.WantedTxAnalysis(const wanted_txlist: ILISBCTxList;
  out resultarray: PSafeArray): HResult;
{
type bds = array[0..1] of SAFEARRAYBOUND;
var tx0: ILISBCTx;
    res: PSafeArray;
    bounds: bds;
    n: integer;
 }
begin
{  bounds[0].lLbound := 0;
  bounds[0].cElements := 2;
  wanted_txlist.Get_Size(n);
  bounds[1].lLbound := 0;
  bounds[1].cElements := n-1;
//  res := SafeArrayCreate(VT_R8, 2, bounds);
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
//  _txlist.Set_Tx(0, tx0);
}
  result := S_OK;

end;



function TCoLISBCCalc.ERPSelection(erp_start,
  erp_finish: Integer): PSafeArray;
type bds = array[0..1] of SAFEARRAYBOUND;
var tx0: ILISBCTx;
    res: PSafeArray;
    bounds: bds;
    erp, j: integer; // erp (dBkW)
    idx: array[0..1] of integer;
    zi, zn: PSafeArray;
    ss0, ss1, maxss0: double;
    polar: TBCPolarization;
begin
  bounds[0].lLbound := 0;
  bounds[0].cElements := 2;
  bounds[1].lLbound := erp_start;
  bounds[1].cElements := erp_finish - erp_start + 1;

  res := SafeArrayCreate(VT_R8, 2, bounds);
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  GetZone_NoiseLimited(tx0, 10, zn);
{
  Нам надо расчитеть максимальное значение параметра ss0,
  чтобы испльзовать его потом для нормализации.
}
  maxss0 := 0;

  for erp := erp_start to erp_finish do
  begin
    tx0.Get_polarization(polar);

    if polar = plHOR then for j := 0 to 35 do tx0.Set_effectpowerhor(j, erp);
    if polar = plVER then for j := 0 to 35 do tx0.Set_effectpowervert(j, erp);

    GetZone_InterferenceLimited(10, zi);
//    ss0 := GetArea(zi) / GetArea(zn);
    ss0 := GetArea(zi);
    if ss0 > maxss0 then maxss0 := ss0;
    ss1 := GetMinSS;
    idx[0] := 0;
    idx[1] := erp;
    SafeArrayPutElement(res, idx, ss0);
    idx[0] := 1;
    SafeArrayPutElement(res, idx, ss1);
  end;
{
  нормализируем все значения ss0
}
  if maxss0 > 0 then
  for erp := erp_start to erp_finish do
  begin
    idx[0] := 0;
    idx[1] := erp;
    SafeArrayGetElement(res, idx, ss0);
    ss0 := ss0 / maxss0;
    SafeArrayPutElement(res, idx, ss0);
  end;
//  resultarray := res;
  result := res;
//  result := S_OK;
end;

procedure TCoLISBCCalc.CalcInterf_Unwanted;
begin
 CalcDuelInterf;
end;


procedure TCoLISBCCalc.CalcInterf_Wanted;
begin
  beep;
  beep;
end;



function GetDistance(lon1, lat1, lon2, lat2: double): double;
var lat, lon: double;
begin
  lat1 := DegToRad(lat1);
  lon1 := DegToRad(lon1);
  lat2 := DegToRad(lat2);
  lon2 := DegToRad(lon2);

  lat:=(lat2 - lat1);
  lon:=(lon2 - lon1) * Cos((lat1 + lat2) / 2);

  result:=ArcCos(Cos(lat) * Cos(lon));
  result:=abs(result * _EARTH_RADIUS);
end;



function TFreeSpacePropag.GetFieldStrength(const Tx: IUnknown; var Path: TRSAGeoPathResults; Perc: TPercentage; out E: Double): HResult; stdcall;
var erp: double;
    az: integer;
    tx0: ILISBCTx;
begin
  E := 106.9 - 20 * Log10 (Path.Distance);
  tx0 := Tx as ILISBCTx;
  az := Trunc(Path.Azimuth/10);
  tx0.Get_erp(az, erp);
  E := E + erp;
  result := S_OK;
end;



function TFreeSpacePropag.Init: HResult; stdcall;
begin
  result := S_OK;
end;



function TCoLISBCCalc.GetFieldStrength(const tx: ILISBCTx; lon,
  lat: Double; perc: Integer): Double;
begin
  if perc > 100 then perc := 100;
  if perc < 1 then perc := 1;
  result := GetE(tx, lon, lat, perc);
//  e := GetE(tx, lon, lat, perc);
//  result := S_OK;
end;


function CorrectFieldStrength(const tx: ILISBCTx): Double;
//var systemcast: TBCTxType;
//    val :double;
//    p : TBCPolarization;
begin
    { Выкинуть это нах
  tx.Get_systemcast(systemcast);
  if systemcast = ttDVB then begin
    tx.Get_polarization(p);
    if (p = plHOR) or (p = plVER) then begin
        tx.get_pol_isol(val);
        result := val;
    end;
  end
  else
  }
    result := 0.0;
end;





procedure TCoLISBCCalc.SetLogFileName(const filename: WideString);
var mode: word;
    dir: string;
    fn: string;
begin
 if _logfile <> nil then
 begin
   _logfile.Free;
   _logfile := nil;
 end;

 if filename <> '' then
 begin
   _debug := true;
   if (ExtractFileExt(filename) <> '.html') and (ExtractFileExt(filename) <> '.htm') then fn := filename + '.html' else fn := filename;

   if FileExists(fn) then mode := fmOpenReadWrite else
   begin
     mode := fmCreate;
     dir := ExtractFilePath(fn);
     if (not DirectoryExists(dir)) and (dir <> '') then CreateNewDirectory(dir);
   end;
   mode := mode or fmShareDenyWrite;
   _logfile := TFileStream.Create(fn, mode);

   AddMessage2('<HTML>');
   AddMessage2('<HEAD>');
   AddMessage2('<TITLE>' + fn + '- LISBCCalc LOG FILE</TITLE>');
   AddMessage2('<META content="text/html; charset=windows-1251" http-equiv=Content-Type>');
   AddMessage2('<BODY>');
   AddMessage2('');

 end else
 begin
  if _logfile <> nil then AddMessage2('</BODY></HTML>');
   _logfile.Free;
   _logfile := nil;
   _debug := false;

   fn := _DEBUG_FILE_NAME;
   if FileExists(fn) then SetLogFileName(fn);
   
 end;
end;



{
  Добавляет сообщение в отладочный файл. Содержит элементы
  HTML - форматирования (в отличие от AddMessage2()).
}
procedure TCoLISBCCalc.AddMessage(s: string);
var str: PChar;
    n: integer;
    htm: string;
begin
  if _logfile = nil then
  begin
    ErrorMessage('TCoLISBCCalc.AddMessage()', 'Error: _logfile = nil');
  end else
  begin
    _logfile.Seek(0, soFromEnd);
    _logfile.Write(#13#10, 2);

{    htm := '<FONT COLOR=' + _message_color_str + '>';
    str := PChar(htm);
    n := Length(htm);
    _logfile.Write(str^, n);
}
    str := PChar(s);
    n := Length(s);
    _logfile.Write(str^, n);

    htm := '<BR>';
    str := PChar(htm);
    n := Length(htm);
    _logfile.Write(str^, n);
  end;
end;



{
Добавляет отладочное сообщение в список _messages. Не содержит форматирования,
добавляет только s и больше ничего.
}
procedure TCoLISBCCalc.AddMessage2(s: string);
var str: PChar;
    n: integer;
begin
  if _logfile = nil then
  begin
    ErrorMessage('TCoLISBCCalc.AddMessage()', 'Error: _logfile = nil');
  end else
  begin
    _logfile.Seek(0, soFromEnd);
    _logfile.Write(#13#10, 2);

    str := PChar(s);
    n := Length(s);
    _logfile.Write(str^, n);
  end;
end;



{
procedure TCoLISBCCalc.AddMessage2(s: string);
begin
 _messages.Add(s);
end;
}




procedure CreateNewDirectory(dir: string);
var newdir: string;
begin
  if not DirectoryExists(dir) then
  begin
    newdir := dir;
    Delete(newdir, Length(newdir), 1);
    newdir := ExtractFilePath(newdir);
    CreateNewDirectory(newdir);
    CreateDir(dir);
  end;
end;



{
  Расчет интеграла вероятности используя аппроксимацию Хастинга
  см. Rec.ITU-R IS.851 par.4.2.2
}
function ProbabilityIntegral(x: double): double;
const
  C1 = 0.319381530;
  C2 = - 0.356563782;
  C3 = 1.781477937;
  C4 = -1.821255978;
  C5 = 1.330274429;
var y, hy, yy, lx: double;
begin
  y := 1 / (1 + 0.2316419 * Abs(x));

  yy := y;     // первая степень y
  hy := C1 * yy;

  yy := yy * y; // вторая степень y
  hy := hy + C2 * yy;

  yy := yy * y;
  hy := hy + C3 * yy;

  yy := yy * y;
  hy := hy + C4 * yy;

  yy := yy * y;
  hy := hy + C5 * yy;

  lx := 1 - (1 / Sqrt(2*PI)) * Exp( - x*x/2) * hy;

  if x < 0 then lx := 1 - lx;

  result := lx;
end;



{
  Расчет используемой напряженности поля в заданной точке
  методом упрощенного перемножения (simplified multiplication method)
     lon, lat    - координаты точки
     probab      - вероятность покрытия (coverage probability) (0..1)
  см. Rec.ITU-R IS.851 par.4.1
}
function TCoLISBCCalc.GetEusableSimplif(lon, lat, probab: double): double;
var i,j,n: integer;
    esi: double;
    tx0, unwanted: ILISBCTx;
    b: wordbool;
    eu, xi, sigma, de: double;
    pc, rec_pc, lxi: double;
    freq: double;
//    it: TBCSInterferenceType;
begin
{
  n - Количество мешающих передатчиков
}
  _txlist.Get_Size(n);
  n := n - 1;
  if (n > 0) then
  begin
    _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
    tx0.get_freq_carrier(freq);
    {
      значение sigma для диапазона I-III равно 8.3
      для остальных диапазонов это значение зависит от
      затухания вблизи поверхн. земли g (terrain attenuation):
      sigma = 9.5 + 0.405*g.  Значение g определяют из DELTA_H (rec. p-370)
      При DELTA_H = 50 => g = 0;
    }
    if freq < 450 then sigma := 8.3 else sigma := 9.5;

        //SortTxList(lon, lat);
      eu := 0;
      de := 0;
      repeat
        eu := eu + de;
        i := 1;   // начинаем отсчет с первого индекса (индекс 0 - планируемый перед. 1,2,3... - мешающие)
        j := 0;
        pc := 1;
        rec_pc := probab;
        while (j < 20)  and (i < n+1) do
        begin
          _txlist.Get_Tx(i, unwanted); unwanted._AddRef; // delphi interface decrements reference when goes off the scope
          _txlist.Get_TxUseInCalc(i, b);
          if b then
          begin
            esi := GetEControlPoint(tx0, unwanted, lon, lat, nil);
            {
            unwanted.Get_station_name(s);
            ShowMessage('#'+inttostr(i)+'  ' +s);
            }
            {
              Инициализируем значение eu при первом проходе. Рекомендуемое
              начальное значение примерно на 6 дБ выше максимальной помехи.
              Если наш список отсортирован по помехе нам, то первый мешающий
              передатчик в списке : j=0 : и будет создавать макс. помеху.
            }
            if eu = 0 then eu := esi + 6;

            xi := (eu - esi) / (sigma * Sqrt(2));
            lxi := ProbabilityIntegral(xi);
            pc := pc * lxi;
            j := j + 1;
          end;
          i := i + 1;
          unwanted := nil;
        end;
        de := (rec_pc - pc) / 0.05
    {
      Если в списке помех ни оди передатчик не участвует в расчете
      - получается бесконечный цикл, поэтому добавил
      " or (eu < -99)" в следующей строке
    }
      until (Abs(de) < _PREC) or (eu < -99);
      if eu < -99 then result := NO_INTERFERENCE else result := eu;
  end else
    result := NO_INTERFERENCE;
end;



function TCoLISBCCalc.GetEusablePowerSum(lon, lat: double): double;
begin
  result := GetSumEControlPoint(lon, lat);
end;



function TCoLISBCCalc.GetAzimuthDeg(lon1, lat1, lon2, lat2: double): double;
var a,b: TRSAGeoPoint;
    az: double;
begin
  if _spherics <> nil then
  begin
    a.H := lat1;
    a.L := lon1;
    a.Alt := 0;
    b.H := lat2;
    b.L := lon2;
    b.Alt := 0;
    _spherics.Azimuth(a,b, az);
    result := az;
  end else
  begin
    az := GetAzimuth(lon1, lat1, lon2, lat2);
    if az < 0 then az := az + 2 * PI;
    result := RadToDeg(az);
  end;
end;



function TCoLISBCCalc.GetDistanceKm(lon1, lat1, lon2,
  lat2: double): double;
var a,b: TRSAGeoPoint;
    d: double;
begin
  if _spherics <> nil then
  begin
    a.H := lat1;
    a.L := lon1;
    a.Alt := 0;
    b.H := lat2;
    b.L := lon2;
    b.Alt := 0;
    _spherics.Distance(a, b, d);
    result := d;
  end else
  begin
    result := GetDistance(lon1, lat1, lon2, lat2);
  end;
end;



procedure TCoLISBCCalc.GetNextCoordDeg(azimuth, distance: double; var lon, lat: double);
var a,b: TRSAGeoPoint;
    az: double;
begin
  if _spherics <> nil then
  begin
    a.H := lat;
    a.L := lon;
    a.Alt := 0;
    _spherics.PolarToGeo(distance, azimuth, a, b);
    lon := b.L;
    lat := b.H;
  end else
  begin
    if azimuth > 180 then azimuth := azimuth - 360;
    az := DegToRad(azimuth);
    GetNextCoord(az, distance, lon, lat);
  end;
end;



function TCoLISBCCalc.CheckE(azimuth, d: double): double;
begin
  case _calc_method of
    cmPowerSum: result := CheckEPowerSum(azimuth, d);
    cmSimplified: result := CheckESimplif(azimuth, d);
    cmChester: result := CheckEChester(azimuth, d);
  else
    result := CheckEPowerSum(azimuth, d);
  end;
end;



function TCoLISBCCalc.Get_CalcMethod: TCalcMethod;
begin
  result := _calc_method;
end;



procedure TCoLISBCCalc.Set_CalcMethod(Value: TCalcMethod);
begin
  if _calc_method <> Value then _calc_method := Value;
{
  IF Value = cmSimplified THEN
  BEGIN
    ShowMessage('Будет включен режим расчета Еисп. ' + #13#10 + 'согласно соглашению Честер97 ');
    _calc_method := cmChester;
  END;
}
end;



function TCoLISBCCalc.Get_CoverageProbability: Double;
begin
  result := _cover_probability;

end;

procedure TCoLISBCCalc.Set_CoverageProbability(Value: Double);
begin
 if _cover_probability <> Value then
 begin
   _cover_probability := Value;
   if _cover_probability < 0 then _cover_probability := 0;
   if _cover_probability > 1 then _cover_probability := 1;
 end;

end;



{
  Расчет радиуса зоны покрытия с заданной точностью (шагом)
  по дистанции <step: double>

  tx_noise_limited на самом деле просто задает, какую зону мы расчитываем:
    ILISBCTx  - для расчета зоны, ограниченной шумами (теоретической зоны)
    nil - для расчета зоны, ограниченной помехами
}
function TCoLISBCCalc.GetMaxDistancePrec(d, azimuth, step_init, step_final: double; tx_noise_limited: ILISBCTx): double;
var de_old: double;
    de: double;
    step: double;
begin
  step_init := Abs(step_init);
  step_final := Abs(step_final);

  if step_init < step_final then step_init := step_final;

  if d < _MIN_DISTANCE then d := _MIN_DISTANCE;
  step := step_init;

  if tx_noise_limited = nil then de := CheckE(azimuth, d) else de := CheckEmin(tx_noise_limited, azimuth, d);
  repeat
    de_old := de;
//    showmessage(FloatToStr(azimuth) + ' ' + FloatToStr(d));
      if tx_noise_limited = nil then de := CheckE(azimuth, d) else de := CheckEmin(tx_noise_limited, azimuth, d);
{
  Если de положительный - prec тоже положительный
  (т.е. "шагаем" вперед), иначе шаг отрицательный
}
    if de <> 0 then step := Abs(step) * de / Abs(de);
    d := d + step;
  until ((de * de_old) <= 0) or (d < _MIN_DISTANCE);
{
  Теперь уточняем шаг и рекурсивно вызываем сами себя
}
  if Abs(step_init) > Abs(step_final) then
  begin
     step_init := step_init / 5;
     d := Max(d - step, 0);
     d := Self.GetMaxDistancePrec(d, azimuth, step_init, step_final, tx_noise_limited);
  end;

//  if d < 0 then result := 0 else result := d;

  result := Max(d, _MIN_DISTANCE);
end;



{
  Эта функция аналогична обычной, но при отключенном клиренсе приемника
  должна работать быстрее. При включенном клиренсе может работать
  неправильно.
  17.05.2004.  - При включенном клиренсе тоже должна работать!
}
function TCoLISBCCalc.GetMaxDistancePrec_Quick(d, azimuth, step_init, step_final: double; tx_noise_limited: ILISBCTx): double;
const _DIVIDER = 10;
var de: double;
    step: double;
    step_old: double;

begin
  result := GetMaxDistancePrec(d, azimuth, step_init, step_final, tx_noise_limited);
  Exit;


  if d < _MIN_DISTANCE then d := _MIN_DISTANCE;
  step := 1e+14;
  repeat
    if tx_noise_limited = nil then de := CheckE(azimuth, d) else de := CheckEmin(tx_noise_limited, azimuth, d);
{
  Если de положительный - step тоже положительный
  (т.е. "шагаем" вперед), иначе шаг отрицательный
}
    step_old := step;
{
  Определяем размер шага. Проверяем, чтобы шаг не вывел нас из
  цикла на первой же итерации. Например при большом абс. значении de
  шаг оказывается большим и в первой итерации дистанция d уже отрицательная
}
    if Abs(de) > _DIVIDER then
      if de < 0 then de := -(_DIVIDER-1) else de := (_DIVIDER-1);

    step := de * d/_DIVIDER;
{
  если поменяли направление, т.е. старый и новый шаг
  имеют разные знаки, то следим, чтобы абс. знач. нового
  шага было меньше старого.
}
    if (Abs(step) >= Abs(step_old)) then
    begin
      if (step_old * step < 0) then step :=  - step_old * 0.5 else step :=  step_old;
    end;

    d := d + step;
  until (Abs(de) < 0.1) or (d < _MIN_DISTANCE/100)  or ((step_old * step < 0) and (Abs(step) <= step_final));

  result := Max(d, _MIN_DISTANCE);
end;


{
procedure TCoLISBCCalc.ShowDebugMessages;
var s, filename: string;
   cmd: PChar;
   temp: PChar;
   buffer: array [0..256] of byte;
begin
  temp := @buffer;
  GetTempPath(256, temp);
  filename := temp;
  filename := filename + 'lis-debug' + IntToStr(Random(1000000000))+'.tmp';
  s := 'Notepad.exe '+ filename;
  cmd := PChar(s);
  _messages.SaveToFile(filename);
  WinExec(cmd , SW_MAXIMIZE);
end;
}

{
procedure TCoLISBCCalc.GetEuDeviation(nl_zone, il_zone: PSafeArray; var eu_values: PSafeArray);
var i: integer;
    lon, lat, az, d_nl, d_il: double;
    array_nl:P36ValuesArray;
    array_il:P36ValuesArray;
    array_eu:P36ValuesArray;

    eu_nl, eu_il, delta_eu: double;

    s: string;
    id: integer;
    tx0: ILISBCTx;
begin
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  _messages.Clear;

  array_nl := @nl_zone;
  array_il := @il_zone;
  array_eu := @eu_values;

  tx0.Get_id(id);

  s := 'TX_ID = ' + IntToStr(id);
  _messages.Add(s);

  s := 'eu_nl';
  s := s + #09 + 'eu_il';
  s := s + #09 + 'delta_eu';
  _messages.Add(s);

  for i := 0 to 35 do
  begin
    az := i * 10;
    d_nl := array_nl^[i];
    d_il := array_il^[i];

    tx0.Get_longitude(lon);
    tx0.Get_latitude(lat);
    GetNextCoordDeg(az, d_nl, lon, lat);
    eu_nl := GetUsableFieldStrength(lon, lat);

    tx0.Get_longitude(lon);
    tx0.Get_latitude(lat);
    GetNextCoordDeg(az, d_il, lon, lat);
    eu_il := GetUsableFieldStrength(lon, lat);

    delta_eu := eu_nl - eu_il;
    array_eu^[i] := delta_eu;

    s := FormatFloat('0.00', eu_nl);
    s := s + #09 + FormatFloat('0.00', eu_il);
    s := s + #09 + FormatFloat('0.00', delta_eu);
    AddMessage2(s);
  end;

  if MessageDlg('Вывести информацию об изменении Еисп для TX_ID ' + IntToStr(id), mtConfirmation, [mbYes, mbNo], 0) = mrYes then
  begin
    ShowDebugMessages;
  end;
end;

}



function TCoLISBCCalc.GetEmin(const tx: ILISBCTx): Double;
var f: double;
    sys: TBCTvSystems;
    tx_type: TBCTxType;
    i: integer;
    band: byte;
begin
{
  Эту функцию надо модифицировать с учетом типа передатчика.
  Эти значения Емин. верны для АТВ, но не для ОВЧ ЧМ или ЦТВ...
}
  tx.Get_systemcast(tx_type);

  case tx_type of
    ttTV:
      begin
        tx.Get_video_carrier(f);
        band := FreqToBand(f);
        case band of
          1: result := 48;
          2: result := 52;
          3: result := 55;
          4: result := 65;
          5: result := 70;
        else
          result := 48;
        end;
        if (band = 4) or (band = 5) then
        begin
          tx.Get_typesystem(sys);
          if sys = tvK then result := result + 2;
        end;
      end;
    ttFM:
      begin
{
        Это эначения для сельской местности
        Geneva84 Annex 2 3.6
}
        tx.Get_monostereo_primary(i);
        if i = 0 then result := 48 else result := 54;
      end;
    ttDVB:
      begin
        result := EminDVB(tx);
      end;
    ttDAB:
      begin
        result := EminDAB(tx);
      end;
    ttFxm:
      begin
        result := EminFXM(tx);
      end;
    else
      result := 48;
    end;

end;



{
Здесь азимут в градусах
}
function TCoLISBCCalc.GetAntennaDiscrimination(f, angle: Double): Double;
var band: byte;
begin
  band := FreqToBand(f);

  result := UAntenna.GetAntennaDiscriminationDeg(band, angle, 0);
end;



function TCoLISBCCalc.GetMaxRadiusEmin(const tx: ILISBCTx; d_initial,
  azimuth, emin: Double): Double;
begin
  result := GetMaxDistanceEmin(tx, d_initial, azimuth, emin);
end;



{
  Метод < CalcDuelInterf > для всех
  передатчиков в выборке рассчитывает:
  1. дуэльные помехи НАМ; (UnWantedInterf)
  2. дуэльные помехи ОТ НАС; (WantedInterf)
  3. тип помехи НАМ;
  4. тип помехи ОТ НАС;
  5. расстояние до основного передатчика;
  6. азимут с основного передатчика на мешающий;
  7. величину перекрытия теоретических зон основного и
     мешающего передатчика, км. (если <= 0, то зоны не перекрываются,
     если > 0, зоны перекрываются).
}
procedure TCoLISBCCalc.CalcDuelInterf;
begin
 if _quick_calc_duel_interf then CalcDuelInterfQuick else CalcDuelInterfSlow;
end;


{
  Быстрый расчет дуэльных помех.
  Параметр указывает, нужно ли расчитывать пересечения зон покрытия.
}
procedure TCoLISBCCalc.CalcDuelInterfQuick;
var i: integer;
    tx1: ILISBCTx;
    tx0: ILISBCTx;
    lon0, lat0, lon1, lat1: double;
    e1: double;
    e0: double;
    d, az: double;
    it1: TBCSInterferenceType;
    it0: TBCSInterferenceType;
    n: integer;
    cp_res0: TControlPointResult;
    cp_res1: TControlPointResult;
    perc: integer;
begin

{
  Устанавливаем большое значение помехи (999999), чтобы
  нулевой передатчик остался первым в списке при сортировке
}
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope

  _txlist.Set_TxWantInterfere(0, 999999);
  _txlist.Set_TxUnWantInterfere(0, 999999);

  _txlist.Get_Size(n);

  for i := 1 to n-1 do
  begin
    _txlist.Get_Tx(i, tx1); tx1._AddRef; // delphi interface decrements reference when goes off the scope

    tx0.Get_longitude(lon0);
    tx0.Get_latitude(lat0);

    tx1.Get_longitude(lon1);
    tx1.Get_latitude(lat1);
    {
      Считаем помеху от Tx1 в точке установки Tx0  (помеха НАМ)
    }
    e0 := GetEControlPoint(tx0, tx1, lon0, lat0, @cp_res0);
    {
      Считаем помеху от Tx0 в точке установки Tx1 (помеха от НАС)
    }
    e1 := GetEControlPoint(tx1, tx0, lon1, lat1, @cp_res1);

    it0 := cp_res0.int_type;
    it1 := cp_res1.int_type;

    _txlist.Set_TxUnWantInterfere(i, e0);
    _txlist.Set_TxUnwantedKind(i, it0);

    _txlist.Set_TxWantInterfere(i, e1);
    _txlist.Set_TxwantedKind(i, it1);

    if ZonesOverlapping(tx0, tx1, d, az)
      then _txlist.Set_TxZoneOverlapping(i, 1)
      else _txlist.Set_TxZoneOverlapping(i, -1);

    _txlist.Set_TxDistance(i, d);
    _txlist.Set_TxAzimuth(i, az);

    perc := Round(100 * i / n);
    Fire_Progress(perc);
    tx1 := nil;
    if perc > 100 then Exit;
  end;
end;


{
function GetAzimuthDeg(lon1, lat1, lon2, lat2: double): double;
begin
  result := RadToDeg(GetAzimuth(lon1, lat1, lon2, lat2));
  if result < 0 then result := result + 360;
end;

}

{
  Расчитывает дуэль двух передатчиков.
}
procedure TCoLISBCCalc.CalcDuel3(const tx0, tx1: ILISBCTx; var pointDuelResult: TPointDuelResultArray);
var az_to_want: double;
    az_to_unwant: double;
    lon, lat, lon0, lat0, lon1, lat1, d: double;

    d1: double;
    d2: double;
    d3: double;
    d4: double;

    e1: double;
    e2: double;
    e3: double;
    e4: double;

    cpr: TControlPointResult;
begin


    tx0.Get_longitude(lon0);
    tx0.Get_latitude(lat0);

    tx1.Get_longitude(lon1);
    tx1.Get_latitude(lat1);
{ **********************************************
    txl := CoLISBCTxList.Create;
************************************************}

{
  Теперь расчитываем азимуты в градусах
}
      az_to_unwant := GetAzimuthDeg(lon0, lat0, lon1, lat1);
      az_to_want := GetAzimuthDeg(lon1, lat1, lon0, lat0);
///      if az_to_unwant > 180 then az_to_want := az_to_unwant - 180 else az_to_want := az_to_unwant + 180 ;

{
  Расчитываем расстояния до границы зон:

            tx0(wanted)                  tx1(unwanted)
  d1 --------x-------- d2        d3 ------x------ d4
}
      d := 30;
      d1 := GetMaxDistanceWithOutInterferences(tx0, d, az_to_want);
      d2 := GetMaxDistanceWithOutInterferences(tx0, d1, az_to_unwant);
      d3 := GetMaxDistanceWithOutInterferences(tx1, d, az_to_want);
      d4 := GetMaxDistanceWithOutInterferences(tx1, d3, az_to_unwant);

      pointDuelResult.point1.radius := d1;
      pointDuelResult.point2.radius := d2;
      pointDuelResult.point3.radius := d3;
      pointDuelResult.point4.radius := d4;

{
  Расчитываем напряж. поля помехи в точках 1,2,3,4. Если радиус зоны
  меньше 3 км, то напряженность расчитываем в точке установки передатчика.

  Изменения 05.05.04:
    1. напряженность по-любому расчитываем на границе зоны
    2. если зона меньше километра - делаем ее равной 1 км.
}


      lon := lon0;
      lat := lat0;
//      if d1 > 3 then GetNextCoordDeg(az_to_want, d1, lon, lat);
      if d1 < _MIN_DISTANCE then d1 := _MIN_DISTANCE;
      GetNextCoordDeg(az_to_want, d1, lon, lat);
      e1 := GetEControlPoint(tx0, tx1, lon, lat, @cpr);
      pointDuelResult.point1.eInt := cpr.e_int;
      pointDuelResult.point1.aPR := cpr.a_pr;
      pointDuelResult.point1.aDiscr := cpr.a_discr;
      pointDuelResult.point1.aPolar := cpr.a_polar;
      pointDuelResult.point1.intType := cpr.int_type;
      pointDuelResult.point1.azimuth := GetAzimuthDeg(lon0, lat0, lon, lat);
{ **********************************************
      txl.Clear;
      txl.AddTx(tx0, idx);
      txl.AddTx(tx1, idx);
      txl_old := _txlist;
      SetTxListServer(txl);
      try
        e1 := GetUsableFieldStrength(lon, lat);
        SetTxListServer(txl_old);
      finally
        SetTxListServer(txl_old);
      end;
}
      pointDuelResult.point1.eUsable := e1;
      pointDuelResult.point1.geoPoint.H := lat;
      pointDuelResult.point1.geoPoint.L := lon;


      lon := lon0;
      lat := lat0;
//      if d2 > 3 then GetNextCoordDeg(az_to_unwant, d2, lon, lat);
      if d2 < _MIN_DISTANCE then d2 := _MIN_DISTANCE;
      GetNextCoordDeg(az_to_unwant, d2, lon, lat);
      e2 := GetEControlPoint(tx0, tx1, lon, lat, @cpr);
      pointDuelResult.point2.eInt := cpr.e_int;
      pointDuelResult.point2.aPR := cpr.a_pr;
      pointDuelResult.point2.aDiscr := cpr.a_discr;
      pointDuelResult.point2.aPolar := cpr.a_polar;
      pointDuelResult.point2.intType := cpr.int_type;
      pointDuelResult.point2.azimuth := GetAzimuthDeg(lon0, lat0, lon, lat);
{ **********************************************
      txl.Clear;
      txl.AddTx(tx0, idx);
      txl.AddTx(tx1, idx);
      txl_old := _txlist;
      SetTxListServer(txl);
      try
        e2 := GetUsableFieldStrength(lon, lat);
        SetTxListServer(txl_old);
      finally
        SetTxListServer(txl_old);
      end;
}
      pointDuelResult.point2.eUsable := e2;
      pointDuelResult.point2.geoPoint.H := lat;
      pointDuelResult.point2.geoPoint.L := lon;

      lon := lon1;
      lat := lat1;
//      if d3 > 3 then GetNextCoordDeg(az_to_want, d3, lon, lat);
      if d3 < _MIN_DISTANCE then d3 := _MIN_DISTANCE;
      GetNextCoordDeg(az_to_want, d3, lon, lat);
      e3 := GetEControlPoint(tx1, tx0, lon, lat, @cpr);
      pointDuelResult.point3.eInt := cpr.e_int;
      pointDuelResult.point3.aPR := cpr.a_pr;
      pointDuelResult.point3.aDiscr := cpr.a_discr;
      pointDuelResult.point3.aPolar := cpr.a_polar;
      pointDuelResult.point3.intType := cpr.int_type;
      pointDuelResult.point3.azimuth := GetAzimuthDeg(lon1, lat1, lon, lat);
{ **********************************************
      txl.Clear;
      txl.AddTx(tx1, idx);
      txl.AddTx(tx0, idx);
      txl_old := _txlist;
      SetTxListServer(txl);
      try
        e3 := GetUsableFieldStrength(lon, lat);
        SetTxListServer(txl_old);
      finally
        SetTxListServer(txl_old);
      end;
}
      pointDuelResult.point3.eUsable := e3;
      pointDuelResult.point3.geoPoint.H := lat;
      pointDuelResult.point3.geoPoint.L := lon;

      lon := lon1;
      lat := lat1;
//      if d4 > 3 then GetNextCoordDeg(az_to_unwant, d4, lon, lat);
      if d4 < _MIN_DISTANCE then d4 := _MIN_DISTANCE;
      GetNextCoordDeg(az_to_unwant, d4, lon, lat);
      e4 := GetEControlPoint(tx1, tx0, lon, lat, @cpr);
      pointDuelResult.point4.eInt := cpr.e_int;
      pointDuelResult.point4.aPR := cpr.a_pr;
      pointDuelResult.point4.aDiscr := cpr.a_discr;
      pointDuelResult.point4.aPolar := cpr.a_polar;
      pointDuelResult.point4.intType := cpr.int_type;
      pointDuelResult.point4.azimuth := GetAzimuthDeg(lon1, lat1, lon, lat);
{ **********************************************
      txl.Clear;
      txl.AddTx(tx1, idx);
      txl.AddTx(tx0, idx);
      txl_old := _txlist;
      SetTxListServer(txl);
      try
        e4 := GetUsableFieldStrength(lon, lat);
        SetTxListServer(txl_old);
      finally
        SetTxListServer(txl_old);
      end;
}
      pointDuelResult.point4.eUsable := e4;
      pointDuelResult.point4.geoPoint.H := lat;
      pointDuelResult.point4.geoPoint.L := lon;

      pointDuelResult.point1.eMin := GetEmin(tx0);
      pointDuelResult.point2.eMin := pointDuelResult.point1.eMin;
      pointDuelResult.point3.eMin := GetEmin(tx1);
      pointDuelResult.point4.eMin := pointDuelResult.point3.eMin;


end;



procedure TCoLISBCCalc.CalcDuelInterfSlow;
var i: integer;
    tx1: ILISBCTx;
    tx0: ILISBCTx;
    lon0, lat0, lon1, lat1, az, d: double;
    d2: double;
    d3: double;
    e1: double;
    e2: double;
    e3: double;
    e4: double;
    it1: TBCSInterferenceType;
    it2: TBCSInterferenceType;
    it3: TBCSInterferenceType;
    it4: TBCSInterferenceType;
    n: integer;
    pointDuelResult: TPointDuelResultArray;
    perc: integer;
begin

{
  Устанавливаем большое значение помехи (999999), чтобы
  нулевой передатчик остался первым в списке при сортировке
}
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope

  _txlist.Set_TxWantInterfere(0, 999999);
  _txlist.Set_TxUnWantInterfere(0, 999999);

  _txlist.Get_Size(n);

  for i := 1 to n-1 do
  begin
    _txlist.Get_Tx(i, tx1); tx1._AddRef; // delphi interface decrements reference when goes off the scope

    tx0.Get_longitude(lon0);
    tx0.Get_latitude(lat0);

    tx1.Get_longitude(lon1);
    tx1.Get_latitude(lat1);

    CalcDuel3(tx0, tx1, pointDuelResult);

    it1 := pointDuelResult.point1.intType;
    it2 := pointDuelResult.point2.intType;
    it3 := pointDuelResult.point3.intType;
    it4 := pointDuelResult.point4.intType;

    e1 := pointDuelResult.point1.eUsable;
    e2 := pointDuelResult.point2.eUsable;
    e3 := pointDuelResult.point3.eUsable;
    e4 := pointDuelResult.point4.eUsable;

    d2 := pointDuelResult.point2.radius;
    d3 := pointDuelResult.point3.radius;

    {
      Расчитываем расстояние между передатчиками
    }
      d := GetDistanceKm(lon1, lat1, lon0, lat0);
      _txlist.Set_TxDistance(i, d);

      az := GetAzimuthDeg(lon0, lat0, lon1, lat1);
      _txlist.Set_TxAzimuth(i, az);

    {
      Рассчитываем перекрытие зон
    }
      d := d2 + d3 - d;
      _txlist.Set_TxZoneOverlapping(i, d);


      if e1 > e2 then
      begin
        _txlist.Set_TxUnWantInterfere(i, e1);
        _txlist.Set_TxUnwantedKind(i, it1);
      end else
      begin
        _txlist.Set_TxUnWantInterfere(i, e2);
        _txlist.Set_TxUnwantedKind(i, it2);
      end;

      if e3 > e4 then
      begin
        _txlist.Set_TxWantInterfere(i, e3);
        _txlist.Set_TxwantedKind(i, it3);
      end else
      begin
        _txlist.Set_TxWantInterfere(i, e4);
        _txlist.Set_TxwantedKind(i, it4);
      end;

      perc := Round(100 * i / n);
      Fire_Progress(perc);

      tx1 := nil;

      if perc > 100 then Exit;
  end;
end;



{
  Проверяет, пересекаются ли теоретические зоны передатчиков tx0, tx1.
}
function TCoLISBCCalc.ZonesOverlapping(tx0, tx1: ILISBCTx; var distance, azimuth: double): boolean;
var d, az0 ,az1: double;
    d0, d1, dd: double;
    lon0, lat0: double;
    lon1, lat1: double;
    emin0, emin1, e0, e1: double;
    quit: boolean;
begin
  tx0.Get_longitude(lon0);
  tx0.Get_latitude(lat0);
  tx1.Get_longitude(lon1);
  tx1.Get_latitude(lat1);

  d := GetDistanceKm(lon0, lat0, lon1, lat1);
  if d < 0.1 then
  begin
    result := true;
    distance := 0;
    azimuth := 0;
    Exit;
  end;

  distance := d;

  az0 := GetAzimuthDeg(lon0, lat0, lon1, lat1);
  az1 := GetAzimuthDeg(lon1, lat1, lon0, lat0);
  azimuth := az0;

  emin0 := GetEmin(tx0);
  emin1 := GetEmin(tx1);

  dd := d / 2;
  d0 := dd;
  d1 := d - d0;

  quit := false;

  result := false;

  repeat
    e0 := GetE_Azimuth(tx0, az0, d0, prFifty);
    e1 := GetE_Azimuth(tx1, az1, d1, prFifty);

    if (e0 >= emin0) and (e1 >= emin1) then
    begin
      result := true;
      quit := true;
    end else

        if (e0 < emin0) and (e1 < emin1) then
        begin
          result := false;
          quit := true;
        end else

            if (e0 > emin0) and (e1 < emin1) then
            begin
              dd := dd / 2;
              d0 := d0 + dd;
              d1 := d - d0;
              if d1 <= _MIN_DISTANCE then
              begin
                result := true;
                Exit;
              end;
            end else

                if (e0 <= emin0) and (e1 >= emin1) then
                begin
                  dd := dd / 2;
                  d0 := d0 - dd;
                  d1 := d - d0;
                  if d0 <= _MIN_DISTANCE then
                  begin
                    result := true;
                    Exit;
                  end;
                end;
  until quit;
end;



procedure TCoLISBCCalc.LoadParamsFromRegistry;
var reg: TStringRegistry;
begin
  reg := TStringRegistry.Create;
  reg.RootKey := HKEY_CURRENT_USER;
  reg.OpenKey('\Software\LIS\LISBCCalc', True);
{
  _emin_dvb_200 := reg.ReadStringFloat('_emin_dvb_200', 38.5);
  _emin_dvb_500 := reg.ReadStringFloat('_emin_dvb_500', 42.6);
  _emin_dvb_700 := reg.ReadStringFloat('_emin_dvb_700', 46.6);
}
  _emin_dvb_200 := reg.ReadFloat('_emin_dvb_200');
  _emin_dvb_500 := reg.ReadFloat('_emin_dvb_500');
  _emin_dvb_700 := reg.ReadFloat('_emin_dvb_700');
{
  _dvb_antenna_discrimination := reg.ReadStringBool('_dvb_antenna_discrimination', true);
  _quick_calc_duel_interf := reg.ReadStringBool('_quick_calc_duel_interf', true);

  _dvb_antenna_discrimination := reg.ReadBool('_dvb_antenna_discrimination');
}  _quick_calc_duel_interf := reg.ReadBool('_quick_calc_duel_interf');
  _coord_dist_ini_file := reg.ReadString('_coord_dist_ini_file');

{
  UAntenna._back_lobe_fm_mono := reg.ReadStringFloat('_back_lobe_fm_mono', -6);
  UAntenna._back_lobe_fm_stereo := reg.ReadStringFloat('_back_lobe_fm_stereo', -12);
  UAntenna._back_lobe_tv_band2 := reg.ReadStringFloat('_back_lobe_tv_band2', -6);
  UAntenna._polar_correct_fm := reg.ReadStringFloat('_polar_correct_fm', -10);
}
  UAntenna._back_lobe_fm_mono := reg.ReadFloat('_back_lobe_fm_mono');
  UAntenna._back_lobe_fm_stereo := reg.ReadFloat('_back_lobe_fm_stereo');
  UAntenna._back_lobe_tv_band2 := reg.ReadFloat('_back_lobe_tv_band2');
  UAntenna._polar_correct_fm := reg.ReadFloat('_polar_correct_fm');


{
  Параметр _rx_clearance_enabled включает быстрый расчет макс. радиуса зоны.
  Бастрый расчет работает только если не учитываются углы закытия в
  месте устаноки приемника.
}
  _quick_calc_max_dist := reg.ReadBool('_quick_calc_max_dist');

  _request_for_coord_dist := reg.ReadBool('_request_for_coord_dist');

  if reg.ValueExists('_step_calc_max_dist')
    then _STEP_FINAL := reg.ReadFloat('_step_calc_max_dist')
    else _STEP_FINAL := 0.1;

  reg.OpenKey('\Software\LIS\BC\CalcParams', True);
  _quick_calc_max_dist := _quick_calc_max_dist and not reg.ReadBool('UseRxClearance');

  _quick_calc_max_dist := FALSE;

end;


{
  Расчет координационного расстояния для передатчика
}
{
function TCoLISBCCalc.GetTxCoordDistance(tx: ILISBCTx; azimuth: double): double;
var
    dist, f, erp:  double;
    band, h, heff, seaperc: integer;
    p1: TRSAGeoPoint;
    geodata: TRSAGeoPathData;
    path: TRSAGeoPathResults;
    plon, plat: double;
    delta_dist, dist2: double;
    tx_type: TBCTxType;
    is_TV: boolean;
begin
  tx.get_freq_carrier(f);
  band := FreqToBand(f);
  tx.get_h_eff(Round(azimuth), heff);
  tx.get_erp(Round(azimuth), erp);

//  Сначала расчитываем КР для суши,
//  а затем будем проверять на наличие воды на трассе

  seaperc := 0;

  tx.Get_systemcast(tx_type);
  is_TV := (tx_type = ttTV) or (tx_type = ttDVB);

  _cd.GetCoordDist(erp, band, heff, seaperc, is_TV, dist);

  tx.Get_longitude(plon);
  tx.Get_latitude(plat);
  p1.H := plat;
  p1.L := plon;
  p1.Alt := 0;

  if _relief <> nil then
  begin
    tx.Get_heightantenna(h);
    geodata.TxHeight := h;
    geodata.RxHeight := _RX_ANTENNA_HEIGHT;
//    OleCheck(NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
    (NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
    if IsNaN(path.HEff) then path.HEff := heff;
    delta_dist := 9999999;

    while (Abs(delta_dist) >= 1) and (path.SeaPercent >= 10) do
    begin
      dist2 := dist;
//      OleCheck(NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
      (NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
      if IsNaN(path.HEff) then path.HEff := heff;
      _cd.GetCoordDist(erp, band, Round(path.Heff), Round(path.SeaPercent), is_TV, dist);
      delta_dist := dist2 - dist;
    end;
  end;

  result := dist;
end;

}


procedure TCoLISBCCalc.GetCoordinationZone(const tx: ILISBCTx; out zone_km: PSafeArray);
var t: TBCTxType;
    f: double;
begin
  tx.Get_systemcast(t);
  tx.get_freq_carrier(f);
  if (t = ttTV) and (f >= 174) then
    GetCoordinationZone_GE06(tx, zone_km)
  else if (t = ttDVB) or (t = ttDAB) then
    GetCoordinationZone_GE06(tx, zone_km)
  else
    GetCoordinationZone_OLD(tx, zone_km);
end;


procedure TCoLISBCCalc.GetCoordinationZone_GE06(const tx: ILISBCTx;
  out zone_km: PSafeArray);
var
    d, e_trigger: double;
    i, k, perc: integer;
    a_deg: integer;
    s: string;
    idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    mr: TModalResult;

begin

  bounds.cElements := 36;
  bounds.lLbound := 0;
  zone_km := SafeArrayCreate(varDouble, 1, bounds);

  s := 'Розрахувати координаційний контур?';
  if _request_for_coord_dist
    then mr := MessageDlg(s, mtConfirmation, [mbYes, mbCancel], 0)
    else mr := mrYes;

  if mr = mrCancel then
  begin
    d := 1;
    for i := 0 to 35 do
    begin
      idx[0] := i;
      SafeArrayPutElement(zone_km, idx, d);
    end;
    Exit;
  end;


//  d := GetTxCoordDistance3(tx, 0, @res, use_max_parameters);

  e_trigger := _coordFieldStrength;

  if (e_trigger  < -MaxDouble )
    then
        e_trigger := GetEmin_trigger(tx);

  d := GetMaxDistanceEmin_GE06(tx, 0, e_trigger);
  idx[0] := 0;
  SafeArrayPutElement(zone_km, idx, d);
  perc := Round(100 / 36);
  Fire_Progress(perc);
  for i := 1 to 35 do
  begin
    a_deg := i * 10;
    d := GetMaxDistanceEmin_GE06(tx, a_deg, e_trigger);
    
    idx[0] := i;
    SafeArrayPutElement(zone_km, idx, d);
    perc := Round(i*100 / 36);
    Fire_Progress(perc);
    if perc > 100 then
    begin
      for k := i+1 to 35 do
      begin
       idx[0] := k;
       d := 0;
       SafeArrayPutElement(zone_km, idx, d);
      end;
      Break;
    end;
  end;
end;

procedure TCoLISBCCalc.GetCoordinationZone_OLD(const tx: ILISBCTx;
  out zone_km: PSafeArray);
var ///////////////////pzonearray:P36ValuesArray;
    d: double;
    i, k, perc: integer;
    a_deg: integer;
    s, mess: string;
    res: TCoordDistResult;
    idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    use_max_parameters: boolean;
    mr: TModalResult;

begin

  bounds.cElements := 36;
  bounds.lLbound := 0;
  zone_km := SafeArrayCreate(varDouble, 1, bounds);

//////////////////  pzonearray := @zone;

  s := 'Можливі два варіанти розрахунку координаційного контура:' + #10#13;
  s := s + 'а) для максимальних значень Неф. та ЕВП;' + #10#13;
  s := s + 'б) для значень Неф. та ЕВП за азимутом.' + #10#13;
  s := s + #10#13;
  s := s + 'Ви бажаєте використовувати лише М А К С И М А Л Ь Н І значення Неф. та ЕВП?';
//  use_max_parameters := (MessageDlg(s, mtConfirmation, [mbYes, mbNo], 0) = mrYes);

  if _request_for_coord_dist
    then mr := MessageDlg(s, mtConfirmation, [mbYes, mbNo, mbCancel], 0)
    else mr := mrYes;

  if mr = mrCancel then
  begin
    d := 1;
    for i := 0 to 35 do
    begin
      idx[0] := i;
      SafeArrayPutElement(zone_km, idx, d);
    end;
    Exit;
  end;


  use_max_parameters := (mr = mrYes);

  d := GetTxCoordDistance3(tx, 0, @res, use_max_parameters);
  idx[0] := 0;
  SafeArrayPutElement(zone_km, idx, d);

  if _logfile <> nil then
  begin
    s := 'az = ' + IntToSTr(res.az) + '°';
    while Length(s) < 13 do s := s + ' ';
    s := s + ' d = ' + IntToSTr(res.d) + ' km.';
    while Length(s) < 30 do s := s + ' ';
    s := s + ' Heff = ' + IntToSTr(res.heff) + ' m.';
    while Length(s) < 48 do s := s + ' ';
    s := s + ' ERP = ' + FormatFloat('0.00', Power(10, res.erp/10)) + ' kW';
    while Length(s) < 68 do s := s + ' ';
    s := s + ' Sea = ' + IntToSTr(res.seaperc) + ' %' + #13;

    mess := 'Розрахунок координаційних відстаней:' + #13;
    mess := mess + s;
  end;

  perc := Round(100 / 36);
  Fire_Progress(perc);
  for i := 1 to 35 do
  begin
    a_deg := i * 10;
    d := GetTxCoordDistance3(tx, a_deg, @res, use_max_parameters);
    idx[0] := i;
    SafeArrayPutElement(zone_km, idx, d);
    ////////////    pzonearray^[i] := d;
    perc := Round(i*100 / 36);
    Fire_Progress(perc);
    if perc > 100 then
    begin
      for k := i+1 to 35 do
      begin
       idx[0] := k;
       d := 0;
       SafeArrayPutElement(zone_km, idx, d);
      end;
      Break;
    end;

    if _logfile <> nil then
    begin
        s := 'az = ' + IntToSTr(res.az) + '°';
        while Length(s) < 13 do s := s + ' ';
        s := s + ' d = ' + IntToSTr(res.d) + ' km.';
        while Length(s) < 30 do s := s + ' ';
        s := s + ' Heff = ' + IntToSTr(res.heff) + ' m.';
        while Length(s) < 48 do s := s + ' ';
        s := s + ' ERP = ' + FormatFloat('0.00', res.erp) + ' dBkW';
        while Length(s) < 68 do s := s + ' ';
        s := s + ' Sea = ' + IntToSTr(res.seaperc) + ' %' + #13;

        mess := mess + s;
    end;
  end;

  //if _logfile <> nil then
  //  ShowMessage(mess);
  
end;

procedure TCoLISBCCalc.SetProgressServer(const progress: ILISProgress);
begin
  if _lis_progress <> progress then _lis_progress := progress;
end;

procedure TCoLISBCCalc.Fire_LISProgress(var perc: integer);
begin
  if _lis_progress <> nil then _lis_progress.Progress(perc);
end;

//function TCoLISBCCalc.GetTxCoordDistance2(tx: ILISBCTx; azimuth: double; res: PCoordDistResult): double;
function TCoLISBCCalc.GetTxCoordDistance3(tx: ILISBCTx; azimuth: double; res: PCoordDistResult; use_max_parameters: boolean): double;
var
    dist, f, erp:  double;
    erps:  double;
    erpv:  double;
    band, h, heff, seaperc: integer;
    p1: TRSAGeoPoint;
    geodata: TRSAGeoPathData;
    path: TRSAGeoPathResults;
    plon, plat: double;
    delta_dist, dist2: double;
    tx_type: TBCTxType;
    is_TV: boolean;
begin
  tx.get_freq_carrier(f);
  band := FreqToBand(f);
{
  Если необходимо учитывать только макс. значения эфф. высоты и ЭИМ
}
  if use_max_parameters then
  begin
    tx.Get_height_eft_max(heff);
    tx.Get_epr_video_max(erpv);
    tx.Get_epr_sound_max_primary(erps);
    erp := Max(erps, erpv);
  end else
  begin
    tx.get_h_eff(Round(azimuth), heff);
    tx.get_erp(Round(azimuth), erp);
  end;

{
  Сначала расчитываем КР для суши,
  а затем будем проверять на наличие воды на трассе
}
  seaperc := 0;

  tx.Get_systemcast(tx_type);
  is_TV := (tx_type = ttTV) or (tx_type = ttDVB);

  _cd.GetCoordDist(erp, band, heff, seaperc, is_TV, dist);

  tx.Get_longitude(plon);
  tx.Get_latitude(plat);
  p1.H := plat;
  p1.L := plon;
  p1.Alt := 0;

  if _relief <> nil then
  begin
    tx.Get_heightantenna(h);
    geodata.TxHeight := h;
    geodata.RxHeight := _RX_ANTENNA_HEIGHT;
//    OleCheck(NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
    (NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
    if not IsNaN(path.HEff) then heff := Round(path.HEff);
    delta_dist := 9999999;

    while (Abs(delta_dist) >= 1) and (path.SeaPercent >= 10) do
    begin
      dist2 := dist;
//      OleCheck(NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
      (NewRunOnAzimuth(p1, azimuth, dist, geodata, path));
      if not IsNaN(path.HEff) then heff := Round(path.HEff);
      _cd.GetCoordDist(erp, band, heff, Round(path.SeaPercent), is_TV, dist2);

      delta_dist := dist2 - dist;
      dist := dist + delta_dist * 0.3;
      if delta_dist < 0 then delta_dist := 0;
    end;
  end;
  if res <> nil then
  begin
    res.heff := heff;
    res.seaperc := Round(path.SeaPercent);
    res.d := Round(dist);
    res.az := Round(azimuth);
    res.erp := erp;
  end;

  result := dist;
end;



{
 Для пары передатчиков расчитываем все что можно в точке с
 заданными координатами.
}
procedure TCoLISBCCalc.GetFieldStrengthControlPoint(const tx_wanted,
  tx_unwanted: ILISBCTx; lon, lat: Double;
  var cp_result: TControlPointCalcResult);
var cpres: TControlPointResult;
    pol: TBCPolarization;
    lon0, lat0: double;
    pr_c, pr_t: double;
    e, erp: double;
    az: integer;
    p1, p2: TRSAGeoPoint;
    h: integer;
    geodata: TRSAGeoPathData;
    path: TRSAGeoPathResults;
    heff: integer;
begin
  e := GetEControlPoint(tx_wanted, tx_unwanted, lon, lat, @cpres);

  tx_unwanted.Get_longitude(lon0);
  tx_unwanted.Get_latitude(lat0);

  p1.H := lat0;
  p1.L := lon0;
  p1.Alt := 0;

  p2.H := lat;
  p2.L := lon;
  p2.Alt := 0;

  tx_unwanted.Get_heightantenna(h);
  geodata.TxHeight := h;
  geodata.RxHeight := _RX_ANTENNA_HEIGHT;

  if _relief = nil then
  begin
    path.Distance := GetDistanceKm(lon0, lat0, lon, lat);
    path.Azimuth := GetAzimuthDeg(lon0, lat0, lon, lat);
    tx_unwanted.get_h_eff(Round(path.Azimuth), heff);
    path.HEff := heff;
    path.TxClearance := NaN;
    path.RxClearance := NaN;
    path.SeaPercent := 0;
  end else NewRunPointToPoint(p1, p2, geodata, path);

  cp_result.azimuth := path.Azimuth;
  cp_result.distance := path.Distance;

  az := Round(path.Azimuth);
  if az > 354 then az := 354;
  tx_unwanted.Get_erp(az, erp);
  cp_result.erp := erp;

  if IsNAN(path.HEff) then tx_unwanted.get_h_eff(az, heff) else heff := Round(path.HEff);
  cp_result.heff := heff;

  cp_result.tx_clearance := path.TxClearance;
  cp_result.rx_clearance := path.RxClearance;
  cp_result.sea_percent := path.SeaPercent;
  cp_result.e_50 := GetE(tx_unwanted, lon, lat, prFifty);
  cp_result.e_10 := GetE(tx_unwanted, lon, lat, prTen);
  cp_result.e_1 := GetE(tx_unwanted, lon, lat, prOne);
  GetProtectRatio(tx_wanted, tx_unwanted, pr_c, pr_t);
  cp_result.pr_continuous := pr_c;
  cp_result.pr_tropospheric := pr_t;
  cp_result.ant_discrimination := cpres.a_discr;
  if cpres.int_type = itCONT then
    cp_result.e_usable := cp_result.e_50 +cp_result.pr_continuous + cp_result.ant_discrimination //e;   !!!!!
  else
    cp_result.e_usable := cp_result.e_1 +cp_result.pr_tropospheric + cp_result.ant_discrimination;
  tx_wanted.Get_polarization(pol);
  cp_result.pol_wanted := pol;

  tx_unwanted.Get_polarization(pol);
  cp_result.pol_unwanted := pol;

  cp_result.interf_type := cpres.int_type;
end;


{
procedure TCoLISBCCalc.GetErpDegradation(idx_unwanted: Integer;
  eu_threshold: Double; out degradation: PSafeArray);
var
    idx: array[0..0] of integer;
    az_degradation_idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    z: PSAFEARRAY;
    lon, lat: double;
    ulon, ulat: double;
    txlon, txlat: double;
    use_in_calc: WordBool;
    d: double;
    i: integer;
    tx0: ILISBCTx;
    tx_unwanted: ILISBCTx;
    az_idx: integer;
    eu1, eu2, delta_eu, delta_eu_old: double;
    perc: integer;
    tx_erp, erp: double;

//***********************************************************
    tx0name, txunwantedname: WideString;
    txcount: integer;
    s: string;
    xxx: double;
//************************************************************
begin
  bounds.cElements := 36;
  bounds.lLbound := 0;
  degradation := SafeArrayCreate(varDouble, 1, bounds);
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  _txlist.Get_Tx(idx_unwanted, tx_unwanted); tx_unwanted._AddRef; // delphi interface decrements reference when goes off the scope

//************************************************************
  tx0.Get_station_name(tx0name);
  tx_unwanted.Get_station_name(txunwantedname);
  _txlist.Get_Size(txcount);
  s := 'TxCount: ' + IntToStr(txcount) + #10#13;
  s := s + 'Tx0: ' + tx0name + #10#13;
  s := s + 'TxUnwanted: ' + txunwantedname;
//************************************************************

  ShowMessage(s);

  s := 'Послаблення:' + #10#13;
  for i := 0 to 35 do
  begin
    perc := Round(i*100 / 36);
    Fire_Progress(perc);

    if i > 5 then delta_eu := 5 else delta_eu := 7;
    SafeArrayPutElement(degradation, az_degradation_idx, delta_eu);
    SafeArrayGetElement(degradation, az_degradation_idx, xxx);

    s := s + IntToStr(i*10) + ': ' + FormatFloat('0.##', xxx) + ' dB' + #10#13;

  end;
  ShowMessage(s);

end;
}

procedure TCoLISBCCalc.GetErpDegradation(idx_unwanted: Integer;
  eu_threshold: Double; out degradation: PSafeArray);
begin
 GetErpDegradation3(idx_unwanted, eu_threshold, degradation);
{
  if MessageDlg('GetErpDegradation3 = YES  GetErpDegradation2 = NO', mtConfirmation, [mbYes, mbNo], 0) = mrYes
    then GetErpDegradation3(idx_unwanted, eu_threshold, degradation)
    else GetErpDegradation2(idx_unwanted, eu_threshold, degradation);
}
end;


procedure TCoLISBCCalc.GetErpDegradation1(idx_unwanted: Integer;
  eu_threshold: Double; out degradation: PSafeArray);
var
    idx: array[0..0] of integer;
    az_degradation_idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    z: PSAFEARRAY;
    lon, lat: double;
    ulon, ulat: double;
    txlon, txlat: double;
    use_in_calc: WordBool;
    d: double;
    i: integer;
    tx0: ILISBCTx;
    tx_unwanted: ILISBCTx;
    az_idx: integer;
    eu1, eu2, delta_eu, delta_eu_old: double;
    perc: integer;

    xxx: double;
begin
  bounds.cElements := 36;
  bounds.lLbound := 0;
  degradation := SafeArrayCreate(varDouble, 1, bounds);
  _txlist.Get_TxUseInCalc(idx_unwanted, use_in_calc);
  _txlist.Set_TxUseInCalc(idx_unwanted, false);
  GetZone_InterferenceLimited(10, z);
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  _txlist.Get_Tx(idx_unwanted, tx_unwanted); tx_unwanted._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(txlon);
  tx0.Get_latitude(txlat);
  tx_unwanted.Get_longitude(ulon);
  tx_unwanted.Get_latitude(ulat);
{
  tx0.Get_station_name(tx0name);
  tx_unwanted.Get_station_name(txunwantedname);
  _txlist.Get_Size(txcount);

  s := 'TxCount: ' + IntToStr(txcount) + #10#13;
  s := s + 'Tx0: ' + tx0name + #10#13;
  s := s + 'TxUnwanted: ' + txunwantedname;

  ShowMessage(s);

  s := 'Послаблення:' + #10#13;
}
  for i := 0 to 35 do
  begin
    perc := Round(i*100 / 36);
    Fire_Progress(perc);
    if perc > 100 then Exit;

    idx[0] := i;
    SafeArrayGetElement(z, idx, d);
    lon := txlon;
    lat := txlat;
    GetNextCoordDeg(i*10, d, lon, lat);

    az_idx := Round(GetAzimuthDeg(ulon, ulat, lon, lat) / 10);
    while az_idx >= 36 do az_idx := az_idx - 36;
    while az_idx < 0 do az_idx := az_idx + 36;

    _txlist.Set_TxUseInCalc(idx_unwanted, false);
    eu1 := GetUsableFieldStrength(lon, lat);
    _txlist.Set_TxUseInCalc(idx_unwanted, true);
    eu2 := GetUsableFieldStrength(lon, lat);

{
  порог не может быть нулевой или отрицательный,
  потому что если нет превышения Еисп (порог=0), это значит что
  все мешающие передатчикм выключены, т.е. их ЭИМ = - 9999999(9)
}

    eu_threshold := Max(0.001, eu_threshold);

{
  теперь надо определить, на сколько уменьшить мощность передатчика,
  чтобы помеха от него не превышала порог
}

    if eu2 > eu1 + eu_threshold
      then  delta_eu := 10 * Log10( DbToW(eu2) - DbToW(eu1) ) - 10 * Log10( DbToW(eu1 + eu_threshold) - DbToW(eu1) )
      else delta_eu := 0;
    delta_eu := Max(0, delta_eu);

    az_degradation_idx[0] := az_idx;
{
  если мы уже расчитывали ослабление по этому направлению -
  сравниваем со старым значением
}
    SafeArrayGetElement(degradation, az_degradation_idx, delta_eu_old);
    delta_eu := Max(delta_eu, delta_eu_old);

    SafeArrayPutElement(degradation, az_degradation_idx, delta_eu);
    SafeArrayGetElement(degradation, az_degradation_idx, xxx);

  end;

  _txlist.Set_TxUseInCalc(idx_unwanted, use_in_calc);

end;



{
  Расчитывает изменение исп напряж поля в заданной точке для рабочего списка передатчиков
  относительно передатчика с индексом idx_unwanted. Учитывает заданный порог превышения
}
function TCoLISBCCalc.GetErpDegradationControlPoint(idx_unwanted: Integer; eu_threshold: Double; lon, lat: double): double;
var eu1, eu2, delta_eu: double;
    use_in_calc: WordBool;
begin
  _txlist.Get_TxUseInCalc(idx_unwanted, use_in_calc);
  try
    _txlist.Set_TxUseInCalc(idx_unwanted, false);
    eu1 := GetUsableFieldStrength(lon, lat);
    _txlist.Set_TxUseInCalc(idx_unwanted, true);
    eu2 := GetUsableFieldStrength(lon, lat);
  finally
    _txlist.Set_TxUseInCalc(idx_unwanted, use_in_calc);
  end;

  delta_eu := eu2 - eu1;
  result := eu_threshold - delta_eu;
end;



procedure TCoLISBCCalc.GetErpDegradation3(idx_unwanted: Integer;
  eu_threshold: Double; out degradation: PSafeArray);
var
    idx: array[0..0] of integer;
    az_degradation_idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    z: PSAFEARRAY;
    lon, lat: double;
    ulon, ulat: double;
    txlon, txlat: double;
    use_in_calc: WordBool;
    d: double;
    i: integer;
    tx0: ILISBCTx;
    tx_unwanted: ILISBCTx;
    az_idx: integer;
    perc: integer;
    erp, delta_erp: double;
    erp_step: double;
    stop_calc: boolean;
    erpv, erph: array[0..35] of double;
    old_az_idx: integer;
    calc_type: TModalResult;
begin
  bounds.cElements := 36;
  bounds.lLbound := 0;
  degradation := SafeArrayCreate(varDouble, 1, bounds);
  _txlist.Get_TxUseInCalc(idx_unwanted, use_in_calc);
  _txlist.Set_TxUseInCalc(idx_unwanted, false);
  GetZone_InterferenceLimited(10, z);
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  _txlist.Get_Tx(idx_unwanted, tx_unwanted);  tx_unwanted._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(txlon);
  tx0.Get_latitude(txlat);
  tx_unwanted.Get_longitude(ulon);
  tx_unwanted.Get_latitude(ulat);
  stop_calc := false;

{
  запоминаем мощности
}
  for i := 0 to 35 do
  begin
    tx_unwanted.Get_effectpowerhor(i,erp);
    erph[i] := erp;
    tx_unwanted.Get_effectpowervert(i,erp);
    erpv[i] := erp;
  end;

  erp_step := 0.1;

  calc_type := MessageDlg('Использовать новый метод расчета?', mtInformation, [mbYes, mbNo], 0);


  while not stop_calc do
  begin
    stop_calc := true;
    old_az_idx := -1;
    for i := 0 to 35 do
    begin
      perc := Round(i*100 / 36);
      Fire_Progress(perc);
      if perc > 100 then Exit;

      idx[0] := i;
      SafeArrayGetElement(z, idx, d);
      lon := txlon;
      lat := txlat;
      GetNextCoordDeg(i*10, d, lon, lat);

      az_idx := Round(GetAzimuthDeg(ulon, ulat, lon, lat) / 10);
      while az_idx >= 36 do az_idx := az_idx - 36;
      while az_idx < 0 do az_idx := az_idx + 36;
      delta_erp := 0;
  {
    Если есть превышение Еисп то отнимаем мощность
  }
      if (GetErpDegradationControlPoint(idx_unwanted, eu_threshold, lon, lat) < 0) then
      begin
        if calc_type = mrYes then
        begin
          if (az_idx <> old_az_idx) then
          begin
            tx_unwanted.Get_effectpowerhor(az_idx,erp);
            erp := erp - erp_step;
            tx_unwanted.Set_effectpowerhor(az_idx, erp);

            tx_unwanted.Get_effectpowervert(az_idx,erp);
            erp := erp - erp_step;
            tx_unwanted.Set_effectpowervert(az_idx, erp);

            old_az_idx := az_idx;
            stop_calc := false;
          end;
        end else
        begin
          tx_unwanted.Get_effectpowerhor(az_idx,erp);
          erp := erp - erp_step;
          tx_unwanted.Set_effectpowerhor(az_idx, erp);

          tx_unwanted.Get_effectpowervert(az_idx,erp);
          erp := erp - erp_step;
          tx_unwanted.Set_effectpowervert(az_idx, erp);

          old_az_idx := az_idx;
          stop_calc := false;
        end;
      end;
    end;
  end;

  for i := 0 to 35 do
  begin
    tx_unwanted.Get_effectpowervert(i,erp);
    delta_erp := erpv[i] - erp;
    tx_unwanted.Get_effectpowerhor(i,erp);
    delta_erp := Max(erph[i] - erp, delta_erp);

    az_degradation_idx[0] := i;

    tx_unwanted.Set_effectpowerhor(i,erph[i]);
    tx_unwanted.Set_effectpowervert(i,erpv[i]);

    SafeArrayPutElement(degradation, az_degradation_idx, delta_erp);
  end;

  _txlist.Set_TxUseInCalc(idx_unwanted, use_in_calc);
end;




procedure TCoLISBCCalc.GetErpDegradation2(idx_unwanted: Integer;
  eu_threshold: Double; out degradation: PSafeArray);
var
    idx: array[0..0] of integer;
    az_degradation_idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    z: PSAFEARRAY;
    lon, lat: double;
    ulon, ulat: double;
    txlon, txlat: double;
    use_in_calc: WordBool;
    d: double;
    i: integer;
    tx0: ILISBCTx;
    tx_unwanted: ILISBCTx;
    az_idx: integer;
    eu1, eu2, delta_eu: double;
    perc: integer;
    erp, delta_erp: double;
    erp_step: double;
begin
  bounds.cElements := 36;
  bounds.lLbound := 0;
  degradation := SafeArrayCreate(varDouble, 1, bounds);
  _txlist.Get_TxUseInCalc(idx_unwanted, use_in_calc);
  _txlist.Set_TxUseInCalc(idx_unwanted, false);
  GetZone_InterferenceLimited(10, z);
  _txlist.Get_Tx(0, tx0); tx0._AddRef; // delphi interface decrements reference when goes off the scope
  _txlist.Get_Tx(idx_unwanted, tx_unwanted);  tx_unwanted._AddRef; // delphi interface decrements reference when goes off the scope
  tx0.Get_longitude(txlon);
  tx0.Get_latitude(txlat);
  tx_unwanted.Get_longitude(ulon);
  tx_unwanted.Get_latitude(ulat);

  for i := 0 to 35 do
  begin
    perc := Round(i*100 / 36);
    Fire_Progress(perc);
    if perc > 100 then Exit;

    idx[0] := i;
    SafeArrayGetElement(z, idx, d);
    lon := txlon;
    lat := txlat;
    GetNextCoordDeg(i*10, d, lon, lat);

    az_idx := Round(GetAzimuthDeg(ulon, ulat, lon, lat) / 10);
    while az_idx >= 36 do az_idx := az_idx - 36;
    while az_idx < 0 do az_idx := az_idx + 36;
    delta_erp := 0;
    erp_step := 0.1;

{
  отнимает от эфф мощности по заданному направлению
  по 0.1 дБ (erp_step) пока превыщение Еисп не опустится ниже порога

  После цикла возвращаем ЭИМ в исходное состояние добавлением
  расчитанного ослабления
}
    try
      repeat
        _txlist.Set_TxUseInCalc(idx_unwanted, false);
        eu1 := GetUsableFieldStrength(lon, lat);
        _txlist.Set_TxUseInCalc(idx_unwanted, true);
        eu2 := GetUsableFieldStrength(lon, lat);
        delta_eu := eu2 - eu1;
        if delta_eu > eu_threshold then
        begin
          delta_erp := delta_erp + erp_step;

          tx_unwanted.Get_effectpowerhor(az_idx,erp);
          erp := erp - erp_step;
          tx_unwanted.Set_effectpowerhor(az_idx, erp);

          tx_unwanted.Get_effectpowervert(az_idx,erp);
          erp := erp - erp_step;
          tx_unwanted.Set_effectpowervert(az_idx, erp);

        end;

      until delta_eu <= eu_threshold;
    finally
      tx_unwanted.Get_effectpowerhor(az_idx,erp);
      erp := erp + delta_erp;
      tx_unwanted.Set_effectpowerhor(az_idx, erp);

      tx_unwanted.Get_effectpowervert(az_idx,erp);
      erp := erp + delta_erp;
      tx_unwanted.Set_effectpowervert(az_idx, erp);
    end;

    az_degradation_idx[0] := az_idx;

    SafeArrayGetElement(degradation, az_degradation_idx, erp);
    delta_erp := Max(delta_erp, erp);
    SafeArrayPutElement(degradation, az_degradation_idx, delta_erp);

//    ShowMessage('az_idx = ' + IntToStr(az_idx) + '  delta_erp = ' + FloatToSTr(delta_erp));
  end;

  _txlist.Set_TxUseInCalc(idx_unwanted, use_in_calc);

end;




function TCoLISBCCalc.GetSumE_DVB(lon, lat: double): double;
var i,n: integer;
    e, ecp_value: double;
    tx: ILISBCTx;
    b: wordbool;
    st, s :WideString;
begin
  _txlist.Get_Size(n);
  e := 0;
  s := '';
  for i := 1 to n-1 do
  begin
    _txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
    _txlist.Get_TxUseInCalc(i, b);
    if true then
    begin
        ecp_value := GetE(tx, lon, lat, prOne);
        e := e + Power(10,  ecp_value / 10);
        tx.Get_station_name(st);
        s := s + st + ' -- ' + FloatToStr(ecp_value) + #13#10;
    end;
    tx := nil; // get rid of final _Release
  end;
    //  ShowMessage(s);
  if e > 0 then result := 10 * Log10(e) else result := NO_INTERFERENCE;

end;



{
  Расчитывает напряженность от эт.сети в контрольной точке с координатами lon, lat.
  При этом сет распологается на границе зоны вплонтную к точке bp_lon, bp_lat
}
function TCoLISBCCalc.GetE_RN_BorderPoint(rn: TReferenceNetwork; bp_lon, bp_lat, lon, lat: double): double;
var i,n: integer;
    e, ecp_value: double;
    tx: ILISBCTx;
    rn_txlist: ILISBCTxList;
    az: double;
begin
  az := GetAzimuthDeg(bp_lon, bp_lat, lon, lat);
  rn.LocateRNBorderPoint(bp_lon, bp_lat, az);
  rn_txlist := rn.TxList;
  rn_txlist.Get_Size(n);
  if (n > 0) then
  begin
    e := 0;
    i := 0;   // начинаем отсчет с первого индекса (индекс 0 - планируемый перед. 1,2,3... - мешающие)
    while (i < n) do
    begin
      rn_txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
      ecp_value := GetE(tx, lon, lat, prOne);
      e := e + Power(10,  ecp_value / 10);
      i := i + 1;
      tx := nil;
    end;
    if e > 0 then result := 10 * Log10(e) else result := NO_INTERFERENCE;
  end else result := NO_INTERFERENCE;
end;



function TCoLISBCCalc.GetSumFieldStrength(lon, lat: Double): Double;
begin
  result := GetSumE_DVB(lon, lat);
end;





{Минимальные медианные значения напряж. поля для ЦТВ.  Табл. А.3.2-2 РКР-06}
function TCoLISBCCalc.EminDVB(tx: ILISBCTx): double;
const
   emin200: array[rmFx..rmPo, dsA1..dsC7] of double = (
   (34.90, 36.90, 38.10, 39.30, 40.30, 40.60, 43.10, 44.70, 45.90, 46.50, 46.20, 48.50, 50.20, 51.70, 52.70),  // FX
   (59.10, 61.20, 62.50, 63.80, 64.90, 64.80, 67.40, 69.10, 70.40, 71.10, 70.40, 72.80, 74.60, 76.20, 77.30),  // MO
   (66.10, 68.20, 69.50, 70.80, 71.90, 71.80, 74.40, 76.10, 77.40, 78.10, 77.40, 79.80, 81.60, 83.20, 84.30),  // PI
   (56.10, 58.20, 59.50, 60.80, 61.90, 61.80, 64.40, 66.10, 67.40, 68.10, 67.40, 69.80, 71.60, 73.20, 74.30)); // PO

   emin500: array[rmFx..rmPo, dsA1..dsC7] of double = (
   (38.90, 40.90, 42.10, 43.30, 44.30, 44.60, 47.10, 48.70, 49.90, 50.50, 50.20, 52.50, 54.20, 55.70, 56.70),  // FX
   (67.10, 69.20, 70.50, 71.80, 72.90, 72.80, 75.40, 77.10, 78.40, 79.10, 78.40, 80.80, 82.60, 84.20, 85.30),  // MO
   (76.10, 78.20, 79.50, 80.80, 81.90, 81.80, 84.40, 86.10, 87.40, 88.10, 87.40, 89.80, 91.60, 93.20, 94.30),  // PI
   (64.10, 66.20, 67.50, 68.80, 69.90, 69.80, 72.40, 74.10, 75.40, 76.10, 75.40, 77.80, 79.60, 81.20, 82.30)); // PO
var dvbsys: TBCDVBSystem;
    rxmode: TBcRxMode;
    f, e, fr: double;
    corr: double;
    polar: TBCPolarization;
    dvbt2p: ILisBcDvbt2;
    txparam: TTxParams;
begin
  txparam.is_dvbt2 := false;
  tx.QueryInterface(IID_ILisBcDvbt2, dvbt2p);
  if dvbt2p <> nil then
  begin
    txparam.is_dvbt2 := dvbt2p.IsDvbt2;
    txparam.modulation := dvbt2p.Modulation;
    txparam.code_rate := dvbt2p.CodeRate;
    txparam.rotated_constellations := dvbt2p.RotatedConstellations;
    txparam.pilot_pattern := dvbt2p.PilotPattern;
  end;

  tx.Get_dvb_system(dvbsys);
  if dvbsys > dsC7 then dvbsys := dvbsys - $0000000F;

  if txparam.is_dvbt2 then
    dvbsys := GetDvbtSys(txparam.modulation, txparam.code_rate);

  tx.Get_rxMode(rxmode);
  tx.get_freq_carrier(f);
  tx.Get_polarization(polar);

  if f < 470 then
  begin
    e := emin200[rxmode, dvbsys];
    fr := 200;
  end else
  begin
    e := emin500[rxmode, dvbsys];
    fr := 500;
  end;

  if rxmode = rmFX
    then corr := 20 * Log10(f / fr)
    else corr := 30 * Log10(f / fr);

  result := e + corr;
{ Если прием мобильный то приемная антенна - это обычный штырь
  который имеет вертикальную поляризацию. Поэтому если у передатчика
  горизонтальная поляризация то делаем добавку равную поляризационной
  развязке приемной антенны (16 дБ)

  убрано 15.12.2011 по протоколу с Дороком и Жавроцким
}
  //if (rxmode = rmMo) and (polar = plHOR) then result := result + 16;

  if txparam.is_dvbt2 then
    result := result + GetDvbt2CnCorrection(@txparam, true);
end;





function TCoLISBCCalc.GetE_Allot_Subarea(allot: ILISBCDigAllot; rn: TReferenceNetwork;
                                            SubAreaID: integer; lon, lat: double): double;
var nb_point: integer;
    bp_lon, bp_lat: double;
    i: integer;
    e, e_max: double;
    idx1, idx2: integer;
    p1, p2: BcCoord;
    az: double;
    d_min, d: double;
    min_bp_lon, min_bp_lat: double;
begin
  allot.Get_points_num(SubAreaID, nb_point);
  e_max := -999;

  // если без учета рельефа - расчитывем ближайшую точку на границе и считаем только для этой точки
  if _relief = nil then
  begin
    d_min := 900000;
    min_bp_lon := -181;
    min_bp_lat := -91;
    for i := 0 to nb_point-1 do
    begin
      idx1 := i;
      if i = nb_point-1 then idx2 := 0 else idx2 := i + 1;
      allot.Get_point(SubAreaID, idx1, p1);
      allot.Get_point(SubAreaID, idx2, p2);
      bp_lon := p1.lon;
      bp_lat := p1.lat;
      d := GetDistanceKm(bp_lon, bp_lat, lon, lat);
      if d < d_min then
      begin
        d_min := d;
        min_bp_lon := bp_lon;
        min_bp_lat := bp_lat;
      end;
      while GetDistanceKm(bp_lon, bp_lat, p2.lon, p2.lat) > _STEP_CALC_ALLOT do
      begin
        d := GetDistanceKm(bp_lon, bp_lat, lon, lat);
        if d < d_min then
        begin
          d_min := d;
          min_bp_lon := bp_lon;
          min_bp_lat := bp_lat;
        end;
        az := GetAzimuthDeg(bp_lon, bp_lat, p2.lon, p2.lat);
        GetNextCoordDeg(az, _STEP_CALC_ALLOT, bp_lon, bp_lat);
      end;
    end;
    if min_bp_lon > -181 then
        e_max := GetE_RN_BorderPoint(rn, min_bp_lon, min_bp_lat, lon, lat);
  end else
  begin
    for i := 0 to nb_point-1 do
    begin
      idx1 := i;
      if i = nb_point-1 then idx2 := 0 else idx2 := i + 1;
      allot.Get_point(SubAreaID, idx1, p1);
      allot.Get_point(SubAreaID, idx2, p2);
      bp_lon := p1.lon;
      bp_lat := p1.lat;
      e := GetE_RN_BorderPoint(rn, bp_lon, bp_lat, lon, lat);

      if e > e_max then e_max := e;
      while GetDistanceKm(bp_lon, bp_lat, p2.lon, p2.lat) > _STEP_CALC_ALLOT do
      begin
        e := GetE_RN_BorderPoint(rn, bp_lon, bp_lat, lon, lat);
        if e > e_max then e_max := e;

        az := GetAzimuthDeg(bp_lon, bp_lat, p2.lon, p2.lat);
        GetNextCoordDeg(az, _STEP_CALC_ALLOT, bp_lon, bp_lat);
      end;
    end;
  end;
  result := e_max;
end;




function RnStrToRn(rnstr: string): TBcRn;
begin
  if rnstr = 'RN1' then Result := rn1 else
  if rnstr = 'RN2' then Result := rn2 else
  if rnstr = 'RN3' then Result := rn3 else
  if rnstr = 'RN4' then Result := rn4 else
  if rnstr = 'RN5' then Result := rn5 else
  if rnstr = 'RN6' then Result := rn6 else Result := rn1;
end;




function RpcStrToRpc(rpcstr: string): TBcRpc;
begin
  if rpcstr = 'RPC1' then Result := rpc1 else
  if rpcstr = 'RPC2' then Result := rpc2 else
  if rpcstr = 'RPC3' then Result := rpc3 else
  if rpcstr = 'RPC4' then Result := rpc4 else
  if rpcstr = 'RPC5' then Result := rpc5 else Result := rpc1;
end;



{
 Расчет суммарной напряженности от аллотмента
}
function TCoLISBCCalc.GetE_Allot(allot: ILISBCDigAllot; rn: TReferenceNetwork; lon, lat: double): double;
var nb_subarea: integer;
    i, id: integer;
    e, e_max: double;
begin
  e_max := -999;
  allot.Get_SubareaCount(nb_subarea);
  for i := 0 to nb_subarea-1 do
  begin
    allot.Get_subareaId(i, id);
    e := GetE_Allot_Subarea(allot, rn, id, lon, lat);
    if e > e_max then e_max := e;
  end;
  result := e_max;
end;





function TCoLISBCCalc.GetE_Contour_Allot_Subarea(allot: ILISBCDigAllot; rn: TReferenceNetwork;
                                                SubAreaID: integer; emin: double): PSafeArray;
type bds = array[0..1] of SAFEARRAYBOUND;
var i: integer;
    minlon, minlat: double;
    lon, lat: double;
    az: double;
    p1: TZPoint;
    az_back, az_norm: double;
    lastpoint: TZPoint;
    firstpoint: TZPoint;
    nb_point: integer;
    p: BcCoord;
    z: TDVBZone;
    z_contour: TDVBZone; // контур напряженности поля
    e: double;
    res: PSafeArray;
    bounds: bds;
    idx: array[0..1] of integer;
begin
  allot.Get_points_num(SubAreaID, nb_point);
  if nb_point <= 0 then
  begin
    result := nil;
    Exit;
  end;
{
  Ищем отправную точку - любую, лежащую вне исходной зоны.
  Для этого находим самую минимальную широту и долготу.
}
  minlon := 1000000;
  minlat := 1000000;

  z := TDVBZone.Create;
  z_contour := TDVBZone.Create;

  for i := 0 to nb_point-1 do
  begin
    allot.Get_point(SubAreaID, i, p);
    if p.lon < minlon then minlon := p.lon;
    if p.lat < minlat then minlat := p.lat;
    z.AddPoint(p.lon, p.lat);
  end;

  lon := minlon;
  lat := minlat;

  repeat
    p1.lon := lon;
    p1.lat := lat;

    RegionToPointDistance(z, p1, az);
    az_back := az - 180;
    az_norm := az - 90;

    while az_back < 0 do az_back := az_back + 360;
    while az_norm < 0 do az_norm := az_norm + 360;

    while az_back >= 360 do az_back := az_back - 360;
    while az_norm >= 360 do az_norm := az_norm - 360;

    {
    Отползаем назад до расстояния повторяемости
    или вперед
    }
    e := GetE_Allot_Subarea(allot, rn, SubAreaID, p1.lon, p1.lat);
    if e > emin then
    begin
      while e > emin do
      begin
        GetNextCoord(az_back, _STEP_FINAL, p1.lon, p1.lat);
        e := GetE_Allot_Subarea(allot, rn, SubAreaID, p1.lon, p1.lat);
      end
    end else
    if e < emin then
    begin
      while e > emin do
      begin
        GetNextCoord(az_back, _STEP_FINAL, p1.lon, p1.lat);
        e := GetE_Allot_Subarea(allot, rn, SubAreaID, p1.lon, p1.lat);
      end
    end;

    z_contour.AddPoint(p1.lon, p1.lat);
  {
    Рассчитываем следующую точку - в сторону от предыдущей
    на расст. step_km
  }
    lon := p1.lon;
    lat := p1.lat;
    GetNextCoord(az_norm, _STEP_CALC_ALLOT, lon, lat);
    lastpoint.lon := p1.lon;
    lastpoint.lat := p1.lat;

    firstpoint.lon := z.GetLon(0);
    firstpoint.lat := z.GetLat(0);

  until (PointToPointDistance(lastpoint.lon, lastpoint.lat, firstpoint.lon, firstpoint.lat) <= 10) and (z_contour.PointsCount > 2);

  bounds[0].lLbound := 0;
  bounds[0].cElements := z_contour.PointsCount;
  bounds[1].lLbound := 0;
  bounds[1].cElements := 2;

  res := SafeArrayCreate(varDouble, 1, bounds);

  for i := 1 to z_contour.PointsCount-1 do
  begin
    idx[0] := i;
    idx[1] := 0;
    lon := z_contour.GetLon(i);
    SafeArrayPutElement(res, idx, lon);

    idx[0] := i;
    idx[1] := 1;
    lat := z_contour.GetLat(i);
    SafeArrayPutElement(res, idx, lat);
  end;
  result := res;

end;



function TCoLISBCCalc.GetEContourAllotSubarea(const allot: ILisBcDigAllot;
  subareaId: Integer; emin: Double): PSafeArray;
var
    rn: TReferenceNetwork;
begin
    rn := CreateRn(allot);
    try
        Result := GetE_Contour_Allot_Subarea(allot, rn, subareaId, emin);
    finally
        rn.Free;
    end;
end;

function TCoLISBCCalc.GetEAllotSubarea(const allot: ILisBcDigAllot;
  subareaId: Integer; lon, lat: Double): Double;
var
    rn: TReferenceNetwork;
begin
    rn := CreateRn(allot);
    try
        result := GetE_Allot_Subarea(allot, rn, subareaId, lon, lat);
    finally
        rn.Free;
    end
end;

function TCoLISBCCalc.GetEAllot(const allot: ILisBcDigAllot; lon,
  lat: Double): Double;
var
    rn: TReferenceNetwork;
begin
    rn := CreateRn(allot);
    try
    result := GetE_Allot(allot, rn, lon, lat);
    finally
        rn.Free;
    end;
end;



function TCoLISBCCalc.GetRNPositionAtBorderPoint(const allot: ILisBcDigAllot; lon, lat,
      azimuth_deg: Double): ILISBCTxList;
var
  rn: TReferenceNetwork;
begin
  rn := CreateRn(allot);
  try
    rn.LocateRNBorderPoint(lon, lat, azimuth_deg);
    result := rn.TxList;
  finally
    rn.Free;
  end
end;



function TCoLISBCCalc.GetRNPosition(const allot: ILisBcDigAllot; lon, lat,
  azimuth_deg: Double): ILISBCTxList;
var
  rn: TReferenceNetwork;
begin
  rn := CreateRn(allot);
  try
    rn.LocateRN(lon, lat, azimuth_deg);
    result := rn.TxList;
  finally
    rn.Free;
  end
end;

function TCoLISBCCalc.GetMaxDistanceEmin_GE06(tx: ILISBCTx; azimuth, emin: double): double;
var
    d: double;
    step_km: double;
    step_deg: double;
    e: double;
begin
  d := 1000;
  step_deg := 1;
  e := -999;
  while (e < emin) and (d > 1) do
  begin    e := GetE_Azimuth(tx, azimuth, d, prOne);
    step_km := d * sin(DegToRad(step_deg/2));
    if step_km < _STEP_FINAL then step_km := _STEP_FINAL;
    d := d - step_km;
  end;
  result := d;
end;



procedure TCoLISBCCalc.GetZone_Emin_GE06(const tx: ILISBCTx; step_deg, emin: Double;
  out zone_km: PSafeArray);
var d: double;
    i: integer;
    a_deg: integer;
    naz: integer;
    idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
begin
  naz := Trunc(360 / step_deg);
  bounds.cElements := naz;
  bounds.lLbound := 0;
  zone_km := SafeArrayCreate(varDouble, 1, bounds);

  d := GetMaxDistanceEmin_GE06(tx, 0, emin);

  idx[0] := 0;
  SafeArrayPutElement(zone_km, idx, d);

  for i := 1 to naz-1 do
  begin
    a_deg := Round(i * step_deg);
    d := GetMaxDistanceEmin_GE06(tx, a_deg, emin);
    idx[0] := i;
    SafeArrayPutElement(zone_km, idx, d);
  end;
end;



function TCoLISBCCalc.GetEmin_trigger(const tx: ILISBCTx): Double;
var txtype: TBCTxType;
    f: double;
    allot: ILisBcDigAllot;
    s: WideString;
begin
  result := -999;
  tx.Get_systemcast(txtype);
  tx.get_freq_carrier(f);

  if txtype=ttAllot then
  begin
    OleCheck(tx.QueryInterface(ILisBcDigAllot, allot));
    allot.Get_typ_ref_netwk(s);
    if (s = 'RN4') or (s = 'RN5') then txtype := ttDAB else txtype := ttDVB;
    allot.Get_freq(f);
  end;

  if txtype=ttDVB then
  begin
    if (f >= 174) and (f < 230) then result := 17;
    if (f >= 470) and (f < 582) then result := 21;
    if (f >= 582) and (f < 718) then result := 23;
    if (f >= 718) and (f < 862) then result := 25;
  end;

  if txtype=ttDAB then
  begin
    if (f >= 174) and (f < 230) then result := 12;
  end;

  if txtype=ttTV then
  begin
    if (f >= 174) and (f < 230) then result := 10;
    if (f >= 470) and (f < 582) then result := 18;
    if (f >= 582) and (f < 718) then result := 20;
    if (f >= 718) and (f < 862) then result := 22;
  end;

end;



function TCoLISBCCalc.GetParam(paramName: Integer): OleVariant;
begin

end;

procedure TCoLISBCCalc.ConfigDialog;
begin

end;

procedure TCoLISBCCalc.LoadConfig(const regPath: WideString);
begin

end;

procedure TCoLISBCCalc.SaveConfig(const regPath: WideString);
begin

end;

procedure TCoLISBCCalc.SetParam(const paramName: WideString;
  Value: OleVariant);
begin

end;

function TCoLISBCCalc.EminDAB(tx: ILISBCTx): double;
var f: double;
    rxmode: TBcRxMode;
    f_ref, e_ref: double;
begin
  tx.Get_rxMode(rxmode);
  tx.get_freq_carrier(f);
  if rxmode = rmMo then e_ref := 60 else e_ref := 66;
  f_ref := 200;

  result := e_ref + 30 * Log10(f / f_ref);
end;


function TCoLISBCCalc.IEminCalc_GetEmin(freq: Double): Double;
begin
  EminDialog := TEminDialog.Create(nil);
  EminDialog.Frequency := freq;
  if EminDialog.ShowModal = mrOk then _emin_for_allotment := EminDialog.Emin else _emin_for_allotment := -999;
  EminDialog.Free;
  result := _emin_for_allotment;
end;


procedure TCoLISBCCalc.GetSfnZone(const txlist: LISBCTxList; var center_lon, center_lat, emin: Double; out zone_km: PSafeArray);
var d: double;
    i: integer;
    a_deg: integer;
    naz: integer;
    idx: array[0..0] of integer;
    bounds: SAFEARRAYBOUND;
    step_deg: double;
    perc: integer;
begin
  step_deg := 10;
  naz := Trunc(360 / 10);
  bounds.cElements := naz;
  bounds.lLbound := 0;
  zone_km := SafeArrayCreate(varDouble, 1, bounds);

  perc := 0;
  Fire_Progress(perc);

  d := GetMaxDistanceEmin_SFN(txlist, center_lon, center_lat, 0, emin);

  idx[0] := 0;
  SafeArrayPutElement(zone_km, idx, d);

  for i := 1 to naz-1 do
  begin
    a_deg := Round(i * step_deg);
    perc := Round(i*100 / naz);
    Fire_Progress(perc);
    if perc > 100 then
        Exit
    else begin
        d := GetMaxDistanceEmin_SFN(txlist, center_lon, center_lat, a_deg, emin);
        idx[0] := i;
        SafeArrayPutElement(zone_km, idx, d);
    end 
  end;
end;




function TCoLISBCCalc.SumPowerdB(pwdb1, pwdb2: double): double;
begin
  result := 10 * Log10(Power(10, pwdb1/10) + Power(10, pwdb2/10));
end;



procedure TCoLISBCCalc.GetE_t50_t1x_all_polar(tx: ILISBCTx; lon, lat: double; perc: TPercentage; var e50_H, e1_H, e50_V, e1_V: double);
var e50, e1: double;
    erp, erp_v, erp_h, erp_vl, erp_vh, txlon, txlat: double;
    az, azl: integer;
begin
  GetE_t50_t1x(tx, lon, lat, perc, e50, e1);
  tx.Get_longitude(txlon);
  tx.Get_latitude (txlat);
  az := Round(GetAzimuthDeg(txlon, txlat, lon, lat));
  tx.get_erp(az, erp);
  if az >= 355 then az := 0;

  azl := Floor(az / 10.0);
  tx.Get_effectpowervert (azl, erp_vl);
  if azl = 35 then
        azl := -1;
  tx.Get_effectpowervert(azl + 1, erp_vh);

  e50_H := e50;
  e1_H  := e1;

  erp_v := erp_vl + (erp_vh - erp_vl)*(az / 10.0 - azl);

  e50_V := e50 - erp + erp_v;
  e1_V  := e1  - erp + erp_v;
end;




function TCoLISBCCalc.GetMaxDistanceEmin_SFN(txlist: ILISBCTxList; center_lon, center_lat, azimuth, emin: double): double;
var dtemp, d, dend: double;
    step_km: double;
    step_deg: double;
    eps, e, e_sum: double;
    bo, i, txcount: integer;
    tx: ILISBCTx;
    lon, lat: double;
    sss: WideString;
    use_in_calc: WordBool;
begin
  d := 1;
  dtemp := 1;
  step_deg := 1;
  step_km := 2;
  txlist.Get_Size(txcount);
  e := -999;
  eps := 0.1;
  bo := 0;
  while (abs(e - emin) > eps) and (d < 1000) do
  begin
    d := dtemp + step_km;
    e_sum := 0;
    sss := '';
    for i := 1 to txcount-1 do
    begin
      txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
      txlist.Get_TxUseInCalc(i, use_in_calc);
      lon := center_lon;
      lat := center_lat;
      GetNextCoordDeg(azimuth, d, lon, lat);
      if  use_in_calc then
      begin
        e := GetE(tx, lon, lat, prOne);
        e_sum := e_sum + Power(10.0, e / 10.0);
       // e_sum := SumPowerdB(e_sum, e);
      end;
      tx := nil; // get rid of final _Release
    end;
     if e_sum > 0 then
        e_sum := 10.0 * Log10(e_sum)
    else
        e_sum := 0;
    e := e_sum;
    if abs(e - emin) > eps then
    begin
        if e < emin then begin
            if bo = 0 then begin
                d := d + step_km;
                e_sum := 0;
                sss := '';
                for i := 1 to txcount-1 do
                begin
                  txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
                  txlist.Get_TxUseInCalc(i, use_in_calc);
                  lon := center_lon;
                  lat := center_lat;
                  GetNextCoordDeg(azimuth, d, lon, lat);
                  if  use_in_calc then
                  begin
                    e := GetE(tx, lon, lat, prOne);
                    e_sum := e_sum + Power(10.0, e / 10.0);
                  end;
                  tx := nil; // get rid of final _Release
                end;
                if e_sum > 0 then
                    e_sum := 10.0 * Log10(e_sum)
                else
                    e_sum := 0;
                if abs(e_sum - emin) > eps then
                begin
                    if e < emin then begin
                        bo := 1;
                        dend := d;
                    end
                    else
                        dtemp := d;
                end
                else
                    e := e_sum;
            end
            else
                dend := d;
        end
        else
            dtemp := d;
    end;
    if bo = 1 then
        step_km := abs((dend - dtemp)) / 1.5;
    if step_km < 0.0001 then break;
  end;
  result := d;
end;





procedure TCoLISBCCalc.NewRunPointToPoint(A: TRSAGeoPoint; B: TRSAGeoPoint; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
begin
  if _USE_NEW_RELIEF_CALC
    then TCARunPointToPoint(_relief, _spherics, _relief_param, A, B, Data, Results)
    else OleCheck(_relief.RunPointToPoint( A, B, Data, Results));
end;


procedure TCoLISBCCalc.NewRunOnAzimuth(A: TRSAGeoPoint; Az: TRSAAzimuth; Dist: TRSADistance; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
begin
  if _USE_NEW_RELIEF_CALC
    then TCARunOnAzimuth(_relief, _spherics, _relief_param, A, Az, Dist, Data, Results)
    else OleCheck(_relief.RunOnAzimuth(A, Az, Dist, Data, Results));
end;



function TCoLISBCCalc.CreateRn(allot: ILisBcDigAllot): TReferenceNetwork;
var
    rn_type_str: WideString;
    rpc_type_str: WideString;
    rn_type: TBcRn;
    rpc_type: TBcRpc;
    freq: double;
begin
    allot.Get_ref_plan_cfg(rpc_type_str);
    allot.Get_typ_ref_netwk(rn_type_str);
    rn_type := RnStrToRn(rn_type_str);
    rpc_type := RpcStrToRpc(rpc_type_str);
    allot.Get_freq(freq);

    result := TReferenceNetwork.Create(rn_type, rpc_type, freq);
end;


function TCoLISBCCalc.EminFXM(tx: ILISBCTx): double;
var em: double;
    fxm_system : integer;
    fxm: ILisBcFxm;
begin
    tx.QueryInterface(IID_ILisBcFxm, fxm);
    fxm_system := fxm.fxm_system;
    case fxm_system of
        osAA2:	em := 24;
        osAA8:	em := 42;
        osAB:	em := 13;
        osBA:	em := 29;
        osBC:	em := 73;
        osBD:	em := 52;
        osFF:	em := 35;
        osFH:	em := 18;
        osFK:	em := 50;
        osFK7:	em := 50;
        osFK8:	em := 50;
        osM1:	em := 15;
        osMA:	em := 4;
        osMT:	em := 20;
        osNA:	em := 13;
        osNB:	em := 50;
        osNB8:	em := 50;
        osNY:	em := 28;
        osXG:	em := -12;
    else
        em := 0;
      end;
    result := em;
end;

function TCoLISBCCalc.getCoordFieldStrength: Double;
begin
    result := _coordFieldStrength;
end;

procedure TCoLISBCCalc.setCoordFieldStrength(fs: Double);
begin
    _coordFieldStrength := fs;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TCoLISBCCalc, Class_CoLISBCCalc,
    ciMultiInstance, tmApartment);

  TTypedComObjectFactory.Create(ComServer, TFreeSpacePropag, Class_CoFreeSpacePropag,
    ciMultiInstance, tmApartment);

end.
