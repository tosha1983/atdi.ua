unit LISBCCalc_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// PASTLWTR : $Revision:   1.130.1.0.1.0.1.6  $
// File generated on 21.08.2012 20:26:46 from Type Library described below.

// ************************************************************************  //
// Type Lib: D:\Projects\_UCRF\BC\BCCalc\LISBCCalc_v2\LISBCCalc.tlb (1)
// LIBID: {6F0974B1-A6D1-11D7-8B30-0090279BA059}
// LCID: 0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v0.1 LISBCTxServer, (D:\Projects\_UCRF\BC\Dll\LISBCTxServer.dll)
//   (3) v1.0 LISPropagation, (D:\Projects\_UCRF\BC\Dll\LISPropag.tlb)
//   (4) v1.0 RSAGeography, (C:\Program Files\ATDI Software\ICS Manager\RSAGeography.dll)
//   (5) v1.0 LISProgress, (D:\Projects\_UCRF\BC\Dll\LISProgress.tlb)
//   (6) v4.0 StdVCL, (C:\WINDOWS\system32\stdvcl40.dll)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
{$WARN SYMBOL_PLATFORM OFF}
{$WRITEABLECONST ON}
{$VARPROPSETTER ON}
interface

uses Windows, ActiveX, Classes, Graphics, LISBCTxServer_TLB, LISProgress_TLB, LISPropagation_TLB, RSAGeography_TLB, 
StdVCL, Variants;
  

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  LISBCCalcMajorVersion = 1;
  LISBCCalcMinorVersion = 0;

  LIBID_LISBCCalc: TGUID = '{6F0974B1-A6D1-11D7-8B30-0090279BA059}';

  IID_ILISBCCalc: TGUID = '{6F0974B2-A6D1-11D7-8B30-0090279BA059}';
  DIID_ILISBCCalcEvents: TGUID = '{BE709300-B768-11D7-8B49-0090279BA059}';
  IID_IParams: TGUID = '{D5467D25-6A00-48F4-88F5-3FD7E12BACC0}';
  CLASS_CoFreeSpacePropag: TGUID = '{B7472F83-25C8-4ADF-8EF5-C4AB27CDE370}';
  IID_IEminCalc: TGUID = '{5DC863FB-6ABC-4FAA-9B0A-07B9C5FDFE5A}';
  IID_ISFNCalc: TGUID = '{4EF5A806-0617-4AF2-8A2B-1244956FDCA0}';
  IID_ICoordZone: TGUID = '{ADD37935-89D1-45F0-9FED-FDD8D9562648}';
  CLASS_CoLISBCCalc: TGUID = '{6F0974B4-A6D1-11D7-8B30-0090279BA059}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum TCalcMethod
type
  TCalcMethod = TOleEnum;
const
  cmPowerSum = $00000000;
  cmSimplified = $00000001;
  cmChester = $00000002;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  ILISBCCalc = interface;
  ILISBCCalcDisp = dispinterface;
  ILISBCCalcEvents = dispinterface;
  IParams = interface;
  IParamsDisp = dispinterface;
  IEminCalc = interface;
  IEminCalcDisp = dispinterface;
  ISFNCalc = interface;
  ISFNCalcDisp = dispinterface;
  ICoordZone = interface;
  ICoordZoneDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  CoFreeSpacePropag = IPropagation;
  CoLISBCCalc = ILISBCCalc;


