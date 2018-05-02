inherited frmUserActivityLog: TfrmUserActivityLog
  Tag = 46
  Caption = #1046#1091#1088#1085#1072#1083' '#1072#1082#1090#1080#1074#1085#1086#1089#1090#1110' '#1082#1086#1088#1080#1089#1090#1091#1074#1072#1095#1110#1074
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1046#1091#1088#1085#1072#1083' '#1072#1082#1090#1080#1074#1085#1086#1089#1090#1110' '#1082#1086#1088#1080#1089#1090#1091#1074#1072#1095#1110#1074
  end
  inherited dstList: TIBDataSet
    SelectSQL.Strings = (
      
        'select av.ID, av.DATECHANGE, ad.LOGIN, av.NAME_TABLE, av.NAME_FI' +
        'ELD, av.NUM_CHANGE, av.TYPECHANGE  '
      'from ACTIVEVIEW av'
      'left join ADMIT ad on (av.ADMIT_ID = ad.ID) '
      'order by av.DATECHANGE')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'ACTIVEVIEW.ID'
      Required = True
      Visible = False
    end
    object dstListDATECHANGE: TDateTimeField
      DisplayLabel = #1044#1072#1090#1072
      FieldName = 'DATECHANGE'
      Origin = 'ACTIVEVIEW.DATECHANGE'
    end
    object dstListLOGIN: TIBStringField
      DisplayLabel = #1050#1086#1088#1080#1089#1090#1091#1074#1072#1095
      FieldName = 'LOGIN'
      Origin = 'ADMIT.LOGIN'
      Size = 16
    end
    object dstListTYPECHANGE: TIBStringField
      DisplayLabel = #1058#1080#1087' '#1079#1084#1110#1085#1080
      FieldName = 'TYPECHANGE'
      Origin = 'ACTIVEVIEW.TYPECHANGE'
      Size = 16
    end
    object dstListNAME_TABLE: TIBStringField
      DisplayLabel = #1058#1072#1073#1083#1080#1094#1103
      FieldName = 'NAME_TABLE'
      Origin = 'ACTIVEVIEW.NAME_TABLE'
      Size = 32
    end
    object dstListNAME_FIELD: TIBStringField
      DisplayLabel = #1055#1086#1083#1077
      DisplayWidth = 32
      FieldName = 'NAME_FIELD'
      Origin = 'ACTIVEVIEW.NAME_FIELD'
      Size = 256
    end
    object dstListNUM_CHANGE: TIntegerField
      DisplayLabel = #1047#1072#1087#1080#1089
      FieldName = 'NUM_CHANGE'
      Origin = 'ACTIVEVIEW.NUM_CHANGE'
    end
  end
end
