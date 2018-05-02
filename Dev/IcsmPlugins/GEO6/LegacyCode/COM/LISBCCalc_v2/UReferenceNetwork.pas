unit UReferenceNetwork;


interface
uses LISBCTxServer_TLB, G1, RSAGeography_TLB, Math, Dialogs, SysUtils,
     ComObj, Classes;

type
  TReferenceNetwork = class(TObject)
  private
     _spherics: IRSASpherics;
    _txlist: ILISBCTxList;
    _rn: TBcRn;
    _rpc: TBcRpc;
    _service_area_diameter: integer;
    _tx_distance: integer;
    _tx_number: integer;
    _tx_antenna_heff: integer;
    _erp_dbw: double;
    _erp_central_dbw: double;
    _directional_power_reduction: double;
    _freq: double;
    _center_lon: double;
    _center_lat: double;
    procedure LoadRnParam(rn: TBcRn; rpc: TBcRpc; freq: double);
    function CreateDefaultTx(id: integer): ILISBCTx;
    procedure GetNextCoordDeg(azimuth, distance: double; var lon, lat: double);
  public
    constructor Create(rn: TBcRn; rpc: TBcRpc; freq: double);
    procedure LocateRN(lon, lat, az_deg: double);
    procedure LocateRNBorderPoint(bp_lon, bp_lat, az_deg: double);
    property TxNumber: integer read _tx_number;
    property TxList: ILISBCTxList read _txlist;
    procedure Free;
    
  end;


implementation
uses ULISBCCalcCOM, Types;




constructor TReferenceNetwork.Create(rn: TBcRn; rpc: TBcRpc; freq: double);
var i, idx, n: integer;
    tx: ILISBCTx;
    txcount: integer;

begin
  _spherics := nil;
  _freq := freq;
  _txlist := CoLisBcTxList.Create;
  _txlist.Clear;
  LoadRnParam(rn, rpc, freq);
  if (rn = rn1) or (rn = rn5) or (rn = rn6) then _tx_number := 7 else _tx_number := 3;
  for i := 1 to _tx_number do
  begin
    tx := CreateDefaultTx(-i); // надо устанавливать отрицательный ИД чтобы с предатчиокм можно было работать
                               // иначе не получится устанавливать координаты и проч.

    OleCheck(_txlist.AddTx(tx, idx));
  end;
  LocateRN(30, 50, 0);
//  inherited Create;
end;





{
  создает заготовку передатчика.
  после этого надо вызвать Locate(..)
}
function TReferenceNetwork.CreateDefaultTx(id: integer): ILISBCTx;
var tx: ILISBCTx;
    i: integer;
    erp: double;
begin
    tx := CoLISBCTx.Create;
    tx.init(0, id);  // надо устанавливать отрицательный ИД чтобы с предатчиокм можно было работать
                     // иначе не получится устанавливать координаты и проч.

    tx.Set_direction(drND);
    tx.Set_longitude(30);
    tx.Set_latitude(50);
    tx.Set_video_carrier(_freq);
    tx.Set_sound_carrier_primary(_freq);
    tx.Set_sound_carrier_second(_freq);
    tx.Set_blockcentrefreq(_freq);
    tx.Set_height_eft_max(_tx_antenna_heff);
    tx.Set_heightantenna(_tx_antenna_heff);

//    if freq >= 470 then erp := _erp_bandIVV_dbw-30 else erp := _erp_bandIII_dbw-30;


    tx.Set_epr_video_max(_erp_dbw);
    tx.Set_epr_video_hor(_erp_dbw);
    tx.Set_epr_video_vert(_erp_dbw);
    tx.Set_epr_sound_max_primary(_erp_dbw);
    tx.Set_epr_sound_hor_primary(_erp_dbw);
    tx.Set_epr_sound_vert_primary(_erp_dbw);

    erp := _erp_dbw - 30;

    for i := 0 to 35 do
    begin
      tx.Set_effectpowerhor(i, erp);
      tx.Set_effectpowervert(i, erp);
      tx.Set_effectheight(i, _tx_antenna_heff);
    end;

    if (_rn = rn4) or (_rn = rn5) or (_rn = rn6) then tx.Set_direction(drD) else tx.Set_direction(drND);

    result := tx;
end;







{
  Инициализируем основные параметры эталонной сети
  (кол-во перед., высоты антенн, мощности, расстояния и т.п.)
}
procedure TReferenceNetwork.LoadRnParam(rn: TBcRn; rpc: TBcRpc; freq: double);
var isUHF: boolean;
begin
  _rn := rn;
  _rpc := rpc;
  isUHF := (freq >= 470);

