<%@ Page Title="Default pages" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ErrorForm.aspx.cs" Inherits="OnlinePortal.ErrorForm" %>



<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <asp:Panel ID="PanelMain" runat="server">
       
            <asp:Table ID="TableError" runat="server" autowidth ="true" HorizontalAlign="Justify" style="text-align: justify">
                <asp:TableRow runat="server">

            <asp:TableCell ID="TableCell1" runat="server" VerticalAlign="Top" Width="240px">
                   
            <asp:Panel ID="Panel2" runat="server" GroupingText="Menu" HorizontalAlign="Left" Width="100%">
            <asp:TreeView ID="TreeViewMenu" runat="server" Width="240px" OnSelectedNodeChanged="TreeViewMenu_SelectedNodeChanged">

        </asp:TreeView>
                            </asp:Panel>


                        
                   </asp:TableCell> 
                    <asp:TableCell runat="server"> <asp:Image ID="Image1" runat="server" Height="148px" ImageUrl="~/Images/Error.jpg" Width="189px" /> </asp:TableCell>
                    <asp:TableCell runat="server"> <h3>An error occurred while trying to connect to the database. Contact your administrator.</h3> </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        
    </asp:Panel>


</asp:Content>