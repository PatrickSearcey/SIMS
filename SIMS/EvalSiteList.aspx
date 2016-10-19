<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EvalSiteList.aspx.vb" Inherits="SIMS.EvalSiteList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>National Evaluation Maps Site List</title>
    <link rel="stylesheet" type="text/css" href="css/styles.css" />
    <link rel="stylesheet" type="text/css" href="css/common.css" />
    <link rel="stylesheet" type="text/css" href="css/custom.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" border="0" width="700px">
            <tr>
                <td>
                <asp:Repeater ID="rptSiteList" runat="server">
                    <ItemTemplate>
                        <p style="font-weight:bold;padding:0px;margin:0px;">
                        <%#DataBinder.Eval(Container.DataItem, "site_no")%> &nbsp;&nbsp;&nbsp;
                        <%#DataBinder.Eval(Container.DataItem, "station_nm")%>
                        </p>
                    </ItemTemplate>
                </asp:Repeater>
                </td>
            </tr> 
        </table>
    </div>
    </form>
</body>
</html>