// *********************************************************************//
// Declaration of structures, unions and aliases.                         
// *********************************************************************//
  PUserType1 = ^TDuelResult; {*}
  PUserType2 = ^TFieldDistribution; {*}
  PUserType3 = ^TDuelResult2; {*}
  PUserType4 = ^TPointDuelResultArray; {*}
  PUserType5 = ^TControlPointCalcResult; {*}

  TDuelResult = packed record
    lon1: Double;
    lat1: Double;
    lon2: Double;
    lat2: Double;
    lon3: Double;
    lat3: Double;
    lon4: Double;
    lat4: Double;
    eu1: Double;
    eu2: Double;
    eu3: Double;
    eu4: Double;
  end;

  TFieldZone = packed record
    zone: Integer;
    e: Double;
  end;

  TFieldDistribution = packed record
    zone1: TFieldZone;
    zone2: TFieldZone;
    zone3: TFieldZone;
    zone4: TFieldZone;
    zone5: TFieldZone;
  end;

  TDuelResult2 = packed record
    Tx1_NoiseLimited: PSafeArray;
    Tx1_InterferenceLimited: PSafeArray;
    Tx2_NoiseLimited: PSafeArray;
    Tx2_InterferenceLimited: PSafeArray;
    Area1_NoiseLimited: Double;
    Area1_InterferenceLimited: Double;
    Area2_NoiseLimited: Double;
    Area2_InterferenceLimited: Double;
    Tx1_Variation_Val: Double;
    Tx1_Variation_Dir: Double;
    Tx2_Variation_Val: Double;
    Tx2_Variation_Dir: Double;
  end;

  TPointDuelResult = packed record
    radius: Double;
    azimuth: Double;
    geoPoint: TRSAGeoPoint;
    emin: Double;
    eInt: Double;
    aPR: Double;
    aDiscr: Double;
    eUsable: Double;
    intType: TBCSInterferenceType;
    aPolar: Double;
  end;

  TPointDuelResultArray = packed record
    point1: TPointDuelResult;
    point2: TPointDuelResult;
    point3: TPointDuelResult;
    point4: TPointDuelResult;
  end;

  TControlPointCalcResult = packed record
    azimuth: Double;
    distance: Double;
    erp: Double;
    heff: Double;
    tx_clearance: Double;
    rx_clearance: Double;
    sea_percent: Double;
    e_50: Double;
    e_10: Double;
    e_1: Double;
    pr_continuous: Double;
    pr_tropospheric: Double;
    ant_discrimination: Double;
    e_usable: Double;
    pol_wanted: TBCPolarization;
    pol_unwanted: TBCPolarization;
    interf_type: TBCSInterferenceType;
  end;


// *********************************************************************//
// Interface: ILISBCCalc
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {6F0974B2-A6D1-11D7-8B30-0090279BA059}
// *********************************************************************//
  ILISBCCalc = interface(IDispatch)
    ['{6F0974B2-A6D1-11D7-8B30-0090279BA059}']
    procedure GetProtectRatio(const tx0: ILISBCTx; const tx1: ILISBCTx; out pr_c: Double; 
                              out pr_t: Double); safecall;
    procedure SetPropagServer(const Propag: IPropagation); safecall;
    procedure SetReliefServer(const Relief: IRSAGeoPath); safecall;
    procedure Init; safecall;
    procedure GetZone_NoiseLimited(const tx: ILISBCTx; step_deg: Double; out zone_km: PSafeArray); safecall;
    function GetUsableFieldStrength(lon: Double; lat: Double): Double; safecall;
    procedure GetZone_InterferenceLimited(step_deg: Double; out zone_km: PSafeArray); safecall;
    procedure CalcDuel(const tx1: ILISBCTx; const tx2: ILISBCTx; var duel_result: TDuelResult); safecall;
    procedure GetFieldDistribution(const tx: ILISBCTx; zone_number: Shortint; 
                                   var distribution: TFieldDistribution); safecall;
    procedure CalcDuel2(const tx1: ILISBCTx; const tx2: ILISBCTx; var duel_result: TDuelResult2); safecall;
    function GetFieldDistribution2(const tx: ILISBCTx; lon1: Double; lat1: Double; lon2: Double; 
                                   lat2: Double; spacing: Double): PSafeArray; safecall;
    function GetFieldDistribution3(const tx: ILISBCTx; a1: Double; a2: Double; da: Double; 
                                   r1: Double; r2: Double; dr: Double): PSafeArray; safecall;
    procedure SetTxListServer(const txlist: ILISBCTxList); safecall;
    function OffsetSelection(offset_start: Integer; offset_finish: Integer): PSafeArray; safecall;
    function ERPSelection(erp_start: Integer; erp_finish: Integer): PSafeArray; safecall;
    procedure CalcInterf_Wanted; safecall;
    procedure CalcInterf_Unwanted; safecall;
    function GetFieldStrength(const tx: ILISBCTx; lon: Double; lat: Double; perc: Integer): Double; safecall;
    procedure SetLogFileName(const filename: WideString); safecall;
    function Get_CalcMethod: TCalcMethod; safecall;
    procedure Set_CalcMethod(Value: TCalcMethod); safecall;
    function Get_CoverageProbability: Double; safecall;
    procedure Set_CoverageProbability(Value: Double); safecall;
    function GetEmin(const tx: ILISBCTx): Double; safecall;
    function GetAntennaDiscrimination(f: Double; azimuth: Double): Double; safecall;
    function GetMaxRadius(d_initial: Double; azimuth: Double): Double; safecall;
    function GetMaxRadiusEmin(const tx: ILISBCTx; d_initial: Double; azimuth: Double; emin: Double): Double; safecall;
    procedure CalcDuelInterf; safecall;
    procedure CalcDuel3(const tx0: ILISBCTx; const tx1: ILISBCTx; 
                        var pointDuelResult: TPointDuelResultArray); safecall;
    procedure GetCoordinationZone(const tx: ILISBCTx; out zone_km: PSafeArray); safecall;
    procedure SetProgressServer(const progress: ILISProgress); safecall;
    procedure GetFieldStrengthControlPoint(const tx_wanted: ILISBCTx; const tx_unwanted: ILISBCTx; 
                                           lon: Double; lat: Double; 
                                           var cp_result: TControlPointCalcResult); safecall;
    procedure GetErpDegradation(idx_unwanted: Integer; eu_threshold: Double; 
                                out degradation: PSafeArray); safecall;
    function GetSumFieldStrength(lon: Double; lat: Double): Double; safecall;
    function GetEContourAllotSubarea(const allot: ILisBcDigAllot; subareaId: Integer; emin: Double): PSafeArray; safecall;
    function GetEAllotSubarea(const allot: ILisBcDigAllot; subareaId: Integer; lon: Double; 
                              lat: Double): Double; safecall;
    function GetEAllot(const allot: ILisBcDigAllot; lon: Double; lat: Double): Double; safecall;
    function GetRNPosition(const allot: ILisBcDigAllot; lon: Double; lat: Double; 
                           azimuth_deg: Double): ILISBCTxList; safecall;
    function GetRNPositionAtBorderPoint(const allot: ILisBcDigAllot; lon: Double; lat: Double; 
                                        azimuth_deg: Double): ILISBCTxList; safecall;
    property CalcMethod: TCalcMethod read Get_CalcMethod write Set_CalcMethod;
    property CoverageProbability: Double read Get_CoverageProbability write Set_CoverageProbability;
  end;

