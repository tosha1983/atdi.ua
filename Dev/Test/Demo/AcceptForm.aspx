<%@ Page Title="Default pages" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="AcceptForm.aspx.cs" Inherits="OnlinePortal.AcceptForm" %>



<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="<%: ResolveUrl("~/Scripts/JavaScriptCustom.js") %>"></script>
</asp:Content>

 


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <asp:Panel ID="PanelMain" runat="server" Width="100%" BorderWidth="1" HorizontalAlign="Center">
       
       
            <asp:Table ID="TableAccept" runat="server" autowidth ="true" HorizontalAlign="Center" style="text-align: center" Width="100%" Height="190px">
                <asp:TableRow runat="server" HorizontalAlign="Center" Width="50%">
                    <asp:TableCell runat="server"> <asp:Image ID="Image1" runat="server" Height="100px"  ImageUrl="~/Images/Error.jpg" Width="130px" ImageAlign="Right" /> </asp:TableCell>
                    <asp:TableCell runat="server"> <h2> Dear user! <p> </h2> <h3> If you want to access the Frequency Spectrum Management Portal, you should click on the "Agree" button.<p> In this case, your data will be transferred with RVIS site to Frequency Spectrum Management Portal. </h3> </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server" HorizontalAlign="Center" Width="20%">
                    <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Center"> 
                      </asp:TableCell>
                        <asp:TableCell ID="TableCell2" runat="server" HorizontalAlign="Center"> 
                            <asp:Button ID="Accept" runat="server" OnClick="Accept_Click" OnClientClick="ConfirmAgree()" Text="Agree" Width="20%" />
                        </asp:TableCell>
                        
                    </asp:TableRow>
            </asp:Table>
    
    </asp:Panel>


</asp:Content>