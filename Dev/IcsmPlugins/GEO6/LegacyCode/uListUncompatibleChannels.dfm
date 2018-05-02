inherited frmListUncompatibleChannels: TfrmListUncompatibleChannels
  Tag = 41
  Left = 634
  Top = 184
  Caption = #1053#1077#1089#1091#1084#1110#1089#1085#1110' '#1082#1072#1085#1072#1083#1080' '#1082#1072#1073#1077#1083#1100#1085#1086#1075#1086' '#1058#1041
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1053#1077#1089#1091#1084#1110#1089#1085#1110' '#1082#1072#1085#1072#1083#1080' '#1082#1072#1073#1077#1083#1100#1085#1086#1075#1086' '#1058#1041
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from UNCOMPATIBLECHANNELS'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into UNCOMPATIBLECHANNELS'
      
        '  (ID, COCHANNEL, LOWERADJACENT, UPPERADJACENT, HETERODYNEHARMON' +
        'IC1, HETERODYNEHARMONIC2, '
      
        '   HETERODYNEHARMONIC3, LOWERIMAGE1, LOWERIMAGE2, UPPERIMAGE1, U' +
        'PPERIMAGE2)'
      'values'
      
        '  (:ID, :COCHANNEL, :LOWERADJACENT, :UPPERADJACENT, :HETERODYNEH' +
        'ARMONIC1, '
      
        '   :HETERODYNEHARMONIC2, :HETERODYNEHARMONIC3, :LOWERIMAGE1, :LO' +
        'WERIMAGE2, '
      '   :UPPERIMAGE1, :UPPERIMAGE2)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  COCHANNEL,'
      '  LOWERADJACENT,'
      '  UPPERADJACENT,'
      '  HETERODYNEHARMONIC1,'
      '  HETERODYNEHARMONIC2,'
      '  HETERODYNEHARMONIC3,'
      '  LOWERIMAGE1,'
      '  LOWERIMAGE2,'
      '  UPPERIMAGE1,'
      '  UPPERIMAGE2'
      'from UNCOMPATIBLECHANNELS '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, COCHANNEL, LOWERADJACENT, UPPERADJACENT, HETERODYNEHA' +
        'RMONIC1, HETERODYNEHARMONIC2, HETERODYNEHARMONIC3, LOWERIMAGE1, ' +
        'LOWERIMAGE2, UPPERIMAGE1, UPPERIMAGE2 '
      'from UNCOMPATIBLECHANNELS'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update UNCOMPATIBLECHANNELS'
      'set'
      '  COCHANNEL = :COCHANNEL,'
      '  LOWERADJACENT = :LOWERADJACENT,'
      '  UPPERADJACENT = :UPPERADJACENT,'
      '  HETERODYNEHARMONIC1 = :HETERODYNEHARMONIC1,'
      '  HETERODYNEHARMONIC2 = :HETERODYNEHARMONIC2,'
      '  HETERODYNEHARMONIC3 = :HETERODYNEHARMONIC3,'
      '  LOWERIMAGE1 = :LOWERIMAGE1,'
      '  LOWERIMAGE2 = :LOWERIMAGE2,'
      '  UPPERIMAGE1 = :UPPERIMAGE1,'
      '  UPPERIMAGE2 = :UPPERIMAGE2'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'UNCOMPATIBLECHANNELS.ID'
      Required = True
      Visible = False
    end
    object dstListCOCHANNEL: TIBStringField
      DisplayLabel = #1057#1086'-'#1082#1072#1085#1072#1083
      FieldName = 'COCHANNEL'
      Origin = 'UNCOMPATIBLECHANNELS.COCHANNEL'
      Size = 4
    end
    object dstListLOWERADJACENT: TIBStringField
      DisplayLabel = 'n-1'
      FieldName = 'LOWERADJACENT'
      Origin = 'UNCOMPATIBLECHANNELS.LOWERADJACENT'
      Size = 4
    end
    object dstListUPPERADJACENT: TIBStringField
      DisplayLabel = 'n+1'
      FieldName = 'UPPERADJACENT'
      Origin = 'UNCOMPATIBLECHANNELS.UPPERADJACENT'
      Size = 4
    end
    object dstListHETERODYNEHARMONIC1: TIBStringField
      DisplayLabel = '1 '#1075#1072#1088#1084#1086#1085#1110#1082#1072' '
      FieldName = 'HETERODYNEHARMONIC1'
      Origin = 'UNCOMPATIBLECHANNELS.HETERODYNEHARMONIC1'
      Size = 4
    end
    object dstListHETERODYNEHARMONIC2: TIBStringField
      DisplayLabel = '2 '#1075#1072#1088#1084#1086#1085#1110#1082#1072' '
      FieldName = 'HETERODYNEHARMONIC2'
      Origin = 'UNCOMPATIBLECHANNELS.HETERODYNEHARMONIC2'
      Size = 4
    end
    object dstListHETERODYNEHARMONIC3: TIBStringField
      DisplayLabel = '3 '#1075#1072#1088#1084#1086#1085#1110#1082#1072' '
      FieldName = 'HETERODYNEHARMONIC3'
      Origin = 'UNCOMPATIBLECHANNELS.HETERODYNEHARMONIC3'
      Size = 4
    end
    object dstListLOWERIMAGE1: TIBStringField
      DisplayLabel = '1 '#1076#1079#1077#1088#1082#1072#1083#1086' (-)'
      FieldName = 'LOWERIMAGE1'
      Origin = 'UNCOMPATIBLECHANNELS.LOWERIMAGE1'
      Size = 4
    end
    object dstListLOWERIMAGE2: TIBStringField
      DisplayLabel = '2 '#1076#1079#1077#1088#1082#1072#1083#1086' (-)'
      FieldName = 'LOWERIMAGE2'
      Origin = 'UNCOMPATIBLECHANNELS.LOWERIMAGE2'
      Size = 4
    end
    object dstListUPPERIMAGE1: TIBStringField
      DisplayLabel = '1 '#1076#1079#1077#1088#1082#1072#1083#1086' (+)'
      FieldName = 'UPPERIMAGE1'
      Origin = 'UNCOMPATIBLECHANNELS.UPPERIMAGE1'
      Size = 4
    end
    object dstListUPPERIMAGE2: TIBStringField
      DisplayLabel = '2 '#1076#1079#1077#1088#1082#1072#1083#1086' (+)'
      FieldName = 'UPPERIMAGE2'
      Origin = 'UNCOMPATIBLECHANNELS.UPPERIMAGE2'
      Size = 4
    end
  end
end