// RN1 TABLE A.3.6-1
  if rn = rn1 then
  begin
    _tx_number := 7;
    _tx_antenna_heff := 150;
    case rpc of
      rpc1: begin
              _tx_distance := 70;
              _service_area_diameter := 161;
              if isUHF then _erp_dbw := 42.8 else _erp_dbw := 34.1;
            end;
      rpc2: begin
              _tx_distance := 50;
              _service_area_diameter := 115;
              if isUHF then _erp_dbw := 49.7 else _erp_dbw := 36.2;
            end;
      rpc3: begin
              _tx_distance := 40;
              _service_area_diameter := 92;
              if isUHF then _erp_dbw := 52.4 else _erp_dbw := 40.0;
            end;
    end;
    _erp_central_dbw := _erp_dbw;
    _directional_power_reduction := 0;
  end;

// RN2 TABLE A.3.6-2
  if rn = rn2 then
  begin
    _tx_number := 3;
    _tx_antenna_heff := 150;
    case rpc of
      rpc1: begin
              _tx_distance := 40;
              _service_area_diameter := 53;
              if isUHF then _erp_dbw := 31.8 else _erp_dbw := 24.1;
            end;
      rpc2: begin
              _tx_distance := 25;
              _service_area_diameter := 33;
              if isUHF then _erp_dbw := 39.0 else _erp_dbw := 26.6;
            end;
      rpc3: begin
              _tx_distance := 25;
              _service_area_diameter := 33;
              if isUHF then _erp_dbw := 46.3 else _erp_dbw := 34.1;
            end;
    end;
    _erp_central_dbw := _erp_dbw;
    _directional_power_reduction := 0;
  end;

// RN3 TABLE A.3.6-3
  if rn = rn3 then
  begin
    _tx_number := 3;
    _tx_antenna_heff := 150;
    case rpc of
      rpc1: begin
              _tx_distance := 40;
              _service_area_diameter := 53;
              if isUHF then _erp_dbw := 31.8 else _erp_dbw := 24.1;
            end;
      rpc2: begin
              _tx_distance := 25;
              _service_area_diameter := 33;
              if isUHF then _erp_dbw := 44.9 else _erp_dbw := 32.5;
            end;
      rpc3: begin
              _tx_distance := 25;
              _service_area_diameter := 33;
              if isUHF then _erp_dbw := 52.2 else _erp_dbw := 40.1;
            end;
    end;
    _erp_central_dbw := _erp_dbw;
    _directional_power_reduction := 0;
  end;

// RN4 TABLE A.3.6-4
  if rn = rn4 then
  begin
    _tx_number := 3;
    _tx_antenna_heff := 150;
    case rpc of
      rpc1: begin
              _tx_distance := 40;
              _service_area_diameter := 46;
              if isUHF then _erp_dbw := 29.4 else _erp_dbw := 22.0;
            end;
      rpc2: begin
              _tx_distance := 25;
              _service_area_diameter := 29;
              if isUHF then _erp_dbw := 37.2 else _erp_dbw := 24.0;
            end;
      rpc3: begin
              _tx_distance := 25;
              _service_area_diameter := 29;
              if isUHF then _erp_dbw := 44.8 else _erp_dbw := 32.5;
            end;
    end;
    _erp_central_dbw := _erp_dbw;
    _directional_power_reduction := 6;
  end;

// RN5 TABLE A.3.6-5
  if (rn = rn5) or (rn = rn6) then
  begin
    _tx_number := 7;
    _tx_antenna_heff := 150;
    _tx_distance := 60;
    _service_area_diameter := 120;
    case rpc of
      rpc4: _erp_dbw := 30.0;
      rpc5: _erp_dbw := 39.0;
    end;
    _erp_central_dbw := _erp_dbw - 10;
    _directional_power_reduction := 12;
  end;

//  корректируем мощность

  if _rpc = rpc1 then
  begin
    if _freq >= 470
      then _erp_dbw := _erp_dbw + 20 * log10(_freq/650)
      else _erp_dbw := _erp_dbw + 20 * log10(_freq/200)
  end else
  begin
    if _freq >= 470
      then _erp_dbw := _erp_dbw + 30 * log10(_freq/650)
      else _erp_dbw := _erp_dbw + 30 * log10(_freq/200)
  end;

end;




procedure TReferenceNetwork.GetNextCoordDeg(azimuth, distance: double; var lon, lat: double);
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



{
  Распологает сеть в заданной позиции устанавливая
  центр сети в заданных координатах и поворачивая сеть
  в заданном направлении. Для полузакрытых сетей
  пересчитываются направление излучения
}
procedure TReferenceNetwork.LocateRN(lon, lat, az_deg: double);
var i: integer;
    tx: ILISBCTx;
    newlon, newlat: double;
    az: double;
    r: double;
    reduction_az1: double;
    reduction_az2: double;
    idx1, idx2, idx, idx_az: integer;
    angle: double;
    txcount: integer;
    s, filename: string;
//    l: TStringList;
    start_idx: integer;
begin
  _center_lon := lon;
  _center_lat := lat;
  _txlist.Get_Size(txcount);

