unit UZRoutine;

interface
uses Classes, SysUtils, Math, UShare, Variants, Dialogs;

const
//  _MIN_CH = 21;
//  _MAX_CH = 61;
  _WRONG_SYMB = [' ', #9];
  _Z01 = 'Z';
  _Z02 = '0';
  _Z03 = '1';
type
 // TString3 = string[3];
  TChannelKind = (ckAssigned, ckPlanned, ckAllowed);
//  TChannel = byte;
//  TAllChannels = ('6A', '5A');
  TChannel2 = string[3];
  TZPoint = record
    case integer of
    0: (
         lon: double;
         lat: double;
       );
    1: (
         X: double;
         Y: double;
       )
  end;
TFirstNext = (fnFirst, fnNext);

//TTriangleType = (trLeft, trRight);

TDA1 = class(TStringList)
private
  _eof: boolean;
  _eoc: booleaN; // End Of Contour
  _itemindex: integer;
public
    function ContourIdx(firstnext: TFirstNext): integer;
//    procedure ContourToSTringList(ctridx: integer; sl: TStringList);
    function ContourToSTringList(ctridx: integer; sl: TStringList): string;
    function ContourPoint(firstnext: TFirstNext): TZPoint;
    function RemoveWrongSymbols(s: string): string;
    constructor Create;
    property EoF: boolean read _eof;
end;

TDVBZone = class(TObject)
private
//  _ch: TChannel; // присвоенный канал, если такого нет, то =0
//  _chplan: TChannel; // канал, присвоенный при планировании
  _chassi_list: TList; // список присвоенных каналов
  _chplan_list: TList;  // каналы, присвоенный при планировании
  _challo_list: TList;     // список разрешенных каналов, если пустой - то разрешены все каналы
  _points: TList;     // точки полигона
  _available_for_planning: boolean;  // можно ли использовать для планирования (например, заграничные - нельзя)
  _name: string;
  _max_dist: INTEGER;
  _id: integer;
    function GetZoneType: double;
public
  constructor Create;
  procedure AddPoint(lon, lat: double);
  procedure AddPointString(s: string);
    procedure LoadFromFile(f: string);
    function GetPointsCount: integer;
    function GetLon(idx: integer): double;
    function GetLat(idx: integer): double;
    procedure ShowZoneDlg;
    procedure AddChannel(ch: TChannel2; ck: TChannelKind);
    procedure ClearChannels(ck: TChannelKind);
    procedure ClearPoints;
    procedure LoadFromStringList(l: TStringList);
    function GetChannelsCount(ck: TChannelKind): integer;
    function GetAllowedChannel(idx: integer): TChannel2;
    procedure LoadChannelsFromStringList(l: TStringList; ck: TChannelKind);
//    procedure SetCh(ch: TChannel);
//    procedure SetChPlan(ch: Tchannel);
//    property AssignedChannel: TChannel read _ch write SetCh;
//    property PlannedChannel: TChannel read _chplan write SetChPlan;
    function GetPoint(idx: integer): TZPoint;
    function GetDistanceToZone(z2: TDVBZone): double;
    property Name: string read _name write _name;
    function GetAssignedChannel(idx: integer): TChannel2;
    function GetChannel(idx: integer; ck: TChannelKind): TChannel2;
    function ChanelInList(ch: TChannel2; ck: TChannelKind): boolean;
    property AvailableForPlanning: boolean read _available_for_planning write _available_for_planning;
    procedure SaveToStream(stream: TStream);
    procedure ReadFromStream(stream: TStream);
    property MaxDist: integer read _max_dist write _max_dist;
    property Id: integer read _id write _id;
//    function PointInZone(lon, lat: double): boolean;
    function PointInZone(lon, lat: double): boolean;
    procedure DeletePoint(idx: integer);
    procedure CreateMinDistanceContour(z: TDVBZone; step_km: double);
    function PointsCount: integer;
end;

TCalcEvent = procedure (z: TDVBZone);
TCalcEventPercent = procedure (perc: integer);

TDVBZoneList = class(TList)
  private
    _matrix: Variant;
//    _min_distance: double;
    _min_ch: TChannel2;
    _max_ch: TChannel2;
    _calc_event: TCalcEvent;
    _calc_event_perc: TCalcEventPercent;
    _name: string;
  public
    procedure AddZone(z: TDVBZone);
    procedure CreateDistanceMatrixArray;
    constructor Create;
    function GetZone(idx: integer): TDVBZone;
    function GetDistance(z1idx, z2idx: integer): double;
    function GetBestZoneIdx(ch: TChannel2): integer;
    function GetCoeffOptimal1(ch: TChannel2; idx: integer): double;
    procedure CalcPlannedChannels;
//    procedure SetCalcEvent(p: TCalcEvent);

    property OnCalcEvent: TCalcEvent read _calc_event write _calc_event;
    property OnCalcEventPercent: TCalcEventPercent read _calc_event_perc write _calc_event_perc;
    property MaxCh: TChannel2 read _max_ch write _max_ch;
    property MinCh: TChannel2 read _min_ch write _min_ch;
    property Name: string read _name write _name;
    procedure DoCalcEvent(z: TDVBZone);
    procedure DoCalcEventPercent(perc: integer);
//    property MinDistance: double read _min_distance write _min_distance;
    function ChannelIsFree(ch: TChannel2; idx: integer): boolean;
    function GetMaxChannelsCount(ck: TChannelKind): integer;
    function GetCoeffOptimal2(ch: TChannel2; idx: integer): double;
    procedure ShowStatistic;
    function GetCoeffOptimal(ch: TChannel2; idx: integer): double;
    procedure SaveToStream(stream: TStream);
    procedure LoadFromStream(stream: TStream);
    procedure SaveToFile(fn: string);
    procedure LoadFromFile(fn: string);
    procedure LoadFromDA1(filename: string);

end;

function RegionDistance(z1, z2: TDVBZone): double;
//function GetTriangleType(x1, y1, x2, y2, x3, y3: double): TTriangleType;
function GetTriangleType(x1, y1, x2, y2, x3, y3: double): double;
function GetAzimuthDeg(x1, y1, x2, y2: double): double;
function PointInTriangle(x1, y1, x2, y2, x3, y3, xp, yp: double): boolean;
function Geron(a, b, c: double): double;
function LineLength(x1, y1, x2, y2: double): double;
function RegionToPointDistance(z: TDVBZone; p1: TZPoint; var az_deg: double): double;
function PointToPointDistance(x1, y1, x2, y2: double): double;


implementation
uses UZDialog;



procedure TDVBZone.AddChannel(ch: TChannel2; ck: TChannelKind);
var p: ^TChannel2;
begin
  New(p);
  p^ := ch;
  case ck of
    ckAllowed: _challo_list.Add(p);
    ckAssigned: _chassi_list.Add(p);
    ckPlanned: _chplan_list.Add(p);
  end;
end;



{
procedure TDVBZoneList.SetCalcEvent(p: TCalcEvent);
begin
  _calc_event := p;
end;
}

procedure TDVBZone.AddPoint(lon, lat: double);
var p: ^TZPoint;
begin
  new(p);
  p^.lon := lon;
  p^.lat := lat;
  _points.Add(p);
end;



procedure TDVBZone.AddPointString(s: string);
var lon, lat: double;
    slon, slat: string;
    n: integer;
begin
  while (s[1] = ' ') or (s[1]=#09) do Delete(s,1,1);
  while (s[Length(s)] = ' ') or (s[Length(s)]=#09) do Delete(s,Length(s),1);
  n := Max(Pos(' ', s), Pos(#09, s));
  if n > 0 then
  begin
    slon := Copy(s, 1, n-1);
    slat := Copy(s, n+1, Length(s) - n);
    lon := StrToFloat(slon);
    lat := StrToFloat(slat);
    AddPoint(lon, lat);
  end;
end;



function TDVBZone.ChanelInList(ch: TChannel2; ck: TChannelKind): boolean;
var l: TList;
    i: integer;
    p: ^TChannel2;
begin
  result := false;
  case ck of
    ckAssigned: l := _chassi_list;
    ckAllowed: l := _challo_list;
    ckPlanned: l := _chplan_list;
  else
    l := _challo_list;
  end;
  for i := 0 to l.Count-1 do
  begin
    p := l.Items[i];
    if p^ = ch then
    begin
      result := true;
      Exit;
    end;
  end;


end;

procedure TDVBZone.ClearChannels(ck: TChannelKind);
begin
  case ck of
    ckAllowed: _challo_list.Clear;
    ckAssigned: _chassi_list.Clear;
    ckPlanned: _chplan_list.Clear;
  else
    _challo_list.Clear;
  end;
end;



procedure TDVBZone.ClearPoints;
begin
  _points.Clear;
end;



constructor TDVBZone.Create;
begin
  Inherited Create;
  _challo_list := TList.Create;
  _points := TList.Create;
  _chassi_list := TList.Create;
  _chplan_list := TList.Create;
  _available_for_planning := true;
  _max_dist := 100;

//  _ch := 0;
//  _chplan := 0;
end;



{
  Создает из нашей зоны контур мин. дистанции повторяемости для
  зоны z.
}
procedure TDVBZone.CreateMinDistanceContour(z: TDVBZone; step_km: double);
var i: integer;
    minlon, minlat: double;
    lon, lat: double;
//    nextlon, nextlat: double;
    d,az: double;
    p1: TZPoint;
    az_back, az_norm: double;
//    counter: integer;
    lastpoint: TZPoint;
    firstpoint: TZPoint;
begin
  if z.PointsCount <= 0 then Exit;

  ClearChannels(ckAssigned);
  ClearChannels(ckPlanned);
  ClearChannels(ckAllowed);
  ClearPoints;
{
  Ищем отправную точку - любую, лежащую вне исходной зоны.
  Для этого находим самую минимальную широту и долготу.
}
  minlon := 1000000;
  minlat := 1000000;

  for i := 0 to z.PointsCount-1 do
  begin
    if z.GetLon(i) < minlon then minlon := z.GetLon(i);
    if z.GetLat(i) < minlat then minlat := z.GetLat(i);
  end;

  lon := minlon;
  lat := minlat;
//  counter := 0;
  repeat
    p1.lon := lon;
    p1.lat := lat;
    d := RegionToPointDistance(z, p1, az);

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
    if d < _max_dist then GetNextCoord(az_back, _max_dist-d, p1.lon, p1.lat);
    if d > _max_dist then GetNextCoord(az, d - _max_dist, p1.lon, p1.lat);

    AddPoint(p1.lon, p1.lat);
  {
    Рассчитываем следующую точку - в сторону от предыдущей
    на расст. step_km
  }
    lon := p1.lon;
    lat := p1.lat;
    GetNextCoord(az_norm, step_km, lon, lat);
//    counter := counter + 1;
    lastpoint.lon := p1.lon;
    lastpoint.lat := p1.lat;

    firstpoint.lon := GetLon(0);
    firstpoint.lat := GetLat(0);

  until (PointToPointDistance(lastpoint.lon, lastpoint.lat, firstpoint.lon, firstpoint.lat) <= 10) and (PointsCount > 2);

end;



procedure TDVBZone.DeletePoint(idx: integer);
var p: ^TZPoint;
begin
  p := _points.Items[idx];
  Dispose(p);
  _points.Delete(idx);
end;

function TDVBZone.GetAllowedChannel(idx: integer): TChannel2;
begin
  result := GetChannel(idx, ckAllowed);
end;



function TDVBZone.GetAssignedChannel(idx: integer): TChannel2;
begin
  result := GetChannel(idx, ckAssigned);
end;



function TDVBZone.GetChannel(idx: integer; ck: TChannelKind): TChannel2;
var p: ^TChannel2;
begin
  case ck of
    ckAssigned: p := _chassi_list.Items[idx];
    ckAllowed: p := _challo_list.Items[idx];
    ckPlanned: p := _chplan_list.Items[idx];
  else
    p := _challo_list.Items[idx];
  end;
  Result := p^;
end;



function TDVBZone.GetChannelsCount(ck: TChannelKind): integer;
begin
  case ck of
    ckAssigned: result := _chassi_list.Count;
    ckPlanned: result := _chplan_list.Count;
    ckAllowed: result := _challo_list.Count;
  else
    result := _challo_list.Count;
  end;

end;



function TDVBZone.GetDistanceToZone(z2: TDVBZone): double;
begin
  result := RegionDistance(self, z2);
end;

function TDVBZone.GetLat(idx: integer): double;
var p: ^TZPoint;
begin
  p := _points.Items[idx];
  result := p^.lat;
end;



function TDVBZone.GetLon(idx: integer): double;
var p: ^TZPoint;
begin
  p := _points.Items[idx];
  result := p^.lon;
end;



function TDVBZone.GetPoint(idx: integer): TZPoint;
var p: ^TZPoint;
begin
  p := _points.Items[idx];
  result := p^;
end;

function TDVBZone.GetPointsCount: integer;
begin
  result := _points.Count;
end;


{
  Тип зоны - левый или правый
}
function TDVBZone.GetZoneType: double;
var i, n: integer;
    x1,x2,x3: double;
    y1,y2,y3: double;
    s, tt: double;
begin
  n := GetPointsCount;
  if n >= 3 then
  begin
    s := 0;
    for i := 0 to n - 3 do
    begin
      x1 := GetLon(i);
      y1 := GetLat(i);
      x2 := GetLon(i+1);
      y2 := GetLat(i+1);
      x3 := GetLon(i+2);
      y3 := GetLat(i+2);
      tt := GetTriangleType(x1,y1,x2,y2,x3,y3);
      s := s + tt
    end;

    x1 := GetLon(n-2);
    y1 := GetLat(n-2);
    x2 := GetLon(n-1);
    y2 := GetLat(n-1);
    x3 := GetLon(0);
    y3 := GetLat(0);
    tt := GetTriangleType(x1,y1,x2,y2,x3,y3);
    s := s + tt;

    x1 := GetLon(n-1);
    y1 := GetLat(n-1);
    x2 := GetLon(0);
    y2 := GetLat(0);
    x3 := GetLon(1);
    y3 := GetLat(1);
    tt := GetTriangleType(x1,y1,x2,y2,x3,y3);
    s := s + tt;

    result := s;
  end else result := 0;

end;



procedure TDVBZone.LoadChannelsFromStringList(l: TStringList; ck: TChannelKind);
var i: integer;
    s: string;
    ch: TChannel2;
begin
  for i := 0 to l.Count-1 do
  begin
    s := l.Strings[i];
    ch := s;
    AddChannel(ch, ck);
  end;
end;



procedure TDVBZone.LoadFromFile(f: string);
var l: TStringList;
begin
  l := TStringList.Create;
  l.LoadFromFile(f);
  LoadFromStringList(l);
  _name := ExtractFileName(f);
  _name := ChangeFileExt(_name, '');
  l.Free;
end;



procedure TDVBZone.LoadFromStringList(l: TStringList);
var i: integer;
    s: string;
begin
  for i := 0 to l.Count-1 do
  begin
    s := l.Strings[i];
    if s <> '' then  AddPointString(s);
  end;
end;


{
procedure TDVBZone.SetCh(ch: TChannel);
begin
  _ch := ch;
end;



procedure TDVBZone.SetChPlan(ch: Tchannel);
begin
  _chplan := ch;
end;
}



function TDVBZone.PointInZone(lon, lat: double): boolean;
var zonetype: double;
    z: TDVBZone;
    i, i2, i3, n, k: integer;
    x,y: double;
    x0,y0: double;
    x1,y1: double;
    x2,y2: double;
    x3,y3: double;
    xp, yp: double;
    badtriangle: boolean;
begin
  result := false;

  z := TDVBZone.Create;

  n := GetPointsCount;
  if n < 3 then
  begin
    result := false;
    Exit;
  end;
  x0 := -999;
  y0 := -999;
  for i := 0 to n-1 do
  begin
    x := GetLon(i);
    y := GetLat(i);
    if not ((x0 = x) and (y0 = y)) then z.AddPoint(x, y);
    x0 := x;
    y0 := y;
  end;

  x := z.GetLon(z.GetPointsCount-1);
  y := z.GetLat(z.GetPointsCount-1);
  x0 := z.GetLon(0);
  y0 := z.GetLat(0);
  if ((x0 = x) and (y0 = y)) then z.DeletePoint(z.GetPointsCount-1);

  zonetype := z.GetZoneType;
{
  Если зона сильно перектрученая, то ну ее на фиг!!!
}
  if Abs(zonetype) < 300 then Exit;

  i := 0;
  while z.GetPointsCount >= 3 do
  begin
    x1 := z.GetLon(i);
    y1 := z.GetLat(i);

    i2 := i + 1;
    if i2 >= z.GetPointsCount then i2 := 0;
    x2 := z.GetLon(i2);
    y2 := z.GetLat(i2);

    i3 := i2 + 1;
    if i3 >= z.GetPointsCount then i3 := 0;
    x3 := z.GetLon(i3);
    y3 := z.GetLat(i3);


{
  Если рассматриваемый треугольник (вершина нашего полигона)
  выпуклый, тогда ищем там точку
}
    if (zonetype * GetTriangleType(x1,y1,x2,y2,x3,y3) >= 0) then
    begin
      badtriangle := false;
      for k := 0 to z.GetPointsCount-1 do
      begin
{
  Если какая-то другая точка из нашей зоны
  попадает в выбранный треугольник, значит
  треугольник не подходит и мы его пропускаем
}
        xp := z.GetLon(k);
        yp := z.GetLat(k);
        if PointInTriangle(x1,y1,x2,y2,x3,y3,xp,yp)
        and (k <> i)
        and (k <> i2)
        and (k <> i3) then badtriangle := true;
      end;
{
  Если треугольник подходящий но точку там не находим,
  значит удаляем вершину и анализируем дальше
}
      if not badtriangle then
      begin
{
  наша вершина - i2
}
        z.DeletePoint(i2);
        if PointInTriangle(x1,y1,x2,y2,x3,y3,lon,lat) then
        begin
          result := true;
          z.ClearPoints;
        end;
      end;
    end;

    i := i + 1;
    if i >= z.GetPointsCount then i := 0;
  end;

  z.Free;

end;



function TDVBZone.PointsCount: integer;
begin
  result := _points.Count;
end;

procedure TDVBZone.ReadFromStream(stream: TStream);
var i, n: integer;
    ch: char;
//    d: double;
    pchannel: ^TChannel2;
    pzpoint: ^TZPoint;
    s16: string[16];
    s3: string[3];
begin
  s3 := '';
  stream.Read(ch, SizeOf(ch));
  s3 := s3 + ch;
  stream.Read(ch, SizeOf(ch));
  s3 := s3 + ch;
  stream.Read(ch, SizeOf(ch));
  s3 := s3 + ch;
  if not (s3 = 'Z01') then Exit;

  _chassi_list.Clear;
  _chplan_list.Clear;
  _challo_list.Clear;
  _points.Clear;
  stream.Read(n, SizeOf(n));
  for i := 0 to n-1 do
  begin
    New(pchannel);
    stream.Read(pchannel^, SizeOf(TChannel2));
    _chassi_list.Add(pchannel);
  end;

  stream.Read(n, SizeOf(n));
  for i := 0 to n-1 do
  begin
    New(pchannel);
    stream.Read(pchannel^, SizeOf(TChannel2));
    _chplan_list.Add(pchannel);
  end;

  stream.Read(n, SizeOf(n));
  for i := 0 to n-1 do
  begin
    New(pchannel);
    stream.Read(pchannel^, SizeOf(TChannel2));
    _challo_list.Add(pchannel);
  end;

  stream.Read(n, SizeOf(n));
  for i := 0 to n-1 do
  begin
    New(pzpoint);
    stream.Read(pzpoint^, SizeOf(TZPoint));
    _points.Add(pzpoint);
  end;

  stream.Read(_available_for_planning, SizeOf(_available_for_planning));
  stream.Read(s16,SizeOf(s16));
  _name := s16;
  {
  _chassi_list: TList; // список присвоенных каналов
  _chplan_list: TList;  // каналы, присвоенный при планировании
  _challo_list: TList;     // список разрешенных каналов, если пустой - то разрешены все каналы
  _points: TList;     // точки полигона
  _available_for_planning: boolean;  // можно ли использовать для планирования (например, заграничные - нельзя)
  _name: string;

}

end;

procedure TDVBZone.SaveToStream(stream: TStream);
var i, n: integer;
    ch: char;
//    d: double;
    pchannel: ^TChannel2;
    pzpoint: ^TZPoint;
    s16: string[16];
begin
  ch := _Z01;
  stream.Write(ch, 1);
  ch := _Z02;
  stream.Write(ch, 1);
  ch := _Z03;
  stream.Write(ch, 1);

  n := _chassi_list.Count;
  stream.Write(n, SizeOf(n));
  for i := 0 to _chassi_list.Count-1 do
  begin
    pchannel := _chassi_list.Items[i];
    stream.Write(pchannel^, SizeOf(TChannel2));
  end;

  n := _chplan_list.Count;
  stream.Write(n, SizeOf(n));
  for i := 0 to _chplan_list.Count-1 do
  begin
    pchannel := _chplan_list.Items[i];
    stream.Write(pchannel^, SizeOf(TChannel2));
  end;


  n := _challo_list.Count;
  stream.Write(n, SizeOf(n));
  for i := 0 to _challo_list.Count-1 do
  begin
    pchannel := _challo_list.Items[i];
    stream.Write(pchannel^, SizeOf(TChannel2));
  end;

  n := _points.Count;
  stream.Write(n, SizeOf(n));
  for i := 0 to _points.Count-1 do
  begin
    pzpoint := _points.Items[i];
    stream.Write(pzpoint^, SizeOf(TZpoint));
  end;

  stream.Write(_available_for_planning, SizeOf(_available_for_planning));
  s16 := _name;
  stream.Write(s16, SizeOf(s16));

  {
  _chassi_list: TList; // список присвоенных каналов
  _chplan_list: TList;  // каналы, присвоенный при планировании
  _challo_list: TList;     // список разрешенных каналов, если пустой - то разрешены все каналы
  _points: TList;     // точки полигона
  _available_for_planning: boolean;  // можно ли использовать для планирования (например, заграничные - нельзя)
  _name: string;

}
end;

procedure TDVBZone.ShowZoneDlg;
var
  ZDialog: TZDialog;
begin
   ZDialog := TZDialog(TZDialog.NewInstance);
    try
        ZDialog.Create(Nil);
        ZDialog.SetZ(Self);
        ZDialog.ShowModal;
    except
//        ZDialog := nil;
        raise;
    end;
end;



procedure TDVBZoneList.AddZone(z: TDVBZone);
begin
  Add(z);
end;






{
function PointToPointDistance(x1, y1, x2, y2: double): double;
var x,y,k: double;
begin
  k:=1; //Coefficient for Latitude and longitude
  x:=k*abs(x2-x1);
  y:=k*abs(y2-y1);
  result:=sqrt(sqr(x)+sqr(y));
end;
}

function PointToPointDistance(x1, y1, x2, y2: double): double;
begin
  result := UShare.GetDistance(x1,y1,x2,y2);
end;



function PointToPointAzimuth(x1, y1, x2, y2: double): double;
begin
  result := UShare.GetAzimuthDeg(x1,y1,x2,y2);
end;


function MinDistance2(x1,y1,x2,y2,x3,y3: double): double;
const e=1;
var p1p2, p1p3, p2p3: double;
    ang_p2, ang_p3: double;
    half_pi: double;
    a: double;
    Sqp1p2, Sqp1p3: double;
begin
  p1p2:=PointToPointDistance(x1, y1, x2, y2);
  if p1p2<e then
  begin
     result:=0;
     exit;
  end;
  p1p3:=PointToPointDistance(x1, y1, x3, y3);
  if p1p3<e then
  begin
     result:=0;
     exit;
  end;
  p2p3:=PointToPointDistance(x2, y2, x3, y3);
  if p2p3<e then
  begin
     result:=p1p3;
     exit;
  end;
  half_pi:=pi/2;
  sqp1p2:=Sqr(p1p2);
  sqp1p3:=Sqr(p1p3);
  a:=(Sqp1p2+p2p3*p2p3-Sqp1p3)/(2*p1p2*p2p3);
  ang_p2:=arccos(a);
  if ang_p2>half_pi then
  begin
          result:=p1p2;
          exit;
  end;
  a:=(Sqp1p3+p2p3*p2p3-Sqp1p2)/(2*p1p3*p2p3);
  ang_p3:=arccos(a);
  if ang_p3>half_pi then
  begin
          result:=p1p3;
          exit;
  end;
  result:=p1p2*sin(ang_p2);
end;



{
  Расчет  расстояния от точки до отрезка прямой (ребра зоны).
  Также расчитывает азимут до ближайшей точки ребра.
}
function MinDistance(P1, P2, P3: TZPoint; var az_deg: double): double;
const e=1;
var p1p2, p1p3, p2p3: double;
    ang_p2, ang_p3: double;
    half_pi: double;
    a: double;
    Sqp1p2, Sqp1p3: double;
    az2: double;
    lon, lat: double;
    d: double;
begin
  p1p2:=PointToPointDistance(P1.X, P1.Y, P2.X, P2.Y);
  if p1p2<e then
     begin
       result:=0;
       az_deg := PointToPointAzimuth(P1.X, P1.Y, P2.X, P2.Y);
       exit;
     end;
  p1p3:=PointToPointDistance(P1.X, P1.Y, P3.X, P3.Y);
  if p1p3<e then
     begin
       result := 0;
       az_deg := PointToPointAzimuth(P1.X, P1.Y, P3.X, P3.Y);
       exit;
     end;
  p2p3:=PointToPointDistance(P2.X, P2.Y, P3.X, P3.Y);
  if p2p3<e then
     begin
       result:=p1p3;
       az_deg := PointToPointAzimuth(P1.X, P1.Y, P3.X, P3.Y);
       exit;
     end;
  half_pi:=pi/2;
  sqp1p2:=Sqr(p1p2);
  sqp1p3:=Sqr(p1p3);
  a:=(Sqp1p2+p2p3*p2p3-Sqp1p3)/(2*p1p2*p2p3);
  if a > 1 then a := 1;
  if a < -1 then a := -1;
  ang_p2:=arccos(a);
  if ang_p2>half_pi then
          begin
            result:=p1p2;
            az_deg := PointToPointAzimuth(P1.X, P1.Y, P2.X, P2.Y);
            exit;
          end;
  a:=(Sqp1p3+p2p3*p2p3-Sqp1p2)/(2*p1p3*p2p3);
  if a > 1 then a := 1;
  if a < -1 then a := -1;
  ang_p3:=arccos(a);
  if ang_p3>half_pi then
          begin
            result:=p1p3;
            az_deg := PointToPointAzimuth(P1.X, P1.Y, P3.X, P3.Y);
            exit;
          end;
  result:=p1p2*sin(ang_p2);

  d := p1p2 * cos(ang_p2);
  az2 := PointToPointAzimuth(P2.X, P2.Y, P3.X, P3.Y);
  lon := p2.lon;
  lat := p2.lat;
  GetNextCoord(az2, d, lon, lat);

  az_deg := PointToPointAzimuth(P1.X, P1.Y, lon, lat);

end;



//function RegionToPointDistance(Map1: TMap; reg1: feature; p1: Point): double;
function RegionToPointDistance(z: TDVBZone; p1: TZPoint; var az_deg: double): double;
const e=1;
var i1: integer;
    c1: integer;
    d, dmin: double;
    az: double;
begin
  //c1:=reg1.Parts.Item[1].Count;
  c1:=z.GetPointsCount;

  dmin:=MinDistance(p1, z.GetPoint(1), z.GetPoint(2), az_deg);

  for i1:=0 to c1-2 do
  begin
    d:=MinDistance(p1, z.GetPoint(i1), z.GetPoint(i1+1), az);
    if d<e then
            begin
              result:=d;
              az_deg := az;
              exit;
            end;
    if d<=dmin then
    begin
      dmin:=d;
      az_deg := az;
    end;
  end;
  d:=MinDistance(p1, z.GetPoint(0), z.GetPoint(c1-1), az);
  if d<e then
  begin
    result:=d;
    az_deg := az;
    exit;
  end;
  if d<=dmin then
  begin
    dmin:=d;
    az_deg := az;
  end;
  result:=dmin;
end;



function RegionDistance(z1, z2: TDVBZone): double;
const e=1;
var i1, i2: integer;
    c1, c2: integer;
    d, dmin: double;
    az: double;
begin

if z1 = z2 then
begin
  result := 0;
  Exit;
end;

c1:=z1.GetPointsCount;
c2:=z2.GetPointsCount;

dmin:=MinDistance(z1.GetPoint(1), z2.GetPoint(1), z2.GetPoint(2), az);

for i1:=0 to c1-1 do
    begin
    for i2:=1 to c2-1 do
        begin
        d:=MinDistance(z1.GetPoint(i1), z2.GetPoint(i2-1), z2.GetPoint(i2), az);
        if d<e then
                begin
                result:=d;
                exit;
                end;
        if d<=dmin then dmin:=d;
        end;
    d:=MinDistance(z1.GetPoint(i1), z2.GetPoint(0), z2.GetPoint(c2-1),az);
    if d<e then
                begin
                result:=d;
                exit;
                end;
    if d<=dmin then dmin:=d;
    end;

for i1:=0 to c2-1 do
    begin
    for i2:=1 to c1-1 do
        begin
        d:=MinDistance(z2.GetPoint(i1), z1.GetPoint(i2-1), z1.GetPoint(i2),az);
        if d<e then
                begin
                result:=d;
                exit;
                end;
        if d<=dmin then dmin:=d;
        end;
    d:=MinDistance(z2.GetPoint(i1), z1.GetPoint(0), z1.GetPoint(c1-1),az);
    if d<e then
                begin
                result:=d;
                exit;
                end;
    if d<=dmin then dmin:=d;
    end;
result:=dmin;
end;



{
  Распределяем каналы по зонам
}
procedure TDVBZoneList.CalcPlannedChannels;
var i, n: integer;
    z: TDVBZone;
    idx_first: integer;
    ch_first: TChannel2;
    idx_best, xxx_int: integer;
begin
  Exit;
  CreateDistanceMatrixArray;
  idx_first := -1;
{
  копируем присвоенные каналы в список каналов планирования
}
  for i := 0 to Count-1 do
  begin
    z := GetZone(i);

    z.ClearChannels(ckPlanned);
    for n := 0 to z.GetChannelsCount(ckAssigned)-1 do z.AddChannel(z.GetChannel(n, ckAssigned), ckPlanned);
//    z.PlannedChannel := z.AssignedChannel;

    if (idx_first = -1) and (z.AvailableForPlanning) then idx_first := i;
  end;

  z := GetZone(idx_first);
  if z.GetChannelsCount(ckAllowed) > 0 then ch_first := z.GetAllowedChannel(0) else ch_first := _MIN_CH;
//  xxx_int := _max_ch - ch_first;
  while ch_first <= _MAX_CH do
  begin
    repeat
      idx_best := GetBestZoneIdx(ch_first);
      if idx_best >=0 then
      begin
        z := GetZone(idx_best);
        z.AddChannel(ch_first, ckPlanned);
        DoCalcEvent(z);
      end
    until idx_best < 0;
//    ch_first := ch_first + 1;
//    DoCalcEventPercent(Round(100 - 100 * (_max_ch - ch_first)/xxx_int));
  end;

end;



function TDVBZoneList.ChannelIsFree(ch: TChannel2; idx: integer): boolean;
var i,  n2: integer;
    z2, z1: TDVBZone;
    d: double;
    ch2: TChannel2;
    _min_distance: double;
begin
  result := true;
{
  перебираем все зоны и для всех близлежащих зон просматриваем список
  каналов планирования
}
  for i := 0 to Count - 1 do
  begin


    d := GetDistance(idx, i);
    z2 := GetZone(i);
    z1 := GetZone(idx);

    _min_distance := Max(z1.MaxDist, z2.MaxDist);

    if d < _min_distance then
    begin
      z2 := GetZone(i);
      for n2 := 0 to z2.GetChannelsCount(ckPlanned)-1 do
      begin
        ch2 := z2.GetChannel(n2, ckPlanned);

        if ch = ch2 then
        begin
{
              if (ch=25) and (idx = 10) then
              begin
                beep;
              end;
}
          result := false;
          Exit;
        end;
      end;
    end;
  end;
end;



constructor TDVBZoneList.Create;
begin
  inherited Create;
//  _min_distance := 100; // km
  _min_ch := '55';
  _max_ch := '69';
end;



procedure TDVBZoneList.CreateDistanceMatrixArray;
var n: integer;
    i, j: integer;
    zi, zj: TDVBZone;
    d: double;
    perc: integer;
begin
  n := Count-1;
  _matrix := VarArrayCreate([0, n, 0, n], varDouble);

  for i := 0 to n do
    for j := 0 to n do
    begin
      zi := GetZone(i);
      zj := GetZone(j);
      d := zi.GetDistanceToZone(zj);
      VarArrayPut(_matrix, d, [i, j]);
      perc := Round(100 * (i*n + j)/(n*n));
      DoCalcEventPercent(perc);
    end;
end;



procedure TDVBZoneList.DoCalcEvent(z: TDVBZone);
begin
  _calc_event(z);
end;



procedure TDVBZoneList.DoCalcEventPercent(perc: integer);
begin
  _calc_event_perc(perc);
end;



{
  возвращает индекс зоны, где лучше всего использовать этот канал
}
function TDVBZoneList.GetBestZoneIdx(ch: TChannel2): integer;
var i: integer;
    z: TDVBZone;
    no_zones: boolean;
    best_zone_idx: integer;
    best_zone_coef, sd: double;
    max_ch_count: integer;
    ch_count, min_ch_count: integer;
begin
//  Полагем, что зон с таким каналом нет
  no_zones := true;
  result := -1;
  min_ch_count := 1000000000;

  for i := 0 to Count-1 do
  begin
    z := GetZone(i);
{
  Проверяем наличие зон с заданным каналов в списке  плаирования,
  заодно запоминаем любую зону, в которой этот канал не используется.

  Если ни в одной зоне данный канал не исп. - просто возвращаем запомненный индекс.
}
    if z.ChanelInList(ch, ckPlanned) then no_zones := false else
    begin
       if z.GetChannelsCount(ckPlanned) < min_ch_count then
       begin
         min_ch_count := z.GetChannelsCount(ckPlanned);
         result := i;
       end;

    end;
  end;


//  если зон с заданным каналом нет - возвращаем любой (запомненный) индекс
  if no_zones then Exit else
  begin
{
  Сначала просматриваем зоны без каналов планирования,
  затем с одним каналом в списке планирования, двумя, тремя и т.д.
}
    max_ch_count := GetMaxChannelsCount(ckPlanned);
    ch_count := 0;
    while ch_count <= max_ch_count do
    begin;
      best_zone_idx := -1;
      result := -1;
      best_zone_coef := 1e+15;
      for i := 0 to Count-1 do
      begin
        sd := GetCoeffOptimal(ch, i);
        z := GetZone(i);
        if (sd > -1) and (sd < best_zone_coef) and (z.GetChannelsCount(ckPlanned) <= ch_count) then
        begin
          best_zone_coef := sd;
          best_zone_idx := i;
        end;
      end;

      if best_zone_idx > -1 then
      begin
        Result := best_zone_idx;
        Exit;
      end;

      ch_count := ch_count + 1
    end;
  end;
//  if Result > -1 then Result := best_zone_idx;
end;



{
  Расчитываем коэфф. оптимальности для заданной зоны и канала
  Таким коэфф. является сумма дистанций (минус мин.дист) до зон с таким же каналом
  Чем эта самма меньше тем лучше
}
function TDVBZoneList.GetCoeffOptimal(ch: TChannel2; idx: integer): double;
begin
  result := GetCoeffOptimal1(ch, idx);
//result := GetCoeffOptimal2(ch, idx);
end;



function TDVBZoneList.GetCoeffOptimal1(ch: TChannel2; idx: integer): double;
var i: integer;
    z2, z1: TDVBZone;
    sd, d: double;
    _min_distance: double;
begin
  sd := 0;
  result := -1;
//                          z1 := GetZone(idx);
{
  Если в этой зоне нельзя использовать данный канал, тогда
  выходим с результатом -1
}
  if not ChannelIsFree(ch, idx) then Exit;


  for i := 0 to Count-1 do
  begin
    if (i <> idx) then
    begin
      z2 := GetZone(i);
      z1 := GetZone(idx);
      _min_distance := Max(z1.MaxDist, z2.MaxDist);
      if z2.ChanelInList(ch, ckPlanned) then
      begin
        d := GetDistance(idx, i);
        if d >= _min_distance then
        begin
          result := 0;
          sd := sd + d - _min_distance;
        end else
        begin
          result := -1;
          Exit;
        end;
      end;
    end;
  end;
  if result > -1  then
  begin
    result := sd;
{
  наш коэфф. оптимальности зависит также и от кол-ва
  каналов планирования. Если каналов в списке уже достаточно много,
  то канал ch лучше отдать другой зоне, у которой каналов мало.
}
//    z1 := GetZone(idx);
//    result := sd + 10000 * z1.GetChannelsCount(ckPlanned);


{
  Если у нас каналов планирования уже дохрена, возвращаем отриц. результат
}
//    if z1.GetChannelsCount(ckPlanned) > 8 then result := -1;
  end;
end;




{
  Другой алгоритм расчета коэффициента оптимальности
  -
}
function TDVBZoneList.GetCoeffOptimal2(ch: TChannel2; idx: integer): double;
var i: integer;
    z2, z1: TDVBZone;
    d, sdmin: double;
    _min_distance: double;
begin
  result := -1;
//                          z1 := GetZone(idx);
{
  Если в этой зоне нельзя использовать данный канал, тогда
  выходим с результатом -1
}
  if not ChannelIsFree(ch, idx) then Exit;
  sdmin := 1000000000;

  for i := 0 to Count-1 do
  begin
    if (i <> idx) then
    begin
      z2 := GetZone(i);
      z1 := GetZone(idx);
      _min_distance := Max(z1.MaxDist, z2.MaxDist);
      if z2.ChanelInList(ch, ckPlanned) then
      begin
        d := GetDistance(idx, i);
        if d >= _min_distance then
        begin
          result := 0;
          if (d - _min_distance) < sdmin then sdmin := d - _min_distance;

        end else
        begin
          result := -1;
          Exit;
        end;
      end;
    end;
  end;
  if result > -1  then
  begin
    result := sdmin;
{
  наш коэфф. оптимальности зависит также и от кол-ва
  каналов планирования. Если каналов в списке уже достаточно много,
  то канал ch лучше отдать другой зоне, у которой каналов мало.
}
//    z1 := GetZone(idx);
//    result := sd + 10000 * z1.GetChannelsCount(ckPlanned);


{
  Если у нас каналов планирования уже дохрена, возвращаем отриц. результат
}
//    if z1.GetChannelsCount(ckPlanned) > 8 then result := -1;
  end;
end;



function TDVBZoneList.GetDistance(z1idx, z2idx: integer): double;
begin
  result := VarArrayGet(_matrix, [z1idx, z2idx]);
end;



function TDVBZoneList.GetMaxChannelsCount(ck: TChannelKind): integer;
var z: TDVBZone;
    i, n: integer;
begin
  n := 0;
  for i := 0 to Count-1 do
  begin
    z := GetZone(i);
    if z.GetChannelsCount(ck) > n then n := z.GetChannelsCount(ck);
  end;
  result := n;
end;



function TDVBZoneList.GetZone(idx: integer): TDVBZone;
begin
  result := TDVBZone(Items[idx]);
end;



procedure TDVBZoneList.LoadFromDA1(filename: string);
var da1: TDA1;
    sl: TStringList;
    l: TList;
    i, idx: integer;
    z: TDVBZone;
    zname: string;
begin
  Self.Clear;
  da1 := TDa1.Create;
  sl := TStringList.Create;
  l := TList.Create;
  da1.LoadFromFile(filename);
  l.Add(Pointer(da1.ContourIdx(fnFirst)));
  while not da1.EoF do l.Add(Pointer(da1.ContourIdx(fnNext)));
  for i := 0 to l.Count-1 do
  begin
    idx := Integer(l.Items[i]);
    if idx >-1 then
    begin
      zname := da1.ContourToSTringList(idx, sl);

      z := TDVBZone.Create;
      z.LoadFromStringList(sl);
      z.Name := zname;
      Self.AddZone(z);
    end;
  end;
  da1.Free;
  sl.Free;
  l.Free;
end;



procedure TDVBZoneList.LoadFromFile(fn: string);
var f: TFileStream;
begin
  f := TFileStream.Create(fn, fmOpenRead);
  LoadFromStream(f);
  f.Free;

end;

procedure TDVBZoneList.LoadFromStream(stream: TStream);
var z: TDVBZone;
    n, i: integer;
begin
  Clear;
  stream.Read(n, SizeOf(n));
  for i := 0 to n-1 do
  begin
    z := TDVBZone.Create;
    z.ReadFromStream(stream);
    AddZone(z);
  end;

end;


procedure TDVBZoneList.SaveToFile(fn: string);
var f: TFileStream;
begin
  f := TFileStream.Create(fn, fmCreate);
  f.Size := 0;
  SaveToStream(f);
  f.Free;
end;

procedure TDVBZoneList.SaveToStream(stream: TStream);
var i, n: integer;
     z: TDVBZone;
begin
  n := Count;
  stream.Write(n, sizeOf(n));
  for i := 0 to Count-1 do
  begin
    z := GetZone(i);
    z.SaveToStream(stream);
  end;
end;

procedure TDVBZoneList.ShowStatistic;
var n,i: integer;
    z: TDVBZone;
begin
  n := 0;
  for i := 0 to Count-1 do
  begin
    z := GetZone(i);
    n := n + z.GetChannelsCount(ckPlanned);
  end;

  ShowMessage('Среднее кол-во каналов в зоне: ' + FormatFloat('0.00', n / Count));
end;


function TDA1.ContourToSTringList(ctridx: integer; sl: TStringList): string;
var s: string;
    point: TZPoint;
begin
  sl.Clear;
  _itemindex := ctridx;
  _eoc := false;
  s := '';
  while not (UpperCase(Copy(s,1,14))='RRC_CONTOUR_ID') do
  begin
    s := Strings[_itemindex];
    s := RemoveWrongSymbols(s);
    _itemindex := _itemindex +1;
  end;

  result := UpperCase(Copy(s,16,8));

  while not _eoc do
  begin
    point := ContourPoint(fnNext);
    if not _eoc then
    begin
      s := FloatToStr(point.lon) + ' ' + FloatToStr(point.lat);
      sl.Add(s);
    end;
  end
end;

function TDA1.ContourIdx(firstnext: TFirstNext): integer;
var i: integer;
    stmp, s: string;
begin
  if firstnext =fnFirst then
  begin
    i := 0;
    _itemindex := 0;
  end else i := _itemIndex;
  result := -1;
  _Eof := false;
  while i < Count do
  begin
    stmp := Strings[i];
    s := RemoveWrongSymbols(stmp);
    stmp := Copy(s, 1, 8);
    if UpperCase(stmp) = '<NOTICE>' then
    begin
      result := i;
      _itemindex := i+1;
      Exit;
    end;
    i := i + 1;
    _itemindex := i;
  end;
  _Eof := true;
end;


function TDA1.ContourPoint(firstnext: TFirstNext): TZPoint;
var lon, lat: double;
    s, scounter, stmp, deg, min, sec: string;
begin
  lon := 0;
  lat := 0;
  while (_itemindex < Count) and (scounter <> '</POINT>') and (s <> '</NOTICE>') do
  begin
    s := Strings[_itemindex];
    s := RemoveWrongSymbols(s);
    stmp := Copy(s, 1, 7);
    if UpperCase(stmp) = '<POINT>' then
    begin
      repeat
        s := Strings[_itemindex];
        s := RemoveWrongSymbols(s);
        stmp := Copy(s, 1, 7);
        if Uppercase(stmp) = 'RRC_LON' then
        begin
          deg := Copy(s, 11, 3);
          min := Copy(s, 14, 2);
          if Length(s) > 16 then sec := Copy(s, 16, 2) else sec := '00';
          lon := StrToInt(deg) + StrToInt(min)/60 + StrToInt(sec)/3600;
          if s[10] = '-' then lon := -lon;
        end;
        if Uppercase(stmp) = 'RRC_LAT' then
        begin
          deg := Copy(s, 10, 2);
          min := Copy(s, 12, 2);
          if Length(s) > 14 then sec := Copy(s, 14, 2) else sec := '00';
          lat := StrToInt(deg) + StrToInt(min)/60 + StrToInt(sec)/3600;
          if s[10] = '-' then lat := -lat;
        end;
        if Length(s) >= 8 then scounter := Uppercase(Copy(s, 1, 8)) else scounter := '';
        _itemindex := _itemindex + 1;
      until (scounter = '</POINT>') or (_itemindex > Count);
    end;
    if scounter <> '</POINT>' then _itemindex := _itemindex + 1;
  end;
  result.lon := lon;
  result.lat := lat;
  if (lon = 0) and (lat=0) then _eoc := true else _eoc := false;
end;

function TDA1.RemoveWrongSymbols(s: string): string;
var n: integer;
begin
  result := '';
  if s <> '' then for n := 1 to Length(s) do if not (s[n] in _WRONG_SYMB) then result := result + s[n];

end;

constructor TDA1.Create;
begin
  inherited Create;
  _eof := false;
  _eoc := false;
  _itemindex := -1;
end;



{
  Определяем тип треугольника - левый или правый.
}
function GetTriangleType(x1, y1, x2, y2, x3, y3: double): double;
var az1, az2, delta_az: double;
begin
  az1 := GetAzimuthDeg(x1, y1, x2, y2);
  az2 := GetAzimuthDeg(x2, y2, x3, y3);

  delta_az := az2 - az1;

  while delta_az > 180 do delta_az := delta_az - 360;
  while delta_az < -180 do delta_az := delta_az + 360;

  Result := delta_az;
end;



{
  Азимут в градусах в диапазоне -180 .. 180
}
function GetAzimuthDeg(x1, y1, x2, y2: double): double;
var x, y: double;
begin
  x := x2 - x1;
  y := y2 - y1;
  if x = 0 then x := 1e-250;
  result := RadToDeg(ArcTan(y/x));
  if x < 0
    then result := result + 90
    else result := result - 90;
end;


{
  Лежит ли точка (xp, yp) внутри трeугольника
}
function PointInTriangle(x1, y1, x2, y2, x3, y3, xp, yp: double): boolean;
var a, b ,c, p1, p2, p3 : double;
    spa, spb, spc, sabc: double;
begin
  if (  ((xp=x1) and (yp=y1)) or ((xp=x2) and (yp=y2))  or ((xp=x3) and (yp=y3))   ) then
  begin
   result := false;
  end else
  begin
    a := LineLength(x1,y1,x2,y2);
    b := LineLength(x2,y2,x3,y3);
    c := LineLength(x1,y1,x3,y3);

    p1 := LineLength(x1,y1,xp,yp);
    p2 := LineLength(x2,y2,xp,yp);
    p3 := LineLength(x3,y3,xp,yp);

    spa := Geron(p1, a, p2);
    spb := Geron(p2, b, p3);
    spc := Geron(p3, c, p1);
    sabc := Geron(a, b, c);

    result := Abs(sabc - spa - spb - spc) < 1e-10;
  end;
end;



function Geron(a, b, c: double): double;
var p: double;
begin
  if (a=0) or (b=0) or (c=0) then result := 0 else
  begin
    p := (a+b+c)/2;
    p := p*(p-a)*(p-b)*(p-c);
    if p >= 0 then result := Sqrt(p) else result := 0;
  end;
end;



function LineLength(x1, y1, x2, y2: double): double;
begin
  result := Sqrt(Sqr(x2-x1) + Sqr(y2-y1));
end;

end.
