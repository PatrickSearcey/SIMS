<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Instructions.aspx.cs" Inherits="RMS.Modal.Instructions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <title>RMS Record Processing Instructions</title>
    <style type="text/css">
        body {
            background:white;
            font-family: Arial, Helvetica, sans-serif;
            min-height:500px;
            width:100%;
            margin:0;
            padding:10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="rsm2" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Bootstrap" RenderMode="Lightweight" />
    <telerik:RadStyleSheetManager ID="rssm2" runat="server" CdnSettings-TelerikCdn="Enabled" />
    <div>
        <h2>Record Processing Instructions</h2>
        <p>Coming soon!</p>
    </div>
    </form>
</body>
</html>
