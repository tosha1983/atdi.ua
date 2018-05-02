unit TCALib;

interface
uses
LISBCTxServer_TLB, LISBCCalc_TLB, RSAGeography_TLB, LISPropagation_TLB, Math, Dialogs, SysUtils;

const
  MAX_SEA_POINTS = 50; // максимальное кол-во точек, снимаемое для определения процента моря

procedure TCARunPointToPoint(relief: IRSAGeoPath; spherics: IRSASpherics; rpar: TRSAPathParams; A: TRSAGeoPoint; B: TRSAGeoPoint; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
procedure TCARunOnAzimuth(relief: IRSAGeoPath; spherics: IRSASpherics; rpar: TRSAPathParams; A: TRSAGeoPoint; Az: TRSAAzimuth; Dist: TRSADistance; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
function TCACalcAverageHeight(relief: IRSAGeoPath; spherics: IRSASpherics; p1: TRSAGeoPoint; d, az, step: double): double;
function TCACalcTerrainClearenceAngle(relief: IRSAGeoPath; spherics: IRSASpherics; p1: TRSAGeoPoint; d, az, step, h: double): double;
function TCACalcSeaPercent(relief: IRSAGeoPath; spherics: IRSASpherics; p1: TRSAGeoPoint; d, az, step: double): double;

implementation


{
   _calc: TCoLISBCCalc;
    _relief: IRSAGeoPath;
    _propag: IPropagation;
    _spherics: IRSASpherics;
    _terra: IRSATerrainInfo;

}


function TCACalcAverageHeight(relief: IRSAGeoPath; spherics: IRSASpherics; p1: TRSAGeoPoint; d, az, step: double): double;
var p3: TRSAGeoPoint;
    distance: double;
    alt: smallint;
    n, h_sum: integer;
    terra: IRSATerrainInfo;
begin
  distance := 0;
  relief.Get_TerrainInfo(terra);
  n := 0;
  h_sum := 0;
  while distance <= d do
  begin
    spherics.PolarToGeo(distance, az, p1, p3);
    terra.Get_Altitude(p3, alt);
    h_sum := h_sum + alt;
    n := n + 1;
    distance := distance + step;       
  end;
  if n > 0 then result := h_sum / n else
  begin
    terra.Get_Altitude(p1, alt);
    result := alt;
  end;
end;



function TCACalcSeaPercent(relief: IRSAGeoPath; spherics: IRSASpherics; p1: TRSAGeoPoint; d, az, step: double): double;
var p3: TRSAGeoPoint;
    distance, new_step: double;
    m: TRSAMorpho;
    n, n_sea: integer;
    terra: IRSATerrainInfo;
begin
  distance := 0;
  relief.Get_TerrainInfo(terra);
  new_step := d / MAX_SEA_POINTS;
  step := Max(step, new_step);
  n := 0;
  n_sea := 0;
  while distance <= d do
  begin
    spherics.PolarToGeo(distance, az, p1, p3);
    terra.Get_Morpho(p3, m);
    if m.Kind > 0 then n_sea := n_sea + 1;
    n := n + 1;
    distance := distance + step;
  end;
  if n > 0 then result := 100 * n_sea / n else result := 0;
end;



function TCACalcTerrainClearenceAngle(relief: IRSAGeoPath; spherics: IRSASpherics; p1: TRSAGeoPoint; d, az, step, h: double): double;
var p3: TRSAGeoPoint;
    distance, a, a_max, h0: double;
    alt: smallint;
    terra: IRSATerrainInfo;
begin
  distance := 0;
  relief.Get_TerrainInfo(terra);
  terra.Get_Altitude(p1, alt);
  h0 := alt + h;
  a_max := -9999;
  while distance <= d do
  begin
    spherics.PolarToGeo(distance, az, p1, p3);
    terra.Get_Altitude(p3, alt);
    a := ArcTan2((alt - h0), distance*1000);
    if a > a_max then a_max := a;
    distance := distance + step;
  end;
  result := RadToDeg(a_max);
end;

procedure TCARunPointToPoint(relief: IRSAGeoPath; spherics: IRSASpherics; rpar: TRSAPathParams; A: TRSAGeoPoint; B: TRSAGeoPoint; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
var d, az: double;
begin
  spherics.Azimuth(A,B,az);
  spherics.Distance(A,B,d);
  TCARunOnAzimuth(relief, spherics, rpar, A, az, d, Data, Results);
end;



procedure TCARunOnAzimuth(relief: IRSAGeoPath; spherics: IRSASpherics; rpar: TRSAPathParams; A: TRSAGeoPoint; Az: TRSAAzimuth; Dist: TRSADistance; Data: TRSAGeoPathData; var Results: TRSAGeoPathResults);
var terra: IRSATerrainInfo;
    p1: TRSAGeoPoint;
    step, az_back: double;
    h, h1, h2: smallint;
    fin_dist, d_shift, d, htx, hrx, ang: double;

begin
  step := 0.1;
  d_shift := 3;
  relief.Get_TerrainInfo(terra);
  Results.Distance := Dist;
  Results.Azimuth := Az;
  Results.HEff := NaN;
  Results.TxClearance := NaN;
  Results.RxClearance := NaN;
  Results.SeaPercent := 0;

  if rpar.CalcHEff then
  begin
    if Dist > d_shift then
    begin
      terra.Get_Altitude(A, h);
      spherics.PolarToGeo(3, Az, A, p1); // сдвигаемся на 3 км, т.к. Нэф измеряется на участке 3-15 км.
      fin_dist := Dist;
      if fin_dist > 15 then
        fin_dist := 15; //Нэфф считается на участке 3-15 км, но не более 15
      Results.HEff := Data.TxHeight + h - TCACalcAverageHeight(relief, spherics, p1, fin_dist-d_shift, Az, step);
     end;
  end;

  if rpar.CalcTxClearance then
  begin
    if Dist > 15 then d := 15 else d := Dist;
    Results.TxClearance := TCACalcTerrainClearenceAngle(relief, spherics, A, d, Az, step, Data.TxHeight);
    Results.TxClearance := -Results.TxClearance;
  end;


  if rpar.CalcRxClearance then
  begin
    if Dist > 16 then d := 16 else d := Dist;
    spherics.PolarToGeo(Dist, Az, A, p1);
    spherics.Azimuth(p1, A, az_back);
    Results.RxClearance := TCACalcTerrainClearenceAngle(relief, spherics, p1, d, az_back, step, Data.RxHeight);
    terra.Get_Altitude(A, h1);
    terra.Get_Altitude(p1, h2);
    htx := h1 + Data.TxHeight;
    hrx := h2 + Data.RxHeight;
    ang := RadToDeg(ArcTan2(htx-hrx, Dist*1000));
    Results.RxClearance := Results.RxClearance - ang;
    Results.RxClearance := -Results.RxClearance;
  end;

  if rpar.CalcSeaPercent then
  begin
    Results.SeaPercent := TCACalcSeaPercent(relief, spherics, A, Dist, Az, step);
  end;
end;

end.
