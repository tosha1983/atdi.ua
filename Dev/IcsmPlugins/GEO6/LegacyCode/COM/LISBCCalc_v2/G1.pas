unit g1;

//
//  Ќабор констант, типов, процедур и функций дл€ работы с файлами
//  GTOPO30 (формат данных - G1)
//
//
//


interface
uses SysUtils, Classes, Math;


const
  TILE_WIDTH = 10;        // ширина (размерность по широте) тайла 10 градусов
  TILE_LENGTH = 10;       // длина (размерность по долготе) тайла 10 градусов
  X_COUNT = 1200;         // кол-во элементов в файле по горизонтали
  Y_COUNT = 1200;         // кол-во элементов в файле по вертикали
  G1_SUFFIX = '10D';      // расширение G1 файла (например: W020N60.10D)
  LAT_RESOLUTION = 30;    // разрешение файла данных G1 (шаг сетки) по широте
  LON_RESOLUTION = 30;   // разрешение файла данных G1 (шаг сетки) по долготе
  LAND = 0;               // тип поверхности
  COLD_SEA = 1;           //
  HOT_SEA = 2;            //
  WARM_SEA = 3;           //
  EARTH_RADIUS = 6371.032;    // средний радиус «емли (км)
  EARTH_RADIUS_POLAR = 6356.777;    // пол€рный радиус «емли (км)
  EARTH_RADIUS_EQUATOR = 6378.160;    // радиус «емли по экватору(км)

  LAT_DEGREE_LENGTH = 2 * PI * EARTH_RADIUS_POLAR / 360;
  DEGREE_LENGTH = 2 * PI * EARTH_RADIUS / 360;

type

  TG1Coord = record
    LonD: byte;
    LonM: byte;
    LonS: byte;
    LatD: byte;
    LatM: byte;
    LatS: byte;
    Lon: char;    // E or W
    Lat: char;     // N or S
  end;

  TG1Data = record
    Height: SmallInt;
    Surface: byte;
  end;
  TUA50Data = TG1Data;

  TCoordString = string[7];

procedure FloatToCoordStr(lon, lat: double; var lon_str, lat_str: TCoordString);
procedure CoordToStr(coord: TG1Coord; var lon_str, lat_str: TCoordString);
function StrToCoord (lon_str, lat_str: TCoordString): TG1Coord;
procedure CoordStrToFloat (lon_str, lat_str: TCoordString; var lon, lat: double);
procedure CoordToFloat (coord: TG1Coord; var lon, lat: double);
function FloatToCoord (lon, lat: double): TG1Coord;
function GetTileName(coord: TG1Coord):string;
function GetTileFeatures(coord: TG1Coord; var ro_coord: TG1Coord):string;
function GetTileNameByFloat(lat, lon: double):string;
function GetG1Data(coord: TG1Coord; tile: TStream; ro_coord: TG1Coord):TG1Data;   // ro_coord - коорд. левого нижнего угла тайла
function LonDegreeLength(lat: double): double;
function GetDistance(lon1, lat1, lon2, lat2: double): double;
function GetAzimuth(lon1, lat1, lon2, lat2: double): double;
function GetHeight(coord: TG1Coord; gtopo_path: string): SmallInt;
procedure GetNextCoord(azimuth: double; distance: double; var lon, lat: double);
function ValidateAzimuthDegree(az: double): double;

implementation

procedure GetMemForPointArray(var p: pointer; k: integer);
begin

end;

procedure GetNextCoord(azimuth: double; distance: double; var lon, lat: double);
var delta_lat, delta_lon, lon_lat: double;
begin
////////////////////////////////////////////////
// процедура возвращает координаты точки
// наход€щейс€ на рассто€ниии distnce
// по направлению azimuth от точки,
// заданной координатами (lon, lat)
// результирующие координаты возвращаютс€
// в тех же (lon, lat)...
////////////////////////////////////////////////

  if distance=0 then Exit;

  lat := DegToRad(lat);
  lon := DegToRad(lon);

  lon_lat:=distance / EARTH_RADIUS;                    // дистанци€ в градусах

 // azimuth := azimuth - PI/2;

//  delta_lat:=ArcSin(Sin(lon_lat) * Sin(azimuth));      // широта
//  delta_lon:=ArcTan(Tan(lon_lat) * Cos(azimuth));     // долгота