// Сети из 7-х передатчиков
  if (_rn = rn1) or (_rn = rn5)  or (_rn = rn6)then
  begin
    _txlist.Get_Tx(0, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
    tx.Set_longitude(_center_lon);
    tx.Set_longitude(_center_lat);
    s := 'Point ' + FloatToStr(_center_lon) + ' ' + FloatToStr(_center_lat) + #13#10;
    s := s + '    Symbol (35,0,12)' + #13#10;
    tx := nil;
    for i := 1 to 6 do
    begin
      _txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
      az := ValidateAzimuthDegree(az_deg + 30 + 60*(i-1));
      newlon := _center_lon;
      newlat := _center_lat;
      GetNextCoordDeg(az, _tx_distance, newlon, newlat);
      tx.Set_longitude(newlon);
      tx.Set_latitude(newlat);
      s := s + 'Point ' + FloatToStr(newlon) + ' ' + FloatToStr(newlat) + #13#10;
      s := s + '    Symbol (35,0,12)' + #13#10;
      tx := nil;
    end;
  end;


// Сети из 3-х передатчиков
  if (_rn = rn2) or (_rn = rn3) or (_rn = rn4) then
  begin
    for i := 0 to 2 do
    begin
      _txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
      az := ValidateAzimuthDegree(az_deg + 30 + 120*(i-1));
      newlon := _center_lon;
      newlat := _center_lat;
      r := (_tx_distance/2)/cos(DegToRad(30)); // расстояние от центра до передатчиков
      GetNextCoordDeg(az, r, newlon, newlat);
      tx.Set_longitude(newlon);
      tx.Set_latitude(newlat);
      s := s + 'Point ' + FloatToStr(newlon) + ' ' + FloatToStr(newlat) + #13#10;
      s := s + '    Symbol (35,0,12)' + #13#10;
      tx := nil;
    end;
  end;


//  ShowMessage(s);
{  l := TStringList.Create;
  filename := 'd:\looooooooooooooooooooog.log';
  if FileExists(filename) then l.LoadFromFile(filename);
  l.Add(s);
  l.SaveToFile(filename);
  l.Free;
}


// корректируем коеффициенты усиления для полузакрытых сетей
  if (_rn = rn4) or (_rn = rn5) or (_rn = rn6) then
  begin
    if (_rn=rn5) or (_rn=rn6) then
    begin
      _txlist.Get_Tx(0, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
      for idx := 0 to 35 do
      begin
        idx_az := idx;
        tx.Set_effectpowerhor(idx_az, _erp_central_dbw-30);
        tx.Set_effectpowervert(idx_az, _erp_central_dbw-30);
      end;
      tx := nil;
    end;

    if _rn=rn4 then start_idx := 0 else start_idx := 1;
    for i := start_idx to _tx_number-1 do
    begin
      _txlist.Get_Tx(i, tx); tx._AddRef; // delphi interface decrements reference when goes off the scope
      if _rn = rn4 then angle := 120;
      if _rn = rn5 then angle := 60;
      if _rn = rn6 then angle := 60;
      reduction_az1 := (az_deg - 90) + angle * (i-1);
      reduction_az2 := (az_deg + 150) + angle * (i-1);
      idx1 := Round(reduction_az1 / 10) * 10;
      idx2 := Round(reduction_az2 / 10) * 10;
      for idx := 0 to 35 do
      begin
        idx_az := idx;
        tx.Set_effectpowerhor(idx_az, _erp_dbw-30);
        tx.Set_effectpowervert(idx_az, _erp_dbw-30);
      end;
      for idx := idx1 to idx2 do
      begin
        idx_az := Round(idx/10);
        while idx_az >= 36 do idx_az := idx_az - 36;
        while idx_az < 0 do idx_az := idx_az + 36;
        tx.Set_effectpowerhor(idx_az, _erp_dbw - 30 - _directional_power_reduction);
        tx.Set_effectpowervert(idx_az, _erp_dbw - 30 - _directional_power_reduction);
      end;
      tx := nil;
    end;
  end;
end;



{
  Распологает сеть в заданной позиции относительно
  точки на границе зоны выделения в направлении контрольной точки
  (см. Appendix 3 to Section I Position and orientation of the reference network for allotment)
}
procedure TReferenceNetwork.LocateRNBorderPoint(bp_lon, bp_lat, az_deg: double);
var center_lon, center_lat, az_to_center: double;
    r: double;
begin
  az_to_center := ValidateAzimuthDegree(az_deg - 180);
  r := (_service_area_diameter/2) * cos(DegToRad(30));
//  r := 50;
  center_lon := bp_lon;
  center_lat := bp_lat;
  GetNextCoordDeg(az_to_center, r, center_lon, center_lat);
  LocateRN(center_lon, center_lat, az_deg);
end;

procedure TReferenceNetwork.Free;
begin
  _tx_number := 0;
  _txlist := nil; // here _Release is called
  inherited Free;
end;

end.
