unit CoordConv;

interface

uses
  SysUtils, Classes, math, windows, dialogs;

type
  TCoordinateConvertor = class(TComponent)
  private
    m_Text: string;
    m_Value: double;
    m_Direction: char;
    E, W, N, S: string;
    axesX, axesY: char;
    IsInit: Boolean;
    isNoDividers: Boolean;
    isSignMandatory: Boolean;
    procedure SetText(_text: string);
	  function GetText(): string;
	  procedure SetValue(val: double);
	  function GetValue(): double;
  	procedure SetDirection(direction: char);
	  function GetDirection(): char;
  	procedure SetE(val: string);
	  procedure SetW(val: string);
  	procedure SetN(val: string);
	  procedure SetS(val: string);
  	procedure SetAxesX(val: char);
	  procedure SetAxesY(val: char);
    function GetE(): string;
	  function GetW(): string;
  	function GetN(): string;
	  function GetS(): string;
  	function GetAxesX(): char;
	  function GetAxesY(): char;
  	procedure corrector(oldVal: string; newVal: string);
    function fabs(val: double): double;
  public
  	function CoordToStr(coord: double; _direction: char): string;
	  function StrToCoord(TextCoord: string): double;
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
  published
    property Text: string read GetText write SetText nodefault;
	  property Value: double read GetValue write SetValue nodefault;
  	property Direction: char read GetDirection write SetDirection nodefault;
	  property EastLongitude: string read GetE write SetE;
  	property WestLongitude: string read GetW write SetW;
	  property NorthLatitude: string read GetN write SetN;
  	property SouthLatitude: string read GetS write SetS;
	  property AxesXName: char read GetAxesX write SetAxesX;
  	property AxesYName: char read GetAxesY write SetAxesY;
    property NoDividers: Boolean read isNoDividers write isNoDividers;
    property SignMandatory: Boolean read isSignMandatory write isSignMandatory;
  end;

procedure Register;
implementation
//---------------------------------------------------------------------------

constructor TCoordinateConvertor.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  if IsInit = False then
  begin
    m_Text := '';
  	m_Value := 0.0;
	  m_Direction := 'X';
  	E := 'E';
	  W := 'W';
  	N := 'N';
	  S := 'S';
  	axesX := 'X';
	  axesY := 'Y';
    isNoDividers := false;
    isSignMandatory := false;

    IsInit := True;
  end;
end;
//---------------------------------------------------------------------------

destructor TCoordinateConvertor.Destroy;
begin
  inherited Destroy;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.CoordToStr(coord: double; _direction: char): string;
{
  CoordToStr - открытый метод
  конвертирует цифру и ось в строку координат
}
var
  grad, min, sec: integer;
	_Result: string;
	modif: string;
  temp1, temp2: string;
begin
		if (UpperCase(_direction) = UpperCase(axesY)) then  //широта(latitude)
    begin
			if (coord > 0) then//если положительная
				modif := N       //север
			else
				modif := S;      //юг
    end
		else if (UpperCase(_direction) = UpperCase(axesX)) then //долгота(longitude)
    begin
			if (coord > 0) then//если положительная
				modif := E       //восток
			else
				modif := W;      //запад
    end
		else//если ось неопределена
		begin
      if m_Direction = axesX then
        modif := W
      else
        modif := S;
			if (coord > 0) then
      begin
        if m_Direction = axesX then
          modif := E
        else
          modif := N;
      end;
    end;
		grad := floor(fabs(coord));
		min :=  floor((fabs(coord) - grad) * 60);
		sec := floor((fabs(coord) - grad - min / 60.0) * 3600 + 0.5);
    while sec > 59 do
    begin
      sec := sec - 60;
      min := min +1;
    end;
    while min > 59 do
    begin
      min := min - 60;
      grad := grad +1;
    end;
    if(min < 10)then//добавляем старшие нули где надо
      temp1 := '0'
    else
      temp1 := '';
    if(sec < 10)then
      temp2 := '0'
    else
      temp2 := '';
    //форматируем строку
    // поддержка Nodividers и SignMandatory
    if Nodividers then
    begin

        _Result := IntToStr(grad)+temp1+IntToStr(min)+temp2+IntToStr(sec);

        while Length(_Result) < 6 do
            _Result := '0'+_Result;

        if (UpperCase(_direction) = UpperCase(axesX)) and (Length(_Result) = 6) then
            _Result := '0'+_Result;

        if coord < 0 then
            _Result := '-'+_Result
        else if SignMandatory then
            _Result := '+'+_Result;

    end else begin

		_Result := IntToStr(grad) + #176 + modif + ' ' +
			temp1 + IntToStr(min) + #39 + ' ' +
			temp2 + IntToStr(sec) + #39#39;

    end;

		m_Text := _Result;
		m_Value := coord;
		m_Direction := _direction;
		Result := _Result;//возвращаем

    exit;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.StrToCoord(TextCoord: string): double;
{
  StrToCoord - открытый метод
  конвертирует строку координат в double
}
var
  i,k: integer;
  len: integer;
  grad, min, sec: integer;
  s_param, s_temp: string;
  s_modif: string;
  str: array[1..3] of string;
  negative: boolean;
  coord: double;