// *********************************************************************//
// DispIntf:  ILISBCCalcDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {6F0974B2-A6D1-11D7-8B30-0090279BA059}
// *********************************************************************//
  ILISBCCalcDisp = dispinterface
    ['{6F0974B2-A6D1-11D7-8B30-0090279BA059}']
    procedure GetProtectRatio(const tx0: ILISBCTx; const tx1: ILISBCTx; out pr_c: Double; 
                              out pr_t: Double); dispid 101;
    procedure SetPropagServer(const Propag: IPropagation); dispid 102;
    procedure SetReliefServer(const Relief: IRSAGeoPath); dispid 103;
    procedure Init; dispid 107;
    procedure GetZone_NoiseLimited(const tx: ILISBCTx; step_deg: Double; 
                                   out zone_km: {??PSafeArray}OleVariant); dispid 108;
    function GetUsableFieldStrength(lon: Double; lat: Double): Double; dispid 109;
    procedure GetZone_InterferenceLimited(step_deg: Double; out zone_km: {??PSafeArray}OleVariant); dispid 110;
    procedure CalcDuel(const tx1: ILISBCTx; const tx2: ILISBCTx; 
                       var duel_result: {??TDuelResult}OleVariant); dispid 111;
    procedure GetFieldDistribution(const tx: ILISBCTx; zone_number: {??Shortint}OleVariant; 
                                   var distribution: {??TFieldDistribution}OleVariant); dispid 120;
    procedure CalcDuel2(const tx1: ILISBCTx; const tx2: ILISBCTx; 
                        var duel_result: {??TDuelResult2}OleVariant); dispid 121;
    function GetFieldDistribution2(const tx: ILISBCTx; lon1: Double; lat1: Double; lon2: Double; 
                                   lat2: Double; spacing: Double): {??PSafeArray}OleVariant; dispid 122;
    function GetFieldDistribution3(const tx: ILISBCTx; a1: Double; a2: Double; da: Double; 
                                   r1: Double; r2: Double; dr: Double): {??PSafeArray}OleVariant; dispid 123;
    procedure SetTxListServer(const txlist: ILISBCTxList); dispid 124;
    function OffsetSelection(offset_start: Integer; offset_finish: Integer): {??PSafeArray}OleVariant; dispid 201;
    function ERPSelection(erp_start: Integer; erp_finish: Integer): {??PSafeArray}OleVariant; dispid 202;
    procedure CalcInterf_Wanted; dispid 203;
    procedure CalcInterf_Unwanted; dispid 204;
    function GetFieldStrength(const tx: ILISBCTx; lon: Double; lat: Double; perc: Integer): Double; dispid 205;
    procedure SetLogFileName(const filename: WideString); dispid 206;
    property CalcMethod: TCalcMethod dispid 207;
    property CoverageProbability: Double dispid 208;
    function GetEmin(const tx: ILISBCTx): Double; dispid 209;
    function GetAntennaDiscrimination(f: Double; azimuth: Double): Double; dispid 210;
    function GetMaxRadius(d_initial: Double; azimuth: Double): Double; dispid 211;
    function GetMaxRadiusEmin(const tx: ILISBCTx; d_initial: Double; azimuth: Double; emin: Double): Double; dispid 212;
    procedure CalcDuelInterf; dispid 213;
    procedure CalcDuel3(const tx0: ILISBCTx; const tx1: ILISBCTx; 
                        var pointDuelResult: {??TPointDuelResultArray}OleVariant); dispid 214;
    procedure GetCoordinationZone(const tx: ILISBCTx; out zone_km: {??PSafeArray}OleVariant); dispid 215;
    procedure SetProgressServer(const progress: ILISProgress); dispid 216;
    procedure GetFieldStrengthControlPoint(const tx_wanted: ILISBCTx; const tx_unwanted: ILISBCTx; 
                                           lon: Double; lat: Double; 
                                           var cp_result: {??TControlPointCalcResult}OleVariant); dispid 217;
    procedure GetErpDegradation(idx_unwanted: Integer; eu_threshold: Double; 
                                out degradation: {??PSafeArray}OleVariant); dispid 218;
    function GetSumFieldStrength(lon: Double; lat: Double): Double; dispid 219;
    function GetEContourAllotSubarea(const allot: ILisBcDigAllot; subareaId: Integer; emin: Double): {??PSafeArray}OleVariant; dispid 220;
    function GetEAllotSubarea(const allot: ILisBcDigAllot; subareaId: Integer; lon: Double; 
                              lat: Double): Double; dispid 221;
    function GetEAllot(const allot: ILisBcDigAllot; lon: Double; lat: Double): Double; dispid 222;
    function GetRNPosition(const allot: ILisBcDigAllot; lon: Double; lat: Double; 
                           azimuth_deg: Double): ILISBCTxList; dispid 223;
    function GetRNPositionAtBorderPoint(const allot: ILisBcDigAllot; lon: Double; lat: Double; 
                                        azimuth_deg: Double): ILISBCTxList; dispid 224;
  end;

