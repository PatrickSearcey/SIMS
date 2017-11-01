<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FieldTripMap.aspx.cs" Inherits="SIMS2017.FieldTripMap" %>
<%@ Register Assembly="GMaps" Namespace="Subgurim.Controles" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SIMS - Field Trip Maps</title>
    <style type="text/css">
        h3 {
            color: #314d93;
            font-size: 16pt;
            font-weight:bold;
            font-family: Verdana, Arial, Helvetica;
        }
        h4 {
            color: #314d93;
            font-size: 14pt;
            font-weight:bold;
            font-family: Verdana, Arial, Helvetica;
            border-bottom: 1px solid #d4d4d5;
        }
        .SiteCount {
            font-family: Verdana, Arial, Helvetica;
            font-size:12pt;
        }
        .Map
        {
            margin-left: 10px;
            width: 700px;
            height: 700px;
            background-color: #d1ddf0;
            font-family: Verdana, Arial, Helvetica;
            font-size: .90em;
            color: #000000;
            border: 1px solid #314d93;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h3>Field Trip Maps</h3>
        <h4><asp:Label ID="lblTripName" runat="server" /></h4>
        <asp:Label ID="lblResultsCount" runat="server" CssClass="SiteCount" />
        <asp:Panel id="pnlMap" runat="server" CssClass="Map">
            <cc1:gmap id="GMap" runat="server" Width="100%" Height="100%" enableServerEvents="False"></cc1:gmap>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