begin
    s_param := Trim(TextCoord);//убираем пробелы
    if Length(s_param)=0 then
      s_param := '0';
    len := Length(s_param);
    negative := false;
    i := 1;
    k := 1;

    //////////////////////////////////////////////////////////
    if s_param[1] = '-' then //признак отрицательного значения
    begin
      negative := true;
      i := 2;
    end;
    //////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////
    ///  вычисляем символ-разделитель
    if Pos(UpperCase(W), UpperCase(s_param)) > 0 then
      s_modif := W
    else if Pos(UpperCase(E), UpperCase(s_param)) > 0 then
      s_modif := E
    else if Pos(UpperCase(S), UpperCase(s_param)) > 0 then
      s_modif := S
    else if Pos(UpperCase(N), UpperCase(s_param)) > 0 then
      s_modif := N
    else
    begin
      if m_Direction = axesX then
        s_modif := E
      else
        s_modif := N;
			if negative then
      begin
        if m_Direction = axesX then
          s_modif := W
        else
          s_modif := S;
      end;
    end;
    if(s_modif = N)or(s_modif = S) then
      m_Direction := axesY
    else
      m_Direction := axesX;
    ///  вычислили
    /////////////////////////////////////////////////////

    //////////////////////////////////////////////////
    while ((i <= len)and(k<4)) do//делим на три группы
    begin
      if ((s_param[i]<'0')or(s_param[i]>'9')) then
      begin
        while i <= len do
        begin
          if ((s_param[i]>='0')and(s_param[i]<='9')) then
            break
          else
            i := i + 1;
        end;
        k := k + 1;
      end;
      if ((s_param[i]>='0')and(s_param[i]<='9')) then
        str[k]:=str[k]+s_param[i];
      i := i + 1;
    end;//поделили
    //////////////////////////////////////////////////

    if (Length(str[1])=0)and(Length(str[2])=0)and(Length(str[3])=0) then
      str[1] := '0';

    ///////////////////////////////////////////////
    //проверяем количество символов в каждой группе
    if Length(str[2])=0 then
    begin//если вторая группа цифр не задана
      s_temp := str[1];
      if (s_modif = E) or (s_modif = W) then
      begin//если долгота (0...180)
        if StrToInt(Copy(s_temp, 1, 3)) <= 180 then
        begin//берем первые три цифры
          str[1] := Copy(s_temp, 1,3);
          str[2] := Copy(s_temp, 4,Length(s_temp)-3);//остаток в str[2]
        end
        else
        begin//берем первые две цифры
          str[1] := Copy(s_temp, 1,2);
          str[2] := Copy(s_temp, 3,Length(s_temp)-2);//остаток в str[2]
        end;
      end
      else
      begin//если широта (0...90)
        if StrToInt(Copy(str[1], 1, 2)) <= 90 then
        begin//берем первые две цифры
          str[1] := Copy(s_temp, 1, 2);
          str[2] := Copy(s_temp, 3, Length(s_temp)-2);//остаток в str[2]
        end
        else
        begin//берем первую цифры
          str[1] := Copy(s_temp, 1, 1);
          str[2] := Copy(s_temp, 2, Length(s_temp)-1);//остаток в str[2]
        end;
      end;
      if Length(str[2])=1 then
        str[2] := str[2]+'0'
      else if Length(str[2])=0 then
        str[2] := '0';
    end;
    if Length(str[3])=0 then
    begin//если последняя группа цифр не задана
      s_temp := str[2];
      if StrToInt(Copy(s_temp, 1, 2)) <= 59 then
      begin
        str[2] := Copy(s_temp, 1, 2);
        str[3] := Copy(s_temp, 3, 2);//остаток в str[3]
      end
      else
      begin
        str[2] := Copy(s_temp, 1, 1);
        str[3] := Copy(s_temp, 2, 2);//остаток в str[3]
      end;
      if Length(str[3])=1 then
        str[3] := str[3]+'0'
      else if Length(str[3]) = 0 then
        str[3] := '0';
    end;
    if Length(str[1]) = 0 then
      str[1] := '0';
    //разгребли
    ///////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    //отсекаем лишние цифры
    if ((StrToInt(str[1])>90)and((s_modif = N)or(s_modif = S))) then
    begin
      if StrToInt(Copy(str[1],1,2)) > 90 then
        str[1] := Copy(str[1],1,1)
      else
        str[1] := Copy(str[1],1,2);
    end
    else if ((StrToInt(str[1])>180)and((s_modif = E)or(s_modif = W))) then
    begin
      if StrToInt(Copy(str[1],1,3)) > 180 then
        str[1] := Copy(str[1],1,2)
      else
        str[1] := Copy(str[1],1,3);
    end;
    if Length(str[2])>2 then
      str[2] := Copy(str[2],1,2);
    if Length(str[3])>2 then
      str[3] := Copy(str[3],1,2);
    ////////////////////////////////////////////////////////////////////////////

    ///////////////////////////
    //формируем цифры
    grad := StrToInt(str[1]);
    min := StrToInt(str[2]);
    sec := StrToInt(str[3]);
    while sec > 59 do
    begin
      sec := sec - 60;
      min := min + 1;
    end;
    while min > 59 do
    begin
      min := min - 60;
      grad := grad + 1;
    end;
    //grad, min и sec сформировали
    ///////////////////////////

    if (((s_modif = W)or(s_modif = S))and(not negative)) then
      negative := true;
    coord := grad + (min + sec/60.0) / 60.0;

    if negative then//если отрицательное значение
      coord := -coord;

    m_Value := coord;
    m_Text := s_param;
    Result := coord;
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetText(_text: string);
begin
  m_Text := _text;
	StrToCoord(_text);
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetText(): string;
begin
  Result := m_Text;
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetValue(val: double);
begin
  m_Value := val;
	CoordToStr(val, m_Direction);
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetValue(): double;
begin
  Result := m_Value;
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetDirection(direction: char);
begin
  if (UpperCase(string(direction)) = UpperCase(string(axesY))) then
		m_Direction := axesY
	else if (UpperCase(string(direction)) = UpperCase(string(axesX))) then
		m_Direction := axesX
	else
		raise Exception.Create('Параметр "Direction" задан некорректно.');
	CoordToStr(m_Value, m_Direction);
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetDirection(): char;
begin
  Result := m_Direction;
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetE(val: string);
var
  _e: string;