// *********************************************************************//
// DispIntf:  ILISBCCalcEvents
// Flags:     (4096) Dispatchable
// GUID:      {BE709300-B768-11D7-8B49-0090279BA059}
// *********************************************************************//
  ILISBCCalcEvents = dispinterface
    ['{BE709300-B768-11D7-8B49-0090279BA059}']
    procedure progress(var perc: Integer); dispid 201;
  end;

// *********************************************************************//
// Interface: IParams
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {D5467D25-6A00-48F4-88F5-3FD7E12BACC0}
// *********************************************************************//
  IParams = interface(IDispatch)
    ['{D5467D25-6A00-48F4-88F5-3FD7E12BACC0}']
    procedure SetParam(const paramName: WideString; Value: OleVariant); safecall;
    function GetParam(paramName: Integer): OleVariant; safecall;
    procedure ConfigDialog; safecall;
    procedure LoadConfig(const regPath: WideString); safecall;
    procedure SaveConfig(const regPath: WideString); safecall;
  end;

// *********************************************************************//
// DispIntf:  IParamsDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {D5467D25-6A00-48F4-88F5-3FD7E12BACC0}
// *********************************************************************//
  IParamsDisp = dispinterface
    ['{D5467D25-6A00-48F4-88F5-3FD7E12BACC0}']
    procedure SetParam(const paramName: WideString; Value: OleVariant); dispid 201;
    function GetParam(paramName: Integer): OleVariant; dispid 202;
    procedure ConfigDialog; dispid 203;
    procedure LoadConfig(const regPath: WideString); dispid 204;
    procedure SaveConfig(const regPath: WideString); dispid 205;
  end;