//  delta_lat:=ArcSin(Sin(lon_lat) * Sin(PI/2 - azimuth));      // широта
//  delta_lon:=ArcTan(Tan(lon_lat) * Cos(PI/2 - azimuth));     // долгота

  delta_lat:=ArcSin(Sin(lon_lat) * Sin(azimuth + PI/2));      // широта
  delta_lon:=ArcTan(Tan(lon_lat) * Cos(azimuth - PI/2));     // долгота
{
  PI/2 добавл€ем и отнимаем потому что у нас отсчет азимута - от севера (см GetAzimuth)
}

//  delta_lon:=delta_lon * Cos (lat + delta_lat / 2);   // поправка дл€ долготы
  delta_lon:=delta_lon / Cos (lat + delta_lat / 2);   // поправка дл€ долготы

  lat:=lat + delta_lat;
  lon:=lon + delta_lon;
  lat := RadToDeg(lat);
  lon := RadToDeg(lon);
end;

function GetHeight(coord: TG1Coord; gtopo_path: string): SmallInt;
var ro_coord: TG1Coord;
    g1_data: TG1Data;
    tile: TFileStream;
    tile_file_name: string;
begin
  tile_file_name:=gtopo_path+GetTileFeatures(coord, ro_coord);
  if FileExists(tile_file_name) then
  begin
    tile:=TFileStream.Create(tile_file_name, fmOpenRead);
    g1_data:=GetG1Data(coord, tile, ro_coord);
    result:=g1_data.Height;
    tile.Free;
  end else result:=0;
end;


{
function GetDistance(lon1, lat1, lon2, lat2: double): double;
var  // x,y: double;
    lat, lon, lon_lat, degree_length: double;
    cos_lon, sin_lon, a, b, c: double;
begin
  lat1:=lat1 * PI / 180;
  lon1:=lon1 * PI / 180;
  lat2:=lat2 * PI / 180;
  lon2:=lon2 * PI / 180;

  lat:=Abs(lat2 - lat1);
  lon:=Abs(lon2 - lon1) * Cos ((lat1+lat2) / 2);
//  lat:=lat * PI / 180;
//  lon:=lon * PI / 180;
  a:=1 / Cos(lat);
  b:=1 / Cos(lon);
  c:=(Sqr(Tan(lat)) + Sqr(Tan(lon)));
  lon_lat:=(Sqr(a) + Sqr(b) - (c));

  lon_lat:=lon_lat / (2 * a * b);

  lon_lat:=ArcCos( lon_lat );

  result:=EARTH_RADIUS * lon_lat;
end;
}

// рассто€ние (км.) между двум€ точками
// на земной пов-ти с коорд. (lon1, lat1) и (lon2, lat2)

function GetDistance(lon1, lat1, lon2, lat2: double): double;
var lat, lon: double;
begin
{
  lat1:=lat1 * PI / 180;
  lon1:=lon1 * PI / 180;
  lat2:=lat2 * PI / 180;
  lon2:=lon2 * PI / 180;
}
  lat1 := DegToRad(lat1);
  lon1 := DegToRad(lon1);
  lat2 := DegToRad(lat2);
  lon2 := DegToRad(lon2);

  lat:=(lat2 - lat1);
  lon:=(lon2 - lon1) * Cos((lat1 + lat2) / 2);

  result:=ArcCos(Cos(lat) * Cos(lon));
  result:=abs(result * EARTH_RADIUS);
end;

{
function GetAzimuth(lon1, lat1, lon2, lat2: double): double;
var lat, lon: double;
begin
  lat1:=lat1 * PI / 180;
  lon1:=lon1 * PI / 180;
  lat2:=lat2 * PI / 180;
  lon2:=lon2 * PI / 180;
  lat:=(lat2 - lat1);
  lon:=(lon2 - lon1) * Cos((lat1 + lat2) / 2);
  if lon=0 then
  begin
    if lat1<=lat2 then result:=PI / 2 else result:=-(PI / 2);
    Exit;
  end;
  result:=ArcTan(Tan(lat) / Sin(lon));
end;
}

//
//  расчитывает азимут по направлению от точки (lon1, lat1)
//  до точки (lon2, lat2) относительно севера в радианах
//                    N
//                  -0 0
//                    |
//         W  -pi/2 --+-- pi/2  E
//                    |
//                -pi   pi
//                    S
//
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


function LonDegreeLength(lat: double): double;
begin
  lat:=Abs(lat);
  result:=(2 * PI * EARTH_RADIUS_EQUATOR / 360) * Cos(lat);