begin
	_e := E;
	E := val;
	corrector(_e, E);
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetW(val: string);
var
  _w: string;
begin
	_w := W;
	W := val;
	corrector(_w, W);
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetN(val: string);
var
  _n: string;
begin
	_n := N;
	N := val;
	corrector(_n, N);
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetS(val: string);
var
  _s: string;
begin
	_s := S;
	S := val;
	corrector(_s, S);
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetAxesX(val: char);
begin
	if(val = axesY) then
		raise Exception.Create('Параметр "AxesXName" задан некорректно.');
	if(m_Direction = axesX) then
    m_Direction := val;
	axesX := val;
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.SetAxesY(val: char);
begin
	if(val = axesX) then
		raise Exception.Create('Параметр "AxesYName" задан некорректно.');
	if(m_Direction = axesY) then
    m_Direction := val;
	axesY := val;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetE(): string;
begin
  Result := E;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetW(): string;
begin
  Result := W;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetN(): string;
begin
  Result := N;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetS(): string;
begin
  Result := S;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetAxesX(): char;
begin
  Result := axesX;
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.GetAxesY(): char;
begin
  Result := axesY;
end;
//---------------------------------------------------------------------------

procedure TCoordinateConvertor.corrector(oldVal: string; newVal: string);
var
  i: integer;
begin
    i := Pos(oldVal, m_Text);
    while i > 0 do
    begin
        m_Text := Copy(m_Text, 1, i) + newVal + Copy(m_Text, i + Length(oldVal), Length(m_Text));
        i := Pos(oldVal, m_Text);
    end
end;
//---------------------------------------------------------------------------

function TCoordinateConvertor.fabs(val: double): double;
begin
  Result := val;
  if (val < 0)
    then Result := -val;
end;
//---------------------------------------------------------------------------

procedure Register;
begin
  RegisterComponents('Samples', [TCoordinateConvertor]);
end;

end.