// *********************************************************************//
// Interface: IEminCalc
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5DC863FB-6ABC-4FAA-9B0A-07B9C5FDFE5A}
// *********************************************************************//
  IEminCalc = interface(IDispatch)
    ['{5DC863FB-6ABC-4FAA-9B0A-07B9C5FDFE5A}']
    function GetEmin(freq: Double): Double; safecall;
  end;

// *********************************************************************//
// DispIntf:  IEminCalcDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5DC863FB-6ABC-4FAA-9B0A-07B9C5FDFE5A}
// *********************************************************************//
  IEminCalcDisp = dispinterface
    ['{5DC863FB-6ABC-4FAA-9B0A-07B9C5FDFE5A}']
    function GetEmin(freq: Double): Double; dispid 201;
  end;

// *********************************************************************//
// Interface: ISFNCalc
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {4EF5A806-0617-4AF2-8A2B-1244956FDCA0}
// *********************************************************************//
  ISFNCalc = interface(IDispatch)
    ['{4EF5A806-0617-4AF2-8A2B-1244956FDCA0}']
    procedure GetSfnZone(const txlist: LISBCTxList; var center_lon: Double; var center_lat: Double; 
                         var emin: Double; out zone_km: PSafeArray); safecall;
  end;

// *********************************************************************//
// DispIntf:  ISFNCalcDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {4EF5A806-0617-4AF2-8A2B-1244956FDCA0}
// *********************************************************************//
  ISFNCalcDisp = dispinterface
    ['{4EF5A806-0617-4AF2-8A2B-1244956FDCA0}']
    procedure GetSfnZone(const txlist: LISBCTxList; var center_lon: Double; var center_lat: Double; 
                         var emin: Double; out zone_km: {??PSafeArray}OleVariant); dispid 201;
  end;

// *********************************************************************//
// Interface: ICoordZone
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {ADD37935-89D1-45F0-9FED-FDD8D9562648}
// *********************************************************************//
  ICoordZone = interface(IDispatch)
    ['{ADD37935-89D1-45F0-9FED-FDD8D9562648}']
    procedure setCoordFieldStrength(fs: Double); safecall;
    function getCoordFieldStrength: Double; safecall;
  end;

// *********************************************************************//
// DispIntf:  ICoordZoneDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {ADD37935-89D1-45F0-9FED-FDD8D9562648}
// *********************************************************************//
  ICoordZoneDisp = dispinterface
    ['{ADD37935-89D1-45F0-9FED-FDD8D9562648}']
    procedure setCoordFieldStrength(fs: Double); dispid 1;
    function getCoordFieldStrength: Double; dispid 2;
  end;

// *********************************************************************//
// The Class CoCoFreeSpacePropag provides a Create and CreateRemote method to          
// create instances of the default interface IPropagation exposed by              
// the CoClass CoFreeSpacePropag. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCoFreeSpacePropag = class
    class function Create: IPropagation;
    class function CreateRemote(const MachineName: string): IPropagation;
  end;

// *********************************************************************//
// The Class CoCoLISBCCalc provides a Create and CreateRemote method to          
// create instances of the default interface ILISBCCalc exposed by              
// the CoClass CoLISBCCalc. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCoLISBCCalc = class
    class function Create: ILISBCCalc;
    class function CreateRemote(const MachineName: string): ILISBCCalc;
  end;

implementation

uses ComObj;

class function CoCoFreeSpacePropag.Create: IPropagation;
begin
  Result := CreateComObject(CLASS_CoFreeSpacePropag) as IPropagation;
end;

class function CoCoFreeSpacePropag.CreateRemote(const MachineName: string): IPropagation;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CoFreeSpacePropag) as IPropagation;
end;

class function CoCoLISBCCalc.Create: ILISBCCalc;
begin
  Result := CreateComObject(CLASS_CoLISBCCalc) as ILISBCCalc;
end;

class function CoCoLISBCCalc.CreateRemote(const MachineName: string): ILISBCCalc;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CoLISBCCalc) as ILISBCCalc;
end;

end.
