<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmDelete.aspx.cs" Inherits="SIMS2017.Modal.ConfirmDelete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Delete Confirmation</title>
    <style type="text/css">
        body {
            background:white;
            font-family: Arial, Helvetica, sans-serif;
            min-height:500px;
            width:100%;
            margin:0;
        }
    </style>
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
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="rsm2" runat="server"></telerik:RadScriptManager>
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Bootstrap" RenderMode="Lightweight" />
    <telerik:RadStyleSheetManager ID="rssm2" runat="server" CdnSettings-TelerikCdn="Enabled" />
    <asp:HiddenField ID="hfStatus" runat="server" />
    <div style="padding:5px;">
        <h4>Confirm Delete Action</h4>
        <asp:Panel ID="pnlFieldTrip" runat="server" Visible="false">
            <p>Are you sure you wish to delete this field trip?</p>
            <asp:Button ID="btnDeleteTrip" runat="server" Text="Yes, delete field trip!" CommandArgument="deletetrip" OnCommand="DeleteInfo" />
            <asp:Button ID="btnCancel1" runat="server" Text="Cancel" OnClientClick="CancelDelete()" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
