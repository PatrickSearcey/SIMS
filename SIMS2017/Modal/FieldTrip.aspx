<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FieldTrip.aspx.cs" Inherits="SIMS2017.Modal.FieldTrip" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Field Trip Sites</title>
    <style type="text/css">
        body {
            background:white;
            font-family: Arial, Helvetica, sans-serif;
            min-height:500px;
            width:100%;
            margin:0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="rsm2" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Bootstrap" RenderMode="Lightweight" />
    <telerik:RadStyleSheetManager ID="rssm2" runat="server" CdnSettings-TelerikCdn="Enabled" />
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function closeWindow() {
            GetRadWindow().close();
        }
    </script>
    <div>
        <h3>Sites Assigned to <asp:Literal ID="ltlTripName" runat="server" /></h3>
        <asp:DataList ID="dlSites" runat="server">
            <ItemTemplate>
                <%# Eval("site_no") %> - <%# Eval("station_full_nm") %>
            </ItemTemplate>
        </asp:DataList>
        <button title="Close" id="close" onclick="closeWindow(); return false;">Close</button>
    </div>
    </form>
</body>
</html>