end;

function GetTileNameByFloat(lat, lon: double):string;
begin
  result:='';
end;

function GetTileFeatures(coord: TG1Coord; var ro_coord: TG1Coord):string;
var
  lon_char: char;
  lat_char: char;
  lon_value: string[3];
  lat_value: string[2];
  lon, lat: double;
begin
  result:='';
  lat_char:=coord.Lat;
  lon_char:=coord.Lon;

  lat:=coord.LatD/TILE_WIDTH;                   // округл€ем широту до числа, кратного TILE_WIDTH
  ro_coord.LatD:=Trunc(lat);                    //
  if coord.Lat='S' then ro_coord.LatD:=ro_coord.LatD+1;        //
  ro_coord.LatD:=ro_coord.LatD*TILE_WIDTH;      //

  lon:=coord.LonD/TILE_LENGTH;                // округл€ем долготу до числа, кратного TILE_LENGTH
  ro_coord.LonD:=Trunc(lon);                  //
  if coord.Lon='W' then ro_coord.LonD:=ro_coord.LonD+1;      //
  ro_coord.LonD:=ro_coord.LonD*TILE_LENGTH;   //



  ro_coord.LonM:=0;
  ro_coord.LonS:=0;
  ro_coord.Lon:=coord.Lon;

  ro_coord.LatM:=0;
  ro_coord.LatS:=0;
  ro_coord.Lat:=coord.Lat;

  lat_value:=IntToStr(ro_coord.LatD);
  lon_value:=IntToStr(ro_coord.LonD);
  while Length(lat_value)<2 do lat_value:='0'+lat_value;
  while Length(lon_value)<3 do lon_value:='0'+lon_value;
  result:=lon_char+lon_value+lat_char+lat_value+'.'+G1_SUFFIX;
end;

function GetTileName(coord: TG1Coord):string;
var
  lon_char: char;
  lat_char: char;
  lon_value: string[3];
  lat_value: string[2];
  lon, lat: double;
begin
  result:='';
  lat_char:=coord.Lat;
  lon_char:=coord.Lon;

  lat:=coord.LatD/TILE_WIDTH;           // округл€ем широту до числа, кратного TILE_WIDTH
  coord.LatD:=Trunc(lat);               //
  if coord.Lat='S' then coord.LatD:=coord.LatD+1;        //
  coord.LatD:=coord.LatD*TILE_WIDTH;    //

  lon:=coord.LonD/TILE_LENGTH;        // округл€ем долготу до числа, кратного TILE_LENGTH
  coord.LonD:=Trunc(lon);             //
  if coord.Lon='W' then coord.LonD:=coord.LonD+1;      //
  coord.LonD:=coord.LonD*TILE_LENGTH; //


  lat_value:=IntToStr(coord.LatD);
  lon_value:=IntToStr(coord.LonD);
  while Length(lat_value)<2 do lat_value:='0'+lat_value;
  while Length(lon_value)<3 do lon_value:='0'+lon_value;
  result:=lon_char+lon_value+lat_char+lat_value+'.'+G1_SUFFIX;
end;



function StrToCoord (lon_str, lat_str: TCoordString): TG1Coord;
var s: string[2];
begin
  UpperCase(lon_str);
  UpperCase(lat_str);

  s := Copy(lon_str, 1, 2);
  result.LonD := StrToInt(s);

  s := Copy(lon_str, 4, 2);
  result.LonM := StrToInt(s);
  s := Copy(lon_str, 6, 2);
  result.LonS := StrToInt(s);
  result.Lon := lon_str[3];

  s := Copy(lat_str, 1, 2);
  result.LatD := StrToInt(s);
  s := Copy(lat_str, 4, 2);
  result.LatM := StrToInt(s);
  s := Copy(lat_str, 6, 2);
  result.LatS := StrToInt(s);
  result.Lat := lat_str[3];
end;

procedure FloatToCoordStr(lon, lat: double; var lon_str, lat_str: TCoordString);
var coord: TG1Coord;
begin
  coord := FloatToCoord(lon, lat);
  CoordToStr(coord, lon_str, lat_str);
end;

