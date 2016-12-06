<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecordEdit.aspx.cs" Inherits="SIMS2017.Modal.RecordEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Record</title>
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

        function returnToParent() {
            //create the argument that will be returned to the parent page
            var oArg = new Object();

            oArg.type = "record";
            
            //get a reference to the current RadWindow
            var oWnd = GetRadWindow();

            //Close the RadWindow and send the argument to the parent page
            oWnd.close(oArg);
        }
    </script>
    <div>
        <h2>Edit Records</h2>
    </div>
    </form>
</body>
</html>
