<%@ Page Title="" Language="C#" MasterPageFile="~/Modal.Master" AutoEventWireup="true" CodeBehind="DeleteEmergencyInfo.aspx.cs" Inherits="Safety.DeleteEmergencyInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().BrowserWindow.refreshGrid(args);
            GetRadWindow().close();
        }
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)

            return oWindow;
        }
        function CancelDelete() {
            GetRadWindow().close();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <asp:HiddenField ID="hfStatus" runat="server" />
    <div style="padding:5px;">
        <asp:Panel ID="pnlHospital" runat="server" Visible="false">
            <p>By deleting this hospital, you are removing it as the nearest hospital from all sites' SHAs that have it assigned.</p>
            <asp:Button ID="btnDeleteHospital" runat="server" Text="Yes, delete hospital!" CommandArgument="deletehospital" OnCommand="DeleteInfo" />
            <asp:Button ID="btnCancel1" runat="server" Text="Cancel" OnClientClick="CancelDelete()" />
        </asp:Panel>
        <asp:Panel ID="pnlContact" runat="server" Visible="false">
            <p>By deleting this contact, you are removing it as the emergency contact from all sites' SHAs that have it assigned.</p>
            <asp:Button ID="btnDeleteContact" runat="server" Text="Yes, delete contact!" CommandArgument="deletecontact" OnCommand="DeleteInfo" />
            <asp:Button ID="btnCancel2" runat="server" Text="Cancel" OnClientClick="CancelDelete()" />
        </asp:Panel>
        <asp:Panel ID="pnlHospitalForSite" runat="server" Visible="false">
            <p>Please confirm you wish to remove this hospital from this site's SHA.</p>
            <asp:Button ID="btnDeleteHospitalForSite" runat="server" Text="Yes, delete hospital!" CommandArgument="deletehospitalforsite" OnCommand="DeleteInfo" />
            <asp:Button ID="btnCancel3" runat="server" Text="Cancel" OnClientClick="CancelDelete()" />
        </asp:Panel>
        <asp:Panel ID="pnlContactForSite" runat="server" Visible="false">
            <p>Please confirm you wish to remove this emergency contact from this site's SHA.</p>
            <asp:Button ID="btnDeleteContactForSite" runat="server" Text="Yes, delete contact!" CommandArgument="deletecontactforsite" OnCommand="DeleteInfo" />
            <asp:Button ID="btnCancel4" runat="server" Text="Cancel" OnClientClick="CancelDelete()" />
        </asp:Panel>
    </div>
</asp:Content>