procedure CoordToStr(coord: TG1Coord; var lon_str, lat_str: TCoordString);
var s: string[2];
begin
 s := IntToStr(coord.LonD);
 while Length(s) < 2 do s := '0' + s;
 lon_str := s;
 lon_str := lon_str + UpperCase(coord.Lon);
 s := IntToStr(coord.LonM);
 while Length(s) < 2 do s := '0' + s;
 lon_str := lon_str + s;
 s := IntToStr(coord.LonS);
 while Length(s) < 2 do s := '0' + s;
 lon_str := lon_str + s;

 s := IntToStr(coord.LatD);
 while Length(s) < 2 do s := '0' + s;
 lat_str := s;
 lat_str := lat_str + UpperCase(coord.Lat);
 s := IntToStr(coord.LatM);
 while Length(s) < 2 do s := '0' + s;
 lat_str := lat_str + s;
 s := IntToStr(coord.LatS);
 while Length(s) < 2 do s := '0' + s;
 lat_str := lat_str + s;
end;

procedure CoordStrToFloat (lon_str, lat_str: TCoordString; var lon, lat: double);
var coord: TG1Coord;
begin
  coord := StrToCoord(lon_str, lat_str);
  CoordToFloat(coord, lon, lat);
end;

procedure CoordToFloat (coord: TG1Coord; var lon, lat: double);
begin
  lon:=coord.LonD+coord.LonM/60+coord.LonS/3600;
  lat:=coord.LatD+coord.LatM/60+coord.LatS/3600;
  if coord.Lon='W' then lon:=-lon;
  if coord.Lat='S' then lat:=-lat;
end;

function GetG1Data(coord: TG1Coord; tile: TStream; ro_coord: TG1Coord):TG1Data;   // ro_coord - коорд. левого нижнего угла тайла
var //lat, lon, ro_lat, ro_lon: double;
    x, y: integer;
    height: SmallInt;
    lat, lon: integer;

begin
  if tile=nil then Exit;
  Lon:=coord.LonD*3600+coord.LonM*60+coord.LonS;
  lat:=coord.LatD*3600+coord.LatM*60+coord.LatS;
  Lon:=Lon-(ro_coord.LonD*3600 + ro_coord.LonM*60 + ro_coord.LonS);
  lat:=lat-(ro_coord.LatD*3600 + ro_coord.LatM*60 + ro_coord.LatS);
  x:=Lon div LON_RESOLUTION;
  y:=lat div LAT_RESOLUTION;
//  x_count:=(TILE_LENGTH*3600) div Lon_RESOLUTION;
//  y_count:=(TILE_WIDTH*3600) div LAT_RESOLUTION;

//  y:=y_count-y-1; // дл€ преобразовани€ пр€моугольных координат внутри файла (прив€зываемс€ к вернему левому углу)

{
  CoordToFloat(coord, lat, Lon);
  CoordToFloat(ro_coord, ro_lat, ro_Lon);

  lat:=lat-ro_lat;
  Lon:=Lon-ro_Lon;

  x:=trunc(Lon/(Lon_RESOLUTION/3600));
  y:=trunc(lat/(LAT_RESOLUTION/3600));
  x_count:=(TILE_LENGTH*3600) div Lon_RESOLUTION;
  y_count:=(TILE_WIDTH*3600) div LAT_RESOLUTION;

  y:=y_count-y-1;
}
  tile.Seek((y*x_count+x-1)*2, soFromBeginning);
  tile.Read(height, SizeOf(height));

  result.Height:=height and $3FFF;
  result.Surface:=(height and $C000) shr 14;


//**************
//   if result.surface=1 then beep;
//**************
end;

function FloatToCoord (lon, lat: double): TG1Coord;
begin
  if lat<0 then result.Lat:='S' else result.Lat:='N';
  if Lon<0 then result.Lon:='W' else result.Lon:='E';

  lat:=Abs(lat);
  Lon:=Abs(Lon);

  result.LonD:=Trunc(Lon);     // ќЅя«ј“≈Ћ№Ќќ  Trunc !!!!
  Lon:=(Lon-result.LonD)*60;   // не Round !!!! ???
  result.LonM:=Trunc(Lon);
  Lon:=(Lon-result.LonM)*60;
  result.LonS:=Trunc(Lon);

  result.LatD:=Trunc(lat);
  lat:=(lat-result.LatD)*60;
  result.LatM:=Trunc(lat);
  lat:=(lat-result.LatM)*60;
  result.LatS:=Trunc(lat);
end;



function ValidateAzimuthDegree(az: double): double;
begin
  result := az;
  while result >= 360 do result := result - 360;
  while result < 0 do result := result + 360;
end;

end.
