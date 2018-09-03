<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContainerFinder.ascx.cs" Inherits="OnlinePortal.ContainerFinder" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1"  %>

<style type="text/css">
    .auto-style1 {
        width: 128px;
    }
</style>

<script src="Scripts/MaskedEditFix.js" type="text/javascript"></script>

<script language="Javascript">
    function EnsureNumeric() {
        var key = window.event.keyCode;
        if (key < 48 || key > 57)
            window.event.returnValue = false;
    };


    function cmtChanged5(mycmt) {
        if (mycmt.value > 5)
        {
            mycmt.value = 5;
            alert("Spindulys negali būti didesnis nei 5 km");
        }
    };

    function cmtChanged50(mycmt) {
        if (mycmt.value > 50) {
            mycmt.value = 50;
            alert("Spindulys negali būti didesnis nei 50 km");
        }
    };

</script> 


<asp:Table ID="TableFinder" runat ="server" Width="100%" ValidateRequestMode="Enabled">
    <asp:TableRow runat ="server" >
            <asp:TableCell runat="server" Width="33%">
                <asp:Panel ID="PanelFinder1" runat="server"  ValidateRequestMode="Enabled" >
                     
                </asp:Panel>
            </asp:TableCell>
           
        </asp:TableRow>
    </asp:Table>
     
<asp:Panel ID="PanelSearch" runat="server" HorizontalAlign="Center" OnInit="PanelSearch_Init" ValidateRequestMode="Enabled">
    <asp:Button ID="ButtonSearch" runat="server" Text="Пошук" Width="112px" OnClick="Button1_Click"  ValidationGroup="FindGroupEx"  />
</asp:Panel>
     
            


